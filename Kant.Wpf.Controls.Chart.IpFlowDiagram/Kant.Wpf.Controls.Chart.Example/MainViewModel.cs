using Kant.Wpf.Toolkit;
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
            var count = 55;
            var datas = new List<IpFlowData>();

            for(var index = 0; index < count; index++)
            {
                var port = random.Next(55, 57);

                datas.Add(new IpFlowData()
                {
                    //SourceIp = "192.168.1." + random.Next(1, 255).ToString(),
                    //DestinationIp = "10.0.0." + random.Next(1, 255).ToString(),
                    //DestinationPort = random.Next(0, 200),
                    //DestinationPort = random.Next(0, 2) == 1 ? 155 : 55555,
                    //SourcePort = random.Next(0, 200),
                    //SourcePort = random.Next(0, 2) == 1 ? 55 : 25555,
                    SourcePort = port,
                    DestinationPort = port,
                    SourceIp = "192.168.1." + random.Next(1, 5),
                    DestinationIp = "10.0.0." + random.Next(1, 5)
                });
            }

            for (var index = count; index < count + count; index++)
            {
                var port = random.Next(45555, 45560);

                datas.Add(new IpFlowData()
                {
                    SourcePort = port,
                    DestinationPort = port,
                    SourceIp = "192.168.1." + random.Next(1, 5),
                    DestinationIp = "10.0.0." + random.Next(1, 5)
                });
            }

            datas[0].DestinationPort = 0;
            datas[1].DestinationPort = 199;
            datas[1].SourcePort = 65535;
            datas[2].SourcePort = 60000;
            datas[3].SourcePort = 50000;
            datas[4].SourcePort = 40000;
            datas[5].SourcePort = 20000;
            //datas[6].DestinationPort = datas[6].SourcePort = 10;
            //datas[7].DestinationPort = datas[7].SourcePort = 60;
            //datas[8].DestinationPort = datas[8].SourcePort = 50;
            //datas[9].DestinationPort = datas[9].SourcePort = 40;
            //datas[10].DestinationPort = datas[10].SourcePort = 20;
            //datas[11].DestinationPort = datas[11].SourcePort = 100;
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
                            SourceIp = random.Next(1, 192).ToString() + "." + random.Next(0, 168).ToString() + "." + random.Next(0, 55).ToString() + "." + random.Next(1, 255),
                            DestinationIp = random.Next(1, 192).ToString() + "." + random.Next(0, 168).ToString() + "." + random.Next(0, 55).ToString() + "." + random.Next(1, 255),
                            DestinationPort = random.Next(0, 65535),
                            SourcePort = random.Next(0, 65535),
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
                    Datas = null;
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
        public IReadOnlyList<IpFlowData> Datas
        {
            get
            {
                return datas;
            }
            private set
            {
                datas = value == null ? null : value.ToList();
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
