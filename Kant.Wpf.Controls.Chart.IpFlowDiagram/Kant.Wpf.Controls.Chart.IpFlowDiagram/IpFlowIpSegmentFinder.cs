using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kant.Wpf.Controls.Chart
{
    public class IpFlowIpSegmentFinder
    {
        public IpFlowIpSegmentFinder()
        {
        }

        public bool Equals(IpFlowIpSegmentFinder segmentFinder)
        {
            if(segmentFinder == null)
            {
                return false;
            }

            if(Index == segmentFinder.Index && IsSource == segmentFinder.IsSource && Segment == segmentFinder.Segment)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int Index { get; set; }

        /// <summary>
        /// false means destination, 
        /// </summary>
        public bool IsSource { get; set; }

        public string Segment { get; set; }
    }
}
