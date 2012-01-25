using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.MARC
{
    /// <summary> Holds the data about a single subfield in a <see cref="MARC_Field"/>. <br /> <br /> </summary>
    public class MARC_Subfield
    {
        private char subfield_code;
        private string data;

        /// <summary> Constructor for a new instance the MARC_Subfield class </summary>
		/// <param name="Subfield_Code"> Code for this subfield in the MARC record </param>
		/// <param name="Data"> Data stored for this subfield </param>
        public MARC_Subfield(char Subfield_Code, string Data)
		{
			// Save the parameters
			this.subfield_code = Subfield_Code;
			this.data = Data;
		}

		/// <summary> Gets the MARC subfield code associated with this data  </summary>
		public char Subfield_Code
		{
			get
			{
				return subfield_code;
			}
		}

		/// <summary> Gets the data associated with this MARC subfield  </summary>
		public string Data
		{
			get
			{
				return data;
			}
			set
			{
				data = value;
			}
		}

		/// <summary> Gets the data associated with this MARC subfield  </summary>
		public string Data_XML
		{
			get
			{
				string xml_safe = data;
				int i = xml_safe.IndexOf("&");
				while ( i >= 0 )
				{
					if (( i != xml_safe.IndexOf("&amp;", i )) && ( i != xml_safe.IndexOf("&quot;", i )) && 
						( i != xml_safe.IndexOf("&gt;", i )) && ( i != xml_safe.IndexOf("&lt;", i )))
					{
						xml_safe = xml_safe.Substring( 0, i + 1 ) + "amp;" + xml_safe.Substring( i + 1 );
					}

					i = xml_safe.IndexOf("&", i + 1 );
				}
				return xml_safe.Replace("<","&lt;").Replace(">","&gt;").Replace("\"","&quot;").Replace("[","").Replace("]","");
			}
		}


    }
}
