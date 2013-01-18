// --------------------------------------------------------------------------------------------------------------------
// <copyright file="jsonws.cs" company="">
//   
// </copyright>
// <summary>
//   The jsonws.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    #region Using Directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.SessionState;

    using JRPCServer.cs;

    using Jayrock.Json;
    using Jayrock.JsonRpc;
    using Jayrock.JsonRpc.Web;

    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Google;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Ptv;
    using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.Logging;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// The jsonws.
    /// </summary>
    [JsonRpcHelp("This is a JSON-RPC service that exposes functionality related to the RMIT Journey Planner.")]
    public class SimpleJsonWs : JsonRpcHandler, IRequiresSessionState
    {
        #region Constants and Fields

        /// <summary>
        /// The log event created.
        /// </summary>
        private static bool LogEventCreated;

        private const int MaxObjectNameLength = 20;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleJsonWs"/> class.
        /// </summary>
        public SimpleJsonWs()
        {
            if (!LogEventCreated)
            {
                // RmitJourneyPlanner.CoreLibraries.Logging.Logger.LogEvent += 
                // (sender, message) => {Console.WriteLine("[{0}]: {1}",sender,message);};
                LogEventCreated = true;
            }
            this.LoadProviders();
        }

        #endregion

        #region Public Methods and Operators

        [JsonRpcMethod("LoadProviders", Idempotent = true)]
        [JsonRpcHelp("Initialises the data providers.")]
        public void LoadProviders()
        {
            ObjectCache.DeregisterObjects(typeof(IPointDataProvider));

            // this.DeregisterObjects(typeof(INetworkDataProvider));
            IPointDataProvider provider = new WalkingDataProvider();
            ObjectCache.RegisterObject(provider);

            if (!ObjectCache.GetObjects<State>().Any())
            {
                var state = new State();
                Logger.LogEvent += new LogEventHandler(Logger_LogEvent);
                state.LogEventHooked = true;

                ObjectCache.RegisterObject(state);


            }

            if (!ObjectCache.GetObjects<PtvDataProvider>().Any())
            {
                ObjectCache.RegisterObject(new PtvDataProvider());
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

        /// <summary>
        /// The get node data.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The get node data.
        /// </returns>
        [JsonRpcMethod("GetNodeData", Idempotent = true)]
        [JsonRpcHelp("Returns basic information pertaining to the specified stop ID.")]
        public object GetNodeData(int id)
        {
            var provider = ObjectCache.GetObject<PtvDataProvider>();
            var data = provider.GetNodeData(id);
            string mode = data.Rows[0]["StopModeName"].ToString();
            if (mode != "Bus" && mode != "Tram" && mode != "Train")
            {
                mode = "Unknown";
            }

            return
                new
                    {
                        id, 
                        name = data.Rows[0]["StopSpecName"].ToString(), 
                        mode, 
                        travelSt = data.Rows[0]["TravelStName"] + " " + data.Rows[0]["TravelStType"], 
                        crossSt = data.Rows[0]["CrossStName"] + " " + data.Rows[0]["CrossStType"], 
                        latitude = data.Rows[0]["GPSLat"].ToString(), 
                        longitude = data.Rows[0]["GPSLong"].ToString(), 
                    };
        }

        
       
        /// <summary>
        /// The query stops.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// </returns>
        [JsonRpcMethod("QueryStops", Idempotent = true)]
        [JsonRpcHelp("Returns an array of stop names from the query.")]
        public object[] QueryStops(string query)
        {
            var provider = ObjectCache.GetObjects<PtvDataProvider>().FirstOrDefault();
            var stops = provider.QueryStops(query);

            // var stopStrings = stops.Select(s => s.StopSpecName);
            // return stopStrings.ToArray();
            return stops.ToArray();
        }


        [JsonRpcMethod("GetStatus", Idempotent = true)]
        [JsonRpcHelp(
            "Accepts a user key and returns the optimisation status related to it."
            )]
        public object GetStatus(string userKey)
        {
            var optimiser = ObjectCache.GetObject<JourneyOptimiser>();
            if (optimiser.CurrentJourney != null)
            {

                if (optimiser.CurrentJourney.Uuid == userKey)
                {
                    double progress = optimiser.CurrentIteration / (double)optimiser.MaxIterations;
                    if (double.IsNaN(progress) || double.IsInfinity(progress))
                    {
                        progress = 0.0;
                    }
                    return new { status = "optimising", progress, iteration = optimiser.CurrentIteration, totalIterations = optimiser.MaxIterations };
                }

                if (optimiser.GetQueue().Contains(userKey))
                {
                    return new { status = "queued", progress = 0.0, iteration = 0, totalIterations = 0 };
                }
            }
            //Default message TODO: Maybe the default status shouldn't be 'finished'.
            return new { status = "finished", progress = 1.0, iteration = 0, totalIterations = 0 };
            
        }


        public object GetJourneys(string userKey)
        {
            dynamic status = this.GetStatus(userKey);
            // Check if optimisation finished.
            if (status.status == "finished")
            {
                

            }
            return null;

        }


        [JsonRpcMethod("Search", Idempotent = true)]
        [JsonRpcHelp(
            "Accepts a user key and an array of property names and values and performs a search. Returns an array of validation errors."
            )]
        public ValidationError[] Search(string userKey, PropertyValue[] propVals)
        {

            var journeyManager = ObjectCache.GetObject<JourneyManager>();
            if (journeyManager == null)
            {
                journeyManager = new JourneyManager();
                ObjectCache.RegisterObject(journeyManager);
            }
            
            

            bool validationError = false;

            //Load the default journey
            Journey journey;
            try
            {
                journey = (Journey)journeyManager.GetJourney("default").Clone();
                journey.Uuid = userKey;
                if (journeyManager.Contains(journey))
                {
                    journeyManager.DeleteJourney(userKey);
                }
                journeyManager.Add(journey);
                journeyManager.Save();
            }
            catch (KeyNotFoundException e)
            {
                throw new Exception(
                   "No default journey to base this journey off. Please specify one using the control panel.");
                
            }

            journeyManager.Clean(journey);
            journeyManager.Save();

            
            /*
			 FORMAT:
			 { "propVals" : [{ "propVal" : {"name":"CrossoverRate","value":"0.7"} }] }
			 */

            if (propVals == null)
            {
                throw new Exception(
                    "Property value collection is null. Cannot proceed with validation. Please check your JSON syntax.");

            }

           

            List<ValidationError> valErrors = new List<ValidationError>();
            foreach (var propVal in propVals)
            {
                var valError = this.SetProperty(journey, propVal);
                if (valError != null)
                {
                    valErrors.Add(valError);
                    if (valError.Message != Strings.VALIDATION_SUCCESS)
                    {
                        validationError = true;
                    }
                }
            }

            // Only perform search if there are no validation errors.
            if (!validationError)
            {
                var optimiser = ObjectCache.GetObject<JourneyOptimiser>();
                optimiser.Remove(journey);
                optimiser.EnqueueJourney(journey,1);
                optimiser.Resume();

            }
           
            return valErrors.ToArray();
        }

        /// <summary>
        /// The set property.
        /// </summary>
        /// <param name="journey">
        /// The journey.
        /// </param>
        /// <param name="propVal">
        /// The prop val.
        /// </param>
        /// <returns>
        /// </returns>
        [JsonRpcMethod("SetProperty", Idempotent = true)]
        [JsonRpcHelp(
            "Accepts a property name and value and attempts to modify the property object. Returns a validation error.")
        ]
        public ValidationError SetProperty(Journey journey, PropertyValue propVal)
        {
            return this.SetProperty(journey, propVal, false);
        }

        /// <summary>
        /// The stop journey optimiser.
        /// </summary>
        [JsonRpcMethod("StopJourneyOptimiser", Idempotent = true)]
        [JsonRpcHelp("Stops the journey optimiser.")]
        public void StopJourneyOptimiser()
        {
            var jo = ObjectCache.GetObject<JourneyOptimiser>();
            jo.Cancel();
        }

        /// <summary>
        /// The validate field.
        /// </summary>
        /// <param name="propVal">
        /// The prop val.
        /// </param>
        /// <returns>
        /// </returns>
        [JsonRpcMethod("ValidateField", Idempotent = true)]
        public ValidationError ValidateField(PropertyValue propVal)
        {
            return this.SetProperty(null, propVal, true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The set property.
        /// </summary>
        /// <param name="journey">
        /// The journey.
        /// </param>
        /// <param name="propVal">
        /// The prop val.
        /// </param>
        /// <param name="testOnly">
        /// The test only.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        /// <exception cref="JsonException">
        /// </exception>
        /// <exception cref="JsonException">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        private ValidationError SetProperty(Journey journey, PropertyValue propVal, bool testOnly)
        {
            if (propVal.Name == null)
            {
                throw new Exception(
                    "Property value name is null. Cannot proceed with validation. Please check your JSON syntax.");
            }

            if (journey == null && !testOnly)
            {
                throw new JsonException("The journey UUID can only be null if the testOnly attribute is true.");
            }

            // var valErrors = new List<ValidationError>();
            //var jp = ObjectCache.GetObject<JourneyManager>();

            PropertyInfo[] propertyInfos = typeof(EvolutionaryProperties).GetProperties();
            EvolutionaryProperties properties;
            try
            {
                properties = journey.Properties;
            }
            catch (Exception)
            {
                throw new JsonException("The supplied journey UUID was incorrect.");
            }
            {
                // foreach (var propVal in propVals)
                var propertyInfo = propertyInfos.FirstOrDefault(s => s.Name == propVal.Name);

                if (propertyInfo == null)
                {
                    return new ValidationError { Target = propVal.Name, Message = Strings.ERR_PROP_NOT_FOUND };

                    // continue;
                }

                object castVal = null;
                string errMess = string.Empty;
                bool isList = false;
                bool isArray = false;

                if (propertyInfo.PropertyType.IsValueType)
                {
                    try
                    {
                        if (propertyInfo.PropertyType.IsEnum)
                        {
                            castVal = Enum.Parse(propertyInfo.PropertyType, propVal.Value);
                        }
                        else
                        {
                            castVal = Convert.ChangeType(propVal.Value, propertyInfo.PropertyType);
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

                                if (propVal.Value == null)
                                {
                                    throw new Exception("This field cannot be blank.");
                                }

                                PtvDataProvider provider = ObjectCache.GetObjects<PtvDataProvider>()[0];
                                PtvNode node = null;
                                try
                                {
                                    node = (PtvNode)provider.GetNodeFromId(Convert.ToInt32(propVal.Value));
                                }
                                catch (KeyNotFoundException e)
                                {
                                    throw new Exception("There is no such node with the given ID or name.");
                                }
                                
                                    
                                    // provider.GetNodeFromName(propVal.Value);
                                if (node == null)
                                {
                                    throw new Exception("No matching node for supplied name.");
                                }

                                castVal = node;
                                break;

                            case "FitnessParameter[]":

                                var objs = propVal.Value.Split(new[] { ',' });
                                var objectives = new FitnessParameter[objs.Length];
                                int i = 0;
                                foreach (var obj in objs)
                                {
                                    FitnessParameter e;
                                    if (!Enum.TryParse(obj, out e))
                                    {
                                        throw new Exception("Cannot parse string into enum.");
                                    }

                                    objectives[i++] = e;
                                }

                                castVal = objectives;

                                break;
                            default:

                                // var pType = new;
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

                                try
                                {
                                    var types = Assembly.GetAssembly(pType).GetTypes();
                                    pType = types.Where(t => t.Name == propVal.Value).First();
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("Cannot find type in assembly. (" + propVal.Value + ")");
                                }

                                // var pType = Assembly.GetAssembly (propertyInfo.PropertyType).GetType(propVal.Value,true,true);
                                var objects = ObjectCache.GetObjects(pType);
                                if (objects.Length == 0)
                                {
                                    object newObj = null;
                                    try
                                    {
                                        // newObj = pType.InvokeMember("",BindingFlags.CreateInstance,null,null,new[] {properties});
                                        newObj = Activator.CreateInstance(pType, new[] { properties });
                                    }
                                    catch (Exception e)
                                    {
                                        // newObj = pType.InvokeMember("",BindingFlags.CreateInstance,null,null,null);
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
                                    var array = Array.CreateInstance(pType, 1);
                                    array.SetValue(castVal, 0);
                                    castVal = array;
                                }

                                break;

                                // throw new Exception("No handler for type: " + pType.Name);
                        }
                    }
                    catch (Exception e)
                    {
                        return new ValidationError
                            {
                                Target = propVal.Name, 
                                Message = string.Format("{0}: {1}", Strings.ERR_UNSUPP_REFTYPE, e.Message)
                            };
                    }
                }

                if (castVal == null)
                {
                    // ERR_INVALID_CAST
                    return new ValidationError
                        {
                            Target = propVal.Name, 
                            Message = string.Format("{0}: {1}", Strings.ERR_INVALID_CAST, errMess)
                        };

                    // continue;
                }

                if (!testOnly)
                {
                    if (isList)
                    {
                        IList a = (IList)propertyInfo.GetValue(properties, null);

                        a.Add(castVal);
                    }
                    else
                    {
                        propertyInfo.SetValue(properties, castVal, null);
                    }
                }
            }

            return new ValidationError { Target = propVal.Name, Message = Strings.VALIDATION_SUCCESS };
        }

        #endregion

        void Logger_LogEvent(object sender, string message)
        {
            string senderName = sender.GetType().Name;
            //Truncate object names that are too long.
            if (senderName.Length > MaxObjectNameLength)
            {
                senderName = senderName.Substring(0, MaxObjectNameLength);
            }
            Console.WriteLine(String.Format("[{0} ({1})]:{2}", senderName, DateTime.Now.ToShortTimeString(), message));
        }
    }
}