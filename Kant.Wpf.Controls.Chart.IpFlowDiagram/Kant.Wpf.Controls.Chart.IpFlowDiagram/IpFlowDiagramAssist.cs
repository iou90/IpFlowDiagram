using Kant.Wpf.Toolkit;
using Kant.Wpf.Toolkit.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

            UpdateDiagram(diagram.Datas);
        }

        public void UpdateDiagram(IEnumerable<IpFlowData> datas)
        {
            // clear diagram first
            ClearDiagram();

            if (datas == null || datas.Count() == 0)
            {
                return;
            }

            // drawing...
            if (diagram.IsLoaded)
            {
                // check template parts sizes
                if (DiagramGrid.ActualHeight <= 0 || (DiagramGrid.ActualWidth - 5 - diagram.IpSegmentColumnWidth * 8) <= 0)
                {
                    return;
                }

                // filter datas
                var filteredDatas = FilterDatas(datas);

                if(filteredDatas == null || filteredDatas.Count == 0)
                {
                    return;
                }

                CreateDiagram(filteredDatas);
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
            ClearDatas<IpFlowIpNode>(srcIpSegment1Nodes);
            ClearDatas<IpFlowIpNode>(srcIpSegment2Nodes);
            ClearDatas<IpFlowIpNode>(srcIpSegment3Nodes);
            ClearDatas<IpFlowIpNode>(srcIpSegment4Nodes);
            ClearDatas<IpFlowIpNode>(destIpSegment1Nodes);
            ClearDatas<IpFlowIpNode>(destIpSegment2Nodes);
            ClearDatas<IpFlowIpNode>(destIpSegment3Nodes);
            ClearDatas<IpFlowIpNode>(destIpSegment4Nodes);
            ClearDatas<IpFlowPortLabel>(srcPortLabels);
            ClearDatas<IpFlowPortLabel>(destPortLabels);
            ClearDiagramCanvasChilds();
        }

        public void ClearDiagramCanvasChilds()
        {
            if (SrcIpToPortContainer != null && SrcIpToPortContainer.Children != null)
            {
                SrcIpToPortContainer.Children.Clear();
            }

            if (DestPortToIpContainer != null && DestPortToIpContainer.Children != null)
            {
                DestPortToIpContainer.Children.Clear();
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
                    BuildDictionary(data.SourceIp, srcIpDic);
                    BuildDictionary(data.DestinationIp, destIpDic);
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

        private void CreateDiagram(List<IpFlowData> datas)
        {
            var srcIp1Dic = new Dictionary<string, int>();
            var srcIp2Dic = new Dictionary<string, int>();
            var srcIp3Dic = new Dictionary<string, int>();
            var srcIp4Dic = new Dictionary<string, int>();
            var destIp1Dic = new Dictionary<string, int>();
            var destIp2Dic = new Dictionary<string, int>();
            var destIp3Dic = new Dictionary<string, int>();
            var destIp4Dic = new Dictionary<string, int>();

            foreach (var data in datas)
            {
                var srcIpStringArray = data.SourceIp.ToString().Split('.');
                BuildDictionary(srcIpStringArray[0], srcIp1Dic);
                BuildDictionary(srcIpStringArray[1], srcIp2Dic);
                BuildDictionary(srcIpStringArray[2], srcIp3Dic);
                BuildDictionary(srcIpStringArray[3], srcIp4Dic);
                var destIpStringArray = data.DestinationIp.ToString().Split('.');
                BuildDictionary(destIpStringArray[0], destIp1Dic);
                BuildDictionary(destIpStringArray[1], destIp2Dic);
                BuildDictionary(destIpStringArray[2], destIp3Dic);
                BuildDictionary(destIpStringArray[3], destIp4Dic);
            }

            var ipColumnHeight = SrcIpSegment1Container.ActualHeight;
            styleManager.DefaultNodesPaletteIndex = 0;
            var src1Ips = new List<IpFlowIpNode>();
            var src2Ips = new List<IpFlowIpNode>();
            var src3Ips = new List<IpFlowIpNode>();
            var src4Ips = new List<IpFlowIpNode>();
            var dest1Ips = new List<IpFlowIpNode>();
            var dest2Ips = new List<IpFlowIpNode>();
            var dest3Ips = new List<IpFlowIpNode>();
            var dest4Ips = new List<IpFlowIpNode>();
            var srcPort200 = CreatePortLabel(200, true);
            var destPort200 = CreatePortLabel(200, false);

            if (srcPort200 == null || destPort200 == null)
            {
                return;
            }

            srcPortLabels = new List<IpFlowPortLabel>() { srcPort200 };
            destPortLabels = new List<IpFlowPortLabel>() { destPort200 };
            SrcIpToPortContainer.Children.Add(srcPort200.Label);
            DestPortToIpContainer.Children.Add(destPort200.Label);
            styleManager.UpdateLabelAdjustY();

            foreach (var data in datas)
            {
                var srcIpStringArray = data.SourceIp.Split('.');
                AddNode(srcIpStringArray[0], data, ipColumnHeight, srcIp1Dic, src1Ips);
                AddNode(srcIpStringArray[1], data, ipColumnHeight, srcIp2Dic, src2Ips);
                AddNode(srcIpStringArray[2], data, ipColumnHeight, srcIp3Dic, src3Ips);
                AddNode(srcIpStringArray[3], data, ipColumnHeight, srcIp4Dic, src4Ips);
                var destIpStringArray = data.DestinationIp.Split('.');
                AddNode(destIpStringArray[0], data, ipColumnHeight, destIp1Dic, dest1Ips);
                AddNode(destIpStringArray[1], data, ipColumnHeight, destIp2Dic, dest2Ips);
                AddNode(destIpStringArray[2], data, ipColumnHeight, destIp3Dic, dest3Ips);
                AddNode(destIpStringArray[3], data, ipColumnHeight, destIp4Dic, dest4Ips);
                AddPortLabel(data.SourcePort, srcPortLabels, true);
                AddPortLabel(data.DestinationPort, destPortLabels, false);
            }

            SrcIpSegment1Nodes = src1Ips;
            SrcIpSegment2Nodes = src2Ips;
            SrcIpSegment3Nodes = src3Ips;
            SrcIpSegment4Nodes = src4Ips;
            DestIpSegment1Nodes = dest1Ips;
            DestIpSegment2Nodes = dest2Ips;
            DestIpSegment3Nodes = dest3Ips;
            DestIpSegment4Nodes = dest4Ips;
        }

        private void BuildDictionary(string value, Dictionary<string, int> dictionary)
        {
            if (dictionary.Keys.Contains(value))
            {
                dictionary[value]++;
            }
            else
            {
                dictionary.Add(value, 1);
            }
        }

        private void AddNode(string value, IpFlowData data, double columnHeight, Dictionary<string, int> ipSegmentDic, List<IpFlowIpNode> nodes)
        {
            var count = ipSegmentDic.Values.Sum();
            var height = ipSegmentDic[value] / (double)count * columnHeight;

            var node = new IpFlowIpNode()
            {
                Height = height,
                Value = value,
                Color = data.Color != null ? data.Color.CloneCurrentValue() : styleManager.SetNodeBrush()
            };

            node.OriginalBrush = node.Color.CloneCurrentValue();

            if (!nodes.Exists(n => n.Value == value))
            {
                nodes.Add(node);
            }
        }

        /// <summary>
        /// add label to src\destPortLabels
        /// </summary>
        /// <param name="srcOrDest">true means src, positioning left</param>
        private void AddPortLabel(int port, List<IpFlowPortLabel> labels, bool srcOrDest)
        {
            var label = CreatePortLabel(port, srcOrDest);

            if (label == null)
            {
                return;
            }

            if (labels.Exists(p => p.Value == label.Value))
            {
                return;
            }

            labels.Add(label);

            if (srcOrDest)
            {
                SrcIpToPortContainer.Children.Add(label.Label);
            }
            else
            {
                DestPortToIpContainer.Children.Add(label.Label);
            }
        }

        /// <summary>
        /// create label & set position
        /// </summary>
        /// <param name="srcOrDest">true means src, positioning left</param>
        private IpFlowPortLabel CreatePortLabel(int port, bool srcOrDest)
        {
            var containerHeight = SrcToDestPortContainer.ActualHeight;

            if(containerHeight <= 0)
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

            if(y < 0)
            {
                return null;
            }

            y = y - styleManager.LabelAdjustedY;

            var portLabel = new IpFlowPortLabel()
            {
                Value = value,
                Y = y,

                Label = new TextBlock()
                {
                    Text = value.ToString()
                }
            };

            if(diagram.LabelStyle != null)
            {
                portLabel.Label.Style = diagram.LabelStyle;
            }

            Canvas.SetBottom(portLabel.Label, portLabel.Y);

            if (srcOrDest)
            {
                Canvas.SetRight(portLabel.Label, 0);
            }
            else
            {
                Canvas.SetLeft(portLabel.Label, 0);
            }

            return portLabel;
        }

        private double AdjustLabelYPosition(int port, double height)
        {
            if(port > maxPort)
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
                if (portLabel.Value > 200)
                {
                    portLabel.Y = (1 - (portLabel.Value / maxPort)) * height - styleManager.LabelAdjustedY;
                    Canvas.SetBottom(portLabel.Label, portLabel.Y);
                }
            }
        }

        #region node events

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

        public Canvas DestPortToIpContainer { get; set; }

        #endregion

        #region nodes containers sources

        private List<IpFlowIpNode> srcIpSegment1Nodes;
        public IReadOnlyList<IpFlowIpNode> SrcIpSegment1Nodes
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

        private List<IpFlowIpNode> srcIpSegment2Nodes;
        public IReadOnlyList<IpFlowIpNode> SrcIpSegment2Nodes
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

        private List<IpFlowIpNode> srcIpSegment3Nodes;
        public IReadOnlyList<IpFlowIpNode> SrcIpSegment3Nodes
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

        private List<IpFlowIpNode> srcIpSegment4Nodes;
        public IReadOnlyList<IpFlowIpNode> SrcIpSegment4Nodes
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

        private List<IpFlowIpNode> destIpSegment1Nodes;
        public IReadOnlyList<IpFlowIpNode> DestIpSegment1Nodes
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

        private List<IpFlowIpNode> destIpSegment2Nodes;
        public IReadOnlyList<IpFlowIpNode> DestIpSegment2Nodes
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

        private List<IpFlowIpNode> destIpSegment3Nodes;
        public IReadOnlyList<IpFlowIpNode> DestIpSegment3Nodes
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

        private List<IpFlowIpNode> destIpSegment4Nodes;
        public IReadOnlyList<IpFlowIpNode> DestIpSegment4Nodes
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

        private IpFlowDiagram diagram;

        private IpFlowStyleManager styleManager;

        #endregion
    }
}
