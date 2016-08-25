using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Kant.Wpf.Controls.Chart
{
    public class IpFlowLink
    {
        public IpFlowLink()
        {
        }

        public IpFlowIpToPortLink SrcIpToPortLink { get; set; }

        public IpFlowIpToPortLink DestPortToIpLink { get; set; }

        public IpFlowPortLink SrcToDestPortLink { get; set; }

        public Brush OriginalLinkFillBrush { get; set; }

        public Brush OriginalLinkStrokeBrush { get; set; }
    }
}
