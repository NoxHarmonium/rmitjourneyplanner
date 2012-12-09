using System;
using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary;
using Jayrock.Json.Conversion;

namespace JRPCServer
{
	public class Journey : ICloneable
	{
		/// <summary>
		/// The UUID of this journey.
		/// </summary>
		private string uuid;
		
		/// <summary>
		/// The properties of this journey.
		/// </summary>
		private EvolutionaryProperties properties;
		
		/// <summary>
		/// The short name for this journey.
		/// </summary>
		private string shortName;
		
		/// <summary>
		/// The description of this journey;
		/// </summary>
		private string description;
		
		/// <summary>
		/// The UUIDs of associated runs.
		/// </summary>
		private string[] runUuids = new string[0];
		
		/// <summary>
		/// Gets or sets the unique identifier of the journey.
		/// </summary>
		/// <value>
		/// The UUID of the journey.
		/// </value>
		public string Uuid
		{
			get
			{
				return uuid;	
			}
			set
			{
				uuid = value;	
			}
		}
		
		/// <summary>
		/// Gets or sets the properties of this journey.
		/// </summary>
		/// <value>
		/// The property object associated with this journey.
		/// </value>
		public EvolutionaryProperties Properties
		{
			get
			{
				return properties;	
			}	
			set
			{
				properties = value;	
			}
		}
		
		/// <summary>
		/// Gets or sets the short name of this journey. Used to identify the journey.
		/// </summary>
		/// <value>
		/// The short name of this journey.
		/// </value>
		public string ShortName
		{
			get
			{
				return shortName;
			}
			set
			{
				shortName = value;	
			}
			
		}
		
		/// <summary>
		/// Gets or sets the description of the journey.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		public string Description
		{
			get
			{
				return description;	
			}
			set
			{
				description = value;	
			}
			
		}
		
		
		/// <summary>
		/// Gets the run UUIDs associated with this journey.
		/// </summary>
		/// <value>
		/// The run UUIDs assocated with this journey.
		/// </value>
		public string[] RunUuids {
			get {
				return this.runUuids;
			}
			set
			{
				this.runUuids = value;	
			}
		}
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Journey"/> class.
		/// </summary>
		public Journey ()
		{
			uuid = Guid.NewGuid().ToString();
			shortName = String.Empty;
			description = String.Empty;
			properties = new EvolutionaryProperties();
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Journey"/> class.
		/// </summary>
		/// <param name='uuid'>
		/// The UUID of the journey.
		/// </param>
		/// <param name='shortName'>
		/// The short name of the journey.
		/// </param>
		/// <param name='description'>
		/// The description of the journey.
		/// </param>
		/// <param name='properties'>
		/// The evolutionary properties associated with the journey.
		/// </param>
		public Journey (string uuid, EvolutionaryProperties properties, string shortName, string description)
		{
			this.uuid = uuid;
			this.properties = properties;
			this.shortName = shortName;
			this.description = description;
		}

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns></returns>
	    public object Clone()
        {
            return new Journey
                {
                    ShortName = this.shortName + " (Clone)",
                    description = this.description,
                    properties = (EvolutionaryProperties)this.properties.Clone()
                };
        }
	}
}

