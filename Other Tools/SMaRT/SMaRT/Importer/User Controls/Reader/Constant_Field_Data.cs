using System;
using SobekCM.Resource_Object;

namespace SobekCM.Management_Tool.Importer
{
	/// <summary>
	/// Summary description for Constant_Field_Data.
	/// </summary>
	public class Constant_Field_Data
	{
        public readonly SobekCM.Resource_Object.Mapped_Fields Field;
		public readonly string Data;

        public Constant_Field_Data(SobekCM.Resource_Object.Mapped_Fields Field, string Data)
		{
			// Save the parameters
			this.Data = Data;
			this.Field = Field;
		}
	}
}
