// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClosedRoute.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   A struct which is used to define a possible way to traverse part
//   of a journey defined by a <see cref="Route" /> object. The <see cref="AlFitnessFunction" />
//   class uses it to preprocess a journey into possible services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region Using Directives

    using RmitJourneyPlanner.CoreLibraries.RoutePlanners.Evolutionary.FitnessFunctions;

    #endregion

    /// <summary>
    /// A struct which is used to define a possible way to traverse part
    ///   of a journey defined by a <see cref="Route"/> object. The <see cref="AlFitnessFunction"/>
    ///   class uses it to preprocess a journey into possible services.
    /// </summary>
    public struct ClosedRoute
    {
        #region Constants and Fields

        /// <summary>
        ///   The end index.
        /// </summary>
        private int end;

        /// <summary>
        ///   The route identifier.
        /// </summary>
        private int id;

        /// <summary>
        ///   The start index.
        /// </summary>
        private int start;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedRoute"/> struct with the specified parameters.
        /// </summary>
        /// <param name="id">
        /// The route identifier.
        /// </param>
        /// <param name="start">
        /// The start index.
        /// </param>
        /// <param name="end">
        /// The end index.
        /// </param>
        public ClosedRoute(int id, int start, int end)
        {
            this.id = id;
            this.start = start;
            this.end = end;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the end index of the closed route.
        /// </summary>
        public int End
        {
            get
            {
                return this.end;
            }

            set
            {
                this.end = value;
            }
        }

        /// <summary>
        ///   Gets or sets the route identifier of the closed route.
        /// </summary>
        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        /// <summary>
        ///   Gets the length of the closed route.
        /// </summary>
        public int Length
        {
            get
            {
                return this.end - this.start;
            }
        }

        /// <summary>
        ///   Gets or sets the start index of the closed route.
        /// </summary>
        public int Start
        {
            get
            {
                return this.start;
            }

            set
            {
                this.start = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("Start: {0}, End: {1}, Id: {2}", this.start, this.end, this.id);
        }

        #endregion
    }
}