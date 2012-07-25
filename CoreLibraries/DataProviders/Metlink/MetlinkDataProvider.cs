// RMIT Journey Planner
// Written by Sean Dawson 2011.
// Supervised by Xiaodong Li and Margret Hamilton for the 2011 summer studentship program.

namespace RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;

    using RmitJourneyPlanner.CoreLibraries.Comparers;
    using RmitJourneyPlanner.CoreLibraries.DataAccess;
    using RmitJourneyPlanner.CoreLibraries.Logging;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    ///   Providers data on the TransNET database.
    /// </summary>
    public sealed class MetlinkDataProvider : INetworkDataProvider, IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   The stop mode table.
        /// </summary>
        private static readonly Dictionary<string, string> StopModeTable = new Dictionary<string, string>();
		
		/// <summary>
		/// The ammount of records to read in at any given time.
		/// </summary>
		private const int RecordChunk = 100000000;

        /// <summary>
        ///   The Metlink database object.
        /// </summary>
        private readonly MySqlDatabase database = new MySqlDatabase("20110606fordistributionforrmit");

        /// <summary>
        ///   The list.
        /// </summary>
        private readonly AdjacencyList list = new AdjacencyList();

        /// <summary>
        ///   The route map.
        /// </summary>
        private readonly Dictionary<int, List<int>> routeMap = new Dictionary<int, List<int>>();

        private readonly Timetable timetable = new Timetable();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref="MetlinkDataProvider" /> class.
        /// </summary>
        static MetlinkDataProvider()
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
        ///   Initializes a new instance of the <see cref="MetlinkDataProvider" /> class.
        /// </summary>
        public MetlinkDataProvider()
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
            try
            {

                Logger.Log(this, "Route Generator Initializing...");

                Logger.Log(this, "Building node-route map...");
                Logger.Log(this, "-->Querying this.database...");
                DataTable nodeData = null;
				int i = 0;
				do {
					
                    Logger.Log(this,"Reading records {0}-{1}",i*RecordChunk,RecordChunk);
					/*
					nodeData = this.database.GetDataSet(
                        String.Format(@"SELECT s.ServiceID,s.RouteID, st.MetlinkStopID, st.Sequence 
                            FROM tblServices s
                            INNER JOIN tblServiceTimes st
                            ON s.ServiceID=st.ServiceID
                            ORDER BY RouteID, s.ServiceID,ArrivalTime
							LIMIT {0}, {1};",i *1000, (i*1000) + 1000));\
					*/
					nodeData = this.database.GetDataSet(
                        String.Format(@"SELECT RouteID, MetlinkStopID 
                            FROM tblStopRoutes
                            ORDER BY RouteID, StopOrder
							LIMIT {0}, {1};",i * RecordChunk, RecordChunk));
					
					if (nodeData != null)
					{
		                foreach (DataRow row in nodeData.Rows)
		                {
		                    var id = (int)row["MetlinkStopID"];
		                    if (!this.routeMap.ContainsKey(id))
		                    {
		                        this.routeMap[id] = new List<int>();
		                    }
		
		                    var routeId = (int)row["RouteID"];
		                    if (!this.routeMap[id].Contains(routeId))
		                    {
		                        this.routeMap[id].Add(routeId);
		                    }
	                	}
					}
					i++;
				} while (nodeData != null && nodeData.Rows.Count > 0);
               //ogger.Log(this, "Building route path map...");
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
                    var node = new MetlinkNode
                    {
                        Id = (int)row["MetlinkStopID"],
                        TransportType = row["StopModeName"].ToString(),
                        Latitude = Convert.ToSingle(row["GPSLat"]),
                        Longitude = Convert.ToSingle(row["GPSLong"]),
                    
                     * StopSpecName = row["StopSpecName"].ToString()
                    };*/

                    var node = new MetlinkNode(
                        (int)row["MetlinkStopID"],
                        row["StopModeName"].ToString(),
                        row["StopSpecName"].ToString(),
                        Convert.ToDouble(row["GPSLat"]),
                        Convert.ToDouble(row["GPSLong"]),
                        this);

                    // Logger.Log(this,"-->Adding node (id: {0})... [{1} s]", node.Id, stopwatch.ElapsedMilliseconds / 1000.0);
                    // Console.SetCursorPosition(0, Console.CursorTop);
                    // Console.Write("{0,10:f2}%", 100.0 * ((double)count++ / (Double)nodeData.Rows.Count));
                    this.list[node.Id] = new List<MetlinkNode> { node };
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

                    for (i = 0; i < nodeData.Rows.Count - 1; i++)
                    {
                        DataRow row = nodeData.Rows[i];
                        DataRow nextRow = nodeData.Rows[i + 1];

                        // var RouteID = (int)row["RouteID"];
                        var rowId = (int)row["MetlinkStopID"];
                        var rowStopOrder = (int)row["StopOrder"];
                        var rowServiceId = (int)row["RouteID"];
                   

                        var nextId = (int)nextRow["MetlinkStopID"];
                        var nextStopOrder = (int)nextRow["StopOrder"];
                        var nextServiceId = (int)nextRow["RouteID"];

                    
                        if ((nextStopOrder >= rowStopOrder ||  nextStopOrder == 0) && 
                            !this.list[rowId].Contains(this.list[nextId][0])&&
                            nextServiceId == rowServiceId)
                        {
                            this.list[rowId].Add(this.list[nextId][0]);
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

                    // var nakedNodes = new List<MetlinkNode>();
                    var cycleNodes = new List<MetlinkNode>();
                    int count = 0;
                    foreach (DataRow node in nodeData.Rows)
                    {
                        // Console.SetCursorPosition(0, Console.CursorTop);
                        // Console.Write("{0,10:f2}%", 100.0 * ((double)count++ / (Double)nodeData.Rows.Count));
                        var id = (int)node["MetlinkStopID"];
                        double progress = (count++ / (double)nodeData.Rows.Count) * 100;
                        Logger.UpdateProgress(this, (int)progress);

                        //TODO: Make scan distance variable.

                        MetlinkNode metlinkNode = this.list[id][0];
                        var nodes =
                            this.GetNodesAtLocation(new Location(metlinkNode.Latitude, metlinkNode.Longitude), 0.5);



                        foreach (var closeNode in nodes)
                        {
                            closeNode.EuclidianDistance = GeometryHelper.GetStraightLineDistance(
                                metlinkNode.Latitude, metlinkNode.Longitude, closeNode.Node.Latitude, closeNode.Node.Longitude);
                        }

                        nodes.Sort(new NodeComparer<MetlinkNode>());

                        var vistedRoutes = new HashSet<int>();
                        

                        foreach (var closeNode in nodes)
                        {
                            if ( !this.InSameLine(closeNode.Node,metlinkNode) &&
                                closeNode.Node.Id != metlinkNode.Id
                                && !vistedRoutes.Contains(Convert.ToInt32(closeNode.CurrentRoute)))
                            {
                                vistedRoutes.Add(Convert.ToInt32(closeNode.CurrentRoute));
                                if (!this.list[id].Contains(this.list[Convert.ToInt32(closeNode.Node.Id)][0]))
                                {
                                    // Logger.Log(this,"-->Adding proximity link: id1: {0} id2: {1}... [{2} s]", MetlinkNode.Id, closeNode.Id, stopwatch.ElapsedMilliseconds / 1000.0);
                                    this.list[id].Add(this.list[Convert.ToInt32(closeNode.Node.Id)][0]);
                                    totalLinks++;

                                    // list[Convert.ToInt32(closeNode.Id)].Add(MetlinkNode);
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
                        foreach (MetlinkNode nakedNode in nakedNodes)
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
                        foreach (KeyValuePair<int, List<MetlinkNode>> kvp in this.list)
                        {
                            var sb = new StringBuilder();
                            foreach (MetlinkNode node in kvp.Value)
                            {
                                sb.Append(node.Id.ToString(CultureInfo.InvariantCulture) + ",");
                            }

                            writer.WriteLine(string.Format("{0}: {1}", kvp.Key, sb));
                        }
                    }
                }

                var craigAdg = list[20035];
             

                Logger.Log(this, "\nAdjacency data structure loaded successfully in {0} seconds.");


                Logger.Log(this, "Starting experimental timetable code...");

                if (!File.Exists("TimetableCache.dat"))
                {
                    Logger.Log(this,"Timetable cache non existant. Rebuilding...");
                    DataTable timetableData = new DataTable();
                        
					i = 0;
                    
                  	do
					{
	                    Logger.Log(this,"Adding rows {0} to {1} of {2}...",i*RecordChunk,RecordChunk,timetableData.Rows.Count);
						timetableData = database.GetDataSet(
	                            String.Format(@"SELECT st.MetlinkStopId, s.RouteID, s.DOW,  st.ArrivalTime, st.DepartTime,st.ServiceID  FROM tblServiceTimes  st
	                                        INNER JOIN tblServices s
											ON s.ServiceID = st.ServiceID 
											ORDER BY MetlinkStopID, RouteId,DOW,arrivaltime,departtime 
											LIMIT {0},{1};",i*RecordChunk,RecordChunk));
						
						if(timetableData != null)
						{
							foreach (DataRow row in timetableData.Rows)
		                    {                        
									
								var serviceId = Convert.ToInt32(timetableData.Rows[0][5]);
		                        
		                        timetable.AddTimetableEntry(
		                            Convert.ToInt32(row[0]),
		                            Convert.ToInt32(row[1]),
		                            Convert.ToInt32(row[2]),
		                            Convert.ToInt32(row[3]),
		                            Convert.ToInt32(row[4]),
		                            Convert.ToInt32(row[5]));
		
		                    }
						}
						i++;
					} 
					while(timetableData != null && timetableData.Rows.Count > 0);
					
                    Logger.Log(this,"Saving cache file...");
                    timetable.Save("TimetableCache.dat");

					Logger.Log(this,"Optimising...");
                    timetable.Optimise();
                   
                    //IFormatter formatter = new BinaryFormatter();
//Stream stream = new FileStream("TimetableCache.dat", FileMode.Create, FileAccess.Write, FileShare.None);
                    //formatter.Serialize(stream, timetable);

                    //ProtoBuf.Serializer.Serialize(stream, timetable);
                    //stream.Close();
                }
                else
                {
                    Logger.Log(this,"Timetable cache found. Loading...");
                    //IFormatter formatter = new BinaryFormatter();
                   // Stream stream = new FileStream("TimetableCache.dat", FileMode.Open, FileAccess.Read, FileShare.Read);
                    //timetable = (Timetable)formatter.Deserialize(stream);
                    //timetable = ProtoBuf.Serializer.Deserialize<Timetable>(stream);
                    //stream.Close();
                    timetable.Load("TimetableCache.dat");
               
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
            catch (Exception e)
            {
                Logger.Log(this, "Error intitilizing MetlinkDataProvider: " + e.Message + "\n" + e.StackTrace + "\n");
                throw;
            }

           
        }

        /// <summary>
        ///   Finalizes an instance of the <see cref="MetlinkDataProvider" /> class.
        /// </summary>
        ~MetlinkDataProvider()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Cleans up the resources used by this object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Cleans up the resources used by this object.
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose(bool disposing)
        {
            this.database.Dispose();

        }
        
        /// <summary>
        ///   Gets the network nodes that are adjacent to the specified node.
        /// </summary>
        /// <param name="node"> The specified node. </param>
        /// <returns> A list of <see cref="INetworkNode" /> objects. </returns>
        public List<INetworkNode> GetAdjacentNodes(INetworkNode node)
        {
            List<MetlinkNode> adjacentNodes = this.list[node.Id];
            var outputNodes = new List<INetworkNode>(this.list[node.Id].Count);
            for (int i = 2; i < adjacentNodes.Count; i++)
            {
                if (adjacentNodes[i] == node)
                {
                    throw new Exception(":(");

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
        ///   Gets the network nodes that are on the same route as the specified node.
        /// </summary>
        /// <param name="node"> The specified node. </param>
        /// <param name="routeId"> The route identifier. </param>
        /// <returns> A list of <see cref="INetworkNode" /> objects. </returns>
        public List<INetworkNode> GetAdjacentNodes(INetworkNode node, int routeId)
        {
            List<MetlinkNode> adjacent = this.list[node.Id];
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

        private bool InSameLine(MetlinkNode node1, MetlinkNode node2)
        {
            string query = String.Format(
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
                    AND l1.LineMainID = l2.LineMainID",node1.Id,node2.Id);

            return (database.GetDataSet(query).Rows.Count > 0);




        }

        /// <summary>
        ///   Returns the type of node that this provider services.
        /// </summary>
        /// <returns> A Type representing the type of node that this provider handles. </returns>
        public Type GetAssociatedType()
        {
            return typeof(MetlinkNode);
        }

        /// <summary>
        ///   Gets the shortest distance between nodes for a specific route.
        /// </summary>
        /// <param name="source"> The source node. </param>
        /// <param name="destination"> The destination node. </param>
        /// <param name="departureTime"> The optimum time of departure. </param>
        /// <returns> A list of <see cref="Arc" /> objects that represent the multiple ways to get between the 2 points. </returns>
        public List<Arc> GetDistanceBetweenNodes(INetworkNode source, INetworkNode destination, DateTime departureTime)
        {
            var arcs = new List<Arc>();
            
            int dowFilter = 1 << 7 - (int)departureTime.DayOfWeek;
            var departures = timetable.GetDepartures(source.Id, dowFilter, Convert.ToInt32(departureTime.ToString("Hmm")));
            if (departures.Length == 0)
            {
                return arcs;
            }

            //Departure departure = departures.FirstOrDefault(departure1 => departure1.routeId == routeId);
            // Departure arrival = de


            //Departure departure = 
            foreach (var departure in departures)
            {
                DateTime departTime = this.ParseDate(departure.departureTime.ToString(CultureInfo.InvariantCulture));
                int routeId = departure.routeId;

                var arrivals = timetable.GetDepartures(
                    destination.Id, dowFilter, Convert.ToInt32(departTime.ToString("Hmm")));


                var arrival = arrivals.FirstOrDefault(arrival1 => arrival1.routeId == routeId);

                if (arrival.Equals(default(Departure)))
                {
                    continue;
                }

                DateTime arrivalTime = this.ParseDate(arrival.arrivalTime.ToString(CultureInfo.InvariantCulture));
                //Normalize dates
                arrivalTime += departureTime.Date - default(DateTime).Date;
                departTime += departureTime.Date - default(DateTime).Date;

                TransportTimeSpan output = default(TransportTimeSpan);
                output.WaitingTime = departTime - departureTime;
                output.TravelTime = arrivalTime - departTime;
                if (output.TotalTime.Ticks < 0)
                {
                    // throw new Exception("Negitive time span detected.");
                    Logger.Log(this,"WARNING: Negitive timespan between nodes detected!");
                    //return default(TransportTimeSpan);
                    continue;
                }
                arcs.Add(new Arc((Location)source,(Location)destination,output,GeometryHelper.GetStraightLineDistance((Location)source,(Location)destination),departTime,"Unknown",departure.routeId));


            }
            return arcs;


        }

        /// <summary>
        ///   Gets the shortest distance between nodes.
        /// </summary>
        /// <param name="source"> The source node. </param>
        /// <param name="destination"> The destination node. </param>
        /// <param name="departureTime"> The optimum time of departure. </param>
        /// <param name="routeId"> The route to calculate the distance for. </param>
        /// <returns> A list of <see cref="Arc" /> objects that represent the multiple ways to get between the 2 points. </returns>
        public TransportTimeSpan GetDistanceBetweenNodes(
            INetworkNode source, INetworkNode destination, DateTime departureTime, int routeId)
        {
            /*
            string dowFilter = string.Empty;
            for (int j = 0; j < (int)departureTime.DayOfWeek; j++)
            {
                dowFilter = dowFilter.Insert(0, "_");
            }

            dowFilter += "0%";
            

           


            string query =
                string.Format(
                    @"select st1.ServiceID, 
                    min(st1.DepartTime),st2.ArriveTime frOM tblSST st1
                    INNER JOIN tblSST st2
                    ON st1.ServiceID=st2.ServiceID
                    WHERE st1.RouteID={3} AND st1.DepartTime>{2} AND st1.SourceID={0} AND st1.DOW LIKE '{4}' 
                    AND st2.DestID={1}
                        ",
                    source.Id,
                    destination.Id,
                    departureTime.ToString("Hmm"),
                    routeId,
                    dowFilter);

            DataTable result = this.database.GetDataSet(query);
            */

            int dow = (int)departureTime.DayOfWeek;

            if (dow < 1)
            {
                dow = 7;
            }
            int dowFilter = 1 << 7 - dow;
            var departures = timetable.GetDepartures(source.Id, dowFilter, Convert.ToInt32(departureTime.ToString("Hmm")));
            
            
            Departure departure = departures.FirstOrDefault(departure1 => departure1.routeId == routeId);
           // Departure arrival = de


            if (!departure.Equals(default(Departure)))
            {
              
                
                //Departure departure = 
                DateTime departTime = this.ParseDate(departure.departureTime.ToString(CultureInfo.InvariantCulture));

                 var arrivals = timetable.GetDepartures(
                destination.Id, dowFilter, Convert.ToInt32(departTime.ToString("Hmm")));
                
               
                var arrival = arrivals.FirstOrDefault(arrival1 => arrival1.routeId == routeId && arrival1.order == departure.order);

                if (arrival.arrivalTime < departure.departureTime || departure.departureTime == 0 || arrival.Equals(default (Departure)))
                {
                    return default(TransportTimeSpan);
                }

                DateTime arrivalTime = this.ParseDate(arrival.arrivalTime.ToString(CultureInfo.InvariantCulture));
                //Normalize dates
                arrivalTime += departureTime.Date - default(DateTime).Date;
                departTime += departureTime.Date - default(DateTime).Date;

                TransportTimeSpan output = default(TransportTimeSpan);
                output.WaitingTime = departTime - departureTime;
                output.TravelTime = arrivalTime - departTime;
                if (output.TotalTime.Ticks < 0)
                {
                    // throw new Exception("Negitive time span detected.");
                    // Logger.Log(this,"WARNING: Negitive timespan between nodes detected!");
                    return default(TransportTimeSpan);
                }

                return output;
            }
            // Logger.Log(this,"WARNING: Null timespan between nodes detected!");
            return default(TransportTimeSpan);
        }

        /// <summary>
        ///   Gets the network node that is closest to the specified point on the specified route.
        /// </summary>
        /// <param name="destination"> The node to measure the distance to. </param>
        /// <param name="routeId"> The route to take the nodes from. </param>
        /// <returns> A network node </returns>
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
            List<NodeWrapper<INetworkNode>> nodes = (from DataRow row in result.Rows select new NodeWrapper<INetworkNode>(this.list[(int)row["MetlinkStopID"]][0])).ToList();
            double minDistance = double.MaxValue;
            NodeWrapper<INetworkNode> minNode = null;

            foreach (var node in nodes)
            {
                node.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)node.Node, (Location)destination);
                if (node.EuclidianDistance < minDistance)
                {
                    minDistance = node.EuclidianDistance;
                    minNode = node;
                }
            }

            return minNode.Node;
        }

        /// <summary>
        ///   Gets the network nodes that are located within radius distance to the specified location.
        /// </summary>
        /// <param name="source"> The source point. </param>
        /// <param name="destination"> The point that is the target of the search. </param>
        /// <param name="radius"> The distance to look around the source point. </param>
        /// <param name="allowTransfer"> </param>
        /// <returns> The <see cref="INetworkNode" /> object that is the closest to the destination inside the radius. </returns>
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

            var nodes = (from DataRow row in table.Rows select new NodeWrapper<INetworkNode>(this.list[(int)row["MetlinkStopID"]][0])).ToList();

            double minDistance = double.MaxValue;
            NodeWrapper<INetworkNode> minNode = null;

            foreach (var node in nodes)
            {
                node.EuclidianDistance = GeometryHelper.GetStraightLineDistance((Location)node.Node, (Location)destination);
                if (node.EuclidianDistance < minDistance && !node.Node.Equals(source))
                {
                    minDistance = node.EuclidianDistance;
                    minNode = node;
                }
            }

            return minNode.Node;
        }

        /// <summary>
        ///   Gets a datatable filled with data related to this stop.
        /// </summary>
        /// <param name="metlinkStopId"> The Metlink stop identifier. </param>
        /// <returns> A <see cref="DataTable" /> filled with relivant data. </returns>
        public DataTable GetNodeData(int metlinkStopId)
        {
            return
                this.database.GetDataSet(
                    string.Format("SELECT * FROM tblStopInformation si INNER JOIN tblModes m on si.StopModeID = m.StopModeID WHERE MetlinkStopID = '{0}'", metlinkStopId));
        }

        /// <summary>
        ///   Returns a node from a given Id.
        /// </summary>
        /// <param name="id"> The identifier of the node. </param>
        /// <returns> A node. </returns>
        public INetworkNode GetNodeFromId(int id)
        {
            INetworkNode node = this.list[id][0];
            return node;
        }

        /// <summary>
        ///   Get the location of the node with the specified Id.
        /// </summary>
        /// <param name="id"> The unique node identifier. </param>
        /// <returns> The <see cref="Location" /> of the node. </returns>
        public Location GetNodeLocation(int id)
        {
            return new Location(
                Convert.ToDouble(this.GetNodeData(id).Rows[0]["GPSLat"]),
                Convert.ToDouble(this.GetNodeData(id).Rows[0]["GPSLong"]));
        }

        /// <summary>
        ///   Gets the network nodes that are located within radius distance to the specified location.
        /// </summary>
        /// <param name="location"> The center point for the search. </param>
        /// <param name="radius"> The distance to look around the center point. </param>
        /// <returns> A list of <see cref="INetworkNode" /> objects that are in the specified area. </returns>
        public List<NodeWrapper<MetlinkNode>> GetNodesAtLocation(Location location, double radius)
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

            var nodes = new List<NodeWrapper<MetlinkNode>>();
            foreach (DataRow row in table.Rows)
            {


                var node = new NodeWrapper<MetlinkNode>(list[(int)row["MetlinkStopID"]][0]);
                
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
        ///   Gets a list of routes that this node passes through.
        /// </summary>
        /// <param name="node"> The node you wish to query. </param>
        /// <returns> A list of routes that intersect this node. </returns>
        public List<int> GetRoutesForNode(INetworkNode node)
        {
            if (this.routeMap.ContainsKey(node.Id))
            {
                return this.routeMap[node.Id];
            }

            Logger.Log(this,"Warning: Node {0} has no associated routes.",node);
            return new List<int>();
        }

        /// <summary>
        ///   Gets the stop mode string for the specified ID.
        /// </summary>
        /// <param name="stopModeId"> The stop mode identifier. </param>
        /// <returns> The corrosponding string. </returns>
        public string GetStopMode(string stopModeId)
        {
            return StopModeTable[stopModeId];
        }

        /// <summary>
        ///   Returns true if there is at least 1 route common between the 2 nodes.
        /// </summary>
        /// <param name="first"> The first node to test. </param>
        /// <param name="second"> The second node to test. </param>
        /// <returns> A boolean value. </returns>
        public bool RoutesIntersect(INetworkNode first, INetworkNode second)
        {
            return this.routeMap[first.Id].Intersect(this.routeMap[second.Id]).Any();
        }

        /// <summary>
        /// Returns a random integer that corrosponds to a stop in the Metlink database. 
        /// Use for debugging.
        /// </summary>
        /// <returns>A Metlink stop ID.</returns>
        public int GetRandomNodeId()
        {
            string query = "SELECT MetlinkStopID FROM tblStopInformation ORDER BY rand() LIMIT 1;";
            return (int)this.database.GetDataSet(query).Rows[0][0];

        }

        #endregion

        #region Methods

        /// <summary>
        ///   The parse date.
        /// </summary>
        /// <param name="date"> The date. </param>
        /// <returns> </returns>
        private DateTime ParseDate(string date)
        {

            try
            {


                if (date == "9999")
                {
                    throw new Exception("Invalid time");

                }
                int minutes = Convert.ToInt32(String.Format("{0:d4}", Convert.ToInt32(date)).Substring(2, 2));
                int hours = Convert.ToInt32(String.Format("{0:d4}", Convert.ToInt32(date)).Substring(0, 2));
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
                //Console.WriteLine("Warning, bogus depart/arrive time detected in parser.");
                return new DateTime().Add(new TimeSpan(10, 0, 0, 0));
            }
        }

        #endregion
    }
}