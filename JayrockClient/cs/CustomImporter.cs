using System;
using  RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
namespace JayrockClient
{
	public class CustomImporter<T> : Jayrock.Json.Conversion.IImporter
	{
		private EvolutionaryProperties properties;
		
		public CustomImporter(EvolutionaryProperties properties)
		{
			this.properties = properties;
		}
		
		public object Import (Jayrock.Json.Conversion.ImportContext context, Jayrock.Json.JsonReader reader)
		{
			Jayrock.Json.JsonToken token = reader.Token;
			string type = token.Text;
			Type ttype = typeof(T);
			Type t = ttype.Assembly.GetType(type);			
			return Activator.CreateInstance(t, new object[]{properties});
		}

		public Type OutputType {
			get {
				return typeof(T);
			}
		}
	}
}

