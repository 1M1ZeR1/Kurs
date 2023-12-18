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

        DateTime? dateTime;
        FavoritesList favoritesList = new FavoritesList();

        PdfReport pdfReport = new PdfReport();
        WhichFeald whichFeald = WhichFeald.None;
        ProccessesForReport[] massiv;

        stage stageStage = stage.expectation;
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
                otherProccesses[i].Id = mas[i].Id;
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
                    otherProccesses = otherProccesses.OrderByDescending(s => s.OtherProccessMemory).ToArray();
                    break;
                case WhichFeald.CPU:
                    otherProccesses = otherProccesses.OrderByDescending(s => s.OtherProccessCPU).ToArray();
                    break;

            }

                for (int i = 0; i < mas.Length; i++)
                {
                    resultName[i] = otherProccesses[i].OtherProccessName;
                    resultMemory[i] = Math.Round((double)otherProccesses[i].OtherProccessMemory / 1048576, 1);
                    resultCPU[i] = Math.Round(otherProccesses[i].OtherProccessCPU,2);
                }

            Papa.AddNewPointForMemory(resultMemory.Sum());
            Papa.AddNewPointForCpu(resultCPU.Sum());

            if (dateTime > DateTime.Now)
            {
                if(stageStage == stage.expectation)
                {
                    for (int i = 0; i < otherProccesses.Length; i++)
                    {
                        massiv[i].proccessName = otherProccesses[i].OtherProccessName;
                        massiv[i].Id = otherProccesses[i].Id;
                        massiv[i].proccessMemory += resultMemory[i];
                        massiv[i].proccessCpu += resultCPU[i];

                        favoritesList.addingInfo(otherProccesses[i], resultCPU[i], resultMemory[i]);
                    }

                    pdfReport.NameOfComputerAndTime(dateTime);
                    stageStage = stage.inProgress;
                }
                if(stageStage == stage.inProgress)
                {
                    pdfReport.AddNewPointForReportCPU(resultCPU.Sum());
                    pdfReport.AddNewPointForReportMemory(resultMemory.Sum());
                    for(int i = 0;i < otherProccesses.Length; i++)
                    {
                        if (massiv[i].Id == otherProccesses[i].Id)
                        {
                            massiv[i].proccessCpu += resultCPU[i];
                            massiv[i].proccessMemory += resultMemory[i];
                        }

                        favoritesList.addingInfo(otherProccesses[i], resultCPU[i], resultMemory[i]);
                    }
                }
            }
            else if(dateTime <= DateTime.Now)
            {

                FlowDocument flowdocument = new FlowDocument();
                massiv = massiv.OrderByDescending(s => s.proccessMemory).ToArray();
                string result1 = massiv[0].proccessName;
                massiv = massiv.OrderByDescending(s => s.proccessCpu).ToArray();
                string result2 = massiv[0].proccessName;

                flowdocument.Blocks.Add(pdfReport.CreateTable(result1,result2,favoritesList.finishList()));

                flowdocument.ColumnWidth = 500;
                PrintDialog printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    IDocumentPaginatorSource pageSource = (IDocumentPaginatorSource)flowdocument;
                    printDialog.PrintDocument(pageSource.DocumentPaginator, "Flow");
                }
                dateTime = null;
                stageStage = stage.expectation;
            }

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
                
                MenuItem favorites = new MenuItem()
                {
                    Header = "В избранные",Name = "favorites"
                };
                favorites.Click += (object sender, RoutedEventArgs e) =>
                {
                    var menuItem = (MenuItem)sender;

                    var contextMenu = (ContextMenu)menuItem.Parent;

                    var item = contextMenu.PlacementTarget;
                    favoritesList.addingProccess(otherProccesses[(int)item.GetValue(Grid.RowProperty)].OtherProccessName, otherProccesses[(int)item.GetValue(Grid.RowProperty)].Id);
                };

                MenuItem killProccess = new MenuItem()
                {
                    Header = "Остановить",Name = "killProccess"
                };
                killProccess.Click += (object sender, RoutedEventArgs e)=> 
                {
                    var menuItem = (MenuItem)sender;

                    var contextMenu = (ContextMenu)menuItem.Parent;

                    var item = contextMenu.PlacementTarget;
                    otherProccesses[(int)item.GetValue(Grid.RowProperty)].OtherKill();
                };

                contextMenu.Items.Add(killProccess);
                contextMenu.Items.Add(favorites);

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
                    Text = resultCPU[j].ToString() + "%"
                };
                OfProccesings.Children.Add(txt3);
                
                Grid.SetColumn(txt3,2);Grid.SetRow(txt3 , j);
            }

            return OfProccesings;
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

            Button MemoryOfProccess = new Button()
            {
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Content = new TextBlock() { Text = "Память" },
                Background = Brushes.WhiteSmoke
            };
            MemoryOfProccess.Click += (object sender, RoutedEventArgs e) =>
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

            grid.Children.Add(nameOfProccess);grid.Children.Add(MemoryOfProccess);grid.Children.Add(CpuOfProccess);

            Grid.SetColumn(nameOfProccess, 0);Grid.SetRow(nameOfProccess, 0);
            Grid.SetColumn(MemoryOfProccess, 1);Grid.SetRow(MemoryOfProccess , 0);
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
        private void products_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MemoryButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void CpuButton_Click(object sender, RoutedEventArgs e)
        {
            Papa.MyChartCpu.IsLegendVisible = true;
            Papa.MyChartMemory.IsLegendVisible = false;
        }
        
        private void DataResult_Click(object sender, RoutedEventArgs e)
        {
            Window GainTime = new Window();

            GainTime.Height = 150; GainTime.Width = 250;

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { });

            grid.RowDefinitions.Add(new RowDefinition() { });
            grid.RowDefinitions.Add(new RowDefinition() { });
            grid.RowDefinitions.Add(new RowDefinition() { });

            TextBlock textBlock = new TextBlock()
            {
                Text = "Введите время для записи отчёта",
                FontSize = 14,
            };
            grid.Children.Add(textBlock);

            Grid.SetRow(textBlock, 0); Grid.SetColumn(textBlock, 0);

            TextBox textBox = new TextBox()
            {
                Height = 20,
                Width = 200,
                MinWidth = 80,
            };

            grid.Children.Add(textBox);

            Grid.SetRow(textBox, 1); Grid.SetColumn(textBox, 0);

            WrapPanel wrapPanel = new WrapPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                ItemHeight = 20,
                ItemWidth = 60,
                Margin = new Thickness(0, 0, 20, 0),
            };

            Button confirm = new Button()
            {
                Content = "Confirm"
            };
            confirm.Click += (s, e) =>
            {
                if (textBox.Text == null || textBox.Text == "") MessageBox.Show("Вы ничего не ввели");
                else if (int.TryParse(textBox.Text, out int value))
                {
                    if (value < 0) MessageBox.Show("Ошибка: значение ввода отрицательно");
                    if (value == 0) MessageBox.Show("Ошибка: вы ввели 0");
                    if (value > 0)
                    {
                        dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute + value, DateTime.Now.Second);
                        massiv = new ProccessesForReport[mas.Length];
                        for (int i = 0; i < massiv.Length; i++)
                        {
                            massiv[i] = new ProccessesForReport();
                        }
                        pdfReport.CreatingCpuChart();
                        pdfReport.CreatingMemoryChart();
                        MessageBox.Show("Успешно, ожидайте");
                        GainTime.DialogResult = false;
                    }
                }
                else
                {
                    MessageBox.Show("Введены неверные значения");
                }
            };


            Button cancel = new Button()
            {
                Content = "Cancel"
            };
            cancel.Click += (s, e) => { GainTime.DialogResult = false; };

            wrapPanel.Children.Add(confirm);
            wrapPanel.Children.Add(cancel);

            grid.Children.Add(wrapPanel);

            Grid.SetColumn(wrapPanel, 0); Grid.SetRow(wrapPanel, 2);


            GainTime.Content = grid;
            GainTime.ShowDialog();
        }
    }
}
