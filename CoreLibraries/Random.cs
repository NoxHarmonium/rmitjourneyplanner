// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Random.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Wraps <see cref="System.Random" /> to be thread safe. Initializes one random object per thread.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    #region Using Directives

    using System;

    using NPack;

    #endregion

    /// <summary>
    /// Wraps <see cref="System.Random"/> to be thread safe. Initializes one random object per thread.
    /// </summary>
    public class Random
    {
        #region Constants and Fields

        /// <summary>
        ///   The random.
        /// </summary>
        [ThreadStatic]
        private static MersenneTwister random;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns a thread safe version of the <see cref="System.Random"/> class.
        /// </summary>
        /// <returns>
        /// </returns>
        public static MersenneTwister GetInstance()
        {
            return random ?? (random = new MersenneTwister());
        }

        #endregion
    }
}