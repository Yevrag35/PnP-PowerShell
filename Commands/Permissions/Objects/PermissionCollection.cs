using Microsoft.SharePoint.Client.Utilities;
using SharePointPnP.PowerShell.Commands.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Provides an <see cref="ICollection"/> class for holding <see cref="Permission"/> objects.
    /// </summary>
    public class PermissionCollection : ICollection<Permission>, ICollection
    {
        #region FIELDS/CONSTANTS

        /// <summary>
        /// The internal, backing <see cref="List{T}"/> collection that all methods invoke against.
        /// </summary>
        protected List<Permission> InnerList;

        #endregion

        #region INDEXERS
        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-bsaed index of the element to get.</param>
        public Permission this[int index] => this.InnerList[index];

        public Permission this[string principal] => this.InnerList
            .Find(x => 
                x.LoginName.Equals(principal) ||
                x.MemberName.Equals(principal));

        #endregion

        #region PROPERTIES
        /// <summary>
        /// The current SharePoint context backing this object.
        /// </summary>
        public ClientRuntimeContext Context { get; set; }

        /// <summary>
        /// Get the number of elements contained within the <see cref="PermissionCollection"/>.
        /// </summary>
        public int Count => this.InnerList.Count;

        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionCollection"/> class that is empty
        /// and has the default initial capacity.
        /// </summary>
        public PermissionCollection()
        {
            this.InnerList = new List<Permission>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionCollection"/> class that is empty
        /// and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new collection can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public PermissionCollection(int capacity)
        {
            this.InnerList = new List<Permission>(capacity);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionCollection"/> class that
        /// contains elements copied from the specified <see cref="IEnumerable{T}"/> and has
        /// sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="items">The collection whose elements are copied to the new list.</param>
        /// <exception cref="ArgumentNullException"/>
        public PermissionCollection(IEnumerable<Permission> items)
        {
            this.InnerList = new List<Permission>(items);
        }

        public PermissionCollection(RoleAssignmentCollection roleAssCol)
            : this(roleAssCol.Count)
        {
            this.Context = roleAssCol.Context;
            for (int i = 0; i < roleAssCol.Count; i++)
            {
                RoleAssignment ass = roleAssCol[i];
                Permission perm = new Permission(ass);
                this.InnerList.Add(perm);
            }
        }

        #endregion

        #region BASE METHODS
        /// <summary>
        /// Adds an item to the end of the collection.
        /// </summary>
        /// <param name="item">The object to be added to the end of the collection.</param>
        public void Add(Permission item) => this.InnerList.Add(item);
        /// <summary>
        /// Removes all elements from the <see cref="PermissionCollection"/>.
        /// </summary>
        public void Clear() => this.InnerList.Clear();
        /// <summary>
        /// Determines whether an element is in the <see cref="PermissionCollection"/>.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="PermissionCollection"/>.  The value can be null for reference types.
        /// </param>
        public bool Contains(Permission item) => this.InnerList.Contains(item);

        public bool Contains(Predicate<Permission> match) => this.InnerList.Exists(match);

        public bool ContainsPrincipal(string principal) => this.InnerList
            .Exists(x =>
                x.LoginName.Equals(principal) ||
                x.MemberName.Equals(principal));

        /// <summary>
        /// Copies the entire <see cref="PermissionCollection"/> to a compatible one-dimensional array, starting at
        /// the specified index of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from
        /// <see cref="PermissionCollection"/>.  The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in the target array at which copying begins.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentException"/>
        public void CopyTo(Permission[] array, int arrayIndex) => this.InnerList.CopyTo(array, arrayIndex);
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence
        /// within the entire <see cref="PermissionCollection"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="PermissionCollection"/>.  The value can be null for reference types.</param>
        public int IndexOf(Permission item) => this.InnerList.IndexOf(item);
        /// <summary>
        /// Sorts the elements in the entire <see cref="PermissionCollection"/> using the default comparer.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public virtual void Sort() => this.InnerList.Sort();
        /// <summary>
        /// Sorts the elements in the entire <see cref="PermissionCollection"/> using the specified comparer.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> implementation to use when comparing elements.
        /// </param>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
		public void Sort(IComparer<Permission> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            this.InnerList.Sort(comparer);
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="PermissionCollection"/>.  The
        /// value can be null for reference types.
        /// </summary>
        /// <param name="item">
        /// The object to remove from the <see cref="PermissionCollection"/>.
        /// The value can be null for reference types.
        /// </param>
        public bool Remove(Permission item) => this.InnerList.Remove(item);

        #endregion

        #region ENUMERATOR
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="PermissionCollection"/>.
        /// </summary>
        public IEnumerator<Permission> GetEnumerator() => this.InnerList.GetEnumerator();
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="IEnumerable"/>.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => this.InnerList.GetEnumerator();

        #endregion

        #region STATIC METHODS
        public static PermissionCollection ResolvePermissions(SecurableObject securableObject)
        {
            RoleAssignmentCollection roleAssCol = securableObject.RoleAssignments;
            if (!roleAssCol.AreItemsAvailable)
            {
                roleAssCol.LoadAllAssignments();
            }

            var permCol = new PermissionCollection(roleAssCol);
            permCol.AddSecuringObject(securableObject);
            return permCol;
        }

        #endregion

        #region INTERFACE MEMBERS

        #region IMPLEMENTED INTERFACE PROPERTIES
        bool ICollection<Permission>.IsReadOnly => ((ICollection<Permission>)this.InnerList).IsReadOnly;
        bool ICollection.IsSynchronized => ((ICollection)this.InnerList).IsSynchronized;
        object ICollection.SyncRoot => ((ICollection)this.InnerList).SyncRoot;

        #endregion

        #region IMPLEMENTED INTERFACE METHODS
        void ICollection.CopyTo(Array array, int index) => ((ICollection)this.InnerList).CopyTo(array, index);

        #endregion

        #endregion

        #region BACKEND/PRIVATE METHODS
        private void AddSecuringObject(SecurableObject secObj)
        {
            if (this.InnerList.Count > 0)
            {
                this.InnerList.ForEach((a) =>
                {
                    a.SecuringObject = secObj;
                });
            }
        }

        #endregion
    }
}