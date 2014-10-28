#region Using directives

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

#endregion

namespace SobekCM.Core.MemoryMgmt
{
	
	/// <summary> Contains static helper methods for ADO.Net objects including Fast Serialization </summary>
    /// <remarks>Code written by Simon Hewitt and dedicated to public domain (2006)</remarks>
	public class AdoNetHelper
	{
		
		#region Constructors


		private AdoNetHelper() {}
		#endregion Constructors
		
		#region Private
		private static readonly int[] zeroIntegers = new int[0];
		#endregion Private
		
		#region Static Methods

		/// <summary>
		/// Serializes a Typed DataTable to a byte[] containing only the data (ie no infrastructure)
		/// </summary>
		/// <param name="dataTable">The Typed DataTable to serialize.</param>
		/// <returns>A byte[] containing the serialized data.</returns>
        /// <remarks>The DataTable must be Typed and not a plain DataTable. It must also not have had any 
        /// new columns added to it. In either of these cases, use SerializeDataTable instead.</remarks>
		public static byte[] SerializeTypedDataTable(DataTable dataTable)
		{
			if (dataTable == null) throw new ArgumentNullException("dataTable");
			if (dataTable.GetType() == typeof(DataTable)) throw new ArgumentException("Is not a typed DataTable", "dataTable");
			if (dataTable.GetType().GetConstructor(Type.EmptyTypes) == null) throw new ArgumentException("Does not have a public, empty constructor", "dataTable");
			
			return new FastSerializer().SerializeDataOnly(dataTable);
		}
		
		/// <summary>
		/// Serializes a Typed DataSet to a byte[] containing only the data for each DataTable (ie no infrastructure)
		/// </summary>
		/// <param name="dataSet">The Typed DataSet to serialize.</param>
		/// <returns>A byte[] containing the serialized data.</returns>
        /// <remarks>The DataSet must be Typed and not a plain DataSet. It must also not have had any 
        /// new columns/tables added to it. In either of these cases, use SerializeDataSet instead.</remarks>
		public static byte[] SerializeTypedDataSet(DataSet dataSet)
		{
			if (dataSet == null) throw new ArgumentNullException("dataSet");
			if (dataSet.GetType() == typeof(DataSet)) throw new ArgumentException("Is not a typed DataSet", "dataSet");

			return new FastSerializer().SerializeDataOnly(dataSet);
		}

		/// <summary>
		/// Serializes a DataSet to a byte[].
		/// </summary>
		/// <param name="dataSet">The DataSet to serialize.</param>
		/// <returns>A byte[] containing the serialized data.</returns>
		public static byte[] SerializeDataSet(DataSet dataSet)
		{
			if (dataSet == null) throw new ArgumentNullException("dataSet");

			return new FastSerializer().Serialize(dataSet);
		}
		
		/// <summary>
		/// Serializes a DataTable to a byte[].
		/// </summary>
		/// <param name="dataTable">The DataTable to serialize.</param>
		/// <returns>A byte[] containing the serialized data.</returns>
		public static byte[] SerializeDataTable(DataTable dataTable)
		{
			if (dataTable == null) throw new ArgumentNullException("dataTable");
			
			return new FastSerializer().Serialize(dataTable);
		}
		
		/// <summary>
		/// Serializes a simple Typed DataTable to a byte[] containing only data.
		/// </summary>
		/// <param name="dataTable">The Typed DataTable to serialize.</param>
		/// <returns>A byte[] containing the serialized data.</returns>
        /// <remarks>A simple Typed DataTable will have no Errors associated with the rows
        /// or columns and all rows should be Unchanged/Added (deserialized
        /// rows will always be Unchanged).  Deleted rows will throw an exception.
        /// Designed for read-only tables which need to be serialized to a minumum size. </remarks>
		public static byte[] SerializeSimpleTypedDataTable(DataTable dataTable)
		{
			if (dataTable == null) throw new ArgumentNullException("dataTable");
			if (dataTable.HasErrors) throw new ArgumentException("Table has errors so is not a simple table", "dataTable");
			if (dataTable.GetType().GetConstructor(Type.EmptyTypes) == null) throw new ArgumentException("Does not have a public, empty constructor", "dataTable");
			
			return new FastSerializer().SerializeSimpleDataOnly(dataTable);
		}
		
		/// <summary>
		/// Deserializes a Typed DataSet from a byte[] containing serialized data only.
		/// </summary>
		/// <param name="dataSetType">The Type of Typed DataSet to deserialize.</param>
		/// <param name="serializedData">A byte[] containing the serialized data.</param>
		/// <returns>A new DataSet of the requested type.</returns>
        /// <remarks>The Type must match that from which the serialized data was originally obtained
        /// and it must have a parameterless constructor.</remarks>
		public static DataSet DeserializeTypedDataSet(Type dataSetType, byte[] serializedData)
		{
			if (dataSetType == null) throw new ArgumentNullException("dataSetType");
			if (serializedData == null) throw new ArgumentNullException("serializedData");
			
			DataSet dataSet = Activator.CreateInstance(dataSetType) as DataSet;
			if(dataSet == null || dataSet.GetType() == typeof(DataSet)) {
				throw new ArgumentException("Type is not a typed DataSet", "dataSetType");
			}
			
			return new FastDeserializer().DeserializeDataSetDataOnly(dataSet, serializedData);
		}
		
		/// <summary>
		/// Deserializes a Typed DataSet from a byte[] containing serialized data only.
		/// </summary>
		/// <param name="dataSet">The Typed DataSet to deserialize into.</param>
		/// <param name="serializedData">A byte[] containing the serialized data.</param>
		/// <returns>The same DataSet passed in.</returns>
        /// <remarks>The DataSet must be of the same type from which the serialized data was originally obtained.</remarks>
		public static DataSet DeserializeTypedDataSet(DataSet dataSet, byte[] serializedData)
		{
			if (dataSet == null) throw new ArgumentNullException("dataSet");
			if (serializedData == null) throw new ArgumentNullException("serializedData");
			if (dataSet.GetType() == typeof(DataSet)) throw new ArgumentException("Is not a typed DataSet", "dataSet");
			
			return new FastDeserializer().DeserializeDataSetDataOnly(dataSet, serializedData);
		}
		
		/// <summary>
		/// Deserializes a Typed DataTable from a byte[] containing serialized data only.
		/// </summary>
		/// <param name="dataTableType">The Type of Typed DataTable to deserialize.</param>
		/// <param name="serializedData">A byte[] containing the serialized data.</param>
		/// <returns>A new DataTable of the requested type.</returns>
        /// <remarks>The Type must match that from which the serialized data was originally obtained
        /// and it must have a parameterless constructor.</remarks>
		public static DataTable DeserializeTypedDataTable(Type dataTableType, byte[] serializedData)
		{
			if (dataTableType == null) throw new ArgumentNullException("dataTableType");
			if (serializedData == null) throw new ArgumentNullException("serializedData");
			if (dataTableType.GetConstructor(Type.EmptyTypes) == null) throw new ArgumentException("Does not have a public, empty constructor", "dataTable");
			
			DataTable DataTable = Activator.CreateInstance(dataTableType) as DataTable;
			if (DataTable == null || DataTable.GetType() == typeof(DataTable))
			{
				throw new ArgumentException("Type is not a typed DataTable", "dataTableType");
			}
			
			return new FastDeserializer().DeserializeDataTableDataOnly(DataTable, serializedData);
		}
		
		/// <summary>
		/// Deserializes a Typed DataTable from a byte[] containing serialized data only.
		/// </summary>
		/// <param name="dataTable">The Typed DataTable to deserialize into.</param>
		/// <param name="serializedData">A byte[] containing the serialized data.</param>
		/// <returns>The same DataTable passed in.</returns>
        /// <remarks>The DataTable must be of the same type from which the serialized data was originally obtained.</remarks>
		public static DataTable DeserializeTypedDataTable(DataTable dataTable, byte[] serializedData)
		{
			if (dataTable == null) throw new ArgumentNullException("dataTable");
			if (serializedData == null) throw new ArgumentNullException("serializedData");
			if (dataTable.GetType() == typeof(DataTable)) throw new ArgumentException("Is not a typed DataTable", "dataTable");
			
			return new FastDeserializer().DeserializeDataTableDataOnly(dataTable, serializedData);
		}
		
