using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Kant.Wpf.Controls.Chart
{
    public class IpFlowData
    {
        public Brush Color { get; set; }

        public string SourceIp { get; set; }

        public string DestinationIp { get; set; }

        public int SourcePort { get; set; }

        public int DestinationPort { get; set; }
    }
}
