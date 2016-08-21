using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Types
{
    public interface IScriptOptions
    {
        string outputDevice { get; set; }
        string paperSize { get; set; }
        string paperUnits { get; set; }
        string drawingOrientation { get; set; }
        string plotArea { get; set; }
        string plotScale { get; set; }
        string plotOffset { get; set; }
        string detailedPlotConfig { get; set; }
        string plotUpsideDown { get; set; }
        string plotWithStyles { get; set; }
        string plotStyleTable { get; set; }
        string plotWithLinewieghts { get; set; }
        string enterFileName { get; set; }
        string saveChanges { get; set; }
        string proceedWithplot { get; set; }
        string modelOrLayoutName { get; set; }
    }
}
