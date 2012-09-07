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
		    var properties = (EvolutionaryProperties)value;
            PropertyInfo[] propertyInfos = typeof(EvolutionaryProperties).GetProperties().OrderBy(p => p.PropertyType.Name).ToArray();
            var objectProperties = new ObjectProperty[propertyInfos.Length];
            //EvolutionaryProperties properties = ObjectCache.GetObjects<EvolutionaryProperties>().First();
            var jp = ObjectCache.GetObject<JourneyManager>();
      
            

            string delimeter = ", ";
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                bool editable = true;
                object v = null;
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

                        v = "";
                        MetlinkNode node = (MetlinkNode)propertyInfos[i].GetValue(properties, null) ?? null;
                        if (node != null)
                        {
                            v= node.StopSpecName + delimeter + node.Id;
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
                        v= String.Join(delimeter, candidates.Select(c => c.Name)) + "@" + valstring;
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
                            valstring = String.Join(",", ((RmitJourneyPlanner.CoreLibraries.Types.FitnessParameter[])val));
                            seperator = "|";
                        }
                        else
                        {
                            valstring = (val != null ? val.ToString() : " ");

                        }

                        v = string.Join(delimeter, Enum.GetNames(pType)) + seperator + valstring;
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

            //return objectProperties;
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

