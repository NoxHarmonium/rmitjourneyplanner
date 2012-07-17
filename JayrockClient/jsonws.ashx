<%@ WebHandler Class="JayrockClient.Jsonws" Language="C#" %>

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
    using Jayrock.JsonRpc;
    using Jayrock.JsonRpc.Web;
    using RmitJourneyPlanner.CoreLibraries.DataProviders;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Google;
    using System.Linq;

    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;

    #endregion

    [JsonRpcHelp("This is a JSON-RPC service that demonstrates the basic features of the Jayrock library.")]
    public class Jsonws : JsonRpcHandler, IRequiresSessionState
    {
        private static readonly Dictionary<String,List<Object>> objectMap = new Dictionary<String, List<object>>();
        
       
        /// <summary>
        /// Removes all instances of the specified  type from the object map.
        /// </summary>
        /// <param name="t"></param>
        private void DeregisterSubObjects(Type t)
        {
            this.RemoveObjects(t.Name);
            
            
            var sc = findSubclassesOf(t);

            foreach (var subT in sc)
            {
                this.DeregisterSubObjects(subT);
            }
            

        }
        
        private void DeregisterObjects(Type t)
        {
            
            Type[] interfaces = t.GetInterfaces();
            Type[] prev = new []{t};

            //Explore all interfaces until the base interface is reached (_Type).
            while (!interfaces.Select(s => s.Name).Contains("_Type") && interfaces.Any())
            {
                prev = interfaces;
                interfaces = interfaces.First().GetInterfaces();
            }
            
            this.DeregisterSubObjects(prev[0]);
            
            
        }
        
        private Type[] findSubclassesOf(Type t)
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   from type in assembly.GetTypes()
                   where type.IsSubclassOf(t) || type.GetInterfaces().Contains(t)
                   select type).ToArray();
        }


        /// <summary>
        /// Removes the specific object from the object map.
        /// </summary>
        /// <param name="o"></param>
        private void DeregisterObject(Object o)
        {

            this.RemoveObject(o.GetType().Name, o);
            
            Type[] interfaces = o.GetType().GetInterfaces();
            
            //Explore all interfaces until the base interface is reached (_Type).
            if (!interfaces.Select(s => s.Name).Contains("_Type"))
            {
                foreach (var interFace in interfaces)
                {
                    this.RemoveObject(interFace.Name, o);
                }
            }



        }
        
        /// <summary>
        /// Registers an object in the object map.
        /// </summary>
        /// <param name="o">The object that is to be registered.</param>
        private void RegisterObject(Object o)
        {
            
            Type[] interfaces = o.GetType().GetInterfaces();
            
            //Explore all interfaces until the base interface is reached (_Type).
            if (!interfaces.Select(s => s.Name).Contains("_Type"))
            {
                foreach (var interFace in interfaces)
                {
                    this.AddObject(interFace.Name,o);
                }
                
            }
            
            Type objectType = o.GetType();
            this.AddObject(objectType.Name, o);
        }
        
        /// <summary>
        /// Adds the object o to the object map under the type t.
        /// </summary>
        /// <param name="t">The type of the object.</param>
        /// <param name="o">The object.</param>
        private void AddObject(String t, Object o)
        {
            if (!objectMap.ContainsKey(t))
            {
                objectMap[t] = new List<object>();
            }

            objectMap[t].Add(o);
            
        }
        
        /// <summary>
        /// Removes all instances of the specified type from the object map.
        /// </summary>
        /// <param name="t"></param>
        private void RemoveObjects(string t)
        {
            if (objectMap.ContainsKey(t))
            {
                objectMap[t].Clear();
                objectMap.Remove(t);
            }
        }
       
        private void RemoveObject(string t, object o)
        {
            if (objectMap.ContainsKey(t))
            {
                objectMap[t].Remove(o);
            }
        }
        
        /// <summary>
        /// Gets all objects of type T.
        /// </summary>
        /// <typeparam name="T">The type of objects returned.</typeparam>
        /// <returns>All objects registered under the specified type.</returns>
        private T[] GetObjects<T>()
        {
            Type type = typeof(T);
            Type[] interfaces = type.GetInterfaces();
            while (interfaces.Any())
            {
                type = interfaces[0].GetInterfaces().First();
                interfaces = type.GetInterfaces();
            }

            return objectMap[type.Name].Cast<T>().ToArray();

        }
        
        [JsonRpcMethod("LoadProviders", Idempotent = true)]
        [JsonRpcHelp("Initialises the data providers.")]
        public void LoadProviders()
        {
            this.DeregisterObjects(typeof(IPointDataProvider));
            this.DeregisterObjects(typeof(INetworkDataProvider));
            IPointDataProvider provider = new WalkingDataProvider();
            this.RegisterObject(provider);
            this.RegisterObject(new EvolutionaryProperties());
        }

        [JsonRpcMethod("GetProperties", Idempotent = true)]
        [JsonRpcHelp("Gets all the properties accessable for the journey planner run.=.")]
        public ObjectProperty[] GetProperties()
        {
            PropertyInfo[] propertyInfos = typeof(EvolutionaryProperties).GetProperties();
            var objectProperties = new ObjectProperty[propertyInfos.Length];
            EvolutionaryProperties properties = this.GetObjects<EvolutionaryProperties>().First();

            for (int i = 0; i < propertyInfos.Length; i++ )
            {
                object value = propertyInfos[i].GetValue(properties, null) ?? "";
                objectProperties[i] = new ObjectProperty
                    { Name = propertyInfos[i].Name, Type = propertyInfos[i].PropertyType.Name, Value = value.ToString()};
            }

            return objectProperties;
        }

        
       
    }
}
