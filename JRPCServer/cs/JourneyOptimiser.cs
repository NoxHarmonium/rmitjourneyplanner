using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JRPCServer
{
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading;

    using Jayrock.Json;
    using Jayrock.Json.Conversion;

    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.Types;

    public enum OptimiserState
    {
        Waiting,
        Optimising,
        Saving,
        Cancelling,
        Idle,
        Paused

    }


    /// <summary>
    /// Optimises journeys in a first in first out manner.
    /// </summary>
    public class JourneyOptimiser
    {
        /// <summary>
        /// The queue of journeys to be optimised.
        /// </summary>
        private readonly ConcurrentQueue<string> optimiseQueue = new ConcurrentQueue<string>();

        /// <summary>
        /// The wrapper to <see cref="optimiseQueue"/> that enables blocking access.
        /// </summary>
        private readonly BlockingCollection<string> bc;

        /// <summary>
        /// The <see cref="CancellationTokenSource"/> used to cancel blocking access to the queue.
        /// </summary>
        private readonly CancellationTokenSource cTokenSource;

        /// <summary>
        /// The thread that handles optimisation operations.
        /// </summary>
        private readonly Thread optimisationThread;

        /// <summary>
        /// The system journey manager.
        /// </summary>
        private readonly JourneyManager journeyManager;

        /// <summary>
        /// Represents the state of the <see cref="JourneyOptimiser"/> instance.
        /// </summary>
        private OptimiserState state = OptimiserState.Idle;

        /// <summary>
        /// The current iteration the optimiser is up to.
        /// </summary>
        private int currentIteration = 0;

        /// <summary>
        /// The maximum iteration the optimiser will get to before progressing in the queue.
        /// </summary>
        private int maxIterations = 0;

        /// <summary>
        /// A value which determines if the optimisation thread will end after the queue empties.
        /// </summary>
        private bool exitThreadWhenQueueEmpty = false;

        /// <summary>
        /// Stores an exception thrown by the optimisation thread;
        /// </summary>
        private Exception thrownException = null;

        /// <summary>
        /// Stores the current journey that is being optimised.
        /// </summary>
        private Journey currentJourney;

        /// <summary>
        /// Determines whether the optimiser is paused or not.
        /// </summary>
        private bool paused;

        private const string queueFile = "optimisationQueue.json";

        /// <summary>
        /// The list of directories used by the journey optimiser.
        /// </summary>
        //TODO: Make a better way to handle directories.
        private readonly string[] directories = 
		{
			"JSONStore",	
			"JSONStore/Journeys"	
		};

        /// <summary>
        /// Initialises a new instance of the <see cref="JourneyOptimiser"/> class with the specified <see cref="JourneyManager"/> instance.
        /// </summary>
        /// <param name="journeyManager">The journey manager associated with this system.</param>
        public JourneyOptimiser(JourneyManager journeyManager)
        {
            if (journeyManager == null)
            {
                throw new NullReferenceException(Strings.ERR_JM_NULL);
            }
            cTokenSource = new CancellationTokenSource();
            this.journeyManager = journeyManager;
            bc = new BlockingCollection<string>(optimiseQueue);

            Load();

            optimisationThread = new Thread(this.OptimisationLoop);
            optimisationThread.Start();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="JourneyOptimiser"/> and automatically get
        /// the <see cref="JourneyManager"/> from the <see cref="ObjectCache"/>.
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

        /// <summary>
        /// Represents the state of the <see cref="JourneyOptimiser"/> instance.
        /// </summary>
        public OptimiserState State
        {
            get
            {
                return this.state;
            }
        }

        /// <summary>
        /// Gets the current iteration the optimiser is up to.
        /// </summary>
        public int CurrentIteration
        {
            get
            {
                return this.currentIteration;
            }
        }

        /// <summary>
        /// Gets the maximum iteration the optimiser will get to before progressing in the queue.
        /// </summary>
        public int MaxIterations
        {
            get
            {
                return this.maxIterations;
            }
        }

        /// <summary>
        /// Gets an exception thrown by the optimisation thread;
        /// </summary>
        public Exception ThrownException
        {
            get
            {
                return this.thrownException;
            }
        }

        /// <summary>
        /// Stores the current journey that is being optimised.
        /// </summary>
        public Journey CurrentJourney
        {
            get
            {
                return this.currentJourney;
            }
        }

        /// <summary>
        /// Enqueues the journey in the optimisation queue.
        /// </summary>
        /// <param name="journey">The journey you wish to enqueue.</param>
        /// <param name="runs">The amount of times to add the journey to the queue.</param>
        public void EnqueueJourney(Journey journey, int runs)
        {
           EnqueueJourney(journey.Uuid,runs);
        }

        /// <summary>
        /// Enqueues the journey in the optimisation queue.
        /// </summary>
        /// <param name="journeyUuid">The UUID of the journey you wish to enqueue.</param>
        /// <param name="runs">The amount of times to add the journey to the queue.</param>
        public void EnqueueJourney(string journeyUuid, int runs)
        {
            for (int i = 0; i < runs; i++)
            {
                //optimiseQueue.Enqueue(journey.Uuid);
                bc.Add(journeyUuid);
            }
            this.Save();
        }

        /// <summary>
        /// Cancels the current job and halts the optimisation.
        /// </summary>
        public void Cancel()
        {
            paused = false;
            cTokenSource.Cancel();
            this.state = OptimiserState.Cancelling;
            
        }

        /// <summary>
        /// Pauses the current optimisation run at the next available oportunity.
        /// </summary>
        public void Pause()
        {
            if (state == OptimiserState.Optimising)
            {
                paused = true;
                state = OptimiserState.Paused;
            }
        }

        /// <summary>
        /// Resumes the current optimisation run.
        /// </summary>
        public void Resume()
        {
            if (state == OptimiserState.Paused)
            {
                paused = false;
                state = OptimiserState.Optimising;
            }
        }

        /// <summary>
        /// Saves the optimisation queue to a file.
        /// </summary>
        private void Save()
        {
            string dir = directories[0] + "/";
            using (var writer = new StreamWriter(dir + queueFile))
            {
                JsonConvert.Export(bc.ToArray(),new JsonTextWriter(writer));
            }
        }

        private void Load()
        {
            string dir = directories[0] + "/";
            if (File.Exists(dir + queueFile))
            {
                string[] values;
                using (var reader = new StreamReader(dir + queueFile))
                {
                    values = JsonConvert.Import<string[]>(reader);
                }
               
                foreach (var value in values)
                {
                    bc.Add(value);
                }
            }

        }


        /// <summary>
        /// Gets the current optimiser queue.
        /// </summary>
        /// <returns>An array of jounrney UUIDs.</returns>
        public string[] GetQueue()
        {
            return bc.ToArray();

      
            
        }
        
        /// <summary>
        /// Method used for debugging where the optimisation thread will be joined by
        /// the current thread. Also sets a property that causes the optimisation thread
        /// to abort when the queue is empty.
        /// </summary>
        public void WaitOnOptimisation()
        {
            Thread.Sleep(500);
            this.exitThreadWhenQueueEmpty = true;
            optimisationThread.Join();
        }


        /// <summary>
        /// Called internally to process the journey optimisation queue.
        /// </summary>
        private void OptimisationLoop()
        {

            var exportContext = JsonConvert.CreateExportContext();
            exportContext.Register(new CritterExporter());

            //try
            {
                while (!cTokenSource.IsCancellationRequested)
                {
                    string jUuid = String.Empty;
                    Run run = null;
                    try
                    {

                    
                    currentIteration = 0;

                    this.state = OptimiserState.Waiting;
                    if (bc.Count == 0 && this.exitThreadWhenQueueEmpty)
                    {
                        return;
                    }

                    this.Save();
                        
                    jUuid = bc.Take(cTokenSource.Token);
                    this.state = OptimiserState.Optimising;
                    run = new Run();
                    
                    var journey = journeyManager.GetJourney(jUuid);
                    currentJourney = journey;
                    run.JourneyUuid = journey.Uuid;
                    run.TimeStarted = DateTime.Now;
                    var planner = new MoeaRoutePlanner(journey.Properties);
                    planner.Start();
                    var results = new List<Result>(journey.Properties.MaxIterations);
                    maxIterations = journey.Properties.MaxIterations;

                    for (int i = 0; i < journey.Properties.MaxIterations; i++)
                    {
                        currentIteration++;
                        planner.SolveStep();
                        results.Add((Result)planner.IterationResult.Clone());
                        while (paused)
                        {
                            Thread.Sleep(100);
                        }
                        //results.Add(planner.re);
                        if (cTokenSource.IsCancellationRequested)
                        {
                            break;
                        }

                    }
                    run.TimeFinished = DateTime.Now;

                    var maxJT = results.Max(r => r.Population.Max(p => p.Fitness.TotalJourneyTime)).TotalSeconds;
                    var minJT = results.Min(r => r.Population.Min(p => p.Fitness.TotalJourneyTime)).TotalSeconds;
                    var maxCh = results.Max(r => r.Population.Max(p => p.Fitness.Changes));
                    var minCh = results.Min(r => r.Population.Min(p => p.Fitness.Changes));

                    foreach (var result in results)
                    {
                        foreach (var p in result.Population)
                        {
                            p.Fitness.NormalisedChanges = Math.Min(1.0f, (p.Fitness.Changes) / 10.0);
                            p.Fitness.NormalisedJourneyTime = Math.Max(1.0f,p.Fitness.TotalJourneyTime.TotalSeconds / 7200.0f);
                        }
                    }

                    if (!cTokenSource.IsCancellationRequested)
                    {
                        lock (journey.RunUuids)
                        {
                            //TODO: Make this more efficient/better coded
                            var newList = new List<string>(journey.RunUuids);
                            newList.Add(run.Uuid);
                            journey.RunUuids = newList.ToArray();


                            string dir = directories[1] + "/" + journey.Uuid + "/";
                            Directory.CreateDirectory(dir + run.Uuid);
                            using (var resultWriter = new StreamWriter(dir + run.Uuid + "/results.csv", false))
                            {
                                resultWriter.Write("Iteration,Hypervolume,Cardinality,");
                                foreach (var p in Enum.GetValues(typeof(FitnessParameter)))
                                {
                                    resultWriter.Write("{0}, ", p.ToString());
                                }
                                resultWriter.WriteLine();
                               
                                for (int i = 0; i < results.Count; i++)
                                {
                                    using (var writer = new StreamWriter(dir + run.Uuid + "/iteration." + i + ".json"))
                                    {

                                        results[i].Hypervolume = 0.0;
                                            /*results[i].Population.CalculateHyperVolume(
                                                journey.Properties.Objectives,
                                                new double[journey.Properties.Objectives.Length]);*/
                                        exportContext.Export(results[i], new JsonTextWriter(writer));
                                        resultWriter.Write("{0}, {1}, {2}, ", i, results[i].Hypervolume,results[i].Cardinality);
                                        foreach (var p in Enum.GetValues(typeof(FitnessParameter)))
                                        {
                                            resultWriter.Write("{0},",results[i].Population.Average(c => c.Fitness[(FitnessParameter)p]));
                                        }
                                       resultWriter.WriteLine();
                                    }
                                }
                                using (var writer = new StreamWriter(dir + run.Uuid + ".json"))
                                {
                                    exportContext.Export(run, new JsonTextWriter(writer));
                                }
                                results.Clear();
                            }
                        }
                        journeyManager.Save();
                    }
                    

                    currentJourney = null;
                    currentIteration = 0;
                    maxIterations = 0;

                    }
                    catch (Exception e)
                    {

                        StreamWriter logWriter = new StreamWriter("Log.txt");
                        logWriter.WriteLine("Exception: {0} jUUID: {1}, rUUID{2}, ST {3}\n\n",e.Message,jUuid,(run ?? new Run{ErrorCode = -1,JourneyUuid = jUuid}).Uuid,e.StackTrace);
                        logWriter.Close();
                        //throw;
                    }
                   

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



    }
}