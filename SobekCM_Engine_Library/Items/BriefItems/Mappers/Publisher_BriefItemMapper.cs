#region Using directives

using System;
using System.Collections.Generic;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

#endregion

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
            // Keep track of place of publications alreadyadded
            Dictionary<string, string> pub_places = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Add the publisher, and place of publications
            if (Original.Bib_Info.Publishers_Count > 0)
            {

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

            if ((Original.Bib_Info.Origin_Info != null) && (Original.Bib_Info.Origin_Info.Places_Count > 0))
            {
                foreach (Origin_Info_Place thisPlace in Original.Bib_Info.Origin_Info.Places)
                {
                    if (!pub_places.ContainsKey(thisPlace.Place_Text))
                    {
                        New.Add_Description("Place of Publication", thisPlace.Place_Text);
                        pub_places.Add(thisPlace.Place_Text, thisPlace.Place_Text);
                    } 
                }
            }

            return true;
        }
    }
}
