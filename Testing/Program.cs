using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RmitJourneyPlanner.CoreLibraries;

namespace Testing
{
    using RmitJourneyPlanner.CoreLibraries.Comparers;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;

    class Program
    {
        static void Main(string[] args)
        {
            RmitJourneyPlanner.CoreLibraries.Logging.Logger.LogEvent += Logger_LogEvent;
            RmitJourneyPlanner.CoreLibraries.Logging.Logger.ProgressEvent += Logger_ProgressEvent;

            //Console.WriteLine(~0);
            //Console.WriteLine(~1);
            //TestDFS dfsTest = new TestDFS();
            //TestStoSort();
            //TestJP jp = new TestJP();
            //jp.Test();
            //SpawnTravelPlanner sp = new SpawnTravelPlanner();
            TestFitnessFunction f = new TestFitnessFunction();
            Console.ReadLine();


            //Console.ReadLine();
        }

       
        static void TestStoSort()
        {

            var nodes = new List<NodeWrapper<INetworkNode>>();
            for (int i = 1; i < 11; i++)
            {
                var node = new NodeWrapper<INetworkNode>(new MetlinkNode(i, "", "", 0, 0, null));
                node.EuclidianDistance = i;
                node.TotalTime = TimeSpan.FromSeconds(i);
                nodes.Add(node);
                //nodes.Add();

            }
            nodes.Shuffle();
          

            Console.WriteLine("Before: " + String.Join(",", nodes.Cast<object>()));
            for (int i = 1; i < 11; i++)
            {
                var newNodes = new List<NodeWrapper<INetworkNode>>(nodes);
                newNodes.StochasticSort((i - 1) / 10.0);
                newNodes.Reverse();
               
                Console.WriteLine("After:  " + String.Join(",", newNodes.Cast<object>()));
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
