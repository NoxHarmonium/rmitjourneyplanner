using System;
using System.IO;
using System.Collections.Generic;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary;
using RmitJourneyPlanner.CoreLibraries.DataProviders.Metlink;
using System.Linq;

namespace JayrockClient	
{
	public class JourneyManager
	{
		
		/// <summary>
		/// The journey map that holds all the journeys.
		/// </summary>
		private readonly Dictionary<string,Journey> journeyMap = new Dictionary<string, Journey>(); 
		
		
		/// <summary>
		/// The list of directories used by the journey manager.
		/// </summary>
		private readonly string[] directories = 
		{
			"JSONStore",	
			"JSONStore/Journeys"			
		};
		
		/// <summary>
		/// Initializes a new instance of the <see cref="JayrockClient.JourneyManager"/> class.
		/// </summary>
		public JourneyManager ()
		{
				
			CreateDirectoryStructure();
			LoadJourneys();
			
		}
		
		
		public Journey[] GetJourneys()
		{
			return journeyMap.Values.ToArray();	
		}
		
		public Journey GetJourney(string uuid)
		{
			return journeyMap[uuid];	
		}
		
		private void LoadJourneys()
		{
			
			var context = JsonConvert.CreateImportContext();

			context.Register(new PropertiesImporter());
			
			
			foreach (var f in new DirectoryInfo(directories[1]).GetFiles())
			{
				using (var reader = new StreamReader(f.FullName))
				{
										

					var j =  (Journey) context.Import(typeof(Journey), JsonText.CreateReader(reader));
					
					
					//Journey j = ObjectTools.getObject<Journey>(o);			
					journeyMap.Add(j.Uuid,j);					
										
				}
				
			}
		}
			
		/// <summary>
		/// Deletes a journey from the journey manager.
		/// </summary>
		/// <param name='uuid'>
		/// The UUID of the journey you want to delete.
		/// </param>
		public void DeleteJourney(string uuid)
		{
			journeyMap.Remove(uuid);	
		}
		
		
		/// <summary>
		/// Creates the directory structure needed by the journey manager.
		/// </summary>
		private void CreateDirectoryStructure()
		{
			foreach (var d in directories)
			{
				CreateIfNotExist(d);	
			}
		}
		
		/// <summary>
		/// Creates the specified directory if it doesn't already exist.
		/// </summary>
		/// <param name='directoryPath'>
		/// The path of the directory.
		/// </param>
		private void CreateIfNotExist(string directoryPath)
		{
			if(!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);				
			}
			
		}
		
        /// <summary>
        /// Deletes all saved journeys and associated runs. Also clears the internal dictionary.
        /// </summary>
        public void Clean()
        {
            this.journeyMap.Clear();
            new DirectoryInfo(directories[1]).Delete(true);
            this.CreateDirectoryStructure();

        }


		/// <summary>
		/// Saves the current journeys to disk overwriting all existing files.
		/// </summary>
		public void Save()
		{
			var context = JsonConvert.CreateExportContext();
			context.Register(new PropertiesExporter());
			
			foreach (var kvp in journeyMap)
			{
				var path = string.Format("{0}/{1}.json",directories[1],kvp.Value.Uuid);
				using (StreamWriter sw = new StreamWriter(path,false))
				{
					context.Export(kvp.Value,new JsonTextWriter(sw));
				}
			}
			
		}
		
		/// <summary>
		/// Add the specified journey to the manager.
		/// </summary>
		/// <param name='journey'>
		/// The journey that is to be added to the manager.
		/// </param>
		public void Add(Journey journey)
		{
			this.journeyMap.Add(journey.Uuid, journey);	
		}
	}
}

