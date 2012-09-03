
namespace JayrockClient
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Web;
    using System.Web.SessionState;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Jayrock.Json;
    using Jayrock.JsonRpc;
    using Jayrock.JsonRpc.Web;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
	using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Google;
    using System.Linq;
    using RmitJourneyPlanner.CoreLibraries.Types;

    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;

    #endregion

    [JsonRpcHelp("This is a JSON-RPC service that exposes functionality related to the RMIT Journey Planner.")]
    public class Jsonws : JsonRpcHandler, IRequiresSessionState
    {
        private static bool LogEventCreated;
		
		
		
		public Jsonws ()
		{
			if (!LogEventCreated)
			{
				RmitJourneyPlanner.CoreLibraries.Logging.Logger.LogEvent += 
				(sender, message) => {Console.WriteLine("[{0}]: {1}",sender,message);};
				LogEventCreated = true;
			}
			
		}
		
		[JsonRpcMethod("NewJourney", Idempotent = true)]
		[JsonRpcHelp("Creates a new default journey with the supplied name and description and returns it's UUID.")]
       	public string NewJourney(string shortName, string description)
		{
			Journey newJourney = new Journey();
			newJourney.ShortName = shortName;
			newJourney.Description = description;
			newJourney.Properties = new EvolutionaryProperties();
			var jm = ObjectCache.GetObject<JourneyManager>();
			jm.Add(newJourney);
			return newJourney.Uuid;
		}
		
		[JsonRpcMethod("DeleteJourney", Idempotent = true)]
        [JsonRpcHelp("Deletes the specified journey.")]
		public void DeleteJourney(string uuid)
		{
			var jm = ObjectCache.GetObject<JourneyManager>();
			jm.DeleteJourney(uuid);
		}
		
		[JsonRpcMethod("SetJourneyName", Idempotent = true)]
        [JsonRpcHelp("Sets the short name and description of the journey.")]
		public void SetJourneyName(string uuid, string shortName, string description)
		{
			var jm = ObjectCache.GetObject<JourneyManager>();
			var j = jm.GetJourney(uuid);
			j.ShortName = shortName;
			j.Description = description;	
			
		}
				
		
		[JsonRpcMethod("GetJourneys", Idempotent = true)]
        [JsonRpcHelp("Gets the list of journey names and UUIDs.")]
		public Object[] GetJourneys()
		{
			var jm = ObjectCache.GetObject<JourneyManager>();
			return jm.GetJourneys().Select(s=> (object) new {uuid = s.Uuid , shortName = s.ShortName, description = s.Description}).ToArray();
		}
        
        
        [JsonRpcMethod("LoadProviders", Idempotent = true)]
        [JsonRpcHelp("Initialises the data providers.")]
        public void LoadProviders()
        {
            ObjectCache.DeregisterObjects(typeof(IPointDataProvider));
            //this.DeregisterObjects(typeof(INetworkDataProvider));
            IPointDataProvider provider = new WalkingDataProvider();
            ObjectCache.RegisterObject(provider);

           if (!ObjectCache.GetObjects<MetlinkDataProvider>().Any())
           {
               ObjectCache.RegisterObject(new MetlinkDataProvider());
           }
           if (!ObjectCache.GetObjects<EvolutionaryProperties>().Any())
           {
               ObjectCache.RegisterObject(new EvolutionaryProperties());
           }
           if (!ObjectCache.GetObjects<JourneyManager>().Any())
           {
               ObjectCache.RegisterObject(new JourneyManager());
           }
           if (!ObjectCache.GetObjects<JourneyOptimiser>().Any())
           {
               ObjectCache.RegisterObject(new JourneyOptimiser());
           }
        }

        [JsonRpcMethod("GetProperties", Idempotent = true)]
        [JsonRpcHelp("Gets all the properties accessable for the journey planner run.")]
        public ObjectProperty[] GetProperties(string journeyUuid)
        {
            PropertyInfo[] propertyInfos = typeof(EvolutionaryProperties).GetProperties().OrderBy(p => p.PropertyType.Name).ToArray();
            var objectProperties = new ObjectProperty[propertyInfos.Length];
            //EvolutionaryProperties properties = ObjectCache.GetObjects<EvolutionaryProperties>().First();
            var jp = ObjectCache.GetObject<JourneyManager>();
            EvolutionaryProperties properties;
            try
            {
                properties = journeyUuid == null ? new EvolutionaryProperties() : jp.GetJourney(journeyUuid).Properties;
            }
            catch (Exception)
            {
                throw new JsonException("The supplied journey Uuid was incorrect.");
            }

			string delimeter = ", ";
            for (int i = 0; i < propertyInfos.Length; i++) 
            {
                bool editable = true;
				object value = null;
                bool isArray = false;
             

				Type pType = propertyInfos[i].PropertyType;
//TODO: Really fix this!
            restart:
				if (!pType.IsValueType)
				{
					if (pType.IsGenericType)
					{
						if (pType.Name.Contains("List"))
						{
							pType = pType.GetGenericArguments()[0];
						}
					}	
					
					if (pType.IsArray)
					{
						pType = pType.GetElementType();
					    isArray = true;
                        goto restart;
					    
					}
					
					if (pType.Name == "INetworkNode")
					{
						
						value = "";
						MetlinkNode node = (MetlinkNode) propertyInfos[i].GetValue(properties, null) ?? null;
						if (node != null)
						{
							value = node.StopSpecName + delimeter + node.Id;
						}
						
					}
					else
					{
						editable = false;
						var types = Assembly.GetAssembly(pType).GetTypes();
						var candidates = types.Where(t => (t == pType && !t.IsInterface) ||
						                             	t.GetInterfaces().Contains(pType));
						var val = propertyInfos[i].GetValue(properties, null);
					    string valstring = " ";
                        if (val != null)
                        {
                            var type = val.GetType();
                            if (type.IsArray)
                            {
                                valstring = type.GetElementType().Name;

                            }
                            else
                            {
                                valstring = type.Name;
                            }

                        }
					   


						//string valstring = (val != null ? val.GetType().Name : " ");
						value = String.Join(delimeter,candidates.Select(c => c.Name)) + "@" + valstring;
					}
					
					
				}
				else
				{
					if (pType.IsEnum)
					{
						var val = propertyInfos[i].GetValue(properties, null);
					    string valstring;
					    string seperator = "@";

                        if (isArray)
                        {
                            //TODO: Make this generic
                            valstring = String.Join(",", ((RmitJourneyPlanner.CoreLibraries.Types.FitnessParameters[])val));
                            seperator = "|";
                        }
                        else
                        {
                            valstring = (val != null ? val.ToString() : " ");
                            
                        }

                        value = string.Join(delimeter, Enum.GetNames(pType)) + seperator + valstring;
                        editable = false;
						
                       
					}
					else
					{
						value = propertyInfos[i].GetValue(properties, null) ?? "null";				
					}
				}
				
				
                objectProperties[i] = new ObjectProperty
                    {
						Name = propertyInfos[i].Name,
						Type = propertyInfos[i].PropertyType.Name, 
						Value = value.ToString(), 
						Editable = editable
					};
            }

            return objectProperties;
        }
		
		[JsonRpcMethod("SetProperties", Idempotent = true)]
		[JsonRpcHelp("Accepts an array of property names and values and attempts to modify the property object. Returns an array of validation errors.")]
		public ValidationError[] SetProperties(string journeyUuid, PropertyValue[] propVals)
		{
			/*
			 FORMAT:
			 { "propVals" : [{ "propVal" : {"name":"CrossoverRate","value":"0.7"} }] }
			 */
			
			
			List<ValidationError> valErrors = new List<ValidationError>();
			foreach (var propVal in propVals)
			{
				var valError = SetProperty(journeyUuid,propVal);
				if (valError != null)
				{
					valErrors.Add(valError);	
				}
			}
			return valErrors.ToArray();
		}
		
		[JsonRpcMethod("SetProperty", Idempotent = true)]
		[JsonRpcHelp("Accepts a property name and value and attempts to modify the property object. Returns a validation error.")]
		public ValidationError SetProperty(string journeyUuid, PropertyValue propVal)
    	{
    		return SetProperty(journeyUuid, propVal,false);
			
    	}
		
		private ValidationError SetProperty(string journeyUuid, PropertyValue propVal, bool testOnly)
		{
			if (propVal.Name == null)
			{
				throw new Exception("Property value name is null. Cannot proceed with validation. Please check your JSON syntax.");
				
			}
            if (journeyUuid == null  && !testOnly)
            {
                throw new JsonException("The journey UUID can only be null if the testOnly attribute is true.");

            }


		    //var valErrors = new List<ValidationError>();
		    var jp = ObjectCache.GetObject<JourneyManager>();
           
    		PropertyInfo[] propertyInfos = typeof(EvolutionaryProperties).GetProperties();
		    EvolutionaryProperties properties;
		    try
		    {
                
                properties = journeyUuid == null ? new EvolutionaryProperties() : jp.GetJourney(journeyUuid).Properties;
		    }
		    catch (Exception)
		    {
		        throw new JsonException("The supplied journey Uuid was incorrect.");
		    }
		    
    		
    		//foreach (var propVal in propVals)
    		{
    			var propertyInfo =  propertyInfos.FirstOrDefault(s => s.Name == propVal.Name);
    			
    			if (propertyInfo == null)
    			{
    				return new ValidationError{Target = propVal.Name, Message = Strings.ERR_PROP_NOT_FOUND};
    				//continue;
    			}
    			
				
				
				Object castVal = null;
				String errMess = String.Empty;
				bool isList = false;
				bool isArray = false;
				
				if (propertyInfo.PropertyType.IsValueType)
				{				
					try
					{
						if (propertyInfo.PropertyType.IsEnum)
						{
							castVal = Enum.Parse(propertyInfo.PropertyType,propVal.Value);
						}
						else
						{
							castVal = Convert.ChangeType(propVal.Value,propertyInfo.PropertyType);
						}
					}
					catch (Exception e)
					{
						errMess = e.Message;					
					}
				}
				else
				{
					try
					{
						Type pType = propertyInfo.PropertyType;
						
						switch (pType.Name)
						{
						    case "INetworkNode":
								MetlinkDataProvider provider = ObjectCache.GetObjects<MetlinkDataProvider>()[0];
								MetlinkNode node = (MetlinkNode)provider.GetNodeFromId(Convert.ToInt32(propVal.Value));//provider.GetNodeFromName(propVal.Value);
								if (node == null)
								{
									throw new Exception("No matching node for supplied name.");	
								}
								castVal = node;
							break;
							
                            case "FitnessParameters[]":

						        var objs = propVal.Value.Split(new[]
						        {
						            ','
						        });
                                var objectives = new FitnessParameters[objs.Length];
						        int i = 0;
                                foreach (var obj in objs)
                                {
                                    FitnessParameters e;
                                    if (!Enum.TryParse(obj, out e))
                                    {
                                        throw new Exception("Cannot parse string into enum.");
                                    }
                                    objectives[i++] = e;
                                }


						        castVal = objectives;
                                

						        break;
						    default:
							    //var pType = new;
							
							    if (pType.IsGenericType)
							    {
								    if (pType.Name.Contains("List"))
								    {
									    pType = pType.GetGenericArguments()[0];
									    isList = true;
								    }
							    }	
							    if (pType.IsArray)
							    {
								    pType = pType.GetElementType();	
								    isArray = true;
								
								
							    }
							
							    try{
								    var types = Assembly.GetAssembly (pType).GetTypes();
								    pType = types.Where(t=> t.Name == propVal.Value).First();
							    }
							    catch(Exception e)
							    {
									    throw new Exception("Cannot find type in assembly. (" + propVal.Value + ")");
							    }
							                                                                        
							    //var pType = Assembly.GetAssembly (propertyInfo.PropertyType).GetType(propVal.Value,true,true);
							
							
							    var objects = ObjectCache.GetObjects(pType);
							    if (objects.Length == 0)
							    {
								    object newObj = null;
								    try {
									    //newObj = pType.InvokeMember("",BindingFlags.CreateInstance,null,null,new[] {properties});
									    newObj = Activator.CreateInstance(pType,new[] {properties});
								    }
								    catch (Exception e)
								    {
									    //newObj = pType.InvokeMember("",BindingFlags.CreateInstance,null,null,null);
									    newObj = Activator.CreateInstance(pType);
								    }
								    ObjectCache.RegisterObject(newObj);
								    castVal = newObj;
							    }
							    else
							    {
								    castVal = objects.First();	
							    }
							
							    if (isArray)
							    {
								    var array =  Array.CreateInstance(pType,1);
								    array.SetValue(castVal,0);
								    castVal = array;
								
								
							    }
							
							break;
							//throw new Exception("No handler for type: " + pType.Name);
							
						}
					}
					catch (Exception e)
					{
						return new ValidationError{Target = propVal.Name, 
								Message = String.Format("{0}: {1}",Strings.ERR_UNSUPP_REFTYPE,e.Message)};
					}
				}
				
				
				if (castVal == null)
				{
					//ERR_INVALID_CAST
					return new ValidationError{Target = propVal.Name, 
						Message = String.Format("{0}: {1}",Strings.ERR_INVALID_CAST,errMess)};
    				//continue;
				}
					
				if (!testOnly)
				{
    				if (isList)
					{
						System.Collections.IList a = (System.Collections.IList) propertyInfo.GetValue(properties,null);
									
						a.Add(castVal);
						
					}					
					else
					{
						propertyInfo.SetValue(properties, castVal , null);
					}
				}
    		
    		}
    		
    		return new ValidationError{Target = propVal.Name, 
						Message = Strings.VALIDATION_SUCCESS};	
			
		}
		
		[JsonRpcMethod("ValidateField", Idempotent = true)]
		public ValidationError ValidateField(PropertyValue propVal)
		{
			return SetProperty(null,propVal,true);
			
		}
       
		[JsonRpcMethod("QueryStops", Idempotent = true)]
		[JsonRpcHelp("Returns an array of stop names from the query.")]
		public object[] QueryStops(string query)
		{
			var provider = ObjectCache.GetObjects<MetlinkDataProvider>().First();
			var stops = provider.QueryStops(query);
			//var stopStrings = stops.Select(s => s.StopSpecName);
			//return stopStrings.ToArray();
			return stops.ToArray();
			
			
			
			
		}




        [JsonRpcMethod("GetOptimisationState", Idempotent = true)]
        [JsonRpcHelp("Returns the state and queue of the optimisation mananger.")]
        public object GetOptimisationState()
        {
            var jo = ObjectCache.GetObject<JourneyOptimiser>();
            return new { state = jo.State, queue = jo.GetQueue() };
        }


        [JsonRpcMethod("EnqueueJourney", Idempotent = false)]
        [JsonRpcHelp("Enqueues the journey specified by its UUID in the optimisation queue the number of times specified by runs.")]
        public void EnqueueJourney(string uuid, int runs)
        {
            if (runs == default(int))
            {
                runs = 1;
            }
            var jo = ObjectCache.GetObject<JourneyOptimiser>();
            //var jm = ObjectCache.GetObject<JourneyManager>();
            jo.EnqueueJourney(uuid,runs);

        }


        
    }
}
