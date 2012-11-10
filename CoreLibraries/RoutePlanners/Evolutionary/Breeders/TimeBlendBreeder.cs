// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeBlendBreeder.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   A stock standard crossover algorithm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Breeders
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
    using RmitJourneyPlanner.CoreLibraries.Types;

    using Random = RmitJourneyPlanner.CoreLibraries.Random;

    #endregion

    /// <summary>
    /// A stock standard crossover algorithm.
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
        /// Initializes a new instance of the <see cref="TimeBlendBreeder"/> class. 
        ///   Initializes a new instance of the <see cref="StandardBreeder"/> class.
        /// </summary>
        /// <param name="properties">
        /// The properties. 
        /// </param>
        public TimeBlendBreeder(EvolutionaryProperties properties)
        {
            this.properties = properties;
        }

        #endregion

        #region Public Methods and Operators

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
            var random = Random.GetInstance();

            List<NodeWrapper<INetworkNode>> firstNodes = first.Route;
            List<NodeWrapper<INetworkNode>> secondNodes = second.Route;
            var crossoverPoints = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < firstNodes.Count; i++)
            {
                for (int j = 0; j < secondNodes.Count; j++)
                {
                    if (firstNodes[i].Node.Equals(secondNodes[j].Node))
                    {
                        crossoverPoints.Add(new KeyValuePair<int, int>(i, j));
                        break;
                    }
                }
            }

            if (crossoverPoints.Count == 0)
            {
                // throw new Exception("StandardBreeder.cs: The crossover points are undefined.");
                // crossoverPoints.Add(new KeyValuePair<int, int>(random.Next(firstNodes.Count - 1), random.Next(secondNodes.Count - 1)));
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

            var rand = Random.GetInstance();
            double u1 = rand.NextDouble(); // these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // random no

            double xOverWeight = Tools.Clamp(0.5 + 0.5 * randStdNormal);
            long difference = Math.Abs(first.departureTime.Ticks - second.departureTime.Ticks);
            long diffTime = (long)(difference * xOverWeight);
            var newDepart = new DateTime(Math.Min(first.departureTime.Ticks, second.departureTime.Ticks) + diffTime);

            output[0].departureTime = newDepart;
            output[1].departureTime = newDepart;

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