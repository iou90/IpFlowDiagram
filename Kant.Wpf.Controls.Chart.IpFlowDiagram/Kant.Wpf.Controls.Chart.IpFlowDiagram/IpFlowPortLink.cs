﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kant.Wpf.Controls.Chart
{
    public class IpFlowPortLink : IpFlowLinkBase
    {
        public IpFlowPortLink()
        {
        }

        public int SourcePort { get; set; }

        public int DestinationPort { get; set; }

        public double PositionInSourcePorts { get; set; }

        public double PositionInDestinationPorts { get; set; }
    }
}
