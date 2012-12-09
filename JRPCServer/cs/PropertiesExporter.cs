// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertiesExporter.cs" company="">
//   
// </copyright>
// <summary>
//   The properties exporter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    #region Using Directives

    using System;
    using System.Linq;
    using System.Reflection;

    using Jayrock.Json;
    using Jayrock.Json.Conversion;

    using RmitJourneyPlanner.CoreLibraries.DataProviders.Ptv;
    using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// The properties exporter.
    /// </summary>
    public class PropertiesExporter : IExporter
    {
        #region Public Properties

        /// <summary>
        /// Gets InputType.
        /// </summary>
        public Type InputType
        {
            get
            {
                return typeof(EvolutionaryProperties);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The export.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public void Export(ExportContext context, object value, JsonWriter writer)
        {
            var properties = (EvolutionaryProperties)value;
            PropertyInfo[] propertyInfos =
                typeof(EvolutionaryProperties).GetProperties().OrderBy(p => p.PropertyType.Name).ToArray();
            var objectProperties = new ObjectProperty[propertyInfos.Length];

            // EvolutionaryProperties properties = ObjectCache.GetObjects<EvolutionaryProperties>().First();
            var jp = ObjectCache.GetObject<JourneyManager>();

            string delimeter = ", ";
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                bool editable = true;
                object v = null;
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
                        v = string.Empty;
                        PtvNode node = (PtvNode)propertyInfos[i].GetValue(properties, null) ?? null;
                        if (node != null)
                        {
                            v = node.StopSpecName + delimeter + node.Id;
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
                        v = string.Join(delimeter, candidates.Select(c => c.Name)) + "@" + valstring;
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

            // return objectProperties;
            var exporter = context.FindExporter(objectProperties.GetType());
            exporter.Export(context, objectProperties, writer);

            // return objectProperties;
        }

        #endregion
    }
}