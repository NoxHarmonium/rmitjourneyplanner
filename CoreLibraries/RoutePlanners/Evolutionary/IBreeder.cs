// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBreeder.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Represents a class which crosses over 2 routes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region Using Directives

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Represents a class which crosses over 2 routes.
    /// </summary>
    public interface IBreeder
    {
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
        Critter[] Crossover(Critter first, Critter second);

        #endregion
    }
}