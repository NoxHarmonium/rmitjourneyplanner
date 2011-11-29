// -----------------------------------------------------------------------
// <copyright file="TreeWrapper.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Wraps a type to be used in tree traversal.
    /// </summary>
    public class TreeWrapper <T>
    {
        private TreeWrapper<T> parent;
        private T obj;

        public TreeWrapper(T obj, TreeWrapper<T> parent)
        {
            this.parent = parent;
            this.obj = obj;
        }

        /// <summary>
        /// Gets the wrapped object
        /// </summary>
        public T Obj
        {
            get
            {
                return obj;
            }
        }

        /// <summary>
        /// Gets the parent of this wrapper.
        /// </summary>
        public TreeWrapper<T> Parent
        {
            get
            {
                return parent;
            }
        }



    }
}
