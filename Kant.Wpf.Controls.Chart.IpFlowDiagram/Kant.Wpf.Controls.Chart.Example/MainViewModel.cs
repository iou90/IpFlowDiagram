﻿using Kant.Wpf.Toolkit;
using Kant.Wpf.Toolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Kant.Wpf.Controls.Chart.Example
{
    public class MainViewModel : ViewModelBase
    {
        #region Constructor

        public MainViewModel()
        {
            random = new Random();
            //bubbleColor = (Brush)Application.Current.FindResource("BubbleColor");
            //bubbleLabelStyle1 = (Style)Application.Current.FindResource("BubbleLabelStyle1");
            //bubbleLabelStyle2 = (Style)Application.Current.FindResource("BubbleLabelStyle2");
            //BubbleLabelStyle = bubbleLabelStyle2;

            // random datas
            var count = 15;
            var datas = new List<IpFlowData>();

            for(var index = 0; index < count; index++)
            {
                datas.Add(new IpFlowData()
                {
                    //SourceIp = "192.168.1." + random.Next(1, 255).ToString(),
                    //DestinationIp = "10.0.0." + random.Next(1, 255).ToString(),
                    DestinationPort = random.Next(1, 65535),
                    SourcePort = random.Next(1, 65535),
                    SourceIp = "192.168.1." + random.Next(1, 5),
                    DestinationIp = "10.0.0." + random.Next(1, 5)
                });
            }

            Datas = datas;
            BubbleBrushes = new Dictionary<string, Brush>() { { "word1", bubbleColor } };
        }

        #endregion

        #region Commands

        private ICommand changeDatas;
        public ICommand ChangeDatas
        {
            get
            {
                return GetCommand(changeDatas, new RelayCommand<object>(o =>
                {
                    var count = random.Next(5, 55);
                    var datas = new List<IpFlowData>();

                    for (var index = 0; index < count; index++)
                    {
                        datas.Add(new IpFlowData()
                        {
                            SourceIp = random.Next(1, 192).ToString() + "." + random.Next(1, 168).ToString() + "." + random.Next(1, 55).ToString() + "." + random.Next(1, 255),
                            DestinationIp = random.Next(1, 192).ToString() + "." + random.Next(1, 168).ToString() + "." + random.Next(1, 55).ToString() + "." + random.Next(1, 255),
                            DestinationPort = random.Next(1, 65535),
                            SourcePort = random.Next(1, 65535),
                        });
                    }

                    Datas = datas;
                }));
            }
        }

        private ICommand clearDiagram;
        public ICommand ClearDiagram
        {
            get
            {
                return GetCommand(clearDiagram, new RelayCommand(() =>
                {
                }));
            }
        }

        private ICommand clearHighlight;
        public ICommand ClearHighlight
        {
            get
            {
                return GetCommand(clearHighlight, new RelayCommand(() =>
                {
                }));
            }
        }

        private ICommand highlightingNode;
        public ICommand HighlightingNode
        {
            get
            {
                return GetCommand(highlightingNode, new RelayCommand(() =>
                {
                }));
            }
        }

        private ICommand highlightingLink;
        public ICommand HighlightingLink
        {
            get
            {
                return GetCommand(highlightingLink, new RelayCommand(() =>
                {
                }));
            }
        }

        private ICommand changeStyles;
        public ICommand ChangeStyles
        {
            get
            {
                return GetCommand(changeStyles, new RelayCommand(() =>
                {
                    
                }));
            }
        }

        #endregion

        #region Fields & Properties

        private List<IpFlowData> datas;
        public List<IpFlowData> Datas
        {
            get
            {
                return datas;
            }
            set
            {
                datas = value;
                RaisePropertyChanged(() => Datas);
            }
        }

        private Brush bubbleBrush;
        public Brush BubbleBrush
        {
            get
            {
                return bubbleBrush;
            }
            set
            {
                if (value != bubbleBrush)
                {
                    bubbleBrush = value;
                    RaisePropertyChanged(() => BubbleBrush);
                }
            }
        }

        private Dictionary<string, Brush> bubbleBrushes;
        public Dictionary<string, Brush> BubbleBrushes
        {
            get
            {
                return bubbleBrushes;
            }
            set
            {
                if (value != bubbleBrushes)
                {
                    bubbleBrushes = value;
                    RaisePropertyChanged(() => BubbleBrushes);
                }
            }
        }

        private HighlightMode highlightMode;
        public HighlightMode HighlightMode
        {
            get
            {
                return highlightMode;
            }
            set
            {
                highlightMode = value;
                RaisePropertyChanged(() => HighlightMode);
            }
        }

        private string highlightNode;
        public string HighlightNode
        {
            get
            {
                return highlightNode;
            }
            set
            {
                highlightNode = value;
                RaisePropertyChanged(() => HighlightNode);
            }
        }

        #region bubble test

        private double diameter;
        public double Diameter
        {
            get
            {
                return diameter;
            }
            set
            {
                diameter = value;
                RaisePropertyChanged(() => Diameter);
            }
        }

        private string label;
        public string Label
        {
            get
            {
                return label;
            }
            set
            {
                label = value;
                RaisePropertyChanged(() => Label);
            }
        }

        #endregion

        private Style bubbleLabelStyle;
        public Style BubbleLabelStyle
        {
            get
            {
                return bubbleLabelStyle;
            }
            set
            {
                if (value != bubbleLabelStyle)
                {
                    bubbleLabelStyle = value;
                    RaisePropertyChanged(() => BubbleLabelStyle);
                }
            }
        }

        private Style bubbleLabelStyle1;

        private Style bubbleLabelStyle2;

        private Brush bubbleColor;

        private Random random;

        #endregion
    }
}