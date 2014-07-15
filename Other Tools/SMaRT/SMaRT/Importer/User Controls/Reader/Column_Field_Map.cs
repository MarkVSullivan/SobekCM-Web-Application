using System;
using SobekCM.Resource_Object;

namespace SobekCM.Management_Tool.Importer
{
	/// <summary>
	/// Summary description for Column_Field_Map.
	/// </summary>
	public class Column_Field_Map
	{
		public readonly int Column_Index;
        public readonly SobekCM.Resource_Object.Mapped_Fields Field;

        public Column_Field_Map(int Column_Index, SobekCM.Resource_Object.Mapped_Fields Field)
		{
			// Save these values
			this.Column_Index = Column_Index;
			this.Field = Field;
		}
	}
}
