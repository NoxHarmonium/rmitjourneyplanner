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
        #region Public Properties

        /// <summary>
        /// Gets InputType.
        /// </summary>
        public Type InputType
        {
            get
            {
                return typeof(Critter);
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
            writer.WriteStartObject();
            writer.WriteMember("Critter");
            writer.WriteStartObject();
            var critter = (Critter)value;
            writer.WriteMember("Age");
            writer.WriteNumber(critter.Age);
            writer.WriteMember("N");
            writer.WriteNumber(critter.N);
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
            writer.WriteMember("Route");
            writer.WriteStartArray();
            foreach (var node in critter.Route)
            {
                writer.WriteNumber(node.Node.Id);
            }

            writer.WriteEndArray();

            writer.WriteMember("Legs");
            writer.WriteStartArray();
            foreach (var leg in critter.Fitness.JourneyLegs)
            {
                writer.WriteStartObject();
                writer.WriteMember("Start");
                writer.WriteNumber(leg.Origin.Id);
                writer.WriteMember("End");
                writer.WriteNumber(leg.Destination.Id);
                writer.WriteMember("Mode");
                writer.WriteString(leg.TransportMode.ToString());
                writer.WriteMember("TotalTime");
                context.Export(leg.TotalTime, writer);

                // writer.WriteString(leg.TotalTime.ToString());
                writer.WriteMember("Route");
                writer.WriteString(leg.RouteId1);
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