// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestOptimisationManager.cs" company="">
//   
// </copyright>
// <summary>
//   The test optimisation manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    #region Using Directives

    using System;

    using NUnit.Framework;

    using RmitJourneyPlanner.CoreLibraries.DataAccess;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Google;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Ptv;
    using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary.Breeders;
    using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary.FitnessFunctions;
    using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary.Mutators;
    using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary.RouteGenerators;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// The test optimisation manager.
    /// </summary>
    [TestFixture]
    public class TestOptimisationManager
    {
        #region Public Methods and Operators

        /// <summary>
        /// The test queue.
        /// </summary>
        /// <exception cref="Exception">
        /// </exception>
        [Test]
        public void TestQueue()
        {
            Journey j = new Journey();
            j.ShortName = "Jeff";
            j.Description = "blah";

            j.RunUuids = new[] { "a", "b", "c" };
            var properties = j.Properties;
            var provider = new PtvDataProvider();
            ObjectCache.RegisterObject(provider);
            properties.NetworkDataProviders = new[] { provider };
            properties.PointDataProviders = new[] { new WalkingDataProvider() };
            properties.PopulationSize = 100;
            properties.DepartureTime = DateTime.Now;
            properties.MutationRate = 0.1;
            properties.CrossoverRate = 0.7;

            // properties.Bidirectional = bidir;
            // properties.RouteGenerator = new AlRouteGenerator(properties);
            properties.SearchType = SearchType.Greedy_BiDir;
            properties.RouteGenerator = new DFSRoutePlanner(properties);
            properties.Mutator = new StandardMutator(properties);
            properties.Breeder = new StandardBreeder(properties);
            properties.FitnessFunction = new AlFitnessFunction(properties);
            properties.Database = new MySqlDatabase();
            properties.SearchType = SearchType.Greedy_BiDir;
            properties.Destination = new PtvNode(22180, provider);
            properties.Origin = new PtvNode(19843, provider);
            properties.Planner = new MoeaJourneyPlanner(properties);
            properties.MaxIterations = 25;
            properties.Objectives = new[]
                {
                   FitnessParameter.Changes, FitnessParameter.PercentTrains, FitnessParameter.PercentBuses 
                };

            JourneyManager jm = new JourneyManager();
            jm.Clean();
            jm.Add(j);
            JourneyOptimiser jo = new JourneyOptimiser(jm);
            jo.EnqueueJourney(j.Uuid, 2);
            jo.WaitOnOptimisation();
            if (jo.ThrownException != null)
            {
                throw jo.ThrownException;
            }
        }

        #endregion
    }
}