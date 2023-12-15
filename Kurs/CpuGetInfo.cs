using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kurs
{
    class CpuGetInfo
    {
        public DateTime lastTime;
        public CpuGetInfo()
        {
            lastTime = DateTime.Now;
        }
        public double CpuUsage(Process process)
        {
                if (process.ProcessName == "Idle") { return 0; }
                TimeSpan lastTotalCpuTime = new TimeSpan(process.TotalProcessorTime.Hours, process.TotalProcessorTime.Minutes, process.TotalProcessorTime.Seconds);


            DateTime curTime = DateTime.Now;
                TimeSpan curTotalCpuTime = process.TotalProcessorTime;

               double result = (curTotalCpuTime.TotalMilliseconds - lastTotalCpuTime.TotalMilliseconds) / curTime.Subtract(lastTime).TotalMilliseconds / Convert.ToDouble(Environment.ProcessorCount);
                
            
            return result;
        }
    }
}
