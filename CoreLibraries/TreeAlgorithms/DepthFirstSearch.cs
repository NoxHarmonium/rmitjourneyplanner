// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DepthFirstSearch.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Performs a depth first search on a graph.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.TreeAlgorithms
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.Logging;

    #endregion

    /// <summary>
    /// Performs a depth first search on a graph.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public abstract class DepthFirstSearch<T>
    {
        #region Constants and Fields

        /// <summary>
        ///   The depth limit.
        /// </summary>
        protected int DepthLimit = 16;

        /// <summary>
        ///   The provider.
        /// </summary>
        protected INetworkDataProvider Provider;

        /// <summary>
        ///   The visited.
        /// </summary>
        protected HashSet<T>[] Visited = new HashSet<T>[2];

        /// <summary>
        ///   The current.
        /// </summary>
        protected NodeWrapper<T>[] current = new NodeWrapper<T>[2];

        /// <summary>
        ///   The results.
        /// </summary>
        protected T[][] results = new T[2][];

        /// <summary>
        ///   The map.
        /// </summary>
        private readonly Dictionary<NodeWrapper<T>, NodeWrapper<T>> map =
            new Dictionary<NodeWrapper<T>, NodeWrapper<T>>();

        /// <summary>
        ///   The bidirectional.
        /// </summary>
        private bool bidirectional;

        /// <summary>
        ///   The current index.
        /// </summary>
        private int currentIndex;

        /// <summary>
        ///   The destination.
        /// </summary>
        private T destination;

        /// <summary>
        ///   The entropy.
        /// </summary>
        private double entropy = 0.8;

        /// <summary>
        ///   The iterations.
        /// </summary>
        private int iterations;

        /// <summary>
        ///   The origin.
        /// </summary>
        private T origin;

        /// <summary>
        ///   The start time.
        /// </summary>
        private DateTime startTime;

        /// <summary>
        ///   The use visited.
        /// </summary>
        private bool useVisited = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DepthFirstSearch{T}"/> class.
        /// </summary>
        /// <param name="depth">
        /// The depth.
        /// </param>
        /// <param name="bidirectional">
        /// The bidirectional.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="goal">
        /// The goal.
        /// </param>
        protected DepthFirstSearch(int depth, bool bidirectional, T origin, T goal)
            : this(bidirectional, origin, goal)
        {
            this.DepthLimit = depth;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DepthFirstSearch{T}"/> class. 
        ///   Instantiates a new instance of the DepthFirstSerch class.
        /// </summary>
        /// <param name="bidirectional">
        /// </param>
        /// <param name="origin">
        /// The origin node.
        /// </param>
        /// <param name="destination">
        /// The destination node.
        /// </param>
        protected DepthFirstSearch(bool bidirectional, T origin, T destination)
        {
            this.origin = origin;
            this.destination = destination;
            this.bidirectional = bidirectional;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets a value indicating whether Bidirectional.
        /// </summary>
        public bool Bidirectional
        {
            get
            {
                return this.bidirectional;
            }

            set
            {
                this.bidirectional = value;
            }
        }

        /// <summary>
        ///   Gets CurrentIndex.
        /// </summary>
        public int CurrentIndex
        {
            get
            {
                return this.currentIndex;
            }
        }

        /// <summary>
        ///   Gets or sets Destination.
        /// </summary>
        public T Destination
        {
            get
            {
                return this.destination;
            }

            set
            {
                this.destination = value;
            }
        }

        /// <summary>
        ///   Gets or sets Entropy.
        /// </summary>
        public double Entropy
        {
            get
            {
                return this.entropy;
            }

            set
            {
                this.entropy = value;
            }
        }

        /// <summary>
        ///   Gets Iterations.
        /// </summary>
        public int Iterations
        {
            get
            {
                return this.iterations;
            }
        }

        /// <summary>
        ///   Gets or sets Origin.
        /// </summary>
        public T Origin
        {
            get
            {
                return this.origin;
            }

            set
            {
                this.origin = value;
            }
        }

        /// <summary>
        ///   Determines if the algorithm keeps track of visited nodes.
        /// </summary>
        public bool UseVisited
        {
            get
            {
                return this.useVisited;
            }

            set
            {
                this.useVisited = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The run.
        /// </summary>
        /// <returns>
        /// </returns>
        public T[] Run()
        {
            if (this.origin.Equals(this.destination))
            {
                return new[] { this.origin };
            }

            this.map.Clear();
            this.iterations = 0;
            this.startTime = DateTime.Now;

            this.Visited[0] = new HashSet<T>();

            var stack = new Stack<NodeWrapper<T>>[2];
            stack[0] = new Stack<NodeWrapper<T>>();
            T[] goal = new T[2];
            goal[0] = this.destination;
            this.current[0] = new NodeWrapper<T>(this.origin);
            this.current[0].CurrentRoute = 0;

            if (this.bidirectional)
            {
                this.Visited[1] = new HashSet<T>();
                stack[1] = new Stack<NodeWrapper<T>>();
                stack[1].Push(new NodeWrapper<T>(this.destination));
                goal[1] = this.origin;
                this.current[1] = new NodeWrapper<T>(this.destination);
                this.current[1].CurrentRoute = 0;
            }

            stack[0].Push(new NodeWrapper<T>(this.origin));

            this.current[0] = default(NodeWrapper<T>);
            while ((Equals(this.current[0], default(T)) || (stack[0].Any() && !this.current[0].Node.Equals(goal[0])))
                   &&
                   (!this.bidirectional
                    ||
                    (this.bidirectional
                     &&
                     (Equals(this.current[1], default(T))
                      ||
                      stack[1].Any() && !this.current[1].Node.Equals(goal[1])
                      && !this.Visited[0].Intersect(this.Visited[1]).Any()))))
            {
                // while (Equals(current[0], default(T)) || (stack[0].Any() && !current[0].Node.Equals(goal[0])))
                // Update debug info
                this.iterations++;
#if (VERBOSE_DEBUG)

               if (iterations % 10000 == 0)
               {
                   Console.CursorLeft = 0;

                   Console.Write(
                       "[" + 0 + "] - " + iterations + " , "
                       + (iterations / (DateTime.Now - startTime).TotalSeconds).ToString() + " i/s");

               }
#endif
                for (int i = 0; i < (this.bidirectional ? 2 : 1); i++)
                {
                    this.currentIndex = i;

                    // Update goals
                    if (this.bidirectional)
                    {
                        goal[i] = this.current[i == 0 ? 1 : 0].Node;
                    }
                    else
                    {
                        goal[i] = this.destination;
                    }

                    // Update currents
                    do
                    {
                        this.current[i] = stack[i].Pop();
                    }
                    while (this.Visited[i].Contains(this.current[i].Node));

                    if (this.useVisited)
                    {
                        this.Visited[i].Add(this.current[i].Node);
                    }

                    NodeWrapper<T>[] children = this.OrderChildren(this.GetChildren(this.current[i]));

                    foreach (NodeWrapper<T> child in children)
                    {
                        if (!child.Node.Equals(this.current[i].Node))
                        {
                            this.map[child] = this.current[i];
                            stack[i].Push(child);
                        }
                    }
                }
            }

            if (stack[0].Count == 0 || (this.bidirectional && stack[1].Count == 0))
            {
                Logger.Log(this, "Warning: Stack was empty on break. Path may be incomplete.");
            }

            var path = new List<T>();

            NodeWrapper<T> v = this.current[0];

            while (!v.Node.Equals(this.origin))
            {
                path.Add(v.Node);
                v = this.map[v];
            }

            path.Add(v.Node);

            if (this.bidirectional)
            {
                var path2 = new List<T>();

                v = this.current[1];
                while (!v.Node.Equals(this.destination))
                {
                    path2.Add(v.Node);
                    v = this.map[v];
                }

                path2.Add(v.Node);

                var intersection = path.Intersect(path2).ToList();
                int iCount = intersection.Count();
                if (iCount > 0)
                {
                    var iNode = intersection.First();
                    int i1 = path.IndexOf(iNode);
                    int i2 = path2.IndexOf(iNode);
                    path.RemoveRange(0, i1 + 1);
                    path2.RemoveRange(0, i2);
                }

                path.Reverse();
                path.AddRange(path2);
            }

            return path.ToArray();
        }

        #endregion

        /*
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
                threadId = 0;
                results[threadId] = this.RunDFS(this.Origin, 0);

            });
            var threadB = new Thread(delegate()
                {
                    Thread.CurrentThread.Name = "Thread B";
                    threadId = 1;
                    results[threadId] = this.RunDFS(this.Destination, 0);
                });
            threadA.Start();
            threadB.Start();
            threadA.Join();
            threadB.Join();

            if (results[0] == null || results [1] == null)
            {
                return null;
            }
            return results[0].Concatenate(results[1]);
        }

        private void WaitAndFlip()
        {
            if(threadId == runningThread)
            {
                mutex.ReleaseMutex();
            }
            else
            {
                mutex.WaitOne();
            }
            runningThread = (runningThread == 0 ? 1 : 0);

        }
         

        protected T[] RunDFS(T node, int depth)
        {
            if (this.runningThread == threadId)
            {
                mutex.ReleaseMutex();
            }
            if (Bidirectional)
            {
                mutex.WaitOne();
                this.runningThread = threadId;
            }
            if (finished)
            {
               
                mutex.ReleaseMutex();
                return new[] { node };
            }
            
            // this.Visited[threadId].Add(node);
            this.iterations++;
            T[] path = null;
            if (iterations % 10000 == 0)
            {
                
                Console.CursorLeft = 0;

                Console.Write(
                    "[" + threadId + "] - " + iterations + " , "
                    + (iterations / (DateTime.Now - startTime).TotalSeconds).ToString() + " i/s");
                

            }

            if (Bidirectional)
            {

                if (EqualityComparer<T>.Default.Equals(node, current[threadId == 0 ? 1 : 0]))
                {
                    finished = true;
                    runningThread = -1;
                    return new[] { node };
                }

            }
            else
            {
                if (EqualityComparer<T>.Default.Equals(node, Destination))
                {
                    return new[] { node };
                }
            }

            
            current[threadId] = node;
            Console.WriteLine("Setting threadId: {0} to {1}", threadId, node);


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
    */
        #region Methods

        /// <summary>
        /// The get children.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// </returns>
        protected abstract NodeWrapper<T>[] GetChildren(NodeWrapper<T> node);

        /// <summary>
        /// The order children.
        /// </summary>
        /// <param name="nodes">
        /// The nodes.
        /// </param>
        /// <returns>
        /// </returns>
        protected abstract NodeWrapper<T>[] OrderChildren(NodeWrapper<T>[] nodes);

        #endregion
    }
}