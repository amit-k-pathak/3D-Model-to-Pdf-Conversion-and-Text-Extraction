using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;

namespace AccoreconsoleLauncher
{
    internal class ModelScriptOptions : ScriptOptions
    {
        public string _shadePlotSettings;

        public ModelScriptOptions()
        {

        }

        public void SetOptions(string modelName)
        {
            base.modelOrLayoutName = modelName;
            base.detailedPlotConfig = "Yes";
            base.plotUpsideDown = "No";
            base.plotWithStyles = "Yes";
            base.plotStyleTable = ".";
            base.plotWithLinewieghts = "Yes";
            base.outputDevice = "DWG To PDF.pc3";
            base.paperSize = "ANSI expand A (8.50 x 11.00 Inches)";
            base.paperUnits = "Inches";
            base.drawingOrientation = "Landscape";
            base.plotArea = "Extents";
            base.plotScale = "Fit";
            base.plotOffset = "Center";
            base.enterFileName = "";
            base.saveChanges = "N";
            base.proceedWithplot = "Y";
            this._shadePlotSettings = "As displayed";
        }
    }
}
