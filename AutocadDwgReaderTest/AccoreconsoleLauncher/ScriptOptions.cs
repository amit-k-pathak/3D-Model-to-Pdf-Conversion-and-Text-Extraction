using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;

namespace AccoreconsoleLauncher
{
    public class ScriptOptions : IScriptOptions
    {
        public string drawingOrientation
        {
            get
            {
                return drawingOrientation;
            }
            set
            {
                this.drawingOrientation = value;
            }
        }

        public string outputDevice
        {
            get
            {
                return outputDevice;
            }
            set
            {
                this.outputDevice = value;
            }
        }

        public string paperSize
        {
            get
            {
                return paperSize;
            }
            set
            {
                this.paperSize = value;
            }
        }

        public string paperUnits
        {
            get
            {
                return paperUnits;
            }
            set
            {
                this.paperUnits = value;
            }
        }

        public string plotArea
        {
            get
            {
                return plotArea;
            }
            set
            {
                this.plotArea = value;
            }
        }

        public string plotOffset
        {
            get
            {
                return plotOffset;
            }
            set
            {
                this.plotOffset = value;
            }
        }

        public string plotScale
        {
            get
            {
                return plotScale;
            }
            set
            {
                this.plotScale = value;
            }
        }

        public string detailedPlotConfig
        {
            get
            {
                return detailedPlotConfig;
            }
            set
            {
                this.detailedPlotConfig = value;
            }
        }

        public string enterFileName
        {
            get
            {
                return enterFileName;
            }
            set
            {
                this.enterFileName = value;
            }
        }

        public string plotStyleTable
        {
            get
            {
                return plotStyleTable;
            }
            set
            {
                this.plotStyleTable = value;
            }
        }

        public string plotUpsideDown
        {
            get
            {
                return plotUpsideDown;
            }
            set
            {
                this.plotUpsideDown = value;
            }
        }

        public string plotWithLinewieghts
        {
            get
            {
                return plotWithLinewieghts;
            }
            set
            {
                this.plotWithLinewieghts = value;
            }
        }

        public string plotWithStyles
        {
            get
            {
                return plotWithStyles;
            }
            set
            {
                this.plotWithStyles = value;
            }
        }

        public string proceedWithplot
        {
            get
            {
                return proceedWithplot;
            }
            set
            {
                this.proceedWithplot = value;
            }
        }

        public string saveChanges
        {
            get
            {
                return saveChanges;
            }
            set
            {
                this.saveChanges = value;
            }
        }


        public string modelOrLayoutName
        {
            get
            {
                return modelOrLayoutName;
            }
            set
            {
                this.modelOrLayoutName = value;
            }
        }
    }
}
