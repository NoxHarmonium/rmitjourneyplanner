using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using RmitJourneyPlanner.CoreLibraries.DataAccess;
using RmitJourneyPlanner.CoreLibraries.DataProviders.Google;
using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Breeders;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Mutators;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.RouteGenerators;
using RmitJourneyPlanner.CoreLibraries.Types;
using System.IO.Compression;

namespace TestConsole
{

    public sealed class GaussianRandom
    {
        private bool _hasDeviate;
        private double _storedDeviate;
        private readonly Random _random;

        public GaussianRandom(Random random = null)
        {
            _random = random ?? new Random();
        }

        /// <summary>
        /// Obtains normally (Gaussian) distributed random numbers, using the Box-Muller
        /// transformation.  This transformation takes two uniformly distributed deviates
        /// within the unit circle, and transforms them into two independently
        /// distributed normal deviates.
        /// </summary>
        /// <param name="mu">The mean of the distribution.  Default is zero.</param>
        /// <param name="sigma">The standard deviation of the distribution.  Default is one.</param>
        /// <returns></returns>
        public double NextGaussian(double mu = 0, double sigma = 1)
        {
            if (sigma <= 0)
                throw new ArgumentOutOfRangeException("sigma", "Must be greater than zero.");

            if (_hasDeviate)
            {
                _hasDeviate = false;
                return _storedDeviate * sigma + mu;
            }

            double v1, v2, rSquared;
            do
            {
                // two random values between -1.0 and 1.0
                v1 = 2 * _random.NextDouble() - 1;
                v2 = 2 * _random.NextDouble() - 1;
                rSquared = v1 * v1 + v2 * v2;
                // ensure within the unit circle
            } while (rSquared >= 1 || rSquared == 0);

            // calculate polar tranformation for each deviate
            var polar = Math.Sqrt(-2 * Math.Log(rSquared) / rSquared);
            // store first deviate
            _storedDeviate = v2 * polar;
            _hasDeviate = true;
            // return second deviate
            return v1 * polar * sigma + mu;
        }
    }

    class Program
    {
        private static Dictionary<int,KeyValuePair<int, int>> journeys = new Dictionary<int,KeyValuePair<int, int>>();


