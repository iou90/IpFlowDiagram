using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Kant.Wpf.Controls.Chart
{
    public class IpFlowStyleManager
    {
        #region Constructor

        public IpFlowStyleManager(IpFlowDiagram diagram)
        {
            this.diagram = diagram;
            SetDefaultStyles();
        }

        #endregion

        #region Methods

        public void SetDefaultStyles()
        {
            var opacity = 0.55;
            diagram.IpSegmentColumnWidth = 25.0;
            diagram.MaxDisplayIpCount = 10;
            diagram.GraphElementBorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#bbbbbb"));
            //diagram.HighlightOpacity = 1.0;
            //diagram.LoweredOpacity = 0.25;
            defaultNodesPalette = GetNodeLinksPalette(opacity);
        }

        public Brush SetNodeBrush()
        {
            var brush = defaultNodesPalette[DefaultNodesPaletteIndex].CloneCurrentValue();
            DefaultNodesPaletteIndex++;

            if (DefaultNodesPaletteIndex >= defaultNodesPalette.Count)
            {
                DefaultNodesPaletteIndex = 0;
            }

            return brush;
        }

        public void ClearHighlight()
        {
            diagram.SetCurrentValue(IpFlowDiagram.HighlightNodeProperty, null);
        }

        public void HighlightingNode()
        {
        }

        private void RecoverHighlights()
        {
            //foreach ()
            //{
            
            //}
        }

        //private void RecoverHighlights()
        //{
        //}

        private List<Brush> GetNodeLinksPalette(double opacity)
        {
            return new List<Brush>()
            {
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0095fb")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff0000")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffa200")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00f2c8")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7373ff")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#91bc61")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dc89d9")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fff100")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#44c5f1")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#85e91f")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00b192")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1cbe65")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#278bcc")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#954ab3")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f3bc00")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e47403")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ce3e29")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d8dddf")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#60e8a4")) { Opacity = opacity },
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffb5ff")) { Opacity = opacity }
            };
        }

        #endregion

        #region Fields & Properties

        public int DefaultNodesPaletteIndex { get; set; }

        private IpFlowDiagram diagram;

        private List<Brush> defaultNodesPalette;

        #endregion
    }
}
