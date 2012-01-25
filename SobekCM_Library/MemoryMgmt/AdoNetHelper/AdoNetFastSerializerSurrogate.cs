using System;
using System.Data;
using System.Runtime.Serialization;

namespace SobekCM.Library.MemoryMgmt
{

	/// <summary>
	/// An ISurrogateSelector implementation which looks for supported ADO.Net serializable types
	/// (DataSet and DataTable) and returns itself as an ISerializationSurrogate which can serialize
	/// those objects using Fast Serialization.
	/// </summary>
    /// <remarks>Code written by Simon Hewitt and dedicated to public domain (2006)</remarks>
	public class AdoNetFastSerializerSurrogate: ISurrogateSelector, ISerializationSurrogate
	{
		#region Fields
		private ISurrogateSelector nextSelector;
		#endregion Fields

		#region ISurrogateSelector implementation
		/// <summary>
		/// Stores the next surrogate selector in the chain.
		/// </summary>
		/// <param name="nextSelector">The ISurrogateSelector to chain.</param>
		public void ChainSelector(ISurrogateSelector nextSelector)
		{
			this.nextSelector = nextSelector;
		}

		/// <summary>
		/// Returns the next surrogate in the chain.
		/// </summary>
		/// <returns>Returns the next ISurrogateSelector in the chain.</returns>
		public ISurrogateSelector GetNextSelector()
		{
			return nextSelector;
		}

		/// <summary>
		/// Checks the Type and, if supported, returns this as an appropriate ISerializationSurrogate object.
		/// </summary>
		/// <param name="type">The Type to check.</param>
		/// <param name="context">See .Net serialization.</param>
		/// <param name="selector">this if the Type is supported; null otherwise.</param>
		/// <returns>An ISerializationSurrogate object (this) if Type is supported; null otherwise.</returns>
		public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
		{
			if (typeof(DataSet).IsAssignableFrom(type) || typeof(DataTable).IsAssignableFrom(type))
			{
				selector = this;
				return this;
			} else
			{
				selector = null;
				return null;
			}
		}
		#endregion ISurrogateSelector implementation
		
		#region ISerializationSurrogate implementation
		/// <summary>
		/// Creates a new instance of the object being deserialized (since no constructors are called)
		/// then deserializes data into it.
		/// </summary>
		/// <param name="obj">The DataSet or DataTable to serialize.</param>
		/// <param name="info">See .Net serialization.</param>
		/// <param name="context">See .Net serialization.</param>
		/// <param name="selector">The selector which selected this surrogate.</param>
		/// <returns></returns>
		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			obj = createNewInstance(obj);
			byte[] data = (byte[]) info.GetValue("_", typeof(byte[]));

			if (obj.GetType() == typeof(DataSet) || obj is IModifiedTypedDataSet)
				return AdoNetHelper.DeserializeDataSet(obj as DataSet, data);
			else if (obj.GetType() == typeof(DataTable))
				return AdoNetHelper.DeserializeDataTable(obj as DataTable, data);
			else if (obj is DataSet)
				return AdoNetHelper.DeserializeTypedDataSet(obj as DataSet, data);
			else if (obj is DataTable)
				return AdoNetHelper.DeserializeTypedDataTable(obj as DataTable, data);
			else {
				throw new InvalidOperationException("Not a supported Ado.Net object");
			}

		}

		/// <summary>
		/// Serializes the contents of the object into a byte[] and stores in the SerializationInfo block.
		/// </summary>
		/// <param name="obj">The object to serialize.</param>
		/// <param name="info">See .Net serialization.</param>
		/// <param name="context">See .Net serialization.</param>
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			byte[] data;
			if (obj.GetType() == typeof(DataSet) || obj is IModifiedTypedDataSet )
				data = AdoNetHelper.SerializeDataSet(obj as DataSet);
			else if (obj.GetType() == typeof(DataTable))
				data = AdoNetHelper.SerializeDataTable(obj as DataTable);
			else if (obj is DataSet)
				data = AdoNetHelper.SerializeTypedDataSet(obj as DataSet);
			else if (obj is DataTable)
				data = AdoNetHelper.SerializeTypedDataTable(obj as DataTable);
			else
			{
				throw new InvalidOperationException("Not a supported Ado.Net object");
			}
			info.AddValue("_", data);
		}
		#endregion ISerializationSurrogate implementation

		#region Private Methods
		/// <summary>
		/// Create a new instance of the supplied object because the object will be completely
		/// uninitialized as supplied to SetObjectData.
		/// If the object is a DataSet or DataTable (not derived from these) then a new
		/// instance is created directly otherwise a new instance is created by
		/// Activator.CreateInstance() so the type must have a parameterless constructor.
		/// </summary>
		/// <param name="obj">The uninitialized object to recreate.</param>
		/// <returns>A new instance of the type of object passed in.</returns>
		private object createNewInstance(object obj)
		{
			Type destinationType = obj.GetType();
			if (destinationType == typeof(DataSet))
				return new DataSet();
			else if (destinationType == typeof(DataTable))
				return new DataTable();
			else
			{
				return Activator.CreateInstance(destinationType);
			}
		}
		#endregion Private Methods
		
	}
	
}
