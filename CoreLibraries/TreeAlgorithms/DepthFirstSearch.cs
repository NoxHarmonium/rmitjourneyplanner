// -----------------------------------------------------------------------
// <copyright file="DepthFirstSearch.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.TreeAlgorithms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    /// <summary>
    /// Performs a depth first search on a graph.
    /// </summary>
    public abstract class DepthFirstSearch<T>
    {
        protected HashSet<T>[] Visited = new HashSet<T>[2];
        protected INetworkDataProvider Provider;
        protected T Origin;

        protected int DepthLimit = 16;
        protected T Goal;

        [ThreadStatic]
        protected static int threadId;

        private bool finished = false;
        protected bool Bidirectional;

        protected Mutex mutex = new Mutex();


        protected T[] current = new T[2];

     
        protected T[][] results = new T[2][];




        private int iterations;

        private DateTime startTime;


        protected DepthFirstSearch(int depth, bool bidirectional, T origin, T goal)
            : this(bidirectional, origin, goal)
        {
            this.DepthLimit = depth;
        }

        /// <summary>
        /// Instantiates a new instance of the DepthFirstSerch class.
        /// </summary>
        /// <param name="provider">The data provider which the graph can be derived from.</param>
        /// <param name="bidirectional"> </param>
        /// <param name="origin">The origin node.</param>
        /// <param name="goal">The destination node.</param>
        protected DepthFirstSearch(bool bidirectional, T origin, T goal)
        {
            this.Visited[0] = new HashSet<T>();
            if (bidirectional)
            {
                this.Visited[1] = new HashSet<T>();
            }
            this.Origin = origin;
            this.Goal = goal;
            this.Bidirectional = bidirectional;

        }

        public int Iterations
        {
            get
            {
                return iterations;
            }
        }

       

        /// <summary>
        /// Executes the Depth First Search.
        /// </summary>
        /// <returns></returns>
        public T[] Run()
        {
            this.iterations = 0;
            startTime = DateTime.Now;

            if (!this.Bidirectional)
            {
                return this.RunDFS(this.Origin, 0);
            }

            var threadA = new Thread(delegate()
            {

                Thread.CurrentThread.Name = "Thread A";
                results[threadId] = this.RunDFS(this.Origin, 0);

            });
            var threadB = new Thread(delegate()
                {
                    Thread.CurrentThread.Name = "Thread B";
                    threadId = 1;
                    results[threadId] = this.RunDFS(this.Goal, 0);
                });
            threadA.Start();
            threadB.Start();
            threadA.Join();
            threadB.Join();

            return results[0].Concatenate(results[1]);
        }

        protected T[] RunDFS(T node, int depth)
        {
            if (Bidirectional)
            {
                mutex.WaitOne();
            }
            if (finished)
            {
                mutex.ReleaseMutex();
                return new[] { node };

            }
            this.Visited[threadId].Add(node);
            this.iterations++;
            T[] path = null;
            //if (iterations % 10000 == 0)
            {
                Console.CursorLeft = 0;
                Console.Write("[" + Thread.CurrentThread.Name+ "] - " + iterations + " , " + (iterations / (DateTime.Now - startTime).TotalSeconds).ToString() + " i/s");

            }

            if (Bidirectional)
            {

                if (EqualityComparer<T>.Default.Equals(node, current[threadId == 0 ? 1 : 0]))
                {
                    finished = true;
                    mutex.ReleaseMutex();
                    return new[] { node };
                }

            }
            else
            {
                if (EqualityComparer<T>.Default.Equals(node, Goal))
                {
                    return new[] { node };
                }
            }

            current[threadId] = node;

            if (depth > this.DepthLimit)
            {
                if (Bidirectional)
                {
                    mutex.ReleaseMutex();
                }
                return null;
            }
            T[] children = this.OrderChildren(GetChildren(node));

            foreach (T child in children)
            {
                if (this.Visited[threadId].Contains(child))
                {
                    continue;
                }
                //var newVisited = new HashSet<T>(visited) { current };

                path = this.RunDFS(child, depth + 1);


                if (path == null)
                {
                    continue;
                }

                var newPath = new T[path.Length + 1];
                Array.Copy(path, newPath, path.Length);
                newPath[path.Length] = node;
                path = newPath;
                break;
            }
            //Visited.Add(current);
            if (Bidirectional)
            {
                mutex.ReleaseMutex();
            }
            return path;
        }
    
        protected abstract T[] GetChildren(T node);

        protected abstract T[] OrderChildren(T[] nodes);


    }
}
