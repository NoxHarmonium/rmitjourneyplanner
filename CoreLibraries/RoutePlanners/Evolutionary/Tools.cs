// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   A collection of static tools for use by the evolutionary route planner.
    /// </summary>
    public static class Tools
    {
        #region Public Methods

        /// <summary>
        ///   The save population.
        /// </summary>
        /// <param name="population"> The population. </param>
        /// <param name="generation"> The generation. </param>
        /// <param name="properties"> The properties. </param>
        /// <param name="filename">The filename. </param>
        public static void SavePopulation(List<Critter> population, int generation, EvolutionaryProperties properties, string filename)
        {
            //Console.ForegroundColor = ConsoleColor.Green;

            // List<KeyValuePair<List<int>, TimeSpan>> kvps = population.ToList();
            // kvps.Sort((x, y) => x.Value.CompareTo(y.Value));
            // File.Delete("Results.txt");
            string template = new StreamReader("Template.htm").ReadToEnd();
            foreach (Critter kvp in population)
            {
                // Console.WriteLine("{0} : {1}", kvp.UnifiedFitnessScore, kvp.Key.Count);
                string file = filename;
                           
                string markerCode = string.Empty;
                foreach (NodeWrapper<INetworkNode> t in kvp.Route)
                {
                    int key = Convert.ToInt32(t.Node.Id);
                    TimeSpan time = t.TotalTime;

                    string latlng = string.Format(
                        "new google.maps.LatLng({0},{1})",
                        properties.NetworkDataProviders[0].GetNodeFromId(key).Latitude,
                        properties.NetworkDataProviders[0].GetNodeFromId(key).Longitude);
                    markerCode +=
                        string.Format(
                            "var marker = new google.maps.Marker({{position: {0}, map: map,title:\"{1}: {2} ({3})\"}});\n",
                            latlng,
                            key,
                            time.ToString(),
                            ((MetlinkNode)properties.NetworkDataProviders[0].GetNodeFromId(key)).StopSpecName);
                }

                using (var sw = new StreamWriter(file, false))
                {
                    sw.Write(template.Replace(@"//##//", markerCode));
                }

                /*
                using (StreamWriter sw = new StreamWriter("Results.txt", true))
                {
                    sw.WriteLine(String.Format("{0} : {1} \n", kvp.Value.ToString(CultureInfo.InvariantCulture), string.Join(",", kvp.Key)));

                    try
                    {
                        sw.WriteLine(
                            String.Format(
                                "{0} : {1} \n\n",
                                kvp.Value.ToString(CultureInfo.InvariantCulture),
                                string.Join(",", kvp.Key.Select(id => properties.DataStructures.List[id][0].StopSpecName).ToList())));
                    }
                    catch
                    {
                        string why = "why?";

                    }
                }
                 */
            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        ///   Converts a list of nodes into a linked list of nodes.
        /// </summary>
        /// <param name="nodes"> The nodes to convert. </param>
        /// <returns> A node that is the head of the linked list. </returns>
        public static INetworkNode ToLinkedNodes(List<INetworkNode> nodes)
        {
            INetworkNode prev = null;
            foreach (INetworkNode node in nodes)
            {
                var newNode = (INetworkNode)node.Clone();

                if (prev == null)
                {
                    prev = newNode;
                    prev.Parent = null;
                }
                else
                {
                    newNode.Parent = prev;
                    prev = newNode;
                }
            }
            /*
            if (prev != null && prev.Parent != null && prev.TotalTime < prev.Parent.TotalTime)
            {
                throw new Exception("This should not happen");
            }
             * */

            return prev;
        }

        /// <summary>
        /// Clamps a specified value between the bounds l and u.
        /// </summary>
        /// <param name="value">The value that is to be clamped.</param>
        /// <param name="l">The lower bounds of the clamp.</param>
        /// <param name="u">The upper bounds of the clamp.</param>
        /// <returns>The clamped value.</returns>
        public static double Clamp(double value, double l = 0, double u = 1)
        {

            return Math.Min(Math.Max(value, l),u);


        }
        #endregion
    }
}