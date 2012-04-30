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

    using RmitJourneyPlanner.CoreLibraries.DataProviders;

    /// <summary>
    /// Performs a depth first search on a graph.
    /// </summary>
    public abstract class DepthFirstSearch<T>
    {
        protected HashSet<T> Visited;
        protected INetworkDataProvider Provider;
        protected T Origin;
        
        protected int DepthLimit = 16;
        protected T Goal;

        protected bool Bidirectional;

        protected T current;

        private int iterations;

        private DateTime startTime;
        

        protected  DepthFirstSearch(int depth, bool bidirectional, T origin, T goal) :this(bidirectional,origin,goal)
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
        protected DepthFirstSearch(bool bidirectional,T origin, T goal)
        {
            this.Visited = new HashSet<T>();
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
            return this.RunDFS(Origin,0, new HashSet<T>());
        }

        protected T[] RunDFS(T node,int depth,HashSet<T> visited )
        {
            this.iterations++;
            if (iterations %1000000 == 0)
            {
                Console.CursorLeft = 0;
                Console.Write(iterations + " , " + (iterations / (DateTime.Now - startTime).TotalSeconds).ToString() + " i/s");
               

            }
            current = node;
            if (EqualityComparer<T>.Default.Equals(node,Goal))
            {
                return new T[] {node};
            }
            if (depth > this.DepthLimit)
            {
                return null;
            }
            T[] children = this.OrderChildren(GetChildren(node));
            var paths = new T[children.Length][];
            foreach (T child in children)
            {
                if (!visited.Contains(child))
                {
                    var newVisited = new HashSet<T>(visited) { current };
                    T[] path = this.RunDFS(child,depth + 1,newVisited);

                    if (path != null)
                    {
                        T[] newPath = new T[path.Length+1];
                        Array.Copy(path,newPath,path.Length);
                        newPath[path.Length] = node;
                        return newPath;
                    }
                    
                }
            }

            return null;
        }

        protected abstract T[] GetChildren(T node);

        protected abstract T[] OrderChildren(T[] nodes);


    }
}
