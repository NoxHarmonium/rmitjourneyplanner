using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace JayrockClient
{
	public static class ObjectCache
	{
		private static readonly Dictionary<String,List<Object>> objectMap = new Dictionary<String, List<object>>();
     	
		
				
		/// <summary>
        /// Removes all instances of the specified  type from the object map.
        /// </summary>
        /// <param name="t"></param>
        public static void DeregisterSubObjects(Type t)
        {
            RemoveObjects(t.Name);
            
            
            var sc = findSubclassesOf(t);

            foreach (var subT in sc)
            {
                DeregisterSubObjects(subT);
            }
            

        }
        
        public static void DeregisterObjects(Type t)
        {
            
            Type[] interfaces = t.GetInterfaces();
            Type[] prev = new []{t};

            //Explore all interfaces until the base interface is reached (_Type).
            while (!interfaces.Select(s => s.Name).Contains("_Type") && interfaces.Any())
            {
                prev = interfaces;
                interfaces = interfaces.First().GetInterfaces();
            }
            
           DeregisterSubObjects(prev[0]);
            
            
        }
        
        private static Type[] findSubclassesOf(Type t)
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
        public static void DeregisterObject(Object o)
        {

            RemoveObject(o.GetType().Name, o);
            
            Type[] interfaces = o.GetType().GetInterfaces();
            
            //Explore all interfaces until the base interface is reached (_Type).
            if (!interfaces.Select(s => s.Name).Contains("_Type"))
            {
                foreach (var interFace in interfaces)
               {
                    RemoveObject(interFace.Name, o);
                }
            }



        }
        
        /// <summary>
        /// Registers an object in the object map.
        /// </summary>
        /// <param name="o">The object that is to be registered.</param>
        public static void RegisterObject(Object o)
        {
            
           // Type[] interfaces = o.GetType().GetInterfaces();
            
            //Explore all interfaces until the base interface is reached (_Type).
            //if (!interfaces.Select(s => s.Name).Contains("_Type"))
           // {
            //    foreach (var interFace in interfaces)
             //   {
                    //this.AddObject(interFace.Name,o);
             //   }
                
            //}
            
            Type objectType = o.GetType();
            AddObject(objectType.Name, o);
        }
        
        /// <summary>
        /// Adds the object o to the object map under the type t.
        /// </summary>
        /// <param name="t">The type of the object.</param>
        /// <param name="o">The object.</param>
        public static void AddObject(String t, Object o)
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
        public static void RemoveObjects(string t)
        {
            if (objectMap.ContainsKey(t))
            {
                objectMap[t].Clear();
                objectMap.Remove(t);
            }
        }
       
        public static void RemoveObject(string t, object o)
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
        public static T[] GetObjects<T>()
        {
            Type type = typeof(T);
            //Type[] interfaces = type.GetInterfaces();
            //while (interfaces.Any())
            //{
             //   type = interfaces[0].GetInterfaces().First();
              //  interfaces = type.GetInterfaces();
           // }
			
			var objects = GetObjects(type);
			
			return objects.Cast<T>().ToArray();
			
        }
		
		public static T GetObject<T>()
		{
			return  GetObjects<T>().FirstOrDefault();	
		}
		
		public static Object[] GetObjects(Type type)
		{
			if (objectMap.ContainsKey(type.Name))
			{
            	return objectMap[type.Name].ToArray();
			}
			else
			{	
				return new Object[0];
			}
		}
	}
}

