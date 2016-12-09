using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Metadata_Modules.VRACore;

namespace SobekCM.Resource_Object.Mapping
{
    /// <summary> Custom Henderson Libraries mapping which takes data and field name and maps that data into the SobekCM field
    /// within the main SobekCM Item package </summary>
    /// <remarks> This class implements the <see cref="iBibliographicMapper" /> interface </remarks>
    public class Henderson_Bibliographic_Mapper : iBibliographicMapper
    {
        /// <summary> Name of this bibliographic mapper, used for logging mapping </summary>
        public string Name { get { return "Henderson Mapper"; } }

        /// <summary> Returns the list of preferred mappings for the elements handled
        /// by this mapper </summary>
        /// <remarks> For example this might just return 'title', 'creator', 'subject', etc.. </remarks>
        public List<string> Preferred_Mappings
        {
            get { return null; }
        }

        /// <summary> Adds a bit of data to a bibliographic package using the mapping </summary>
        /// <param name="Package">Bibliographic package to receive the data</param>
        /// <param name="Data">Text of the data</param>
        /// <param name="Field">Mapped field</param>
        /// <returns> TRUE if the field was mapped, FALSE if there was data and no mapping was found </returns>
        public bool Add_Data(SobekCM_Item Package, string Data, string Field)
        {
            // If no field listed, just skip (but not a mapping error, so return TRUE)
            if (String.IsNullOrEmpty(Field))
                return true;

            // If no data listed, just skip (but not a mapping error, so return TRUE)
            if (String.IsNullOrWhiteSpace(Data))
                return true;

            // Trim the data
            Data = Data.Trim();

            // Normalize the field name
            string correctName = Field.ToLower().Replace("(s)", "").Replace("#", "").Replace(" ", "").Replace(".", "").Replace(":", "").Replace("\\", "").Replace("/", "").Replace(")", "").Replace("(", "").Trim();

            // Everything depends on the field which is mapped
            switch (correctName)
            {
                case "acknowledgements":
                case "acknowledgments":
                    Package.Bib_Info.Add_Note(Data, Note_Type_Enum.CreationCredits, "Acknowledgments");
                    return true;

                case "bulletincommitteemembers":
                    Package.Bib_Info.Add_Named_Entity(Data, "Bulletin Committee Member").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "collectionlocation":
                    Package.Bib_Info.Add_Note(Data, Note_Type_Enum.AdditionalPhysicalForm, "Collection Location");
                    return true;

                case "columnist":
                    Package.Bib_Info.Add_Named_Entity(Data, "Columnist").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "contents":
                    Package.Bib_Info.Get_TableOfContents("Contents").Add(Data);
                    return true;

                case "contributinginstitution":
                    Package.Bib_Info.Add_Named_Entity(Data, "Contributing Institution").Name_Type = Name_Info_Type_Enum.Corporate;
                    return true;

                case "contributor":
                    Package.Bib_Info.Add_Named_Entity(Data, "Contributing Institution").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "coverstories":
                    Package.Bib_Info.Get_TableOfContents("Cover Stories").Add(Data);
                    return true;

                case "coverstoriescontents":
                    Package.Bib_Info.Get_TableOfContents("Cover Stories / Contents").Add(Data);
                    return true;

                case "covertitle":
                    Package.Bib_Info.Get_TableOfContents("Contents").Add(Data);
                    return true;

                case "creator":
                    Package.Bib_Info.Add_Named_Entity(Data, "Creator").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "creator,performer":
                    Package.Bib_Info.Add_Named_Entity(Data, "Creator / Performer").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "date":
                    Package.Bib_Info.Origin_Info.Date_Created = Data;
                    return true;

                case "dateoriginal":
                    Package.Bib_Info.Origin_Info.Date_Created = Data;
                    return true;

                case "description":
                    Package.Bib_Info.Add_Abstract(new Abstract_Info { Abstract_Text = Data, Type = "Summary", Display_Label = "Description" });
                    return true;

                case "designer":
                    Package.Bib_Info.Add_Named_Entity(Data, "Designer").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "digitalid":
                    Package.Bib_Info.Add_Identifier(Data, "Digital Id");
                    return true;

                case "digitizationspecifications":
                    Package.Bib_Info.Add_Note(Data, Note_Type_Enum.Ownership, "Digitization Specifications");
                    return true;

                case "editing":
                case "editor":
                    Package.Bib_Info.Add_Named_Entity(Data, "Editor").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "editorwriter":
                    Package.Bib_Info.Add_Named_Entity(Data, "Editor / Writer").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "emcee":
                    Package.Bib_Info.Add_Named_Entity(Data, "Emcee").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "features":
                    Package.Bib_Info.Get_TableOfContents("Features").Add(Data);
                    return true;

                case "filesizeandduration":
                    // Get the duration from the parantheses
                    int fsd_start = Data.IndexOf("(");
                    int fsd_end = Data.IndexOf(")");
                    if ((fsd_start > 0) && (fsd_end > fsd_start))
                    {
                        string duration_string = Data.Substring(fsd_start + 1, fsd_end - fsd_start - 1);
                        if (( duration_string.IndexOf("second", StringComparison.OrdinalIgnoreCase) > 0 ) || (duration_string.IndexOf("minute", StringComparison.OrdinalIgnoreCase) > 0 ))
                            Package.Bib_Info.Original_Description.Extent = duration_string;
                    }
                    return true;

                case "founder":
                    Package.Bib_Info.Add_Named_Entity(Data, "Founder").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "frontpageheadlines":
                    Package.Bib_Info.Get_TableOfContents("Front Page Headlines").Add(Data);
                    return true;

                case "generalsubjects":
                    split_add_subjects(Package, Data);
                    return true;

                case "genre":
                    Package.Bib_Info.Add_Genre(Data);
                    return true;

                case "geographiccoverage":
                    Package.Bib_Info.Add_Spatial_Subject(Data);
                    return true;

                case "historicalnarrative":
                    Package.Bib_Info.Add_Named_Entity(Data, "Historical Narrative").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "identifiedindividuals":
                    Subject_Info_Name nameSubj = Package.Bib_Info.Add_Name_Subject();
                    nameSubj.Full_Name = Data;
                    nameSubj.Name_Type = Name_Info_Type_Enum.Personal;
                    nameSubj.Description = "Identified Individuals";
                    return true;

                case "image-specificsubjects":
                    split_add_subjects(Package, Data);
                    return true;

                case "internalnote":
                    Package.Bib_Info.Add_Note(Data, Note_Type_Enum.InternalComments, "Acknowledgments");
                    return true;

                case "interviewdate":
                    Package.Bib_Info.Origin_Info.Add_Date_Other(Data, "Interview Date");
                    return true;

                case "interviewlocation":
                    Package.Bib_Info.Origin_Info.Add_Place(Data);
                    return true;

                case "interviewee":
                    Package.Bib_Info.Add_Named_Entity(Data, "Interviewee").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "interviewer":
                    Package.Bib_Info.Add_Named_Entity(Data, "Interviewer").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "issue":
                case "issuenumber":
                    // If there is already an enumberation (1), use (2)
                    if (!String.IsNullOrWhiteSpace(Package.Bib_Info.Series_Part_Info.Enum1))
                    {
                        int issue_number = -1;
                        if (Int32.TryParse(Data, out issue_number))
                        {
                            Package.Bib_Info.Series_Part_Info.Enum1 = "Issue " + Data;
                            Package.Bib_Info.Series_Part_Info.Enum1_Index = issue_number;
                        }
                        else
                        {
                            Package.Bib_Info.Series_Part_Info.Enum1 = Data + " Issue";
                        }
                    }
                    else
                    {
                        int issue_number = -1;
                        if (Int32.TryParse(Data, out issue_number))
                        {
                            Package.Bib_Info.Series_Part_Info.Enum2 = "Issue " + Data;
                            Package.Bib_Info.Series_Part_Info.Enum2_Index = issue_number;
                        }
                        else
                        {
                            Package.Bib_Info.Series_Part_Info.Enum2 = Data + " Issue";
                        }
                    }
                    return true;

                case "issue-specificsubjects":
                    split_add_subjects(Package, Data);
                    return true;

                case "item-specificsubjects":
                    split_add_subjects(Package, Data);
                    return true;

                case "language":
                    Package.Bib_Info.Add_Language(Data);
                    return true;

                case "layout":
                    Package.Bib_Info.Add_Named_Entity(Data, "Layout").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "location":
                    Package.Bib_Info.Origin_Info.Add_Place(Data);
                    return true;

                case "locationdepicted":
                    Package.Bib_Info.Add_Spatial_Subject(Data);
                    return true;

                case "monthseason":
                    Package.Bib_Info.Series_Part_Info.Month = Data;
                    return true;

                case "monthscovered":
                    Package.Bib_Info.Series_Part_Info.Month = Data;
                    return true;

                case "namesorganizations":
                    Package.Bib_Info.Add_Name_Subject().Full_Name = Data;
                    return true;

                case "originaldate":
                    Package.Bib_Info.Origin_Info.Date_Issued = Data;
                    return true;

                case "originalitemid":
                    Package.Bib_Info.Add_Identifier(Data, "Original ItemID");
                    return true;

                case "originalmedium":
                    VRACore_Info vraCoreInfo2 = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo2 == null)
                    {
                        vraCoreInfo2 = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo2);
                    }
                    vraCoreInfo2.Add_Material(Data, "medium");
                    return true;

