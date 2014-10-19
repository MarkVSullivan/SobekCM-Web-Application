#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SobekCM.Core.Configuration;

#endregion

namespace SobekCM.Core.ApplicationState
{
	/// <summary> Class stores all the common translations used in the web interface </summary>
	/// <remarks> This is being deprecated </remarks>
	[DataContract]
	public class Language_Support_Info
	{
	    /// <summary> Constructor for a new instance of the Language_Support_Info class </summary>
		public Language_Support_Info()
		{
			// Declare the hashtables
            FrenchTable = new Dictionary<string, string>(100);
            SpanishTable = new Dictionary<string, string>(100);
		}

        /// <summary> Table of translations from english to french </summary>
        [DataMember]
        public Dictionary<string, string> FrenchTable { get; set; }

        /// <summary> Table of translations from english to spanish </summary>
        [DataMember]
        public Dictionary<string, string> SpanishTable { get; set; }

        /// <summary> Clears all the data stored in this object </summary>
        public void Clear()
        {
            FrenchTable.Clear();
            SpanishTable.Clear();
        }

        /// <summary> Add a spanish translation to the translation dictionary </summary>
        /// <param name="English"> Term in english </param>
        /// <param name="Spanish"> Term in spanish </param>
		public void Add_Spanish( string English, string Spanish )
		{
			SpanishTable[ English ] = Spanish;
		}

        /// <summary> Add a french translation to the translation dictionary </summary>
        /// <param name="English"> Term in english </param>
        /// <param name="French"> Term in french </param>
		public void Add_French( string English, string French )
		{
			FrenchTable[ English ] = French;
		}

        /// <summary> Gets the spanish translation of an english term </summary>
        /// <param name="English"> Term in english </param>
        /// <returns> Spanish translation from dictionary, or the same english term if the term does not exist </returns>
		public string Get_Spanish( string English )
        {
            return SpanishTable.ContainsKey( English ) ? SpanishTable[ English ] : English;
        }

	    /// <summary> Gets the french translation of an english term </summary>
        /// <param name="English"> Term in english </param>
        /// <returns> French translation from dictionary, or the same english term if the term does not exist </returns>
		public string Get_French( string English )
	    {
	        return FrenchTable.ContainsKey(English) ? FrenchTable[ English ] : English;
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
