// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Extensions for various classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    #region Using Directives

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
        ///   The rnd.
        /// </summary>
        private static readonly System.Random Rnd;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref = "Extensions" /> class.
        /// </summary>
        static Extensions()
        {
            Rnd = Random.GetInstance();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Concatenates 2 arrays and returns the result.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="arrayA">
        /// </param>
        /// <param name="arrayB">
        /// </param>
        /// <returns>
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
        /// </typeparam>
        /// <param name="list">
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
        /// </param>
        public static void StochasticSort(this ICollection<NodeWrapper<INetworkNode>> list)
        {
            StochasticSort(list, 0.5);
        }

        /// <summary>
        /// Sorts nodes probabilistically so they are in a rough order.
        /// </summary>
        /// <param name="list">
        /// </param>
        /// <param name="randomness">
        /// </param>
        public static void StochasticSort(this ICollection<NodeWrapper<INetworkNode>> list, double randomness)
        {
            var pDict = new Dictionary<NodeWrapper<INetworkNode>, double>();

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

                pDict.Add(t, pr);
                totalPr += pr;
            }

            /*
            for (var i = 0; i < probabilities.Count; i++ )
            {
                probabilities[i] /= totalPr;
            }
            */
            var pairs = (from entry in pDict orderby entry.Value descending select entry).ToList();

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

        #endregion
    }
}