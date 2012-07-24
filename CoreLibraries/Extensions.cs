// -----------------------------------------------------------------------
// <copyright file="Class1.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;

    /// <summary>
    /// Extensions for various classes.
    /// </summary>
    public static class Extensions
    {
        private static readonly System.Random Rnd;

        static Extensions()
        {
            Rnd = CoreLibraries.Random.GetInstance();
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
        /// Shuffles a list of nodes so that they are placed in random order in the list.
        /// </summary>
        /// <param name="list"></param>
        public static void Shuffle<T>(this List<T> list )
        {
            var newList = new List<T>();
            while (list.Any())
            {
                int index = Rnd.Next(list.Count - 1);
                newList.Add(list[index]);
                list.RemoveAt(index);
            }
            list.AddRange(newList);
        }

       

        /// <summary>
        /// Sorts nodes probabilistically so they are in a rough order. 
        /// </summary>
        /// <param name="list"></param>
        public static void StochasticSort(this ICollection<NodeWrapper<INetworkNode>> list)
        {
            StochasticSort(list, 0.5);
        }

         /// <summary>
        /// Sorts nodes probabilistically so they are in a rough order. 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="randomness"></param>
        public static void StochasticSort(this ICollection<NodeWrapper<INetworkNode>> list, double randomness)
        {
            var pDict = new Dictionary<NodeWrapper<INetworkNode>, double>();
            //List<INetworkNode> nodes = new List<INetworkNode>(list.Count);
            double totalHeuristic = 0;
            double totalCost = 0;
            foreach (NodeWrapper<INetworkNode> wrapper in list)
            {
                totalHeuristic += wrapper.EuclidianDistance;

                totalCost += wrapper.Cost;
            }
            double totalPr = 0;

            foreach (NodeWrapper<INetworkNode> t in list)
            {
                double dPr = 1.0 / (t.EuclidianDistance / totalHeuristic);
                double tPr = 1.0 / (t.Cost / totalCost);

                double pr =
                    (Double.IsNaN(dPr) || Double.IsInfinity(dPr) ? 1 : dPr) +
                    (Double.IsNaN(tPr) || Double.IsInfinity(tPr) ? 1 : tPr);

                pDict.Add(t, pr);
                totalPr += pr;
            }
            /*
            for (var i = 0; i < probabilities.Count; i++ )
            {
                probabilities[i] /= totalPr;
            }
            */
            var pairs = (from entry in pDict orderby entry.Value ascending select entry).ToList();

            var newNodes = new List<NodeWrapper<INetworkNode>>();
            var usedNodes = new List<NodeWrapper<INetworkNode>>();

            while ((list.Except(usedNodes)).Any())
            {
                double r = (Rnd.NextDouble() * randomness) * totalPr;
                int i;
                for (i = 0; i < pairs.Count; i++)
                {
                    r -= pairs[i].Value;
                    if (r < 0) break;
                }
                NodeWrapper<INetworkNode> candidate = pairs[i].Key;
                newNodes.Add(candidate);
                //list.Remove(candidate);

                // Have to do it this way incase this function is running on a fixed array
                // and remove wont work.
                usedNodes.Add(candidate);


                totalPr -= pairs[i].Value;
                pairs.RemoveAt(i);

            }


            var lArray = list as NodeWrapper<INetworkNode>[];
            if (lArray != null)
            {
                for (int i = 0; i < lArray.Length; i++)
                {
                    lArray[i] = newNodes[i];
                }
            }
            else
            {
                var lList = (List<NodeWrapper<INetworkNode>>)list;
                lList.Clear();
                lList.AddRange(newNodes);
            }

        }

       
    }
}
