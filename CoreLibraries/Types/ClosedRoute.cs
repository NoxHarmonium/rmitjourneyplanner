namespace RmitJourneyPlanner.CoreLibraries.Types
{
    public struct ClosedRoute
    {
        public int start;

        public int end;

        public int id;

        public int Length
        {
            get
            {
                return this.end - this.start;
            }

        }

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
    }
}