                case "originalpublisher":
                    Package.Bib_Info.Add_Publisher(Data);
                    return true;

                case "photographer":
                    Package.Bib_Info.Add_Named_Entity(Data, "Photographer").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "photographs":
                    Package.Bib_Info.Add_Named_Entity(Data, "Photograph Studio").Name_Type = Name_Info_Type_Enum.Corporate;
                    return true;

                case "physicalcollection":
                    Package.Bib_Info.Location.Holding_Name = Data;
                    return true;

                case "presentedatboardmeeting":
                    Package.Bib_Info.Origin_Info.Add_Date_Other(Data, "Presented at board meeting");
                    return true;

                case "presenter":
                    Package.Bib_Info.Add_Named_Entity(Data, "Presenter").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "president":
                    Package.Bib_Info.Add_Named_Entity(Data, "President").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "projectcoordinator":
                    Package.Bib_Info.Add_Named_Entity(Data, "Project Coordinator").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "projectcreator":
                    Package.Bib_Info.Add_Named_Entity(Data, "Project Creator").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "publicationdate":
                    Package.Bib_Info.Origin_Info.Date_Issued = Data;
                    return true;

                case "publisher":
                    Package.Bib_Info.Add_Publisher(Data);
                    return true;

                case "relatedbminewsletter":
                    Related_Item_Info relatedNewsletter = new Related_Item_Info();
                    relatedNewsletter.Main_Title.Title = Data;
                    relatedNewsletter.Relationship = Related_Item_Type_Enum.UNKNOWN;
                    relatedNewsletter.Add_Note("Related BMI Newsletter");
                    Package.Bib_Info.Add_Related_Item(relatedNewsletter);
                    return true;

