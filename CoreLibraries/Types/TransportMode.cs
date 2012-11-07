// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransportMode.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Defines the transport mode used.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    /// <summary>
    /// Defines the transport mode used.
    /// </summary>
    public enum TransportMode
    {
        /// <summary>
        ///   The transport mode is walking.
        /// </summary>
        Walking = 0, 

        /// <summary>
        ///   The transport mode is driving.
        /// </summary>
        Driving = 1, 

        /// <summary>
        ///   The transport mode is cycling.
        /// </summary>
        Bicycling = 2, 

        /// <summary>
        ///   The train.
        /// </summary>
        Train = 3, 

        /// <summary>
        ///   The bus.
        /// </summary>
        Bus = 4, 

        /// <summary>
        ///   The tram.
        /// </summary>
        Tram = 5, 

        /// <summary>
        ///   The unknown.
        /// </summary>
        Unknown = 6
    }
}