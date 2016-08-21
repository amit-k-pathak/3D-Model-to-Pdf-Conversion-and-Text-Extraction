using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;

namespace AccoreconsoleLauncher
{
    public class LayoutScriptOptions : ScriptOptions
    {
        public string _plotPaperSpaceFirst;
        public string _hidePaperSpaceObjects;

        public LayoutScriptOptions()
        {

        }

        public void SetOptions(string layoutName)
        {
            base.modelOrLayoutName = layoutName;
            base.detailedPlotConfig = "Yes";
            base.plotUpsideDown = "No";
            base.plotWithStyles = "Yes";
            base.plotStyleTable = ".";
            base.plotWithLinewieghts = "Yes";
            base.outputDevice = "DWG To PDF.pc3";
            base.paperSize = "ANSI expand A (8.50 x 11.00 Inches)";
            base.paperUnits = "Inches";
            base.drawingOrientation = "Landscape";
            base.plotArea = "Layout";
            base.plotScale = "Fit";
            base.plotOffset = "";
            base.enterFileName = "";
            base.saveChanges = "N";
            base.proceedWithplot = "Y";
            this._plotPaperSpaceFirst = "No";
            this._hidePaperSpaceObjects = "No";
        }
    }
}
