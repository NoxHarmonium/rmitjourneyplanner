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
        
        
        public void RegisterNetworkDataProvider(DataProviders.INetworkDataProvider provider)
        {
            throw new NotImplementedException();
        }

        public void RegisterPointDataProvider(DataProviders.IPointDataProvider provider)
        {
            throw new NotImplementedException();
        }

        public void Start(List<DataProviders.INetworkNode> itinerary)
        {
            throw new NotImplementedException();
        }

        public bool SolveStep()
        {
            throw new NotImplementedException();
        }

        public DataProviders.INetworkNode Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DataProviders.INetworkNode BestNode
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
