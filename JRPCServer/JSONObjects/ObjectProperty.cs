using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JayrockClient
{
    
    public struct ObjectProperty
    {
        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Name;

        /// <summary>
        /// The type of the property.
        /// </summary>
        public string Type;

        /// <summary>
        /// The value of the property.
        /// </summary>
        public string Value;
		
		/// <summary>
		/// Specified if the property is editable as a value type or not.
		/// </summary>
		public bool Editable;
    }
}