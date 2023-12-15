using System;
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
using System.Diagnostics;
using System.Threading;
using System.Data;
using System.Timers;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Diagnostics.Eventing.Reader;

namespace Kurs
{
    public partial class MainWindow : Window
    {
        enum WhichFeald
        {
            None,
            name,
            RAM,
            CPU
        }

        
        WhichFeald whichFeald = WhichFeald.None; 

        CpuGetInfo CpuGetInfo = new CpuGetInfo();

        public Process[] mas;

        private Grid gr;
        private ScrollViewer ScV = ScrollOfProccesings();
        private void SetTimer()
        {
            gr = GridMain();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (object sender, EventArgs e) =>
            {
                UpdateInfo(GridOfProccesings(gr), ScV);
                CpuGetInfo.lastTime = DateTime.Now;

            };
            timer.Interval = new TimeSpan(0, 0, 3);
            timer.Start();
        }

        public MainWindow()
        {
            InitializeComponent();
            var gridFor = DataGrid();
            TabProccesing.Content = gridFor;
            SetTimer();

        }
        private Grid GridOfProccesings(Grid OfProccesings)
        {
            mas = Process.GetProcesses();
            OtherProccess[] otherProccesses = new OtherProccess[mas.Length] ;
            for(int i = 0; i < otherProccesses.Length; i++)
            {
                otherProccesses[i] = new OtherProccess(mas[i].ProcessName, mas[i].PrivateMemorySize64, CpuGetInfo.CpuUsage(mas[i]));
            }

            OfProccesings.ColumnDefinitions.Clear();
            OfProccesings.RowDefinitions.Clear();
            OfProccesings.Children.Clear();

            OfProccesings.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(256) });
            OfProccesings.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(256) });
            OfProccesings.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(256) });

            string[] resultName = new string[mas.Length];double[] resultMemory = new double[mas.Length]; TimeSpan[] timeProccessor = new TimeSpan[mas.Length];
            double[] resultCPU = new double[mas.Length];
            switch (whichFeald)
            {
                case WhichFeald.name:
                    otherProccesses = otherProccesses.OrderBy(s => s.OtherProccessName).ToArray();
                    break;

                case WhichFeald.RAM:
                    otherProccesses = otherProccesses.OrderByDescending(s => s.OtherProccessMemmory).ToArray();
                    break;
                case WhichFeald.CPU:
                    otherProccesses = otherProccesses.OrderByDescending(s => s.OtherProccessCPU).ToArray();
                    break;

            }

                for (int i = 0; i < mas.Length; i++)
                {
                    resultName[i] = otherProccesses[i].OtherProccessName;
                    resultMemory[i] = Math.Round((double)otherProccesses[i].OtherProccessMemmory / 1048576, 1);
                    resultCPU[i] = otherProccesses[i].OtherProccessCPU;
                }

            Papa.AddNewPoint(resultMemory.Sum());

            for (int i = 0; i < mas.Length; i++)
            {
                OfProccesings.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20) });
            }

            RowDefinition myRowDefinitions = new RowDefinition();

            for (int j = 0; j < mas.Length; j++)
            {
                myRowDefinitions.Height = GridLength.Auto;
                OfProccesings.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2,GridUnitType.Auto) });

                Button button = new Button() {Background = Brushes.White, Content = new TextBlock { Text = $"{resultName[j]}", Margin = new Thickness(0, 0, 180, 0), FontWeight = FontWeights.Bold,FontSize = 14} };

                ContextMenu contextMenu = new ContextMenu();
                MenuItem menuItem = new MenuItem()
                {
                    Header = "Остановить",Name = "MenuItem" 
                };
                menuItem.Click += MenuItem_Click;

                contextMenu.Items.Add(menuItem);

                button.ContextMenu = contextMenu;
                var var = (int)button.GetValue(Grid.RowProperty);

                OfProccesings.Children.Add(button);
                Grid.SetColumn(button, 0);Grid.SetRow(button, j);


                TextBlock txt2 = new TextBlock();
                txt2.FontSize = 14;
                txt2.FontWeight = FontWeights.Bold;
                txt2.Text = resultMemory[j].ToString();
                OfProccesings.Children.Add(txt2);

                Grid.SetColumn(txt2, 1);
                Grid.SetRow(txt2, j);

                TextBlock txt3 = new TextBlock()
                {
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Text = resultCPU[j].ToString()
                };
                OfProccesings.Children.Add(txt3);
                
                Grid.SetColumn(txt3,2);Grid.SetRow(txt3 , j);
            }

            return OfProccesings;
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;

            var contextMenu = (ContextMenu)menuItem.Parent;

            var item = contextMenu.PlacementTarget;
            mas[(int)item.GetValue(Grid.RowProperty)].Kill();

        }


        private Grid GridMain()
        {
            Grid OfProccesings = new Grid();

            OfProccesings.Width = 768;
            OfProccesings.HorizontalAlignment = HorizontalAlignment.Left;
            OfProccesings.VerticalAlignment = VerticalAlignment.Top;
            OfProccesings.ShowGridLines = true;

            return OfProccesings;
        }
        private Grid GridWithToggles()
        {
            Grid grid = new Grid() 
            {
                Width = 768,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(256) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(256) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(256) });

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });

            Button nameOfProccess = new Button()
            {
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Content = new TextBlock() { Text = "Имя процесса" },
                Background = Brushes.WhiteSmoke
            };
            nameOfProccess.Click += (object sender, RoutedEventArgs e)=> {
                whichFeald = WhichFeald.name;
                UpdateInfo(GridOfProccesings(gr), ScV);
            };

            Button memmoryOfProccess = new Button()
            {
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Content = new TextBlock() { Text = "Память" },
                Background = Brushes.WhiteSmoke
            };
            memmoryOfProccess.Click += (object sender, RoutedEventArgs e) =>
            {
                whichFeald = WhichFeald.RAM;
                UpdateInfo(GridOfProccesings(gr), ScV);
            };

            Button CpuOfProccess = new Button()
            {
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Content = new TextBlock() { Text = "Процессор (CPU)" },
                Background = Brushes.WhiteSmoke
            };
            CpuOfProccess.Click += (object sender, RoutedEventArgs e) =>
            {
                whichFeald = WhichFeald.CPU;
                UpdateInfo(GridOfProccesings(gr), ScV);
            };

            grid.Children.Add(nameOfProccess);grid.Children.Add(memmoryOfProccess);grid.Children.Add(CpuOfProccess);

            Grid.SetColumn(nameOfProccess, 0);Grid.SetRow(nameOfProccess, 0);
            Grid.SetColumn(memmoryOfProccess, 1);Grid.SetRow(memmoryOfProccess , 0);
            Grid.SetColumn(CpuOfProccess, 2);Grid.SetRow(CpuOfProccess , 0);

            return grid;
        }
        private Grid DataGrid()
        {
            Grid grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition() {Height=new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition());

            var Toggles = GridWithToggles();

            grid.Children.Add(Toggles);
            grid.Children.Add(ScV);

            Grid.SetRow(Toggles, 0);Grid.SetColumn(Toggles, 0);
            Grid.SetRow(ScV, 1);Grid.SetColumn(ScV, 0);
            return grid;
        }

        private static ScrollViewer ScrollOfProccesings()
        {
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            return scrollViewer;

        }
        private void UpdateInfo(Grid grid, ScrollViewer scrollViewer)
        {
            scrollViewer.Content = grid;
        }
        private string GetProcessName(Process process)
        {
            string processName = "";
            string baseProcessName = process.ProcessName;
            int baseProcessId = process.Id;

            int processOptionsChecked = 0;
            int maxNrOfParallelProcesses = 3 + 1;

            bool notFound = true;
            
            while(notFound)
            {
                processName = baseProcessName;
                if(processOptionsChecked > maxNrOfParallelProcesses)
                {
                    break;
                }

                if(1 == processOptionsChecked)
                {
                    processName = string.Format("{0}_{1}",baseProcessName,baseProcessId);
                }
                else if(processOptionsChecked > 1)
                {
                    processName = string.Format("{0}_{1}", baseProcessName, processOptionsChecked-1);
                }

                try
                {
                    PerformanceCounter counter = new PerformanceCounter("Process", "ID Process", processName);
                    if(baseProcessId == (int)counter.NextValue())
                    {
                        notFound = false;
                    }
                }
                catch (Exception ex) { }
                processOptionsChecked++;
            }
            return processName;
        }

        private void products_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MemmoryButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CpuButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
