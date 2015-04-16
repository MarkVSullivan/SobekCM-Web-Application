using System;
using System.Collections.Generic;
using SobekCM.Core.Aggregations;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Rest_API.BriefItem;

// THINGS THAT DON'T SEEM TO WORK RIGHT:
//        GeoSpatial
//        Hierarchical subjkects (in Subject_BriefItemMapper )
// 



namespace SobekCM.Engine_Library.Items.BriefItems
{
    public class TEMP_Citation_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // If there is an External link, PURL EAD link, or EAD Name, add them
            if (Original.Bib_Info.hasLocationInformation)
            {
                New.Add_Citation("External Link", Original.Bib_Info.Location.Other_URL);
                New.Add_Citation("Permanent Link", Original.Bib_Info.Location.PURL);

                // Add the EAD, with the URL if it exists
                if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Location.EAD_URL))
                    New.Add_Citation("Finding Guide Name", Original.Bib_Info.Location.EAD_Name).Add_URI(Original.Bib_Info.Location.EAD_URL);
                else
                    New.Add_Citation("Finding Guide Name", Original.Bib_Info.Location.EAD_Name);
            }

            // Add the genres
            if (Original.Bib_Info.Genres_Count > 0)
            {
                foreach (Genre_Info thisGenre in Original.Bib_Info.Genres)
                {
                    if (!String.IsNullOrWhiteSpace(thisGenre.Authority))
                    {
                        New.Add_Citation("Genre", thisGenre.Genre_Term).Authority = thisGenre.Authority;
                    }
                    else
                    {
                        New.Add_Citation("Genre", thisGenre.Genre_Term);
                    }
                }
            }

            // Add the physical description
            if ((Original.Bib_Info.Original_Description != null) && (!String.IsNullOrWhiteSpace(Original.Bib_Info.Original_Description.Extent)))
            {
                New.Add_Citation("Physical Description", Original.Bib_Info.Original_Description.Extent);
            }

            // Add the TYPE
            New.Add_Citation("Type", Original.Bib_Info.SobekCM_Type_String);

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
                        New.Add_Citation("Language", language_text);
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(thisLanguage.Language_ISO_Code))
                        {
                            string language_text = thisLanguage.Get_Language_By_Code(thisLanguage.Language_ISO_Code);
                            if (language_text.Length > 0)
                            {
                                New.Add_Citation("Language", language_text);
                            }
                        }
                    }
                }
            }


            // Add all the (author) affiliations
            if (Original.Bib_Info.Affiliations_Count > 0)
            {
                foreach (Affiliation_Info thisAffiliation in Original.Bib_Info.Affiliations)
                {
                    New.Add_Citation("Affiliation", thisAffiliation.ToString());
                }
            }

            // Add the donor
            if ((Original.Bib_Info.hasDonor) && (!String.IsNullOrWhiteSpace(Original.Bib_Info.Donor.Full_Name)))
            {
                New.Add_Citation("Donor", Original.Bib_Info.Donor.ToString());
            }

            // Add the publisher, and place of publications
            if (Original.Bib_Info.Publishers_Count > 0)
            {
                // Keep track of place of publications alreadyadded
                Dictionary<string, string> pub_places = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                // Step through each publisher
                foreach (Publisher_Info thisPublisher in Original.Bib_Info.Publishers)
                {
                    // Add the name
                    New.Add_Citation("Publisher", thisPublisher.Name);

                    // Add the places of publication
                    foreach (Origin_Info_Place thisPubPlace in thisPublisher.Places)
                    {
                        if (!pub_places.ContainsKey(thisPubPlace.Place_Text))
                        {
                            New.Add_Citation("Place of Publication", thisPubPlace.Place_Text);
                            pub_places.Add(thisPubPlace.Place_Text, thisPubPlace.Place_Text);
                        }
                    }
                }
            }

            // Add the manufacturers
            if (Original.Bib_Info.Manufacturers_Count > 0)
            {
                foreach (Publisher_Info thisPublisher in Original.Bib_Info.Manufacturers)
                {
                    // Add the name
                    New.Add_Citation("Manufacturer", thisPublisher.Name);
                }
            }

            // Add all the origination information, primarily dates )
            if (Original.Bib_Info.Origin_Info != null)
            {
                // Add the creation date
                if ((!String.IsNullOrWhiteSpace(Original.Bib_Info.Origin_Info.Date_Created)) && (Original.Bib_Info.Origin_Info.Date_Created.Trim() != "-1"))
                {
                    New.Add_Citation("Creation Date", Original.Bib_Info.Origin_Info.Date_Created);
                }

                // Add the publication date, looking under DATE ISSUED, or under MARC DATE ISSUED
                if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Origin_Info.Date_Issued))
                {
                    if (Original.Bib_Info.Origin_Info.Date_Issued.Trim() != "-1")
                    {
                        New.Add_Citation("Publication Date", Original.Bib_Info.Origin_Info.Date_Issued);
                    }
                }
                else if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Origin_Info.MARC_DateIssued))
                {
                    New.Add_Citation("Publication Date", Original.Bib_Info.Origin_Info.MARC_DateIssued);
                }

                // Add the copyright date
                if ((!String.IsNullOrWhiteSpace(Original.Bib_Info.Origin_Info.Date_Copyrighted)) && (Original.Bib_Info.Origin_Info.Date_Copyrighted.Trim() != "-1"))
                {
                    New.Add_Citation("Copyright Date", Original.Bib_Info.Origin_Info.Date_Copyrighted);
                }

                // Add the frequency
                if (Original.Bib_Info.Origin_Info.Frequencies_Count > 0)
                {
                    Dictionary<string, string> frequencies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (Origin_Info_Frequency thisFrequency in Original.Bib_Info.Origin_Info.Frequencies)
                    {
                        if (!frequencies.ContainsKey(thisFrequency.Term))
                        {
                            frequencies.Add(thisFrequency.Term, thisFrequency.Term);
                            New.Add_Citation("Frequency", thisFrequency.Term);
                        }
                    }
                }

                // Collect the state/edition information
                if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Origin_Info.Edition))
                {
                    New.Add_Citation("Edition", Original.Bib_Info.Origin_Info.Edition);
                }
            }



            // Add the subjects and coordinate information if that exists
            if (Original.Bib_Info.TemporalSubjects_Count > 0)
            {
                foreach (Temporal_Info thisTemporal in Original.Bib_Info.TemporalSubjects)
                {
                    if (thisTemporal.TimePeriod.Length > 0)
                    {
                        if ((thisTemporal.Start_Year > 0) && (thisTemporal.End_Year > 0))
                        {
                            New.Add_Citation("Temporal Coverage", thisTemporal.TimePeriod + " ( " + thisTemporal.Start_Year + " - " + thisTemporal.End_Year + " )");
                        }
                        if ((thisTemporal.Start_Year > 0) && (thisTemporal.End_Year <= 0))
                        {
                            New.Add_Citation("Temporal Coverage", thisTemporal.TimePeriod + " ( " + thisTemporal.Start_Year + " - )");
                        }
                        if ((thisTemporal.Start_Year <= 0) && (thisTemporal.End_Year > 0))
                        {
                            New.Add_Citation("Temporal Coverage", thisTemporal.TimePeriod + " (  - " + thisTemporal.End_Year + " )");
                        }
                    }
                    else
                    {
                        if ((thisTemporal.Start_Year > 0) && (thisTemporal.End_Year > 0))
                        {
                            New.Add_Citation("Temporal Coverage", thisTemporal.Start_Year + " - " + thisTemporal.End_Year);
                        }
                        if ((thisTemporal.Start_Year > 0) && (thisTemporal.End_Year <= 0))
                        {
                            New.Add_Citation("Temporal Coverage", thisTemporal.Start_Year + " - ");
                        }
                        if ((thisTemporal.Start_Year <= 0) && (thisTemporal.End_Year > 0))
                        {
                            New.Add_Citation("Temporal Coverage", " - " + thisTemporal.End_Year);
                        }
                    }
                }
            }

            // Add the target audiences
            if (Original.Bib_Info.Target_Audiences_Count > 0)
            {
                foreach (TargetAudience_Info thisAudience in Original.Bib_Info.Target_Audiences)
                {
                    if (!String.IsNullOrWhiteSpace(thisAudience.Authority))
                    {
                        New.Add_Citation("Target Audience", thisAudience.Audience).Authority = thisAudience.Authority;
                    }
                    else
                    {
                        New.Add_Citation("Target Audience", thisAudience.Audience).Authority = thisAudience.Authority;
                    }
                }
            }

            // Add the notes
            if (Original.Bib_Info.Notes_Count > 0)
            {
                foreach (Note_Info thisNote in Original.Bib_Info.Notes)
                {
                    if (thisNote.Note_Type != Note_Type_Enum.NONE)
                    {
                        if (thisNote.Note_Type != Note_Type_Enum.internal_comments)
                        {
                            New.Add_Citation("Note", thisNote.Note).SubTerm = thisNote.Note_Type_Display_String;
                        }
                    }
                    else
                    {
                        New.Add_Citation("Note", thisNote.Note);
                    }
                }
            }

            // Add the abstracts
            if (Original.Bib_Info.Abstracts_Count > 0)
            {
                foreach (Abstract_Info thisAbstract in Original.Bib_Info.Abstracts)
                {
                    if (!String.IsNullOrWhiteSpace(thisAbstract.Display_Label))
                    {
                        New.Add_Citation("Abstract", thisAbstract.Abstract_Text).SubTerm = thisAbstract.Display_Label;
                    }
                    else
                    {
                        New.Add_Citation("Abstract", thisAbstract.Abstract_Text);
                    }
                }
            }

            // Add the desciption user tags
            if (Original.Behaviors.User_Tags_Count > 0)
            {
                foreach (Descriptive_Tag tag in Original.Behaviors.User_Tags)
                {
                    New.Add_Citation("User Description", tag.Description_Tag).Authority = tag.UserName + "|" + tag.Date_Added.ToShortDateString();
                }
            }



            // Add the SOURCE INSTITUTION information
            if ( !String.IsNullOrWhiteSpace(Original.Bib_Info.Source.Statement))
            {
                // Add the source institution
                BriefItem_CitationElementValue sourceValue = New.Add_Citation("Source Institution", Original.Bib_Info.Source.Statement);

                // Was the code present, and active?
                if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Source.Code))
                {
                    if ((Engine_ApplicationCache_Gateway.Codes != null) && (Engine_ApplicationCache_Gateway.Codes.isValidCode("i" + Original.Bib_Info.Source.Code)))
                    {
                        Item_Aggregation_Related_Aggregations sourceAggr = Engine_ApplicationCache_Gateway.Codes["i" + Original.Bib_Info.Source.Code];
                        if (sourceAggr.Active)
                        {
                            sourceValue.Add_URI("[%BASEURL%]" + "i" + Original.Bib_Info.Source.Code + "[%URLOPTS%]");
                        }

                        // Was there an external link on this agggreation?
                        if (!String.IsNullOrWhiteSpace(sourceAggr.External_Link))
                        {
                            sourceValue.Add_URI(sourceAggr.External_Link);
                        }
                    }
                }
            }

            // Add the HOLDING LOCATION information
            if ((Original.Bib_Info.hasLocationInformation) && (!String.IsNullOrWhiteSpace(Original.Bib_Info.Location.Holding_Name)))
            {
                // Add the source institution
                BriefItem_CitationElementValue sourceValue = New.Add_Citation("Holding Location", Original.Bib_Info.Location.Holding_Name);

                // Was the code present, and active?
                if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Location.Holding_Code))
                {
                    if ((Engine_ApplicationCache_Gateway.Codes != null) && (Engine_ApplicationCache_Gateway.Codes.isValidCode("i" + Original.Bib_Info.Location.Holding_Code)))
                    {
                        Item_Aggregation_Related_Aggregations sourceAggr = Engine_ApplicationCache_Gateway.Codes["i" + Original.Bib_Info.Location.Holding_Code];
                        if (sourceAggr.Active)
                        {
                            sourceValue.Add_URI("[%BASEURL%]" + "i" + Original.Bib_Info.Location.Holding_Code + "[%URLOPTS%]");
                        }

                        // Was there an external link on this agggreation?
                        if (!String.IsNullOrWhiteSpace(sourceAggr.External_Link))
                        {
                            sourceValue.Add_URI(sourceAggr.External_Link);
                        }
                    }
                }
            }

 
            // Add the RIGHTS STATEMENT
            if ( !String.IsNullOrWhiteSpace(Original.Bib_Info.Access_Condition.Text))
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

                BriefItem_CitationElementValue rightsVal = New.Add_Citation("Rights Management", value);
                if (uri.Length > 0)
                    rightsVal.Add_URI(uri);
            }

            // Add the IDENTIFIERS
            if (Original.Bib_Info.Identifiers_Count > 0)
            {
                foreach (Identifier_Info thisIdentifier in Original.Bib_Info.Identifiers)
                {
                    if ( !String.IsNullOrWhiteSpace(thisIdentifier.Type))
                    {
                        New.Add_Citation("Resource Identifier", thisIdentifier.Identifier).Authority = thisIdentifier.Type;
                    }
                    else
                    {
                        New.Add_Citation("Resource Identifier", thisIdentifier.Identifier);
                    }
                }
            }

            // Add the CLASSIFICATIONS
            if (Original.Bib_Info.Classifications_Count > 0)
            {
                foreach (Classification_Info thisClassification in Original.Bib_Info.Classifications)
                {
                    if ( !String.IsNullOrWhiteSpace(thisClassification.Authority))
                    {
                        New.Add_Citation("Classification", thisClassification.Classification).Authority = thisClassification.Authority;
                    }
                    else
                    {
                        New.Add_Citation("Classification", thisClassification.Classification);
                    }
                }
            }

            // Add the system id
            if (Original.METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL)
                New.Add_Citation("System ID", Original.BibID + ":" + Original.VID);
            else
                New.Add_Citation("System ID", Original.BibID + ":" + Original.VID);

            // Add the RELATED ITEMS
            if (Original.Bib_Info.RelatedItems_Count > 0)
            {
                foreach (Related_Item_Info relatedItem in Original.Bib_Info.RelatedItems)
                {
                    // Determine the field to display
                    string relatedDisplay = String.Empty;
                    if ((!String.IsNullOrWhiteSpace(relatedItem.URL)) && (!String.IsNullOrWhiteSpace(relatedItem.URL_Display_Label)))
                        relatedDisplay = relatedItem.URL_Display_Label;
                    else if (!String.IsNullOrWhiteSpace(relatedItem.Main_Title.Title))
                        relatedDisplay = relatedItem.Main_Title.Title;
                    else if (!String.IsNullOrWhiteSpace(relatedItem.URL))
                        relatedDisplay = relatedItem.URL;

                    // If nothing to display, move to next one
                    if (relatedDisplay.Length == 0)
                        continue;

                    // Add this related item
                    BriefItem_CitationElementValue relatedObj = New.Add_Citation("Related Item", relatedDisplay);

                    // Add the relationship
                    switch (relatedItem.Relationship)
                    {
                        case Related_Item_Type_Enum.host:
                            relatedObj.SubTerm = "Host material";
                            break;

                        case Related_Item_Type_Enum.otherFormat:
                            relatedObj.SubTerm = "Other format";
                            break;

                        case Related_Item_Type_Enum.otherVersion:
                            relatedObj.SubTerm = "Other version";
                            break;

                        case Related_Item_Type_Enum.preceding:
                            relatedObj.SubTerm = "Preceded by";
                            break;

                        case Related_Item_Type_Enum.succeeding:
                            relatedObj.SubTerm = "Succeeded by";
                            break;
                    }

                    // Add the URI if one was indicated
                    if (!String.IsNullOrWhiteSpace(relatedItem.URL))
                    {
                        relatedObj.Add_URI(relatedItem.URL);
                    }
                    else if (!String.IsNullOrWhiteSpace(relatedItem.SobekCM_ID))
                    {
                        relatedObj.Add_URI("[%BASEURL%]" + relatedItem.SobekCM_ID.Replace("_", "/") + "[%URLOPTS%]");
                    }
                }
            }

            // Add the ticklers
            if (Original.Behaviors.Ticklers_Count > 0)
            {
                New.Add_Citation("Ticklers", Original.Behaviors.Ticklers);
            }

            // Add the aggregations
            if ((Original.Behaviors.Aggregation_Count > 0) && (Engine_ApplicationCache_Gateway.Codes != null ))
            {
                foreach (Aggregation_Info thisAggr in Original.Behaviors.Aggregations)
                {
                    Item_Aggregation_Related_Aggregations aggrObj = Engine_ApplicationCache_Gateway.Codes[thisAggr.Code];
                    if (aggrObj.Active)
                    {
                        New.Add_Citation("Aggregations", aggrObj.ShortName).Add_URI("[%BASEURL%]" + aggrObj.Code + "[%URLOPTS%]");
                    }
                    else
                    {
                        New.Add_Citation("Aggregations", aggrObj.ShortName);
                    }
                }
            }

            // MAKE ADD INTERNAL CLASS!!

            // Add some internal values (usually not displayed for non-internal users)
            New.Add_Citation("Format", Original.Bib_Info.SobekCM_Type_String);
            New.Add_Citation("Creation Date", Original.METS_Header.Create_Date.ToShortDateString());
            New.Add_Citation("Last Modified", Original.METS_Header.Modify_Date.ToShortDateString());
            New.Add_Citation("Last Type", Original.METS_Header.RecordStatus);
            New.Add_Citation("Last User", Original.METS_Header.Creator_Individual);
            New.Add_Citation("System Folder", Original.Web.AssocFilePath.Replace("/", "\\"));

            return true;
        }
    }
}
