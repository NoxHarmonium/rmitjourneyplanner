// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetlinkDataProvider.cs" company="RMIT University">
//   This code is currently owned by RMIT by default until permission is recieved to licence it under a more liberal licence. 
// Except as provided by the Copyright Act 1968, no part of this publication may be reproduced, stored in a retrieval system or transmitted in any form or by any means without the prior written permission of the publisher.
// </copyright>
// <summary>
//   Providers data on the TransNET database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using NUnit.Framework;

    using RmitJourneyPlanner.CoreLibraries.DataAccess;
    using RmitJourneyPlanner.CoreLibraries.Logging;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// Providers data on the TransNET database.
    /// </summary>
    public sealed class PtvDataProvider : INetworkDataProvider, IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   The ammount of records to read in at any given time.
        /// </summary>
        private const int RecordChunk = 10000000;

        /// <summary>
        ///   The stop mode table.
        /// </summary>
        private static readonly Dictionary<string, string> StopModeTable = new Dictionary<string, string>();

        /// <summary>
        ///   The PTV database object.
        /// </summary>
        private readonly MySqlDatabase database = new MySqlDatabase();

        /// <summary>
        ///   The list.
        /// </summary>
        private readonly AdjacencyList list = new AdjacencyList();

        /// <summary>
        ///   The route map.
        /// </summary>
        private readonly Dictionary<int, List<int>> routeMap = new Dictionary<int, List<int>>();

        /// <summary>
        ///   The timetable
        /// </summary>
        private readonly Timetable timetable = new Timetable();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref = "PtvDataProvider" /> class.
        /// </summary>
        static PtvDataProvider()
        {
            StopModeTable.Add("1", "Bus");
            StopModeTable.Add("2", "Train");
            StopModeTable.Add("3", "Tram");
            StopModeTable.Add("4", "V/Line Coach");
            StopModeTable.Add("5", "V/Line Train");
            StopModeTable.Add("6", "Regional Bus");
            StopModeTable.Add("7", "Taxi");
            StopModeTable.Add("8", "School Bus");
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PtvDataProvider" /> class.
        /// </summary>
        public PtvDataProvider()
        {
            try
            {
                this.database.Open();
            }
            catch (Exception e)
            {
                Logger.Log(this, "Error opening database: " + e.Message);
                throw e;
            }
            {
                // try
                var nonNumericCharacters = new Regex(@"[^\d^\.]");
                Logger.Log(this, "Route Generator Initializing...");

                Logger.Log(this, "Building node-route map...");
                Logger.Log(this, "-->Querying this.database...");
                DataTable nodeData = null;
                int i = 0;
                do
                {
                    Logger.Log(this, "Reading records {0}-{1}", i * RecordChunk, RecordChunk);

                    /*
					nodeData = this.database.GetDataSet(
                        String.Format(@"SELECT s.ServiceID,s.RouteID, st.MetlinkStopID, st.Sequence 
                            FROM tblServices s
                            INNER JOIN tblServiceTimes st
                            ON s.ServiceID=st.ServiceID
                            ORDER BY RouteID, s.ServiceID,ArrivalTime
							LIMIT {0}, {1};",i *1000, (i*1000) + 1000));\
					*/
                    nodeData =
                        this.database.GetDataSet(
                            string.Format(
                                @"SELECT LineID, MetlinkStopID 
                            FROM tblLinesStops
                            LIMIT {0}, {1};", 
                                i * RecordChunk, 
                                RecordChunk));

                    if (nodeData != null)
                    {
                        foreach (DataRow row in nodeData.Rows)
                        {
                            var id = (int)row["MetlinkStopID"];
                            if (!this.routeMap.ContainsKey(id))
                            {
                                this.routeMap[id] = new List<int>();
                            }

                            var routeId = (int)row["LineID"];
                            if (!this.routeMap[id].Contains(routeId))
                            {
                                this.routeMap[id].Add(routeId);
                            }
                        }
                    }

                    i++;
                }
                while (nodeData != null && nodeData.Rows.Count > 0);

                // ogger.Log(this, "Building route path map...");
                Logger.Log(this, "Querying this.database...");

                /*
               nodeData =
                   this.database.GetDataSet(
                       @"SELECT RouteID, MetlinkStopID, StopOrder FROM tblStopRoutes
                   ORDER BY RouteID,StopOrder;
                   ");
               
               foreach (DataRow row in nodeData.Rows)
               {
                   var sourceRoute = (int)row["RouteID"];
                   var nodeId = (int)row["MetlinkStopID"];
                   var order = (int)row["Sequence"];
                   if (!this.routePathMap.ContainsKey(sourceRoute))
                   {
                       this.routePathMap[sourceRoute] = new Dictionary<int, int>();
                   }

                   this.routePathMap[sourceRoute][nodeId] = order;
               }
                */
                Logger.Log(this, "Reading nodes into structure...");

                Logger.Log(this, "Querying this.database...");
                nodeData =
                    this.database.GetDataSet(
                        @"SELECT si.MetlinkStopID, si.GPSLat, si.GPSLong, m.StopModeName, si.StopSpecName
                FROM tblStopInformation si
                INNER JOIN tblModes m
                ON si.StopModeID=m.StopModeID");
                foreach (DataRow row in nodeData.Rows)
                {
                    /*
                    var node = new PtvNode
                    {
                        Id = (int)row["MetlinkStopID"],
                        TransportType = row["StopModeName"].ToString(),
                        Latitude = Convert.ToSingle(row["GPSLat"]),
                        Longitude = Convert.ToSingle(row["GPSLong"]),
                    
                     * StopSpecName = row["StopSpecName"].ToString()
                    };*/
                    var stopwatch = Stopwatch.StartNew();

                    TransportMode mode;
                    bool success = Enum.TryParse(row["StopModeName"].ToString(), out mode);
                    if (!success)
                    {
                        mode = TransportMode.Unknown;
                    }

                    var node = new PtvNode(
                        (int)row["MetlinkStopID"], 
                        mode, 
                        row["StopSpecName"].ToString(), 
                        Convert.ToDouble(row["GPSLat"]), 
                        Convert.ToDouble(row["GPSLong"]), 
                        this);

                    // node.RetrieveData();

                    // Console.SetCursorPosition(0, Console.CursorTop);
                    // Console.Write("{0,10:f2}%", 100.0 * ((double)count++ / (Double)nodeData.Rows.Count));
                    this.list[node.Id] = new List<PtvNode> { node };

                    // Logger.Log(this,"-->Adding node (id: {0}) ({2}/{3})... [{1} s]", node.Id, stopwatch.ElapsedMilliseconds / 1000.0,nodeCount++,nodeData.Rows.Count);
                }

                if (File.Exists("AdjacencyCache.dat"))
                {
                    Logger.Log(this, "Loading links from cache...");
                    using (var reader = new StreamReader("AdjacencyCache.dat"))
                    {
                        string s;
                        while ((s = reader.ReadLine()) != null)
                        {
                            string[] v = s.Split(':');
                            int id = Convert.ToInt32(v[0]);
                            foreach (string value in v[1].Split(','))
                            {
                                if (value.Trim() != string.Empty)
                                {
                                    this.list[id].Add(this.list[Convert.ToInt32(value)][0]);
                                }
                            }
                        }
                    }

                    Logger.Log(this, "Done!");
                }
                else
                {
                    int totalLinks = 0;
                    Logger.Log(this, "\nBuilding route adjacencies... (Second Pass) [{0} s]");

                    /*
                    Logger.Log(this, "Querying this.database...");
                    nodeData =
                          this.database.GetDataSet(
                              @"SELECT s.ServiceID,s.RouteID, st.MetlinkStopID, st.DepartTime
                            FROM tblServices s
                            INNER JOIN tblServiceTimes st
                            ON s.ServiceID=st.ServiceID
                            WHERE st.DepartTime <> -1 AND st.ArrivalTime <> -1
                            ORDER BY RouteID,s.ServiceID, ArrivalTime,Sequence
                    ");
                    */
                    Logger.Log(this, "Querying this.database...");
                    nodeData =
                        this.database.GetDataSet(
                            @"SELECT * FROM tblStopRoutes sr
ORDER BY sr.RouteID, sr.StopOrder;
                    ");

                    int prevRowOrder = -1;
                    int prevRowId = -1;
                    for (i = 0; i < nodeData.Rows.Count - 1; i++)
                    {
                        DataRow row = nodeData.Rows[i];
                        DataRow nextRow = nodeData.Rows[i + 1];

                        // var RouteID = (int)row["RouteID"];
                        var rowId = (int)row["MetlinkStopID"];
                        var rowStopOrder =
                            Convert.ToInt32(nonNumericCharacters.Replace(row["StopOrder"].ToString(), string.Empty));
                        var rowServiceId = (int)row["RouteID"];

                        if (prevRowOrder != rowStopOrder)
                        {
                            prevRowOrder = rowStopOrder;
                            prevRowId = rowId;
                        }

                        var nextId = (int)nextRow["MetlinkStopID"];
                        var nextStopOrder =
                            Convert.ToInt32(nonNumericCharacters.Replace(nextRow["StopOrder"].ToString(), string.Empty));
                        var nextServiceId = (int)nextRow["RouteID"];

                        if ((nextStopOrder >= rowStopOrder || nextStopOrder == 0)
                            && !this.list[prevRowId].Contains(this.list[nextId][0]) && nextServiceId == rowServiceId)
                        {
                            this.list[prevRowId].Add(this.list[nextId][0]);
                            totalLinks++;
                        }
                    }

                    Logger.Log(this, "\nGenerating proximity links  ... (Third Pass)");

                    nodeData =
                        this.database.GetDataSet(
                            @"SELECT MetlinkStopID 
                    FROM tblStopInformation          
                    ");

                    int linkCount = 0;

                    // var nakedNodes = new List<PtvNode>();
                    var cycleNodes = new List<PtvNode>();
                    int count = 0;
                    foreach (DataRow node in nodeData.Rows)
                    {
                        // Console.SetCursorPosition(0, Console.CursorTop);
                        // Console.Write("{0,10:f2}%", 100.0 * ((double)count++ / (Double)nodeData.Rows.Count));
                        var id = (int)node["MetlinkStopID"];
                        double progress = (count++ / (double)nodeData.Rows.Count) * 100;
                        Logger.UpdateProgress(this, (int)progress);

                        ////TODO: Make scan distance variable.
                        PtvNode ptvNode = this.list[id][0];
                        var nodes = this.GetNodesAtLocation(
                            new Location(ptvNode.Latitude, ptvNode.Longitude), 0.5);

                        foreach (var closeNode in nodes)
                        {
                            closeNode.EuclidianDistance = GeometryHelper.GetStraightLineDistance(
                                ptvNode.Latitude, 
                                ptvNode.Longitude, 
                                closeNode.Node.Latitude, 
                                closeNode.Node.Longitude);
                        }

                        // nodes.Sort(n => n.EuclidianDistance);
                        var vistedRoutes = new HashSet<int>();

                        foreach (var closeNode in nodes.OrderBy(n => n.EuclidianDistance))
                        {
                            if (!this.InSameLine(closeNode.Node, ptvNode) && closeNode.Node.Id != ptvNode.Id
                                && !vistedRoutes.Contains(Convert.ToInt32(closeNode.CurrentRoute)))
                            {
                                vistedRoutes.Add(Convert.ToInt32(closeNode.CurrentRoute));
                                if (!this.list[id].Contains(this.list[Convert.ToInt32(closeNode.Node.Id)][0]))
                                {
                                    // Logger.Log(this,"-->Adding proximity link: id1: {0} id2: {1}... [{2} s]", PtvNode.Id, closeNode.Id, stopwatch.ElapsedMilliseconds / 1000.0);
                                    this.list[id].Add(this.list[Convert.ToInt32(closeNode.Node.Id)][0]);
                                    totalLinks++;

                                    // list[Convert.ToInt32(closeNode.Id)].Add(PtvNode);
                                    linkCount++;
                                }
                            }
                        }
                    }

                    Logger.Log(this, "\n{0} proximity links created.", linkCount);

                    // Logger.Log(this,"Writing result...");
                    // using (var sw = new StreamWriter("proxLink.csv", true))
                    // {
                    // sw.WriteLine("{0},{1}", maxDistance, linkCount);
                    // }
                    if (cycleNodes.Any())
                    {
                        Logger.Log(this, "Warning: There are {0} cyclic nodes.", cycleNodes.Count);
                    }

                    /*
                    if (nakedNodes.Any())
                    {
                        Logger.Log(this, "Warning: There are {0} naked nodes.", nakedNodes.Count);
                        Logger.Log(this, "Resolving... [{0} s]");
                        foreach (PtvNode nakedNode in nakedNodes)
                        {
                            double distance = 1.0;
                            var nodes = new List<INetworkNode>();
                            while (nodes.Count == 0)
                            {
                                nodes = this.GetNodesAtLocation(
                                    new Location(nakedNode.Latitude, nakedNode.Longitude), distance);
                                distance += 1;
                            }

                            foreach (INetworkNode networkNode in nodes)
                            {
                                networkNode.EuclidianDistance = GeometryHelper.GetStraightLineDistance(
                                    nakedNode.Latitude, nakedNode.Longitude, networkNode.Latitude, networkNode.Longitude);
                            }

                            nodes.Sort(new NodeComparer());
                            int index = 0;
                            while (nodes[++index].Id == nakedNode.Id)
                            {
                            }

                            this.list[nakedNode.Id].Add(this.list[Convert.ToInt32(nodes[index].Id)][0]);
                            totalLinks++;
                        }
                    }
                  
                    */
                    Logger.Log(this, "Resolved. [{0} s]");
                    Logger.Log(this, "Total links: {0}", totalLinks);

                    Logger.Log(this, "Saving cache file...");
                    using (var writer = new StreamWriter("AdjacencyCache.dat", false))
                    {
                        foreach (KeyValuePair<int, List<PtvNode>> kvp in this.list)
                        {
                            var sb = new StringBuilder();
                            foreach (PtvNode node in kvp.Value)
                            {
                                sb.Append(node.Id.ToString(CultureInfo.InvariantCulture) + ",");
                            }

                            writer.WriteLine(string.Format("{0}: {1}", kvp.Key, sb));
                        }
                    }
                }

                var craigAdg = this.list[20035];

                Logger.Log(this, "\nAdjacency data structure loaded successfully in {0} seconds.");

                Logger.Log(this, "Starting experimental timetable code...");

                if (!File.Exists("TimetableCache.dat"))
                {
                    Logger.Log(this, "Timetable cache non existant. Rebuilding...");
                    DataTable timetableData = new DataTable();

                    i = 0;

                    do
                    {
                        Logger.Log(
                            this, 
                            "Adding rows {0} to {1} of {2}...", 
                            i * RecordChunk, 
                            RecordChunk, 
                            timetableData.Rows.Count);
                        timetableData =
                            this.database.GetDataSet(
                                string.Format(
                                    @"SELECT st.MetlinkStopId, s.RouteID, s.DOW,  st.ArrivalTime, st.DepartTime,st.ServiceID,st.Sequence  FROM tblServiceTimes  st
	                                        INNER JOIN tblServices s
											ON s.ServiceID = st.ServiceID 
											ORDER BY MetlinkStopID, RouteId,DOW,arrivaltime,departtime 
											LIMIT {0},{1};", 
                                    i * RecordChunk, 
                                    RecordChunk));

                        if (timetableData != null)
                        {
                            foreach (DataRow row in timetableData.Rows)
                            {
                                // var serviceId = Convert.ToInt32(timetableData.Rows[0][6]);
                                this.timetable.AddTimetableEntry(
                                    Convert.ToInt32(row[0]), 
                                    Convert.ToInt32(row[1]), 
                                    Convert.ToInt32(row[2]), 
                                    Convert.ToInt32(row[3]), 
                                    Convert.ToInt32(row[4]), 
                                    Convert.ToInt32(row[5]), 
                                    (int)Convert.ToDouble(nonNumericCharacters.Replace(row[6].ToString(), string.Empty)));
                            }
                        }

                        i++;
                    }
                    while (timetableData != null && timetableData.Rows.Count > 0);

                    Logger.Log(this, "Saving cache file...");
                    this.timetable.Save("TimetableCache.dat");

                    Logger.Log(this, "Optimising...");
                    this.timetable.Optimise();

                    // IFormatter formatter = new BinaryFormatter();
                    // Stream stream = new FileStream("TimetableCache.dat", FileMode.Create, FileAccess.Write, FileShare.None);
                    // formatter.Serialize(stream, timetable);

                    // ProtoBuf.Serializer.Serialize(stream, timetable);
                    // stream.Close();
                }
                else
                {
                    Logger.Log(this, "Timetable cache found. Loading...");

                    // IFormatter formatter = new BinaryFormatter();
                    // Stream stream = new FileStream("TimetableCache.dat", FileMode.Open, FileAccess.Read, FileShare.Read);
                    // timetable = (Timetable)formatter.Deserialize(stream);
                    // timetable = ProtoBuf.Serializer.Deserialize<Timetable>(stream);
                    // stream.Close();
                    this.timetable.Load("TimetableCache.dat");
                }

                Logger.Log(this, "Ending experimental timetable code...");

                /*
                int sstCount = Convert.ToInt32(this.database.GetDataSet("SELECT count(*) FROM tblSST").Rows[0][0]);
                if (sstCount == 0)
                {
                    Logger.Log(this, "Services timetable is empty. Populating...");
                    this.database.RunQuery(
                        @"TRUNCATE tblSST;
                        INSERT INTO tblSST
                        SELECT
                        `tblservicearcs`.`ServiceID`,
                        `tblservicearcs`.`SourceID`,
                        `tblservicearcs`.`RouteID`,
                        `tblservicearcs`.`DepartTime`,
                        `tblservicearcs`.`ArriveTime`,
                        `tblservicearcs`.`DOW`,
                        `tblservicearcs`.`DestID`
                        FROM tblServiceArcs;
                        ");

                }
				 */
            }

            // catch (Exception e)
            // {
            // Logger.Log(this, "Error intitilizing MetlinkDataProvider: " + e.Message + "\n" + e.StackTrace + "\n");
            // throw;
            // }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PtvDataProvider"/> class.
        /// </summary>
        ~PtvDataProvider()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Cleans up the resources used by this object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Cleans up the resources used by this object.
        /// </summary>
        /// <param name="disposing">
        /// </param>
        public void Dispose(bool disposing)
        {
            this.database.Dispose();
        }

        /// <summary>
        /// Gets the network nodes that are adjacent to the specified node.
        /// </summary>
        /// <param name="node">
        /// The specified node. 
        /// </param>
        /// <returns>
        /// A list of <see cref="INetworkNode"/> objects. 
        /// </returns>
        public List<INetworkNode> GetAdjacentNodes(INetworkNode node)
        {
            List<PtvNode> adjacentNodes = this.list[node.Id];
            var outputNodes = new List<INetworkNode>(this.list[node.Id].Count);
            for (int i = 2; i < adjacentNodes.Count; i++)
            {
                if (adjacentNodes[i] == node)
                {
                    throw new Exception("The node is adjacent to itself!");
                }

                /*
                List<int> routes = this.routeMap[this.list[node.Id][i].Id];
                for (int j = 0; j < routes.Count; j++)
                {
                    INetworkNode subNode = (INetworkNode)this.list[node.Id][i].Clone();
                    subNode.RouteId = routes[j];
                    outputNodes.Add(subNode);
                }
                 */
                outputNodes.Add(adjacentNodes[i]);
            }

            return outputNodes;
        }

        /// <summary>
        /// Gets the network nodes that are on the same route as the specified node.
        /// </summary>
        /// <param name="node">
        /// The specified node. 
        /// </param>
        /// <param name="routeId">
        /// The route identifier. 
        /// </param>
        /// <returns>
        /// A list of <see cref="INetworkNode"/> objects. 
        /// </returns>
        public List<INetworkNode> GetAdjacentNodes(INetworkNode node, int routeId)
        {
            List<PtvNode> adjacent = this.list[node.Id];
            var valid = new List<INetworkNode>();
            for (int i = 1; i < adjacent.Count; i++)
            {
                if (this.routeMap[adjacent[i].Id].Contains(routeId))
                {
                    valid.Add(adjacent[i]);
                }
            }

            return valid;
        }

        /// <summary>
        /// Returns the type of node that this provider services.
        /// </summary>
        /// <returns>
        /// A Type representing the type of node that this provider handles. 
        /// </returns>
        public Type GetAssociatedType()
        {
            return typeof(PtvNode);
        }

        /// <summary>
        /// Gets the shortest distance between nodes for a specific route.
        /// </summary>
        /// <param name="source">
        /// The source node. 
        /// </param>
        /// <param name="destination">
        /// The destination node. 
        /// </param>
        /// <param name="departureTime">
        /// The optimum time of departure. 
        /// </param>
        /// <returns>
        /// A list of <see cref="Arc"/> objects that represent the multiple ways to get between the 2 points. 
        /// </returns>
        public List<Arc> GetDistanceBetweenNodes(INetworkNode source, INetworkNode destination, DateTime departureTime)
        {
            Assert.That(!departureTime.Equals(default(DateTime)), "Departure time is default. Something is wrong.");

            var arcs = new List<Arc>();
            int dow = (int)departureTime.DayOfWeek;

            if (dow < 1)
            {
                dow = 7;
            }

            int dowFilter = 1 << 7 - dow;

            int flatDepartureTime = Convert.ToInt32(departureTime.ToString("Hmm"));

            var departures = this.timetable.GetDepartures(source.Id, destination.Id, dowFilter, flatDepartureTime);
            if (departures.Length == 0)
            {
                return arcs;
            }

            // Departure departure = departures.FirstOrDefault(departure1 => departure1.routeId == routeId);
            // Departure arrival = de

            // Departure departure = 
            foreach (var departure in departures)
            {
                var arrival = this.timetable.GetArrivals(destination.Id, departure.serviceId);

                if (arrival.Equals(default(Departure)))
                {
                    continue;
                }

                if (arrival.order <= departure.order)
                {
                    // Logger.Log(this,"Backwards service: No route");
                    continue;
                }

                if (departure.departureTime == 0 && Math.Abs(departure.departureTime - arrival.arrivalTime) > 30.0)
                {
                    continue;
                }
                if (arrival.arrivalTime < departure.departureTime)
                {
                    arrival.arrivalTime += 2400;
                }

                // int waitingTime = departure.departureTime - flatDepartureTime;
                // int travelTime = arrival.arrivalTime - departure.departureTime;
                int waitingTime = this.SubtractTimes(departure.departureTime, flatDepartureTime);
                int travelTime = this.SubtractTimes(arrival.arrivalTime, departure.departureTime);

                TransportTimeSpan output = default(TransportTimeSpan);
                output.WaitingTime = this.parseSpan(waitingTime);
                output.TravelTime = this.parseSpan(travelTime);
                if (output.TravelTime.Ticks < 0 || output.WaitingTime.Ticks < 0)
                {
                    // throw new Exception("Negitive time span detected.");
                    Assert.Fail("Negitive time span");
                    Logger.Log(this, "WARNING: Negitive timespan between nodes detected!");
                    Logger.Log(
                        this, 
                        "Total: {0}, Travel: {1}, Waiting: {2}", 
                        output.TotalTime, 
                        output.TravelTime, 
                        output.WaitingTime);
                    Logger.Log(
                        this, 
                        "departTime: {0}, departureTime: {1}, arrivalTime: {2}", 
                        departureTime, 
                        departure.departureTime, 
                        arrival.arrivalTime);

                    // return default(TransportTimeSpan);
                }

                arcs.Add(
                    new Arc(
                        (Location)source, 
                        (Location)destination, 
                        output, 
                        GeometryHelper.GetStraightLineDistance((Location)source, (Location)destination), 
                        departureTime, 
                        "Unknown", 
                        departure.routeId));
            }

            return arcs.OrderBy(arc => arc.Time.TotalTime).ToList();
        }

        /// <summary>
        /// Gets the shortest distance between nodes.
        /// </summary>
        /// <param name="source">
        /// The source node. 
        /// </param>
        /// <param name="destination">
        /// The destination node. 
        /// </param>
        /// <param name="departureTime">
        /// The optimum time of departure. 
        /// </param>
        /// <param name="routeId">
        /// The route to calculate the distance for. 
        /// </param>
        /// <returns>
        /// A list of <see cref="Arc"/> objects that represent the multiple ways to get between the 2 points. 
        /// </returns>
        public TransportTimeSpan GetDistanceBetweenNodes(
            INetworkNode source, INetworkNode destination, DateTime departureTime, int routeId)
        {
            throw new NotImplementedException();

            /*
            int dow = (int)departureTime.DayOfWeek;
            

            if (dow < 1)
            {
                dow = 7;
            }
            int dowFilter = 1 << 7 - dow;
            int flatDepartureTime = Convert.ToInt32(
                departureTime.ToString("Hmm"));
            var departures = timetable.GetDepartures(source.Id, dowFilter, flatDepartureTime);
            
            
            Departure departure = departures.FirstOrDefault(departure1 => departure1.routeId == routeId && !departure1.Equals(default(Departure)));
            
            Console.WriteLine("Chosen departure: {0}", departure);
           // Departure arrival = de


            if (!departure.Equals(default(Departure)))
            {

                int actualDepature = departure.departureTime;
                //Departure departure = 
                DateTime departTime = this.ParseDate(departure.departureTime.ToString(CultureInfo.InvariantCulture));

                 var arrivals = timetable.GetArrivals(
                destination.Id,departure.serviceId);
                
               
                var arrival = arrivals.FirstOrDefault(arrival1 => arrival1.routeId == routeId && arrival1.serviceId == departure.serviceId);
                if (arrival.arrivalTime == 0)
                {
                    arrival.arrivalTime = arrival.departureTime;
                }
                Console.WriteLine("Chosen arrival: {0}", arrival);

                 if (arrival.order <= departure.order)
                 {
                     Logger.Log(this,"Backwards service: Returning empty.");
                     return default(TransportTimeSpan);
                 }

                if (arrival.Equals(default (Departure)))
                {
                    Logger.Log(this, "No arrival: Returning empty.");
                    return default(TransportTimeSpan);
                }

                DateTime arrivalTime = this.ParseDate(arrival.arrivalTime.ToString(CultureInfo.InvariantCulture)); ;
                //Normalize dates
                arrivalTime += departureTime.Date - default(DateTime).Date;
                departTime += departureTime.Date - default(DateTime).Date;

                if (arrival.arrivalTime < actualDepature)
                {
                    arrival.arrivalTime += 1200;
                }

                int waitingTime = actualDepature - flatDepartureTime;
                int travelTime = arrival.arrivalTime - actualDepature;

                TransportTimeSpan output = default(TransportTimeSpan);
                output.WaitingTime = this.parseSpan(waitingTime);
                output.TravelTime = this.parseSpan(travelTime) ;
                if (output.TravelTime.Ticks < 0 || output.WaitingTime.Ticks < 0)
                {
                    // throw new Exception("Negitive time span detected.");

                    Logger.Log(this,"WARNING: Negitive timespan between nodes detected!");
                    Logger.Log(this,"Total: {0}, Travel: {1}, Waiting: {2}",output.TotalTime,output.TravelTime,output.WaitingTime);
                    Logger.Log(this,"departTime: {0}, departureTime: {1}, arrivalTime: {2}",departTime,departureTime,arrivalTime);
                    //return default(TransportTimeSpan);
                }

                return output;
            }
            Logger.Log(this,"WARNING: Null timespan between nodes detected!");
            return default(TransportTimeSpan);
            */
        }

        /// <summary>
        /// The get line nodes.
        /// </summary>
        /// <param name="lineId">
        /// The line id.
        /// </param>
        /// <returns>
        /// </returns>
        public List<INetworkNode> GetLineNodes(int lineId)
        {
            string query = string.Format(
                "SELECT MetlinkStopID FROM tblLinesStops WHERE LineID={0} ORDER BY Sequence", lineId);
            var result = this.database.GetDataSet(query);
            var nodes = new List<INetworkNode>(result.Rows.Count);
            nodes.AddRange(from DataRow row in result.Rows select this.list[Convert.ToInt32(row["MetlinkStopID"])][0]);
            return nodes;
        }

        /// <summary>
        /// Gets the network node that is closest to the specified point on the specified route.
        /// </summary>
        /// <param name="destination">
        /// The node to measure the distance to. 
        /// </param>
        /// <param name="routeId">
        /// The route to take the nodes from. 
        /// </param>
        /// <returns>
        /// A network node 
        /// </returns>
        public INetworkNode GetNodeClosestToPoint(INetworkNode destination, int routeId)
        {
            if (routeId == -1)
            {
                return null;
            }

            string query =
                string.Format(
                    @"
                            SELECT si.MetlinkStopID, si.GPSLat, si.GPSLong, si.StopModeID, sr.StopOrder FROM tblStopRoutes sr
                            INNER JOIN tblStopInformation si ON sr.MetlinkStopID=si.MetlinkStopID
                           
                    ", 
                    routeId);

            // WHERE RouteID={0}
            DataTable result = this.database.GetDataSet(query);
            List<NodeWrapper<INetworkNode>> nodes =
                (from DataRow row in result.Rows
                 select new NodeWrapper<INetworkNode>(this.list[(int)row["MetlinkStopID"]][0])).ToList();
            double minDistance = double.MaxValue;
            NodeWrapper<INetworkNode> minNode = null;

            foreach (var node in nodes)
            {
                node.EuclidianDistance = GeometryHelper.GetStraightLineDistance(
                    (Location)node.Node, (Location)destination);
                if (node.EuclidianDistance < minDistance)
                {
                    minDistance = node.EuclidianDistance;
                    minNode = node;
                }
            }

            return minNode.Node;
        }

        /// <summary>
        /// Gets the network nodes that are located within radius distance to the specified location.
        /// </summary>
        /// <param name="source">
        /// The source point. 
        /// </param>
        /// <param name="destination">
        /// The point that is the target of the search. 
        /// </param>
        /// <param name="radius">
        /// The distance to look around the source point. 
        /// </param>
        /// <param name="allowTransfer">
        /// </param>
        /// <returns>
        /// The <see cref="INetworkNode"/> object that is the closest to the destination inside the radius. 
        /// </returns>
        public INetworkNode GetNodeClosestToPointWithinArea(
            INetworkNode source, INetworkNode destination, double radius, bool allowTransfer)
        {
            Location topLeft = GeometryHelper.Travel((Location)source, 315.0, radius);
            Location bottomRight = GeometryHelper.Travel((Location)source, 135.0, radius);

            string query;
            if (allowTransfer || source.RouteId == -1)
            {
                query =
                    string.Format(
                        "SELECT sr.MetlinkStopID, sr.RouteID, si.GPSLat, si.GPSLong, si.StopModeID FROM tblStopInformation si "
                        + "INNER JOIN tblStopRoutes sr " + "ON sr.MetlinkStopID=si.MetlinkStopID " + "WHERE "
                        + "si.GPSLat < {0} AND " + "si.GPSLong > {1} AND " + "si.GPSLat  > {2} AND "
                        + "si.GPSLong < {3};", 
                        topLeft.Latitude, 
                        topLeft.Longitude, 
                        bottomRight.Latitude, 
                        bottomRight.Longitude);
            }
            else
            {
                query =
                    string.Format(
                        "SELECT sr.MetlinkStopID, sr.RouteID, si.GPSLat, si.GPSLong, si.StopModeID FROM tblStopInformation si "
                        + "INNER JOIN tblStopRoutes sr " + "ON sr.MetlinkStopID=si.MetlinkStopID " + "WHERE "
                        + "si.GPSLat < {0} AND " + "si.GPSLong > {1} AND " + "si.GPSLat  > {2} AND "
                        + "si.GPSLong < {3} AND sr.RouteID = {4};", 
                        topLeft.Latitude, 
                        topLeft.Longitude, 
                        bottomRight.Latitude, 
                        bottomRight.Longitude, 
                        source.RouteId);

                /*
                 * 
                 * 
                 *    "ON sr.MetlinkStopID=si.MetlinkStopID " +
                    "WHERE " + "si.GPSLat < {0} AND " + "si.GPSLong > {1} AND "
                    + "si.GPSLat  > {2} AND " + "si.GPSLong < {3};",
                    topLeft.Latitude,
                    topLeft.Longitude,
                    bottomRight.Latitude,
                    bottomRight.Longitude);
                 * 
                 */
            }

            DataTable table = this.database.GetDataSet(query);

            var nodes =
                (from DataRow row in table.Rows
                 select new NodeWrapper<INetworkNode>(this.list[(int)row["MetlinkStopID"]][0])).ToList();

            double minDistance = double.MaxValue;
            NodeWrapper<INetworkNode> minNode = null;

            foreach (var node in nodes)
            {
                node.EuclidianDistance = GeometryHelper.GetStraightLineDistance(
                    (Location)node.Node, (Location)destination);
                if (node.EuclidianDistance < minDistance && !node.Node.Equals(source))
                {
                    minDistance = node.EuclidianDistance;
                    minNode = node;
                }
            }

            return minNode.Node;
        }

        /// <summary>
        /// Gets a datatable filled with data related to this stop.
        /// </summary>
        /// <param name="metlinkStopId">
        /// The Metlink stop identifier. 
        /// </param>
        /// <returns>
        /// A <see cref="DataTable"/> filled with relivant data. 
        /// </returns>
        public DataTable GetNodeData(int metlinkStopId)
        {
            return
                this.database.GetDataSet(
                    string.Format(
                        "SELECT * FROM tblStopInformation si INNER JOIN tblModes m on si.StopModeID = m.StopModeID WHERE MetlinkStopID = '{0}'", 
                        metlinkStopId));
        }

        /// <summary>
        /// Returns a node from a given Id.
        /// </summary>
        /// <param name="id">
        /// The identifier of the node. 
        /// </param>
        /// <returns>
        /// A node. 
        /// </returns>
        public INetworkNode GetNodeFromId(int id)
        {
            INetworkNode node = this.list[id][0];
            return node;
        }

        /// <summary>
        /// The get node from name.
        /// </summary>
        /// <param name="stopSpecName">
        /// The stop spec name.
        /// </param>
        /// <returns>
        /// </returns>
        public PtvNode GetNodeFromName(string stopSpecName)
        {
            string query = string.Format(
                @"SELECT MetlinkStopID FROM tblStopInformation WHERE StopSpecName='{0}';", stopSpecName);
            var data = this.database.GetDataSet(query);
            Assert.That(data.Rows.Count < 2, "StopSpecName should be unique.");
            if (data.Rows.Count == 1)
            {
                return this.list[(int)data.Rows[0][0]][0];
            }

            return null;
        }

        /// <summary>
        /// Get the location of the node with the specified Id.
        /// </summary>
        /// <param name="id">
        /// The unique node identifier. 
        /// </param>
        /// <returns>
        /// The <see cref="Location"/> of the node. 
        /// </returns>
        public Location GetNodeLocation(int id)
        {
            return new Location(
                Convert.ToDouble(this.GetNodeData(id).Rows[0]["GPSLat"]), 
                Convert.ToDouble(this.GetNodeData(id).Rows[0]["GPSLong"]));
        }

        /// <summary>
        /// Gets the network nodes that are located within radius distance to the specified location.
        /// </summary>
        /// <param name="location">
        /// The center point for the search. 
        /// </param>
        /// <param name="radius">
        /// The distance to look around the center point. 
        /// </param>
        /// <returns>
        /// A list of <see cref="INetworkNode"/> objects that are in the specified area. 
        /// </returns>
        public List<NodeWrapper<PtvNode>> GetNodesAtLocation(Location location, double radius)
        {
            Location topLeft = GeometryHelper.Travel(location, 315.0, radius);
            Location bottomRight = GeometryHelper.Travel(location, 135.0, radius);

            string query =
                string.Format(
                    @"SELECT DISTINCT si.MetlinkStopId, sr.RouteId, LineMainID, si.GPSLong, si.GPSLat, 
	                    min(SQRT(POW(si.GPSLat - {4},2) + POW(si.GPSLong - {5},2))) AS distance 
	                    FROM tblStopInformation si
                    INNER JOIN tblStopRoutes sr 
                    ON sr.MetlinkStopID=si.MetlinkStopID
                    INNER JOIN tblLinesRoutes lr
                    ON sr.RouteID=lr.RouteID
                    INNER JOIN tblLines l
                    ON lr.LineID=l.LineID
                    WHERE si.GPSLat < {0} and si.GPSLong > {1} AND
                    si.GPSLat > {2} AND si.GPSLong < {3}
                    GROUP BY LineMainID
                    ORDER BY DISTANCE", 
                    topLeft.Latitude, 
                    topLeft.Longitude, 
                    bottomRight.Latitude, 
                    bottomRight.Longitude, 
                    location.Latitude, 
                    location.Longitude);

            DataTable table = this.database.GetDataSet(query);

            var nodes = new List<NodeWrapper<PtvNode>>();
            foreach (DataRow row in table.Rows)
            {
                var node = new NodeWrapper<PtvNode>(this.list[(int)row["MetlinkStopID"]][0]);

                node.CurrentRoute = (int)row["RouteID"];
                if (node.CurrentRoute == -1)
                {
                    throw new Exception("null route encountered!");
                }

                // node.RetrieveData();
                nodes.Add(node);
            }

            return nodes;
        }

        /// <summary>
        /// Returns a random integer that corrosponds to a stop in the Metlink database. 
        ///   Use for debugging.
        /// </summary>
        /// <returns>
        /// A Metlink stop ID.
        /// </returns>
        public int GetRandomNodeId()
        {
            string query = "SELECT MetlinkStopID FROM tblStopInformation ORDER BY rand() LIMIT 1;";
            return (int)this.database.GetDataSet(query).Rows[0][0];
        }

        /// <summary>
        /// Gets a list of routes that this node passes through.
        /// </summary>
        /// <param name="node">
        /// The node you wish to query. 
        /// </param>
        /// <returns>
        /// A list of routes that intersect this node. 
        /// </returns>
        public List<int> GetRoutesForNode(INetworkNode node)
        {
            if (this.routeMap.ContainsKey(node.Id))
            {
                return this.routeMap[node.Id];
            }

            Logger.Log(this, "Warning: Node {0} has no associated routes.", node);
            return new List<int>();
        }

        /// <summary>
        /// Gets the stop mode string for the specified ID.
        /// </summary>
        /// <param name="stopModeId">
        /// The stop mode identifier. 
        /// </param>
        /// <returns>
        /// The corrosponding string. 
        /// </returns>
        public string GetStopMode(string stopModeId)
        {
            return StopModeTable[stopModeId];
        }

        /// <summary>
        /// The in same line.
        /// </summary>
        /// <param name="node1">
        /// The node 1.
        /// </param>
        /// <param name="node2">
        /// The node 2.
        /// </param>
        /// <returns>
        /// The in same line.
        /// </returns>
        public bool InSameLine(PtvNode node1, PtvNode node2)
        {
            string query =
                string.Format(
                    @"SELECT l1.LineMainID, l2.LineMainID FROM tblLinesStops ls1
                    INNER JOIN tblLines l1
                    ON l1.LineID = ls1.LineID
                    INNER JOIN tblLinesStops ls2
                    ON ls2.LineID = ls2.LineID
                    INNER JOIN tblLines l2
                    ON l2.LineID = ls2.LineID
                    WHERE ls1.MetlinkStopID = {0} 
                    AND
                    ls2.MetlinkStopID =  {1}
                    AND l1.LineMainID = l2.LineMainID", 
                    node1.Id, 
                    node2.Id);

            return this.database.GetDataSet(query).Rows.Count > 0;
        }

        /// <summary>
        /// Returns a list of stops that match the query. Currently the query is just a simple
        ///   match string with wildcard.
        /// </summary>
        /// <returns>
        /// The stops that match the query.
        /// </returns>
        /// <param name="query">
        /// The query. i.e. 'Cob' -&gt; Coburg Station
        /// </param>
        public object[] QueryStops(string query)
        {
            // List<PtvNode> stops = new List<PtvNode>();
            string sqlQuery =
                string.Format(
                    @"SELECT StopSpecName,CONCAT(TravelSTName,' ',TravelSTType,'/',COALESCE(CrossSTName,''),' ',COALESCE(CrossSTType,'')), 
											StopModeName, MetlinkStopID FROM tblStopInformation si INNER JOIN tblModes sm ON sm.StopModeId=si.StopModeID 
											WHERE StopSpecName LIKE '%{0}%' ORDER BY sm.StopModeID DESC", 
                    query);
            var data = this.database.GetDataSet(sqlQuery);
            object[] stops = new object[data.Rows.Count];

            for (int i = 0; i < data.Rows.Count; i++)
            {
                stops[i] =
                    new
                        {
                            label = data.Rows[i].ItemArray[0], 
                            stopSpecName = data.Rows[i].ItemArray[1] + " (" + data.Rows[i].ItemArray[3] + ")", 
                            stopMode = data.Rows[i].ItemArray[2], 
                            value = data.Rows[i].ItemArray[3]
                        };
            }

            return stops;
        }

        /// <summary>
        /// Returns true if there is at least 1 route common between the 2 nodes.
        /// </summary>
        /// <param name="first">
        /// The first node to test. 
        /// </param>
        /// <param name="second">
        /// The second node to test. 
        /// </param>
        /// <returns>
        /// A boolean value. 
        /// </returns>
        public bool RoutesIntersect(INetworkNode first, INetworkNode second)
        {
            List<int> a;
            List<int> b;

            this.routeMap.TryGetValue(first.Id, out a);
            this.routeMap.TryGetValue(second.Id, out b);
            if (a == null || !a.Any() || b == null || !b.Any())
            {
                return false;
            }

            return a.Intersect(b).Any();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The parse date.
        /// </summary>
        /// <param name="date">
        /// The date. 
        /// </param>
        /// <returns>
        /// </returns>
        private DateTime ParseDate(string date)
        {
            try
            {
                if (date == "9999")
                {
                    throw new Exception("Invalid time");
                }

                int minutes = Convert.ToInt32(string.Format("{0:d4}", Convert.ToInt32(date)).Substring(2, 2));
                int hours = Convert.ToInt32(string.Format("{0:d4}", Convert.ToInt32(date)).Substring(0, 2));
                var dt = new DateTime();
                int days = 0;
                if (hours >= 24)
                {
                    days = 1;
                    hours -= 24;
                }

                dt = dt.Add(new TimeSpan(days, hours, minutes, 0));

                /*
                if (!DateTime.TryParseExact(date, "Hmm", null, DateTimeStyles.None, out dt))
                {
                    if (date.Substring(0, 2) == "24")
                    {
                        date = "00" + date.Substring(2, 2);
                        if (!DateTime.TryParseExact(date, "Hmm", null, DateTimeStyles.None, out dt))
                        {
                            throw new Exception("Time parsing error");
                        }
                    }
                }
                */
                return dt;
            }
            catch (Exception)
            {
                Console.WriteLine("Warning, bogus depart/arrive time detected in parser.");
                throw;

                // return new DateTime().Add(new TimeSpan(10, 0, 0, 0));
            }
        }

        /// <summary>
        /// The round up.
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <returns>
        /// </returns>
        private DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
        }

        /// <summary>
        /// The subtract times.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The subtract times.
        /// </returns>
        private int SubtractTimes(int a, int b)
        {
            int ha = a / 100;
            int hb = b / 100;
            int ma = a - (ha * 100);
            int mb = b - (hb * 100);

            int md = ma - mb;
            int hd = ha - hb;

            return (hd * 60) + md;
        }

        /// <summary>
        /// The parse date.
        /// </summary>
        /// <param name="timespan">
        /// The timespan.
        /// </param>
        /// <returns>
        /// </returns>
        private TimeSpan parseSpan(int timespan)
        {
            try
            {
                string date = timespan.ToString();

                int minutes = Convert.ToInt32(string.Format("{0:d4}", Convert.ToInt32(date)).Substring(2, 2));
                int hours = Convert.ToInt32(string.Format("{0:d4}", Convert.ToInt32(date)).Substring(0, 2));
                return TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes);
            }
            catch (Exception)
            {
                Console.WriteLine("Warning, bogus depart/arrive time detected in parser.");
                throw;

                // return TimeSpan.MaxValue;
            }
        }

        #endregion
    }
}