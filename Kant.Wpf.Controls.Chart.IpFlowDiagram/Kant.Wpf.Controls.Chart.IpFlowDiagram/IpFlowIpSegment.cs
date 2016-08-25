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
    public class IpFlowIpSegment
    {
        public IpFlowIpSegment()
        {
        }

        public double Height { get; set; }

        public Brush Color { get; set; }

        public string Segment { get; set; }

        public string IpAddress { get; set; }

        public Brush OriginalBrush { get; set; }
    }
}
