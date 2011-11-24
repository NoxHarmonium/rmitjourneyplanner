// -----------------------------------------------------------------------
// <copyright file="TramNetworkProvider.cs" company="">
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
    /// Provides network data for the Yarra Trams network.
    /// </summary>
    public class TramNetworkProvider : INetworkDataProvider
    {

        
        public List<INetworkNode> GetNodesAtLocation(Positioning.Location location, double radius)
        {
            throw new NotImplementedException();
        }

        public List<INetworkNode> GetAdjacentNodes(INetworkNode node)
        {
            throw new NotImplementedException();
        }

    }
}
