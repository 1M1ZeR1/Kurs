using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurs
{
    public class OtherProccess:Process
    {
        public string OtherProccessName;
        public double OtherProccessMemory = 0;
        public double OtherProccessCPU = 0;
        public int Id;
        public OtherProccess(string otherProccessName, double otherProccessMemory, double otherProccessCPU)
        {
            OtherProccessName = otherProccessName;
            OtherProccessMemory = otherProccessMemory;
            OtherProccessCPU = otherProccessCPU;
        }
        public void OtherKill() { 
            Process process = Process.GetProcessById(Id);
            process.Kill();
        }
        
    }
}
