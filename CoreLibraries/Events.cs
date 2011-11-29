// -----------------------------------------------------------------------
// <copyright file="Events.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DataProviders;

    /// <summary>
    /// Represents the data produced by a next iteration event.
    /// </summary>
    public class NextIterationEventArgs : EventArgs
    {
        private INetworkNode node;
        public NextIterationEventArgs(INetworkNode currentNode)
        {
            this.node = currentNode;
        }
        public INetworkNode CurrentNode
        {
            get
            {
                return node;
            }
        }
    }
}
