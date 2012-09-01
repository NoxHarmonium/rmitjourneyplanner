using System;

namespace RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions
{
    #region

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
	
	using NUnit.Framework;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.Logging;
    using RmitJourneyPlanner.CoreLibraries.Positioning;
    using RmitJourneyPlanner.CoreLibraries.TreeAlgorithms;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion
	public class NewFitnessFunction : IFitnessFunction
	{
		EvolutionaryProperties properties;
		
		
		public NewFitnessFunction (EvolutionaryProperties properties)
		{
			this.properties = properties;
		}

		#region IFitnessFunction implementation
		public Fitness GetFitness (Route route)
		{
			return GetFitness(route,this.properties.DepartureTime);
		}

		public Fitness GetFitness (Route route, DateTime initialDeparture)
		{
			var fitness = new Fitness();

            INetworkDataProvider provider = properties.NetworkDataProviders[0];
            
            var openRoutes = new List<int>();
            var closedRoutes = new List<int>();
            var closedRoutesIndex = new List<List<ClosedRoute>>();
            var routeIndex = new Dictionary<int, int>();


            for (int i = 0; i <= route.Count; i++)
            {
                List<int> routes = new List<int>();
				if (i < route.Count)
				{
					route[i].Node.RetrieveData();
	                closedRoutesIndex.Add(new List<ClosedRoute>());
	                routes = provider.GetRoutesForNode(route[i].Node);
	                
	                foreach (int routeId in routes)
	                {
	                    if (!openRoutes.Contains(routeId))// && !closedRoutes.Contains(routeId))
	                    {
	                        openRoutes.Add(routeId);
	                        routeIndex[routeId] = i;
	
	                    }
	                }
				}
                var newOpenRoute = new List<int>();
                foreach (var openRoute in openRoutes)
                {
                    if (!routes.Contains(openRoute))
                    {
						//Close routes                        
	                    closedRoutes.Add(openRoute);
	                    ClosedRoute cr = new ClosedRoute { end = i - 1, id = openRoute, start = routeIndex[openRoute] };
	                    closedRoutesIndex[routeIndex[openRoute]].Add(cr);
                    }
                    else
                    {
                        
                        newOpenRoute.Add(openRoute);
                        
                    }
                }

                openRoutes = newOpenRoute;
            }
			
			Assert.IsFalse(openRoutes.Any(),"There should be no open routes here.");
			
			int pointer = 0;
			
			DateTime departureTime = properties.DepartureTime;
			
			while (pointer < route.Count-1)
			{
				var candidateRoutes = closedRoutesIndex[pointer];
				
				foreach (var candidateRoute in candidateRoutes)
				{
					var arcs = provider.GetDistanceBetweenNodes (route[pointer].Node,route[candidateRoute.end].Node,departureTime);
					
				}
				
				
			}
			
			
			
			
			throw new NotImplementedException ();
		}
		#endregion
	}
}

