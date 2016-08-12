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
    public class IpFlowNode : Element
    {
        public double Height { get; set; }

        public Brush Color { get; set; }

        public string Value { get; set; }
    }
}
