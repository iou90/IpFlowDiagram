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
            ipSegments = new IpFlowIpSegment[4];
        }

        public bool IsSegmentExist(IpFlowIpSegmentFinder segmentFinder)
        {
            foreach (var s in ipSegments)
            {
                if(s.SegmentFinder.Equals(segmentFinder))
                {
                    return true;
                }
            }

            return false;
        }

        public IpFlowIpSegment GetSegment(int index)
        {
            return ipSegments[index];
        }

        public void SetSegment(IpFlowIpSegment segment, int index)
        {
            if(ipSegments[index] != null)
            {
                return;
            }

            ipSegments[index] = segment;
        }

        // ip address
        public string IpAddress { get; set; }

        private IpFlowIpSegment[] ipSegments;
    }
}
