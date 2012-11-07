// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMutator.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Interface that represents a gene mutator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary
{
    #region Using Directives

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Interface that represents a gene mutator.
    /// </summary>
    public interface IMutator
    {
        #region Public Methods and Operators

        /// <summary>
        /// Mutate the specified critter and return the result
        /// </summary>
        /// <param name="critter">
        /// The critter. 
        /// </param>
        /// <returns>
        /// A mutated critter. 
        /// </returns>
        Critter Mutate(Critter critter);

        #endregion
    }
}