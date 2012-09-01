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
	using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
    using RmitJourneyPlanner.CoreLibraries.DataProviders.Google;
    using System.Linq;
	using Jayrock.Json;
	using Jayrock.Json.Conversion;
    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;

namespace JayrockClient
{
	public class PropertiesExporter : IExporter
	{
		public void Export (ExportContext context, object value, JsonWriter writer)
		{
			PropertyInfo[] propertyInfos = typeof(EvolutionaryProperties).GetProperties();
            var objectProperties = new ObjectProperty[propertyInfos.Length];
            EvolutionaryProperties properties = (EvolutionaryProperties) value;
		
			string delimeter = ", ";
            for (int i = 0; i < propertyInfos.Length; i++) 
            {
                bool editable = true;
				bool isArray = false;
				object v = null;
				Type pType = propertyInfos[i].PropertyType;
				
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
						
					}
					
					if (pType.Name == "INetworkNode")
					{
						
						v = "";
						MetlinkNode node = (MetlinkNode) propertyInfos[i].GetValue(properties, null) ?? null;
						if (node != null)
						{
							v = node.StopSpecName + delimeter + node.Id;
						}
						
					}
					else
					{
						editable = false;
						//var types = Assembly.GetAssembly(pType).GetTypes();
						//var candidates = types.Where(t => (t == pType && !t.IsInterface) ||
						//                             	t.GetInterfaces().Contains(pType));
						
						if (isArray)
						{
							object o = propertyInfos[i].GetValue(properties, null);
							IEnumerable arr = 	(IEnumerable) o;
							foreach (var a in arr)
							{
								v = (a != null ? a.GetType().FullName : " ");
								break;
							}
							
							
							
							
						}
						else
						{
							var val = propertyInfos[i].GetValue(properties, null);
							v = (val != null ? val.ToString() : " ");
							
						}
						
					}
					
					
				}
				else
				{
					if (pType.IsEnum)
					{
						var val = propertyInfos[i].GetValue(properties, null);
						string valstring = (val != null ? val.ToString() : " ");
						
						v = valstring;
						editable = false;
					}
					else
					{
						v = propertyInfos[i].GetValue(properties, null) ?? "null";				
					}
				}
				
				
                objectProperties[i] = new ObjectProperty
                    {
						Name = propertyInfos[i].Name,
						Type = propertyInfos[i].PropertyType.Name, 
						Value = v.ToString(), 
						Editable = editable
					};
            }
			/*
			writer.WriteStartArray();
			foreach (var o in objectProperties)
			{
				writer.WriteStartObject();
				writer.WriteMember("Name");
				writer.WriteString(o.Name);
				writer.WriteMember("Type");
				writer.WriteString(o.Type);
				writer.WriteMember("Value");
				writer.WriteString(o.Value);
				writer.WriteMember("Editable");
				writer.WriteBoolean(o.Editable);				
				
			}
			writer.WriteEndArray();
			*/
			var exporter = context.FindExporter(objectProperties.GetType());
			exporter.Export(context,objectProperties,writer);
			
			
            //return objectProperties;
		}

		public Type InputType {
			get {
				return typeof(EvolutionaryProperties);
			}
		}
	}
}

