#region Using directives

using System;
using System.Data;
using System.Data.Common;

#endregion

namespace EngineAgnosticLayerDbAccess
{
    /// <summary> Database parameter for the Engine Agnostic Layer database access </summary>
    public sealed class EalDbParameter : DbParameter
    {
        /// <summary> Constructor for a new Engine Agnostic Layer database parameter </summary>
        public EalDbParameter()
        {
            DbType = DbType.String;
            Direction = ParameterDirection.Input;
        }

        /// <summary> Constructor for a new Engine Agnostic Layer database parameter </summary>
        /// <param name="ParameterName"> Name of this parameter </param>
        public EalDbParameter(string ParameterName) 
        {
            DbType = DbType.String;
            Direction = ParameterDirection.Input;
            this.ParameterName = ParameterName;
        }

        /// <summary> Constructor for a new Engine Agnostic Layer database parameter </summary>
        /// <param name="ParameterName"> Name of this parameter </param>
        /// <param name="ParamType"></param>
        public EalDbParameter(string ParameterName, DbType ParamType)
        {
            DbType = ParamType;
            Direction = ParameterDirection.Input;
            this.ParameterName = ParameterName;
        }

        /// <summary> Constructor for a new Engine Agnostic Layer database parameter </summary>
        /// <param name="ParameterName"> Name of this parameter </param>
        /// <param name="Value"> String value of this parameter </param>
        public EalDbParameter(string ParameterName, string Value)
        {
            DbType = DbType.String;
            Direction = ParameterDirection.Input;
            this.ParameterName = ParameterName;
            this.Value = Value;
        }

        /// <summary> Constructor for a new Engine Agnostic Layer database parameter </summary>
        /// <param name="ParameterName"> Name of this parameter </param>
        /// <param name="Value"> Integer value of this parameter </param>
        public EalDbParameter(string ParameterName, int Value)
        {
            DbType = DbType.Int32;
            Direction = ParameterDirection.Input;
            this.ParameterName = ParameterName;
            this.Value = Value;
        }

        /// <summary> Constructor for a new Engine Agnostic Layer database parameter </summary>
        /// <param name="ParameterName"> Name of this parameter </param>
        /// <param name="Value"> DateTime value of this parameter </param>
        public EalDbParameter(string ParameterName, DateTime Value)
        {
            DbType = DbType.DateTime;
            Direction = ParameterDirection.Input;
            this.ParameterName = ParameterName;
            this.Value = Value;
        }

        /// <summary> Constructor for a new Engine Agnostic Layer database parameter </summary>
        /// <param name="ParameterName"> Name of this parameter </param>
        /// <param name="Value"> Boolean value of this parameter </param>
        public EalDbParameter(string ParameterName, bool Value)
        {
            DbType = DbType.Boolean;
            Direction = ParameterDirection.Input;
            this.ParameterName = ParameterName;
            this.Value = Value;
        }

        /// <summary> Constructor for a new Engine Agnostic Layer database parameter </summary>
        /// <param name="ParameterName"> Name of this parameter </param>
        /// <param name="Value"> Value of this parameter </param>
        public EalDbParameter(string ParameterName, object Value)
        {
            DbType = DbType.String;
            Direction = ParameterDirection.Input;
            this.ParameterName = ParameterName;
            this.Value = Value;
        }

        /// <summary> Resets the DbType property to its original settings </summary>
        public override void ResetDbType()
        {
            // Not fully implemented
        }

        /// <summary> Gets or sets the DbType of the parameter </summary>
        public override DbType DbType { get; set; }

        /// <summary> Gets or sets a value that indicates whether the parameter is input-only, output-only, bidirectional, or a stored procedure return value parameter </summary>
        public override ParameterDirection Direction { get; set; }

        /// <summary> Gets or sets a value that indicates whether the parameter accepts null values </summary>
        public override bool IsNullable { get; set; }

        /// <summary> Gets or sets the name of the DbParameter </summary>
        public override string ParameterName { get; set; }

        /// <summary> Gets or sets the name of the source column mapped to the DataSet and used for loading or returning the Value </summary>
        public override string SourceColumn { get; set; }

        /// <summary> Gets or sets the DataRowVersion to use when you load Value </summary>
        public override DataRowVersion SourceVersion { get; set; }

        /// <summary> Gets or sets the value of the parameter </summary>
        public override object Value { get; set; }

        /// <summary> Sets or gets a value which indicates whether the source column is nullable </summary>
        public override bool SourceColumnNullMapping { get; set; }

        /// <summary> Gets or sets the maximum size, in bytes, of the data within the column </summary>
        public override int Size { get; set; }
    }
}
