using System;
using System.Collections.Generic;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Rest_API.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the publishers (and place of publication) from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Publisher_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {

            // Add the publisher, and place of publications
            if (Original.Bib_Info.Publishers_Count > 0)
            {
                // Keep track of place of publications alreadyadded
                Dictionary<string, string> pub_places = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                // Step through each publisher
                foreach (Publisher_Info thisPublisher in Original.Bib_Info.Publishers)
                {
                    // Add the name
                    New.Add_Description("Publisher", thisPublisher.Name);

                    // Add the places of publication
                    foreach (Origin_Info_Place thisPubPlace in thisPublisher.Places)
                    {
                        if (!pub_places.ContainsKey(thisPubPlace.Place_Text))
                        {
                            New.Add_Description("Place of Publication", thisPubPlace.Place_Text);
                            pub_places.Add(thisPubPlace.Place_Text, thisPubPlace.Place_Text);
                        }
                    }
                }
            }

            return true;
        }
    }
}
