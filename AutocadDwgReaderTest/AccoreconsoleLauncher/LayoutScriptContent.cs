using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccoreconsoleLauncher
{
    internal class LayoutScriptContent : ScriptContent
    {
        public LayoutScriptContent()
        {

        }

        public virtual string PrepareScript(LayoutScriptOptions opts)
        {
            string content = "";

            //Command:
            content += GetCommand();

            //Detailed plot configuration? [Yes/No] <No>:
            content += opts.detailedPlotConfig;

            //Enter a layout name or [?] <Model>:
            content += "\"" + opts.modelOrLayoutName + "\"";

            //Enter an output device name or [?] <None>:
            content += opts.outputDevice;

            //Enter paper size or [?] <ANSI A (11.00 x 8.50 Inches)>:
            content += opts.paperSize;

            //Enter paper units [Inches/Millimeters] <Inches>:
            content += opts.paperUnits;

            //Enter drawing orientation [Portrait/Landscape] <Portrait>:
            content += opts.drawingOrientation;

            //Plot upside down? [Yes/No] <No>:
            content += opts.plotUpsideDown;

            //Enter plot area [Display/Extents/Limits/View/Window/layout] <Display>:
            content += opts.plotArea;

            //Enter plot scale (Plotted Inches=Drawing Units) or [Fit] <Fit>:
            content += opts.plotScale;

            //Enter plot offset (x,y)<0.00,0.00>:
            content += opts.plotOffset;

            //Plot with plot styles? [Yes/No] <Yes>:
            content += opts.plotWithStyles;

            //Enter plot style table name or [?] (enter . for none) <>:
            content += opts.plotStyleTable;

            //Plot with lineweights? [Yes/No] <Yes>:
            content += opts.plotWithLinewieghts;

            //Plot paper space first?[Yes/No] <No>:
            content += opts._plotPaperSpaceFirst;

            //Hide paper space objects[Yes/No] <No>:
            content += opts._plotPaperSpaceFirst;

            //Enter file name <C:\Work\solids-Model.pdf>:
            content += opts.enterFileName;

            //Save changes to page setup? Or set shade plot quality? [Yes/No/Quality] <N>:
            content += opts.saveChanges;

            //Proceed with plot [Yes/No] <Y>:
            content += opts.proceedWithplot;

            return content;
        }
    }
}
