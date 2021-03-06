﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kant.Wpf.Controls.Chart
{
    [TemplatePart(Name = "PartDiagramGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PartSrcIpSegment1", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PartSrcIpSegment2", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PartSrcIpSegment3", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PartSrcIpSegment4", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PartDestIpSegment1", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PartDestIpSegment2", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PartDestIpSegment3", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PartDestIpSegment4", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PartSrcIpToPort", Type = typeof(Canvas))]
    [TemplatePart(Name = "PartSrcToDestPort", Type = typeof(Canvas))]
    [TemplatePart(Name = "PartDestIpToPort", Type = typeof(Canvas))]
    public class IpFlowDiagram : Control, IDisposable
    {
        #region Constructor

        static IpFlowDiagram()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IpFlowDiagram), new FrameworkPropertyMetadata(typeof(IpFlowDiagram)));
        }

        public IpFlowDiagram()
        {
            disposedValue = false;
            styleManager = new IpFlowStyleManager(this);
            assist = new IpFlowDiagramAssist(this, styleManager);
            Loaded += IpFlowDiagramLoaded;
            SizeChanged += assist.DiagramSizeChanged;
        }

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            var grid = GetTemplateChild("PartDiagramGrid") as Grid;
            var src1 = GetTemplateChild("PartSrcIpSegment1") as ItemsControl;
            var src2 = GetTemplateChild("PartSrcIpSegment2") as ItemsControl;
            var src3 = GetTemplateChild("PartSrcIpSegment3") as ItemsControl;
            var src4 = GetTemplateChild("PartSrcIpSegment4") as ItemsControl;
            var dest1 = GetTemplateChild("PartDestIpSegment1") as ItemsControl;
            var dest2 = GetTemplateChild("PartDestIpSegment2") as ItemsControl;
            var dest3 = GetTemplateChild("PartDestIpSegment3") as ItemsControl;
            var dest4 = GetTemplateChild("PartDestIpSegment4") as ItemsControl;
            var srcIpToPort = GetTemplateChild("PartSrcIpToPort") as Canvas;
            var srcToDestPort = GetTemplateChild("PartSrcToDestPort") as Canvas;
            var destIpToPort = GetTemplateChild("PartDestIpToPort") as Canvas;

            if (CheckTemplatePartExists(grid, "PartDiagramGrid"))
            {
                assist.DiagramGrid = grid;
            }

            if (CheckTemplatePartExists(src1, "PartSrcIpSegment1"))
            {
                assist.SrcIpSegment1Container = src1;
            }
         
            if (CheckTemplatePartExists(src2, "PartSrcIpSegment2"))
            {
                assist.SrcIpSegment2Container = src2;
            }

            if (CheckTemplatePartExists(src3, "PartSrcIpSegment3"))
            {
                assist.SrcIpSegment3Container = src3;
            }

            if (CheckTemplatePartExists(src4, "PartSrcIpSegment4"))
            {
                assist.SrcIpSegment4Container = src4;
            }

            if (CheckTemplatePartExists(dest1, "PartDestIpSegment1"))
            {
                assist.DestIpSegment1Container = dest1;
            }

            if (CheckTemplatePartExists(dest2, "PartDestIpSegment2"))
            {
                assist.DestIpSegment2Container = dest2;
            }

            if (CheckTemplatePartExists(dest3, "PartDestIpSegment3"))
            {
                assist.DestIpSegment3Container = dest3;
            }

            if (CheckTemplatePartExists(dest4, "PartDestIpSegment4"))
            {
                assist.DestIpSegment4Container = dest4;
            }

            if (CheckTemplatePartExists(srcIpToPort, "PartSrcIpToPort"))
            {
                assist.SrcIpToPortContainer = srcIpToPort;
            }

            if (CheckTemplatePartExists(srcToDestPort, "PartSrcToDestPort"))
            {
                assist.SrcToDestPortContainer = srcToDestPort;
            }

            if(CheckTemplatePartExists(destIpToPort, "PartDestIpToPort"))
            {
                assist.DestIpToPortContainer = destIpToPort;
            }

            assist.SetIpNodesSource();
            base.OnApplyTemplate();
        }

        private bool CheckTemplatePartExists(FrameworkElement element, string name)
        {
            if (element == null)
            {
                throw new MissingMemberException("can not find template child" + name + ".");
            }
            else
            {
                return true;
            }
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (assist != null)
                    {
                        assist.ClearDiagram();
                    }
                }

                disposedValue = true;
            }
        }

        ~IpFlowDiagram()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        private static void OnHighlightModeSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((IpFlowDiagram)o).styleManager.ClearHighlight();
        }

        private static void OnDatasSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((IpFlowDiagram)o).assist.UpdateDiagram((IEnumerable<IpFlowData>)e.NewValue);
        }

        private static void OnSegmentNodeToolTipTemplateSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var diagram = (IpFlowDiagram)o;
            diagram.styleManager.ChangeSegmentNodeToolTipTemplate((ControlTemplate)e.NewValue, diagram.assist.SrcIpSegment1Nodes);
            diagram.styleManager.ChangeSegmentNodeToolTipTemplate((ControlTemplate)e.NewValue, diagram.assist.SrcIpSegment2Nodes);
            diagram.styleManager.ChangeSegmentNodeToolTipTemplate((ControlTemplate)e.NewValue, diagram.assist.SrcIpSegment3Nodes);
            diagram.styleManager.ChangeSegmentNodeToolTipTemplate((ControlTemplate)e.NewValue, diagram.assist.SrcIpSegment4Nodes);
            diagram.styleManager.ChangeSegmentNodeToolTipTemplate((ControlTemplate)e.NewValue, diagram.assist.DestIpSegment1Nodes);
            diagram.styleManager.ChangeSegmentNodeToolTipTemplate((ControlTemplate)e.NewValue, diagram.assist.DestIpSegment2Nodes);
            diagram.styleManager.ChangeSegmentNodeToolTipTemplate((ControlTemplate)e.NewValue, diagram.assist.DestIpSegment3Nodes);
            diagram.styleManager.ChangeSegmentNodeToolTipTemplate((ControlTemplate)e.NewValue, diagram.assist.DestIpSegment4Nodes);
        }

        private static object HighlightNodeValueCallback(DependencyObject o, object value)
        {
            var diagram = (IpFlowDiagram)o;
            diagram.styleManager.HighlightingNode((IpFlowIpSegmentFinder)value, diagram.assist.Nodes);

            return value;
        }

        private void IpFlowDiagramLoaded(object sender, RoutedEventArgs e)
        {
            assist.DrawDiagram();
            Loaded -= IpFlowDiagramLoaded;
        }

        #endregion

        #region Commands

        public ICommand MouseLeftButtonUpIpFlowIpSegment
        {
            get
            {
                if (HighlightMode == HighlightMode.MouseLeftButtonUp)
                {
                    return assist.SelectIpFlowIpSegment;
                }
                else
                {
                    return null;
                }
            }
        }

        public ICommand MouseEnterIpFlowIpSegment
        {
            get
            {
                if (HighlightMode == HighlightMode.MouseEnter)
                {
                    return assist.SelectIpFlowIpSegment;
                }
                else
                {
                    return null;
                }
            }
        }

        public ICommand MouseLeaveIpFlowIpSegment
        {
            get
            {
                if (HighlightMode == HighlightMode.MouseEnter)
                {
                    return assist.UnselectIpFlowIpSegment;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Fields & Properties

        #region dependency properties

        public IEnumerable<IpFlowData> Datas
        {
            get { return (IEnumerable<IpFlowData>)GetValue(DatasProperty); }
            set { SetValue(DatasProperty, value); }
        }

        public static readonly DependencyProperty DatasProperty = DependencyProperty.Register("Datas", typeof(IEnumerable<IpFlowData>), typeof(IpFlowDiagram), new PropertyMetadata(new List<IpFlowData>(), OnDatasSourceChanged));

        public HighlightMode HighlightMode
        {
            get { return (HighlightMode)GetValue(HighlightModeProperty); }
            set { SetValue(HighlightModeProperty, value); }
        }

        public static readonly DependencyProperty HighlightModeProperty = DependencyProperty.Register("HighlightMode", typeof(HighlightMode), typeof(IpFlowDiagram), new PropertyMetadata(HighlightMode.MouseLeftButtonUp, OnHighlightModeSourceChanged));

        public IpFlowIpSegmentFinder HighlightNode
        {
            get { return (IpFlowIpSegmentFinder)GetValue(HighlightNodeProperty); }
            set { SetValue(HighlightNodeProperty, value); }
        }

        public static readonly DependencyProperty HighlightNodeProperty = DependencyProperty.Register("HighlightNode", typeof(IpFlowIpSegmentFinder), typeof(IpFlowDiagram), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, HighlightNodeValueCallback));

        public Style NodeLabelStyle
        {
            get { return (Style)GetValue(NodeLabelStyleProperty); }
            set { SetValue(NodeLabelStyleProperty, value); }
        }

        public static readonly DependencyProperty NodeLabelStyleProperty = DependencyProperty.Register("NodeLabelStyle", typeof(Style), typeof(IpFlowDiagram));

        public Style LegendLabelStyle
        {
            get { return (Style)GetValue(LegendLabelStyleProperty); }
            set { SetValue(LegendLabelStyleProperty, value); }
        }

        public static readonly DependencyProperty LegendLabelStyleProperty = DependencyProperty.Register("LegendLabelStyle", typeof(Style), typeof(IpFlowDiagram));

        public ControlTemplate SegmentNodeToolTipTemplate
        {
            get { return (ControlTemplate)GetValue(SegmentNodeToolTipTemplateProperty); }
            set { SetValue(SegmentNodeToolTipTemplateProperty, value); }
        }

        public static readonly DependencyProperty SegmentNodeToolTipTemplateProperty = DependencyProperty.Register("SegmentNodeToolTipTemplate", typeof(ControlTemplate), typeof(IpFlowDiagram), new PropertyMetadata(OnSegmentNodeToolTipTemplateSourceChanged));

        #endregion

        #region initial settings

        /// <summary>
        /// 20 by default
        /// </summary>
        public double IpSegmentColumnWidth { get; set; }

        public Brush PortSplitLineBrush { get; set; }

        public Brush IpSegmentNodeBorderBrush { get; set; }

        public Brush LinkFill { get; set; }

        public Brush LinkStroke { get; set; }

        /// <summary>
        /// 0.55 by default, ranged from 0 to 1
        /// </summary>
        public double LinkCurvature { get; set; }

        /// <summary>
        /// apply to nodes, links
        /// 1.0 by default
        /// </summary>
        public double HighlightOpacity { get; set; }

        /// <summary>
        /// apply to nodes, links
        /// 0.25 by default
        /// </summary>
        public double LoweredOpacity { get; set; }

        /// <summary>
        /// 10 by default
        /// </summary>
        public int MaxDisplayIpCount { get; set; }

        #endregion

        private IpFlowDiagramAssist assist;

        private IpFlowStyleManager styleManager;

        private bool disposedValue;

        #endregion
    }
}
