using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Kant.Wpf.Controls.Chart
{
    public class IpFlowPortLabel
    {
        /// <summary>
        /// port
        /// </summary>
        public int Value { get; set; }

        public TextBlock Label { get; set; }

        public double Y { get; set; }
    }
}
