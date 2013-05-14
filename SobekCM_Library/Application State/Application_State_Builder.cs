#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Database;
using SobekCM.Library.Settings;
using SobekCM.Library.Skins;

#endregion

namespace SobekCM.Library.Application_State
{
	/// <summary> Builds the application state and global values from database queries </summary>
    /// <remarks> These methods are used the first time the application launches and every fifteen minutes to refresh some values </remarks>
	public class Application_State_Builder
	{       

        /// <summary> Refreshes the item list and aggregation/greenstone code information from the database on a regular basis </summary>
        /// <param name="Code_Manager"> [REF] List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="All_Items_Lookup"> [REF] Lookup object used to pull basic information about any item loaded into this library </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool Refresh_Item_List_And_Aggregation_Codes(ref Aggregation_Code_Manager Code_Manager, ref Item_Lookup_Object All_Items_Lookup )
        {
            try
            {
                // Load SobekCM Codes to Greenstone Codes conversion again
                if (Code_Manager != null)
                {
                    lock (Code_Manager)
                    {
                        SobekCM_Database.Populate_Code_Manager(Code_Manager, null);
                    }
                }
                else
                {
                    Code_Manager = new Aggregation_Code_Manager();
                    SobekCM_Database.Populate_Code_Manager(Code_Manager, null);
                }

                //lock (item_list_lock_object)
                //{
                //    if (All_Items_Lookup == null)
                //        All_Items_Lookup = new Item_Lookup_Object();

                //    // Have the database popoulate the little bit of bibid/vid information we retain
                //    SobekCM_Database.Populate_Item_Lookup_Object(true, All_Items_Lookup, null);
                //}

                return true;
            }
            catch
            {
                return false;
            }
        }
    
