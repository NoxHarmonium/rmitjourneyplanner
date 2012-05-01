using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RmitJourneyPlanner.CoreLibraries;

namespace Testing
{
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;

    class Program
    {
        static void Main(string[] args)
        {
            RmitJourneyPlanner.CoreLibraries.Logging.Logger.LogEvent += Logger_LogEvent;
            RmitJourneyPlanner.CoreLibraries.Logging.Logger.ProgressEvent += new RmitJourneyPlanner.CoreLibraries.Logging.ProgressEventHandler(Logger_ProgressEvent);

            TestDFS dfsTest = new TestDFS();
            //TestStoSort();


            Console.ReadLine();
        }

       
        static void TestStoSort()
        {

            var nodes = new List<INetworkNode>();
            for (int i = 2; i < 20; i++)
            {
                INetworkNode node = new MetlinkNode(i, "", "", 0, 0, null);
                node.EuclidianDistance = i;
                node.TotalTime = TimeSpan.FromSeconds(i);
                nodes.Add(node);
                //nodes.Add();

            }
            Console.WriteLine("Before: " + String.Join(",", nodes.Cast<object>()));
            for (int i = 0; i < 10; i++)
            {

                nodes.StochasticSort();
                Console.WriteLine("After:  " + String.Join(",", nodes.Cast<object>()));
            }
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
