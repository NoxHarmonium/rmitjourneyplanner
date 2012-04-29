// -----------------------------------------------------------------------
// <copyright file="TestDFS.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Testing
{
    using System;
    using System.Collections.Generic;
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

            Console.WriteLine("Testing on PT network...");
            var provider = new MetlinkDataProvider();
            INetworkNode[] route = null;
            int depth = 1;
            while (route == null)
            {
                Console.Write("Solving to depth: {0} --> ", depth++);
                var tdfs = new PTDepthFirstSearch(depth,provider, provider.GetNodeFromId(19965), provider.GetNodeFromId(19842));
                route = tdfs.Run();
                Console.WriteLine("Iterations: " + tdfs.Iterations);
            }

        
            
            Console.WriteLine("Result: " + String.Join(",",route.Cast<object>()));

        }
    }
}
