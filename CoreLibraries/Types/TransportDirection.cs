// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransportDirection.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Represents the direction the public transport is going relative to the city.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    /// <summary>
    /// Represents the direction the public transport is going relative to the city.
    /// </summary>
    public enum TransportDirection
    {
        /// <summary>
        ///   The public transport is travelling towards the city
        /// </summary>
        TowardsCity = 0, 

        /// <summary>
        ///   The public transport is travelling away from the city.
        /// </summary>
        FromCity = 1, 

        /// <summary>
        ///   The direction of the transport relitive to the city is unknown.
        /// </summary>
        Unknown = 3
    }
}