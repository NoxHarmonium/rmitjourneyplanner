// -----------------------------------------------------------------------
// <copyright file="Class1.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    /// <summary>
    /// Extensions for various classes.
    /// </summary>
    public static class Extensions
    {
        private static readonly Random Rnd;

        static Extensions()
        {
            Rnd = new Random();
        }
        
        /// <summary>
        /// Concatenates 2 arrays and returns the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayA"></param>
        /// <param name="arrayB"></param>
        /// <returns></returns>
        
        public static T[] Concatenate<T>(this T[] arrayA, T[] arrayB)
        {
            if (arrayA == null && arrayB == null)
            {
                return null;
            }
            
            if (arrayA == null)
            {
                return arrayB;
            }
            
            if(arrayB == null )
            {
                return arrayA;
            }

            T[] result = new T[arrayA.Length + arrayB.Length];
            arrayA.CopyTo(result,0);
            arrayB.CopyTo(result,arrayA.Length);
            return result;

        }
        
        /// <summary>
        /// Sorts nodes probabilistically so they are in a rough order. 
        /// </summary>
        /// <param name="list"></param>
        public static void StochasticSort(this List<INetworkNode> list)
        {
          
            var pDict = new Dictionary<INetworkNode, double>();
            //List<INetworkNode> nodes = new List<INetworkNode>(list.Count);
            double totalDistance = 0;
            TimeSpan totalTime = default(TimeSpan);
            foreach (INetworkNode node in list)
            {
                totalDistance += node.EuclidianDistance;

                totalTime += node.TotalTime;
            }
            double totalPr = 0;

            foreach (INetworkNode t in list)
            {
                double dPr = 1.0 / (t.EuclidianDistance / totalDistance);
                double tPr = 1.0 / (t.TotalTime.TotalSeconds / totalTime.TotalSeconds);
                double pr = 
                    (Double.IsNaN(dPr) || Double.IsInfinity(dPr) ? 0: dPr) +
                    (Double.IsNaN(tPr) || Double.IsInfinity(tPr) ? 0: tPr);
              
                pDict.Add(t, pr);
                totalPr += pr;
            }
            /*
            for (var i = 0; i < probabilities.Count; i++ )
            {
                probabilities[i] /= totalPr;
            }
            */
            var pairs = new List<KeyValuePair<INetworkNode, double>>();
            foreach (var kvp in (from entry in pDict orderby entry.Value descending select entry))
            {
                pairs.Add(kvp);
            }

           

            var newNodes = new List<INetworkNode>();

            while (list.Count > 0)
            {
                double r = (Rnd.NextDouble() * Rnd.NextDouble()) * totalPr;
                int i;
                for (i = 0; i < pairs.Count; i++)
                {
                    r -= pairs[i].Value;
                    if (r < 0) break;
                }
                INetworkNode candidate = pairs[i].Key;
                newNodes.Add(candidate);
                list.Remove(candidate);
                totalPr -= pairs[i].Value;
                pairs.RemoveAt(i);
                
            }
            list.AddRange(newNodes);

        }
    }
}
