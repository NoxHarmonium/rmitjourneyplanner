using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JayrockClient
{
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading;

    using Jayrock.Json;
    using Jayrock.Json.Conversion;

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
        /// Saves the changes to a file.
        /// </summary>
        private void Save(IEnumerable<Result> results)
        {
            

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
                    currentIteration = 0;

                    this.state = OptimiserState.Waiting;
                    if (bc.Count == 0 && this.exitThreadWhenQueueEmpty)
                    {
                        return;
                    }
                    var jUuid = bc.Take(cTokenSource.Token);
                    this.state = OptimiserState.Optimising;
                    var runUuid = Guid.NewGuid().ToString();
                    var journey = journeyManager.GetJourney(jUuid);
                    currentJourney = journey;
                    var planner = journey.Properties.Planner;
                    planner.Start();
                    var results = new List<Result>(journey.Properties.MaxIterations);
                    maxIterations = journey.Properties.MaxIterations;

                    for (int i = 0; i < journey.Properties.MaxIterations; i++)
                    {
                        currentIteration++;
                        planner.SolveStep();
                        results.Add(planner.IterationResult);
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

                    lock (journey.RunUuids)
                    {
                        //TODO: Make this more efficient/better coded
                        var newList = new List<string>(journey.RunUuids);
                        journey.RunUuids = newList.ToArray();


                        string dir = directories[1] + "/" + journey.Uuid + "/";
                        
                        Directory.CreateDirectory(dir);
                        using (var writer = new StreamWriter(dir +  runUuid + ".json"))
                        {

                            exportContext.Export(results, new JsonTextWriter(writer));
                        }
                    }

                    journeyManager.Save();

                    if (!cTokenSource.IsCancellationRequested)
                    {
                        Save(results);
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