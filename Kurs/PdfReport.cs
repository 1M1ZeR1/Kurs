using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kurs
{
    enum stage
    {
        inProgress,
        expectation
    }
    class PdfReport:CreatingChartForReport
    {
        private Paragraph nameAndTime;
        public FlowDocument flowDocument = new FlowDocument();
        public Paragraph AddInfoAboutProccessRam(string result)
        {
            Paragraph paragraph = new Paragraph(new Run($"Самое ресурсно-ёмкий(RAM) процесс за это  время:{result}\n\n"));
            paragraph.FontSize = 14;
            paragraph.TextAlignment = TextAlignment.Center;
            return paragraph;
        }
        public Paragraph AddInfoAboutProccessCpu(string result)
        {
            Paragraph paragraph = new Paragraph(new Run($"Самое ресурсно-ёмкий(Cpu) процесс за это  время:{result}\n\n"));
            paragraph.FontSize = 14;
            paragraph.TextAlignment = TextAlignment.Center;
            return paragraph;
        }
        private BlockUIContainer AddAverageWork(PlotModel chart)
        {
            var pngExporter = new PngExporter {  };
            var bitmap = pngExporter.ExportToBitmap(chart);
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = bitmap;
            BlockUIContainer blockUIContainer = new BlockUIContainer(image);
            return blockUIContainer;
        }
        public void NameOfComputerAndTime(DateTime? date)
        {
            nameAndTime = new Paragraph(new Run($"Название устройства:{Environment.MachineName}\nОтчёт потготовлен на период времени {DateTime.Now} - {date}\n\n"));
        }
       public Table CreateTable(string resultCpu,string resultMemory,List<ProccessesForReport> list)
        {
            Table table = new Table();
            table.Columns.Add(new TableColumn());

            table.RowGroups.Add(new TableRowGroup());
            table.RowGroups.Add(new TableRowGroup());
            table.RowGroups.Add(new TableRowGroup());
            table.RowGroups.Add(new TableRowGroup());

            table.RowGroups[0].Rows.Add(new TableRow());

            table.RowGroups[1].Rows.Add(new TableRow());

            table.RowGroups[2].Rows.Add(new TableRow());
            table.RowGroups[2].Rows.Add(new TableRow());

            TableRow nameAndTimeStart = table.RowGroups[0].Rows[0];
            nameAndTimeStart.FontSize = 14;
            nameAndTimeStart.Cells.Add(new TableCell(nameAndTime));

            TableRow rowMostRequiredProccess = table.RowGroups[1].Rows[0];
            rowMostRequiredProccess.FontSize = 12;
            rowMostRequiredProccess.Cells.Add(new TableCell(AddInfoAboutProccessCpu(resultCpu)));
            rowMostRequiredProccess.Cells.Add(new TableCell(AddInfoAboutProccessRam(resultMemory)));

            TableRow namesOfCharts = table.RowGroups[2].Rows[0];

            namesOfCharts.Cells.Add(new TableCell(new Paragraph(new Run("Cpu(Процессор)"))));
            namesOfCharts.Cells.Add(new TableCell(new Paragraph(new Run("RAM(Оперативная)"))));

            TableRow charts = table.RowGroups[2].Rows[1];
            charts.Cells.Add(new TableCell(AddAverageWork(plotModelCpu)));
            charts.Cells.Add(new TableCell(AddAverageWork(plotModelMemory)));

            if(list.Count != 0)
            {
                table.RowGroups[3].Rows.Add(new TableRow());
                TableRow nameOfTable = table.RowGroups[3].Rows[0];
                nameOfTable.FontSize = 18;
                nameOfTable.Cells.Add(new TableCell(new Paragraph(new Run("\nТаблица избранных процессов\n"))));

                table.RowGroups[3].Rows.Add(new TableRow());
                TableRow columnNames = table.RowGroups[3].Rows[1];
                columnNames.Cells.Add(new TableCell(new Paragraph(new Run("Name"))));
                columnNames.Cells.Add(new TableCell(new Paragraph(new Run("CPU"))));
                columnNames.Cells.Add(new TableCell(new Paragraph(new Run("RAM"))));

                for(int i=0; i<list.Count; i++)
                {
                    table.RowGroups[3].Rows.Add(new TableRow());
                    TableRow proccess = table.RowGroups[3].Rows[2+i];
                    proccess.Cells.Add(new TableCell(new Paragraph(new Run($"{list[i].proccessName}"))));
                    proccess.Cells.Add(new TableCell(new Paragraph(new Run($"{Math.Round(list[i].proccessCpu,2)} %"))));
                    proccess.Cells.Add(new TableCell(new Paragraph(new Run($"{Math.Round(list[i].proccessMemory,2)} Mb"))));
                } 
            }

            return table;
        }
    }
}
