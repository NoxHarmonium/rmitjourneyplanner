// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectCache.cs" company="">
//   
// </copyright>
// <summary>
//   The object cache.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    /// The object cache.
    /// </summary>
    public static class ObjectCache
    {
        #region Constants and Fields

        /// <summary>
        /// The object map.
        /// </summary>
        private static readonly Dictionary<string, List<object>> objectMap = new Dictionary<string, List<object>>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds the object o to the object map under the type t.
        /// </summary>
        /// <param name="t">
        /// The type of the object.
        /// </param>
        /// <param name="o">
        /// The object.
        /// </param>
        public static void AddObject(string t, object o)
        {
            if (!objectMap.ContainsKey(t))
            {
                objectMap[t] = new List<object>();
            }

            objectMap[t].Add(o);
        }

        /// <summary>
        /// Removes the specific object from the object map.
        /// </summary>
        /// <param name="o">
        /// </param>
        public static void DeregisterObject(object o)
        {
            RemoveObject(o.GetType().Name, o);

            Type[] interfaces = o.GetType().GetInterfaces();

            // Explore all interfaces until the base interface is reached (_Type).
            if (!interfaces.Select(s => s.Name).Contains("_Type"))
            {
                foreach (var interFace in interfaces)
                {
                    RemoveObject(interFace.Name, o);
                }
            }
        }

        /// <summary>
        /// The deregister objects.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        public static void DeregisterObjects(Type t)
        {
            Type[] interfaces = t.GetInterfaces();
            Type[] prev = new[] { t };

            // Explore all interfaces until the base interface is reached (_Type).
            while (!interfaces.Select(s => s.Name).Contains("_Type") && interfaces.Any())
            {
                prev = interfaces;
                interfaces = interfaces.First().GetInterfaces();
            }

            DeregisterSubObjects(prev[0]);
        }

        /// <summary>
        /// Removes all instances of the specified  type from the object map.
        /// </summary>
        /// <param name="t">
        /// </param>
        public static void DeregisterSubObjects(Type t)
        {
            RemoveObjects(t.Name);

            var sc = findSubclassesOf(t);

            foreach (var subT in sc)
            {
                DeregisterSubObjects(subT);
            }
        }

        /// <summary>
        /// The get object.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public static T GetObject<T>()
        {
            return GetObjects<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets all objects of type T.
        /// </summary>
        /// <typeparam name="T">
        /// The type of objects returned.
        /// </typeparam>
        /// <returns>
        /// All objects registered under the specified type.
        /// </returns>
        public static T[] GetObjects<T>()
        {
            Type type = typeof(T);

            // Type[] interfaces = type.GetInterfaces();
            // while (interfaces.Any())
            // {
            // type = interfaces[0].GetInterfaces().First();
            // interfaces = type.GetInterfaces();
            // }
            var objects = GetObjects(type);

            return objects.Cast<T>().ToArray();
        }

        /// <summary>
        /// The get objects.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// </returns>
        public static object[] GetObjects(Type type)
        {
            if (objectMap.ContainsKey(type.Name))
            {
                return objectMap[type.Name].ToArray();
            }
            else
            {
                return new object[0];
            }
        }

        /// <summary>
        /// Registers an object in the object map.
        /// </summary>
        /// <param name="o">
        /// The object that is to be registered.
        /// </param>
        public static void RegisterObject(object o)
        {
            // Type[] interfaces = o.GetType().GetInterfaces();

            // Explore all interfaces until the base interface is reached (_Type).
            // if (!interfaces.Select(s => s.Name).Contains("_Type"))
            // {
            // foreach (var interFace in interfaces)
            // {
            // this.AddObject(interFace.Name,o);
            // }

            // }
            Type objectType = o.GetType();
            AddObject(objectType.Name, o);
        }

        /// <summary>
        /// The remove object.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <param name="o">
        /// The o.
        /// </param>
        public static void RemoveObject(string t, object o)
        {
            if (objectMap.ContainsKey(t))
            {
                objectMap[t].Remove(o);
            }
        }

        /// <summary>
        /// Removes all instances of the specified type from the object map.
        /// </summary>
        /// <param name="t">
        /// </param>
        public static void RemoveObjects(string t)
        {
            if (objectMap.ContainsKey(t))
            {
                objectMap[t].Clear();
                objectMap.Remove(t);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The find subclasses of.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <returns>
        /// </returns>
        private static Type[] findSubclassesOf(Type t)
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.IsSubclassOf(t) || type.GetInterfaces().Contains(t)
                    select type).ToArray();
        }

        #endregion
    }
}