using System;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Core.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the titles from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Titles_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add the main title
            New.Add_Description("Title", (Original.Bib_Info.Main_Title.NonSort + " " + Original.Bib_Info.Main_Title.Title + " " + Original.Bib_Info.Main_Title.Subtitle).Trim());

            // Add all the "other titles"
            if (Original.Bib_Info.Other_Titles_Count > 0)
            {
                foreach (Title_Info thisTitle in Original.Bib_Info.Other_Titles)
                {
                    switch (thisTitle.Title_Type)
                    {
                        case Title_Type_Enum.UNSPECIFIED:
                        case Title_Type_Enum.alternative:
                            string titleType = thisTitle.Display_Label;
                            if ((!String.IsNullOrWhiteSpace(titleType)) && ( String.Compare(titleType, "OTHER TITLE", StringComparison.InvariantCultureIgnoreCase ) != 0 ))
                                New.Add_Description("Alternative Title", (thisTitle.NonSort + " " + thisTitle.Title + " " + thisTitle.Subtitle).Trim()).SubTerm = titleType;
                            else
                                New.Add_Description("Alternative Title", (thisTitle.NonSort + " " + thisTitle.Title + " " + thisTitle.Subtitle).Trim());
                            break;

                        case Title_Type_Enum.uniform:
                            New.Add_Description("Uniform Title", (thisTitle.NonSort + " " + thisTitle.Title + " " + thisTitle.Subtitle).Trim());
                            break;

                        case Title_Type_Enum.translated:
                            New.Add_Description("Translated Title", (thisTitle.NonSort + " " + thisTitle.Title + " " + thisTitle.Subtitle).Trim()).Language = thisTitle.Language;
                            break;

                        case Title_Type_Enum.abbreviated:
                            New.Add_Description("Abbreviated Title", (thisTitle.NonSort + " " + thisTitle.Title + " " + thisTitle.Subtitle).Trim());
                            break;
                    }
                }
            }

            // Add the series title
            if (Original.Bib_Info.hasSeriesTitle)
                New.Add_Description("Series Title", (Original.Bib_Info.SeriesTitle.NonSort + " " + Original.Bib_Info.SeriesTitle.Title + " " + Original.Bib_Info.SeriesTitle.Subtitle).Trim());

            return true;
        }
    }
}
