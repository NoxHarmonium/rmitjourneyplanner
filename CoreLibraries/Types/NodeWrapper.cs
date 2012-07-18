using System;
using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
using RmitJourneyPlanner.CoreLibraries.DataProviders;

namespace RmitJourneyPlanner.CoreLibraries
{
	public class NodeWrapper : ICloneable
	{
		private INetworkNode node;
		
		public INetworkNode Node
		{
			get
			{
				return node;
			}
		}
		
		/// <summary>
        ///   Gets or sets the current route that the node is traversing.
        /// </summary>
        public int CurrentRoute { get; set; }

        /// <summary>
        ///   Gets or sets the Euclidian distance to the goal. Used for traversing route trees.
        /// </summary>
        public double EuclidianDistance { get; set; }
		
		/// <summary>
        ///   Gets or sets the total time taken to reach this node. Used for traversing route trees.
        /// </summary>
        public TimeSpan TotalTime { get; set; }

       	
		
		public NodeWrapper (INetworkNode node)
		{
			this.node = node;
		}
		public object Clone ()
		{
			return new NodeWrapper(this.Node);
		}
		
	}
}

