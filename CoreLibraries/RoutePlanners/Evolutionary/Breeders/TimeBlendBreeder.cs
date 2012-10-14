// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Breeders
{
    #region

    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   A stock standard crossover algorithm.
    /// </summary>
    public class TimeBlendBreeder : IBreeder
    {
        #region Constants and Fields

        /// <summary>
        ///   The properties.
        /// </summary>
        private readonly EvolutionaryProperties properties;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="StandardBreeder" /> class.
        /// </summary>
        /// <param name="properties"> The properties. </param>
        public TimeBlendBreeder (EvolutionaryProperties properties)
        {
            this.properties = properties;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Applies crossover to 2 parents to create a child.
        /// </summary>
        /// <param name="first"> The first parent of the crossover. </param>
        /// <param name="second"> The second parent of the crossover. </param>
        /// <returns> If the operation is successful then the result is returned, otherwise null. </returns>
        public Critter[] Crossover(Critter first, Critter second)
        {
            var random = CoreLibraries.Random.GetInstance();

            List<NodeWrapper<INetworkNode>> firstNodes = first.Route;
            List<NodeWrapper<INetworkNode>> secondNodes = second.Route;
            var crossoverPoints = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < firstNodes.Count; i++)
            {
                for (int j = 0; j < secondNodes.Count; j++)
                {
                    if (firstNodes[i].Node.Equals(secondNodes[j].Node) && !(firstNodes[i].Node is TerminalNode)
                        && !(secondNodes[j].Node is TerminalNode))
                    {
                        crossoverPoints.Add(new KeyValuePair<int, int>(i, j));
                        break;
                    }
                }
            }

            if (crossoverPoints.Count == 0)
            {
                //throw new Exception("StandardBreeder.cs: The crossover points are undefined.");
                //crossoverPoints.Add(new KeyValuePair<int, int>(random.Next(firstNodes.Count - 1), random.Next(secondNodes.Count - 1)));
                return null;
            }

            var firstChild = new Route(-1);
            var secondChild = new Route(-1);
            KeyValuePair<int, int> crossoverPoint = crossoverPoints[random.Next(crossoverPoints.Count - 1)];

            firstChild.AddRange(firstNodes.GetRange(0, crossoverPoint.Key));
            firstChild.AddRange(secondNodes.GetRange(crossoverPoint.Value, secondNodes.Count - crossoverPoint.Value));

            secondChild.AddRange(secondNodes.GetRange(0, crossoverPoint.Value));
            secondChild.AddRange(firstNodes.GetRange(crossoverPoint.Key, firstNodes.Count - crossoverPoint.Key));

          	
            var output = new[]
                {
                    new Critter((Route)firstChild.Clone(), new Fitness()),
                    new Critter((Route)secondChild.Clone(), new Fitness())
                };

            output[0].departureTime = new DateTime((first.departureTime.Ticks + second.departureTime.Ticks) / 2);
            output[1].departureTime = new DateTime((first.departureTime.Ticks + second.departureTime.Ticks) / 2);

            Assert.That(output[0].departureTime != default(DateTime));
            Assert.That(output[1].departureTime != default(DateTime));
			
            if (output == null || output[0] == null || output[1] == null)
            {
                throw new Exception("StandardBreeder.cs: One or more decendants of crossover are null.");
            }
            return output;
        }

        #endregion
    }
}