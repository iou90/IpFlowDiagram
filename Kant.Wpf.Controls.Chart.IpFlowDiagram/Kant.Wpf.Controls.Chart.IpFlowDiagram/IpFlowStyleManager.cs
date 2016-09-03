using Kant.Wpf.Toolkit;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            defaultNodesPalette = GetNodeLinksPalette(opacity);
            diagram.LinkFill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9ceaff")) { Opacity = 0.15 };
            diagram.LinkStroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9ceaff")) { Opacity = 0.75 };
            diagram.IpSegmentColumnWidth = 25.0;
            diagram.MaxDisplayIpCount = 10;
            diagram.LinkCurvature = 0.55;
            diagram.PortSplitLineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#bbbbbb"));
            diagram.IpSegmentNodeBorderBrush = Brushes.Transparent;
            diagram.HighlightOpacity = 1.0;
            diagram.LoweredOpacity = 0.25;
        }

        public void UpdateLabelAdjustY()
        {
            if (diagram.NodeLabelStyle != null)
            {
               LabelAdjustedY = MeasureHepler.MeasureString("5", diagram.NodeLabelStyle, CultureInfo.CurrentCulture).Height;
            }
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

        public void HighlightingNode(IpFlowIpSegmentFinder segmentFinder, IReadOnlyList<IpFlowNode> nodes)
        {
            if (segmentFinder == null && diagram.HighlightNode == null || nodes == null || nodes.Count == 0)
            {
                return;
            }

            // reset each element's brush first
            RecoverHighlights(nodes, false);

            if (segmentFinder == null || !nodes.ToList().Exists(node => node.SourceIpNode.IsSegmentExist(segmentFinder) || node.DestinationIpNode.IsSegmentExist(segmentFinder)))
            {
                return;
            }

            // reset highlight if highlighting the same node twice
            if ((from node in nodes where node.HighlightSegment != null && node.HighlightSegment.Equals(diagram.HighlightNode) && node.IsHighlight select node).Count() > 0 && segmentFinder.Equals(diagram.HighlightNode))
            {
                RecoverHighlights(nodes);
                diagram.SetCurrentValue(IpFlowDiagram.HighlightNodeProperty, null);

                return;
            }

            var highlightLinkFillOpacity = diagram.LinkFill.Opacity / diagram.LinkStroke.Opacity * diagram.HighlightOpacity;

            // ensure minimize same element once && not minmize hightlight element
            var minimizedSegments = new HashSet<IpFlowIpSegment>();
            var highlightSegments = new HashSet<IpFlowIpSegment>();
            var minimizedLinks = new HashSet<IpFlowLinkBase>();
            var highlightLinks = new HashSet<IpFlowLinkBase>();

            foreach (var node in nodes)
            {
                #region highlight

                var highlight = segmentFinder.IsSource ? node.SourceIpNode.IsSegmentExist(segmentFinder) : node.DestinationIpNode.IsSegmentExist(segmentFinder);

                if (highlight)
                {
                    for (var index = 0; index < 4; index++)
                    {
                        var srcNode = node.SourceIpNode.GetSegment(index);
                        var destNode = node.DestinationIpNode.GetSegment(index);
                        srcNode.Color.Opacity = diagram.HighlightOpacity;
                        destNode.Color.Opacity = diagram.HighlightOpacity;
                        highlightSegments.Add(srcNode);
                        highlightSegments.Add(destNode);
                    }

                    foreach (var link in node.Links)
                    {
                        link.SourceIpToPortLink.Shape.Fill.Opacity = highlightLinkFillOpacity;
                        link.DestinationIpToPortLink.Shape.Fill.Opacity = highlightLinkFillOpacity;
                        link.SourceToDestinationPortLink.Shape.Fill.Opacity = highlightLinkFillOpacity;
                        link.SourceIpToPortLink.Shape.Stroke.Opacity = diagram.HighlightOpacity;
                        link.DestinationIpToPortLink.Shape.Stroke.Opacity = diagram.HighlightOpacity;
                        link.SourceToDestinationPortLink.Shape.Stroke.Opacity = diagram.HighlightOpacity;
                        highlightLinks.Add(link.SourceIpToPortLink);
                        highlightLinks.Add(link.DestinationIpToPortLink);
                        highlightLinks.Add(link.SourceToDestinationPortLink);
                    }

                    node.IsHighlight = true;
                    node.HighlightSegment = segmentFinder;

                    #endregion
                }
                else
                {
                    #region minimize

                    var l = node.Links[0];
                    var minimizeFillOpacity = l.SourceIpToPortLink.Shape.Fill.Opacity - diagram.LoweredOpacity < 0 ? 0 : l.SourceIpToPortLink.Shape.Fill.Opacity - diagram.LoweredOpacity;
                    var minimizeStrokeOpacity = l.SourceIpToPortLink.Shape.Stroke.Opacity - diagram.LoweredOpacity < 0 ? 0 : l.SourceIpToPortLink.Shape.Stroke.Opacity - diagram.LoweredOpacity;

                    foreach (var link in node.Links)
                    {
                        if (!minimizedLinks.Contains(link.SourceIpToPortLink) && !highlightLinks.Contains(link.SourceIpToPortLink))
                        {
                            link.SourceIpToPortLink.Shape.Fill.Opacity = minimizeFillOpacity;
                            link.SourceIpToPortLink.Shape.Stroke.Opacity = minimizeStrokeOpacity;
                            minimizedLinks.Add(link.SourceIpToPortLink);
                        }

                        if (!minimizedLinks.Contains(link.DestinationIpToPortLink) && !highlightLinks.Contains(link.DestinationIpToPortLink))
                        {
                            link.DestinationIpToPortLink.Shape.Fill.Opacity = minimizeFillOpacity;
                            link.DestinationIpToPortLink.Shape.Stroke.Opacity = minimizeStrokeOpacity;
                            minimizedLinks.Add(link.DestinationIpToPortLink);
                        }

                        if (!minimizedLinks.Contains(link.SourceToDestinationPortLink) && !highlightLinks.Contains(link.SourceToDestinationPortLink))
                        {
                            link.SourceToDestinationPortLink.Shape.Fill.Opacity = minimizeFillOpacity;
                            link.SourceToDestinationPortLink.Shape.Stroke.Opacity = minimizeStrokeOpacity;
                            minimizedLinks.Add(link.SourceToDestinationPortLink);
                        }
                    }

                    for (var index = 0; index < 4; index++)
                    {
                        var srcNode = node.SourceIpNode.GetSegment(index);
                        var destNode = node.DestinationIpNode.GetSegment(index);

                        if(!minimizedSegments.Contains(srcNode) && !highlightSegments.Contains(srcNode))
                        {
                            var minimizeSrcOpacity = srcNode.Color.Opacity - diagram.LoweredOpacity < 0 ? 0 : srcNode.Color.Opacity - diagram.LoweredOpacity;
                            srcNode.Color.Opacity = minimizeSrcOpacity;
                            minimizedSegments.Add(srcNode);
                        }

                        if(!minimizedSegments.Contains(destNode) && !highlightSegments.Contains(destNode))
                        {
                            var minimizeDestOpacity = destNode.Color.Opacity - diagram.LoweredOpacity < 0 ? 0 : destNode.Color.Opacity - diagram.LoweredOpacity;
                            destNode.Color.Opacity = minimizeDestOpacity;
                            minimizedSegments.Add(destNode);
                        }
                    }

                    node.IsHighlight = false;
                    node.HighlightSegment = null;

                    #endregion
                }
            }
        }

        private void RecoverHighlights(IReadOnlyList<IpFlowNode> nodes, bool resetHighlightStatus = true)
        {
            foreach (var node in nodes)
            {
                for(var index = 0; index < 4; index++)
                {
                    var srcNode = node.SourceIpNode.GetSegment(index);
                    srcNode.Color = srcNode.OriginalBrush.CloneCurrentValue();
                    var destNode = node.DestinationIpNode.GetSegment(index);
                    destNode.Color = destNode.OriginalBrush.CloneCurrentValue();
                }

                foreach(var link in node.Links)
                {
                    link.SourceIpToPortLink.Shape.Fill = link.OriginalLinkFillBrush.CloneCurrentValue();
                    link.DestinationIpToPortLink.Shape.Fill = link.OriginalLinkFillBrush.CloneCurrentValue();
                    link.SourceToDestinationPortLink.Shape.Fill = link.OriginalLinkFillBrush.CloneCurrentValue();
                    link.SourceIpToPortLink.Shape.Stroke = link.OriginalLinkStrokeBrush.CloneCurrentValue();
                    link.DestinationIpToPortLink.Shape.Stroke = link.OriginalLinkStrokeBrush.CloneCurrentValue();
                    link.SourceToDestinationPortLink.Shape.Stroke = link.OriginalLinkStrokeBrush.CloneCurrentValue();
                }

                if (resetHighlightStatus)
                {
                    node.IsHighlight = false;
                    node.HighlightSegment = null;
                }
            }
        }

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

        /// <summary>
        /// update while update label style 
        /// </summary>
        public double LabelAdjustedY { get; set; }

        private IpFlowDiagram diagram;

        private List<Brush> defaultNodesPalette;

        #endregion
    }
}
