// -----------------------------------------------------------------------
// <copyright file="TestFitnessFunction.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Testing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TestFitnessFunction
    {
        public TestFitnessFunction()
        {


            MetlinkDataProvider provider = new MetlinkDataProvider();

            EvolutionaryProperties properties = new EvolutionaryProperties();



            properties.DepartureTime = DateTime.Parse("13/07/2012 2:00 PM");
            properties.PointDataProviders.Add(new WalkingDataProvider());
            properties.NetworkDataProviders.Add(provider);
            properties.FitnessFunction = new AlFitnessFunction(properties);
            properties.Database = new MySqlDatabase("20110606fordistributionforrmit");


            Route testRoute = new Route(-1)
                {
                    
                    new MetlinkNode(19967,provider),
                    new MetlinkNode(19966,provider),
                    new MetlinkNode(19965,provider),
                    new MetlinkNode(9327,provider),
                    new MetlinkNode(9326,provider),
                    new MetlinkNode(9325,provider),
                    new MetlinkNode(9324,provider),
                    new MetlinkNode(9323,provider),
                    new MetlinkNode(20754,provider),
                    new MetlinkNode(9321,provider),
                    new MetlinkNode(9320,provider),
                    new MetlinkNode(9319,provider),
                    new MetlinkNode(9318,provider),
                    new MetlinkNode(9317,provider),
                    new MetlinkNode(9316,provider),
                    new MetlinkNode(9313,provider),
                    new MetlinkNode(9312,provider),
                    new MetlinkNode(20756,provider)
                };

            foreach (var node in testRoute)
            {
                node.RetrieveData();
                Console.WriteLine(node.ToString());
            }
            Fitness score = properties.FitnessFunction.GetFitness(testRoute);
            /*
            testRoute = new Route(-1)
                {
                    
                    new MetlinkNode(19876,provider),
                    new MetlinkNode(19877,provider)
                };
            score = properties.FitnessFunction.GetFitness(testRoute);
             * */
            Console.WriteLine("Score: {0}", score);
            //properties.Database.Open();
            //properties.DataStructures = new DataStructures(properties);

            //EvolutionaryRoutePlanner planner = new EvolutionaryRoutePlanner(properties);
            // Stopwatch sw = Stopwatch.StartNew();
            // planner.Start();
        }

    }
}
