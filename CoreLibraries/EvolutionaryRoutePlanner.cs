// -----------------------------------------------------------------------
// <copyright file="EvolutionaryRoutePlanner.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Finds the best route between nodes using evolutionary algorithms.
    /// </summary>
    public class EvolutionaryRoutePlanner : IRoutePlanner
    {
        void IRoutePlanner.RegisterNetworkDataProvider(DataProviders.INetworkDataProvider provider)
        {
            throw new NotImplementedException();
        }

        void IRoutePlanner.RegisterPointDataProvider(DataProviders.IPointDataProvider provider)
        {
            throw new NotImplementedException();
        }

        List<Types.Arc>[] IRoutePlanner.Solve(List<DataProviders.INetworkNode> itinerary)
        {
            throw new NotImplementedException();
        }
    }
}
