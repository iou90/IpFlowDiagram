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

        //public ControlTemplate SegmentNodeToolTipTemplate { get; set; }

        private ControlTemplate segmentNodeToolTipTemplate;
        public ControlTemplate SegmentNodeToolTipTemplate
        {
            get
            {
                return segmentNodeToolTipTemplate;
            }
            set
            {
                segmentNodeToolTipTemplate = value;
                RaisePropertyChanged(() => SegmentNodeToolTipTemplate);
            }
        }

        public string Segment { get; set; }

        /// <summary>
        /// if segment is "55" and the index is 3, then part ip should be xxx.xxx.55
        /// </summary>
        public string PartIp { get; set; }

        /// <summary>
        /// count of flow from this segment node 
        /// </summary>
        public int Count { get; set; }

        public Brush OriginalBrush { get; set; }
    }
}
