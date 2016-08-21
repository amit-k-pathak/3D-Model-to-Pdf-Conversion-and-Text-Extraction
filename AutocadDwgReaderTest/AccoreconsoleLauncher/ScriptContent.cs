using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;

namespace AccoreconsoleLauncher
{
    internal class ScriptContent
    {
        private string _command;  
        private ApplicationType _appType;
        private DWGPageType _pageType;

        public ScriptContent()
        {

        }

        public ScriptContent(ApplicationType type)
        {
            this._appType = type;
        }

        public void SetCommand(string cmd)
        {
            if (!string.IsNullOrWhiteSpace(cmd))
                this._command = cmd;    
            else
            {
                this._command = (this._appType == ApplicationType.DWGTrueView) ? "_PLOT" : "-PLOT"; //default is plot command
            }
        }

        public string GetCommand()
        {
            return this._command;
        }

        public virtual string PrepareScript(ScriptOptions opts)
        {
            return "";
        }
    }

    public enum ApplicationType
    {
        DWGTrueView,
        AutoCAD2016
    }

    public enum DWGPageType
    {
        Model,
        Layout
    }

    public enum OperationType
    {
        PlotPdf,
        ExtratAllText
    }
}
