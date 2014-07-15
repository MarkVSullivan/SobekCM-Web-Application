using System;
using System.Collections;
using System.Collections.Generic;
using SobekCM.Resource_Object;

namespace SobekCM.Management_Tool.Importer
{
	/// <summary>
	/// Summary description for Constant_Fields.
	/// </summary>
	public class Constant_Fields : IEnumerator
	{
		public List<Constant_Field_Data> constantCollection;

		public Constant_Fields()
		{
			// Declare the collection of constant fields
            constantCollection = new List<Constant_Field_Data>();
		}

        public void Add(SobekCM.Resource_Object.Mapped_Fields Field, string Data)
		{
			constantCollection.Add( new Constant_Field_Data( Field, Data ));
		}       

        public SobekCM_Item Create_Package()
		{
			// Create a new bibliographic package
			SobekCM_Item newPackage = new SobekCM_Item();

			// Add constant fields
			Add_To_Package( newPackage );

			// Return the build package
			return newPackage;
		}

		public void Add_To_Package( SobekCM_Item Package )
		{
            // reset the static variables in the mappings class
            Bibliographic_Mapping.clear_static_variables();

			// Step through each constant data and add it
			foreach( Constant_Field_Data thisData in constantCollection )
			{
                Bibliographic_Mapping.Add_Data(Package, thisData.Data, thisData.Field);
			}
		}

		#region IEnumerator Members

			public void Reset()
		{
			// TODO:  Add Constant_Fields.Reset implementation
		}

		public object Current
		{
			get
			{
				// TODO:  Add Constant_Fields.Current getter implementation
				return null;
			}
		}

		public bool MoveNext()
		{
			// TODO:  Add Constant_Fields.MoveNext implementation
			return false;
		}

		#endregion
	}
}