		/// <summary>
		/// Deserializes a DataSet or Typed DataSet from a byte[].
		/// </summary>
		/// <param name="dataSetType">The Type of DataSet to deserialize.</param>
		/// <param name="serializedData">A byte[] containing the serialized data.</param>
		/// <returns>Deserialized dataset</returns>
        /// <remarks>The Type must match that from which the serialized data was originally obtained
        /// and it must have a parameterless constructor.</remarks>
		public static DataSet DeserializeDataSet(Type dataSetType, byte[] serializedData)
		{
			if (dataSetType == null) throw new ArgumentNullException("dataSetType");
			if (serializedData == null) throw new ArgumentNullException("serializedData");
			
			DataSet dataSet = Activator.CreateInstance(dataSetType) as DataSet;
			if (dataSet == null) throw new ArgumentException("Type is not a DataSet", "dataSetType");
			
			return new FastDeserializer().DeserializeDataSetDataOnly(dataSet, serializedData);
		}
		
		/// <summary>
		/// Creates a new DataSet and populates it from originally serialized data.
		/// </summary>
		/// <param name="serializedData">A byte[] containing the serialized data.</param>
		/// <returns>A new and populated DataSet.</returns>
		public static DataSet DeserializeDataSet(byte[] serializedData)
		{
			if (serializedData == null) throw new ArgumentNullException("serializedData");
			
			return new FastDeserializer().DeserializeDataSet(new DataSet(), serializedData);
		}

		/// <summary>
		/// Deserializes data and infrastructure into the supplied DataSet.
		/// </summary>
		/// <param name="dataSet">The DataSet to populate.</param>
		/// <param name="serializedData">A byte[] containing the serialized data.</param>
		/// <returns>The same DataSet passed in.</returns>
		public static DataSet DeserializeDataSet(DataSet dataSet, byte[] serializedData)
		{
			if (dataSet == null) throw new ArgumentNullException("dataSet");
			if (serializedData == null) throw new ArgumentNullException("serializedData");
			
			return new FastDeserializer().DeserializeDataSet(dataSet, serializedData);
		}
		
		/// <summary>
		/// Creates a new DataTable and populates it with previously serialized data.
		/// </summary>
		/// <param name="serializedData">A byte[] containing the serialized data.</param>
		/// <returns>A new and populated DataTable</returns>
		public static DataTable DeserializeDataTable(byte[] serializedData)
		{
			return DeserializeDataTable(new DataTable(), serializedData);
		}
		
		/// <summary>
		/// Populates the supplied DataTable with previously serialized data.
		/// </summary>
		/// <param name="dataTable">The DataTable to populate.</param>
		/// <param name="serializedData">A byte[] containing the serialized data.</param>
		/// <returns>The same DataTable passed in.</returns>
		public static DataTable DeserializeDataTable(DataTable dataTable, byte[] serializedData)
		{
			if (dataTable == null) throw new ArgumentNullException("dataTable");
			if (serializedData == null) throw new ArgumentNullException("serializedData");

			return new FastDeserializer().DeserializeDataTable(dataTable, serializedData);
		}
		
		/// <summary>
		/// Populates an existing Typed DataTable with row data previously serialized using
		/// SerializeSimpleTypedDataTable.
		/// </summary>
		/// <param name="dataTable">The DataTable into which to deserialize row data.</param>
		/// <param name="serializedData">A byte[] containing the serialized data.</param>
		/// <returns>The same DataTable passed in.</returns>
        /// <remarks>All rows will have Unchanged state even if they were Added or Modified at
        /// the time of serialization.</remarks>
		public static DataTable DeserializeSimpleTypedDataTable(DataTable dataTable, byte[] serializedData)
		{
			if (dataTable == null) throw new ArgumentNullException("dataTable");
			if (serializedData == null) throw new ArgumentNullException("serializedData");
			
			return new FastDeserializer().DeserializeSimpleTypedDataTable(dataTable, serializedData);
		}
		
		/// <summary>
		/// Create a new DataTable of the requested type and populates it with row data previously
		/// serialized using SerializeSimpleDataTable.
		/// </summary>
		/// <param name="dataTableType">The Type of DataTable to create.</param>
		/// <param name="serializedData">A byte[] containing the serialized data.</param>
		/// <returns>Deserialized data table</returns>
        /// <remarks>The type must have a parameterless constuctor and should be of the same type as
        /// previously serialized. <br /><br /> 
        /// All rows will have Unchanged state even if they were Added or Modified at
        /// the time of serialization.</remarks>
		public static DataTable DeserializeSimpleTypedDataTable(Type dataTableType, byte[] serializedData)
		{
			if (dataTableType == null) throw new ArgumentNullException("dataTableType");
			if (serializedData == null) throw new ArgumentNullException("serializedData");
			if (dataTableType.GetConstructor(Type.EmptyTypes) == null) throw new ArgumentException("Does not have a public, empty constructor", "dataTable");
			
			DataTable dataTable = Activator.CreateInstance(dataTableType) as DataTable;
			if (dataTable == null) throw new ArgumentException("Type is not a DataTable", "dataTableType");
			
			return new FastDeserializer().DeserializeSimpleTypedDataTable(dataTable, serializedData);
		}

		/// <summary>
		/// Returns an object[] of all the row values for the specified version
		/// </summary>
		/// <param name="row">The row from which to extract values</param>
		/// <param name="version"></param>
		/// <returns>An object[] holding the values.</returns>
		public static object[] GetValuesFromRowVersion(DataRow row, DataRowVersion version)
		{
			int columnCount = row.Table.Columns.Count;
			object[] result = new object[columnCount];
			for(int j = 0; j < columnCount; j++)
			{
				result[j] = row[j, version];
			}
			return result;
		}

		/// <summary>
		/// Maps DateTime.MinValue to DBNull.Value.
		/// </summary>
		/// <param name="value">The DateTime value to check.</param>
		/// <returns>DBNull.Value if the DateTime is MinValue; otherwise the DateTime value.</returns>
		public static object GetNullableDateTime(DateTime value) {
			if (value == DateTime.MinValue)
				return DBNull.Value;
			else {
				return value;
			}
		}
		
		/// <summary>
		/// Gets an object[] holding the PrimaryKey column values.
		/// The Row must be in a DataTable.
		/// </summary>
		/// <param name="row">The row from which to find values.</param>
		/// <returns>An object[] holding the PrimaryKey values for the row, or an empty object[] if there is no PrimaryKey defined.</returns>
		public static object[] GetKeyValuesFromRow(DataRow row) {
			if (row.Table == null) throw new ArgumentOutOfRangeException("row", "Row is not in a table");
			return GetKeyValuesFromRow(row, row.Table);
		}

		/// <summary>
		/// Gets an object[] holding the values from the columns having the same names as those of the PrimaryKey columns
		/// from the supplied DataTable.
		/// </summary>
		/// <param name="row">The row from which to find values.</param>
		/// <param name="primaryKeyProvider">The DataTable holding the PrimaryKey columns.</param>
		/// <returns>An object[] holding the corresponding values for the row, or an empty object[] if there is no PrimaryKey defined.</returns>
		public static object[] GetKeyValuesFromRow(DataRow row, DataTable primaryKeyProvider) {
			if (row == null) throw new ArgumentNullException("row");
			if (primaryKeyProvider == null) throw new ArgumentNullException("primaryKeyProvider");

			DataColumn[] keyColumns = primaryKeyProvider.PrimaryKey;
			DataRowVersion rowVersion = row.RowState == DataRowState.Deleted ? DataRowVersion.Original : DataRowVersion.Current;

			object[] keyValues = new object[keyColumns.Length];
			for(int i = 0; i < keyColumns.Length; i++) {
				keyValues[i] = row[keyColumns[i].ColumnName, rowVersion];
			}

			return keyValues;
		}
		
		/// <summary>
		/// Gets a list of unique values for a column from a list of rows.
		/// </summary>
		/// <param name="rows">The DataRows in which to look for unique values.</param>
		/// <param name="columnName">The name of the column in which to look for unique values.</param>
		/// <returns>An ArrayList containing the unique values.</returns>
		public static ArrayList GetUniqueColumnValues(DataRow[] rows, string columnName) {
			if (rows == null) throw new ArgumentNullException();
			if (columnName == null) throw new ArgumentNullException();
			if (rows.Length == 0) return new ArrayList();
			DataColumn column = rows[0].Table.Columns[columnName];
			if (column == null) throw new DataException(string.Format("Column '{0}' is not in Table '{1}'", columnName, rows[0].Table.TableName));

			return getUniqueValues(rows, column);
		}

		/// <summary>
		/// Gets a list of unique values for a column from a list of rows.
		/// </summary>
		/// <param name="rows">The DataRows in which to look for unique values.</param>
		/// <param name="columnOrdinal">The ordinal of the column in which to look for unique values.</param>
		/// <returns>An ArrayList containing the unique values.</returns>
		public static ArrayList GetUniqueColumnValues(DataRow[] rows, int columnOrdinal) {
			if (rows == null) throw new ArgumentNullException();
			DataColumn column = rows[0].Table.Columns[columnOrdinal];

			return getUniqueValues(rows, column);
		}

