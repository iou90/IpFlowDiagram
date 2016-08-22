using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Kant.Wpf.Controls.Chart
{
    public class IpFlowIpNode : Element
    {
        public double Height { get; set; }

        public double Y { get; set; }

        public Brush Color { get; set; }

        /// <summary>
        /// ip segment
        /// </summary>
        public string Value { get; set; }

        #region for highlight

        public IpFlowIpNode Segment1 { get; set; }

        public IpFlowIpNode Segment2 { get; set; }

        public IpFlowIpNode Segment3 { get; set; }

        public IpFlowIpNode Segment4 { get; set; }

        public List<IpFlowLink> Links { get; set; }

        #endregion
    }
}
