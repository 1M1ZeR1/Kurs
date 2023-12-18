using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurs
{
    class CreatingChartForReport
    {
        public PlotModel plotModelCpu;
        public PlotModel plotModelMemory;
        LineSeries seriesCpu = new LineSeries();
        LineSeries seriesMemory = new LineSeries();
        public void CreatingCpuChart()
        {
             plotModelCpu = new PlotModel() { Title = "CPU"};
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

            plotModelCpu.Axes.Add(AxisPosLeft);
            plotModelCpu.Axes.Add(AxisPosBottom);
            
            
            plotModelCpu.Series.Add(seriesCpu);
        }
        public void CreatingMemoryChart()
        {
            plotModelMemory = new PlotModel() { Title = "Memory" };
            var AxisPosLeft = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "Value",
            };

            var AxisPosBottom = new DateTimeAxis()
            {
                Title = "Time",
                Position = AxisPosition.Bottom,
            };

            plotModelMemory.Axes.Add(AxisPosLeft);
            plotModelMemory.Axes.Add(AxisPosBottom);

            plotModelMemory.Series.Add(seriesMemory);
        }
        public void AddNewPointForReportCPU(double value)
        {
            seriesCpu.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, value));
        }
        public void AddNewPointForReportMemory(double value)
        {
            seriesMemory.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, value));
        }

    }
}
