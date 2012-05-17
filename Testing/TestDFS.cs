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
           /*
            Console.WriteLine("Testing simple search on adjacency matrix...");
            var ADFS =
                new AdjacencyDepthFirstSearch(adjacencyMatrix, 0, 5);
            int[] path = ADFS.Run();
            Console.WriteLine("Result: " + String.Join(",\n",path));

            Console.ReadLine();
            */
            var provider = new MetlinkDataProvider();
            INetworkNode[] route = null;
            int depth = 7;
            
            Stopwatch sw = Stopwatch.StartNew();
            
            Console.WriteLine("Testing on PT network... (DFS)");
            while (route == null)
            {
                for (int i = 3; i < 11; i++)
                {
                    checked
                    {
                        Console.Write("Solving to depth: {0} entropy {1} --> ", depth++, i / 10.0);
                        long totalIterations = 0;
                        long totalNodes = 0;
                        for (int j = 0; j < 500; j++)
                        {

                            var tdfs = new PTGreedySearch(depth, true, provider, provider.GetNodeFromId(19965), provider.GetNodeFromId(19879));
                            tdfs.Entropy = i / 10.0;
                            route = tdfs.Run();
                            totalIterations += tdfs.Iterations;
                            totalNodes += route.Length;

                        }
                        Console.WriteLine(
                            "Average Iterations: {0} Average Route Length: {1}",
                            totalIterations / 500.0,
                            totalNodes / 500.0);
                    }
                }
            }
            
            
            //Console.WriteLine("Result: \n " + String.Join(",\n",route.Cast<object>()) + " Time: " + sw.Elapsed.TotalSeconds + " s");
            Console.WriteLine("Result: Time: " + sw.Elapsed.TotalSeconds + " s  Total nodes: " + route.Length);
            return;
            Console.ReadLine();
             
            Console.WriteLine("Testing on PT network... (Greedy)");
            sw.Restart();
            
            route = null;
            depth = 7;
            while (route == null)
            {
                Console.Write("Solving to depth: {0} --> ", depth++);
                INetworkNode origin = provider.GetNodeFromId(19965);
                INetworkNode destination = provider.GetNodeFromId(19879);
                origin.CurrentRoute = provider.GetRoutesForNode(origin)[0];
                destination.CurrentRoute = provider.GetRoutesForNode(destination)[0];
                var tdfs = new PTGreedySearch(depth, true, provider, origin, destination);
                Console.WriteLine("");
                route = tdfs.Run();
                Console.WriteLine("");
                Console.WriteLine("Iterations: " + tdfs.Iterations);
            }
           // Console.WriteLine("Result: \n" + String.Join(", ", route.Cast<object>()) + " Time: " + sw.Elapsed.TotalSeconds + " s  Total nodes: " + route.Length);
            Console.WriteLine("Result: Time: " + sw.Elapsed.TotalSeconds + " s  Total nodes: " + route.Length);
            Console.ReadLine();
            Console.WriteLine("Testing on PT network... (A*)");
            sw.Restart();
            route = null;
            depth = 7;
            while (route == null)
            {
                Console.Write("Solving to depth: {0} --> ", depth++);
                INetworkNode origin = provider.GetNodeFromId(19965);
                INetworkNode destination = provider.GetNodeFromId(19879);
                origin.CurrentRoute = provider.GetRoutesForNode(origin)[0];
                destination.CurrentRoute = provider.GetRoutesForNode(destination)[0];
                var tdfs = new PTAStarSearch(depth,true, provider, origin, destination);
                Console.WriteLine("");
                route = tdfs.Run();
                Console.WriteLine("");
                Console.WriteLine("Iterations: " + tdfs.Iterations);
            }

            //Console.WriteLine("Result: \n" + String.Join(", ", route.Cast<object>()) + " Time: " + sw.Elapsed.TotalSeconds + " s Total nodes: " + route.Length);
            Console.WriteLine("Result: Time: " + sw.Elapsed.TotalSeconds + " s Total nodes: " + route.Length);
            /*
            Console.WriteLine("Testing on PT network... (Rand)");
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
                var tdfs = new RandDepthFirstSearch(false, depth, provider, origin, destination);
                Console.WriteLine("");
                route = tdfs.Run();
                Console.WriteLine("");
                Console.WriteLine("Iterations: " + tdfs.Iterations);
            }

            Console.WriteLine("Result: " + String.Join(",\n", route.Cast<object>()) + " Time: " + sw.Elapsed.TotalSeconds + " s");
*/
        }

        private MetlinkNode[] GetNodesFromIds(int[] ids,INetworkDataProvider provider)
        {
            MetlinkNode[] output = new MetlinkNode[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                output[i] = new MetlinkNode(ids[i],provider);

            }
            return output;
        }
    }
}
