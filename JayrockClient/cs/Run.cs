using System;

namespace JayrockClient
{
	public class Run
	{
		/// <summary>
		/// The UUID of this run.
		/// </summary>
		private string uuid;
		
		/// <summary>
		/// The UUID of the journey this run is associated with.
		/// </summary>
		private string journeyUuid;
		
		/// <summary>
		/// The time the run started.
		/// </summary>
		private DateTime timeStarted;
		
		/// <summary>
		/// The time the run finished.
		/// </summary>
		private DateTime timeFinished;
		
		/// <summary>
		/// The error code of the run.
		/// </summary>
		private int errorCode;
		
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="JayrockClient.Run"/> class.
		/// </summary>
		/// <param name='uuid'>
		/// The UUID of the run.
		/// </param>
		/// <param name='journeyUuid'>
		/// The UUID of the journey this run is associated with.
		/// </param>
		/// <param name='timeStarted'>
		/// The time the run was started.
		/// </param>
		/// <param name='timeFinished'>
		/// The time the run was finished.
		/// </param>
		/// <param name='errorCode'>
		/// The error code of the run.
		/// </param>
		public Run (string uuid, string journeyUuid, DateTime timeStarted, DateTime timeFinished, int errorCode)
		{
			this.uuid = uuid;
			this.journeyUuid = journeyUuid;
			this.timeStarted = timeStarted;
			this.timeFinished = timeFinished;
			this.errorCode = errorCode;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JayrockClient.Run"/> class.
		/// </summary>
		public Run ()
		{
			this.uuid = Guid.NewGuid().ToString();
			this.journeyUuid = string.Empty;
			this.timeStarted = default(DateTime);
			this.timeFinished = default(DateTime);
			this.errorCode = -1;			
		}
		
		/// <summary>
		/// Gets the UUID of this run.
		/// </summary>
		/// <value>
		/// The UUID of this run.
		/// </value>
		public string Uuid {
			get {
				return this.uuid;
			}
		}
		
		/// <summary>
		/// Gets the UUID of the associated journey.
		/// </summary>
		/// <value>
		/// The UUID of the associated journey.
		/// </value>
		public string JourneyUuid {
			get {
				return this.journeyUuid;
			}
		}
		/// <summary>
		/// Gets or sets the time the run started.
		/// </summary>
		/// <value>
		/// The time the run started.
		/// </value>
		public DateTime TimeStarted {
			get {
				return this.timeStarted;
			}
			set {
				timeStarted = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the time the run finished.
		/// </summary>
		/// <value>
		/// The time the run finished.
		/// </value>
		public DateTime TimeFinished {
			get {
				return this.timeFinished;
			}
			set {
				timeFinished = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the error code of this run.
		/// </summary>
		/// <value>
		/// The error code of this run.
		/// </value>
		public int ErrorCode {
			get {
				return this.errorCode;
			}
			set {
				errorCode = value;
			}
		}
	}
}

