// -----------------------------------------------------------------------
// <copyright file="Timetable.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
	using CoreLibraries.Logging;

    /// <summary>
    /// Represents a public transport timetable.
    /// </summary>
    [DataContract]
    public class Timetable 
    {
        /// <summary>
        /// The index number of the stop Id in the entries list.
        /// </summary>
        [DataMember(Order = 1)]
        private const int StopIdIndex= 0;

        /// <summary>
        /// The index number of the route Id in the entries list.
        /// </summary>
        [DataMember(Order = 2)]
        private const int RouteIDIndex = 1;
        /// <summary>
        /// The index number of the day of week in the entries list.
        /// </summary>
        [DataMember(Order = 3)]
        private const int dayOfWeekIndex = 2;
        /// <summary>
        /// The index number of the arrival time in the entries list.
        /// </summary>
        [DataMember(Order = 4)]
        private const int arrivalTimeIndex = 3;
        /// <summary>
        /// The index number of the departure time in the entries list.
        /// </summary>
        [DataMember(Order = 5)]
        private const int departureTimeIndex = 4;
		
		/// <summary>
        /// The index number of the serviceId in the entries list.
        /// </summary>
        [DataMember(Order = 6)]
        private const int serviceIdIndex = 5;

        /// <summary>
        /// The index number of the order in the entries list.
        /// </summary>
        [DataMember(Order = 7)]
        private const int orderIndex = 6;

        /// <summary>
        /// The total number of parameters used in the timetable.
        /// </summary>
        private const int paramCount = 7;
        
        /// <summary>
        /// Stores the raw timetable entries.
        /// </summary>
        [DataMember(Order = 6)]
        private List<int[]> entries = new List<int[]>();

        /// <summary>
        /// The data structure containing the timetable data.
        /// </summary>
        [DataMember(Order = 7)]
        readonly private Dictionary<int, Dictionary<int, Dictionary<int, List<int[]>>>> dataStructure = new Dictionary<int, Dictionary<int, Dictionary<int, List<int[]>>>>();

        /// <summary>
        /// A value representing if the timetable has been optimised or not.
        /// </summary>
        [DataMember(Order = 8)]
        private bool optimised = false;

        /// <summary>
        /// A value representing if the timetable has been optimised or not.
        /// </summary>
        public bool Optimised
        {
            get
            {
                return this.optimised;
            }
        }

        /// <summary>
        /// Add a raw timetable entry into the timetable.
        /// </summary>
        /// <param name="stopId">The stop identifier.</param>
        /// <param name="arrivalTime">The arrival time at this stop.</param>
        /// <param name="departureTime">The departure time from this stop.</param>
        /// <param name="routeId">The route associated with this stop.</param>
        /// <param name="dayOfWeek">The binary representation of which days the service runs on.</param>
        public void AddTimetableEntry(int stopId, int routeId, int dayOfWeek, int arrivalTime, int departureTime,int serviceId, int order)
        {
            entries.Add(new[] {stopId,routeId,dayOfWeek,arrivalTime,departureTime,serviceId, order} );
        }

        /// <summary>
        /// Compress the timetable structure by finding periodicity in
        /// the timetable data and store in a high speed structure.
        /// This should be run after all the timetable entries have been 
        /// added.
        /// </summary>
        public void Optimise()
        {
            int currentStopId = entries[0][StopIdIndex];
            int currentRouteId = entries[0][RouteIDIndex];
            int currentDOW = entries[0][dayOfWeekIndex];

            var times = new List<int[]>();
            var dowToTimes = new Dictionary<int, List<int[]>>();
            var routeToDOW = new Dictionary<int, Dictionary<int, List<int[]>>>();
            //var stopToRoute = new Dictionary<int, Dictionary<int, Dictionary<int, int[]>>>();

            //int index = 0;
            for (int index = 0; index < entries.Count; index++)
            //foreach (var entry in entries)
            {
                if ((index % 5000) == 0)
				{
					CoreLibraries.Logging.Logger.Log(this,"Optimising {0} of {1}",index,entries.Count);
				}
				
				var entry = entries[index];

                if (currentDOW != entry[dayOfWeekIndex] || currentRouteId != entry[RouteIDIndex]  || currentStopId != entry[StopIdIndex])
                {
                    dowToTimes.Add(currentDOW, times);
                    times = new List<int[]>();
                    currentDOW = entry[dayOfWeekIndex];

                }

                if (currentRouteId != entry[RouteIDIndex] || currentStopId != entry[StopIdIndex])
                {
                    routeToDOW.Add(currentRouteId, dowToTimes);
                    dowToTimes = new Dictionary<int, List<int[]>>();
                    currentRouteId = entry[RouteIDIndex];
                }

                if (currentStopId != entry[StopIdIndex])
                {
                    dataStructure.Add(currentStopId, routeToDOW);
                    routeToDOW = new Dictionary<int, Dictionary<int, List<int[]>>>();
                    currentStopId = entry[StopIdIndex];
                }
                times.Add(new[]{entry[arrivalTimeIndex], entry[departureTimeIndex],entry[serviceIdIndex],entry[orderIndex]});
				
				entries[index] = null;
            }
            
          entries.Clear();
            
        }

        /// <summary>
        /// Gets the route identifiers associated with the specified
        /// stop.
        /// </summary>
        /// <param name="stopId"></param>
        /// <returns></returns>
        public int[] GetRoutes(int stopId)
        {
            return dataStructure[stopId].Keys.ToArray();
        }

        /// <summary>
        /// Saves the unoptimised data to a text file for caching.
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {

            int count = 0;
            using (var writer = new BinaryWriter(new FileStream(filename,FileMode.Create)))
            {
                writer.Write(entries.Count);
                foreach (var intArray in entries)
                {
                    var result = new byte[intArray.Length * sizeof(int)];
                    Buffer.BlockCopy(intArray, 0, result, 0, result.Length);
                    writer.Write(result);
                    count++;

                    if ((count % 10000)== 0)
                    {
                        Logger.Log(this,"Saving row number {0}...", count);

                    }
                }
            }
          
        }

        /// <summary>
        /// Loads unoptimised data from a cached text file.
        /// </summary>
        /// <param name="filename"></param>
        public void Load(string filename)
        {
            int count = 0;
            using (var reader = new BinaryReader(new FileStream(filename, FileMode.Open)))
            {
                int length = reader.ReadInt32();
                entries = new List<int[]>(length+1);
                byte[] row;
                

                while ((row = reader.ReadBytes(paramCount * sizeof(int))).Length > 0)
                {
                    var intArray = new int[paramCount];
                    Buffer.BlockCopy(row, 0, intArray, 0, intArray.Length*sizeof(int));
                    entries.Add(intArray);
                    count++;

                    if ((count % 10000) == 0)
                    {
                        Logger.Log(this, "Loading row number {0} out of {1}...", count,length);

                    }
                }
            }

            this.Optimise();

        }

        /// <summary>
        /// Gets a list of arrival and departure times given a
        /// stop identifier, a day of the week and a current time.
        /// </summary>
        /// <param name="stopId">The stop to query for.</param>
        /// <param name="dayOfWeek">The binary representation of what day of the week it is.</param>
        /// <param name="time">The minimum departure time.</param>
        /// <returns></returns>
        public Departure[] GetDepartures(int stopId, int dayOfWeek, int time)
        {
            Dictionary<int, Dictionary<int, List<int[]>>> routes;
            
            if (dataStructure.ContainsKey(stopId))
            {
                routes = dataStructure[stopId];
            }
            else
            {
                return new Departure[0];;
            }
            /*
            return (from route in routes 
                    from dow in route.Value 
                    where (dow.Key & dayOfWeek) != 0 
                    let times = dow.Value 
                    let minTime = times.First(timePair => timePair.Key > time) 
                    select new Departure { arrivalTime = minTime.Key, 
                        departureTime = minTime.Value, 
                        routeId = route.Key, 
                        stopId = stopId }
                     ).ToArray();
             * 
             */
            var departures = new List<Departure>();

            foreach (var route in routes)
            {
                foreach (var dow in route.Value)
                {
                    bool wrapped = false;

                    if((dow.Key & dayOfWeek) != 0 )
                    {
                        int[] minTime = dow.Value.FirstOrDefault(timePair => timePair[0] >= time || timePair[1] >= time);
                        if (minTime == null)
                        {
                            continue;
                        }

                        departures.Add(new Departure
                        {
                            arrivalTime = minTime[0],
                            departureTime = minTime[1],
                            routeId = route.Key,
                            stopId = stopId,
                            serviceId = minTime[2],
                            order = minTime[3]

                        });

                    }


                }


            }
            return departures.OrderBy(t => t.departureTime).ToArray();
        }

        /// <summary>
        /// Gets a list of arrival times given a
        /// stop identifier, a day of the week and a current time.
        /// </summary>
        /// <param name="stopId">The stop to query for.</param>
        /// <param name="dayOfWeek">The binary representation of what day of the week it is.</param>
        /// <param name="time">The minimum departure time.</param>
        /// <returns></returns>
        public Departure[] GetArrivals(int stopId, int serviceId)
        {
            Dictionary<int, Dictionary<int, List<int[]>>> routes;

            if (dataStructure.ContainsKey(stopId))
            {
                routes = dataStructure[stopId];
            }
            else
            {
                return new Departure[0]; ;
            }
            /*
            return (from route in routes 
                    from dow in route.Value 
                    where (dow.Key & dayOfWeek) != 0 
                    let times = dow.Value 
                    let minTime = times.First(timePair => timePair.Key > time) 
                    select new Departure { arrivalTime = minTime.Key, 
                        departureTime = minTime.Value, 
                        routeId = route.Key, 
                        stopId = stopId }
                     ).ToArray();
             * 
             */
            var departures = new List<Departure>();

            foreach (var route in routes)
            {
                foreach (var dow in route.Value)
                {
                    
                int[] minTime = dow.Value.FirstOrDefault(timePair => timePair[2] == serviceId);
                if (minTime == null)
                {
                    continue;
                }

                departures.Add(new Departure
                {
                    arrivalTime = minTime[0],
                    departureTime = minTime[1],
                    routeId = route.Key,
                    stopId = stopId,
                    serviceId = minTime[2],
                    order = minTime[3]
                });

                    


                }


            }
            return departures.OrderBy(t => t.departureTime).ToArray();
        }
    }

    
}
