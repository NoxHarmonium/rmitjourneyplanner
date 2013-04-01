// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectProperty.cs" company="">
//   
// </copyright>
// <summary>
//   The object property.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    /// <summary>
    /// The object property.
    /// </summary>
    public struct ObjectProperty
    {
        #region Constants and Fields

        /// <summary>
        ///   Specified if the property is editable as a value type or not.
        /// </summary>
        public bool Editable;

        /// <summary>
        ///   The name of the property.
        /// </summary>
        public string Name;

        /// <summary>
        ///   The type of the property.
        /// </summary>
        public string Type;

        /// <summary>
        ///   The value of the property.
        /// </summary>
        public string Value;

        #endregion
    }
}