using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JayrockClient.cs
{
    using System.Collections.Concurrent;
    using System.Threading;

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
        /// The <see cref="CancellationToken"/> used to cancel blocking access to the queue.
        /// </summary>
        private CancellationToken cToken;

        /// <summary>
        /// The thread that handles optimisation operations.
        /// </summary>
        private readonly Thread optimisationThread;

        /// <summary>
        /// The system journey manager.
        /// </summary>
        private JourneyManager journeyManager;

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
            bc = new BlockingCollection<string>(optimiseQueue);
            optimisationThread = new Thread(this.OptimisationLoop);
            optimisationThread.Start();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="JourneyOptimiser"/> and automatically get
        /// the <see cref="JourneyManager"/> from the <see cref="ObjectCache"/>.
        /// </summary>
    public JourneyOptimiser()
        {
            var jm = ObjectCache.GetObject<JourneyManager>();
            if (jm == null)
            {
                throw new Exception(Strings.ERR_JM_NOTFOUND);
            }

        }
       
        /// <summary>
        /// Enqueues the journey in the optimisation queue.
        /// </summary>
        /// <param name="journey">The journey you wish to enqueue.</param>
        /// <param name="runs">The amount of times to add the journey to the queue.</param>
        public void EnqueueJourney(Journey journey, int runs)
        {
            for (int i = 0; i < runs; i++)
            {
                //optimiseQueue.Enqueue(journey.Uuid);
                bc.Add(journey.Uuid);
            }
        }

        /// <summary>
        /// Called internally to process the journey optimisation queue.
        /// </summary>
        private void OptimisationLoop()
        {
            while (!cToken.IsCancellationRequested)
            {
                var jUuid = bc.Take(cToken);
                var journey = journeyManager.GetJourney(jUuid);

            }
        }



    }
}