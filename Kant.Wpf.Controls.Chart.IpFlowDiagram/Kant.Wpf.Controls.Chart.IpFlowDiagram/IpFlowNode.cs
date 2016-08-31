using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kant.Wpf.Controls.Chart
{
    public class IpFlowNode
    {
        public IpFlowNode()
        {
        }

        public bool IsHighlight { get; set; }

        public IpFlowIpSegmentFinder HighlightSegment { get; set; }

        public IpFlowIpNode SourceIpNode { get; set; }

        public IpFlowIpNode DestinationIpNode { get; set; }

        public List<IpFlowLink> Links { get; set; }
    }
}
