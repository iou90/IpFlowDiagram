using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Kant.Wpf.Controls.Chart
{
    public abstract class IpFlowLinkBase
    {
        public Path Shape { get; set; }

        public int Count { get; set; }

        public bool PositionInPortIsSetted { get; set; }
    }
}
