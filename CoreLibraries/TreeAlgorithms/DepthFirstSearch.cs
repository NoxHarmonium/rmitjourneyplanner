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
    /// The object type to search with that must implement <see cref="IEquatable{T}"/>.
    /// </typeparam>
    public abstract class DepthFirstSearch<T>
        where T : IEquatable<T>
    {
        #region Constants and Fields

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
        ///   The current.
        /// </summary>
        private NodeWrapper<T>[] current = new NodeWrapper<T>[2];

        /// <summary>
        ///   The current index.
        /// </summary>
        private int currentIndex;

        /// <summary>
        ///   The depth limit.
        /// </summary>
        private int depthLimit = 16;

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
        ///   The results.
        /// </summary>
        private T[][] results = new T[2][];

        /// <summary>
        ///   The start time.
        /// </summary>
        private DateTime startTime;

        /// <summary>
        ///   The use visited.
        /// </summary>
        private bool useVisited = true;

        /// <summary>
        ///   The visited.
        /// </summary>
        private HashSet<T>[] visited = new HashSet<T>[2];

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
        /// </summary>
        /// <param name="bidirectional">
        /// Determines whether the search is bidirectional or not.
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
        ///   Gets or sets a value indicating whether the algorithm keeps track of visited nodes.
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

        #region Properties

        /// <summary>
        ///   Gets or sets the current node/s in the search.
        /// </summary>
        protected NodeWrapper<T>[] Current
        {
            get
            {
                return this.current;
            }

            set
            {
                this.current = value;
            }
        }

        /// <summary>
        ///   Gets or sets the depth limit of the search.
        /// </summary>
        protected int DepthLimit
        {
            get
            {
                return this.depthLimit;
            }

            set
            {
                this.depthLimit = value;
            }
        }

        ////TODO: Check if INetworkDataProvider reference should be in this generic class.

        /// <summary>
        ///   Gets or sets the <see cref = "INetworkDataProvider" /> associated with the search.
        /// </summary>
        protected INetworkDataProvider Provider { get; set; }

        /// <summary>
        ///   Gets or sets the results of the search.
        /// </summary>
        protected T[][] Results
        {
            get
            {
                return this.results;
            }

            set
            {
                this.results = value;
            }
        }

        /// <summary>
        ///   Gets or sets the visited set of the search.
        /// </summary>
        protected HashSet<T>[] Visited
        {
            get
            {
                return this.visited;
            }

            set
            {
                this.visited = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Runs the search algorithm.
        /// </summary>
        /// <returns>
        /// An array of objects in order of the final search path.
        /// </returns>
        public T[] Run()
        {
            if (this.origin.Equals(this.destination))
            {
                return new[] { this.origin };
            }
restart:
            this.map.Clear();
            this.iterations = 0;
            this.startTime = DateTime.Now;

            this.Visited[0] = new HashSet<T>();

            var stack = new Stack<NodeWrapper<T>>[2];
            stack[0] = new Stack<NodeWrapper<T>>();
            T[] goal = new T[2];
            goal[0] = this.destination;
            this.Current[0] = new NodeWrapper<T>(this.origin);

            if (this.bidirectional)
            {
                this.Visited[1] = new HashSet<T>();
                stack[1] = new Stack<NodeWrapper<T>>();
                stack[1].Push(new NodeWrapper<T>(this.destination));
                goal[1] = this.origin;
                this.Current[1] = new NodeWrapper<T>(this.destination);
            }

            stack[0].Push(new NodeWrapper<T>(this.origin));

            this.Current[0] = default(NodeWrapper<T>);
            while (!this.IsFinished(stack, goal))
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
                        goal[i] = this.Current[i == 0 ? 1 : 0].Node;
                    }
                    else
                    {
                        goal[i] = this.destination;
                    }

                    // Update currents
                    do
                    {
                        if (stack[i].Count == 0)
                        {
                            Logger.Log(this, "Warning: depth first search stack empty, returning direct path from origin->destination");
                            // TODO: Work out why this happens sometimes
                            return new T[2] { this.origin, this.destination };
                        }
                        
                        this.Current[i] = stack[i].Pop();
                    }
                    while (this.Visited[i].Contains(this.Current[i].Node));

                    if (this.useVisited)
                    {
                        this.Visited[i].Add(this.Current[i].Node);
                    }

                    NodeWrapper<T>[] children = this.OrderChildren(this.GetChildren(this.Current[i]));

                    foreach (NodeWrapper<T> child in children)
                    {
                        if (!child.Node.Equals(this.Current[i].Node))
                        {
                            this.map[child] = this.Current[i];
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

            NodeWrapper<T> v = this.Current[0];

            while (!v.Node.Equals(this.origin))
            {
                path.Add(v.Node);
                v = this.map[v];
            }

            path.Add(v.Node);

            if (this.bidirectional)
            {
                var path2 = new List<T>();

                v = this.Current[1];
                while (!v.Node.Equals(this.destination))
                {
                    path2.Add(v.Node);
                    v = this.map[v];
                }

                path2.Add(v.Node);

                var intersection = path.Intersect(path2).ToList();
                int intersectionCount = intersection.Count();
                if (intersectionCount > 0)
                {
                    var intersectionNode = intersection.First();
                    int i1 = path.IndexOf(intersectionNode);
                    int i2 = path2.IndexOf(intersectionNode);
                    path.RemoveRange(0, i1 + 1);
                    path2.RemoveRange(0, i2);
                }

                path.Reverse();
                path.AddRange(path2);
            }

            if (path.Count == 0)
            {
               Logger.Log(this, "Warning:  Path is only one node long which will break optimiser,  returning direct path from origin->destination");
               // TODO: Work out why this happens sometimes
               return new T[2] { this.origin, this.destination };
            }

            return path.ToArray();
        }

        #endregion

        //// TODO: Make this method better for overriding (make stack a field or something?)/
        #region Methods

        /// <summary>
        /// Returns the children of a specified node. Override this function
        ///   to allow the children of a node to be determined.
        /// </summary>
        /// <param name="node">
        /// The node to find the children nodes of.
        /// </param>
        /// <returns>
        /// The child nodes of the specified node.
        /// </returns>
        protected abstract NodeWrapper<T>[] GetChildren(NodeWrapper<T> node);

        /// <summary>
        /// Returns if the search is finished or not.
        /// </summary>
        /// <param name="stack">
        /// The search stack.
        /// </param>
        /// <param name="goal">
        /// The search goal array.
        /// </param>
        /// <returns>
        /// True if the search is finished, otherwise false.
        /// </returns>
        protected bool IsFinished(Stack<NodeWrapper<T>>[] stack, T[] goal)
        {
            for (int i = 0; i < (this.bidirectional ? 2 : 1); i++)
            {
                if (this.current[i] == null)
                {
                    return false;
                }
                
                if (!stack[i].Any())
                {
                    return true;
                }

                if (this.Current[i].Node.Equals(goal[i]))
                {
                    return true;
                }
            }

            if (this.bidirectional)
            {
                if (this.Visited[0].Intersect(this.Visited[1]).Any())
                {
                    return true;
                }
            }

            /*
            return
                !((this.Current[0].Node.Equals(default(T)) || (stack[0].Any() && !this.Current[0].Node.Equals(goal[0])))
                  &&
                  (!this.bidirectional
                   ||
                   (this.bidirectional && (this.Current[1].Node.Equals(default(T)))
                     ||
                     (stack[1].Any() && !this.Current[1].Node.Equals(goal[1]))
                     && !this.Visited[0].Intersect(this.Visited[1]).Any())));
             */
            return false;
        }

        /// <summary>
        /// Returns a set of nodes in a certain order. Override this function 
        ///   to set the order of child nodes.
        /// </summary>
        /// <param name="nodes">
        /// Nodes to be sorted.
        /// </param>
        /// <returns>
        /// The specified nodes in a specific order.
        /// </returns>
        protected abstract NodeWrapper<T>[] OrderChildren(NodeWrapper<T>[] nodes);

        #endregion
    }
}