        /// <summary> Verifies that each global object is built and builds them upon request </summary>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <param name="Reload_All"> Flag indicates if everything should be reloaded/repopulated</param>
        /// <param name="Skins"> [REF] Collection of all the web skins </param>
        /// <param name="Translator"> [REF] Language support object which handles simple translational duties </param>
        /// <param name="Code_Manager"> [REF] List of valid collection codes, including mapping from the Sobek collections to Greenstone collections</param>
        /// <param name="All_Items_Lookup"> [REF] Lookup object used to pull basic information about any item loaded into this library </param>
        /// <param name="Icon_Dictionary"> [REF] Dictionary of information about every wordmark/icon in this digital library </param>
        /// <param name="Stats_Date_Range"> [REF] Object contains the start and end dates for the statistical data in the database </param>
        /// <param name="Thematic_Headings"> [REF] Headings under which all the highlighted collections on the main home page are organized </param>
        /// <param name="Aggregation_Aliases"> [REF] List of all existing aliases for existing aggregations </param>
        /// <param name="IP_Restrictions"> [REF] List of all IP Restriction ranges in use by this digital library </param>
        /// <param name="URL_Portals"> [REF] List of all web portals into this system </param>
        /// <param name="Mime_Types">[REF] Dictionary of MIME types by extension</param>
        public static void Build_Application_State(Custom_Tracer Tracer, bool Reload_All,
            ref SobekCM_Skin_Collection Skins, ref Language_Support_Info Translator,
            ref Aggregation_Code_Manager Code_Manager, ref Item_Lookup_Object All_Items_Lookup,
            ref Dictionary<string, Wordmark_Icon> Icon_Dictionary, 
            ref Statistics_Dates Stats_Date_Range, 
            ref List<Thematic_Heading> Thematic_Headings,
            ref Dictionary<string, string> Aggregation_Aliases, 
            ref IP_Restriction_Ranges IP_Restrictions,
            ref Portal_List URL_Portals,
            ref Dictionary<string, Mime_Type_Info> Mime_Types)
		{
            // Should we reload the data from the exteral configuraiton file?
            if (Reload_All)
            {
                SobekCM_Library_Settings.Read_Configuration_File();
                SobekCM_Database.Connection_String = SobekCM_Library_Settings.Database_Connection_String;
                SobekCM_Library_Settings.Refresh(SobekCM_Database.Get_Settings_Complete(null));
            }

            // If there is no database connection string, there is a problem
            if (String.IsNullOrEmpty(SobekCM_Library_Settings.Database_Connection_String))
            {
                throw new ApplicationException("Missing database connection string!");
            }

            // Set the database connection strings
            Resource_Object.Database.SobekCM_Database.Connection_String = SobekCM_Library_Settings.Database_Connection_String;
            SobekCM_Database.Connection_String = SobekCM_Library_Settings.Database_Connection_String;

            // Set the workflow and disposition types
            if ((SobekCM_Library_Settings.Need_Workflow_And_Disposition_Types) || ( Reload_All ))
            {
                SobekCM_Library_Settings.Set_Workflow_And_Disposition_Types(SobekCM_Database.All_WorkFlow_Types, SobekCM_Database.All_Possible_Disposition_Types);
            }

            // Set the metadata types
            if ((SobekCM_Library_Settings.Need_Metadata_Types) || ( Reload_All ))
            {
                SobekCM_Library_Settings.Set_Metadata_Types(SobekCM_Database.Get_Metadata_Fields(null) );
            }

            // Set the search stop words
            if ((SobekCM_Library_Settings.Need_Search_Stop_Words) || (Reload_All))
            {
                SobekCM_Library_Settings.Search_Stop_Words = SobekCM_Database.Search_Stop_Words(Tracer);
            }
            
            // Check the list of thematic headings
            if ((Thematic_Headings == null) || (Reload_All))
            {
                if (Thematic_Headings != null)
                {
                    lock (Thematic_Headings)
                    {
                        if (!SobekCM_Database.Populate_Thematic_Headings(Thematic_Headings, Tracer))
                        {
                            Thematic_Headings = null;
                            throw SobekCM_Database.Last_Exception;
                        }
                    }
                }
                else
                {
                    Thematic_Headings = new List<Thematic_Heading>();
                    if (!SobekCM_Database.Populate_Thematic_Headings(Thematic_Headings, Tracer))
                    {
                        Thematic_Headings = null;
                        throw SobekCM_Database.Last_Exception;
                    }
                }
            }

            // Check the list of forwardings
            if ((Aggregation_Aliases == null) || (Reload_All))
            {
                if (Aggregation_Aliases != null)
                {
                    lock (Aggregation_Aliases)
                    {
                        SobekCM_Database.Populate_Aggregation_Aliases(Aggregation_Aliases, Tracer);
                    }
                }
                else
                {
                    Aggregation_Aliases = new Dictionary<string, string>();
                    SobekCM_Database.Populate_Aggregation_Aliases(Aggregation_Aliases, Tracer);
                }
            }

            // Check the list of constant skins
            if ((Skins == null) || (Skins.Count == 0) || (Reload_All))
            {
                if (Skins != null)
                {
                    lock (Skins)
                    {
                        SobekCM_Skin_Collection_Builder.Populate_Default_Skins(Skins, Tracer);
                    }
                }
                else
                {
                    Skins = new SobekCM_Skin_Collection();
                    SobekCM_Skin_Collection_Builder.Populate_Default_Skins(Skins, Tracer);
                }
            }

            // Check the list of all web portals
            if ((URL_Portals == null) || (URL_Portals.Count == 0) || (Reload_All))
            {
                if (URL_Portals != null)
                {
                    lock (URL_Portals)
                    {
                        SobekCM_Database.Populate_URL_Portals(URL_Portals, Tracer);
                    }
                }
                else
                {
                    URL_Portals = new Portal_List();
                    SobekCM_Database.Populate_URL_Portals(URL_Portals, Tracer);
                }
            }
			
			// Check the translation table has been loaded
            if (( Translator == null) || (Reload_All))
			{
				// Get the translation hashes into memory
                if (Translator != null)
                {
                    lock (Translator)
                    {
                        SobekCM_Database.Populate_Translations(Translator, Tracer);
                    }
                }
                else
                {
                    Translator = new Language_Support_Info();
                    SobekCM_Database.Populate_Translations(Translator, Tracer);
                }
			}

			// Check that the conversion from SobekCM Codes to Greenstone Codes has been loaded
            if ((Code_Manager == null) || (Reload_All))
			{
                if (Code_Manager != null)
                {
                    lock (Code_Manager)
                    {
                        SobekCM_Database.Populate_Code_Manager(Code_Manager, Tracer);
                    }
                }
                else
                {
                    Code_Manager = new Aggregation_Code_Manager();
                    SobekCM_Database.Populate_Code_Manager(Code_Manager, Tracer);
                }
			}

            // Check the statistics date range information
            if ((Stats_Date_Range == null) || (Reload_All))
            {
                if (Stats_Date_Range != null)
                {
                    // Get the translation hashes into memory
                    lock (Stats_Date_Range)
                    {
                        SobekCM_Database.Populate_Statistics_Dates(Stats_Date_Range, Tracer);
                    }
                }
                else
                {
                    Stats_Date_Range = new Statistics_Dates();
                    SobekCM_Database.Populate_Statistics_Dates(Stats_Date_Range, Tracer);
                }
            }

            // Get the Icon list
            if ((Icon_Dictionary == null) || ( Reload_All ))
            {
                if (Icon_Dictionary != null)
                {
                    // Get the translation hashes into memory
                    lock (Icon_Dictionary)
                    {
                        SobekCM_Database.Populate_Icon_List(Icon_Dictionary, Tracer);
                    }
                }
                else
                {
                    Icon_Dictionary = new Dictionary<string, Wordmark_Icon>();
                    SobekCM_Database.Populate_Icon_List(Icon_Dictionary, Tracer);
                }
            }

            // Check the list of ip range restrictions
            if ((IP_Restrictions == null) || (Reload_All))
            {
                if (IP_Restrictions != null)
                {
                    lock (IP_Restrictions)
                    {
                        DataTable ipRestrictionTbl = SobekCM_Database.Get_IP_Restriction_Ranges(Tracer);
                        if (ipRestrictionTbl != null)
                        {
                            IP_Restrictions.Populate_IP_Ranges(ipRestrictionTbl);
                        }
                    }
                }
                else
                {
                    DataTable ipRestrictionTbl = SobekCM_Database.Get_IP_Restriction_Ranges(Tracer);
                    if (ipRestrictionTbl != null)
                    {
                        IP_Restrictions = new IP_Restriction_Ranges();
                        IP_Restrictions.Populate_IP_Ranges(ipRestrictionTbl);
                    }
                }
            }


            // Get the MIME type list
            if ((Mime_Types == null) || (Reload_All))
            {
                if (Mime_Types != null)
                {
                    // Get the translation hashes into memory
                    lock (Mime_Types)
                    {
                        SobekCM_Database.Populate_MIME_List(Mime_Types, Tracer);
                    }
                }
                else
                {
                    Mime_Types = new Dictionary<string, Mime_Type_Info>();
                    SobekCM_Database.Populate_MIME_List(Mime_Types, Tracer);
                }
            }

		}
	}
}
