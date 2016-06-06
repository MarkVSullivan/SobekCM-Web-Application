using System;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the hierarchical spatial subjects from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Hierarchical_Spatial_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            if (Original.Bib_Info.Subjects_Count > 0)
            {
                int spatial_count = 1;
                foreach (Subject_Info thisSubject in Original.Bib_Info.Subjects)
                {
                    if (thisSubject.Class_Type == Subject_Info_Type.Hierarchical_Spatial)
                    {
                        // Determine the term to use
                        string term = "Hierarchical Spatial";
                        if (spatial_count > 1)
                            term = term + " (" + spatial_count + ")";

                        // Start to build this
                        BriefItem_DescriptiveTerm thisTerm = new BriefItem_DescriptiveTerm(term);

                        // Cast to the hierarchical geographic subject
                        Subject_Info_HierarchicalGeographic hieroSubj = (Subject_Info_HierarchicalGeographic)thisSubject;

                        // Now, step through and add each subterm
                        if (!String.IsNullOrWhiteSpace(hieroSubj.Continent)) thisTerm.Add_Value(hieroSubj.Continent, "Continent");
                        if (!String.IsNullOrWhiteSpace(hieroSubj.Country)) thisTerm.Add_Value(hieroSubj.Country, "Country");
                        if (!String.IsNullOrWhiteSpace(hieroSubj.Province)) thisTerm.Add_Value(hieroSubj.Province, "Province");
                        if (!String.IsNullOrWhiteSpace(hieroSubj.Region)) thisTerm.Add_Value(hieroSubj.Region, "Region");
                        if (!String.IsNullOrWhiteSpace(hieroSubj.State)) thisTerm.Add_Value(hieroSubj.State, "State");
                        if (!String.IsNullOrWhiteSpace(hieroSubj.Territory)) thisTerm.Add_Value(hieroSubj.Territory, "Territory");
                        if (!String.IsNullOrWhiteSpace(hieroSubj.County)) thisTerm.Add_Value(hieroSubj.County, "County");
                        if (!String.IsNullOrWhiteSpace(hieroSubj.City)) thisTerm.Add_Value(hieroSubj.City, "City");
                        if (!String.IsNullOrWhiteSpace(hieroSubj.CitySection)) thisTerm.Add_Value(hieroSubj.CitySection, "City Section");
                        if (!String.IsNullOrWhiteSpace(hieroSubj.Island)) thisTerm.Add_Value(hieroSubj.Island, "Island");
                        if (!String.IsNullOrWhiteSpace(hieroSubj.Area)) thisTerm.Add_Value(hieroSubj.Area, "Area");

                        // Were some values found?
                        if ((thisTerm.Values != null) && (thisTerm.Values.Count > 0))
                        {
                            New.Add_Description(thisTerm);
                            spatial_count++;
                        }
                    }
                }
            }

            return true;
        }
    }
}
