// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionInfo.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Contains the information used to connect to the internet.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataAccess
{
    #region Using Directives

    using System.Net;

    #endregion

    /// <summary>
    /// Contains the information used to connect to the internet.
    /// </summary>
    public static class ConnectionInfo
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the proxy used by internet connections in this assembly.
        /// </summary>
        ////TODO: Merge this into a unified settings class.
        public static WebProxy Proxy { get; set; }

        #endregion
    }
}