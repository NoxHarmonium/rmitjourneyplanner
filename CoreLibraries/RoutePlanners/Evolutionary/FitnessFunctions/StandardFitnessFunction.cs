// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions
{
    #region

    using System;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   A stock standard fitness function based on time.
    /// </summary>
    public class StandardFitnessFunction : IFitnessFunction
    {
        #region Constants and Fields

        /// <summary>
        ///   The properties of the travel planner.
        /// </summary>
        private readonly EvolutionaryProperties properties;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="StandardFitnessFunction" /> class.
        /// </summary>
        /// <param name="properties"> The properties. </param>
        public StandardFitnessFunction(EvolutionaryProperties properties)
        {
            this.properties = properties;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Returns a value representing the fitness of the route.
        /// </summary>
        /// <param name="route"> The route the is to be evaluated. </param>
        /// <returns> A double value representing the fitness. </returns>
        public double GetFitness(Route route)
        {
            List<INetworkNode> nodes = route;
            var totalTime = new TimeSpan();

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                nodes[i].TotalTime = totalTime;
                List<Arc> arcs = this.properties.NetworkDataProviders[0].GetDistanceBetweenNodes(
                    nodes[i], nodes[i + 1], this.properties.DepartureTime + totalTime);
                if (arcs.Count > 0)
                {
                    if (i > 0 && nodes[i].CurrentRoute != nodes[i - 1].CurrentRoute)
                    {
                        totalTime = totalTime.Add(new TimeSpan(0, 0, 0, 30));
                    }

                    totalTime = totalTime.Add(arcs[0].Time.TotalTime);
                }
                else
                {
                    totalTime =
                        totalTime.Add(
                            this.properties.PointDataProviders[0].EstimateDistance(
                                (Location)nodes[i], (Location)nodes[i + 1]).Time.TotalTime);
                }
            }

            nodes[nodes.Count - 1].TotalTime = totalTime;
            return totalTime.Ticks / 100000.0;
        }

        #endregion
    }
}