                case "relatedbmiphoto":
                    Related_Item_Info relatedPhoto = new Related_Item_Info();
                    relatedPhoto.Main_Title.Title = Data;
                    relatedPhoto.Relationship = Related_Item_Type_Enum.UNKNOWN;
                    relatedPhoto.Add_Note("Related BMI Photograph");
                    Package.Bib_Info.Add_Related_Item(relatedPhoto);
                    return true;

                case "resourcetype":
                    Package.Bib_Info.Type.Add_Uncontrolled_Type(Data);
                    return true;

                case "rightsinformation":
                    Package.Bib_Info.Access_Condition.Text = Data;
                    return true;

                case "scrapbook-specificsubjects":
                    split_add_subjects(Package, Data);
                    return true;

                case "seasonmonth":
                    Package.Bib_Info.Series_Part_Info.Month = Data;
                    return true;

                case "subjects":
                    split_add_subjects(Package, Data);
                    return true;

                case "timeperiodcovered":
                    // Was this either a number, or a range?
                    int dash_count = 0;
                    bool isNumbers = true;
                    string dns = Data.Replace(" ", "");
                    foreach (char thisChar in dns)
                    {
                        if ((!Char.IsNumber(thisChar)) && (thisChar != '-'))
                        {
                            isNumbers = false;
                            break;
                        }

                        if (thisChar == '-')
                            dash_count++;
                    }

                    // Just add as is, if not number or range
                    if ((!isNumbers) || (dash_count > 1))
                        Package.Bib_Info.Add_Temporal_Subject(-1, -1, Data);
                    else
                    {
                        int start_year = -1;
                        int end_year = -1;
                        // Was it a range?
                        if (dash_count == 1)
                        {
                            string[] splitter = dns.Split("-".ToCharArray());
                            if (splitter.Length == 2)
                            {
                                if (( Int32.TryParse(splitter[0], out start_year)) && ( Int32.TryParse(splitter[1], out end_year )))
                                    Package.Bib_Info.Add_Temporal_Subject(start_year, end_year, Data);
                                else
                                    Package.Bib_Info.Add_Temporal_Subject(-1, -1, Data);
                            }
                        }
                        else
                        {
                            if ( Int32.TryParse(Data, out start_year))
                                Package.Bib_Info.Add_Temporal_Subject(start_year, -1, Data);
                            else
                                Package.Bib_Info.Add_Temporal_Subject(-1, -1, Data);
                        }
                    }
                    return true;

                case "title":
                    Package.Bib_Info.Main_Title.Title = Data;
                    return true;

                case "topics":
                    split_add_subjects(Package, Data);
                    return true;

                case "transcriptionprovidedby":
                    Package.Bib_Info.Add_Note(Data, Note_Type_Enum.CreationCredits, "Transcription Provided By");
                    return true;

                case "videofilesizeandduration":
                    // Get the duration from the parantheses
                    int vfsd_start = Data.IndexOf("(");
                    int vfsd_end = Data.IndexOf(")");
                    if ((vfsd_start > 0) && (vfsd_end > vfsd_start))
                    {
                        string duration_string = Data.Substring(vfsd_start + 1, vfsd_end - vfsd_start - 1);
                        Package.Bib_Info.Original_Description.Extent = duration_string;
                    }
                    return true;

