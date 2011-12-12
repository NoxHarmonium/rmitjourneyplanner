// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="StandardBreeder.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   A stock standard crossover algorithm.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Breeders
{
    #region

    using System;
    using System.Collections.Generic;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// A stock standard crossover algorithm.
    /// </summary>
    public class StandardBreeder : IBreeder
    {
        #region Constants and Fields

        /// <summary>
        /// The properties.
        /// </summary>
        private readonly EvolutionaryProperties properties;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardBreeder"/> class.
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        public StandardBreeder(EvolutionaryProperties properties)
        {
            this.properties = properties;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Applies crossover to 2 parents to create a child.
        /// </summary>
        /// <param name="first">
        /// The first parent of the crossover.
        /// </param>
        /// <param name="second">
        /// The second parent of the crossover.
        /// </param>
        /// <returns>
        /// If the operation is successful then the result is returned, otherwise null.
        /// </returns>
        public Critter[] Crossover(Critter first, Critter second)
        {
            Random random = new Random();

            var firstNodes = first.Route.GetNodes(true);
            var secondNodes = second.Route.GetNodes(true);
            var crossoverPoints = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < firstNodes.Count; i++)
            {
                for (int j = 0; j < secondNodes.Count; j++)
                {
                    if (firstNodes[i].Equals(secondNodes[j]) && !(firstNodes[i] is TerminalNode)
                        && !(secondNodes[j] is TerminalNode))
                    {
                        crossoverPoints.Add(new KeyValuePair<int, int>(i, j));
                        break;
                    }
                }
            }

            if (crossoverPoints.Count == 0)
            {
                return null;
            }

            var firstChild = new Route(Guid.NewGuid().ToString());
            var secondChild = new Route(Guid.NewGuid().ToString());
            var crossoverPoint = crossoverPoints[random.Next(crossoverPoints.Count - 1)];

            firstChild.AddNodeRange(firstNodes.GetRange(0, crossoverPoint.Key), true);
            firstChild.AddNodeRange(
                secondNodes.GetRange(crossoverPoint.Value, secondNodes.Count - crossoverPoint.Value), true);

            secondChild.AddNodeRange(secondNodes.GetRange(0, crossoverPoint.Value), true);
            secondChild.AddNodeRange(
                firstNodes.GetRange(crossoverPoint.Key, firstNodes.Count - crossoverPoint.Key), true);

            return new[]
                {
                    new Critter(firstChild, this.properties.FitnessFunction.GetFitness(firstChild)), 
                    new Critter(secondChild, this.properties.FitnessFunction.GetFitness(secondChild))
                };
        }

        #endregion
    }
}