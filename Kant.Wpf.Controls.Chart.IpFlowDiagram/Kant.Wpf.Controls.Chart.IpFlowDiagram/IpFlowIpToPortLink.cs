using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Kant.Wpf.Controls.Chart
{
    public class IpFlowIpToPortLink : IpFlowLinkBase
    {
        public IpFlowIpToPortLink()
        {
        }

        public IpFlowIpSegmentNode Node { get; set; }

        public string Segment { get; set; }

        public int Port { get; set; }

        public double Width { get; set; }

        public double PositionInNode { get; set; }

        public double PositionInPorts { get; set; }
    }
}