                case "videographer":
                    Package.Bib_Info.Add_Named_Entity(Data, "Videographer").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "volume":
                    // If there is already an enumberation (1), move it to (2)
                    if (!String.IsNullOrWhiteSpace(Package.Bib_Info.Series_Part_Info.Enum1))
                    {
                        Package.Bib_Info.Series_Part_Info.Enum2 = Package.Bib_Info.Series_Part_Info.Enum1;
                        Package.Bib_Info.Series_Part_Info.Enum2_Index = Package.Bib_Info.Series_Part_Info.Enum1_Index;
                    }

                    // Now, add the volume
                    int volume_number = -1;
                    if (Int32.TryParse(Data, out volume_number))
                    {
                        Package.Bib_Info.Series_Part_Info.Enum1 = "Volume " + Data;
                        Package.Bib_Info.Series_Part_Info.Enum1_Index = volume_number;
                    }
                    else
                    {
                        Package.Bib_Info.Series_Part_Info.Enum1 = Data + " Volume";
                        Package.Bib_Info.Series_Part_Info.Enum1_Index = -1;
                    }
                    return true;


                case "wars":
                    if ( Data.IndexOf("World War") >= 0 )
                        Package.Bib_Info.Add_Temporal_Subject(1939, 1945, "World War");
                    else if ( Data.IndexOf("Vietnam") >= 0 )
                        Package.Bib_Info.Add_Temporal_Subject(1961, 1975, "Vietnam War");
                    else if ( Data.IndexOf("Korean") >= 0 )
                        Package.Bib_Info.Add_Temporal_Subject(1950, 1953, "Korean War");
                    return true;

                case "writer":
                    Package.Bib_Info.Add_Named_Entity(Data, "Writer").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "writerscontributors":
                    Package.Bib_Info.Add_Named_Entity(Data, "Writer / Contributor").Name_Type = Name_Info_Type_Enum.Personal;
                    return true;

                case "year":
                    Package.Bib_Info.Series_Part_Info.Year = Data;
                    return true;

                default:
                    // No mapping exists and this is a non-known no-mapping
                    return false;
            }
        }

        private void split_add_subjects(SobekCM_Item Package, string Data )
        {
            if (Data.IndexOf("--") < 0)
                Package.Bib_Info.Add_Subject(new Subject_Info_Standard(Data, null));
            else
            {
                Subject_Info_Standard subject = new Subject_Info_Standard();
                string[] splitter = Data.Split("-".ToCharArray());
                foreach (string thisSplit in splitter)
                {
                    if (thisSplit.Length == 0)
                        continue;

                    string tsl = thisSplit.ToLower().Trim();
                    
                    // Was this geographic?
                    if ((tsl.IndexOf("county") >= 0) || (tsl.IndexOf("(nev.)") > 0) || (tsl.IndexOf("henderson") >= 0) || (tsl.IndexOf("nevada") >= 0) || (tsl.IndexOf("las vegas") >= 0) || (tsl.IndexOf("(ariz.)") >= 0)
                        || (tsl.IndexOf("california") >= 0) || (tsl.IndexOf("los angeles") >= 0) || (tsl.IndexOf("(calif.)") >= 0) || (tsl.IndexOf("russia") >= 0) || (tsl.IndexOf("lake mead") >= 0) || (tsl.IndexOf("(ariz.)") >= 0)
                        || (tsl.IndexOf("united states") >= 0) || (tsl.IndexOf("(ariz.)") >= 0) || (tsl.IndexOf("el paso") >= 0) || (tsl.IndexOf("texas") >= 0) || (tsl.IndexOf("(ariz.)") >= 0) || (tsl.IndexOf("boulder city") >= 0)
                        || (tsl.IndexOf("pittman") >= 0) || (tsl.IndexOf("gabbs") >= 0) || (tsl.IndexOf("california") >= 0) || (tsl.IndexOf("colorado") >= 0)
                        || (tsl.IndexOf("reno") >= 0) || (tsl.IndexOf("(ariz.)") >= 0) || ( tsl == "japan"))
                        subject.Add_Geographic(thisSplit.Trim().Replace("\"",""));
                    // Was this temporal?
                    else if ((tsl.IndexOf("20th century") >= 0) || (tsl.IndexOf("war, 19") >= 0))
                        subject.Add_Temporal(thisSplit.Trim());
                    else if (tsl.IndexOf("maps, topographic") >= 0)
                        subject.Add_Genre(thisSplit.Trim());
                    else
                        subject.Add_Topic(thisSplit.Trim());
                }

                Package.Bib_Info.Add_Subject(subject);
            }
        }
    }
}
