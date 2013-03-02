// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JourneyOptimiser.cs" company="">
//   
// </copyright>
// <summary>
//   The optimiser state.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    #region Using Directives

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Jayrock.Json;
    using Jayrock.Json.Conversion;

    using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// The optimiser state.
    /// </summary>
    public enum OptimiserState
    {
        /// <summary>
        /// The waiting.
        /// </summary>
        Waiting,

        /// <summary>
        /// The optimising.
        /// </summary>
        Optimising,

        /// <summary>
        /// The saving.
        /// </summary>
        Saving,

        /// <summary>
        /// The cancelling.
        /// </summary>
        Cancelling,

        /// <summary>
        /// The idle.
        /// </summary>
        Idle,

        /// <summary>
        /// The paused.
        /// </summary>
        Paused
    }

    /// <summary>
    /// Optimises journeys in a first in first out manner.
    /// </summary>
    public class JourneyOptimiser
    {
        #region Constants and Fields

        /// <summary>
        /// The queue file.
        /// </summary>
        private const string queueFile = "optimisationQueue.json";

        /// <summary>
        ///   The wrapper to <see cref = "optimiseQueue" /> that enables blocking access.
        /// </summary>
        private readonly BlockingCollection<string> bc;

        /// <summary>
        ///   The <see cref = "CancellationTokenSource" /> used to cancel blocking access to the queue.
        /// </summary>
        private readonly CancellationTokenSource cTokenSource;

        /// <summary>
        ///   The list of directories used by the journey optimiser.
        /// </summary>
        // TODO: Make a better way to handle directories.
        private readonly string[] directories = { "JSONStore", "JSONStore/Journeys" };

        /// <summary>
        ///   The system journey manager.
        /// </summary>
        private readonly JourneyManager journeyManager;

        /// <summary>
        ///   The thread that handles optimisation operations.
        /// </summary>
        private readonly Thread optimisationThread;

        /// <summary>
        ///   The queue of journeys to be optimised.
        /// </summary>
        private readonly ConcurrentQueue<string> optimiseQueue = new ConcurrentQueue<string>();

        /// <summary>
        ///   Stores an exception thrown by the optimisation thread;
        /// </summary>
        private readonly Exception thrownException;

        /// <summary>
        ///   The current iteration the optimiser is up to.
        /// </summary>
        private int currentIteration;

        /// <summary>
        ///   Stores the current journey that is being optimised.
        /// </summary>
        private Journey currentJourney;

        /// <summary>
        ///   A value which determines if the optimisation thread will end after the queue empties.
        /// </summary>
        private bool exitThreadWhenQueueEmpty;

        /// <summary>
        ///   The maximum iteration the optimiser will get to before progressing in the queue.
        /// </summary>
        private int maxIterations;

        /// <summary>
        ///   Determines whether the optimiser is paused or not.
        /// </summary>
        private bool paused;

        /// <summary>
        ///   Represents the state of the <see cref = "JourneyOptimiser" /> instance.
        /// </summary>
        private OptimiserState state = OptimiserState.Idle;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JourneyOptimiser"/> class. 
        /// Initialises a new instance of the <see cref="JourneyOptimiser"/> class with the specified <see cref="JourneyManager"/> instance.
        /// </summary>
        /// <param name="journeyManager">
        /// The journey manager associated with this system.
        /// </param>
        public JourneyOptimiser(JourneyManager journeyManager)
        {
            if (journeyManager == null)
            {
                throw new NullReferenceException(Strings.ERR_JM_NULL);
            }

            this.cTokenSource = new CancellationTokenSource();
            this.journeyManager = journeyManager;
            this.bc = new BlockingCollection<string>(this.optimiseQueue);

            this.Load();

            this.optimisationThread = new Thread(this.OptimisationLoop);
            this.optimisationThread.Start();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JourneyOptimiser"/> class. 
        ///   Initialises a new instance of the <see cref="JourneyOptimiser"/> and automatically get
        ///   the <see cref="JourneyManager"/> from the <see cref="ObjectCache"/>.
        /// </summary>
        public JourneyOptimiser()
            : this(ObjectCache.GetObject<JourneyManager>())
        {
            /*
            var jm = ObjectCache.GetObject<JourneyManager>();
            if (jm == null)
            {
                throw new Exception(Strings.ERR_JM_NOTFOUND);
            }
             * */
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the current iteration the optimiser is up to.
        /// </summary>
        public int CurrentIteration
        {
            get
            {
                return this.currentIteration;
            }
        }

        /// <summary>
        ///   Stores the current journey that is being optimised.
        /// </summary>
        public Journey CurrentJourney
        {
            get
            {
                return this.currentJourney;
            }
        }

        /// <summary>
        ///   Gets the maximum iteration the optimiser will get to before progressing in the queue.
        /// </summary>
        public int MaxIterations
        {
            get
            {
                return this.maxIterations;
            }
        }

        /// <summary>
        ///   Represents the state of the <see cref = "JourneyOptimiser" /> instance.
        /// </summary>
        public OptimiserState State
        {
            get
            {
                return this.state;
            }
        }

        /// <summary>
        ///   Gets an exception thrown by the optimisation thread;
        /// </summary>
        public Exception ThrownException
        {
            get
            {
                return this.thrownException;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Cancels the current job and halts the optimisation.
        /// </summary>
        public void Cancel()
        {
            this.paused = false;
            this.cTokenSource.Cancel();
            this.state = OptimiserState.Cancelling;
        }

        /// <summary>
        /// Enqueues the journey in the optimisation queue.
        /// </summary>
        /// <param name="journey">
        /// The journey you wish to enqueue.
        /// </param>
        /// <param name="runs">
        /// The amount of times to add the journey to the queue.
        /// </param>
        public void EnqueueJourney(Journey journey, int runs)
        {
            EnqueueJourney(journey.Uuid, runs);
        }

        /// <summary>
        /// Enqueues the journey in the optimisation queue.
        /// </summary>
        /// <param name="journeyUuid">
        /// The UUID of the journey you wish to enqueue.
        /// </param>
        /// <param name="runs">
        /// The amount of times to add the journey to the queue.
        /// </param>
        public void EnqueueJourney(string journeyUuid, int runs)
        {
            for (int i = 0; i < runs; i++)
            {
                // optimiseQueue.Enqueue(journey.Uuid);
                this.bc.Add(journeyUuid);
            }

            this.Save();
        }

        /// <summary>
        /// Gets the current optimiser queue.
        /// </summary>
        /// <returns>
        /// An array of journey UUIDs.
        /// </returns>
        public string[] GetQueue()
        {
            return this.bc.ToArray();
        }

        /// <summary>
        /// Pauses the current optimisation run at the next available oportunity.
        /// </summary>
        public void Pause()
        {
            if (this.state == OptimiserState.Optimising)
            {
                this.paused = true;
                this.state = OptimiserState.Paused;
            }
        }

        /// <summary>
        /// Resumes the current optimisation run.
        /// </summary>
        public void Resume()
        {
            if (this.state == OptimiserState.Paused)
            {
                this.paused = false;
                this.state = OptimiserState.Optimising;
            }
        }

        /// <summary>
        /// Method used for debugging where the optimisation thread will be joined by
        ///   the current thread. Also sets a property that causes the optimisation thread
        ///   to abort when the queue is empty.
        /// </summary>
        public void WaitOnOptimisation()
        {
            Thread.Sleep(500);
            this.exitThreadWhenQueueEmpty = true;
            this.optimisationThread.Join();
        }

        public void Remove(Journey journey)
        {
            if (!bc.Contains(journey.Uuid))
                return;

            this.Pause();

            var journeys = new List<string>(bc.Count);
            while (bc.Count > 0)
            {
                journeys.Add(bc.Take());
            }

            foreach (String j in journeys)
            {
                if (j != journey.Uuid)
                {
                    bc.Add(j);
                }
            }

            this.Resume();


        }

        #endregion

        #region Methods

        /// <summary>
        /// The load.
        /// </summary>
        private void Load()
        {
            string dir = this.directories[0] + "/";
            if (File.Exists(dir + queueFile))
            {
                string[] values;
                using (var reader = new StreamReader(dir + queueFile))
                {
                    values = JsonConvert.Import<string[]>(reader);
                }

                foreach (var value in values)
                {
                    this.bc.Add(value);
                }
            }
        }

        /// <summary>
        /// Called internally to process the journey optimisation queue.
        /// </summary>
        private void OptimisationLoop()
        {
            var exportContext = JsonConvert.CreateExportContext();
            exportContext.Register(new CritterExporter());
            {
                // try
                while (!this.cTokenSource.IsCancellationRequested)
                {
                    string jUuid = string.Empty;
                    Run run = null;
                    //try
                    {
                        this.currentIteration = 0;

                        this.state = OptimiserState.Waiting;
                        if (this.bc.Count == 0 && this.exitThreadWhenQueueEmpty)
                        {
                            return;
                        }

                        this.Save();

                        jUuid = this.bc.Take(this.cTokenSource.Token);
                        this.state = OptimiserState.Optimising;
                        run = new Run();

                        var journey = this.journeyManager.GetJourney(jUuid);
                        this.currentJourney = journey;
                        run.JourneyUuid = journey.Uuid;
                        run.TimeStarted = DateTime.Now;
                        var planner = new MoeaJourneyPlanner(journey.Properties);
                        var results = new List<Result>(journey.Properties.MaxIterations);
                        this.maxIterations = journey.Properties.MaxIterations;
                        planner.Start();


                        for (int i = 0; i < journey.Properties.MaxIterations; i++)
                        {
                            this.currentIteration++;
                            planner.SolveStep();
                            results.Add((Result)planner.IterationResult.Clone());
                            while (this.paused)
                            {
                                Thread.Sleep(100);
                            }

                            // results.Add(planner.re);
                            if (this.cTokenSource.IsCancellationRequested)
                            {
                                break;
                            }
                        }

                        run.TimeFinished = DateTime.Now;
                        var maxTT = results.Max(r => r.Population.Max(p => p.Fitness.TotalTravelTime)).TotalSeconds;
                        var minTT = results.Min(r => r.Population.Min(p => p.Fitness.TotalTravelTime)).TotalSeconds;
                        var maxJT = results.Max(r => r.Population.Max(p => p.Fitness.TotalJourneyTime)).TotalSeconds;
                        var minJT = results.Min(r => r.Population.Min(p => p.Fitness.TotalJourneyTime)).TotalSeconds;
                        var maxCh = results.Max(r => r.Population.Max(p => p.Fitness.Changes));
                        var minCh = results.Min(r => r.Population.Min(p => p.Fitness.Changes));

                        foreach (var result in results)
                        {
                            foreach (var p in result.Population)
                            {
                                p.Fitness.NormalisedChanges = Math.Min(
                                    1.0f, p.Fitness.Changes / (double)(maxCh - minCh));
                                p.Fitness.NormalisedJourneyTime = Math.Max(
                                    1.0f, p.Fitness.TotalJourneyTime.TotalSeconds / (maxJT - minJT));
                                p.Fitness.NormalisedTravelTime = Math.Max(
                                    1.0f, p.Fitness.TotalTravelTime.TotalSeconds / (maxTT - minTT));
                            }
                        }

                        if (!this.cTokenSource.IsCancellationRequested)
                        {
                            lock (journey.RunUuids)
                            {
                                // TODO: Make this more efficient/better coded
                                var newList = new List<string>(journey.RunUuids);
                                newList.Add(run.Uuid);
                                journey.RunUuids = newList.ToArray();

                                string dir = this.directories[1] + "/" + journey.Uuid + "/";
                                Directory.CreateDirectory(dir + run.Uuid);
                                using (var resultWriter = new StreamWriter(dir + run.Uuid + "/results.csv", false))
                                {
                                    resultWriter.Write("Iteration,Hypervolume,Cardinality,");
                                    foreach (var p in Enum.GetValues(typeof(FitnessParameter)))
                                    {
                                        resultWriter.Write("{0}, ", p);
                                    }

                                    resultWriter.WriteLine();

                                    for (int i = 0; i < results.Count; i++)
                                    {
                                        using (
                                            var writer = new StreamWriter(dir + run.Uuid + "/iteration." + i + ".json"))
                                        {
                                            results[i].Hypervolume = 0.0;

                                            /*results[i].Population.CalculateHyperVolume(
                                                journey.Properties.Objectives,
                                                new double[journey.Properties.Objectives.Length]);*/
                                            exportContext.Export(results[i], new JsonTextWriter(writer));
                                            resultWriter.Write(
                                                "{0}, {1}, {2}, ", i, results[i].Hypervolume, results[i].Cardinality);
                                            foreach (var p in Enum.GetValues(typeof(FitnessParameter)))
                                            {
                                                resultWriter.Write(
                                                    "{0},",
                                                    results[i].Population.Average(c => c.Fitness[(FitnessParameter)p]));
                                            }

                                            resultWriter.WriteLine();
                                        }

                                        // For the last result entry, Save simplified file.
                                        if (i == results.Count - 1)
                                        {
                                            using (
                                           var writer = new StreamWriter(dir + run.Uuid + "/simple.json"))
                                            {
                                                var grouped = from p in results[i].Population
                                                              group p by p.Route
                                                                  into g
                                                                  select g;

                                                var candidates = new List<Critter>();
                                                foreach (var g in grouped)
                                                {
                                                    TimeSpan minTime = TimeSpan.MaxValue;
                                                    Critter minCrit = null;

                                                    foreach (var critter in g)
                                                    {
                                                        if (critter.Fitness.TotalJourneyTime < minTime)
                                                        {
                                                            minTime = critter.Fitness.TotalJourneyTime;
                                                            minCrit = critter;
                                                        }
                                                    }
                                                    candidates.Add(minCrit);
                                                }

                                                exportContext.Export(candidates.OrderBy(c => c.Fitness.TotalJourneyTime), new JsonTextWriter(writer));
                                            }

                                        }
                                    }

                                    using (var writer = new StreamWriter(dir + run.Uuid + ".json"))
                                    {
                                        exportContext.Export(run, new JsonTextWriter(writer));
                                    }

                                    results.Clear();
                                }
                            }

                            this.journeyManager.Save();
                        }

                        this.currentJourney = null;
                        this.currentIteration = 0;
                        this.maxIterations = 0;
                    }
                    /*
                    catch (Exception e)
                    {
                        StreamWriter logWriter = new StreamWriter("Log.txt");
                        logWriter.WriteLine(
                            "Exception: {0} jUUID: {1}, rUUID{2}, ST {3}\n\n", 
                            e.Message, 
                            jUuid, 
                            (run ?? new Run { ErrorCode = -1, JourneyUuid = jUuid }).Uuid, 
                            e.StackTrace);
                        logWriter.Close();

                        // throw;
                    }
                     */
                }
            }

            /*
            catch (Exception e)
            {
                //this.thrownException = e;
                throw;
            }
            finally
            {
                this.state = OptimiserState.Idle;
            }
             * */
        }

        /// <summary>
        /// Get the <see cref="JsonObject"/> representing the simplified final result of the optimisation run.
        /// </summary>
        /// <param name="uuid">The UUID of the journey you wish to get the results for.</param>
        /// <returns>A <see cref="JsonObject"/> or null if the run has no matching result.</returns>
        public JsonArray GetJourneyResult(string uuid)
        {
            string dir = this.directories[1] + "/" + uuid + "/";
            string filename;
            try
            {
                var dirInfo = new DirectoryInfo(dir);
                var latestDir = dirInfo.GetDirectories().OrderBy(d => d.CreationTime).First();
                filename = latestDir.FullName + "/simple.json";
            }
            catch (Exception)
            {
                return null;
            }

            if (!File.Exists(filename))
            {
                return null;
            }

            using (var reader = new StreamReader(filename))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return JsonConvert.Import<JsonArray>(jsonReader);
                }
            }
        }


        /// <summary>
        /// Get the <see cref="JsonObject"/> representing a specific iteration of the optimisation run.
        /// </summary>
        /// <param name="uuid">The UUID of the journey you wish to get the results for.</param>
        /// <param name="iteration">The iteration number you want to get the results from.</param>
        /// <returns>A <see cref="JsonObject"/> or null if the run has no matching result.</returns>
        public JsonArray GetJourneyResult(string uuid, int iteration)
        {
            string dir = this.directories[1] + "/" + uuid + "/";
            string filename;

            try
            {
                var dirInfo = new DirectoryInfo(dir);
                var latestDir = dirInfo.GetDirectories().OrderBy(d => d.CreationTime).First();
                filename = latestDir.FullName + "/iteration." + iteration + ".json";
            }
            catch (Exception)
            {
                return null;
            }
           
            if (!File.Exists(filename))
            {
                return null;
            }

            using (var reader = new StreamReader(filename))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return JsonConvert.Import<JsonArray>(jsonReader);
                }
            }
        }

        /// <summary>
        /// Saves the optimisation queue to a file.
        /// </summary>
        private void Save()
        {
            string dir = this.directories[0] + "/";
            using (var writer = new StreamWriter(dir + queueFile))
            {
                JsonConvert.Export(this.bc.ToArray(), new JsonTextWriter(writer));
            }
        }

        #endregion
    }
}