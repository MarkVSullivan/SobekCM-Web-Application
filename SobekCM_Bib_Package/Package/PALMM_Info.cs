using System;

namespace SobekCM.Bib_Package
{
	/// <summary> Stores information about PALMM associated with this resource </summary>
	/// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
	public class PALMM_Info
	{
		private bool topalmm;
		private string palmm_project;
		private string palmm_type;
		private string palmm_server;

		/// <summary> Constructor creates a new instance of the PALMM_Info class </summary>
		public PALMM_Info()
		{
			topalmm = false;
		}

		/// <summary> Sets many of the PALMM values, based on the type of material </summary>
		/// <param name="type"></param>
   		public void Set_Values( string type )
		{
			string temp_palmm_type = "monograph";
			string temp_palmm_server = "TC";
		
			if ( type.Length > 0 )
			{
				switch( type.Trim().ToUpper() )
				{
					case "TEXT":
					case "BOOK":
						temp_palmm_type = "monograph";
						temp_palmm_server = "TC";
						break;

					case "IMAGE":
						temp_palmm_type = "photo";
						temp_palmm_server = "IC";
						break;

					case "SERIAL":
					case "NEWSPAPER":
						temp_palmm_type = "serial";
						temp_palmm_server = "IC";
						break;
				}
			}

            if (String.IsNullOrEmpty(palmm_type))
				palmm_type = temp_palmm_type;
            if (String.IsNullOrEmpty(palmm_server))
				palmm_server = temp_palmm_server;
		}

		/// <summary> Gets and sets the flag which indicates this resource should be loaded on PALMM </summary>
		public bool toPALMM
		{
			get	{	return topalmm;		}
			set	{	topalmm = value;	}
		}

		/// <summary> Gets and sets the PALMM project associated with this resource </summary>
		public string PALMM_Project
		{
			get	{	return palmm_project ?? String.Empty;		}
			set	{	palmm_project = value;		}
		}

		/// <summary> Gets and sets the PALMM-compliant type for this resource </summary>
		public string PALMM_Type
		{
            get { return palmm_type ?? String.Empty; }
			set	{	palmm_type = value;		}
		}

		/// <summary> Gets and sets the PALMM target server (i.e. IC, TC, etc...)</summary>
		public string PALMM_Server
		{
            get { return palmm_server ?? String.Empty; }
			set	{	palmm_server = value;		}
		}
	}
}
