#region Using directives

using System;
using System.Collections.Generic;
using SobekCM.Library.Configuration;

#endregion

namespace SobekCM.Library.Application_State
{
	/// <summary> Class stores all the common translations used in the web interface </summary>
	public class Language_Support_Info
	{
		private readonly Dictionary<string,string> frenchTable;
	    private readonly Dictionary<string,string> spanishTable;

	    /// <summary> Constructor for a new instance of the Language_Support_Info class </summary>
		public Language_Support_Info()
		{
			// Declare the hashtables
            frenchTable = new Dictionary<string, string>(100);
            spanishTable = new Dictionary<string, string>(100);
		}

        /// <summary> Clears all the data stored in this object </summary>
        public void Clear()
        {
            frenchTable.Clear();
            spanishTable.Clear();
        }

        /// <summary> Add a spanish translation to the translation dictionary </summary>
        /// <param name="English"> Term in english </param>
        /// <param name="Spanish"> Term in spanish </param>
		public void Add_Spanish( string English, string Spanish )
		{
			spanishTable[ English ] = Spanish;
		}

        /// <summary> Add a french translation to the translation dictionary </summary>
        /// <param name="English"> Term in english </param>
        /// <param name="French"> Term in french </param>
		public void Add_French( string English, string French )
		{
			frenchTable[ English ] = French;
		}

        /// <summary> Gets the spanish translation of an english term </summary>
        /// <param name="English"> Term in english </param>
        /// <returns> Spanish translation from dictionary, or the same english term if the term does not exist </returns>
		public string Get_Spanish( string English )
        {
            return spanishTable.ContainsKey( English ) ? spanishTable[ English ] : English;
        }

	    /// <summary> Gets the french translation of an english term </summary>
        /// <param name="English"> Term in english </param>
        /// <returns> French translation from dictionary, or the same english term if the term does not exist </returns>
		public string Get_French( string English )
	    {
	        return frenchTable.ContainsKey(English) ? frenchTable[ English ] : English;
	    }

	    /// <summary> Generic method requests translation from the appropriate translation dictionary </summary>
        /// <param name="English"> Term in english </param>
        /// <param name="language"> Current language of the web interface </param>
        /// <returns> Translation of term, if it exists, otherwise the original term </returns>
        public string Get_Translation(string English, Web_Language_Enum language)
        {
            if (English.Length == 0)
                return String.Empty;

            switch (language)
            {
                case Web_Language_Enum.Spanish:
                    return Get_Spanish(English);

                case Web_Language_Enum.French:
                    return Get_French(English);

                default:
                    return English;
            }
        }
	}
}
