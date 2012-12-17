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
    public class Jsonws : JsonRpcHandler, IRequiresSessionState
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
        /// Initializes a new instance of the <see cref="Jsonws"/> class.
        /// </summary>
        public Jsonws()
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

        [JsonRpcMethod("RegisterUser", Idempotent = true)]
        [JsonRpcHelp("Creates a new default journey for the given user.")]
        public void RegisterUser(string newUuid)
        {
            var jm = ObjectCache.GetObject<JourneyManager>();
            try
            {
                //Delete user if exist.
                jm.DeleteJourney(newUuid);
            }
            catch (KeyNotFoundException e)
            {
                //No Uuid to delete.
            }

            try
            {
                this.CloneJourney("default", newUuid);
            }
            catch (KeyNotFoundException e)
            {
                throw new Exception(Strings.ERR_NO_DEFAULT,e);
            }


        }


        /// <summary>
        /// The clone journey.
        /// </summary>
        /// <param name="journeyUuid">
        /// The journey uuid.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        [JsonRpcMethod("CloneJourney", Idempotent = true)]
        [JsonRpcHelp("Creates a clone of the specified journey.")]
        public void CloneJourney(string journeyUuid)
        {
            if (journeyUuid == null)
            {
                throw new Exception(Strings.ERR_ANY_NULL);
            }

            var jm = ObjectCache.GetObject<JourneyManager>();
            var j = jm.GetJourney(journeyUuid);
            jm.Add((Journey)j.Clone());
        }

        /// <summary>
        /// The clone journey.
        /// </summary>
        /// <param name="journeyUuid">
        /// The journey uuid.
        /// </param>
        /// <param name="newUuid"></param>
        /// <exception cref="Exception">
        /// </exception>
        [JsonRpcMethod("CloneJourneyWithName", Idempotent = true)]
        [JsonRpcHelp("Creates a clone of the specified journey with the specified new name.")]
        public void CloneJourney(string journeyUuid, string newUuid)
        {
            if (journeyUuid == null)
            {
                throw new Exception(Strings.ERR_ANY_NULL);
            }

            var jm = ObjectCache.GetObject<JourneyManager>();
            var j = jm.GetJourney(journeyUuid);
            var newJ = new Journey(newUuid, j.Properties, newUuid, "");
            jm.Add(newJ);
        }

        /// <summary>
        /// The delete journey.
        /// </summary>
        /// <param name="uuid">
        /// The uuid.
        /// </param>
        [JsonRpcMethod("DeleteJourney", Idempotent = true)]
        [JsonRpcHelp("Deletes the specified journey.")]
        public void DeleteJourney(string uuid)
        {
            var jm = ObjectCache.GetObject<JourneyManager>();
            jm.DeleteJourney(uuid);
        }

        /// <summary>
        /// The dequeue journey.
        /// </summary>
        /// <param name="uuid">
        /// The uuid.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        [JsonRpcMethod("DequeueJourney", Idempotent = false)]
        [JsonRpcHelp("Removes the journey specified by its UUID.")]
        public void DequeueJourney(string uuid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The enqueue journey.
        /// </summary>
        /// <param name="uuid">
        /// The uuid.
        /// </param>
        /// <param name="runs">
        /// The runs.
        /// </param>
        /// <returns>
        /// The enqueue journey.
        /// </returns>
        /// <exception cref="JsonException">
        /// </exception>
        [JsonRpcMethod("EnqueueJourney", Idempotent = false)]
        [JsonRpcHelp(
            "Enqueues the journey specified by its UUID in the optimisation queue the number of times specified by runs."
            )]
        public object EnqueueJourney(string uuid, int runs)
        {
            if (runs == default(int))
            {
                runs = 1;
            }

            if (uuid == null)
            {
                throw new JsonException(Strings.ERR_J_NULL);
            }

            var jo = ObjectCache.GetObject<JourneyOptimiser>();

            // var jm = ObjectCache.GetObject<JourneyManager>();
            jo.EnqueueJourney(uuid, runs);
            return null;
        }

        /// <summary>
        /// The get iteration.
        /// </summary>
        /// <param name="journeyUuid">
        /// The journey uuid.
        /// </param>
        /// <param name="runUuid">
        /// The run uuid.
        /// </param>
        /// <param name="iteration">
        /// The iteration.
        /// </param>
        /// <returns>
        /// The get iteration.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        [JsonRpcMethod("GetIteration", Idempotent = true)]
        [JsonRpcHelp("Returns a snapshot of the specified iteration, run and journey.")]
        public string GetIteration(string journeyUuid, string runUuid, int iteration)
        {
            if (journeyUuid == null || runUuid == null)
            {
                throw new Exception(Strings.ERR_ANY_NULL);
            }

            var jm = ObjectCache.GetObject<JourneyManager>();
            var j = jm.GetJourney(journeyUuid);

            if (!j.RunUuids.Contains(runUuid))
            {
                throw new Exception(Strings.ERR_INVALID_RUNID);
            }

            return jm.GetIterationSnapShot(journeyUuid, runUuid, iteration);
        }

        /// <summary>
        /// The get journeys.
        /// </summary>
        /// <returns>
        /// </returns>
        [JsonRpcMethod("GetJourneys", Idempotent = true)]
        [JsonRpcHelp("Gets the list of journey names and UUIDs.")]
        public object[] GetJourneys()
        {
            var jm = ObjectCache.GetObject<JourneyManager>();
            return
                jm.GetJourneys().Select(
                    s => (object)new { uuid = s.Uuid, shortName = s.ShortName, description = s.Description }).ToArray();
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
        /// The get optimisation state.
        /// </summary>
        /// <returns>
        /// The get optimisation state.
        /// </returns>
        [JsonRpcMethod("GetOptimisationState", Idempotent = true)]
        [JsonRpcHelp("Returns the state and queue of the optimisation mananger.")]
        public object GetOptimisationState()
        {
            var jo = ObjectCache.GetObject<JourneyOptimiser>();
            if (jo == null)
            {
                return new { state = "Not instantiated" };
            }

            return
                new
                    {
                        state = jo.State, 
                        queue = jo.GetQueue(), 
                        currentIteration = jo.CurrentIteration, 
                        totalIterations = jo.MaxIterations, 
                        currentJourney = jo.CurrentJourney == null ? "None" : jo.CurrentJourney.Uuid
                    };
        }

        /// <summary>
        /// The get properties.
        /// </summary>
        /// <param name="journeyUuid">
        /// The journey uuid.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="JsonException">
        /// </exception>
        [JsonRpcMethod("GetProperties", Idempotent = true)]
        [JsonRpcHelp("Gets all the properties accessable for the journey planner run.")]
        public ObjectProperty[] GetProperties(string journeyUuid)
        {
            PropertyInfo[] propertyInfos =
                typeof(EvolutionaryProperties).GetProperties().OrderBy(p => p.PropertyType.Name).ToArray();
            var objectProperties = new ObjectProperty[propertyInfos.Length];

            // EvolutionaryProperties properties = ObjectCache.GetObjects<EvolutionaryProperties>().First();
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

                // TODO: Really fix this!
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
                        value = string.Empty;
                        PtvNode node = (PtvNode)propertyInfos[i].GetValue(properties, null) ?? null;
                        if (node != null)
                        {
                            value = node.StopSpecName + delimeter + node.Id;
                        }
                    }
                    else
                    {
                        editable = false;
                        var types = Assembly.GetAssembly(pType).GetTypes();
                        var candidates =
                            types.Where(t => (t == pType && !t.IsInterface) || t.GetInterfaces().Contains(pType));
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

                        // string valstring = (val != null ? val.GetType().Name : " ");
                        value = string.Join(delimeter, candidates.Select(c => c.Name)) + "@" + valstring;
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
                            // TODO: Make this generic
                            valstring = string.Join(",", (FitnessParameter[])val);
                            seperator = "|";
                        }
                        else
                        {
                            valstring = val != null ? val.ToString() : " ";
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

        /// <summary>
        /// The get runs.
        /// </summary>
        /// <param name="uuid">
        /// The uuid.
        /// </param>
        /// <returns>
        /// </returns>
        [JsonRpcMethod("GetRuns", Idempotent = true)]
        [JsonRpcHelp("Gets a list of runs associated with the specified journey UUID.")]
        public string[] GetRuns(string uuid)
        {
            var jm = ObjectCache.GetObject<JourneyManager>();
            return jm.GetJourney(uuid).RunUuids;
        }

        /// <summary>
        /// The get total iterations.
        /// </summary>
        /// <param name="journeyUuid">
        /// The journey uuid.
        /// </param>
        /// <returns>
        /// The get total iterations.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        [JsonRpcMethod("GetTotalIterations", Idempotent = true)]
        [JsonRpcHelp("Returns the max number of iterations for a journey.")]
        public int GetTotalIterations(string journeyUuid)
        {
            if (journeyUuid == null)
            {
                throw new Exception(Strings.ERR_ANY_NULL);
            }

            var jm = ObjectCache.GetObject<JourneyManager>();
            var j = jm.GetJourney(journeyUuid);
            return j.Properties.MaxIterations;
        }

        /// <summary>
        /// The load providers.
        /// </summary>
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

        void Logger_LogEvent(object sender, string message)
        {
            string senderName = sender.GetType().Name;
            //Truncate object names that are too long.
            if (senderName.Length > MaxObjectNameLength)
            {
                senderName = senderName.Substring(0, MaxObjectNameLength);
            }
            Console.WriteLine(String.Format("[{0} ({1})]:{2}", senderName, DateTime.Now.ToShortTimeString(),message));
        }

        /// <summary>
        /// The new journey.
        /// </summary>
        /// <param name="shortName">
        /// The short name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <returns>
        /// The new journey.
        /// </returns>
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

        /// <summary>
        /// The pause journey optimiser.
        /// </summary>
        [JsonRpcMethod("PauseJourneyOptimiser", Idempotent = true)]
        [JsonRpcHelp("Pauses the journey optimiser at the next available oportunity.")]
        public void PauseJourneyOptimiser()
        {
            var jo = ObjectCache.GetObject<JourneyOptimiser>();
            jo.Pause();
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
            var provider = ObjectCache.GetObjects<PtvDataProvider>().First();
            var stops = provider.QueryStops(query);

            // var stopStrings = stops.Select(s => s.StopSpecName);
            // return stopStrings.ToArray();
            return stops.ToArray();
        }

        /// <summary>
        /// The resume journey optimiser.
        /// </summary>
        [JsonRpcMethod("ResumeJourneyOptimiser", Idempotent = true)]
        [JsonRpcHelp("Resumes the journey optimiser if paused.")]
        public void ResumeJourneyOptimiser()
        {
            var jo = ObjectCache.GetObject<JourneyOptimiser>();
            jo.Resume();
        }

        /// <summary>
        /// The save journeys.
        /// </summary>
        [JsonRpcMethod("SaveJourneys", Idempotent = true)]
        [JsonRpcHelp("Saves the current journeys to disk so they can be persistant.")]
        public void SaveJourneys()
        {
            var jp = ObjectCache.GetObject<JourneyManager>();
            jp.Save();
        }

        /// <summary>
        /// The set journey name.
        /// </summary>
        /// <param name="uuid">
        /// The uuid.
        /// </param>
        /// <param name="shortName">
        /// The short name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        [JsonRpcMethod("SetJourneyName", Idempotent = true)]
        [JsonRpcHelp("Sets the short name and description of the journey.")]
        public void SetJourneyName(string uuid, string shortName, string description)
        {
            var jm = ObjectCache.GetObject<JourneyManager>();
            var j = jm.GetJourney(uuid);
            j.ShortName = shortName;
            j.Description = description;
        }

        /// <summary>
        /// The set properties.
        /// </summary>
        /// <param name="journeyUuid">
        /// The journey uuid.
        /// </param>
        /// <param name="propVals">
        /// The prop vals.
        /// </param>
        /// <returns>
        /// </returns>
        [JsonRpcMethod("SetProperties", Idempotent = true)]
        [JsonRpcHelp(
            "Accepts an array of property names and values and attempts to modify the property object. Returns an array of validation errors."
            )]
        public ValidationError[] SetProperties(string journeyUuid, PropertyValue[] propVals)
        {
            /*
			 FORMAT:
			 { "propVals" : [{ "propVal" : {"name":"CrossoverRate","value":"0.7"} }] }
			 */
            List<ValidationError> valErrors = new List<ValidationError>();
            foreach (var propVal in propVals)
            {
                var valError = this.SetProperty(journeyUuid, propVal);
                if (valError != null)
                {
                    valErrors.Add(valError);
                }
            }

            this.SaveJourneys();
            return valErrors.ToArray();
        }

        /// <summary>
        /// The set property.
        /// </summary>
        /// <param name="journeyUuid">
        /// The journey uuid.
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
        public ValidationError SetProperty(string journeyUuid, PropertyValue propVal)
        {
            return this.SetProperty(journeyUuid, propVal, false);
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
        /// <param name="journeyUuid">
        /// The journey uuid.
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
        private ValidationError SetProperty(string journeyUuid, PropertyValue propVal, bool testOnly)
        {
            if (propVal.Name == null)
            {
                throw new Exception(
                    "Property value name is null. Cannot proceed with validation. Please check your JSON syntax.");
            }

            if (journeyUuid == null && !testOnly)
            {
                throw new JsonException("The journey UUID can only be null if the testOnly attribute is true.");
            }

            // var valErrors = new List<ValidationError>();
            var jp = ObjectCache.GetObject<JourneyManager>();

            PropertyInfo[] propertyInfos = typeof(EvolutionaryProperties).GetProperties();
            EvolutionaryProperties properties;
            try
            {
                properties = journeyUuid == null ? new EvolutionaryProperties() : jp.GetJourney(journeyUuid).Properties;
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
                                PtvDataProvider provider = ObjectCache.GetObjects<PtvDataProvider>()[0];
                                PtvNode node = (PtvNode)provider.GetNodeFromId(Convert.ToInt32(propVal.Value));
                                    
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
    }
}