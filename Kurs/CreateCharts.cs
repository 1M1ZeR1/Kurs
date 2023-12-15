using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Kurs
{
    class CreateCharts
    {
        public PlotModel MyChart { get; set; }
        public LineSeries SeriesCpu = new LineSeries();

        public CreateCharts() 
        {

            MyChart = new PlotModel()
            {
                Title = "CPU",
            };

            var AxisPosLeft = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "LAAAA"
            };

            var AxisPosBottom = new DateTimeAxis()
            {
                Title = "Time",
                Position = AxisPosition.Bottom,
            };

            MyChart.Axes.Add(AxisPosBottom);
            MyChart.Axes.Add(AxisPosLeft);
            MyChart.Series.Add(SeriesCpu);

        }
        public void AddNewPoint(double value)
        {
            SeriesCpu.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now,value));
        }
    }
}
