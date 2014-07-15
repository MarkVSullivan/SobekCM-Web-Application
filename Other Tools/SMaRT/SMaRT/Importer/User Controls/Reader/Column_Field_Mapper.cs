using System;
using System.Collections;

namespace SobekCM.Management_Tool.Importer
{
	/// <summary>
	/// Summary description for Column_Field_Mapper.
	/// </summary>
	public class Column_Field_Mapper
	{
		private Hashtable Columns_To_Field_Hash;

		public Column_Field_Mapper()
		{
			Columns_To_Field_Hash = new Hashtable();
		}

		public bool isMapped( int Column_Index )
		{
			return Columns_To_Field_Hash.Contains( Column_Index );
		}

        public void Add(int Column_Index, SobekCM.Resource_Object.Mapped_Fields Field)
		{
			Columns_To_Field_Hash[ Column_Index ] = new Column_Field_Map( Column_Index, Field );
		}

		public Column_Field_Map Get_Field( int Column_Index )
		{
			return ( Column_Field_Map ) Columns_To_Field_Hash[ Column_Index ];
		}
	}
}
