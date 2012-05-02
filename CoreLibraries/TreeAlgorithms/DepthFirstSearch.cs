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
    using RmitJourneyPlanner.CoreLibraries.Types;

    /// <summary>
    /// Performs a depth first search on a graph.
    /// </summary>
    public abstract class DepthFirstSearch<T> 
    {
        protected HashSet<T>[] Visited = new HashSet<T>[2];
        protected INetworkDataProvider Provider;
        protected T Origin;
        protected T Goal;
        protected int DepthLimit = 16;
        
      
        protected bool Bidirectional;



        protected NodeWrapper<T>[] current = new NodeWrapper<T>[2];

     
        protected T[][] results = new T[2][];

        private readonly Dictionary<NodeWrapper<T>, NodeWrapper<T>> map = new Dictionary<NodeWrapper<T>, NodeWrapper<T>>();


    
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

       public T[] Run()
       {
           startTime = DateTime.Now;
          
           var stack = new Stack<NodeWrapper<T>>();
           stack.Push(new NodeWrapper<T>(this.Origin));
           current[0] = default(NodeWrapper<T>);
           while (object.Equals(current[0],default(T)) || stack.Any() && !current[0].Node.Equals(Goal))
           {
               iterations++;
               if (iterations % 10000 == 0)
               {

                   Console.CursorLeft = 0;

                   Console.Write(
                       "[" + 0 + "] - " + iterations + " , "
                       + (iterations / (DateTime.Now - startTime).TotalSeconds).ToString() + " i/s");


               }
               current[0] = stack.Pop();
               if (!Visited[0].Contains(current[0].Node))
               {
                   Visited[0].Add(current[0].Node);
                   T[] children = this.OrderChildren(GetChildren(current[0].Node));


                   foreach (T child in children){

                       if (!child.Equals(current[0].Node))
                       {
                           var wrapper = new NodeWrapper<T>(child);
                           map[wrapper] = current[0];
                           stack.Push(wrapper);
                       }
                   }

               }

           }

           var path = new List<T>();
           NodeWrapper<T> v = current[0];
           while (!v.Node.Equals(Origin))
           {
               path.Add(v.Node);
               v = map[v];
           }
           path.Add(v.Node);
           return path.ToArray();
       }

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
                    results[threadId] = this.RunDFS(this.Goal, 0);
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
                if (EqualityComparer<T>.Default.Equals(node, Goal))
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
        protected abstract T[] GetChildren(T node);

        protected abstract T[] OrderChildren(T[] nodes);


    }
}