		/// <summary>
		/// Gets a list of unique values for a column from a list of rows.
		/// </summary>
		/// <param name="rows">The DataRows in which to look for unique values.</param>
		/// <param name="column">The column in which to look for unique values.</param>
		/// <returns>An ArrayList containing the unique values.</returns>
		public static ArrayList GetUniqueColumnValues(DataRow[] rows, DataColumn column) {
			if (rows == null) throw new ArgumentNullException();
			if (column == null) throw new ArgumentNullException();
			
			return getUniqueValues(rows, column);
		}

		/// <summary>
		/// Create a simple string dump of the contents (each table) of a DataSet, including the key values for each row.
		/// </summary>
		/// <param name="dataset">The DataSet to dump</param>
		/// <param name="description">A description of the DataSet contents</param>
		/// <returns></returns>
		public static string GetDataSetDump(DataSet dataset, string description) {
			StringBuilder sb = new StringBuilder();

			// Dump the description
			sb.Append(description);
			sb.Append(Environment.NewLine);

			// Dump each contained table
			foreach(DataTable dataTable in dataset.Tables) {
				sb.Append(getTableDump(dataTable));
			}

			// Return the dump string
			return sb.ToString();
			
		}

        /// <summary> Gets the table dump (as a string) for a particular table </summary>
        /// <param name="dataTable"> Datatable to dump </param>
        /// <param name="description"> Description of the table </param>
        /// <returns> Dumped table and description</returns>
		public static string GetTableDump(DataTable dataTable, string description) {
			StringBuilder sb = new StringBuilder();

			// Dump the description
			sb.Append(description);
			sb.Append(Environment.NewLine);

			// Dump the table
			sb.Append(getTableDump(dataTable));

			// Return the dump string
			return sb.ToString();
			
		}
		#endregion Static Methods
		
		#region Private Static Methods
		private static string getTableDump(DataTable dataTable) {
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("table '{0}' contains {1} rows", dataTable.TableName, dataTable.Rows.Count);
			sb.Append(Environment.NewLine);
			int rowNumber = 0;
			foreach(DataRow row in dataTable.Rows) {
				object[] keyValues = GetKeyValuesFromRow(row);
				string[] keyValueStrings = new string[keyValues.Length];
				for(int i = 0; i < keyValues.Length; i++) {
					keyValueStrings[i] = keyValues[i].ToString();
				}

				sb.AppendFormat("{0,3}: State={1}, key={2}", rowNumber, row.RowState, string.Join(";", keyValueStrings));
				sb.Append(Environment.NewLine);

				rowNumber++;
			}
			sb.Append(Environment.NewLine);
			sb.Append(Environment.NewLine);

			return sb.ToString();
		}

		private static ArrayList getUniqueValues(DataRow[] rows, DataColumn column) {
			ArrayList uniqueValues = new ArrayList();

			if (rows.Length == 1)
				uniqueValues.Add(rows[0][column]);
			else if (rows.Length != 0) {
				foreach(DataRow row in rows) {
					object value = row[column];
					if (!uniqueValues.Contains(value)) {
						uniqueValues.Add(value);
					}
				}
			}

			return uniqueValues;
		}
		#endregion Private Static Methods
		
		#region Fast Serialization
		#region Flags
		#region DataSet
		private static readonly int DataSetHasName = BitVector32.CreateMask();
		private static readonly int DataSetIsCaseSensitive = BitVector32.CreateMask(DataSetHasName);
		private static readonly int DataSetHasTables = BitVector32.CreateMask(DataSetIsCaseSensitive);
		private static readonly int DataSetHasRelationships = BitVector32.CreateMask(DataSetHasTables);
		private static readonly int DataSetHasForeignKeyConstraints = BitVector32.CreateMask(DataSetHasRelationships);
		private static readonly int DataSetHasExtendedProperties = BitVector32.CreateMask(DataSetHasForeignKeyConstraints);
		private static readonly int DataSetAreConstraintsEnabled = BitVector32.CreateMask(DataSetHasExtendedProperties);
		private static readonly int DataSetHasNamespace = BitVector32.CreateMask(DataSetAreConstraintsEnabled);
		private static readonly int DataSetHasPrefix = BitVector32.CreateMask(DataSetHasNamespace);

		private static BitVector32 GetDataSetFlags(DataSet dataSet)
		{
			BitVector32 flags = new BitVector32();
			
			flags[DataSetHasName] = dataSet.DataSetName != "NewDataSet";
			flags[DataSetIsCaseSensitive] = dataSet.CaseSensitive;
			flags[DataSetHasTables] = dataSet.Tables.Count != 0;
			flags[DataSetHasRelationships] = dataSet.Relations.Count != 0;
			flags[DataSetHasForeignKeyConstraints] = getForeignKeyConstraints(dataSet).Length != 0;
			flags[DataSetHasExtendedProperties] = dataSet.ExtendedProperties.Count != 0;
			flags[DataSetAreConstraintsEnabled] = dataSet.EnforceConstraints;
			flags[DataSetHasNamespace] = dataSet.Namespace != string.Empty;
			flags[DataSetHasPrefix] = dataSet.Prefix != string.Empty;
			
			return flags;
		}
		#endregion DataSet

		#region Table
		private static readonly int TableHasName = BitVector32.CreateMask();
		private static readonly int TableIsCaseSensitive = BitVector32.CreateMask(TableHasName);
		private static readonly int TableIsCaseSensitiveAmbient = BitVector32.CreateMask(TableIsCaseSensitive);
		private static readonly int TableHasSpecificCulture = BitVector32.CreateMask(TableIsCaseSensitiveAmbient);
		private static readonly int TableHasColumns = BitVector32.CreateMask(TableHasSpecificCulture);
		private static readonly int TableHasRows = BitVector32.CreateMask(TableHasColumns);
		private static readonly int TableHasMinimumCapacity = BitVector32.CreateMask(TableHasRows);
		private static readonly int TableHasDisplayExpression = BitVector32.CreateMask(TableHasMinimumCapacity);
		private static readonly int TableHasNoUniqueConstraints = BitVector32.CreateMask(TableHasDisplayExpression);
		private static readonly int TableHasExtendedProperties = BitVector32.CreateMask(TableHasNoUniqueConstraints);
		private static readonly int TableHasNamespace = BitVector32.CreateMask(TableHasExtendedProperties);
		private static readonly int TableHasPrefix = BitVector32.CreateMask(TableHasNamespace);
		private static readonly int TableHasRepeatableElement = BitVector32.CreateMask(TableHasPrefix);
        private static readonly int TableHasTypeName = BitVector32.CreateMask(TableHasRepeatableElement);
		
