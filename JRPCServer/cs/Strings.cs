// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Strings.cs" company="">
//   
// </copyright>
// <summary>
//   The strings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    /// <summary>
    /// The strings.
    /// </summary>
    public static class Strings
    {
        #region Constants and Fields

        /// <summary>
        /// The er r_ an y_ null.
        /// </summary>
        public const string ERR_ANY_NULL = "There can be no null parameters passed to this function.";

        /// <summary>
        /// The er r_ invali d_ cast.
        /// </summary>
        public const string ERR_INVALID_CAST = "The property value you supplied cannot be cast to the property type.";

        /// <summary>
        /// The er r_ invali d_ runid.
        /// </summary>
        public const string ERR_INVALID_RUNID = "The specified run uuid is invalid for this journey.";

        /// <summary>
        /// The er r_ j m_ notfound.
        /// </summary>
        public const string ERR_JM_NOTFOUND = "There is no JourneyManager object registered with the ObjectCache.";

        /// <summary>
        /// The er r_ j m_ null.
        /// </summary>
        public const string ERR_JM_NULL =
            "The JourneyManager parameter can not be null. It is required for this class to operate.";

        /// <summary>
        /// The er r_ j_ null.
        /// </summary>
        public const string ERR_J_NULL = "The journey UUID parameter cannot be null.";

        /// <summary>
        /// The er r_ pro p_ no t_ found.
        /// </summary>
        public const string ERR_PROP_NOT_FOUND = "The property you are trying to modify can not be found.";

        /// <summary>
        /// The er r_ unsup p_ reftype.
        /// </summary>
        public const string ERR_UNSUPP_REFTYPE = "The property is a reference type that the parser cannot handle.";

        /// <summary>
        /// The validatio n_ success.
        /// </summary>
        public const string VALIDATION_SUCCESS = "Success";

        #endregion
    }
}