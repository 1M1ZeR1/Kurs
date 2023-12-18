using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Kurs
{
    class CreateCharts
    {
        public PlotModel MyChart { get; set; }
        public PlotModel MyChartCpu { get; set; }
        public PlotModel MyChartMemory { get; set; }

        public LineSeries SeriesMemory = new LineSeries();
        public LineSeries SeriesCpu = new LineSeries();

        public CreateCharts() 
        {

            MyChartMemory = new PlotModel()
            {
                Title = "Memory",
            };

            var AxisPosLeftMemory = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "Value",
            };

            var AxisPosBottomMemory = new DateTimeAxis()
            {
                Title = "Time",
                Position = AxisPosition.Bottom,
            };

            MyChartMemory.Axes.Add(AxisPosBottomMemory);
            MyChartMemory.Axes.Add(AxisPosLeftMemory);
            MyChartMemory.Series.Add(SeriesMemory);


            MyChartCpu = new PlotModel()
            {
                Title = "CPU",
            };

            var AxisPosLeft = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "Value",
                Minimum = 0,
                Maximum = 100,
            };

            var AxisPosBottom = new DateTimeAxis()
            {
                Title = "Time",
                Position = AxisPosition.Bottom,
            };

            MyChartCpu.Axes.Add(AxisPosBottom);
            MyChartCpu.Axes.Add(AxisPosLeft);
            MyChartCpu.Series.Add(SeriesCpu);

        }
        public void AddNewPointForCpu(double value)
        {
            SeriesCpu.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now,value));
            MyChartCpu.InvalidatePlot(true);
            if(SeriesCpu.Points.Count > 10)SeriesCpu.Points.RemoveAt(0);
        }
        public void AddNewPointForMemory(double value)
        {
            SeriesMemory.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now,value));
            MyChartMemory.InvalidatePlot(true);
            if (SeriesMemory.Points.Count > 10) SeriesMemory.Points.RemoveAt(0);
        }
    }
}
