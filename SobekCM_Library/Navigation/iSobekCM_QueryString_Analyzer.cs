#region Using directives

using System.Collections.Generic;
using System.Collections.Specialized;
using SobekCM.Library.Application_State;

#endregion

namespace SobekCM.Library.Navigation
{
    /// <summary> iSobekCM_QueryString_Analyzer is the interface which any query string
	/// analyzer must implement. <br /> <br /> </summary>
	/// <remarks> Object written by Mark V Sullivan for the University of Florida. </remarks>
	public interface iSobekCM_QueryString_Analyzer
	{
		/// <summary> Parse the query and set the internal variables </summary>
		/// <param name="QueryString"> QueryString collection passed from the main page </param>
		/// <param name="Navigator"> Navigation object to hold the mode information </param>
        /// <param name="Requested_URL">Requested URL</param>
        /// <param name="User_Languages"> Languages preferred by user, per their browser settings </param>
        /// <param name="Code_Manager"> List of valid collection codes, including mapping from the Sobek collections to Greenstone collections </param>
        /// <param name="Aggregation_Aliases"> List of all existing aliases for existing aggregations</param>
        /// <param name="All_Items_Lookup"> [REF] Lookup object used to pull basic information about any item loaded into this library</param>
        /// <param name="URL_Portals"> List of all web portals into this system </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		void Parse_Query( NameValueCollection QueryString, 
            SobekCM_Navigation_Object Navigator, 
            string Requested_URL,
            string[] User_Languages,
            Aggregation_Code_Manager Code_Manager,
            Dictionary<string, string> Aggregation_Aliases,
            ref Item_Lookup_Object All_Items_Lookup,
            Portal_List URL_Portals,
            Custom_Tracer Tracer
        );
	}
}


