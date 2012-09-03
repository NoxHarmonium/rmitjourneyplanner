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
    using RmitJourneyPlanner.CoreLibraries.Types;

    public class CritterExporter : IExporter
    {
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
            foreach (var n in Enum.GetNames(typeof(FitnessParameters)))
            {
                
                writer.WriteMember(n);
                writer.WriteNumber(critter.Fitness[i++]);

            }
            writer.WriteEndObject();
             
            //writer.WriteMember("Fitness");
//context.Export(critter.Fitness,writer);
            

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
                writer.WriteString(leg.TotalTime.ToString());
                writer.WriteMember("Route");
                writer.WriteString(leg.RouteId1);

                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
            writer.WriteEndObject();

            
            
            
            


            
        }

        public Type InputType
        {
            get
            {
                return typeof(Critter);
            }
        }
    }
}

