// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CritterExporter.cs" company="">
//   
// </copyright>
// <summary>
//   The critter exporter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    #region Using Directives

    using System;

    using Jayrock.Json;
    using Jayrock.Json.Conversion;

    using RmitJourneyPlanner.CoreLibraries.Types;

    #endregion

    /// <summary>
    /// The critter exporter.
    /// </summary>
    public class CritterExporter : IExporter
    {
        /// <summary>
        /// Specifies how many properties of the <see cref="Critter"/> object get 
        /// exported to JSON.
        /// </summary>
        public enum ExportType
        {
            /// <summary>
            /// Only a few essential properties are exported. 
            /// Use this to export a journey summary to the web application.
            /// </summary>
            Summary,
            /// <summary>
            /// Important properties are exported.
            /// </summary>
            Simple,
            /// <summary>
            /// Most properties are exported.
            /// </summary>
            Expanded
        }        
        
        #region Constants and Fields

        private ExportType exportType;
        
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the input type of the <see cref="CritterExporter"/> object.
        /// </summary>
        public Type InputType
        {
            get
            {
                return typeof(Critter);
            }
        }    

        #endregion     
   
        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="CritterExporter"/> class with the specifed <see cref="ExportType"/>.
        /// </summary>
        /// <param name="exportType">An <see cref="ExportType"/> values the represents the required detail level of the export.</param>
        public CritterExporter(ExportType exportType)
        {
            this.exportType = exportType;
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
            writer.WriteStartObject();
            writer.WriteMember("Critter");

            var critter = (Critter)value;
            writer.WriteStartObject();

            if (exportType > ExportType.Expanded)
            {
                writer.WriteMember("Age");
                writer.WriteNumber(critter.Age);
                writer.WriteMember("N");
                writer.WriteNumber(critter.N);             
            }

            writer.WriteMember("Rank");
            writer.WriteNumber(critter.Rank);

            writer.WriteMember("Fitness");
            writer.WriteStartObject();
            int i = 0;
            foreach (var n in Enum.GetNames(typeof(FitnessParameter)))
            {
                writer.WriteMember(n);
                writer.WriteNumber(critter.Fitness[i++]);
            }

            writer.WriteEndObject();

            // writer.WriteMember("Fitness");
            // context.Export(critter.Fitness,writer);
            if (exportType > ExportType.Summary)
            {
                writer.WriteMember("Route");
                writer.WriteStartArray();
                foreach (var node in critter.Route)
                {
                    writer.WriteNumber(node.Node.Id);
                }
                writer.WriteEndArray();
            }

            writer.WriteMember("Legs");
            writer.WriteStartArray();
            foreach (var leg in critter.Fitness.JourneyLegs)
            {
                writer.WriteStartObject();

                if (exportType > ExportType.Summary)
                {
                    writer.WriteMember("Start");
                    writer.WriteNumber(leg.Origin.Id);
                    writer.WriteMember("End");
                    writer.WriteNumber(leg.Destination.Id);
                    writer.WriteMember("Route");
                    writer.WriteString(leg.RouteId1);

                }

                if (exportType > ExportType.Simple)
                {
                    writer.WriteMember("StartLocation");
                    writer.WriteStartObject();
                    writer.WriteMember("Lat");
                    writer.WriteNumber(leg.Origin.Latitude);
                    writer.WriteMember("Long");
                    writer.WriteNumber(leg.Origin.Longitude);
                    writer.WriteEndObject();                    
                }

                if (exportType > ExportType.Simple)
                {
                    writer.WriteMember("EndLocation");
                    writer.WriteStartObject();
                    writer.WriteMember("Lat");
                    writer.WriteNumber(leg.Destination.Latitude);
                    writer.WriteMember("Long");
                    writer.WriteNumber(leg.Destination.Longitude);
                    writer.WriteEndObject();
                }

                writer.WriteMember("Mode");
                writer.WriteString(leg.TransportMode.ToString());
                writer.WriteMember("TotalTime");
                context.Export(leg.TotalTime, writer);
                writer.WriteMember("DepartTime");
                context.Export(leg.DepartureTime, writer); 

                writer.WriteEndObject();
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        #endregion
    }
}