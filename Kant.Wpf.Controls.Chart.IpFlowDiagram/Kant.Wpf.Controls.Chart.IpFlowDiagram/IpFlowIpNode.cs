using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kant.Wpf.Controls.Chart
{
    public class IpFlowIpNode
    {
        public IpFlowIpNode()
        {
            ipSegments = new IpFlowIpSegmentNode[4];
        }

        public IpFlowIpSegmentNode GetSegment(int index)
        {
            return ipSegments[index];
        }

        public void SetSegment(IpFlowIpSegmentNode segment, int index)
        {
            if(ipSegments[index] != null)
            {
                return;
            }

            ipSegments[index] = segment;
        }

        // ip address
        public string IpAddress { get; set; }

        private IpFlowIpSegmentNode[] ipSegments;
    }
}
