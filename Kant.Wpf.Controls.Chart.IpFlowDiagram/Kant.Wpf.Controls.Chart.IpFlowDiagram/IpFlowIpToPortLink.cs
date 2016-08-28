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

        public IpFlowIpSegment4 Node { get; set; }

        public string Segment { get; set; }

        public int Port { get; set; }

        public double PositionInNode { get; set; }

        public double PositionInPorts { get; set; }

        public bool PositionInPortIsSetted { get; set; }
    }
}
