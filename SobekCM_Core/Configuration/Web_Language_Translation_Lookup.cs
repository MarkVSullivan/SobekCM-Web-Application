#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace SobekCM.Core.Configuration
{
    /// <summary> Class encapsulates a simple dictionary with a default value for classes that
    /// contain a simple language lookup ( i.e., what is the query related to this object in XX language? ) </summary>
    public class Web_Language_Translation_Lookup
    {
        private readonly Dictionary<Web_Language_Enum, string> translationLookupObj;

        /// <summary> Create a new instance of the Web_Language_Translation_Lookup class </summary>
        public Web_Language_Translation_Lookup()
        {
            translationLookupObj = new Dictionary<Web_Language_Enum, string>();
        }

        /// <summary> Default value used if the requested language is not present </summary>
        public string DefaultValue { get; set;  }

        /// <summary> Return the number of values within this lookup object </summary>
        /// <remarks> If the default value exists and is also added under the default language, this will
        /// return the value 2.  No checks are made to ensure the value in the lookup object is not the
        /// same as the default value.  That is, this does not check for UNIQUE values, just values.</remarks>
        public int Count
        {
            get
            {
                if (!String.IsNullOrEmpty(DefaultValue))
                    return translationLookupObj.Count + 1;
                else
                    return translationLookupObj.Count;
            }
        }

        /// <summary> Gets the list of translated values with the web language </summary>
        public List<Web_Language_Translation_Value> Values
        {
            get
            {
                return translationLookupObj.Keys.Select(ThisLanguage => new Web_Language_Translation_Value(ThisLanguage, translationLookupObj[ThisLanguage])).ToList();
            }
        }

        /// <summary> Add a translation value for a specific language </summary>
        /// <param name="Language"> Language for the provided value </param>
        /// <param name="Value"> String value for provided language </param>
        public void Add_Translation(Web_Language_Enum Language, string Value)
        {
            translationLookupObj[Language] = Value;
        }

        /// <summary> Gets the value for a provided language </summary>
        /// <param name="Language"> Language to attempt to find in this translation lookup object </param>
        /// <returns> Either a value, or "No Value" string </returns>
        public string Get_Value(Web_Language_Enum Language)
        {
            if (translationLookupObj.ContainsKey(Language))
                return translationLookupObj[Language];

            if (!String.IsNullOrEmpty(DefaultValue))
                return DefaultValue;

            if (translationLookupObj.Count > 0)
                return translationLookupObj[translationLookupObj.Keys.ElementAt(0)];

            return "No Value";
        }
    }
}
