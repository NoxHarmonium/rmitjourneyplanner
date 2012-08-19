// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink
{
    #region

    using System;
    using System.Data;
    using System.Globalization;

    using RmitJourneyPlanner.CoreLibraries.Positioning;
	using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   Represents a stop in the Metlink network.
    /// </summary>
    public class MetlinkNode : Location, INetworkNode
    {
        #region Constants and Fields

        /// <summary>
        ///   The Metlink stop id for this stop.
        /// </summary>
        private readonly int id;

        /// <summary>
        ///   The provider that services this node.
        /// </summary>
        private readonly INetworkDataProvider provider;

        /// <summary>
        ///   Holds all the data associated with this stop.
        /// </summary>
        private DataTable stopData;

        /// <summary>
        ///   The user friendly name of this node.
        /// </summary>
        private string stopSpecName = string.Empty;
		
		/// <summary>
		/// The line identifiers.
		/// </summary>
		private int[] lineIds = null;
        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="MetlinkNode" /> class. Sets the location of this node to (0,0).
        /// </summary>
        /// <param name="id"> The tramtracker Id. </param>
        /// <param name="provider"> The NetworkProvider that contains this tram stop. </param>
        public MetlinkNode(int id, INetworkDataProvider provider)
            : base(0, 0)
        {
            this.provider = provider;
            this.id = id;
            this.Parent = null;

            this.RetrieveData();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MetlinkNode" /> class. Sets the location of this node to position.
        /// </summary>
        /// <param name="id"> The tramtracker Id. </param>
        /// <param name="provider"> The NetworkProvider that contains this tram stop. </param>
        /// <param name="position"> The position of the node. </param>
        public MetlinkNode(int id, INetworkDataProvider provider, Location position)
            : base(position.Latitude, position.Longitude)
        {
            this.provider = provider;
            this.id = id;
            this.Parent = null;

            // LoadData(parent, stopData);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MetlinkNode" /> class. Sets the location of this node to position.
        /// </summary>
        /// <param name="id"> The tramtracker Id. </param>
        /// <param name="transportType"> </param>
        /// <param name="latitude"> </param>
        /// <param name="longitude"> </param>
        /// <param name="provider"> The NetworkProvider that contains this tram stop. </param>
        public MetlinkNode(
            int id, TransportMode transportType, double latitude, double longitude, INetworkDataProvider provider)
            : base(latitude, longitude)
        {
            this.provider = provider;
            this.id = id;
            this.Parent = null;
            this.TransportType = transportType;

            // LoadData(parent, stopData);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MetlinkNode" /> class. Sets the location of this node to position.
        /// </summary>
        /// <param name="id"> The tramtracker Id. </param>
        /// <param name="stopSpecName">The short name of the node.</param>
        /// <param name="transportType"> </param>
        /// <param name="latitude"> </param>
        /// <param name="longitude"> </param>
        /// <param name="provider"> The NetworkProvider that contains this tram stop. </param>
        public MetlinkNode(
            int id, TransportMode transportType, string stopSpecName, double latitude, double longitude, INetworkDataProvider provider)
            : base(latitude, longitude)
        {
            this.provider = provider;
            this.id = id;
            this.Parent = null;
            this.TransportType = transportType;
            this.stopSpecName = stopSpecName;

            // LoadData(parent, stopD
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the route that this node belongs to (when retrieved with <see cref="MetlinkDataProvider.GetNodeClosestToPointWithinArea"/> ).
        /// </summary>
        [Obsolete]
        public int RouteId { get; set; }
		
		/// <summary>
		/// Gets the line ids associated with this route.
		/// </summary>
		/// <value>
		/// The line identifiers.
		/// </value>
		public int[] LineIds {
			get
			{
				return this.lineIds;
			}
		}
		

     
        /// <summary>
        ///   Gets a unique identifier for this node inside of it's network.
        /// </summary>
        public int Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        ///   Gets or sets the parent node to this node. Used for traversing route trees.
        /// </summary>
        public INetworkNode Parent { get; set; }

        /// <summary>
        ///   Gets the DataProvider that the node belongs to.
        /// </summary>
        public INetworkDataProvider Provider
        {
            get
            {
                return this.provider;
            }
        }

        /// <summary>
        ///   The user friendly name of this node.
        /// </summary>
        public string StopSpecName
        {
            get
            {
                return this.stopSpecName;
            }
        }

       
        /// <summary>
        ///   Gets or sets the type of transport this node services.
        /// </summary>
        public Types.TransportMode TransportType { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns> A new object that is a copy of this instance. </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            var newNode = new MetlinkNode(this.id, this.provider, this)
            {
                //EuclidianDistance = this.EuclidianDistance,
                //TotalTime = this.TotalTime,
                //RouteId = this.RouteId,
                TransportType = this.TransportType,
                stopSpecName = this.StopSpecName,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Parent = this.Parent, 
                stopData = this.stopData,
				lineIds = this.lineIds
            };

            newNode.LoadData(this.stopData);

            return newNode;
        }
        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns> true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false. </returns>
        /// <param name="other"> An object to compare with this object. </param>
        public bool Equals(INetworkNode other)
        {
            if (other == null)
            {
                return false;
            }
            return this.id == other.Id && other is MetlinkNode;
        }

        public override bool Equals(object obj)
        {
            var metlinkNode = obj as MetlinkNode;
            if (metlinkNode != null)
            {
                return (metlinkNode).id == this.id;
            }
            
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.id;
        }

        bool IEquatable<INetworkNode>.Equals(INetworkNode other)
        {
            return this.Equals(other);
        }

        /// <summary>
        ///   Sets the internal parameters of this node from a data table.
        /// </summary>
        /// <param name="data"> </param>
        public void LoadData(DataTable data)
        {
            if (data != null)
            {
                this.stopData = data;
                this.Latitude = Convert.ToDouble(data.Rows[0]["GPSLat"]);
                this.Longitude = Convert.ToDouble(data.Rows[0]["GPSLong"]);
                this.stopSpecName = data.Rows[0]["StopSpecName"].ToString();
				TransportMode mode;
                bool success = Enum.TryParse<TransportMode>(data.Rows[0]["StopModeName"].ToString(),out mode);
				if (!success)
				{
					this.TransportType = RmitJourneyPlanner.CoreLibraries.Types.TransportMode.Unknown;
				}
				else
				{
					this.TransportType = mode;	
				}
            }
        }

        /// <summary>
        ///   Loads the properties of the node from the parent.
        /// </summary>
        public void RetrieveData()
        {
            if (this.stopData != null)
            {
                return;
            }

            this.LoadData(this.provider.GetNodeData(this.id));
        }

        /// <summary>
        ///   Returns the Metlink ID of this node.
        /// </summary>
        /// <returns> The ID. </returns>
        public override string ToString()
        {
            if (stopSpecName == String.Empty)
            {
                return string.Format("{0}", this.Id);
            }
            
            return String.Format("[MetlinkNode id: {0} - {1}]", this.id, this.stopSpecName);
            
            
        }

        #endregion

        Types.INode Types.INode.Parent
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