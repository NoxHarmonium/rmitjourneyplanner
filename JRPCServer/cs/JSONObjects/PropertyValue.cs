// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyValue.cs" company="">
//   
// </copyright>
// <summary>
//   The property value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JRPCServer
{
    /// <summary>
    /// The property value.
    /// </summary>
    public class PropertyValue
    {
        #region Public Properties

        /// <summary>
        ///   The name of the property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///   The new value of the property.
        /// </summary>
        public string Value { get; set; }

        #endregion
    }
}