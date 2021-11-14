using Inventor;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using jordanrobot.IPlugin;
using System.Collections.Generic;

namespace IPluginDemo
{
    /// <summary>
    /// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the AddIn is via the methods on this interface.
    /// </summary>
    [ProgId("IPluginDemo.StandardAddinServer")]
    [GuidAttribute("7F39964A-C0DC-4BC7-948E-4B0A060256D1")]
    public class StandardAddInServer : Inventor.ApplicationAddInServer
    {

        // Inventor application object.
        private Inventor.Application m_inventorApplication;

        [ImportMany(typeof(IPlugin))]
        List<IPlugin> plugins = new List<IPlugin> { };

        public StandardAddInServer()
        {
            ComposePlugins();
        }

        private void ComposePlugins()
        {
            //Wire up MEF parts
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            path = System.IO.Path.GetDirectoryName(path);
            path = System.IO.Path.Combine(path, "plugins");
            var directoryCatalog = new DirectoryCatalog(path);
            var assemblyCatalog = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());

            var catalog = new AggregateCatalog(directoryCatalog, assemblyCatalog);

            CompositionContainer container = new CompositionContainer(catalog);
            container.SatisfyImportsOnce(this);
        }

        #region ApplicationAddInServer Members

        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            // This method is called by Inventor when it loads the addin.
            // The AddInSiteObject provides access to the Inventor Application object.
            // The FirstTime flag indicates if the addin is loaded for the first time.

            // Initialize AddIn members.
            m_inventorApplication = addInSiteObject.Application;

            foreach (var item in plugins)
            {
                item.Activate(m_inventorApplication, "7F39964A-C0DC-4BC7-948E-4B0A060256D1");
            }
            // TODO: Add ApplicationAddInServer.Activate implementation.
            // e.g. event initialization, command creation etc.
        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            // TODO: Add ApplicationAddInServer.Deactivate implementation
            foreach (var item in plugins)
            {
                item.Deactivate();
            }
            // Release objects.
            m_inventorApplication = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the
            // ControlDefinition functionality for implementing commands.
        }

        public object Automation
        {
            // This property is provided to allow the AddIn to expose an API
            // of its own to other programs. Typically, this  would be done by
            // implementing the AddIn's API interface in a class and returning
            // that class object through this property.

            get
            {
                // TODO: Add ApplicationAddInServer.Automation getter implementation
                return null;
            }
        }

        #endregion

    }
}
