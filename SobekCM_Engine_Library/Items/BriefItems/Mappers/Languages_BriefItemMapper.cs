#region Using directives

using System;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the languages from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Languages_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add each language
            if (Original.Bib_Info.Languages_Count > 0)
            {
                foreach (Language_Info thisLanguage in Original.Bib_Info.Languages)
                {
                    if (!String.IsNullOrWhiteSpace(thisLanguage.Language_Text))
                    {
                        string language_text = thisLanguage.Language_Text;
                        string from_possible_code = thisLanguage.Get_Language_By_Code(language_text);
                        if (from_possible_code.Length > 0)
                            language_text = from_possible_code;
                        New.Add_Description("Language", language_text);
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(thisLanguage.Language_ISO_Code))
                        {
                            string language_text = thisLanguage.Get_Language_By_Code(thisLanguage.Language_ISO_Code);
                            if (language_text.Length > 0)
                            {
                                New.Add_Description("Language", language_text);
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
