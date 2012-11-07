// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClosedRoute.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   The closed route.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    /// <summary>
    /// The closed route.
    /// </summary>
    public struct ClosedRoute
    {
        #region Constants and Fields

        /// <summary>
        ///   The end.
        /// </summary>
        public int end;

        /// <summary>
        ///   The id.
        /// </summary>
        public int id;

        /// <summary>
        ///   The start.
        /// </summary>
        public int start;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets Length.
        /// </summary>
        public int Length
        {
            get
            {
                return this.end - this.start;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("Start: {0}, End: {1}, Id: {2}", this.start, this.end, this.id);
        }

        #endregion
    }
}