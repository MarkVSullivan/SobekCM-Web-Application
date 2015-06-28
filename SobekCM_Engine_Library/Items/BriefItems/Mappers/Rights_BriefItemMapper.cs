#region Using directives

using System;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;

#endregion

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the rights from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Rights_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add the RIGHTS STATEMENT
            if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Access_Condition.Text))
            {
                string value = Original.Bib_Info.Access_Condition.Text;
                string uri = String.Empty;

                if (value.IndexOf("[cc by-nc-nd]") >= 0)
                {
                    value = value.Replace("[cc by-nc-nd]", String.Empty);
                    uri = "http://creativecommons.org/licenses/by-nc-nd/3.0/";
                }
                if (value.IndexOf("[cc by-nc-sa]") >= 0)
                {
                    value = value.Replace("[cc by-nc-sa]", String.Empty);
                    uri = "http://creativecommons.org/licenses/by-nc-sa/3.0/";
                }
                if (value.IndexOf("[cc by-nc]") >= 0)
                {
                    value = value.Replace("[cc by-nc]", String.Empty);
                    uri = "http://creativecommons.org/licenses/by-nc/3.0/";
                }
                if (value.IndexOf("[cc by-nd]") >= 0)
                {
                    value = value.Replace("[cc by-nd]", String.Empty);
                    uri = "http://creativecommons.org/licenses/by-nd/3.0/";
                }
                if (value.IndexOf("[cc by-sa]") >= 0)
                {
                    value = value.Replace("[cc by-sa]", String.Empty);
                    uri = "http://creativecommons.org/licenses/by-sa/3.0/";
                }
                if (value.IndexOf("[cc by]") >= 0)
                {
                    value = value.Replace("[cc by]", String.Empty);
                    uri = "http://creativecommons.org/licenses/by/3.0/";
                }
                if (value.IndexOf("[cc0]") >= 0)
                {
                    value = value.Replace("[cc0]", String.Empty);
                    uri = "http://creativecommons.org/publicdomain/zero/1.0/";
                }

                BriefItem_DescTermValue rightsVal = New.Add_Description("Rights Management", value);
                if (uri.Length > 0)
                    rightsVal.Add_URI(uri);
            }

            return true;
        }
    }
}
