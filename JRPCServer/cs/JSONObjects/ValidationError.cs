// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationError.cs" company="">
//   
// </copyright>
// <summary>
//   The validation error.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    /// <summary>
    /// The validation error.
    /// </summary>
    public class ValidationError
    {
        #region Constants and Fields

        /// <summary>
        ///   The error message.
        /// </summary>
        public string Message;

        /// <summary>
        ///   The target parameter the error occured on.
        /// </summary>
        public string Target;

        #endregion
    }
}