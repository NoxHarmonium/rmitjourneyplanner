// --------------------------------------------------------------------------------------------------------------------
// <copyright company="RMIT University" file="TramStop.cs">
//   Copyright RMIT University 2011
// </copyright>
// <summary>
//   Represents a stop in the Yarra Trams network.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    #region

    using System;
    using System.Data;
    using System.Text.RegularExpressions;

    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Represents a stop in the Yarra Trams network.
    /// </summary>
    [Serializable]
    public class TramStop : Location, INetworkNode
    {
        #region Constants and Fields

        /// <summary>
        ///   The id.
        /// </summary>
        private readonly string id;

        /// <summary>
        ///   The provider.
        /// </summary>
        private readonly TramNetworkProvider provider;

        /// <summary>
        ///   The city direction.
        /// </summary>
        private TransportDirection cityDirection;

        /// <summary>
        ///   The flag stop no.
        /// </summary>
        private int flagStopNo;

        /// <summary>
        ///   The has connecting buses.
        /// </summary>
        private bool hasConnectingBuses;

        /// <summary>
        ///   The has connecting trains.
        /// </summary>
        private bool hasConnectingTrains;

        /// <summary>
        ///   The has connecting trams.
        /// </summary>
        private bool hasConnectingTrams;

        /// <summary>
        ///   The internal data.
        /// </summary>
        private DataSet internalData;

        /// <summary>
        ///   The is city stop.
        /// </summary>
        private bool isCityStop;

        /// <summary>
        ///   The is platform stop.
        /// </summary>
        private bool isPlatformStop;

        /// <summary>
        ///   The parent.
        /// </summary>
        private INetworkNode parent;

        /// <summary>
        ///   The stop length.
        /// </summary>
        private double stopLength;

        /// <summary>
        ///   The stop name.
        /// </summary>
        private string stopName;

        /// <summary>
        ///   The suburb name.
        /// </summary>
        private string suburbName;

        /// <summary>
        ///   The zones.
        /// </summary>
        private string zones;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the TramStop class with a dataset retreived from the 
        ///   TramTracker API.
        /// </summary>
        /// <param name="id">
        /// The tramtracker Id.
        /// </param>
        /// <param name="provider">
        /// The NetworkProvider that contains this tram stop.
        /// </param>
        public TramStop(string id, TramNetworkProvider provider)
            : base(0, 0)
        {
            this.provider = provider;
            this.id = id;
            this.parent = null;

            // LoadData(parent, stopData);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the tram route without any direction modifiers.
        /// </summary>
        public string BaseRoute
        {
            get
            {
                try
                {
                    return this.CurrentRoute.Remove(this.CurrentRoute.IndexOf("_"));
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        ///   Gets the direction of the tram stop relative to the city.
        /// </summary>
        public TransportDirection CityDirection
        {
            get
            {
                return this.cityDirection;
            }
        }

        /// <summary>
        ///   Gets or sets the current route this node is associated with.
        /// </summary>
        public string CurrentRoute { get; set; }

        /// <summary>
        ///   Gets or sets the Euclidian distance to the goal. Used for traversing route trees.
        /// </summary>
        public double EuclidianDistance { get; set; }

        /// <summary>
        ///   Gets the stop number listed at the tram stop.
        /// </summary>
        public int FlagStopNo
        {
            get
            {
                return this.flagStopNo;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the tram stop has a connecting bus service. This variable is not very reliable.
        /// </summary>
        public bool HasConnectingBuses
        {
            get
            {
                return this.hasConnectingBuses;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the tram stop has a connecting train service. This variable is not very reliable.
        /// </summary>
        public bool HasConnectingTrains
        {
            get
            {
                return this.hasConnectingTrains;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the tram stop has a connecting tram service. This variable is not very reliable.
        /// </summary>
        public bool HasConnectingTrams
        {
            get
            {
                return this.hasConnectingTrams;
            }
        }

        /// <summary>
        ///   Gets the Tramtracker Id of this tram stop.
        /// </summary>
        public string Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        ///   Gets the internal DataSet that hold all the data associated with this node.
        /// </summary>
        public DataSet InternalData
        {
            get
            {
                return this.internalData;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the tram stop is in the city.
        /// </summary>
        public bool IsCityStop
        {
            get
            {
                return this.isCityStop;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the tram stop has a dedicated platform.
        /// </summary>
        public bool IsPlatformStop
        {
            get
            {
                return this.isPlatformStop;
            }
        }

        /// <summary>
        ///   Gets or sets the parent node to this node. Used for traversing 
        ///   route trees.
        /// </summary>
        public INetworkNode Parent
        {
            get
            {
                return this.parent;
            }

            set
            {
                this.parent = value;
            }
        }

        /// <summary>
        ///   Gets the data provider that created this object.
        /// </summary>
        public INetworkDataProvider Provider
        {
            get
            {
                return this.provider;
            }
        }

        /// <summary>
        ///   Gets the length of the tram stop (platform stops only).
        /// </summary>
        public double StopLength
        {
            get
            {
                return this.stopLength;
            }
        }

        /// <summary>
        ///   Gets the name of the tram stop.
        /// </summary>
        public string StopName
        {
            get
            {
                return this.stopName;
            }
        }

        /// <summary>
        ///   Gets the suburb name the tram stop is located in.
        /// </summary>
        public string SuburbName
        {
            get
            {
                return this.suburbName;
            }
        }

        /// <summary>
        ///   Gets or sets the total time taken to reach this node. Used for traversing 
        ///   route trees.
        /// </summary>
        public TimeSpan TotalTime { get; set; }

        /// <summary>
        ///   Gets the zone string of the tram stop.
        /// </summary>
        public string Zones
        {
            get
            {
                return this.zones;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Copies this node to a new node.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public object Clone()
        {
            var cloned = new TramStop(this.id, this.provider)
                {
                    CurrentRoute = this.CurrentRoute, 
                    TotalTime = this.TotalTime, 
                    EuclidianDistance = this.EuclidianDistance
                };
            if (this.internalData != null)
            {
                cloned.LoadData(this.internalData);
            }

            return cloned;
        }

        /// <summary>
        /// Returns if this TramStop object is equal to another object.
        /// </summary>
        /// <param name="obj">
        /// An object to compare this object with.
        /// </param>
        /// <returns>
        /// A value indicating whether the objects are equal.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is INetworkNode)
            {
                if (((INetworkNode)obj).Id == this.id && ((INetworkNode)obj).CurrentRoute == this.CurrentRoute)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Serves as a hash function for a tram stop;
        /// </summary>
        /// <returns>
        /// The get hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return (this.id + this.CurrentRoute).GetHashCode();
        }

        /// <summary>
        /// Sets the properties of a Tram Stop by loading them from a DataSet.
        /// </summary>
        /// <param name="stopData">
        /// The DataSet retreived from the Tramtracker API for the TramStop.
        /// </param>
        public void LoadData(DataSet stopData)
        {
            this.internalData = stopData;
            DataTable dataTable = stopData.Tables[0];
            DataRow dataRow = dataTable.Rows[0];
            string flagStopString = dataRow["FlagStopNo"].ToString();
            Match match = Regex.Match(flagStopString, "[a-zA-Z]");
            while (match.Success)
            {
                flagStopString = flagStopString.Remove(match.Index, 1);
                match = Regex.Match(flagStopString, "[a-zA-Z]");
            }

            this.flagStopNo = Convert.ToInt32(flagStopString);
            this.stopName = dataRow["StopName"].ToString();

            // Determine citydirection
            string sCityDirection = dataRow["CityDirection"].ToString();
            switch (sCityDirection)
            {
                case "towards City":
                    this.cityDirection = TransportDirection.TowardsCity;
                    break;

                case "from City":
                    this.cityDirection = TransportDirection.FromCity;
                    break;

                default:
                    this.cityDirection = TransportDirection.Unknown;
                    break;
            }

            this.Latitude = Convert.ToDouble(dataRow["Latitude"]);
            this.Longitude = Convert.ToDouble(dataRow["longitude"]);
            this.suburbName = dataRow["SuburbName"].ToString();
            this.isCityStop = Convert.ToBoolean(dataRow["IsCityStop"]);
            this.hasConnectingBuses = Convert.ToBoolean(dataRow["HasConnectingBuses"]);
            this.hasConnectingTrains = Convert.ToBoolean(dataRow["HasConnectingTrains"]);
            this.hasConnectingTrams = Convert.ToBoolean(dataRow["HasConnectingTrams"]);
            this.stopLength = Convert.ToDouble(dataRow["StopLength"]);
            this.isPlatformStop = Convert.ToBoolean(dataRow["IsPlatformStop"]);
            this.zones = dataRow["Zones"].ToString();
        }

        /// <summary>
        /// Loads the TramStop data from the provider.
        /// </summary>
        public void RetrieveData()
        {
            if (this.internalData == null)
            {
                DataSet data = this.provider.GetNodeData(this.id);
                this.LoadData(data);
                this.internalData = data;
            }
            else
            {
                this.LoadData(this.internalData);
            }
        }

        /// <summary>
        /// Returns the string representation of this class.
        /// </summary>
        /// <returns>
        /// The to string.
        /// </returns>
        public new string ToString()
        {
            return this.id;
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        /// Returns if 2 INetworkNode objects are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare to.
        /// </param>
        /// <returns>
        /// Gets a value indicating whether the 2 objects are equal.
        /// </returns>
        bool IEquatable<INetworkNode>.Equals(INetworkNode other)
        {
            if (this.id == other.Id && this.CurrentRoute == other.CurrentRoute)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}