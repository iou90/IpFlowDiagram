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

        public IpFlowIpToPortLink SourceIpToPortLink { get; set; }

        public IpFlowIpToPortLink DestinationIpToPortLink { get; set; }

        public IpFlowPortLink SourceToDestinationPortLink { get; set; }

        public Brush OriginalLinkFillBrush { get; set; }

        public Brush OriginalLinkStrokeBrush { get; set; }
    }
}
