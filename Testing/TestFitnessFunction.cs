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
    using System.Threading;

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



            properties.DepartureTime = DateTime.Parse("27/06/2012 10:15 AM");
    		properties.NetworkDataProviders = new [] {provider};
            properties.PointDataProviders = new [] {new WalkingDataProvider()};
            properties.FitnessFunction = new AlFitnessFunction(properties);
            properties.Database = new MySqlDatabase("20110606fordistributionforrmit");

            /*
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
            */
            Console.WriteLine("Press Enter.");
            Console.ReadLine();
            for (int i = 0; i < 12; i++)
            {
                properties.DepartureTime = DateTime.Parse("27/06/2012 6:00 PM").AddMinutes(i*5);
                
                var testRoute = new Route(-1)
                    {
                        new MetlinkNode(19965, provider),
                        new MetlinkNode(19966, provider),
                        new MetlinkNode(19967, provider),
                        new MetlinkNode(19968, provider),
                        new MetlinkNode(19969, provider),
                        new MetlinkNode(19970, provider),
                        new MetlinkNode(19971, provider),
                        new MetlinkNode(19972, provider),
                        new MetlinkNode(19973, provider),
                        new MetlinkNode(20041, provider),
                        new MetlinkNode(20040, provider),
                        new MetlinkNode(20039, provider),
                        
                    };

                
                /**
                var testRoute = new Route(-1)
                    {
                        new MetlinkNode(9335 , provider),
new MetlinkNode(9334 , provider),
new MetlinkNode(9333 , provider),
new MetlinkNode(9332 , provider),
new MetlinkNode(9331 , provider),
new MetlinkNode(9330 , provider),
new MetlinkNode(9329 , provider),
new MetlinkNode(9328 , provider),
new MetlinkNode(9327 , provider),
new MetlinkNode(9326 , provider),
new MetlinkNode(9325 , provider),
new MetlinkNode(9324 , provider),
new MetlinkNode(9323 , provider),
new MetlinkNode(20754 , provider),
new MetlinkNode(9321 , provider),
new MetlinkNode(9320 , provider),
new MetlinkNode(9319 , provider),
new MetlinkNode(9318 , provider),
new MetlinkNode(9317 , provider),
new MetlinkNode(9316 , provider),
new MetlinkNode(9313 , provider),
new MetlinkNode(9312 , provider),
new MetlinkNode(20756 , provider),
new MetlinkNode(9310 , provider),
new MetlinkNode(20757 , provider),

                


                    };
               */
                var score = properties.FitnessFunction.GetFitness(testRoute);

                Console.WriteLine("Score: {0}", score);
                //Thread.Sleep(1000);
            }
            //properties.Database.Open();
            //properties.DataStructures = new DataStructures(properties);

            //EvolutionaryRoutePlanner planner = new EvolutionaryRoutePlanner(properties);
            // Stopwatch sw = Stopwatch.StartNew();
            // planner.Start();
        }

    }
}
