// -----------------------------------------------------------------------
// <copyright file="State.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace JRPCServer.cs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A class the represents the state of a RPC Server. 
    /// </summary>
    public class State
    {
        public State()
        {
            LogEventHooked = false;
        }

        public bool LogEventHooked { get; set; }
    }
}
