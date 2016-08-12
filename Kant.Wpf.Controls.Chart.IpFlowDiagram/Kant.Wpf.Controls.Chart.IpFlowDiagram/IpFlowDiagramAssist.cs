using Kant.Wpf.Toolkit;
using Kant.Wpf.Toolkit.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
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

                // update nodes
                ParseDatasToNodes(datas);

                // create links
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
            SrcIpSegment1Nodes = null;
            SrcIpSegment2Nodes = null;
            SrcIpSegment3Nodes = null;
            SrcIpSegment4Nodes = null;
            DestIpSegment1Nodes = null;
            DestIpSegment2Nodes = null;
            DestIpSegment3Nodes = null;
            DestIpSegment4Nodes = null;
            ClearLinks();
        }

        public void ClearLinks()
        {
        }

        #endregion

        private void ParseDatasToNodes(IEnumerable<IpFlowData> datas)
        {
            #region parse datas

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

            if(srcIpDic.Count <= 0 || destIpDic.Count <= 0)
            {
                return;
            }

            var sortedSrcIps = (from item in srcIpDic orderby item.Value descending select item).ToDictionary(item => item.Key, item => item.Value);
            var sortedDestIps = (from item in destIpDic orderby item.Value descending select item).ToDictionary(item => item.Key, item => item.Value);
            var filterSrcIps = sortedSrcIps.Take(diagram.MaxDisplayIpCount).ToDictionary(item => item.Key, item => item.Value);
            var filterDestIps = sortedDestIps.Take(diagram.MaxDisplayIpCount).ToDictionary(item => item.Key, item => item.Value);

            if(filterSrcIps.Count != filterDestIps.Count)
            {
                return;
            }

            var srcIp1Dic = new Dictionary<string, int>();
            var srcIp2Dic = new Dictionary<string, int>();
            var srcIp3Dic = new Dictionary<string, int>();
            var srcIp4Dic = new Dictionary<string, int>();
            var destIp1Dic = new Dictionary<string, int>();
            var destIp2Dic = new Dictionary<string, int>();
            var destIp3Dic = new Dictionary<string, int>();
            var destIp4Dic = new Dictionary<string, int>();
            var filterDatas = new List<IpFlowData>();

            foreach(var data in datas)
            {
                if (!filterSrcIps.Keys.Contains(data.SourceIp) || !filterDestIps.Keys.Contains(data.DestinationIp))
                {
                    continue;
                }

                filterDatas.Add(data);
            }

            foreach (var data in filterDatas)
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

            #endregion

            #region add nodes

            var ipColumnHeight = SrcIpSegment1Container.ActualHeight;
            styleManager.DefaultNodesPaletteIndex = 0;
            var src1Ips = new List<IpFlowNode>();
            var src2Ips = new List<IpFlowNode>();
            var src3Ips = new List<IpFlowNode>();
            var src4Ips = new List<IpFlowNode>();
            var dest1Ips = new List<IpFlowNode>();
            var dest2Ips = new List<IpFlowNode>();
            var dest3Ips = new List<IpFlowNode>();
            var dest4Ips = new List<IpFlowNode>();

            foreach (var data in filterDatas)
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
        }

        private void AddNode(string value, IpFlowData data, double columnHeight, Dictionary<string, int> ipSegmentDic, List<IpFlowNode> nodes)
        {
            var count = ipSegmentDic.Values.Sum();
            var height = ipSegmentDic[value] / (double)count * columnHeight;

            var node = new IpFlowNode()
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

        public Canvas SrcToDestContainer { get; set; }

        public Canvas DestPortToIpContainer { get; set; }

        #endregion

        #region nodes containers sources

        private List<IpFlowNode> srcIpSegment1Nodes;
        public List<IpFlowNode> SrcIpSegment1Nodes
        {
            get
            {
                return srcIpSegment1Nodes;
            }
            set
            {
                srcIpSegment1Nodes = value;
                RaisePropertyChanged(() => SrcIpSegment1Nodes);
            }
        }

        private List<IpFlowNode> srcIpSegment2Nodes;
        public List<IpFlowNode> SrcIpSegment2Nodes
        {
            get
            {
                return srcIpSegment2Nodes;
            }
            set
            {
                srcIpSegment2Nodes = value;
                RaisePropertyChanged(() => SrcIpSegment2Nodes);
            }
        }

        private List<IpFlowNode> srcIpSegment3Nodes;
        public List<IpFlowNode> SrcIpSegment3Nodes
        {
            get
            {
                return srcIpSegment3Nodes;
            }
            set
            {
                srcIpSegment3Nodes = value;
                RaisePropertyChanged(() => SrcIpSegment3Nodes);
            }
        }

        private List<IpFlowNode> srcIpSegment4Nodes;
        public List<IpFlowNode> SrcIpSegment4Nodes
        {
            get
            {
                return srcIpSegment4Nodes;
            }
            set
            {
                srcIpSegment4Nodes = value;
                RaisePropertyChanged(() => SrcIpSegment4Nodes);
            }
        }

        private List<IpFlowNode> destIpSegment1Nodes;
        public List<IpFlowNode> DestIpSegment1Nodes
        {
            get
            {
                return destIpSegment1Nodes;
            }
            set
            {
                destIpSegment1Nodes = value;
                RaisePropertyChanged(() => DestIpSegment1Nodes);
            }
        }

        private List<IpFlowNode> destIpSegment2Nodes;
        public List<IpFlowNode> DestIpSegment2Nodes
        {
            get
            {
                return destIpSegment2Nodes;
            }
            set
            {
                destIpSegment2Nodes = value;
                RaisePropertyChanged(() => DestIpSegment2Nodes);
            }
        }

        private List<IpFlowNode> destIpSegment3Nodes;
        public List<IpFlowNode> DestIpSegment3Nodes
        {
            get
            {
                return destIpSegment3Nodes;
            }
            set
            {
                destIpSegment3Nodes = value;
                RaisePropertyChanged(() => DestIpSegment3Nodes);
            }
        }

        private List<IpFlowNode> destIpSegment4Nodes;
        public List<IpFlowNode> DestIpSegment4Nodes
        {
            get
            {
                return destIpSegment4Nodes;
            }
            set
            {
                destIpSegment4Nodes = value;
                RaisePropertyChanged(() => DestIpSegment4Nodes);
            }
        }

        #endregion

        private IpFlowDiagram diagram;

        private IpFlowStyleManager styleManager;

        #endregion
    }
}
