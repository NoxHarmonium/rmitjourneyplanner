// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Extensions for various classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;

    #endregion

    /// <summary>
    /// Extensions for various classes.
    /// </summary>
    public static class Extensions
    {
        #region Constants and Fields

        /// <summary>
        ///   The shared <see cref="System.Random"/> instance which is used by this class.
        /// </summary>
        private static readonly System.Random Rnd;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref = "Extensions" /> class.
        /// </summary>
        static Extensions()
        {
            //// TODO: Use global Random (Mersenne Twister) 
            
            Rnd = Random.GetInstance();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Concatenates 2 arrays and returns the result.
        /// </summary>
        /// <typeparam name="T">
        /// The type of array to concatenate.
        /// </typeparam>
        /// <param name="arrayA">
        /// The first array.
        /// </param>
        /// <param name="arrayB">
        /// The second array.
        /// </param>
        /// <returns>
        /// The 2 arrays concatenated.
        /// </returns>
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

            if (arrayB == null)
            {
                return arrayA;
            }

            T[] result = new T[arrayA.Length + arrayB.Length];
            arrayA.CopyTo(result, 0);
            arrayB.CopyTo(result, arrayA.Length);
            return result;
        }

        /// <summary>
        /// Shuffles a list of nodes so that they are placed in random order in the list.
        /// </summary>
        /// <typeparam name="T">
        /// The type of objects to shuffle.
        /// </typeparam>
        /// <param name="list">
        /// The list of objects to shuffle.
        /// </param>
        public static void Shuffle<T>(this List<T> list)
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
        /// <param name="list">
        /// The list of nodes to sort stochastically.
        /// </param>
        public static void StochasticSort(this ICollection<NodeWrapper<INetworkNode>> list)
        {
            StochasticSort(list, 0.5);
        }

        /// <summary>
        /// Sorts nodes probabilistically so they are in a rough order.
        /// </summary>
        /// <param name="list">
        /// The list of nodes to sort stochastically.
        /// </param>
        /// <param name="randomness">
        /// A number which can affect how random the sort is. Usually a value between 0.0 and 1.0 
        /// is best where 0.0 is a standard sort without any randomness and 1.0 is very random.
        /// </param>
        public static void StochasticSort(this ICollection<NodeWrapper<INetworkNode>> list, double randomness)
        {
            var probDict = new Dictionary<NodeWrapper<INetworkNode>, double>();

            // List<INetworkNode> nodes = new List<INetworkNode>(list.Count);
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

                double pr = (double.IsNaN(dPr) || double.IsInfinity(dPr) ? 1 : dPr)
                            + (double.IsNaN(tPr) || double.IsInfinity(tPr) ? 1 : tPr);

                probDict.Add(t, pr);
                totalPr += pr;
            }

            /*
            for (var i = 0; i < probabilities.Count; i++ )
            {
                probabilities[i] /= totalPr;
            }
            */
            var pairs = (from entry in probDict orderby entry.Value descending select entry).ToList();

            var newNodes = new List<NodeWrapper<INetworkNode>>();
            var usedNodes = new List<NodeWrapper<INetworkNode>>();

            while (list.Except(usedNodes).Any())
            {
                double r = (Rnd.NextDouble() * randomness) * totalPr;
                int i;
                for (i = 0; i < pairs.Count; i++)
                {
                    r -= pairs[i].Value;
                    if (r < 0)
                    {
                        break;
                    }
                }

                NodeWrapper<INetworkNode> candidate = pairs[i].Key;
                newNodes.Add(candidate);

                // list.Remove(candidate);

                // Have to do it this way incase this function is running on a fixed array
                // and remove wont work.
                usedNodes.Add(candidate);

                totalPr -= pairs[i].Value;
                pairs.RemoveAt(i);
            }

            var listArray = list as NodeWrapper<INetworkNode>[];
            if (listArray != null)
            {
                for (int i = 0; i < listArray.Length; i++)
                {
                    listArray[i] = newNodes[i];
                }
            }
            else
            {
                var newList = (List<NodeWrapper<INetworkNode>>)list;
                newList.Clear();
                newList.AddRange(newNodes);
            }
        }

        #endregion
    }
}