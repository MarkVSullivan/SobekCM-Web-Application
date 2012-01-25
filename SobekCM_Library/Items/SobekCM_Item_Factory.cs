#region Using directives

using System;
using System.Collections.Generic;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Divisions;
using SobekCM.Library.Application_State;

#endregion

namespace SobekCM.Library.Items
{
	/// <summary> Static class is used to build item and page objects for display in SobekCM </summary>
    /// <remarks> This class relies heavily upon the <see cref="SobekCM_METS_Based_ItemBuilder"/> class to actually
    /// read the METS files and build the items. </remarks>
    public class SobekCM_Item_Factory
	{
	    /// <summary> Builds a digital resource object for the given BibID and VID </summary>
        /// <param name="BibID"> Bibliographic identifier for the digital resource to build </param>
        /// <param name="VID"> Volume identifier for the digital resource to builder </param>
        /// <param name="Icon_Dictionary"> Dictionary of information about every wordmark/icon in this digital library, used to build the HTML for the icons linked to this digital resource</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Fully built digital resource object for a single volume </returns>
        /// <remarks> This method relies heavily upon the <see cref="SobekCM_METS_Based_ItemBuilder"/> class to actually
        /// read the METS files and build the items. </remarks>
        public static SobekCM_Item Get_Item(string BibID, string VID, 
            Dictionary<string, Wordmark_Icon> Icon_Dictionary, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Item_Factory.Get_Item", "Build this item");
            }

            // Get the METS based item builder
            SobekCM_METS_Based_ItemBuilder builder = new SobekCM_METS_Based_ItemBuilder();

            // Allow the builder to build the item
            return builder.Build_Item( BibID, VID, Icon_Dictionary, Tracer);
        }

        /// <summary> Builds a title-level digital resource object for the given BibID </summary>
        /// <param name="BibID"> Bibliographic identifier for the digital resource to build </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <param name="Item_Group_Object"> [OUT] Fully built item group object </param>
        /// <param name="Items_In_Title"> [OUT] List of all the items in this title </param>
        /// <remarks> This method relies heavily upon the <see cref="SobekCM_METS_Based_ItemBuilder"/> class to actually
        /// read the METS files and build the items. </remarks>
        public static void Get_Item_Group(string BibID, Custom_Tracer Tracer, out SobekCM_Items_In_Title Items_In_Title, out SobekCM_Item Item_Group_Object)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Item_Factory.Get_Item_Group", "Build this item group");
            }

            // Get the METS based item builder
            SobekCM_METS_Based_ItemBuilder builder = new SobekCM_METS_Based_ItemBuilder();

            // Allow the builder to build the item
            builder.Build_Item_Group(BibID, Tracer, out Items_In_Title, out Item_Group_Object );
        }

        /// <summary> Gets a page from an existing digital resource, by page sequence </summary>
        /// <param name="Current_Item"> Digital resource from which to pull the current page, by sequence </param>
        /// <param name="Sequence"> Sequence for the page to retrieve from this item </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <returns> Page tree node object for the requested page </returns>
        public static Page_TreeNode Get_Current_Page(SobekCM_Item Current_Item, int Sequence, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("SobekCM_Item_Factory.Get_Current_Page", "Requesting the page (by sequence) from the item");
            }

            Page_TreeNode returnValue = null;

            try
            {
                // Set the current page
                if (Sequence >= 1)
                {
                    int requested_page = Sequence - 1;
                    if ((requested_page < 0) || (requested_page > Current_Item.SobekCM_Web.Static_PageCount - 1))
                        requested_page = 0;

                    if (requested_page <= Current_Item.SobekCM_Web.Static_PageCount - 1)
                    {
                        returnValue = Current_Item.SobekCM_Web.Pages_By_Sequence[requested_page];
                    }
                }
            }
            catch (Exception ee)
            {
                throw new ApplicationException("Error assigning the current page sequence", ee);
            }

            return returnValue;
        }
	}
}
