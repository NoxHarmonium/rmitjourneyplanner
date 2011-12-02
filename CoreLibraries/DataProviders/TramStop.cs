// -----------------------------------------------------------------------
// <copyright file="TramStop.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.Types;
    using System.Security;
    using System.Security.Cryptography;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    /// <summary>
    /// Represents a stop in the Yarra Trams network.
    /// </summary>
    [Serializable]
    public class TramStop : Location, INetworkNode
    {

        private string id;
        private TramNetworkProvider provider;
        private int flagStopNo;
        private string stopName;
        private TransportDirection cityDirection;
        private string suburbName;
        private bool isCityStop;
        private bool hasConnectingBuses;
        private bool hasConnectingTrams;
        private bool hasConnectingTrains;
        private double stopLength;
        private bool isPlatformStop;
        private string zones;
        private INetworkNode parent;
        private DataSet internalData = null;



        /// <summary>
        /// Initializes a new instance of the TramStop class with a dataset retreived from the 
        /// TramTracker API.
        /// </summary>
        /// <param name="id">The tramtracker ID.</param>
        /// <param name="provider">The NetworkProvider that contains this tram stop.</param>
        public TramStop(string id, TramNetworkProvider provider)
            : base(0, 0)
        {
            this.provider = provider;
            this.id = id;
            this.parent = null;
            //LoadData(parent, stopData);


        }

        /// <summary>
        /// Gets the internal DataSet that hold all the data associated with this node.
        /// </summary>
        public DataSet InternalData
        {
            get
            {
                return internalData;
            }
        }

        /// <summary>
        /// Gets or sets the total time taken to reach this node. Used for traversing 
        /// route trees.
        /// </summary>
        public TimeSpan TotalTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parent node to this node. Used for traversing 
        /// route trees.
        /// </summary>
        public INetworkNode Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }

        }
        /// <summary>
        /// Gets the stop number listed at the tram stop.
        /// </summary>
        public int FlagStopNo
        {
            get
            {
                return flagStopNo;
            }
        }
        
        /// <summary>
        /// Gets the name of the tram stop.
        /// </summary>
        public string StopName
        {
            get
            {
                return stopName;
            }
        }
       
        /// <summary>
        /// Gets the direction of the tram stop relative to the city.
        /// </summary>
        public TransportDirection CityDirection
        {
            get
            {
                return cityDirection;
            }
        }

        /// <summary>
        /// Gets the suburb name the tram stop is located in.
        /// </summary>
        public string SuburbName
        {
            get
            {
                return suburbName;
            }
        }

        /// <summary>
        /// Gets if the tram stop is in the city.
        /// </summary>
        public bool IsCityStop
        {
            get
            {
                return isCityStop;
            }
        }

        /// <summary>
        /// Gets if the tram stop has a connecting bus service. This variable is not very reliable.
        /// </summary>
        public bool HasConnectingBuses
        {
            get
            {
                return hasConnectingBuses;
            }
        }

        /// <summary>
        /// Gets if the tram stop has a connecting tram service. This variable is not very reliable.
        /// </summary>
        public bool HasConnectingTrams
        {
            get
            {
                return hasConnectingTrams;
            }
        }

        /// <summary>
        /// Gets if the tram stop has a connecting train service. This variable is not very reliable.
        /// </summary>
        public bool HasConnectingTrains
        {
            get
            {
                return hasConnectingTrains;
            }
        }
        
        /// <summary>
        /// Gets the length of the tram stop (platform stops only).
        /// </summary>
        public double StopLength
        {
            get
            {
                return stopLength;
            }
        }
       

        /// <summary>
        /// Gets if the tram stop has a dedicated platform.
        /// </summary>
        public bool IsPlatformStop
        {
            get
            {
                return isPlatformStop;
            }
        }

        /// <summary>
        /// Gets the zone string of the tram stop.
        /// </summary>
        public string Zones
        {
            get
            {
                return zones;
            }
        }


        /// <summary>
        /// Sets the properties of a Tram Stop by loading them from a DataSet.
        /// </summary>
        /// <param name="stopData">The DataSet retreived from the Tramtracker API for the TramStop.</param>
        public void LoadData(DataSet stopData)
        {

            internalData = stopData;
            DataTable dataTable = stopData.Tables[0];
            DataRow dataRow = dataTable.Rows[0];
            string flagStopString = dataRow["FlagStopNo"].ToString();
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(flagStopString, "[a-zA-Z]");
            while (match.Success)
            {
                flagStopString = flagStopString.Remove(match.Index, 1);
                match = System.Text.RegularExpressions.Regex.Match(flagStopString, "[a-zA-Z]");
            }
            flagStopNo = Convert.ToInt32(flagStopString);
            stopName = dataRow["StopName"].ToString();

            //Determine citydirection
            string sCityDirection = dataRow["CityDirection"].ToString();
            switch (sCityDirection)
            {
                case "towards City":
                    cityDirection = TransportDirection.TowardsCity;
                    break;

                case "from City":
                    cityDirection = TransportDirection.FromCity;
                    break;

                default:
                    cityDirection = TransportDirection.Unknown;
                    break;
            }


            Latitude = Convert.ToDouble(dataRow["Latitude"]);
            Longitude = Convert.ToDouble(dataRow["longitude"]);
            suburbName = dataRow["SuburbName"].ToString();
            isCityStop = Convert.ToBoolean(dataRow["IsCityStop"]);
            hasConnectingBuses = Convert.ToBoolean(dataRow["HasConnectingBuses"]);
            hasConnectingTrains = Convert.ToBoolean(dataRow["HasConnectingTrains"]);
            hasConnectingTrams = Convert.ToBoolean(dataRow["HasConnectingTrams"]);
            stopLength = Convert.ToDouble(dataRow["StopLength"]);
            isPlatformStop = Convert.ToBoolean(dataRow["IsPlatformStop"]);
            zones = dataRow["Zones"].ToString();
        }
               


        /// <summary>
        /// Loads the TramStop data from the provider.
        /// </summary>
        public void RetrieveData()
        {
            if (internalData == null)
            {
                DataSet data = provider.GetNodeData(id);
                LoadData(data);
                internalData = data;
            }
            else
            {
                LoadData(internalData);
            }
        }


       

        /// <summary>
        /// Gets the data provider that created this object.
        /// </summary>
        public INetworkDataProvider Provider
        {
            get
            {
                return provider;
            }
        }


        /// <summary>
        /// Gets or sets the Tramtracker ID of this tram stop.
        /// </summary>
        public string ID
        {
            get { return this.id; }
        }

        /// <summary>
        /// Returns if this TramStop object is equal to another object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is INetworkNode)
            {
                if (((INetworkNode)obj).ID == id && ((INetworkNode)obj).CurrentRoute == CurrentRoute)
                {
                    return true;
                }
            }

            return false;
            
        }

        /// <summary>
        /// Serves as a hash function for a tram stop;
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (id + CurrentRoute).GetHashCode();
        }

        /// <summary>
        /// Returns if 2 INetworkNode objects are equal.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IEquatable<INetworkNode>.Equals(INetworkNode other)
        {
            if (this.id == other.ID && this.CurrentRoute == other.CurrentRoute)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the string representation of this class.
        /// </summary>
        /// <returns></returns>
        public new string ToString()
        {
            return id;
        }

        /// <summary>
        /// Gets or sets the current route this node is associated with.
        /// </summary>
        public string CurrentRoute
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the Euclidian distance to the goal. Used for traversing route trees.
        /// </summary>
        public double EuclidianDistance
        {
            get;
            set;
        }


        /// <summary>
        /// Copies this node to a new node.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            TramStop cloned = new TramStop(this.id, provider);
            cloned.CurrentRoute = this.CurrentRoute;
            cloned.TotalTime = this.TotalTime;
            cloned.EuclidianDistance = this.EuclidianDistance;
            cloned.LoadData(this.internalData);

            return cloned;

        }


        /// <summary>
        /// Gets the tram route without any direction modifiers.
        /// </summary>
        public string BaseRoute
        {
            get
            {
                try
                {
                    return CurrentRoute.Remove(CurrentRoute.IndexOf("_"));
                }
                catch (Exception)
                {
                    return String.Empty;
                }
            }
        }
    }

   
}
