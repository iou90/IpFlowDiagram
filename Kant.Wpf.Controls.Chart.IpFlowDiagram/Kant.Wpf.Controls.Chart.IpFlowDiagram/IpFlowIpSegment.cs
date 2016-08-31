using Kant.Wpf.Toolkit.Mvvm;
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
    public class IpFlowIpSegment : ObservableObject
    {
        public IpFlowIpSegment()
        {
        }

        public double Height { get; set; }

        public IpFlowIpSegmentFinder SegmentFinder { get; set; }

        private Brush color;
        public Brush Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                RaisePropertyChanged(() => Color);
            }
        }

        public string Segment { get; set; }

        public Brush OriginalBrush { get; set; }
    }
}
