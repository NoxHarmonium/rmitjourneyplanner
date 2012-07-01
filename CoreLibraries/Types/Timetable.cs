﻿// -----------------------------------------------------------------------
// <copyright file="Timetable.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

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
        /// The index number of the order in the entries list.
        /// </summary>
        [DataMember(Order = 6)]
        private const int orderIndex = 5;

        
        /// <summary>
        /// Stores the raw timetable entries.
        /// </summary>
        [DataMember(Order = 6)]
        readonly private List<int[]> entries = new List<int[]>();

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
        public void AddTimetableEntry(int stopId, int routeId, int dayOfWeek, int arrivalTime, int departureTime,int order)
        {
            entries.Add(new[] {stopId,routeId,dayOfWeek,arrivalTime,departureTime,order} );
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

            int index = 0;
            
            foreach (var entry in entries)
            {
                index++;
                if (index == 102)
                {
                    string xx = "xx";

                }

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
                times.Add(new[]{entry[arrivalTimeIndex], entry[departureTimeIndex],entry[orderIndex]});
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
                    if((dow.Key & dayOfWeek) != 0 )
                    {
                       int[] minTime = dow.Value.FirstOrDefault(timePair => timePair[0] > time);
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
                            order = minTime[2]
                        });

                    }


                }


            }
            return departures.ToArray();
        }
    }

    
}
