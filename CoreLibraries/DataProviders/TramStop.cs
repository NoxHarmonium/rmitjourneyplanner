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
    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    /// <summary>
    /// Represents a stop in the Yarra Trams network.
    /// </summary>
    [Serializable]
    public class TramStop : Location, INetworkNode
    {

        private TramNetworkProvider parent;
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
        /// Initializes a new instance of the TramStop class with a dataset retreived from the 
        /// TramTracker API.
        /// </summary>
        /// <param name="parent">The NetworkProvider that contains this tram stop.</param>
        public TramStop(TramNetworkProvider parent,DataSet stopData)
            : base(0, 0)
        {
            this.parent = parent;
            DataTable dataTable = stopData.Tables[0];
            DataRow dataRow = dataTable.Rows[0];
            flagStopNo = Convert.ToInt32(dataRow["FlagStopNo"]);
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

            
            latitude = Convert.ToDouble(dataRow["Latitude"]);
            longitude = Convert.ToDouble(dataRow["longitude"]);
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
        /// Gets the data provider that created this object.
        /// </summary>
        public INetworkDataProvider Parent
        {
            get
            {
                return parent;
            }
        }


        /// <summary>
        /// Gets the Tramtracker ID of this tram stop.
        /// </summary>
        public string ID
        {
            get { return this.ID; }
        }
    }
}
