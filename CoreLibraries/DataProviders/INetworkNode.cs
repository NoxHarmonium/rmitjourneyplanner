// -----------------------------------------------------------------------
// <copyright file="INetworkNode.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a node in a transport network such a train station or tram stop.
    /// </summary>
    public interface INetworkNode
    {
        /// <summary>
        /// Gets the DataProvider that the node belongs to.
        /// </summary>
        INetworkDataProvider Parent
        {
            get;
        }




    }
}
