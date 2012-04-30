// -----------------------------------------------------------------------
// <copyright file="TestDFS.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Testing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;

    /// <summary>
    /// Tests the DFS class functionality.
    /// </summary>
    public class TestDFS
    {
        int[][] adjacencyMatrix = new[] {
                new[] {0,1,1,0,0,0,0,0},
                new[] {0,0,0,0,1,1,0,0},
                new[] {0,0,0,0,0,0,1,1},
                new[] {0,0,0,0,0,0,0,0},
                new[] {0,0,0,0,0,0,0,0},
                new[] {0,0,0,0,0,0,0,0},
                new[] {0,0,0,0,0,0,0,0},
                new[] {0,0,0,0,0,0,0,0}
            };

        public TestDFS()
        {
            Console.WriteLine("Testing simple search on adjacency matrix...");
            var ADFS =
                new AdjacencyDepthFirstSearch(adjacencyMatrix, 0, 5);
            int[] path = ADFS.Run();
            Console.WriteLine("Result: " + String.Join(",",path));
            
            
            var provider = new MetlinkDataProvider();
            INetworkNode[] route = null;
            int depth = 7;
            
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine("Testing on PT network... (DFS)");
            while (route == null)
            {
                Console.Write("Solving to depth: {0} --> ", depth++);
                var tdfs = new PTDepthFirstSearch(false,depth,provider, provider.GetNodeFromId(19965), provider.GetNodeFromId(19842));
                Console.WriteLine("");
                route = tdfs.Run();
                Console.WriteLine("");
                Console.WriteLine("Iterations: " + tdfs.Iterations);
            }
            
            Console.WriteLine("Result: " + String.Join(",",route.Cast<object>()) + " Time: " + sw.Elapsed.Seconds + " s");
            
            Console.WriteLine("Testing on PT network... (Greedy)");
            sw.Restart();
            
            route = null;
            depth = 7;
            while (route == null)
            {
                Console.Write("Solving to depth: {0} --> ", depth++);
                INetworkNode origin = provider.GetNodeFromId(19965);
                INetworkNode destination = provider.GetNodeFromId(19842);
                origin.CurrentRoute = provider.GetRoutesForNode(origin)[0];
                destination.CurrentRoute = provider.GetRoutesForNode(destination)[0];
                var tdfs = new PTGreedySearch(depth, false, provider, origin, destination);
                Console.WriteLine("");
                route = tdfs.Run();
                Console.WriteLine("");
                Console.WriteLine("Iterations: " + tdfs.Iterations);
            }

            Console.WriteLine("Result: " + String.Join(",", route.Cast<object>()) + " Time: " + sw.Elapsed.Seconds + " s");

        }
    }
}
