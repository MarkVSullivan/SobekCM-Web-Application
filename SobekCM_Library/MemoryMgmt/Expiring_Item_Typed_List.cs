#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace SobekCM.Library.MemoryMgmt
{
	/// <summary> Generic collection keeps a list of objects which expire after a given timespan and are
	/// then removed from the list and also retains the type of object </summary>
	/// <typeparam name="T"> Type for this generic class </typeparam>
	/// <remarks>This is used to maintain a list of recently added items to the remote caching server</remarks>
	public class Expiring_Item_Typed_List<T> : IEnumerable<Expiring_Typed_Item<T>>
	{
		private readonly TimeSpan expiration;
		private readonly List<Expiring_Typed_Item<T>> items;

		/// <summary> Constructor for a new instance of the generic Expiring_Item_Typed_List generic collection </summary>
		public Expiring_Item_Typed_List()
		{
			items = new List<Expiring_Typed_Item<T>>();
			expiration = new TimeSpan(0, 15, 0);
		}

		/// <summary> Constructor for a new instance of the generic Expiring_Item_Typed_List generic collection </summary>
		/// <param name="Expiration"> Timespan after which an item may be removed from this collection </param>
		public Expiring_Item_Typed_List(TimeSpan Expiration)
		{
			items = new List<Expiring_Typed_Item<T>>();
			expiration = Expiration;
		}

		/// <summary> Constructor for a new instance of the generic Expiring_Item_Typed_List generic collection </summary>
		/// <param name="Expiration"> Timespan (in minutes) after which an item may be removed from this collection </param>
		public Expiring_Item_Typed_List(int Expiration)
		{
			items = new List<Expiring_Typed_Item<T>>();
			expiration = new TimeSpan(0, Expiration, 0);
		}

		/// <summary> Gets the number of items in this collection </summary>
		public int Count
		{
			get
			{
				remove_expired_objects();
				return items.Count;
			}
		}

		/// <summary> Gets the index based item from the item list </summary>
		/// <param name="index">Index for the item to retrieve from this collection</param>
		/// <returns>Expiring typed item </returns>
		public Expiring_Typed_Item<T> this[int index]
		{
			get
			{
				return index < items.Count ? items[index] : null;
			}
		}

		#region IEnumerable<Expiring_Typed_Item<T>> Members

		/// <summary> Get the enumerator for stepping through all members of this collection </summary>
		/// <returns> Generic IEnumerator class </returns>
		public IEnumerator<Expiring_Typed_Item<T>> GetEnumerator()
		{
			return new Expiring_Item_Typed_List_Enumerator<T>(this);
		}

		/// <summary> Get the enumerator for stepping through all members of this collection </summary>
		/// <returns> IEnumerator class </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Expiring_Item_Typed_List_Enumerator<T>(this);
		}

		#endregion

		/// <summary> Add a new item to this queue </summary>
		/// <param name="Item_To_Add"> Item to add to this queue </param>
		/// <param name="Data_Type"> Data type of the object this key points to</param>
		public void Add(T Item_To_Add, Type Data_Type)
		{
			// Always check for expiration first
			remove_expired_objects();

			// Adding again starts the clock again, so try to remove it
			Remove(Item_To_Add);

			// Add this data as an expiring item
			items.Add(new Expiring_Typed_Item<T>(Item_To_Add, Data_Type));
		}

		/// <summary> Check to see if this item exists  </summary>
		/// <param name="Item_To_Check"> Item to check </param>
		/// <returns>TRUE if contained, otherwise FALSE</returns>
		public bool Contains(T Item_To_Check)
		{
			// Always check for expiration first
			remove_expired_objects();
			bool returnValue = false;
			lock (items)
			{
				// Just step through each item
				if (items.Any(existingItem => (existingItem != null) && (existingItem.Equals(Item_To_Check))))
				{
					returnValue = true;
				}
			}
			return returnValue;
		}

		/// <summary> Remove an object from this expiring list, if it exists </summary>
		/// <param name="Item_To_Remove"> Item to remove </param>
		public void Remove(T Item_To_Remove)
		{
			// Always check for expiration first
			remove_expired_objects();

			// Just step through each item
			Expiring_Typed_Item<T> item_found = items.FirstOrDefault(existingItem => existingItem.Equals(Item_To_Remove));
			if (item_found != null)
				items.Remove(item_found);
		}

		/// <summary> Refreshes the list and purges items which are expired </summary>
		public void Refresh()
		{
			remove_expired_objects();
		}

		/// <summary> Clear this collection of all items </summary>
		public void Clear()
		{
			items.Clear();
		}


		/// <summary> Removes all expired objects from this collection </summary>
		private void remove_expired_objects()
		{
			while (items.Count > 0)
			{
				if (items[0].Has_Expired(expiration))
					items.RemoveAt(0);
				else
					return;
			}
		}
	}

	/// <summary> Enumerator for stepping through all members of a Expiring_Item_Typed_List object </summary>
	/// <typeparam name="T"> Type for this generic class </typeparam>
	public class Expiring_Item_Typed_List_Enumerator<T> : IEnumerator<Expiring_Typed_Item<T>>
	{
		private Expiring_Item_Typed_List<T> collection;
		private readonly int count;
		private int index;

		/// <summary> Constructor for a new instance of the Expiring_Item_Typed_List_Enumerator class </summary>
		/// <param name="Collection"> Collection to iterate through </param>
		public Expiring_Item_Typed_List_Enumerator(Expiring_Item_Typed_List<T> Collection)
		{
			collection = Collection;
			index = 0;
			count = collection.Count;
		}

		#region IEnumerator<Expiring_Typed_Item<T>> Members

		/// <summary> Element at the current pointer location within the collection </summary>
		public Expiring_Typed_Item<T> Current
		{
			get { return collection[index]; }
		}

		/// <summary> Dispose of the collection within this class  </summary>
		public void Dispose()
		{
			collection = null;
		}

		/// <summary> Element at the current pointer location within the collection </summary>
		object IEnumerator.Current
		{
			get { return collection[index]; }
		}

		/// <summary> Move to the next element within this collection </summary>
		/// <returns> TRUE if successful, FALSE if there are no more elements </returns>
		public bool MoveNext()
		{
			if ((index + 1) >= count)
			{
				return false;
			}
			index++;
			return true;
		}

		/// <summary> Reset the location within this collection, to start enumerating through again from the beginning </summary>
		public void Reset()
		{
			index = 0;
		}

		#endregion
	}

	/// <summary> Generic class represents data which has a date stamp to allow for automatic expiration </summary>
	/// <typeparam name="T"> Type for this generic class </typeparam>
	/// <remarks>This class is used internally by the Expiring_Item_Typed_List class to encode a timestamp on all data</remarks>
	public class Expiring_Typed_Item<T> : IEquatable<T>
	{
		private readonly DateTime dateAdded;

		/// <summary> Constructor for a new instance of the generic Expiring_Item class </summary>
		/// <param name="Key"> Data to save </param>
		/// <param name="DataType"> Data Type </param>
		public Expiring_Typed_Item(T Key, Type DataType)
		{
			this.Key = Key;
			dateAdded = DateTime.Now;
			this.DataType = DataType;
		}

		/// <summary> Gets and sets the actual key value in this object  </summary>
		public T Key { get; set; }

		/// <summary> Gets and sets the type of the data this key points to </summary>
		public Type DataType { get; set; }

		/// <summary> Gets the expiration date for this object </summary>
		public DateTime Date_Added
		{
			get { return dateAdded; }
		}

		#region IEquatable<T> Members

		/// <summary> Checks to see if the object equals the wrapped data in this item </summary>
		/// <param name="other"> Other object to compare to this item's data payload </param>
		/// <returns>TRUE if equal, otherwise FALSE </returns>
		public bool Equals(T other)
		{
			return other.Equals(Key);
		}

		#endregion

		/// <summary> Given an expiration timespan, this checks to see if this object has expired </summary>
		/// <param name="Expiration"> Timespan for expiration of this object </param>
		/// <returns> TRUE if this item has expired, otherwise FALSE </returns>
		public bool Has_Expired(TimeSpan Expiration)
		{
			TimeSpan age = DateTime.Now.Subtract(dateAdded);
			return age.TotalSeconds > Expiration.TotalSeconds;
		}
	}
}
