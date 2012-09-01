using System;
using Jayrock.Json;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace JayrockClient
{
	public static class ObjectTools
	{
		public static T getObject<T>(JsonObject jObj)
		{
			var constructors = typeof(T).GetConstructors();
			foreach(var c in constructors)
			{
				if (c.GetParameters().Length > 0)
				{
					List<Object> pObjects = new List<Object>();
					foreach (var p in c.GetParameters())
					{
						var o = jObj[p.Name];
						
						if (o == null)
						{
							throw new JsonException("The supplied JSON Object does not match the constructor of object type T.");	
						}
						pObjects.Add(o);
						
					}
					
					return (T)Activator.CreateInstance(typeof(T),pObjects.ToArray());
					
				}
				
			}
			
			return default(T);
			
		}
	}
}

