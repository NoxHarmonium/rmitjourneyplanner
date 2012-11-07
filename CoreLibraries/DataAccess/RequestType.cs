﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestType.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Represents the request type for the XMLRequestor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    /// <summary>
    /// Represents the request type for the XMLRequestor class.
    /// </summary>
    internal enum RequestType
    {
        /// <summary>
        ///   Use a simple GET html request for the XML.
        /// </summary>
        /// <example>
        ///   Google Maps API.
        /// </example>
        Get = 0, 

        /// <summary>
        ///   Use a SOAP request for the XML.
        /// </summary>
        /// <example>
        ///   Tram Tracker API.
        /// </example>
        Soap = 1
    }
}