        static void Main(string[] args)
        {
            //RmitJourneyPlanner.CoreLibraries.Logging.Logger.LogEvent += new RmitJourneyPlanner.CoreLibraries.Logging.LogEventHandler(Logger_LogEvent);



            journeys.Add(1,new KeyValuePair<int, int>(19965, 20039)); //Coburg to ascii
            journeys.Add(2,new KeyValuePair<int, int>(19965,628)); //Coburg to kew
            journeys.Add(3,new KeyValuePair<int, int>(20013, 20042)); //Bell to box hill
            journeys.Add(4,new KeyValuePair<int, int>(20589,19843)); //Barkers road to parliament
            journeys.Add(5,new KeyValuePair<int, int>(18536, 4141));//Abbasford st inter to girtrude

            string instanceUUID = Guid.NewGuid().ToString();
            RmitJourneyPlanner.CoreLibraries.Logging.Logger.LogEvent +=
         (sender, message) => { Console.WriteLine("[{0}]: {1}", sender, message); };

            while (true)
            {

                string runUUID = "None";
                
                MetlinkDataProvider provider = new MetlinkDataProvider();
     
                try
                {



                    while (true)
                    {
                        foreach (var journey in journeys)
                        {
                            foreach (var sType in Enum.GetNames(typeof (SearchType)))
                            {

                                SearchType s = (SearchType) Enum.Parse(typeof (SearchType), sType);
                                

                                int typeNumber = (int) s;
                                int journeyNumber = journey.Key;

                                int testNumber = (typeNumber*5) + journeyNumber;


                                

                                if (sType.Contains("RW"))
                                {
                                    continue;

                                }

                                using (StreamWriter writer = new StreamWriter("TestNums.txt", true))
                                {
                                    writer.WriteLine("{0},{1},{2}", testNumber, sType,journeyNumber);
                                }


                                var properties = new EvolutionaryProperties();
                                properties.NetworkDataProviders = new[] {provider};
                                properties.Bidirectional = false;
                                properties.PointDataProviders = new[] {new WalkingDataProvider()};
                                properties.ProbMinDistance = 0.7;
                                properties.ProbMinTransfers = 0.2;
                                properties.MaximumWalkDistance = 1.5;
                                properties.PopulationSize = 100;
                                properties.MaxDistance = 0.5;
                                properties.InfectionRate = 0.2;
                                properties.DepartureTime = DateTime.Parse("2012/11/10 9:30 AM");
                                    //DateTime.Parse(date + " "+ time);
                                properties.NumberToKeep = 25;
                                properties.MutationRate = 0.25;
                                properties.CrossoverRate = 0.7;
                                //properties.RouteGenerator = new AlRouteGenerator(properties);


                                properties.SearchType = s;
                                properties.RouteGenerator = new DFSRoutePlanner(properties);
                                properties.Mutator = new StandardMutator(properties);
                                properties.Breeder = new TimeBlendBreeder(properties);
                                properties.FitnessFunction = new AlFitnessFunction(properties);
                                properties.Database = new MySqlDatabase("20110606fordistributionforrmit");
                                //properties.Destination = new MetlinkNode(628, provider);// kew
                                //properties.Origin = new MetlinkNode(9601, provider);// reynard st

                                //properties.Origin = new MetlinkNode(18536, provider); //abbotsford interchange
                                properties.Origin = new MetlinkNode(journey.Value.Key, provider); // Coburg
                                properties.Destination = new MetlinkNode(journey.Value.Value, provider); //ascot vale
                                //properties.Origin = new MetlinkNode(19036,provider); //Cotham (o)
                                //properties.Destination = new MetlinkNode(19943, provider); // Caulfielsd
                                //properties.Destination = new MetlinkNode(19933, provider); // Darebinor whateves
                                //properties.Destination = new MetlinkNode(19394,provider); // other kew
                                // properties.Destination = new MetlinkNode(4141, provider); // Jolimont
                                //properties.Destination = new MetlinkNode(628,metlinkProvider);
                                // new TerminalNode(-1, destination);
                                // metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, destination), 0);
                                //properties.Origin = new MetlinkNode(19965, metlinkProvider);//
                                //metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, origin), 0);

                                //metlinkProvider.GetNodeClosestToPoint(new TerminalNode(-1, origin), 0);

                                properties.Destination.RetrieveData();
                                properties.Origin.RetrieveData();
                                properties.Objectives = new[]
                                                            {
                                                                FitnessParameter.NormalisedChanges,
                                                                FitnessParameter.TotalJourneyTime,
                                                                FitnessParameter.DiversityMetric
                                                            };

                                properties.Database.Open();
                                //properties.DataStructures = new DataStructures(properties);

                                Stopwatch sw = Stopwatch.StartNew();
                                properties.Planner = new MoeaRoutePlanner(properties);

                                properties.Planner.Start();

                                runUUID = Guid.NewGuid().ToString();
                                string runDirectory = String.Format("./Results/{0}/{1}/", testNumber, runUUID);

                                Directory.CreateDirectory(runDirectory);
                                
                                /*
                                using (
                                        var writer = new StreamWriter(new GZipStream(new FileStream(String.Format("{0}iteration.{1}.csv.gz", runDirectory, 0), FileMode.Create), CompressionMode.Compress)))
                                {
                                    int count = 0;
                                    foreach (var member in properties.Planner.Population)
                                    {
                                        string nodeIds = "";
                                        string nodeNames = "";
                                        string nodeCoords = "";
                                        foreach (var node in member.Route)
                                        {
                                            nodeIds += node.Node.Id.ToString() + "|";
                                            nodeNames += ((MetlinkNode) node.Node).StopSpecName + "|";
                                            nodeCoords += node.Node.Latitude + "@" + node.Node.Longitude+ "|";

                                        }

                                        writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", count++,member.Rank, member.Distance,
                                                         member.departureTime, member.Fitness.TotalJourneyTime.TotalMinutes,
                                                         member.Fitness.Changes, member.Fitness.DiversityMetric,
                                                         member.Fitness.TotalDistance, nodeIds, nodeNames, nodeCoords);
                                    }

                                }


                                */
                                
                                for (int i = 0; i < 100; i++)
                                {
                                    properties.Planner.SolveStep();

                                    /*
                                    using (
                                        var writer =
                                            new StreamWriter(
                                                new GZipStream(
                                                    new FileStream(
                                                        String.Format("{0}iteration.{1}.csv.gz", runDirectory, i),
                                                        FileMode.Create), CompressionMode.Compress)))
                                    {
                                     * */
                                    using (
                                        var writer =
                                            new StreamWriter(
                                                String.Format("{0}iteration.{1}.csv", runDirectory, i),false))
                                    {
                                        int count = 0;
                                        foreach (var member in properties.Planner.Population)
                                        {
                                            string nodeIds = "";
                                            string nodeNames = "";
                                            string nodeCoords = "";
                                            string legString = "";
                                            foreach (var node in member.Route)
                                            {
                                                nodeIds += node.Node.Id.ToString() + "|";
                                                nodeNames += ((MetlinkNode) node.Node).StopSpecName + "|";
                                                nodeCoords += node.Node.Latitude + "@" + node.Node.Longitude + "|";

                                            }

                                            foreach (var leg in member.Fitness.JourneyLegs)
                                            {
                                                legString += String.Format("[{0}]({1})--<{4}({5} - {3})>--[{2}]({6})|",
                                                                           leg.Origin.StopSpecName, leg.DepartureTime,
                                                                           leg.Destination.StopSpecName, leg.RouteId1,
                                                                           leg.TotalTime, leg.TransportMode,
                                                                           leg.DepartureTime + leg.TotalTime);
                                            }

                                            writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                                                             count++, member.Rank, member.Distance,
                                                             member.departureTime,
                                                             member.Fitness.TotalJourneyTime.TotalMinutes,
                                                             member.Fitness.Changes, member.Fitness.DiversityMetric,
                                                             member.Fitness.TotalDistance, nodeIds, nodeNames,
                                                             nodeCoords, legString, sw.ElapsedMilliseconds);
                                        }

                                    }
                                }
                                 
                                properties.Database.Close();

                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    using (StreamWriter writer = new StreamWriter(String.Format("Error.{0}.log",instanceUUID), true))
                    {
                        writer.WriteLine("[{0}] ({1}) {2} {3}", DateTime.Now, runUUID, e, e.Message, e.StackTrace);

                    }

                }
            }




        }

        static void Logger_LogEvent(object sender, string message)
        {
            Console.WriteLine("[LOGGER: ] " + message);
        }
    }
}
