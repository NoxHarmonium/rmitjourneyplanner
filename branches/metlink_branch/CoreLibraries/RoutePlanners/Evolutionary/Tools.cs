// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="Tools.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   A collection of static tools for use by the evolutionary route planner.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region

    using System;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    #endregion

    /// <summary>
    /// A collection of static tools for use by the evolutionary route planner.
    /// </summary>
    public static class Tools
    {
        #region Public Methods

        /// <summary>
        /// Converts a list of nodes into a linked list of nodes.
        /// </summary>
        /// <param name="nodes">
        /// The nodes to convert.
        /// </param>
        /// <returns>
        /// A node that is the head of the linked list.
        /// </returns>
        public static INetworkNode ToLinkedNodes(List<INetworkNode> nodes)
        {
            INetworkNode prev = null;
            foreach (var node in nodes)
            {
                INetworkNode newNode = (INetworkNode)node.Clone();

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

            if (prev.TotalTime < prev.Parent.TotalTime)
            {
                throw new Exception("This should not happen");
            }

            return prev;
        }

        #endregion
    }
}