// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomImporter.cs" company="">
//   
// </copyright>
// <summary>
//   The custom importer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    #region Using Directives

    using System;

    using Jayrock.Json;
    using Jayrock.Json.Conversion;

    using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary;

    #endregion

    /// <summary>
    /// The custom importer.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class CustomImporter<T> : IImporter
    {
        #region Constants and Fields

        /// <summary>
        /// The properties.
        /// </summary>
        private readonly EvolutionaryProperties properties;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomImporter{T}"/> class.
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        public CustomImporter(EvolutionaryProperties properties)
        {
            this.properties = properties;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets OutputType.
        /// </summary>
        public Type OutputType
        {
            get
            {
                return typeof(T);
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
            JsonToken token = reader.Token;
            string type = token.Text;
            Type ttype = typeof(T);
            Type t = ttype.Assembly.GetType(type);
            return Activator.CreateInstance(t, new object[] { this.properties });
        }

        #endregion
    }
}