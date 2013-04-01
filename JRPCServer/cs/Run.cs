// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Run.cs" company="">
//   
// </copyright>
// <summary>
//   The run.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    #region Using Directives

    using System;

    #endregion

    /// <summary>
    /// The run.
    /// </summary>
    public class Run
    {
        #region Constants and Fields

        /// <summary>
        ///   The UUID of this run.
        /// </summary>
        private readonly string uuid;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Run"/> class.
        /// </summary>
        /// <param name="uuid">
        /// The UUID of the run.
        /// </param>
        /// <param name="journeyUuid">
        /// The UUID of the journey this run is associated with.
        /// </param>
        /// <param name="timeStarted">
        /// The time the run was started.
        /// </param>
        /// <param name="timeFinished">
        /// The time the run was finished.
        /// </param>
        /// <param name="errorCode">
        /// The error code of the run.
        /// </param>
        public Run(string uuid, string journeyUuid, DateTime timeStarted, DateTime timeFinished, int errorCode)
        {
            this.uuid = uuid;
            this.JourneyUuid = journeyUuid;
            this.TimeStarted = timeStarted;
            this.TimeFinished = timeFinished;
            this.ErrorCode = errorCode;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Run" /> class.
        /// </summary>
        public Run()
        {
            this.uuid = Guid.NewGuid().ToString();
            this.JourneyUuid = string.Empty;
            this.TimeStarted = default(DateTime);
            this.TimeFinished = default(DateTime);
            this.ErrorCode = -1;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the error code of this run.
        /// </summary>
        /// <value>
        ///   The error code of this run.
        /// </value>
        public int ErrorCode { get; set; }

        /// <summary>
        ///   Gets or sets the UUID of the associated journey.
        /// </summary>
        public string JourneyUuid { get; set; }

        /// <summary>
        ///   Gets or sets the time the run finished.
        /// </summary>
        /// <value>
        ///   The time the run finished.
        /// </value>
        public DateTime TimeFinished { get; set; }

        /// <summary>
        ///   Gets or sets the time the run started.
        /// </summary>
        /// <value>
        ///   The time the run started.
        /// </value>
        public DateTime TimeStarted { get; set; }

        /// <summary>
        ///   Gets the UUID of this run.
        /// </summary>
        /// <value>
        ///   The UUID of this run.
        /// </value>
        public string Uuid
        {
            get
            {
                return this.uuid;
            }
        }

        #endregion
    }
}