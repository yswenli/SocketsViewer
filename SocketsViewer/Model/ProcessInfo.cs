using System;
using System.Collections.Generic;
using System.Text;

namespace SocketsViewer.Model
{
    public class ProcessInfo
    {
        public int Id { get; set; }

        public string ProcessName { get; set; }

        public double TotalMilliseconds { get; set; }

        public long WorkingSet64 { get; set; }

        public string FileName { get; set; }

        public ProcessInfo()
        {

        }

        public ProcessInfo(int id, string processName, double totalMilliseconds, long workingSet64, string fileName)
        {
            Id = id;
            ProcessName = processName;
            TotalMilliseconds = totalMilliseconds;
            WorkingSet64 = workingSet64;
            FileName = fileName;
        }
    }
}
