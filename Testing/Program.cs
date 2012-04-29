using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            RmitJourneyPlanner.CoreLibraries.Logging.Logger.LogEvent += Logger_LogEvent;
            RmitJourneyPlanner.CoreLibraries.Logging.Logger.ProgressEvent += new RmitJourneyPlanner.CoreLibraries.Logging.ProgressEventHandler(Logger_ProgressEvent);

            TestDFS dfsTest = new TestDFS();
            Console.ReadLine();
        }

        static void Logger_ProgressEvent(object sender, int progress)
        {
            Console.Clear();
            Console.WriteLine("Progress: " + progress);
        }

        static void Logger_LogEvent(object sender, string message)
        {
            Console.WriteLine("[Logger: ]"+ message);
            
        }
    }
}
