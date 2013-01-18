// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Journey.cs" company="">
//   
// </copyright>
// <summary>
//   The journey.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    #region Using Directives

    using System;

    using RmitJourneyPlanner.CoreLibraries.JourneyPlanners.Evolutionary;

    #endregion

    /// <summary>
    /// The journey.
    /// </summary>
    public class Journey : ICloneable
    {
        #region Constants and Fields

        /// <summary>
        ///   The description of this journey;
        /// </summary>
        private string description;

        /// <summary>
        ///   The properties of this journey.
        /// </summary>
        private EvolutionaryProperties properties;

        /// <summary>
        ///   The UUIDs of associated runs.
        /// </summary>
        private string[] runUuids = new string[0];

        /// <summary>
        ///   The short name for this journey.
        /// </summary>
        private string shortName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Journey" /> class.
        /// </summary>
        public Journey()
        {
            this.Uuid = Guid.NewGuid().ToString();
            this.shortName = string.Empty;
            this.description = string.Empty;
            this.properties = new EvolutionaryProperties();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Journey"/> class.
        /// </summary>
        /// <param name="uuid">
        /// The UUID of the journey.
        /// </param>
        /// <param name="properties">
        /// The evolutionary properties associated with the journey.
        /// </param>
        /// <param name="shortName">
        /// The short name of the journey.
        /// </param>
        /// <param name="description">
        /// The description of the journey.
        /// </param>
        public Journey(string uuid, EvolutionaryProperties properties, string shortName, string description)
        {
            this.Uuid = uuid;
            this.properties = properties;
            this.shortName = shortName;
            this.description = description;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the description of the journey.
        /// </summary>
        /// <value>
        ///   The description.
        /// </value>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;
            }
        }

        /// <summary>
        ///   Gets or sets the properties of this journey.
        /// </summary>
        /// <value>
        ///   The property object associated with this journey.
        /// </value>
        public EvolutionaryProperties Properties
        {
            get
            {
                return this.properties;
            }

            set
            {
                this.properties = value;
            }
        }

        /// <summary>
        ///   Gets the run UUIDs associated with this journey.
        /// </summary>
        /// <value>
        ///   The run UUIDs assocated with this journey.
        /// </value>
        /// //TODO: Why did I ever make this an array rather than a list or something?
        public string[] RunUuids
        {
            get
            {
                return this.runUuids;
            }

            set
            {
                this.runUuids = value;
            }
        }

        /// <summary>
        ///   Gets or sets the short name of this journey. Used to identify the journey.
        /// </summary>
        /// <value>
        ///   The short name of this journey.
        /// </value>
        public string ShortName
        {
            get
            {
                return this.shortName;
            }

            set
            {
                this.shortName = value;
            }
        }

        /// <summary>
        ///   Gets or sets the unique identifier of the journey.
        /// </summary>
        /// <value>
        ///   The UUID of the journey.
        /// </value>
        public string Uuid { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public object Clone()
        {
            return new Journey
                {
                    ShortName = this.shortName + " (Clone)", 
                    description = this.description, 
                    properties = (EvolutionaryProperties)this.properties.Clone(),
                    runUuids = this.runUuids
                };
        }

        #endregion
    }
}