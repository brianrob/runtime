// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/*=============================================================================
**
** Class: ReadOnlyCollectionBase
**
** Purpose: Provides the abstract base class for a
**          strongly typed non-generic read-only collection.
**
=============================================================================*/


namespace System.Collections
{
    // Useful base class for typed readonly collections where items derive from object
    public abstract class ReadOnlyCollectionBase : ICollection
    {
        protected ArrayList InnerList => field ??= new ArrayList();

        public virtual int Count
        {
            get { return InnerList.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return InnerList.IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return InnerList.SyncRoot; }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            InnerList.CopyTo(array, index);
        }

        public virtual IEnumerator GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }
    }
}