		private static readonly FieldInfo TableCultureFieldInfo =
			typeof(DataTable).GetField("_cultureUserSet", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly FieldInfo TableCaseSensitiveAmbientFieldInfo =
			typeof(DataTable).GetField("_caseSensitiveUserSet", BindingFlags.Instance | BindingFlags.NonPublic);
	
		private static readonly FieldInfo TableTypeNameFieldInfo =
			typeof(DataTable).GetField("typeName", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly FieldInfo TableRepeatableElementFieldInfo =
			typeof(DataTable).GetField("repeatableElement", BindingFlags.Instance | BindingFlags.NonPublic);
		
		private static BitVector32 GetTableFlags(DataTable dataTable)
		{
			BitVector32 flags = new BitVector32();
			
			flags[TableHasName] = dataTable.TableName != string.Empty;
			flags[TableIsCaseSensitive] = dataTable.CaseSensitive;
			flags[TableIsCaseSensitiveAmbient] = !(bool) TableCaseSensitiveAmbientFieldInfo.GetValue(dataTable);
			flags[TableHasTypeName] = (XmlQualifiedName) TableTypeNameFieldInfo.GetValue(dataTable) != null;
			flags[TableHasSpecificCulture] = (bool) TableCultureFieldInfo.GetValue(dataTable);	
			flags[TableHasColumns] = dataTable.Columns.Count != 0;
			flags[TableHasRows] = dataTable.Rows.Count != 0;
			flags[TableHasMinimumCapacity] = dataTable.MinimumCapacity != 50;
			flags[TableHasDisplayExpression] = dataTable.DisplayExpression != string.Empty;
			flags[TableHasNoUniqueConstraints] = getUniqueConstraints(dataTable).Length == 0;
			flags[TableHasExtendedProperties] = dataTable.ExtendedProperties.Count != 0;
			
			flags[TableHasNamespace] = dataTable.Namespace != string.Empty;
			flags[TableHasPrefix] = dataTable.Prefix != string.Empty;
			flags[TableHasRepeatableElement] = (bool) TableRepeatableElementFieldInfo.GetValue(dataTable);
			
			return flags;
		}
		#endregion Table

		#region Column
		private static readonly int ColumnIsNullable = BitVector32.CreateMask();
		private static readonly int ColumnIsNotStringDataType = BitVector32.CreateMask(ColumnIsNullable);
		private static readonly int ColumnHasCaption = BitVector32.CreateMask(ColumnIsNotStringDataType);
		private static readonly int ColumnHasMaxLength = BitVector32.CreateMask(ColumnHasCaption);
		private static readonly int ColumnHasAutoIncrement = BitVector32.CreateMask(ColumnHasMaxLength);
		private static readonly int ColumnHasAutoIncrementUnusedDefaults = BitVector32.CreateMask(ColumnHasAutoIncrement);
		private static readonly int ColumnHasAutoIncrementNegativeStep = BitVector32.CreateMask(ColumnHasAutoIncrementUnusedDefaults);

		private static readonly int ColumnHasDefaultValue = BitVector32.CreateMask(ColumnHasAutoIncrementNegativeStep);
		private static readonly int ColumnHasExpression = BitVector32.CreateMask(ColumnHasDefaultValue);
		private static readonly int ColumnIsReadOnly = BitVector32.CreateMask(ColumnHasExpression);
		private static readonly int ColumnIsMaxLengthMaxValue = BitVector32.CreateMask(ColumnIsReadOnly);
		private static readonly int ColumnHasExtendedProperties = BitVector32.CreateMask(ColumnIsMaxLengthMaxValue);
		
		private static readonly int ColumnHasPrefix = BitVector32.CreateMask(ColumnHasExtendedProperties);
		private static readonly int ColumnIsNotElementMappingType = BitVector32.CreateMask(ColumnHasPrefix);
		private static readonly int ColumnHasColumnUri = BitVector32.CreateMask(ColumnIsNotElementMappingType);

		private static readonly FieldInfo ColumnUriFieldInfo =
			typeof(DataColumn).GetField("_columnUri", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly FieldInfo AutoIncrementCurrentFieldInfo =
			typeof(DataColumn).GetField("autoIncrementCurrent", BindingFlags.Instance | BindingFlags.NonPublic);

		private static BitVector32 GetColumnFlags(DataColumn dataColumn)
		{
			BitVector32 flags = new BitVector32();

			flags[ColumnIsNullable] = dataColumn.AllowDBNull;
			flags[ColumnIsNotStringDataType] = dataColumn.DataType != typeof(string);
			flags[ColumnHasCaption] = dataColumn.Caption != dataColumn.ColumnName;
			flags[ColumnHasMaxLength] = dataColumn.MaxLength != -1;
			flags[ColumnIsMaxLengthMaxValue] = dataColumn.MaxLength == int.MaxValue;
			flags[ColumnHasAutoIncrement] = dataColumn.AutoIncrement;
			long current = (long) AutoIncrementCurrentFieldInfo.GetValue(dataColumn);
			long step = dataColumn.AutoIncrementStep;
			flags[ColumnHasAutoIncrementUnusedDefaults] = (current == 0 && step == 1) || (current == -1 && step == -1);
			flags[ColumnHasAutoIncrementNegativeStep] = dataColumn.AutoIncrementStep < 0;
			flags[ColumnHasDefaultValue] = dataColumn.DefaultValue != DBNull.Value;
			flags[ColumnHasExpression] = dataColumn.Expression != string.Empty;
			flags[ColumnIsReadOnly] = dataColumn.ReadOnly;
			flags[ColumnHasExtendedProperties] = dataColumn.ExtendedProperties.Count != 0;
			
			flags[ColumnHasPrefix] = dataColumn.Prefix != string.Empty;
			flags[ColumnIsNotElementMappingType] = dataColumn.ColumnMapping != MappingType.Element;
			flags[ColumnHasColumnUri] = ColumnUriFieldInfo.GetValue(dataColumn) != null;
			
			return flags;
		}
		#endregion Column
		
		#region Row
		private static readonly int RowHasOldData = BitVector32.CreateMask();
		private static readonly int RowHasNewData = BitVector32.CreateMask(RowHasOldData);
		private static readonly int RowHasRowError = BitVector32.CreateMask(RowHasNewData);
		private static readonly int RowHasColumnErrors = BitVector32.CreateMask(RowHasRowError);
		
		private static BitVector32 GetRowFlags(DataRow row)
		{
			BitVector32 flags = new BitVector32();
			
			DataRowState state = row.RowState;
			flags[RowHasOldData] = state == DataRowState.Deleted || state == DataRowState.Modified;
			flags[RowHasNewData] = state == DataRowState.Added || state == DataRowState.Modified;
			flags[RowHasRowError] = row.RowError.Length != 0;
			flags[RowHasColumnErrors] = row.GetColumnsInError().Length != 0;
			
			return flags;
		}
		#endregion Row
		
		#region Constraints
		#region Common
		private static readonly FieldInfo ConstraintSchemaNameFieldInfo =
			typeof(Constraint).GetField("_schemaName", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly Regex DefaultConstraintNameMatcher = new Regex(@"^Constraint(\d+)$");
		#endregion Common
		
		#region Unique
		private static readonly int UniqueConstraintHasDefaultName = BitVector32.CreateMask();
		private static readonly int UniqueConstraintHasSchemaName = BitVector32.CreateMask(UniqueConstraintHasDefaultName);
		private static readonly int UniqueConstraintHasMultipleColumns = BitVector32.CreateMask(UniqueConstraintHasSchemaName);
		private static readonly int UniqueConstraintHasExtendedProperties = BitVector32.CreateMask(UniqueConstraintHasMultipleColumns);
		private static readonly int UniqueConstraintIsPrimaryKey = BitVector32.CreateMask(UniqueConstraintHasExtendedProperties);

		private static BitVector32 GetUniqueConstraintFlags(UniqueConstraint uniqueConstraint)
		{
			BitVector32 flags = new BitVector32();

			flags[UniqueConstraintHasDefaultName] = DefaultConstraintNameMatcher.IsMatch(uniqueConstraint.ConstraintName);
			
			flags[UniqueConstraintHasSchemaName] = (string) ConstraintSchemaNameFieldInfo.GetValue(uniqueConstraint) != string.Empty;
			flags[UniqueConstraintHasMultipleColumns] = uniqueConstraint.Columns.Length > 1;
			flags[UniqueConstraintHasExtendedProperties] = uniqueConstraint.ExtendedProperties.Count != 0;
			flags[UniqueConstraintIsPrimaryKey] = uniqueConstraint.IsPrimaryKey;
			
			return flags;
		}
		
		private static UniqueConstraint[] getUniqueConstraints(DataTable dataTable)
		{
			if (dataTable.Constraints.Count == 0) return new UniqueConstraint[0];
			ArrayList constraints = new ArrayList();
			foreach(Constraint constraint in dataTable.Constraints)
			{
				if (constraint is UniqueConstraint) constraints.Add(constraint);
			}
			
			return (UniqueConstraint[]) constraints.ToArray(typeof(UniqueConstraint));
		}
		#endregion Unique
		
		#region Foreign Key
		private static readonly int ForeignKeyConstraintHasDefaultName = BitVector32.CreateMask();
		private static readonly int ForeignKeyConstraintIsPrimaryKeyOnParentTable =
			BitVector32.CreateMask(ForeignKeyConstraintHasDefaultName);
		private static readonly int ForeignKeyConstraintHasMultipleColumns =
			BitVector32.CreateMask(ForeignKeyConstraintIsPrimaryKeyOnParentTable);
		private static readonly int ForeignKeyConstraintHasAcceptRejectRule = BitVector32.CreateMask(ForeignKeyConstraintHasMultipleColumns);
		private static readonly int ForeignKeyConstraintHasNonCascadeDeleteRule =
			BitVector32.CreateMask(ForeignKeyConstraintHasAcceptRejectRule);
		private static readonly int ForeignKeyConstraintHasNonCascadeUpdateRule =
			BitVector32.CreateMask(ForeignKeyConstraintHasNonCascadeDeleteRule);
		private static readonly int ForeignKeyConstraintHasExtendedProperties =
			BitVector32.CreateMask(ForeignKeyConstraintHasNonCascadeUpdateRule);
		private static readonly int ForeignKeyConstraintHasSchemaName = BitVector32.CreateMask(ForeignKeyConstraintHasExtendedProperties);
		
		private static BitVector32 GetForeignKeyConstraintFlags(ForeignKeyConstraint foreignKeyConstraint)
		{
			BitVector32 flags = new BitVector32();

			flags[ForeignKeyConstraintHasDefaultName] = DefaultConstraintNameMatcher.IsMatch(foreignKeyConstraint.ConstraintName);
			flags[ForeignKeyConstraintIsPrimaryKeyOnParentTable] =
				ColumnOrdinalsMatch(foreignKeyConstraint.RelatedColumns, foreignKeyConstraint.RelatedTable.PrimaryKey);
			flags[ForeignKeyConstraintHasMultipleColumns] = foreignKeyConstraint.Columns.Length > 1;
			flags[ForeignKeyConstraintHasAcceptRejectRule] = foreignKeyConstraint.AcceptRejectRule != AcceptRejectRule.None;
			flags[ForeignKeyConstraintHasNonCascadeUpdateRule] = foreignKeyConstraint.UpdateRule != Rule.Cascade;
			flags[ForeignKeyConstraintHasNonCascadeDeleteRule] = foreignKeyConstraint.DeleteRule != Rule.Cascade;
			flags[ForeignKeyConstraintHasExtendedProperties] = foreignKeyConstraint.ExtendedProperties.Count != 0;
			flags[ForeignKeyConstraintHasSchemaName] = (string) ConstraintSchemaNameFieldInfo.GetValue(foreignKeyConstraint) != string.Empty;
			
			return flags;
		}

		private static ForeignKeyConstraint[] getForeignKeyConstraints(DataSet dataSet)
		{
			ArrayList constraints = new ArrayList();
			foreach(DataTable dataTable in dataSet.Tables)
			{
				foreach(Constraint constraint in dataTable.Constraints)
				{
					if (constraint is ForeignKeyConstraint) constraints.Add(constraint);
				}
			}
			return (ForeignKeyConstraint[]) constraints.ToArray(typeof(ForeignKeyConstraint));
		}
		
		private static bool ColumnOrdinalsMatch(DataColumn[] columns1, DataColumn[] columns2)
		{
			if (columns1 != columns2)
			{
				if ( columns1 == null || columns2 == null) return false;
				if (columns1.Length != columns2.Length) return false;

				for (int i = 0; i < columns1.Length; i++)
				{
					int matchingColumn = 0;
					while (matchingColumn < columns2.Length)
					{
						if (columns1[i].Equals(columns2[matchingColumn])) break;
						matchingColumn++;
					}
					if (matchingColumn == columns2.Length)
					{
						return false;
					}
				}
			}

			return true;
		}
		#endregion Foreign Key
		#endregion Constraints

		#region Relation
		private static readonly int RelationHasDefaultName = BitVector32.CreateMask();
		private static readonly int RelationIsNested = BitVector32.CreateMask(RelationHasDefaultName);
		private static readonly int RelationHasMultipleColumns = BitVector32.CreateMask(RelationIsNested);
		private static readonly int RelationIsPrimaryKeyOnParentTable = BitVector32.CreateMask(RelationHasMultipleColumns);
		private static readonly int RelationHasExtendedProperties = BitVector32.CreateMask(RelationIsPrimaryKeyOnParentTable);

		private static BitVector32 GetRelationFlags(DataRelation relation)
		{
			BitVector32 flags = new BitVector32();
			
			flags[RelationHasDefaultName] = relation.RelationName != string.Empty;
			flags[RelationIsNested] = relation.Nested;
			flags[RelationHasMultipleColumns] = relation.ParentColumns.Length > 1;
			flags[RelationIsPrimaryKeyOnParentTable] = relation.ParentKeyConstraint != null && relation.ParentKeyConstraint.IsPrimaryKey;
			flags[RelationHasExtendedProperties] = relation.ExtendedProperties.Count != 0;
			
			return flags;
		}
		#endregion Relation
		#endregion Flags
		
		#region Serializer
		private class FastSerializer
		{
			
			#region Fields
			private DataSet dataSet;
			private SerializationWriter writer;
			#endregion Fields
			
			#region Methods
			public byte[] Serialize(DataSet dataSet)
			{
				this.dataSet = dataSet;
				writer = new SerializationWriter();
			
				BitVector32 flags = GetDataSetFlags(dataSet);
				writer.WriteOptimized(flags);
			
				if (flags[DataSetHasName]) writer.WriteOptimized(dataSet.DataSetName);
				writer.WriteOptimized(dataSet.Locale.LCID);
				if (flags[DataSetHasNamespace]) writer.WriteOptimized(dataSet.Namespace);
				if (flags[DataSetHasPrefix]) writer.WriteOptimized(dataSet.Prefix);
				if (flags[DataSetHasTables]) serializeTables();
				if (flags[DataSetHasForeignKeyConstraints]) serializeForeignKeyConstraints(getForeignKeyConstraints(dataSet));
				if (flags[DataSetHasRelationships]) serializeRelationships();
				if (flags[DataSetHasExtendedProperties]) serializeExtendedProperties(dataSet.ExtendedProperties);

				return getSerializedBytes();
			}
			
			public byte[] SerializeDataOnly(DataTable dataTable)
			{
				writer = new SerializationWriter();
				
				serializeRows(dataTable);								
			
				return getSerializedBytes();
			}
			
			public byte[] SerializeDataOnly(DataSet dataSet)
			{
				writer = new SerializationWriter();
				
				writer.WriteOptimized(dataSet.Tables.Count);
				foreach(DataTable dataTable in dataSet.Tables)
				{
					serializeRows(dataTable);								
				}
			
				return getSerializedBytes();
			}
			
			public byte[] Serialize(DataTable dataTable)
			{
				writer = new SerializationWriter();
			
				serializeTable(dataTable);
			
				return getSerializedBytes();
			}
			
			public byte[] SerializeSimpleDataOnly(DataTable dataTable)
			{
				writer = new SerializationWriter();
				
				int rowCount = dataTable.Rows.Count;
				writer.WriteOptimized(rowCount);
				if (rowCount != 0)
				{
					int[] calculatedColumnOrdinals = getCalculatedColumnOrdinals(dataTable);

					foreach(DataRow row in dataTable.Rows)
					{
						writer.WriteOptimized(getNonCalculatedValuesFromRowVersion(row, DataRowVersion.Current, calculatedColumnOrdinals));
					}
				}
				
				return getSerializedBytes();
			}
			#endregion Methods
			
			#region Private Methods
			private byte[] getSerializedBytes()
			{
				byte[] result = writer.ToArray();
				writer = null;
				dataSet = null;
				return result;
			}
		
			private void serializeRelationships()
			{
				DataRelationCollection relations = dataSet.Relations;
				writer.WriteOptimized(relations.Count);
			
				foreach(DataRelation relation in relations)
				{
					BitVector32 flags = GetRelationFlags(relation);
					writer.WriteOptimized(flags);
				
					if (flags[RelationHasDefaultName]) writer.WriteOptimized(relation.RelationName);
				
					// Write Parent Table column info
					writer.WriteOptimized(dataSet.Tables.IndexOf(relation.ParentTable));
					if (!flags[RelationIsPrimaryKeyOnParentTable]) writeColumnOrdinals(relation.ParentColumns);
				
					// Write Child Table column info
					writeTableAndColumnOrdinals(relation.ChildColumns);
				
					if (flags[RelationHasExtendedProperties]) serializeExtendedProperties(relation.ExtendedProperties);
				}
			}

			private void serializeRows(DataTable dataTable)
			{
				int rowCount = dataTable.Rows.Count;
				writer.WriteOptimized(rowCount);
				
				if (rowCount != 0)
				{
					int[] calculatedColumnOrdinals = getCalculatedColumnOrdinals(dataTable);
				
					for(int i = 0; i < rowCount; i++)
					{
						DataRow row = dataTable.Rows[i];
				
						BitVector32 flags = GetRowFlags(row);
						writer.WriteOptimized(flags);
				
						if (!flags[RowHasOldData])
							writer.WriteOptimized(getNonCalculatedValuesFromRowVersion(row, DataRowVersion.Current, calculatedColumnOrdinals));
						else if (!flags[RowHasNewData])
							writer.WriteOptimized(getNonCalculatedValuesFromRowVersion(row, DataRowVersion.Original, calculatedColumnOrdinals));
						else
						{
							writer.WriteOptimized(
								getNonCalculatedValuesFromRowVersion(row, DataRowVersion.Current, calculatedColumnOrdinals),
								getNonCalculatedValuesFromRowVersion(row, DataRowVersion.Original, calculatedColumnOrdinals));
						}
				
						if (flags[RowHasRowError]) writer.WriteOptimized(row.RowError);
						if (flags[RowHasColumnErrors])
						{
							DataColumn[] columnsInError = row.GetColumnsInError();
							writer.WriteOptimized(columnsInError.Length);
							for(int j = 0; j < columnsInError.Length; j++)
							{
								writer.WriteOptimized(columnsInError[j].Ordinal);
								writer.WriteOptimized(row.GetColumnError(columnsInError[j]));
							}
						}
					}
				}
			}
			
			private int[] getCalculatedColumnOrdinals(DataTable dataTable)
			{
				ArrayList result = null;
				foreach(DataColumn column in dataTable.Columns)
				{
					if (column.Expression.Length != 0)
					{
						if (result == null) result = new ArrayList();
						result.Add(column.Ordinal);
					}
				}
				
				return (int[]) (result == null ? zeroIntegers : result.ToArray(typeof(int)));
			}
			
			private object[] getNonCalculatedValuesFromRowVersion(DataRow row, DataRowVersion version, int[] calculatedColumnOrdinals)
			{
				object[] result = GetValuesFromRowVersion(row, version);
				if (calculatedColumnOrdinals.Length != 0)
				{
					foreach(int calculatedColumnOrdinal in calculatedColumnOrdinals)
					{
						result[calculatedColumnOrdinal] = null;
					}
				}
				return result;
			}

			private void serializeTables()
			{
				DataTableCollection tables = dataSet.Tables;
				writer.WriteOptimized(tables.Count);
			
				foreach(DataTable dataTable in tables)
				{
					serializeTable(dataTable);
				}
			}

			private void serializeTable(DataTable dataTable) 
			{
				#region Flags
				BitVector32 flags = GetTableFlags(dataTable);
				writer.WriteOptimized(flags);

				if (flags[TableHasName]) writer.WriteOptimized(dataTable.TableName);
				if (flags[TableHasNamespace]) writer.WriteOptimized(dataTable.Namespace);
				if (flags[TableHasPrefix]) writer.WriteOptimized(dataTable.Prefix);
				if (flags[TableHasSpecificCulture]) writer.WriteOptimized(dataTable.Locale.LCID);
				if (flags[TableHasTypeName])
				{
					XmlQualifiedName xmlQualifiedName = (XmlQualifiedName) TableTypeNameFieldInfo.GetValue(dataTable);
                    //NEED FIX writer.WriteOptimized(xmlQualifiedName.Name);
                    //NEED FIX writer.WriteOptimized(xmlQualifiedName.Namespace);

				}
				if (flags[TableHasMinimumCapacity]) writer.WriteOptimized(dataTable.MinimumCapacity);
				#endregion Flags
				
				#region Columns
				if (flags[TableHasColumns])
				{
					serializeColumns(dataTable);
					if (flags[TableHasDisplayExpression]) writer.WriteOptimized(dataTable.DisplayExpression);
				}
				#endregion Columns
				
				#region Extended Properties
				if (flags[TableHasExtendedProperties]) serializeExtendedProperties(dataTable.ExtendedProperties);
				#endregion Extended Properties

				#region Data Rows
				if (flags[TableHasRows]) serializeRows(dataTable);
				#endregion Data Rows
			
				#region UniqueConstraints
				if (!flags[TableHasNoUniqueConstraints]) serializeUniqueConstraints(getUniqueConstraints(dataTable));
				#endregion UniqueConstraints
			}

			private void serializeUniqueConstraints(UniqueConstraint[] uniqueConstraints)
			{
				writer.WriteOptimized(uniqueConstraints.Length);
				foreach(UniqueConstraint uniqueConstraint in uniqueConstraints)
				{
					BitVector32 flags = GetUniqueConstraintFlags(uniqueConstraint);
					writer.WriteOptimized(flags);
				
					if (!flags[UniqueConstraintHasDefaultName])
						writer.WriteOptimized(uniqueConstraint.ConstraintName);
					else
					{
						writer.WriteOptimized(int.Parse(DefaultConstraintNameMatcher.Match(uniqueConstraint.ConstraintName).Groups[1].Value));
					}
					writeColumnOrdinals(uniqueConstraint.Columns);
				
					if (flags[UniqueConstraintHasSchemaName]) writer.WriteOptimized((string) ConstraintSchemaNameFieldInfo.GetValue(uniqueConstraint));
					if (flags[UniqueConstraintHasExtendedProperties]) serializeExtendedProperties(uniqueConstraint.ExtendedProperties);
				
				}
			}
			
			private void serializeForeignKeyConstraints(ForeignKeyConstraint[] foreignKeyConstraints)
			{
				writer.WriteOptimized(foreignKeyConstraints.Length);
				foreach(ForeignKeyConstraint foreignKeyConstraint in foreignKeyConstraints)
				{
					BitVector32 flags = GetForeignKeyConstraintFlags(foreignKeyConstraint);
					writer.WriteOptimized(flags);
				
					if (!flags[ForeignKeyConstraintHasDefaultName])
						writer.WriteOptimized(foreignKeyConstraint.ConstraintName);
					else
					{
						writer.WriteOptimized(int.Parse(DefaultConstraintNameMatcher.Match(foreignKeyConstraint.ConstraintName).Groups[1].Value));
					}
				
					// Save Child Table column info
					writeTableAndColumnOrdinals(foreignKeyConstraint.Columns);
	
					// Save Related (parent) Table column info
					writer.WriteOptimized(dataSet.Tables.IndexOf(foreignKeyConstraint.RelatedTable));
					if (!flags[ForeignKeyConstraintIsPrimaryKeyOnParentTable]) writeColumnOrdinals(foreignKeyConstraint.RelatedColumns);

					if (flags[ForeignKeyConstraintHasAcceptRejectRule]) writer.WriteOptimized((int) foreignKeyConstraint.AcceptRejectRule);
					if (flags[ForeignKeyConstraintHasNonCascadeDeleteRule]) writer.WriteOptimized((int) foreignKeyConstraint.DeleteRule);
					if (flags[ForeignKeyConstraintHasNonCascadeUpdateRule]) writer.WriteOptimized((int) foreignKeyConstraint.UpdateRule);
				
					if (flags[ForeignKeyConstraintHasSchemaName]) writer.WriteOptimized((string) ConstraintSchemaNameFieldInfo.GetValue(foreignKeyConstraint));
					if (flags[ForeignKeyConstraintHasExtendedProperties]) serializeExtendedProperties(foreignKeyConstraint.ExtendedProperties);
				
				}
			}
			
			private void serializeExtendedProperties(PropertyCollection properties)
			{
				object[] keys = new object[properties.Count];
				properties.Keys.CopyTo(keys, 0);
				writer.WriteOptimized(keys);
			
				object[] values = new object[properties.Count];
				properties.Values.CopyTo(values, 0);
				writer.WriteOptimized(values);
			
			}

			private void writeColumnOrdinals(DataColumn[] columns)
			{
				if (columns.Length == 1)
					writer.WriteOptimized(columns[0].Ordinal);
				else
				{
					int count = columns.Length;
					writer.WriteOptimized(count);
					foreach(DataColumn column in columns)
					{
						writer.WriteOptimized(column.Ordinal);
					}
				}
			}
		
			private void writeTableAndColumnOrdinals(DataColumn[] columns)
			{
				writer.WriteOptimized(dataSet.Tables.IndexOf(columns[0].Table));
				writeColumnOrdinals(columns);
			}
		
			private void serializeColumns(DataTable dataTable)
			{
				DataColumnCollection columns = dataTable.Columns;
				writer.WriteOptimized(columns.Count);
			
				foreach(DataColumn column in columns)
				{
					BitVector32 flags = GetColumnFlags(column);
					writer.WriteOptimized(flags);
				
					writer.WriteOptimized(column.ColumnName);
					if (flags[ColumnIsNotStringDataType]) writer.WriteOptimized(column.DataType);
					if (flags[ColumnHasExpression]) writer.WriteOptimized(column.Expression);
					if (flags[ColumnIsNotElementMappingType]) writer.WriteOptimized((int) column.ColumnMapping);
				
					if (!flags[ColumnHasAutoIncrementUnusedDefaults])
					{
						writer.WriteOptimized(Math.Abs(column.AutoIncrementSeed));
						writer.WriteOptimized(Math.Abs(column.AutoIncrementStep));
					}
					if (flags[ColumnHasCaption]) writer.WriteOptimized(column.Caption);
					if (flags[ColumnHasColumnUri]) writer.WriteOptimized((string) ColumnUriFieldInfo.GetValue(column));
					if (flags[ColumnHasPrefix]) writer.WriteOptimized(column.Prefix);
					if (flags[ColumnHasDefaultValue]) writer.WriteObject(column.DefaultValue);
					if (flags[ColumnHasMaxLength] && !flags[ColumnIsMaxLengthMaxValue]) writer.WriteOptimized(column.MaxLength);
					if (flags[ColumnHasExtendedProperties]) serializeExtendedProperties(column.ExtendedProperties);
				
				}

			}
			#endregion Private Methods
			
		}

		#endregion Serializer
		
		#region Deserializer
		private class FastDeserializer
		{
			#region Fields
			private DataSet dataSet;
			private SerializationReader reader;
			#endregion Fields
			
			#region Methods
			public DataTable DeserializeSimpleTypedDataTable(DataTable dataTable, byte[] serializedData)
			{
				reader = new SerializationReader(serializedData);
				
				int rowCount = reader.ReadOptimizedInt32();
				
				dataTable.BeginLoadData();
				for(int i = 0; i < rowCount; i++)
				{
					dataTable.LoadDataRow(reader.ReadOptimizedObjectArray(), true);
				}
				dataTable.EndLoadData();
				
				throwIfRemainingBytes();
				return dataTable;
			}
			
			public DataTable DeserializeDataTableDataOnly(DataTable dataTable, byte[] serializedData)
			{
				reader = new SerializationReader(serializedData);
				deserializeRows(dataTable);
				
				throwIfRemainingBytes();
				return dataTable;
			}
			
			public DataSet DeserializeDataSetDataOnly(DataSet dataSet, byte[] serializedData)
			{
				this.dataSet = dataSet;
				bool originalConstraintsSetting = dataSet.EnforceConstraints;
				dataSet.EnforceConstraints = false;
				
				reader = new SerializationReader(serializedData);
				
				int count = reader.ReadOptimizedInt32();
				for(int i = 0; i < count; i++)
				{
					deserializeRows(dataSet.Tables[i]);
				}

				dataSet.EnforceConstraints = originalConstraintsSetting;

				throwIfRemainingBytes();
				return dataSet;
			}
		
			public DataSet DeserializeDataSet(DataSet dataSet, byte[] serializedData)
			{
				this.dataSet = dataSet;
				reader = new SerializationReader(serializedData);

				dataSet.EnforceConstraints = false;
			
				BitVector32 flags = reader.ReadOptimizedBitVector32();
			
				if (flags[DataSetHasName]) dataSet.DataSetName = reader.ReadOptimizedString();

				dataSet.Locale = new CultureInfo(reader.ReadOptimizedInt32());
				dataSet.CaseSensitive = flags[DataSetIsCaseSensitive];
			
				if (flags[DataSetHasNamespace]) dataSet.Namespace = reader.ReadOptimizedString();
			
				if (flags[DataSetHasPrefix]) dataSet.Prefix = reader.ReadOptimizedString();
			
			
				if (flags[DataSetHasTables]) deserializeTables();
				if (flags[DataSetHasForeignKeyConstraints]) deserializeForeignKeyConstraints();

				if (flags[DataSetHasRelationships]) deserializeRelationships();
				if (flags[DataSetHasExtendedProperties]) deserializeExtendedProperties(dataSet.ExtendedProperties);
				
				dataSet.EnforceConstraints = flags[DataSetAreConstraintsEnabled];

				throwIfRemainingBytes();
				return dataSet;
			}
			
			public DataTable DeserializeDataTable(DataTable dataTable, byte[] serializedData)
			{
				reader = new SerializationReader(serializedData);
				deserializeTable(dataTable);

				throwIfRemainingBytes();
				return dataTable;
			}
			#endregion Methods
			
			#region Private Methods
			private void throwIfRemainingBytes()
			{
				if (reader.BytesRemaining != 0)
				{
					throw new InvalidOperationException(string.Format("FastDeserializer did not deserialize {0:n} bytes", reader.BytesRemaining));
				}
			}
			
			private void deserializeRelationships()
			{
				int count = reader.ReadOptimizedInt32();
				for(int i = 0; i < count; i++)
				{
					BitVector32 flags = reader.ReadOptimizedBitVector32();
					DataColumn[] parentColumns;
					DataColumn[] childColumns;
				
					string relationName = flags[RelationHasDefaultName] ? reader.ReadOptimizedString() : string.Empty;
					DataTable parentTable = dataSet.Tables[reader.ReadOptimizedInt32()];
				
					if (flags[RelationIsPrimaryKeyOnParentTable])
						parentColumns = parentTable.PrimaryKey;
					else
					{
						parentColumns = readColumnOrdinals(parentTable, flags[RelationHasMultipleColumns]);
					}
					
					childColumns = readTableAndColumnOrdinals(flags[RelationHasMultipleColumns]);

					DataRelation relation = new DataRelation(relationName, parentColumns, childColumns, false);
					relation.Nested = flags[RelationIsNested];
					if (flags[RelationHasExtendedProperties]) deserializeExtendedProperties(relation.ExtendedProperties);
				
					if (!dataSet.Relations.Contains(relation.RelationName)) dataSet.Relations.Add(relation);
				}
			
			}

			private void deserializeRows(DataTable dataTable)
			{
				ArrayList readOnlyColumns = null;
				int rowCount = reader.ReadOptimizedInt32();

				dataTable.BeginLoadData();
				for(int i = 0; i < rowCount; i++)
				{
					BitVector32 flags = reader.ReadOptimizedBitVector32();
					DataRow row;
				
					if (!flags[RowHasOldData])
						row = dataTable.LoadDataRow(reader.ReadOptimizedObjectArray(), !flags[RowHasNewData]);
					else if (!flags[RowHasNewData])
					{
						row = dataTable.LoadDataRow(reader.ReadOptimizedObjectArray(), true);
						row.Delete();
					} 
					else
					{
						// LoadDataRow doesn't care about ReadOnly columns but ItemArray does
						// Since only deserialization of Modified rows uses ItemArray we do this
						// only if a modified row is detected and just once
						if (readOnlyColumns == null)
						{
							readOnlyColumns = new ArrayList();
							foreach(DataColumn column in dataTable.Columns)
							{
								if (column.ReadOnly && column.Expression.Length == 0)
								{
									readOnlyColumns.Add(column);
									column.ReadOnly = false;
								}
							}
						}				
						
						object[] currentValues;
						object[] originalValues;
						reader.ReadOptimizedObjectArrayPair(out currentValues, out originalValues);
						row = dataTable.LoadDataRow(originalValues, true);
						row.ItemArray = currentValues;
					}

					if (flags[RowHasRowError]) row.RowError = reader.ReadOptimizedString();
					if (flags[RowHasColumnErrors])
					{
						int columnsInErrorCount = reader.ReadOptimizedInt32();
						for(int j = 0; j < columnsInErrorCount; j++)
						{
							row.SetColumnError(reader.ReadOptimizedInt32(), reader.ReadOptimizedString());
						}
					}
		
				}
				
				// Must restore ReadOnly columns if any were found when deserializing a Modified row
				if (readOnlyColumns != null && readOnlyColumns.Count != 0)
				{
					foreach(DataColumn column in readOnlyColumns)
					{
						column.ReadOnly = true;
					}
				}
				
				dataTable.EndLoadData();
				
			}
		
		
			private void deserializeUniqueConstraints(DataTable dataTable)
			{
				int count = reader.ReadOptimizedInt32();
				for(int i = 0; i < count; i++)
				{
					UniqueConstraint uniqueConstraint;
					BitVector32 flags;
					string constraintName = string.Empty;
					DataColumn[] dataColumns;

					flags = reader.ReadOptimizedBitVector32();
					if (!flags[UniqueConstraintHasDefaultName])
						constraintName = reader.ReadOptimizedString();
					else
					{
						constraintName = "Constraint" + reader.ReadOptimizedInt32();
					}
					dataColumns = readColumnOrdinals(dataTable, flags[UniqueConstraintHasMultipleColumns]);
					uniqueConstraint = new UniqueConstraint(constraintName, dataColumns, flags[UniqueConstraintIsPrimaryKey]);
				
					if (flags[UniqueConstraintHasSchemaName]) ConstraintSchemaNameFieldInfo.SetValue(uniqueConstraint, reader.ReadOptimizedString());
					if (flags[UniqueConstraintHasExtendedProperties]) deserializeExtendedProperties(uniqueConstraint.ExtendedProperties);

					if(!uniqueConstraint.Table.Constraints.Contains(uniqueConstraint.ConstraintName))
						uniqueConstraint.Table.Constraints.Add(uniqueConstraint);
				}
			}
		
		
			private void deserializeForeignKeyConstraints()
			{
				int count = reader.ReadOptimizedInt32();
				for(int i = 0; i < count; i++)
				{
					ForeignKeyConstraint foreignKeyConstraint;
					BitVector32 flags;
					string constraintName = string.Empty;
					DataColumn[] childColumns;
					DataColumn[] parentColumns;

					flags = reader.ReadOptimizedBitVector32();
					if (!flags[ForeignKeyConstraintHasDefaultName])
						constraintName = reader.ReadOptimizedString();
					else
					{
						constraintName = "Constraint" + reader.ReadOptimizedInt32();
					}
					childColumns = readTableAndColumnOrdinals(flags[ForeignKeyConstraintHasMultipleColumns]);
				
					DataTable relatedTable = dataSet.Tables[reader.ReadOptimizedInt32()];
					if (flags[ForeignKeyConstraintIsPrimaryKeyOnParentTable])
						parentColumns = relatedTable.PrimaryKey;
					else
					{
						parentColumns = readColumnOrdinals(relatedTable, flags[ForeignKeyConstraintHasMultipleColumns]);
					}

					AcceptRejectRule acceptRejectRule = flags[ForeignKeyConstraintHasAcceptRejectRule]
					                                    	? (AcceptRejectRule) reader.ReadOptimizedInt32()
					                                    	: AcceptRejectRule.None;
					Rule deleteRule = flags[ForeignKeyConstraintHasNonCascadeDeleteRule] ? (Rule) reader.ReadOptimizedInt32() : Rule.Cascade;
					Rule updateRule = flags[ForeignKeyConstraintHasNonCascadeUpdateRule] ? (Rule) reader.ReadOptimizedInt32() : Rule.Cascade;
					
					foreignKeyConstraint = new ForeignKeyConstraint(constraintName, parentColumns, childColumns);
					foreignKeyConstraint.AcceptRejectRule = acceptRejectRule;
					foreignKeyConstraint.DeleteRule = deleteRule;
					foreignKeyConstraint.UpdateRule = updateRule;
				
					if (flags[ForeignKeyConstraintHasSchemaName]) ConstraintSchemaNameFieldInfo.SetValue(foreignKeyConstraint, reader.ReadOptimizedString());
					if (flags[ForeignKeyConstraintHasExtendedProperties]) deserializeExtendedProperties(foreignKeyConstraint.ExtendedProperties);

					if(!foreignKeyConstraint.Table.Constraints.Contains(foreignKeyConstraint.ConstraintName))
						foreignKeyConstraint.Table.Constraints.Add(foreignKeyConstraint);
				}
			}
		
			private DataColumn[] readColumnOrdinals(DataTable dataTable, bool multipleColumns)
			{
				int count = multipleColumns ? reader.ReadOptimizedInt32() : 1;
				DataColumn[] columns = new DataColumn[count];
				for(int i = 0; i < count; i++)
				{
					columns[i] = dataTable.Columns[reader.ReadOptimizedInt32()];
				}
				return columns;
			}
		
			private DataColumn[] readTableAndColumnOrdinals(bool multipleColumns)
			{
				DataTable dataTable = dataSet.Tables[reader.ReadOptimizedInt32()];
				return readColumnOrdinals(dataTable, multipleColumns);
			}
		
		
			private void deserializeColumns(DataTable dataTable)
			{
				int precreatedCount = dataTable.Columns.Count;			
				int count = reader.ReadOptimizedInt32();
			
				for(int i = 0; i < count; i++)
				{
					DataColumn column = null;
					string columnName;
					Type dataType;
					string expression;
					MappingType mappingType;

					BitVector32 flags = reader.ReadOptimizedBitVector32();
				
					columnName = reader.ReadOptimizedString();
					dataType = flags[ColumnIsNotStringDataType] ? reader.ReadOptimizedType() : typeof(string);
					expression = flags[ColumnHasExpression] ? reader.ReadOptimizedString() : string.Empty;
					mappingType = flags[ColumnIsNotElementMappingType] ? (MappingType) reader.ReadOptimizedInt32() : MappingType.Element;
					column = new DataColumn(columnName, dataType, expression, mappingType);
				
					column.AllowDBNull = flags[ColumnIsNullable];
				
					column.AutoIncrement = flags[ColumnHasAutoIncrement];
					
					if (flags[ColumnHasAutoIncrementUnusedDefaults])
					{
						if (flags[ColumnHasAutoIncrementNegativeStep])
						{
							column.AutoIncrementSeed = -1;
							column.AutoIncrementStep = -1;
						} 
						else
						{
							column.AutoIncrementSeed = 0;
							column.AutoIncrementStep = 1;
						}
					}
					else
					{
						long seed = reader.ReadOptimizedInt64();
						long step = reader.ReadOptimizedInt64();
						if (flags[ColumnHasAutoIncrementNegativeStep])
						{
							column.AutoIncrementSeed = -seed;
							column.AutoIncrementStep = -step;
						} 
						else
						{
							column.AutoIncrementSeed = seed;
							column.AutoIncrementStep = step;
						}
					}
				
					if (flags[ColumnHasCaption]) column.Caption = reader.ReadOptimizedString();
					if (flags[ColumnHasColumnUri]) ColumnUriFieldInfo.SetValue(column, reader.ReadOptimizedString());
					if (flags[ColumnHasPrefix]) column.Prefix = reader.ReadOptimizedString();
					if (flags[ColumnHasDefaultValue]) column.DefaultValue = reader.ReadObject();
					column.ReadOnly = flags[ColumnIsReadOnly];
					if (flags[ColumnHasMaxLength])
					{
						column.MaxLength = flags[ColumnIsMaxLengthMaxValue] ? int.MaxValue : reader.ReadOptimizedInt32();
					}
					if (flags[ColumnHasExtendedProperties]) deserializeExtendedProperties(column.ExtendedProperties);
				
					if (i >= precreatedCount) dataTable.Columns.Add(column);
				}

			}

			private void deserializeExtendedProperties(PropertyCollection properties)
			{
				object[] keys = reader.ReadOptimizedObjectArray();
				object[] values = reader.ReadOptimizedObjectArray();
				for(int i = 0; i < keys.Length; i++)
				{
					properties.Add(keys[i], values[i]);
				}
			}

			private void deserializeTables()
			{
				int precreatedTableCount = dataSet.Tables.Count;
				int count = reader.ReadOptimizedInt32();
			
				for(int i = 0; i < count; i++)
				{
					DataTable dataTable;
					if (i >= precreatedTableCount)
						dataTable = new DataTable();
					else
					{
						dataTable = dataSet.Tables[i];
					}
				
					deserializeTable(dataTable);

					if (!dataSet.Tables.Contains(dataTable.TableName)) dataSet.Tables.Add(dataTable);
				}
			
			}

			private void deserializeTable(DataTable dataTable) 
			{

				#region Flags
				BitVector32 flags = reader.ReadOptimizedBitVector32();
			
				dataTable.TableName = (flags[TableHasName]) ? reader.ReadOptimizedString() : string.Empty;
			
				if (flags[TableHasNamespace]) dataTable.Namespace = reader.ReadOptimizedString();
				if (flags[TableHasPrefix]) dataTable.Prefix = reader.ReadOptimizedString();
				dataTable.CaseSensitive = flags[TableIsCaseSensitive];
				TableCaseSensitiveAmbientFieldInfo.SetValue(dataTable, !flags[TableIsCaseSensitiveAmbient]);
				if (flags[TableHasSpecificCulture]) dataTable.Locale = new CultureInfo(reader.ReadOptimizedInt32());
				if (flags[TableHasTypeName])
				{
					TableTypeNameFieldInfo.SetValue(dataTable, new XmlQualifiedName(reader.ReadOptimizedString(), reader.ReadOptimizedString()));
				}
				TableRepeatableElementFieldInfo.SetValue(dataTable, flags[TableHasRepeatableElement]);
				if (flags[TableHasMinimumCapacity]) dataTable.MinimumCapacity = reader.ReadOptimizedInt32();
				#endregion Flags

				#region Columns
				if (flags[TableHasColumns])
				{
					deserializeColumns(dataTable);
					if (flags[TableHasDisplayExpression]) dataTable.DisplayExpression = reader.ReadOptimizedString();
				}
				#endregion Columns
				
				#region Extended Properties
				if (flags[TableHasExtendedProperties]) deserializeExtendedProperties(dataTable.ExtendedProperties);
				#endregion Extended Properties
				
				#region Rows
				if (flags[TableHasRows]) deserializeRows(dataTable);
				#endregion Rows
			
				#region Unique Constraints
				if (!flags[TableHasNoUniqueConstraints])
				{
					deserializeUniqueConstraints(dataTable);
				}
				#endregion Unique Constraints
			
			}
			#endregion Private Methods
		}
		#endregion Deserializer
		#endregion Fast Serialization

	}

	/// <summary>
	/// Marker interface to signify that although the item is a Typed DataSet, it
	/// should be serialized as a plain DataSet since additional tables or columns
	/// may have been added to its schema
	/// </summary>
	public interface IModifiedTypedDataSet {}
	
}
