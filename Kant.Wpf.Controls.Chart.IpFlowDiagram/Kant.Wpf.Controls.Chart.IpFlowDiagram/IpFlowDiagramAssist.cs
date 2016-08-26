using Kant.Wpf.Toolkit;
using Kant.Wpf.Toolkit.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Kant.Wpf.Controls.Chart
{
    public class IpFlowDiagramAssist : ViewModelBase
    {
        #region Constructor

        public IpFlowDiagramAssist(IpFlowDiagram diagram, IpFlowStyleManager manager)
        {
            this.diagram = diagram;
            styleManager = manager;
        }

        #endregion

        #region Methods

        public void DiagramSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!diagram.IsLoaded)
            {
                return;
            }

            ClearDiagramCanvasChilds();
            DrawDiagram();
        }

        public void UpdateDiagram(IEnumerable<IpFlowData> datas)
        {
            // clear diagram first
            ClearDiagram();

            if (datas == null || datas.Count() == 0)
            {
                return;
            }

            // filter datas
            filteredDatas = FilterDatas(datas);

            // drawing...
            if (diagram.IsLoaded)
            {
                DrawDiagram();
            }
        }

        public void SetIpNodesSource()
        {
            SrcIpSegment1Container.SetBinding(ItemsControl.ItemsSourceProperty, BindingHelper.ConfigureBinding("SrcIpSegment1Nodes", this));
            SrcIpSegment2Container.SetBinding(ItemsControl.ItemsSourceProperty, BindingHelper.ConfigureBinding("SrcIpSegment2Nodes", this));
            SrcIpSegment3Container.SetBinding(ItemsControl.ItemsSourceProperty, BindingHelper.ConfigureBinding("SrcIpSegment3Nodes", this));
            SrcIpSegment4Container.SetBinding(ItemsControl.ItemsSourceProperty, BindingHelper.ConfigureBinding("SrcIpSegment4Nodes", this));
            DestIpSegment1Container.SetBinding(ItemsControl.ItemsSourceProperty, BindingHelper.ConfigureBinding("DestIpSegment1Nodes", this));
            DestIpSegment2Container.SetBinding(ItemsControl.ItemsSourceProperty, BindingHelper.ConfigureBinding("DestIpSegment2Nodes", this));
            DestIpSegment3Container.SetBinding(ItemsControl.ItemsSourceProperty, BindingHelper.ConfigureBinding("DestIpSegment3Nodes", this));
            DestIpSegment4Container.SetBinding(ItemsControl.ItemsSourceProperty, BindingHelper.ConfigureBinding("DestIpSegment4Nodes", this));
        }

        #region clear something

        public void ClearDiagram()
        {
            ClearDatas(nodes);
            ClearDatas(srcIpSegment1Nodes);
            ClearDatas(srcIpSegment2Nodes);
            ClearDatas(srcIpSegment3Nodes);
            ClearDatas(srcIpSegment4Nodes);
            ClearDatas(destIpSegment1Nodes);
            ClearDatas(destIpSegment2Nodes);
            ClearDatas(destIpSegment3Nodes);
            ClearDatas(destIpSegment4Nodes);
            ClearDatas(srcPortLabels);
            ClearDatas(destPortLabels);
            ClearDiagramCanvasChilds();
        }

        public void ClearDiagramCanvasChilds()
        {
            if (SrcIpToPortContainer != null && SrcIpToPortContainer.Children != null)
            {
                SrcIpToPortContainer.Children.Clear();
            }

            if (DestIpToPortContainer != null && DestIpToPortContainer.Children != null)
            {
                DestIpToPortContainer.Children.Clear();
            }

            if (SrcToDestPortContainer != null && SrcToDestPortContainer.Children != null)
            {
                SrcToDestPortContainer.Children.Clear();
            }
        }

        private void ClearDatas<T>(List<T> datas)
        {
            if(datas != null)
            {
                datas.Clear();
            }
        }

        #endregion

        public void DrawDiagram()
        {
            if (filteredDatas == null || filteredDatas.Count == 0)
            {
                return;
            }

            if (DiagramGrid.ActualHeight <= 0 || (DiagramGrid.ActualWidth - 5 - diagram.IpSegmentColumnWidth * 8) <= 0)
            {
                return;
            }

            if(SrcIpToPortContainer.ActualWidth <= 0 || SrcToDestPortContainer.ActualWidth <= 0 || DestIpToPortContainer.ActualWidth <=0)
            {
                return;
            }

            if (diagram.IpToPortLinkCurvature < 0 || diagram.IpToPortLinkCurvature > 1)
            {
                throw new ArgumentOutOfRangeException("curvature should be ranged from 0 to 1");
            }

            #region preparing...

            var srcIp1Dic = new Dictionary<string, int>();
            var srcIp2Dic = new Dictionary<string, int>();
            var srcIp3Dic = new Dictionary<string, int>();
            var srcIp4Dic = new Dictionary<string, int>();
            var destIp1Dic = new Dictionary<string, int>();
            var destIp2Dic = new Dictionary<string, int>();
            var destIp3Dic = new Dictionary<string, int>();
            var destIp4Dic = new Dictionary<string, int>();
            var srcIpToPortLinks = new List<IpFlowIpToPortLink>();
            var destIpToPortLinks = new List<IpFlowIpToPortLink>();
            var portLinks = new List<IpFlowPortLink>();
            var links = new List<IpFlowLink>();
            var ipDatas = (from d in filteredDatas select new { Color = d.Color, SourceIp = d.SourceIp, SourcePort = d.SourcePort, DestinationIp = d.DestinationIp, DestinationPort = d.DestinationPort, SrcIpSegments = d.SourceIp.ToString().Split('.'), DestSegments = d.DestinationIp.ToString().Split('.') }).ToList(); // store the results

            foreach (var data in ipDatas)
            {
                var fillBrush = diagram.LinkBrush.CloneCurrentValue();
                fillBrush.Opacity = styleManager.LinkFillOpacity;
                var strokeBrush = diagram.LinkBrush.CloneCurrentValue();
                strokeBrush.Opacity = styleManager.LinkStrokeOpacity;
                BuildCountDictionary(data.SrcIpSegments[0], srcIp1Dic);
                BuildCountDictionary(data.SrcIpSegments[1], srcIp2Dic);
                BuildCountDictionary(data.SrcIpSegments[2], srcIp3Dic);
                var s4 = data.SrcIpSegments[3];
                BuildCountDictionary(s4, srcIp4Dic);
                var srcIpToPortLink = BuildLinks(CreateLink(data.SourcePort, s4, fillBrush, strokeBrush), srcIpToPortLinks, l => l.Segment == s4 && l.Port == data.SourcePort);
                BuildCountDictionary(data.DestSegments[0], destIp1Dic);
                BuildCountDictionary(data.DestSegments[1], destIp2Dic);
                BuildCountDictionary(data.DestSegments[2], destIp3Dic);
                var d4 = data.DestSegments[3];
                BuildCountDictionary(d4, destIp4Dic);
                var destIpToPortLink = BuildLinks(CreateLink(data.DestinationPort, d4, fillBrush, strokeBrush), destIpToPortLinks, l => l.Segment == d4 && l.Port == data.DestinationPort);
                var portLink = BuildLinks(CreateLink(data.SourcePort, data.DestinationPort, fillBrush, strokeBrush), portLinks, l => l.SourcePort == data.SourcePort && l.DestinationPort == data.DestinationPort);

                links.Add(new IpFlowLink()
                {
                    OriginalLinkFillBrush = fillBrush.CloneCurrentValue(),
                    OriginalLinkStrokeBrush = strokeBrush.CloneCurrentValue(),
                    SrcIpToPortLink = srcIpToPortLink,
                    DestPortToIpLink = destIpToPortLink,
                    SrcToDestPortLink = portLink
                });
            }

            #endregion

            #region ip segment node & port labels

            var srcPort200 = CreateSrcPortLabel(200);
            var destPort200 = CreateDestPortLabel(200);

            if (srcPort200 == null || destPort200 == null)
            {
                return;
            }

            srcPortLabels = new List<IpFlowPortLabel>() { srcPort200 };
            destPortLabels = new List<IpFlowPortLabel>() { destPort200 };
            SrcIpToPortContainer.Children.Add(srcPort200.Label);
            DestIpToPortContainer.Children.Add(destPort200.Label);
            styleManager.UpdateLabelAdjustY();
            var ipColumnHeight = SrcIpSegment1Container.ActualHeight;
            styleManager.DefaultNodesPaletteIndex = 0;
            var src1Ips = new List<IpFlowIpSegment>();
            var src2Ips = new List<IpFlowIpSegment>();
            var src3Ips = new List<IpFlowIpSegment>();
            var src4Ips = new List<IpFlowIpSegment4>();
            var dest1Ips = new List<IpFlowIpSegment>();
            var dest2Ips = new List<IpFlowIpSegment>();
            var dest3Ips = new List<IpFlowIpSegment>();
            var dest4Ips = new List<IpFlowIpSegment4>();
            nodes = new List<IpFlowNode>();

            foreach (var data in ipDatas)
            {
                var srcIpNode = new IpFlowIpNode()
                {
                    IpAddress = data.SourceIp
                };

                AddSegment(data.SrcIpSegments[0], data.Color, ipColumnHeight, srcIp1Dic, src1Ips, 0, srcIpNode);
                AddSegment(data.SrcIpSegments[1], data.Color, ipColumnHeight, srcIp2Dic, src2Ips, 1, srcIpNode);
                AddSegment(data.SrcIpSegments[2], data.Color, ipColumnHeight, srcIp3Dic, src3Ips, 2, srcIpNode);
                AddSegment(data.SrcIpSegments[3], data.Color, ipColumnHeight, srcIp4Dic, src4Ips, srcIpNode, srcIpToPortLinks);

                var destIpNode = new IpFlowIpNode()
                {
                    IpAddress = data.DestinationIp
                };

                AddSegment(data.DestSegments[0], data.Color, ipColumnHeight, destIp1Dic, dest1Ips, 0, destIpNode);
                AddSegment(data.DestSegments[1], data.Color, ipColumnHeight, destIp2Dic, dest2Ips, 1, destIpNode);
                AddSegment(data.DestSegments[2], data.Color, ipColumnHeight, destIp3Dic, dest3Ips, 2, destIpNode);
                AddSegment(data.DestSegments[3], data.Color, ipColumnHeight, destIp4Dic, dest4Ips, destIpNode, destIpToPortLinks);

                nodes.Add(new IpFlowNode()
                {
                    DestIpNode = destIpNode,
                    SrcIpNode = srcIpNode
                });

                AddSrcPortLabel(data.SourcePort, srcPortLabels, srcIpToPortLinks, portLinks);
                AddDestPortLabel(data.DestinationPort, destPortLabels, destIpToPortLinks, portLinks);
            }

            SrcIpSegment1Nodes = src1Ips;
            SrcIpSegment2Nodes = src2Ips;
            SrcIpSegment3Nodes = src3Ips;
            SrcIpSegment4Nodes = src4Ips;
            DestIpSegment1Nodes = dest1Ips;
            DestIpSegment2Nodes = dest2Ips;
            DestIpSegment3Nodes = dest3Ips;
            DestIpSegment4Nodes = dest4Ips;

            #endregion

            #region links

            CalculateSegment4VerticalPosition(src4Ips);
            CalculateSegment4VerticalPosition(dest4Ips);

            foreach (var link in srcIpToPortLinks)
            {
                // cal y
                SrcIpToPortContainer.Children.Add(ShapeSrcLink(link, SrcIpToPortContainer.ActualWidth).Shape);
            }

            foreach (var link in destIpToPortLinks)
            {
                // cal y
                DestIpToPortContainer.Children.Add(ShapeDestLink(link, DestIpToPortContainer.ActualWidth).Shape);
            }

            foreach (var node in nodes)
            {
                //node.Links = links.Select
            }

            #endregion
        }

        private List<IpFlowData> FilterDatas(IEnumerable<IpFlowData> datas)
        {
            var srcIpDic = new Dictionary<string, int>();
            var destIpDic = new Dictionary<string, int>();

            foreach (var data in datas)
            {
                var srcIp = default(IPAddress);
                var destIp = default(IPAddress);

                if (IPAddress.TryParse(data.SourceIp, out srcIp) && IPAddress.TryParse(data.DestinationIp, out destIp))
                {
                    BuildCountDictionary(data.SourceIp, srcIpDic);
                    BuildCountDictionary(data.DestinationIp, destIpDic);
                }
            }

            if (srcIpDic.Count == 0 || destIpDic.Count == 0)
            {
                return null;
            }

            // take most commonly occurred ips by MaxDisplayIpCount
            var sortedSrcIps = (from item in srcIpDic orderby item.Value descending select item).ToDictionary(item => item.Key, item => item.Value);
            var sortedDestIps = (from item in destIpDic orderby item.Value descending select item).ToDictionary(item => item.Key, item => item.Value);
            var filterSrcIps = sortedSrcIps.Take(diagram.MaxDisplayIpCount).ToDictionary(item => item.Key, item => item.Value);
            var filterDestIps = sortedDestIps.Take(diagram.MaxDisplayIpCount).ToDictionary(item => item.Key, item => item.Value);

            var filteredDatas = new List<IpFlowData>();

            foreach (var data in datas)
            {
                if (!filterSrcIps.Keys.Contains(data.SourceIp) || !filterDestIps.Keys.Contains(data.DestinationIp))
                {
                    continue;
                }

                filteredDatas.Add(data);
            }

            return filteredDatas;
        }

        #region ip segment node

        private void CalculateSegment4VerticalPosition(List<IpFlowIpSegment4> segments)
        {
           for(var index = 0; index < segments.Count; index++)
            {
                if(index == 0)
                {
                    segments[index].Y = 0;
                }
                else
                {
                    segments[index].Y = segments.Take(index).Sum(s => s.Height);
                }
            }
        }

        private void AddSegment(string ipSegment, Brush color, double columnHeight, Dictionary<string, int> ipSegmentDic, List<IpFlowIpSegment4> segments, IpFlowIpNode ipNode, List<IpFlowIpToPortLink> links)
        {
            var findNode = segments.Find(n => n.Segment == ipSegment);

            if (findNode == null)
            {
                var segment = ConfigureSegment(new IpFlowIpSegment4(), ipSegment, color, columnHeight, ipSegmentDic, segments);
                segments.Add(segment);
                ipNode.SetSegment(segment, 3);
                CalculateLinkPositionInSegment4Node(segment, links);
            }
            else
            {
                ipNode.SetSegment(findNode, 3);
            }
        }

        private void AddSegment(string ipSegment, Brush color, double columnHeight, Dictionary<string, int> ipSegmentDic, List<IpFlowIpSegment> segments, int index, IpFlowIpNode ipNode)
        {
            var findNode = segments.Find(n => n.Segment == ipSegment);

            if (findNode == null)
            {
                var segment = ConfigureSegment(new IpFlowIpSegment(), ipSegment, color, columnHeight, ipSegmentDic, segments);
                segment.IpAddress = ipNode.IpAddress;
                segments.Add(segment);
                ipNode.SetSegment(segment, index);
            }
            else
            {
                ipNode.SetSegment(findNode, index);
            }
        }

        private TSegment ConfigureSegment<TSegment>(TSegment segment, string ipSegment, Brush color, double columnHeight, Dictionary<string, int> ipSegmentDic, List<TSegment> nodes) where TSegment : IpFlowIpSegment
        {
            var count = ipSegmentDic.Values.Sum();
            var height = ipSegmentDic[ipSegment] / (double)count * columnHeight;
            segment.Height = height;
            segment.Segment = ipSegment;
            segment.Color = color != null ? color.CloneCurrentValue() : styleManager.SetNodeBrush();
            segment.OriginalBrush = segment.Color.CloneCurrentValue();

            return segment;
        }

        private void BuildCountDictionary<TKey>(TKey key, Dictionary<TKey, int> dictionary)
        {
            if (dictionary.Keys.Contains(key))
            {
                dictionary[key]++;
            }
            else
            {
                dictionary.Add(key, 1);
            }
        }

        #endregion

        #region port label

        private void AddSrcPortLabel(int port, List<IpFlowPortLabel> labels, List<IpFlowIpToPortLink> links, List<IpFlowPortLink> portLinks)
        {
            var label = CreateSrcPortLabel(port);

            if (AddPortLabel(label, labels))
            {
                SrcIpToPortContainer.Children.Add(label.Label);
            }
        }

        private void AddDestPortLabel(int port, List<IpFlowPortLabel> labels, List<IpFlowIpToPortLink> links, List<IpFlowPortLink> portLinks)
        {
            var label = CreateDestPortLabel(port);

            if (AddPortLabel(label, labels))
            {
                DestIpToPortContainer.Children.Add(label.Label);
            }
        }

        /// <summary>
        /// add label 
        /// </summary>
        /// <returns>true means added</returns>
        private bool AddPortLabel(IpFlowPortLabel label, List<IpFlowPortLabel> labels)
        {
            if (label == null)
            {
                return false;
            }

            if (labels.Exists(p => p.Port == label.Port))
            {
                return false;
            }

            labels.Add(label);

            return true;
        }

        private IpFlowPortLabel CreateSrcPortLabel(int port)
        {
            var portLabel = CreatePortLabel(port);
            Canvas.SetRight(portLabel.Label, 0);

            return portLabel;
        }

        private IpFlowPortLabel CreateDestPortLabel(int port)
        {
            var portLabel = CreatePortLabel(port);
            Canvas.SetLeft(portLabel.Label, 0);

            return portLabel;
        }

        private IpFlowPortLabel CreatePortLabel(int port)
        {
            var containerHeight = SrcToDestPortContainer.ActualHeight;

            if (containerHeight <= 0)
            {
                return null;
            }

            double y = -1;
            var value = port;
            var halfHeight = containerHeight / 2;

            if (port == 200)
            {
                y = halfHeight;
            }
            else if (0 <= port && port < 200)
            {
                y = (1 - port / 200.0) * halfHeight + halfHeight;
                y = y - styleManager.LabelAdjustedY;
            }
            else if (200 < port && port <= 60000)
            {
                value = ((port / 10000) + 1) * 10000;
                y = AdjustLabelYPosition(value, halfHeight);
            }
            else if (60000 < port && port <= 65535)
            {
                value = 65535;
                y = AdjustLabelYPosition(value, halfHeight);
            }
            else
            {
                return null;
            }

            if (y < 0)
            {
                return null;
            }

            var portLabel = new IpFlowPortLabel()
            {
                Port = value,
                Y = y,

                Label = new TextBlock()
                {
                    Text = value.ToString()
                }
            };

            if (diagram.LabelStyle != null)
            {
                portLabel.Label.Style = diagram.LabelStyle;
            }

            Canvas.SetBottom(portLabel.Label, portLabel.Y);

            return portLabel;
        }

        private double AdjustLabelYPosition(int port, double height)
        {
            if (port > maxPort)
            {
                maxPort = port;
            }

            UpdatePortLabelPosition(srcPortLabels, height);
            UpdatePortLabelPosition(destPortLabels, height);
            var y = (1 - (port / maxPort)) * height;

            return y;
        }

        private void UpdatePortLabelPosition(List<IpFlowPortLabel> labels, double height)
        {
            foreach (var portLabel in labels)
            {
                if (portLabel.Port > 200)
                {
                    portLabel.Y = (1 - (portLabel.Port / maxPort)) * height;
                    Canvas.SetBottom(portLabel.Label, portLabel.Y);
                }
            }
        }

        #endregion

        #region link

        private IpFlowIpToPortLink ShapeSrcLink(IpFlowIpToPortLink link, double width)
        {
            var startPoint = new Point();
            var line1EndPoint = new Point();
            var bezier1ControlPoint1 = new Point();
            var bezier1ControlPoint2 = new Point();
            var bezier1EndPoint = new Point();
            var bezier2ControlPoint1 = new Point();
            var bezier2ControlPoint2 = new Point();
            bezier2ControlPoint2.Y = startPoint.Y = link.Node.Y + link.PositionInNode;
            bezier1ControlPoint1.Y = line1EndPoint.Y = startPoint.Y + link.Width;
            bezier1ControlPoint2.Y = bezier1EndPoint.Y = bezier2ControlPoint1.Y = link.PositionInPorts;
            bezier1EndPoint.X = width;
            bezier2ControlPoint2.X = bezier1ControlPoint1.X = diagram.IpToPortLinkCurvature * width + startPoint.X;
            bezier2ControlPoint1.X = bezier1ControlPoint2.X = (1 - diagram.IpToPortLinkCurvature) * width + startPoint.X;

            var geometry = new PathGeometry()
            {
                Figures = new PathFigureCollection()
                {
                    new PathFigure()
                    {
                        StartPoint = startPoint,

                        Segments = new PathSegmentCollection()
                        {
                            new LineSegment() { Point = line1EndPoint },
                            new LineSegment() { Point = bezier1EndPoint },
                            new LineSegment() { Point = startPoint },

                            //new BezierSegment()
                            //{
                            //    Point1 = bezier1ControlPoint1,
                            //    Point2 = bezier1ControlPoint2,
                            //    Point3 = bezier1EndPoint
                            //},
                            //new BezierSegment()
                            //{
                            //    Point1 = bezier2ControlPoint1,
                            //    Point2 = bezier2ControlPoint2,
                            //    Point3 = startPoint
                            //}
                        }
                    },
                }
            };

            link.Shape.Data = geometry;
            Panel.SetZIndex(link.Shape, -1);

            return link;
        }

        private IpFlowIpToPortLink ShapeDestLink(IpFlowIpToPortLink link, double width)
        {
            var startPoint = new Point();
            var line1EndPoint = new Point();
            var bezier1ControlPoint1 = new Point();
            var bezier1ControlPoint2 = new Point();
            var bezier1EndPoint = new Point();
            var bezier2ControlPoint1 = new Point();
            var bezier2ControlPoint2 = new Point();
            bezier2ControlPoint2.Y = bezier1ControlPoint1.Y = startPoint.Y = link.PositionInPorts;
            bezier1EndPoint.Y = bezier1ControlPoint2.Y = link.Node.Y + link.PositionInNode;
            bezier2ControlPoint1.Y = line1EndPoint.Y = bezier1ControlPoint2.Y + link.Width;
            line1EndPoint.X = bezier1EndPoint.X = width;

            //bezier2ControlPoint2.Y = startPoint.Y = link.Node.Y + link.PositionInNode;
            //bezier1ControlPoint1.Y = line1EndPoint.Y = startPoint.Y + link.Width;
            //bezier2ControlPoint1.Y = link.PositionInPorts;
            //bezier1ControlPoint2.Y = bezier1EndPoint.Y = link.Width;
            //bezier1EndPoint.X = width;
            //bezier2ControlPoint2.X = bezier1ControlPoint1.X = diagram.IpToPortLinkCurvature * width + startPoint.X;
            //bezier2ControlPoint1.X = bezier1ControlPoint2.X = (1 - diagram.IpToPortLinkCurvature) * width + startPoint.X;

            var geometry = new PathGeometry()
            {
                Figures = new PathFigureCollection()
                {
                    new PathFigure()
                    {
                        StartPoint = startPoint,

                        Segments = new PathSegmentCollection()
                        {
                            new LineSegment() { Point = bezier1EndPoint },
                            new LineSegment() { Point = line1EndPoint },
                            new LineSegment() { Point = startPoint },

                            //new BezierSegment()
                            //{
                            //    Point1 = bezier1ControlPoint1,
                            //    Point2 = bezier1ControlPoint2,
                            //    Point3 = bezier1EndPoint
                            //},
                            //new BezierSegment()
                            //{
                            //    Point1 = bezier2ControlPoint1,
                            //    Point2 = bezier2ControlPoint2,
                            //    Point3 = startPoint
                            //}
                        }
                    },
                }
            };

            link.Shape.Data = geometry;
            Panel.SetZIndex(link.Shape, -1);

            return link;
        }

        private IpFlowPortLink ShapePortLink(IpFlowPortLink link, double width)
        {
            return null;
        }

        private void CalculateLinkPositionInSegment4Node(IpFlowIpSegment4 node, List<IpFlowIpToPortLink> links)
        {
            var segmentLinks = links.FindAll(l => l.Segment == node.Segment).ToList();
            var sumCount = segmentLinks.Sum(l => { return l.Count; });
            var position = 0.0;

            foreach (var link in segmentLinks)
            {
                link.PositionInNode = position;
                link.Width = node.Height * link.Count / sumCount;
                position += link.Width;
                link.Node = node;
            }
        }

        private IpFlowIpToPortLink CreateLink(int port, string segment, Brush fillBrush, Brush strokeBrush)
        {
            var shape = new Path()
            {
                Fill = fillBrush,
                Stroke = strokeBrush
            };

            return new IpFlowIpToPortLink()
            {
                Shape = shape,
                Port = port,
                Segment = segment
            };
        }

        private IpFlowPortLink CreateLink(int srcPort, int destPort, Brush fillBrush, Brush strokeBrush)
        {
            var shape = new Path()
            {
                Fill = fillBrush,
                Stroke = strokeBrush
            };

            return new IpFlowPortLink()
            {
                Shape = shape,
                SourcePort = srcPort,
                DestinationPort = destPort
            };
        }

        private void CalculateLinkPositionInSrcPorts(int originalPort, IpFlowPortLabel label, List<IpFlowIpToPortLink> links, List<IpFlowPortLink> portLinks)
        {
            if (label == null)
            {
                return;
            }

            CalculateLinkPositionInPorts(originalPort, label, links);
            var findPortLinks = portLinks.FindAll(l => l.SourcePort == originalPort).ToList();

            foreach (var link in findPortLinks)
            {
                if (link.SourcePort <= 200)
                {
                    link.PositionInSrcPorts = label.Y;
                }
                else
                {
                    var position = (1 + ((double)link.SourcePort / label.Port)) * label.Y;
                    link.PositionInSrcPorts = position < 0 ? 0 : position;
                }
            }
        }

        private void CalculateLinkPositionInDestPorts(int originalPort, IpFlowPortLabel label, List<IpFlowIpToPortLink> links, List<IpFlowPortLink> portLinks)
        {
            if (label == null)
            {
                return;
            }

            CalculateLinkPositionInPorts(originalPort, label, links);
            var findPortLinks = portLinks.FindAll(l => l.DestinationPort == originalPort).ToList();

            foreach (var link in findPortLinks)
            {
                if (link.DestinationPort <= 200)
                {
                    link.PositionInDestPorts = label.Y;
                }
                else
                {
                    var position = (1 + ((double)link.DestinationPort / label.Port)) * label.Y;
                    link.PositionInDestPorts = position < 0 ? 0 : position;
                }
            }
        }

        private void CalculateLinkPositionInPorts(int originalPort, IpFlowPortLabel label, List<IpFlowIpToPortLink> links)
        {
            var segmentLinks = links.FindAll(l => l.Port == originalPort).ToList();

            foreach (var link in segmentLinks)
            {
                if (link.Port <= 200)
                {
                    link.PositionInPorts =  label.Y;
                }
                else
                {
                    var position = (1 + ((double)link.Port / label.Port)) * label.Y;
                    link.PositionInPorts = position < 0 ? 0 : position; 
                }
            }
        }

        private TItem BuildLinks<TItem>(TItem link, List<TItem> links, Predicate<TItem> match) where TItem : IpFlowLinkBase
        {
            var l = links.Find(match);

            if(l != null)
            {
                l.Count++;

                return l;
            }
            else
            {
                link.Count = 1;
                links.Add(link);

                return link;
            }
        }

        #endregion

        #region ip segment node events

        private void NodeMouseEnter(object sender, MouseEventArgs e)
        {
            if (diagram.HighlightMode == HighlightMode.MouseEnter)
            {
                diagram.SetCurrentValue(IpFlowDiagram.HighlightNodeProperty, ((FrameworkElement)e.OriginalSource).Tag as string);
            }
        }

        private void NodeMouseLeave(object sender, MouseEventArgs e)
        {
            if (diagram.HighlightMode == HighlightMode.MouseEnter)
            {
                diagram.SetCurrentValue(IpFlowDiagram.HighlightNodeProperty, ((FrameworkElement)e.OriginalSource).Tag as string);
            }
        }

        private void NodeMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (diagram.HighlightMode == HighlightMode.MouseLeftButtonUp)
            {
                diagram.SetCurrentValue(IpFlowDiagram.HighlightNodeProperty, ((FrameworkElement)e.OriginalSource).Tag as string);
            }
        }

        #endregion

        #endregion

        #region Fields & Properties

        #region template parts

        public Grid DiagramGrid { get; set; }

        public ItemsControl SrcIpSegment1Container { get; set; }

        public ItemsControl SrcIpSegment2Container { get; set; }

        public ItemsControl SrcIpSegment3Container { get; set; }

        public ItemsControl SrcIpSegment4Container { get; set; }

        public ItemsControl DestIpSegment1Container { get; set; }

        public ItemsControl DestIpSegment2Container { get; set; }

        public ItemsControl DestIpSegment3Container { get; set; }

        public ItemsControl DestIpSegment4Container { get; set; }

        public Canvas SrcIpToPortContainer { get; set; }

        public Canvas SrcToDestPortContainer { get; set; }

        public Canvas DestIpToPortContainer { get; set; }

        #endregion

        #region nodes containers sources

        private List<IpFlowIpSegment> srcIpSegment1Nodes;
        public IReadOnlyList<IpFlowIpSegment> SrcIpSegment1Nodes
        {
            get
            {
                return srcIpSegment1Nodes;
            }
            set
            {
                srcIpSegment1Nodes = value == null ? null : value.ToList();
                RaisePropertyChanged(() => SrcIpSegment1Nodes);
            }
        }

        private List<IpFlowIpSegment> srcIpSegment2Nodes;
        public IReadOnlyList<IpFlowIpSegment> SrcIpSegment2Nodes
        {
            get
            {
                return srcIpSegment2Nodes;
            }
            set
            {
                srcIpSegment2Nodes = value == null ? null : value.ToList();
                RaisePropertyChanged(() => SrcIpSegment2Nodes);
            }
        }

        private List<IpFlowIpSegment> srcIpSegment3Nodes;
        public IReadOnlyList<IpFlowIpSegment> SrcIpSegment3Nodes
        {
            get
            {
                return srcIpSegment3Nodes;
            }
            set
            {
                srcIpSegment3Nodes = value == null ? null : value.ToList();
                RaisePropertyChanged(() => SrcIpSegment3Nodes);
            }
        }

        private List<IpFlowIpSegment4> srcIpSegment4Nodes;
        public IReadOnlyList<IpFlowIpSegment4> SrcIpSegment4Nodes
        {
            get
            {
                return srcIpSegment4Nodes;
            }
            set
            {
                srcIpSegment4Nodes = value == null ? null : value.ToList();
                RaisePropertyChanged(() => SrcIpSegment4Nodes);
            }
        }

        private List<IpFlowIpSegment> destIpSegment1Nodes;
        public IReadOnlyList<IpFlowIpSegment> DestIpSegment1Nodes
        {
            get
            {
                return destIpSegment1Nodes;
            }
            set
            {
                destIpSegment1Nodes = value == null ? null : value.ToList();
                RaisePropertyChanged(() => DestIpSegment1Nodes);
            }
        }

        private List<IpFlowIpSegment> destIpSegment2Nodes;
        public IReadOnlyList<IpFlowIpSegment> DestIpSegment2Nodes
        {
            get
            {
                return destIpSegment2Nodes;
            }
            set
            {
                destIpSegment2Nodes = value == null ? null : value.ToList();
                RaisePropertyChanged(() => DestIpSegment2Nodes);
            }
        }

        private List<IpFlowIpSegment> destIpSegment3Nodes;
        public IReadOnlyList<IpFlowIpSegment> DestIpSegment3Nodes
        {
            get
            {
                return destIpSegment3Nodes;
            }
            set
            {
                destIpSegment3Nodes = value == null ? null : value.ToList();
                RaisePropertyChanged(() => DestIpSegment3Nodes);
            }
        }

        private List<IpFlowIpSegment4> destIpSegment4Nodes;
        public IReadOnlyList<IpFlowIpSegment4> DestIpSegment4Nodes
        {
            get
            {
                return destIpSegment4Nodes;
            }
            set
            {
                destIpSegment4Nodes = value == null ? null : value.ToList();
                RaisePropertyChanged(() => DestIpSegment4Nodes);
            }
        }

        #endregion

        private List<IpFlowNode> nodes;
        public IReadOnlyList<IpFlowNode> Nodes
        {
            get
            {
                return nodes;
            }
        }

        private List<IpFlowPortLabel> srcPortLabels;
        public IReadOnlyList<IpFlowPortLabel> SrcPortLabels
        {
            get
            {
                return srcPortLabels;
            }
        }

        private List<IpFlowPortLabel> destPortLabels;
        public IReadOnlyList<IpFlowPortLabel> DestPortLabels
        {
            get
            {
                return destPortLabels;
            }
        }

        private double maxPort;

        private List<IpFlowData> filteredDatas;

        private IpFlowDiagram diagram;

        private IpFlowStyleManager styleManager;

        #endregion
    }
}
