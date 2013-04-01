// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectTools.cs" company="">
//   
// </copyright>
// <summary>
//   The object tools.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    using Jayrock.Json;

    #endregion

    /// <summary>
    /// The object tools.
    /// </summary>
    public static class ObjectTools
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get object.
        /// </summary>
        /// <param name="jObj">
        /// The j obj.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        /// <exception cref="JsonException">
        /// </exception>
        public static T getObject<T>(JsonObject jObj)
        {
            var constructors = typeof(T).GetConstructors();
            foreach (var c in constructors)
            {
                if (c.GetParameters().Length > 0)
                {
                    List<object> pObjects = new List<object>();
                    foreach (var p in c.GetParameters())
                    {
                        var o = jObj[p.Name];

                        if (o == null)
                        {
                            throw new JsonException(
                                "The supplied JSON Object does not match the constructor of object type T.");
                        }

                        pObjects.Add(o);
                    }

                    return (T)Activator.CreateInstance(typeof(T), pObjects.ToArray());
                }
            }

            return default(T);
        }

        #endregion
    }
}