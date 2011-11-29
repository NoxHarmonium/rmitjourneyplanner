// -----------------------------------------------------------------------
// <copyright file="TerminalNode.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Positioning;
    using Types;
    using DataProviders;

    /// <summary>
    /// A special class that is used to define terminal points. 
    /// Used to make mixing nodes and location points easier, 
    /// basically a wrapper for a Location class.
    /// </summary>
    public class TerminalNode : Location, INetworkNode
    {
        private string id = String.Empty;

        /// <summary>
        /// Initilizes a new TerminalNode object with a specific location.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public TerminalNode(string id, double latitude, double longitude) : base (latitude,longitude)
        {
            this.id = id;
        }

        /// <summary>
        /// Initilizes a new TerminalNode object with a location string.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public TerminalNode(string id, string locationString) 
            : base (locationString)
        {
            this.id = id;
        }
        
        /// <summary>
        /// Get the parent of the terminal node. 
        /// This is always null.
        /// </summary>
        public INetworkDataProvider Provider
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Get the user-defined ID of this terminal node.
        /// </summary>
        public string ID
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// This method does nothing in a terminal node.
        /// </summary>
        public void RetrieveData()
        {
            
        }

        public override bool Equals(object obj)
        {
            if (obj is INetworkNode)
            {
                if (((INetworkNode)obj).ID == id)
                {
                    return true;
                }
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public bool Equals(INetworkNode other)
        {
            return this.Equals((object)other);
        }


        public string routeId
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public string CurrentRoute
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
