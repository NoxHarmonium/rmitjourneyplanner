// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFitnessFunction.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Represents a function which evaluates the fitness of a route.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region Using Directives

    using System;

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Represents a function which evaluates the fitness of a route.
    /// </summary>
    public interface IFitnessFunction
    {
        #region Public Methods and Operators

        /// <summary>
        /// Returns a value representing the fitness of the route.
        /// </summary>
        /// <param name="route">
        /// The route the is to be evaluated. 
        /// </param>
        /// <returns>
        /// A double value representing the fitness. 
        /// </returns>
        Fitness GetFitness(Route route);

        /// <summary>
        /// The get fitness.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="initialDeparture">
        /// The initial departure.
        /// </param>
        /// <returns>
        /// </returns>
        Fitness GetFitness(Route route, DateTime initialDeparture);

        #endregion
    }
}