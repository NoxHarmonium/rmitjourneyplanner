// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertiesImporter.cs" company="">
//   
// </copyright>
// <summary>
//   The properties importer.
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

    using Jayrock.Json;
    using Jayrock.Json.Conversion;

    using RmitJourneyPlanner.CoreLibraries.DataProviders.Ptv;
    using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary;
    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// The properties importer.
    /// </summary>
    public class PropertiesImporter : IImporter
    {
        #region Constants and Fields

        /// <summary>
        /// The properties.
        /// </summary>
        private EvolutionaryProperties properties = new EvolutionaryProperties();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets OutputType.
        /// </summary>
        public Type OutputType
        {
            get
            {
                return typeof(EvolutionaryProperties);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The import.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The import.
        /// </returns>
        public object Import(ImportContext context, JsonReader reader)
        {
            // var properties = new EvolutionaryProperties();
            // reader.StepOut();
            // JsonToken token = reader.Token;
            this.properties = new EvolutionaryProperties();

            PropertyValue[] o = context.Import<PropertyValue[]>(reader);
            this.SetProperties(o);

            return this.properties;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The set properties.
        /// </summary>
        /// <param name="propVals">
        /// The prop vals.
        /// </param>
        /// <returns>
        /// </returns>
        private ValidationError[] SetProperties(PropertyValue[] propVals)
        {
            /*
			 FORMAT:
			 { "propVals" : [{ "propVal" : {"name":"CrossoverRate","value":"0.7"} }] }
			 */
            List<ValidationError> valErrors = new List<ValidationError>();
            foreach (var propVal in propVals)
            {
                var valError = this.SetProperty(propVal);
                if (valError != null)
                {
                    valErrors.Add(valError);
                }
            }

            return valErrors.ToArray();
        }

        /// <summary>
        /// The set property.
        /// </summary>
        /// <param name="propVal">
        /// The prop val.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        private ValidationError SetProperty(PropertyValue propVal)
        {
            var testOnly = false;
            if (propVal.Name == null)
            {
                throw new Exception(
                    "Property value name is null. Cannot proceed with validation. Please check your JSON syntax.");
            }

            // var valErrors = new List<ValidationError>();
            PropertyInfo[] propertyInfos = typeof(EvolutionaryProperties).GetProperties();

            // EvolutionaryProperties properties = properties'
            {
                // foreach (var propVal in propVals)
                var propertyInfo = propertyInfos.Where(s => s.Name == propVal.Name).FirstOrDefault();

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
                            castVal = Enum.Parse(propertyInfo.PropertyType, propVal.Value.Split(new[] { '@' })[1]);
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
                                PtvDataProvider provider = ObjectCache.GetObjects<PtvDataProvider>().First();
                                PtvNode node =
                                    (PtvNode)
                                    provider.GetNodeFromId(Convert.ToInt32(propVal.Value.Split(new[] { ',' })[1]));
                                    
                                    // provider.GetNodeFromName(propVal.Value);
                                if (node == null)
                                {
                                    throw new Exception("No matching node for supplied name.");
                                }

                                castVal = node;
                                break;

                            case "FitnessParameter[]":

                                var objs = propVal.Value.Split(new[] { '|' })[1].Split(new[] { ',' });
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
                                    var strType = propVal.Value.Split(new[] { '@' })[1];
                                    pType = types.First(t => t.Name == strType);
                                }
                                catch (Exception)
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
                                        newObj = Activator.CreateInstance(pType, new[] { this.properties });
                                    }
                                    catch (Exception)
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
                        IList a = (IList)propertyInfo.GetValue(this.properties, null);

                        a.Add(castVal);
                    }
                    else
                    {
                        propertyInfo.SetValue(this.properties, castVal, null);
                    }
                }
            }

            return new ValidationError { Target = propVal.Name, Message = Strings.VALIDATION_SUCCESS };
        }

        #endregion
    }
}