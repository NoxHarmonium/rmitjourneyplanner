// -----------------------------------------------------------------------
// <copyright file="TestJP.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Testing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using RmitJourneyPlanner.CoreLibraries.DataAccess;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Google;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.Logging;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Breeders;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.Mutators;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.RouteGenerators;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TestJP
    {
        public TestJP()
        {

        }

        public void Test()
        {

            int iterations = 20;
            INetworkDataProvider metlinkProvider = new MetlinkDataProvider();

            //http://ptv.vic.gov.au/stop/view/StopID

            var testRoutes2 = new[,]
                {
                    { new MetlinkNode(19965, metlinkProvider), new MetlinkNode(19879, metlinkProvider) },
                    { new MetlinkNode(12018, metlinkProvider), new MetlinkNode(18475, metlinkProvider) },
                    { new MetlinkNode(19965, metlinkProvider), new MetlinkNode(19842, metlinkProvider) }
                };

            var testRoutes = new[,]
                {
                    // Long inter-suburban routes
                    {
                        new MetlinkNode(19965, metlinkProvider),
                        new MetlinkNode(19879, metlinkProvider)
                    }, // Coburg - Ringwood
                    {
                        new MetlinkNode(20005, metlinkProvider),
                        new MetlinkNode(19855, metlinkProvider)
                    }, // Epping - Frankston
                    {
                        new MetlinkNode(19990, metlinkProvider),
                        new MetlinkNode(19921, metlinkProvider)
                    }, // Hurstbridge - Werribee
                    {
                        new MetlinkNode(19844, metlinkProvider),
                        new MetlinkNode(20000, metlinkProvider)
                    }, // Belgrave - Watergardens
                    {
                        new MetlinkNode(19886, metlinkProvider),
                        new MetlinkNode(40221, metlinkProvider)
                    }, // Cranbourne - Craigieburn
                    
                    // Lateral inter-suburban routes.
                    {
                        new MetlinkNode(19965, metlinkProvider),
                        new MetlinkNode(19935, metlinkProvider)
                    }, // Coburg - Heidelberg
                    {
                        new MetlinkNode(20005, metlinkProvider),
                        new MetlinkNode(19855, metlinkProvider)
                    }, // Epping - Frankston
                    {
                        new MetlinkNode(19990, metlinkProvider),
                        new MetlinkNode(19246, metlinkProvider)
                    },
                    // East Kew Terminus - Box Hill (r202)
                    {
                        new MetlinkNode(12018, metlinkProvider),
                        new MetlinkNode(18475, metlinkProvider)
                    }, // Yarravile - Highpoint (r223)
                    {
                        new MetlinkNode(4808, metlinkProvider),
                        new MetlinkNode(19649, metlinkProvider)
                    }, // Greensborough - Boxhill
                       
                    // Inner-city routes.
                    {
                        new MetlinkNode(19843, metlinkProvider),
                        new MetlinkNode(22180, metlinkProvider)
                    }, // Parliament - Southern Cross
                    {
                        new MetlinkNode(17882, metlinkProvider),
                        new MetlinkNode(19841, metlinkProvider)
                    },
                    // 9-Spring St/Bourke St  - Flagstaff
                    {
                        new MetlinkNode(19489, metlinkProvider),
                        new MetlinkNode(19973, metlinkProvider)
                    }, // Melbourne Uni - North Melbourne 
                    {
                        new MetlinkNode(18034, metlinkProvider),
                        new MetlinkNode(17901, metlinkProvider)
                    },
                    // 2-King St/La Trobe St - Melbourne Town Hall/Collins St
                    {
                        new MetlinkNode(18450, metlinkProvider),
                        new MetlinkNode(19594, metlinkProvider)
                    },
                    // Casino - Royal Childrens Hospital/Flemington Rd

                    // Commuter Routes
                    {
                        new MetlinkNode(19965, metlinkProvider),
                        new MetlinkNode(19842, metlinkProvider)
                    }, // Coburg - Melbourne Central
                    {
                        new MetlinkNode(19876, metlinkProvider),
                        new MetlinkNode(19841, metlinkProvider)
                    }, // Lilydale  - Flagstaff
                    {
                        new MetlinkNode(19489, metlinkProvider),
                        new MetlinkNode(19921, metlinkProvider)
                    }, // Werribee - North Melbourne 
                    {
                        new MetlinkNode(20005, metlinkProvider),
                        new MetlinkNode(19843, metlinkProvider)
                    }, // Epping - Parliament
                    {
                        new MetlinkNode(19855, metlinkProvider),
                        new MetlinkNode(19854, metlinkProvider)
                    } // Frankston - Flinders St


                };
            bool first = true;
            DateTime time = DateTime.Parse("8:00 AM 7/05/2012");

            bool cont = true;

            while (cont)
            {
                for (int i = 0; i < testRoutes.Length; i++)
                {

                    try
                    {
                        testRoutes[i, 0].RetrieveData();
                        testRoutes[i, 1].RetrieveData();

                        EvolutionaryProperties properties = new EvolutionaryProperties();
                        properties.PointDataProviders.Add(new WalkingDataProvider());
                        properties.NetworkDataProviders.Add(metlinkProvider);
                        properties.ProbMinDistance = 0.7;
                        properties.ProbMinTransfers = 0.2;
                        properties.MaximumWalkDistance = 1.5;
                        properties.PopulationSize = 100;
                        properties.MaxDistance = 0.5;
                        properties.DepartureTime = time;
                        properties.NumberToKeep = 25;
                        properties.MutationRate = 0.1;
                        properties.CrossoverRate = 0.7;
                        //properties.RouteGenerator = new AlRouteGenerator(properties);
                        properties.RouteGenerator = new DFSRoutePlanner(properties);
                        properties.Mutator = new StandardMutator(properties);
                        properties.Breeder = new StandardBreeder(properties);
                        properties.FitnessFunction = new AlFitnessFunction(properties);
                        properties.Database = new MySqlDatabase("20110606fordistributionforrmit");
                        properties.Destination = testRoutes[i, 0];
                        properties.Origin = testRoutes[i, 1];
                        properties.Destination.RetrieveData();


                        properties.Database.Open();
                        //properties.DataStructures = new DataStructures(properties);

                        EvolutionaryRoutePlanner planner = new EvolutionaryRoutePlanner(properties);
                        Stopwatch sw = Stopwatch.StartNew();
                        planner.Start();
                        StreamWriter writer =
                            new StreamWriter(
                                "results/" + testRoutes[i, 0].Id + "-" + testRoutes[i, 1].Id + ".csv", true);
                        writer.WriteLine(
                            "[New iteration {0}-{1} ({2}-{3}) @ {4}]",
                            testRoutes[i, 0].Id,
                            testRoutes[i, 1].Id,
                            testRoutes[i, 0].StopSpecName,
                            testRoutes[i, 1].StopSpecName,
                            DateTime.Now.ToString(CultureInfo.InvariantCulture));

                        Console.WriteLine(
                            "[New iteration {0}-{1} ({2}-{3}) @ {4}]",
                            testRoutes[i, 0].Id,
                            testRoutes[i, 1].Id,
                            testRoutes[i, 0].StopSpecName,
                            testRoutes[i, 1].StopSpecName,
                            DateTime.Now.ToString(CultureInfo.InvariantCulture));



                        for (int j = 0; j < 99; j++)
                        {
                            planner.SolveStep();
                            this.writeInfo(writer, planner, sw.Elapsed);
                        }
                        writer.Close();
                        properties.Database.Close();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Exception!: {0} ({1}). Writing to error log...",e,e.Message);
                        StreamWriter writer = new StreamWriter("error.log",true);
                        writer.WriteLine("[{0}] Exception!: {1} ({2}).\n{3}",DateTime.Now.ToString(CultureInfo.InvariantCulture),e,e.Message,e.StackTrace);
                        writer.WriteLine(
                           "[Last error thrown on: {0}-{1} ({2}-{3}) @ {4}]",
                           testRoutes[i, 0].Id,
                           testRoutes[i, 1].Id,
                           testRoutes[i, 0].StopSpecName,
                           testRoutes[i, 1].StopSpecName,
                           DateTime.Now.ToString(CultureInfo.InvariantCulture));
                        writer.Close();
                    }

                }
                

                var reader = new StreamReader("cont.txt");
                string result = reader.ReadToEnd().Trim();
                switch (result)
                {
                    case "yes":
                        cont = true;
                        break;
                    case "no":
                        cont = false;
                        break;
                    default:
                        Console.WriteLine("Cant read {0}" , ((FileStream)reader.BaseStream).Name);
                        break;
                }
                reader.Close();
                
            }

            
        }

        private void writeInfo(StreamWriter writer, EvolutionaryRoutePlanner planner,TimeSpan time)
        {
            writer.WriteLine(
                        "Average Fitness, Minimum Fitenss, Diversity Metric, Total Time (Iteration), Total Time (Test)");
            writer.WriteLine(
                "{0},{1},{2},{3},{4}",
                planner.Result.AverageFitness,
                planner.Result.MinimumFitness,
                planner.Result.DiversityMetric,
                planner.Result.Totaltime,
                time);
        }
    }
}
