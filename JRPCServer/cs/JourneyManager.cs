// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JourneyManager.cs" company="">
//   
// </copyright>
// <summary>
//   The journey manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    #region Using Directives

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Jayrock.Json;
    using Jayrock.Json.Conversion;

    #endregion

    /// <summary>
    /// The journey manager.
    /// </summary>
    public class JourneyManager
    {
        #region Constants and Fields

        /// <summary>
        ///   The list of directories used by the journey manager.
        /// </summary>
        private readonly string[] directories = { "JSONStore", "JSONStore/Journeys" };

        /// <summary>
        ///   The journey map that holds all the journeys.
        /// </summary>
        private readonly Dictionary<string, Journey> journeyMap = new Dictionary<string, Journey>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "JourneyManager" /> class.
        /// </summary>
        public JourneyManager()
        {
            this.CreateDirectoryStructure();
            this.LoadJourneys();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Add the specified journey to the manager.
        /// </summary>
        /// <param name="journey">
        /// The journey that is to be added to the manager.
        /// </param>
        public void Add(Journey journey)
        {
            this.journeyMap.Add(journey.Uuid, journey);
        }

        /// <summary>
        /// Deletes all saved journeys and associated runs. Also clears the internal dictionary.
        /// </summary>
        public void Clean()
        {
            this.journeyMap.Clear();
            new DirectoryInfo(this.directories[1]).Delete(true);
            this.CreateDirectoryStructure();
        }

        /// <summary>
        /// Deletes a journey from the journey manager.
        /// </summary>
        /// <param name="uuid">
        /// The UUID of the journey you want to delete.
        /// </param>
        public void DeleteJourney(string uuid)
        {
            this.journeyMap.Remove(uuid);
        }

        /// <summary>
        /// Returns a raw JSON snapshot of the specified journey ID, run ID and iteration.
        /// </summary>
        /// <param name="journeyUuid">
        /// </param>
        /// <param name="runUuid">
        /// </param>
        /// <param name="iteration">
        /// </param>
        /// <returns>
        /// The get iteration snap shot.
        /// </returns>
        public string GetIterationSnapShot(string journeyUuid, string runUuid, int iteration)
        {
            var path = this.directories[1] + "/" + journeyUuid + "/" + runUuid + "/" + "iteration." + iteration
                       + ".json";
            string data;
            using (StreamReader reader = new StreamReader(path))
            {
                data = reader.ReadToEnd();
            }

            return data;
        }

        /// <summary>
        /// The get journey.
        /// </summary>
        /// <param name="uuid">
        /// The uuid.
        /// </param>
        /// <returns>
        /// </returns>
        public Journey GetJourney(string uuid)
        {
            return this.journeyMap[uuid];
        }

        /// <summary>
        /// The get journeys.
        /// </summary>
        /// <returns>
        /// </returns>
        public Journey[] GetJourneys()
        {
            return this.journeyMap.Values.ToArray();
        }

        /// <summary>
        /// Saves the current journeys to disk overwriting all existing files.
        /// </summary>
        public void Save()
        {
            var context = JsonConvert.CreateExportContext();
            context.Register(new PropertiesExporter());

            foreach (var kvp in this.journeyMap)
            {
                var path = string.Format("{0}/{1}.json", this.directories[1], kvp.Value.Uuid);
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    context.Export(kvp.Value, new JsonTextWriter(sw));
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the directory structure needed by the journey manager.
        /// </summary>
        private void CreateDirectoryStructure()
        {
            foreach (var d in this.directories)
            {
                this.CreateIfNotExist(d);
            }
        }

        /// <summary>
        /// Creates the specified directory if it doesn't already exist.
        /// </summary>
        /// <param name="directoryPath">
        /// The path of the directory.
        /// </param>
        private void CreateIfNotExist(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// The load journeys.
        /// </summary>
        private void LoadJourneys()
        {
            var context = JsonConvert.CreateImportContext();

            context.Register(new PropertiesImporter());

            foreach (var f in new DirectoryInfo(this.directories[1]).GetFiles())
            {
                using (var reader = new StreamReader(f.FullName))
                {
                    var j = (Journey)context.Import(typeof(Journey), JsonText.CreateReader(reader));

                    // Journey j = ObjectTools.getObject<Journey>(o);			
                    this.journeyMap.Add(j.Uuid, j);
                }
            }
        }

        #endregion
    }
}