using System;
using System.Configuration;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Interop;
using System.Reflection;
using Autodesk.AutoCAD.ApplicationServices.Core;
using System.Collections.Generic;

namespace AutocadConnector
{
    public class Connector
    {
        AcadApplication _acAppComObj;
        string _strProgId;
        string _customDllPath;
        bool _isAssemblyLoaded;

        public Connector()
        {
            this._acAppComObj = null;
            this._strProgId = "";
            this._customDllPath = "";
            this._isAssemblyLoaded = false;
        }

        // Read configs
        private void ReadConfig()
        {
            try
            {
                this._strProgId = (ConfigurationSettings.AppSettings["ProgramId"] == null) ? "AutoCAD.Application" : ConfigurationSettings.AppSettings["ProgramId"];               // autocad application
                this._customDllPath = (ConfigurationSettings.AppSettings["CustomDllPath"] == null) ? "" : ConfigurationSettings.AppSettings["CustomDllPath"];                      // add your dll path here
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetCustomDllPath(string path)
        {
            this._customDllPath = path;
        }

        private void GetRunningAutoCADInstance()
        {
            _acAppComObj = (AcadApplication)Marshal.GetActiveObject(_strProgId);
        }

        private void StartNewAutoCADInstance()
        {
            _acAppComObj = (AcadApplication)Activator.CreateInstance(Type.GetTypeFromProgID(_strProgId), true);
        }

        public AcadDocument GetActiveDocument()
        {
            return !(_acAppComObj == null) ? _acAppComObj.ActiveDocument : null;
        }

        public void LoadCustomAssemblyIntoAutoCAD(AcadDocument doc)
        {
            try
            {
                if (!this._isAssemblyLoaded)
                {
                    doc.SendCommand("(command " + (char)34 + "NETLOAD" + (char)34 + " " + (char)34 + _customDllPath + (char)34 + ") ");
                    this._isAssemblyLoaded = true;
                }
            }
            catch (Exception ex)
            {
                this._isAssemblyLoaded = false;
                throw ex;
            }
        }

        public void SendCommand(AcadDocument doc, string command)
        {
            try
            {
                doc.SendCommand(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ConnectToAutoCAD(out string info)
        {
            bool isStarted = false;
            info = "";

            try
            {
                ReadConfig();

                // Get a running instance of AutoCAD
                GetRunningAutoCADInstance();
                isStarted = true;
            }
            catch // An error occurs if no instance is running
            {
                try
                {
                    // Create a new instance of AutoCAD
                    StartNewAutoCADInstance();
                    isStarted = true;
                }
                catch (Exception exc)
                {
                    // If an instance of AutoCAD is not created then message and return
                    info = "Instance of 'AutoCAD.Application'" + " could not be created." + "\n" + exc.Message;
                    isStarted = false;
                    throw exc;
                }
            }

            if (isStarted == true)
            {
                // Display the application and return the name and version
                _acAppComObj.Visible = true;
                info = "Now running " + _acAppComObj.Name + " version " + _acAppComObj.Version;
            }
            return isStarted;

            // Get the active document
            //GetActiveDocument();

            // Optionally, load your assembly and start your command or if your assembly
            // is demandloaded, simply start the command of your in-process assembly.
        }

        public AcadApplication GetAcadApplication()
        {
            return _acAppComObj;
        }

        public void CloseApplication()
        {
            try
            {
                foreach (AcadDocument doc in _acAppComObj.Documents)
                {
                    doc.Close();
                }
                _acAppComObj = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _acAppComObj = null;
            }
        }
       
    }
}
