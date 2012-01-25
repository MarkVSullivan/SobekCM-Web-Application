using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SolrNet.Attributes;

namespace SobekCM.Bib_Package.Solr
{
    /// <summary> Object stores the basic information about a singlee digital object and makes
    /// the full metadata and text available for Solr (or database) indexing </summary>
    public class SolrDocument
    {
        private SobekCM_Item digitalObject;
        private string fileLocation;

        private List<string> country = new List<string>();
        private List<string> state = new List<string>();
        private List<string> county = new List<string>();
        private List<string> city = new List<string>();
        private List<string> allsubjects = new List<string>();
        private List<string> namesubject = new List<string>();
        private List<string> titlesubject = new List<string>();
        private List<string> genre = new List<string>();
        private List<string> spatialcoverage = new List<string>();
        private List<string> subjectkeyword = new List<string>();
        private List<string> temporalsubject = new List<string>();
        private List<string> tocterms = new List<string>();
        private List<SolrPage> solrpages = new List<SolrPage>();
        private List<string> additional_text_files = new List<string>();

        /// <summary> Constructor for a new instance of the SolrDocument class </summary>
        public SolrDocument()
        {

        }

        /// <summary> Constructor for a new instance of the SolrDocument class </summary>
        /// <param name="Digital_Object"> Digital object to create an easily indexable view object for </param>
        /// <param name="File_Location"> Location for all of the text files associated with this item </param>
        /// <remarks> Some work is done in the constructor; in particular, work that eliminates the number of times 
        /// iterations must be made through objects which may be indexed in a number of places.  
        /// This includes subject keywords, spatial information, genres, and information from the table of contents </remarks>
        public SolrDocument(SobekCM_Item Digital_Object, string File_Location)
        {
            this.digitalObject = Digital_Object;
            fileLocation = File_Location;

            // Add the subjects
            if (digitalObject.Bib_Info.Subjects_Count > 0)
            {
                foreach (SobekCM.Bib_Package.Bib_Info.Subject_Info thisSubject in digitalObject.Bib_Info.Subjects)
                {
                    // Add every subject to the complete list of subjects
                    allsubjects.Add(thisSubject.ToString(false));

                    // Add name subjects
                    if (thisSubject.Class_Type == SobekCM.Bib_Package.Bib_Info.Subject_Info_Type.Name)
                    {
                        namesubject.Add(thisSubject.ToString(false));
                    }

                    // Add title subjects
                    if (thisSubject.Class_Type == SobekCM.Bib_Package.Bib_Info.Subject_Info_Type.TitleInfo)
                    {
                        titlesubject.Add(thisSubject.ToString(false));
                    }

                    // Add the subject keywords
                    if ((thisSubject.Class_Type == SobekCM.Bib_Package.Bib_Info.Subject_Info_Type.Standard) && (thisSubject.ID.IndexOf("690") < 0) && (thisSubject.ID.IndexOf("691") < 0))
                    {
                        // Cast to the hierarchical subject type
                        SobekCM.Bib_Package.Bib_Info.Subject_Info_Standard standSubj = (SobekCM.Bib_Package.Bib_Info.Subject_Info_Standard)thisSubject;

                        if (standSubj.Genres_Count > 0)
                        {
                            foreach (string genreTerm in standSubj.Genres)
                            {
                                genre.Add(genreTerm.ToLower());
                            }
                        }
                        if (standSubj.Geographics_Count > 0)
                        {
                            foreach (string geoTerm in standSubj.Geographics)
                            {
                                spatialcoverage.Add(geoTerm);
                            }
                        }
                        if (standSubj.Topics_Count > 0)
                        {
                            foreach (string topicTerm in standSubj.Topics)
                            {
                                subjectkeyword.Add(topicTerm);
                            }
                        }
                    }

                    // Add hierarchical spatial info
                    if (thisSubject.Class_Type == SobekCM.Bib_Package.Bib_Info.Subject_Info_Type.Hierarchical_Spatial)
                    {
                        // Cast to the hierarchical subject type
                        SobekCM.Bib_Package.Bib_Info.Subject_Info_HierarchicalGeographic hiero = (SobekCM.Bib_Package.Bib_Info.Subject_Info_HierarchicalGeographic)thisSubject;

                        // Check for existing subfacets and add if not there
                        if ((hiero.Continent.Length > 0) && (!spatialcoverage.Contains(hiero.Continent)))
                        {
                            spatialcoverage.Add(hiero.Continent);
                        }
                        if ((hiero.Country.Length > 0) && (!country.Contains(hiero.Country)))
                        {
                            country.Add(hiero.Country);
                        }
                        if ((hiero.State.Length > 0) && (!state.Contains(hiero.State)))
                        {
                            state.Add(hiero.State);
                        }
                        if ((hiero.County.Length > 0) && (!county.Contains(hiero.County)))
                        {
                            county.Add(hiero.County);
                        }
                        if ((hiero.City.Length > 0) && (!city.Contains(hiero.City)))
                        {
                            city.Add(hiero.City);
                        }
                        if ((hiero.Island.Length > 0) && (!spatialcoverage.Contains(hiero.Island)))
                        {
                            spatialcoverage.Add(hiero.Island);
                        }
                    }
                }
            }

            // Add the individual genre information (just to be done with genre)
            if (digitalObject.Bib_Info.Genres_Count > 0)
            {
                foreach (SobekCM.Bib_Package.Bib_Info.Genre_Info thisGenre in digitalObject.Bib_Info.Genres)
                {
                    genre.Add(thisGenre.Genre_Term.ToLower());
                }
            }

            // Add all the temporal subjects
            if (digitalObject.Bib_Info.TemporalSubjects_Count > 0)
            {
                foreach (SobekCM.Bib_Package.Bib_Info.Temporal_Info thisTemporal in digitalObject.Bib_Info.TemporalSubjects)
                {
                    if (thisTemporal.TimePeriod.Length > 0)
                    {
                        allsubjects.Add(thisTemporal.TimePeriod);
                        temporalsubject.Add(thisTemporal.TimePeriod);
                    }
                }
            }

            // Prepare to step through all the divisions/pages in this item
            int pageorder = 1;
            tocterms = new List<string>();
            solrpages = new List<SolrPage>();
            List<SobekCM.Bib_Package.Divisions.abstract_TreeNode> divsAndPages = digitalObject.Divisions.Physical_Tree.Divisions_PreOrder;

            // Get the list of all TXT files in this division
            string[] text_files = System.IO.Directory.GetFiles(File_Location, "*.txt");
            Dictionary<string, string> text_files_existing = new Dictionary<string, string>();
            foreach (string thisTextFile in text_files)
            {
                string filename = (new System.IO.FileInfo(thisTextFile)).Name.ToUpper();
                text_files_existing[filename] = filename;
            }

            // Get the list of all THM.JPG files in this division
            string[] thumbnail_files = System.IO.Directory.GetFiles(File_Location, "*thm.jpg");
            Dictionary<string, string> thumbnail_files_existing = new Dictionary<string, string>();
            foreach (string thisTextFile in thumbnail_files)
            {
                string filename = (new System.IO.FileInfo(thisTextFile)).Name;
                thumbnail_files_existing[filename.ToUpper().Replace("THM.JPG", "")] = filename;
            }

            // Step through all division nodes from the physical tree here
            List<string> text_files_included = new List<string>();
            foreach (SobekCM.Bib_Package.Divisions.abstract_TreeNode thisNode in divsAndPages)
            {
                if (thisNode.Page)
                {
                    // Cast to a page to continnue
                    SobekCM.Bib_Package.Divisions.Page_TreeNode pageNode = (SobekCM.Bib_Package.Divisions.Page_TreeNode)thisNode;

                    // If this is a unique page label, add it
                    if (pageNode.Label.Length > 0)
                    {
                        if (pageNode.Label.ToUpper().IndexOf("PAGE ") < 0)
                            tocterms.Add(pageNode.Label);
                    }

                    // Look for the root filename and then look for a matching TEXT file
                    if (pageNode.Files.Count > 0)
                    {
                        string root = pageNode.Files[0].File_Name_Sans_Extension;
                        if (text_files_existing.ContainsKey(root.ToUpper() + ".TXT"))
                        {
                            try
                            {
                                // SInce this is marked to be included, save this name
                                text_files_included.Add(root.ToUpper() + ".TXT");

                                // Read the page text
                                System.IO.StreamReader reader = new System.IO.StreamReader(File_Location + "\\" + root + ".txt");
                                string pageText = reader.ReadToEnd().Trim();
                                reader.Close();

                                // Look for a matching thumbnail
                                string thumbnail = String.Empty;
                                if (thumbnail_files_existing.ContainsKey(root.ToUpper()))
                                    thumbnail = thumbnail_files_existing[root.ToUpper()];

                                SolrPage newPage = new SolrPage(digitalObject.BibID, digitalObject.VID, pageorder, pageNode.Label, pageText, thumbnail);
                                solrpages.Add(newPage);
                            }
                            catch
                            {

                            }
                        }
                    }

                    // Increment the page order for the next page irregardless
                    pageorder++;
                }
                else
                {
                    // Add the label or type for this division
                    if (thisNode.Label.Length > 0)
                        tocterms.Add(thisNode.Label);
                    else if (thisNode.Type.Length > 0)
                        tocterms.Add(thisNode.Type);
                }
            }

            // Now, check for any other valid text files 
            additional_text_files = new List<string>();
            foreach (string thisTextFile in text_files_existing.Keys)
            {
                if ((!text_files_included.Contains(thisTextFile.ToUpper())) && ( thisTextFile.ToUpper() != "AGREEMENT.TXT") && ( thisTextFile.ToUpper().IndexOf("REQUEST") != 0 ))
                {
                    additional_text_files.Add(thisTextFile);
                }
            }
        }

        /// <summary> Gets the current file location for any associated text files </summary>
        public string File_Location
        {
            get
            {
                return fileLocation;
            }
        }

        /// <summary> Gets the collection of page objects for Solr indexing </summary>
        public List<SolrPage> Solr_Pages
        {
            get
            {
                return solrpages;
            }
        }

        /// <summary> Returns the unique DID for the Solr engine to index for this document </summary>
        [SolrUniqueKey("did")]
        public string Did
        {
            get
            {
                return digitalObject.BibID + ":" + digitalObject.VID;
            }
        }

        /// <summary> Returns the bibliographic identifier (BibID) for the Solr engine to index for this document </summary>
        [SolrField("bibid")]
        public string BibID
        {
            get
            {
                return digitalObject.BibID;
            }
        }

        /// <summary> Returns the volumee identifier (VID) for the Solr engine to index for this document </summary>
        [SolrField("vid")]
        public string VID
        {
            get
            {
                return digitalObject.VID;
            }
        }

        /// <summary> Returns the URL for the Solr engine to index for this document </summary>
        [SolrField("url")]
        public string Url
        {
            get
            {
                return null;
            }
        }

        /// <summary> Returns the IP restriction mask for the Solr engine to index for this document </summary>
        [SolrField("iprestrictionmask")]
        public int Iprestrictionmask
        {
            get
            {
                return digitalObject.SobekCM_Web.IP_Restriction_Membership;
            }
        }

        /// <summary> Returns the dark flag for the Solr engine to index for this document </summary>
        [SolrField("dark")]
        public bool Dark
        {
            get
            {
                return digitalObject.SobekCM_Web.Dark_Flag;
            }
        }

        /// <summary> Returns the list of aggregations for the Solr engine to index for this document </summary>
        [SolrField("aggregation_code")]
        public List<string> Aggregations
        {
            get
            {
                List<string> codes = new List<string>();
                foreach (SobekCM.Bib_Package.SobekCM_Info.Aggregation_Info aggregation in digitalObject.SobekCM_Web.Aggregations)
                {
                    codes.Add(aggregation.Code);
                }
                return codes;
            }
        }

        /// <summary> Returns the list of aggregations for the Solr engine to index for this document </summary>
        [SolrField("aggregations_facet")]
        public List<string> Aggregations_Facet
        {
            get
            {
                List<string> names = new List<string>();
                foreach (SobekCM.Bib_Package.SobekCM_Info.Aggregation_Info aggregation in digitalObject.SobekCM_Web.Aggregations)
                {
                    if (aggregation.Name.Length > 0)
                    {
                        names.Add(aggregation.Name);
                    }
                }
                return names;
            }
        }

        /// <summary> Returns the main title for the Solr engine to index for this document </summary>
        [SolrField("maintitle")]
        public string Maintitle
        {
            get
            {
                return digitalObject.Bib_Info.Main_Title.Title.ToString().Replace("<i>", " ").Replace("</i>", " ");
            }
        }

        /// <summary> Returns all the titles for the Solr engine to index for this document </summary>
        [SolrField("titles")]
        public List<string> Titles
        {
            get
            {
                List<string> allTitles = new List<string>();

                // Add the main title
                allTitles.Add(digitalObject.Bib_Info.Main_Title.Title.ToString().Replace("<i>", " ").Replace("</i>", " "));

                // Add any other titles
                if (digitalObject.Bib_Info.Other_Titles_Count > 0)
                {
                    foreach (SobekCM.Bib_Package.Bib_Info.Title_Info thisTitle in digitalObject.Bib_Info.Other_Titles)
                    {
                        if (thisTitle.Title.Length > 0)
                        {
                            allTitles.Add(thisTitle.ToString().Replace("<i>", " ").Replace("</i>", " "));
                        }
                    }
                }

                // Add the series title
                if ((digitalObject.Bib_Info.hasSeriesTitle) && (digitalObject.Bib_Info.SeriesTitle.Title.Length > 0))
                {
                    allTitles.Add(digitalObject.Bib_Info.SeriesTitle.ToString().Replace("<i>", " ").Replace("</i>", " "));
                }

                return allTitles;
            }
        }

        /// <summary> Returns the material type for the Solr engine to index for this document </summary>
        [SolrField("materialtype")]
        public string MaterialType
        {
            get
            {
                return digitalObject.Bib_Info.SobekCM_Type_String;
            }
        }

        /// <summary> Returns the languages for the Solr engine to index for this document </summary>
        [SolrField("language")]
        public List<string> Language
        {
            get
            {
                List<string> languages = new List<string>();
                foreach (SobekCM.Bib_Package.Bib_Info.Language_Info thisLanguage in digitalObject.Bib_Info.Languages)
                {
                    if (thisLanguage.Language_Text.Length > 0)
                    {
                        languages.Add(thisLanguage.Language_Text);
                    }
                }
                return languages;
            }
        }

        /// <summary> Returns the list of creators (and contributors) for the Solr engine to index for this document </summary>
        [SolrField("creator")]
        public List<string> Creator
        {
            get
            {
                List<string> creators = new List<string>();

                // Add the main entry here
                if ((digitalObject.Bib_Info.hasMainEntityName) && (digitalObject.Bib_Info.Main_Entity_Name.hasData))
                {
                    creators.Add(digitalObject.Bib_Info.Main_Entity_Name.ToString(true).Replace("<i>", " ").Replace("</i>", " "));
                }

                // Add any other names here
                if (digitalObject.Bib_Info.Names_Count > 0)
                {
                    foreach (SobekCM.Bib_Package.Bib_Info.Name_Info thisName in digitalObject.Bib_Info.Names)
                    {
                        if (thisName.hasData)
                        {
                            creators.Add(thisName.ToString(true).Replace("<i>", " ").Replace("</i>", " "));
                        }
                    }
                }

                return creators;
            }
        }

        /// <summary> Returns the PUBLISHER for the Solr engine to index for this document </summary>
        [SolrField("publisher")]
        public List<string> Publisher
        {
            get
            {
                List<string> publishers = new List<string>();

                // Add the publishers
                if (digitalObject.Bib_Info.Publishers_Count > 0)
                {
                    foreach (SobekCM.Bib_Package.Bib_Info.Publisher_Info thisPublisher in digitalObject.Bib_Info.Publishers)
                    {
                        if ((thisPublisher.Name.ToLower().IndexOf("s.n") < 0) || (thisPublisher.Name.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace(",", "").Replace(".", "").Replace(";", "").Replace(":", "").Replace(">", "").Replace("<", "").Length > 3))
                        {
                            publishers.Add(thisPublisher.Name);
                        }
                    }
                }

                // Add any manufacturers here
                if (digitalObject.Bib_Info.Manufacturers_Count > 0)
                {
                    foreach (SobekCM.Bib_Package.Bib_Info.Publisher_Info thisManufacturer in digitalObject.Bib_Info.Manufacturers)
                    {
                        publishers.Add(thisManufacturer.Name);
                    }
                }

                return publishers;
            }
        }

        /// <summary> Returns the publication places for the Solr engine to index for this document </summary>
        [SolrField("pubplace")]
        public List<string> Pubplace
        {
            get
            {
                // Add publishers here
                List<string> places = new List<string>();
                if (digitalObject.Bib_Info.Publishers_Count > 0)
                {
                    foreach (SobekCM.Bib_Package.Bib_Info.Publisher_Info thisPublisher in digitalObject.Bib_Info.Publishers)
                    {
                        if (thisPublisher.Places_Count > 0)
                        {
                            foreach (SobekCM.Bib_Package.Bib_Info.Origin_Info_Place thisPlace in thisPublisher.Places)
                            {
                                if ((thisPlace.Place_Text.Length > 0) && (!places.Contains(thisPlace.Place_Text)))
                                    places.Add(thisPlace.Place_Text);
                            }
                        }
                    }
                }

                // Add any extra places
                if (digitalObject.Bib_Info.Origin_Info.Places_Count > 0)
                {
                    foreach (SobekCM.Bib_Package.Bib_Info.Origin_Info_Place thisPlace in digitalObject.Bib_Info.Origin_Info.Places)
                    {
                        if ((thisPlace.Place_Text.Length > 0) && (!places.Contains(thisPlace.Place_Text)))
                            places.Add(thisPlace.Place_Text);
                    }
                }

                return places;
            }
        }

        /// <summary> Returns the topical subject keywords for the Solr engine to index for this document </summary>
        [SolrField("subjectkeyword")]
        public List<string> Subjectkeyword
        {
            get
            {
                return subjectkeyword;
            }
        }

        /// <summary> Returns the genres for the Solr engine to index for this document </summary>
        [SolrUniqueKey("genre")]
        public List<string> Genre
        {
            get
            {
                return genre;
            }
        }

        /// <summary> Returns the target audiences for the Solr engine to index for this document </summary>
        [SolrField("targetaudience")]
        public List<string> Targetaudience
        {
            get
            {
                List<string> audiences = new List<string>();

                // Add all target audiences
                if (digitalObject.Bib_Info.Target_Audiences_Count > 0)
                {
                    foreach (SobekCM.Bib_Package.Bib_Info.TargetAudience_Info thisAudience in digitalObject.Bib_Info.Target_Audiences)
                    {
                        audiences.Add(thisAudience.Audience);
                    }
                }

                return audiences;
            }
        }

        /// <summary> Returns the spatial coverages for the Solr engine to index for this document </summary>
        [SolrField("spatialcoverage")]
        public List<string> Spatialcoverage
        {
            get
            {
                return spatialcoverage;
            }
        }

        /// <summary> Returns the countries for the Solr engine to index for this document </summary>
        [SolrField("country")]
        public List<string> Country
        {
            get
            {
                return country;
            }
        }

        /// <summary> Returns the states for the Solr engine to index for this document </summary>
        [SolrField("state")]
        public List<string> State
        {
            get
            {
                return state;
            }
        }

        /// <summary> Returns the counties for the Solr engine to index for this document </summary>
        [SolrField("county")]
        public List<string> County
        {
            get
            {
                return county;
            }
        }

        /// <summary> Returns the cities for the Solr engine to index for this document </summary>
        [SolrField("city")]
        public List<string> City
        {
            get
            {
                return city;
            }
        }

        /// <summary> Returns the source institution for the Solr engine to index for this document </summary>
        [SolrField("sourceinstitution")]
        public string Sourceinstitution
        {
            get
            {
                if (digitalObject.Bib_Info.Source.Statement.Length > 0)
                {
                    return digitalObject.Bib_Info.Source.Statement;
                }
                return String.Empty;
            }
        }

        /// <summary> Returns the holding location for the Solr engine to index for this document </summary>
        [SolrField("holdinglocation")]
        public string Holdinglocation
        {
            get
            {
                if (digitalObject.Bib_Info.Location.Holding_Name.Length > 0)
                {
                    return digitalObject.Bib_Info.Location.Holding_Name;
                }
                return String.Empty;
            }
        }

        /// <summary> Returns the identifiers for the Solr engine to index for this document </summary>
        [SolrField("identifier")]
        public List<string> Identifier
        {
            get
            {
                List<string> identifiers = new List<string>();
                if (digitalObject.Bib_Info.Identifiers_Count > 0)
                {
                    foreach (SobekCM.Bib_Package.Bib_Info.Identifier_Info thisIdentifier in digitalObject.Bib_Info.Identifiers)
                    {
                        identifiers.Add(thisIdentifier.Identifier);
                    }
                }

                return identifiers;
            }
        }

        /// <summary> Returns any associated notes for the Solr engine to index for this document </summary>
        [SolrField("notes")]
        public List<string> Notes
        {
            get
            {
                // Add any notes here
                List<string> notes = new List<string>();
                if (digitalObject.Bib_Info.Notes_Count > 0)
                {
                    foreach (SobekCM.Bib_Package.Bib_Info.Note_Info thisNote in digitalObject.Bib_Info.Notes)
                    {
                        notes.Add(thisNote.ToString().Replace("<b>", " ").Replace("</b>", " "));
                    }
                }

                // ADd the physical description as just a note
                if ((digitalObject.Bib_Info.Original_Description != null) && (digitalObject.Bib_Info.Original_Description.Notes_Count > 0))
                {
                    foreach (string thisNote in digitalObject.Bib_Info.Original_Description.Notes)
                    {
                        if (thisNote.Length > 0)
                        {
                            notes.Add(thisNote);
                        }
                    }
                }
                return notes;
            }
        }

        /// <summary> Returns any associated abstracts for the Solr engine to index for this document </summary>
        [SolrField("abstract")]
        public List<string> Abstract
        {
            get
            {
                List<string> abstracts = new List<string>();

                // Add abstracts
                if (digitalObject.Bib_Info.Abstracts_Count > 0)
                {
                    foreach (SobekCM.Bib_Package.Bib_Info.Abstract_Info thisAbstract in digitalObject.Bib_Info.Abstracts)
                    {
                        abstracts.Add(thisAbstract.Abstract_Text);
                    }
                }

                return abstracts;
            }
        }

        /// <summary> Returns the OTHERCITATION for the Solr engine to index for this document </summary>
        [SolrField("othercitation")]
        public List<string> Othercitation
        {
            get
            {
                List<string> othercitations = new List<string>();

                // Add any location information that doesn't go anywhere else
                if (digitalObject.Bib_Info.hasLocationInformation)
                {
                    // Add any other URL display label here
                    if (digitalObject.Bib_Info.Location.Other_URL_Display_Label.Length > 0)
                    {
                        othercitations.Add(digitalObject.Bib_Info.Location.Other_URL_Display_Label);
                    }

                    // Add any other URL note here
                    if (digitalObject.Bib_Info.Location.Other_URL_Note.Length > 0)
                    {
                        othercitations.Add(digitalObject.Bib_Info.Location.Other_URL_Note);
                    }

                    // Add any EAD name here
                    if (digitalObject.Bib_Info.Location.EAD_Name.Length > 0)
                    {
                        othercitations.Add(digitalObject.Bib_Info.Location.EAD_Name);
                    }
                }

                // Add the edition here
                if (digitalObject.Bib_Info.Origin_Info.Edition.Length > 0)
                {
                    othercitations.Add(digitalObject.Bib_Info.Origin_Info.Edition);
                }

                return othercitations;
            }
        }

        /// <summary> Returns the donor for the Solr engine to index for this document </summary>
        [SolrField("donor")]
        public string Donor
        {
            get
            {
                // Add the donor information
                if ((digitalObject.Bib_Info.hasDonor) && (digitalObject.Bib_Info.Donor.hasData))
                {
                    return digitalObject.Bib_Info.Donor.ToString();
                }
                return null;
            }
        }

        /// <summary> Returns the format for the Solr engine to index for this document </summary>
        [SolrField("edition")]
        public string Edition
        {
            get
            {
                // Now, add the format part
                if (digitalObject.Bib_Info.Origin_Info.Edition.Length > 0)
                {
                    return digitalObject.Bib_Info.Origin_Info.Edition;
                }
                return null;
            }
        }

        /// <summary> Returns the format for the Solr engine to index for this document </summary>
        [SolrField("format")]
        public string Format
        {
            get
            {
                // Now, add the format part
                if (digitalObject.Bib_Info.Original_Description.Extent.Length > 0)
                {
                    return digitalObject.Bib_Info.Original_Description.Extent;
                }
                return null;
            }
        }

        /// <summary> Returns the publication date as a displayable string for the Solr engine to index for this document </summary>
        [SolrField("pubdate_display")]
        public string PubDate_Display
        {
            get
            {
                if (digitalObject.Bib_Info.Origin_Info.Date_Issued.Length > 0)
                    return digitalObject.Bib_Info.Origin_Info.Date_Issued;
                else
                    return null;
            }
        }

        /// <summary> Returns the publication date for the Solr engine to index for this document </summary>
        [SolrField("pubdate")]
        public Nullable<DateTime> PubDate
        {
            get
            {
                // If there is no date, do nothing
                string dateString = digitalObject.Bib_Info.Origin_Info.Date_Issued;
                if (dateString.Trim().Length == 0)
                {
                    return null;
                }

                // First, check to see if this is a proper date already
                try
                {
                    // Try conversion
                    return Convert.ToDateTime(dateString);
                }
                catch
                {
                    // Didn't work, so need to try and find a year at least
                    dateString = dateString.Replace("[", "").Replace("]", "").Replace("(", "").Replace(")", "");
                    dateString = dateString.ToUpper();
                    dateString = dateString.Replace("circa", "").Replace("ca", "").Replace("c", "");
                    dateString = dateString.Replace(".", "").Replace(",", "").Replace("-", "").Trim();

                    // Step through looking for first four digits
                    int start = -1;
                    for (int i = 0; i < dateString.Length; i++)
                    {
                        if ((Char.IsNumber(dateString[i])) || (dateString[i] == '-') ||
                            (dateString[i] == 'X') || (dateString[i] == '?') || (dateString[i] == 'U'))
                        {
                            if (start < 0)
                            {
                                start = i;
                            }
                        }
                        else
                        {
                            // Did this include four digits?
                            if ((start >= 0) && ((i - start) >= 4))
                            {
                                // You can stop
                                break;
                            }
                            else
                            {
                                start = -1;
                            }
                        }
                    }

                    // If a start was found, use it
                    if ((start >= 0) && ((dateString.Length - start) >= 4))
                    {
                        string year = dateString.Substring(start, 4).Replace("X", "0").Replace("?", "0").Replace("U", "0").Replace("-", "0");
                        return new DateTime(Convert.ToInt16(year), 1, 1);
                    }
                }

                // Return this value, as empty
                return null;
            }
        }

        /// <summary> Returns the affiliation for the Solr engine to index for this document </summary>
        [SolrField("affiliation")]
        public List<string> Affiliation
        {
            get
            {
                List<string> affiliations = new List<string>();

                // Add the affiliation information
                if (digitalObject.Bib_Info.Affiliations_Count > 0)
                {
                    foreach (SobekCM.Bib_Package.Bib_Info.Affiliation_Info thisAffiliation in digitalObject.Bib_Info.Affiliations)
                    {
                        affiliations.Add(thisAffiliation.ToString());
                    }
                }
                
                return affiliations;
            }
        }

        /// <summary> Returns the frequency of publication for the Solr engine to index for this document </summary>
        [SolrField("frequency")]
        public List<string> Frequency
        {
            get
            {
                List<string> frequency = new List<string>();

                // Add any frequency here
                if (digitalObject.Bib_Info.Origin_Info.Frequencies_Count > 0)
                {
                    foreach (SobekCM.Bib_Package.Bib_Info.Origin_Info_Frequency thisFrequency in digitalObject.Bib_Info.Origin_Info.Frequencies)
                    {
                        frequency.Add(thisFrequency.Term);
                    }
                }

                return frequency;
            }
        }

        /// <summary> Returns the name used as subjects for the Solr engine to index for this document </summary>
        [SolrField("namesubject")]
        public List<string> Namesubject
        {
            get
            {
                return namesubject;
            }
        }

        /// <summary> Returns the titles used as subjects for the Solr engine to index for this document </summary>
        [SolrField("titlesubject")]
        public List<string> Titlesubject
        {
            get
            {
                return titlesubject;
            }
        }

        /// <summary> Returns the list of all subjects for the Solr engine to index for this document </summary>
        [SolrField("allsubjects")]
        public List<string> Allsubjects
        {
            get
            {
                return allsubjects;
            }
        }

        /// <summary> Returns the temporal subjects for the Solr engine to index for this document </summary>
        [SolrField("temporalsubject")]
        public List<string> Temporalsubject
        {
            get
            {
                return temporalsubject;
            }
        }

        /// <summary> Returns the rights/attribution for the Solr engine to index for this document </summary>
        [SolrField("attribution")]
        public string Attribution
        {
            get
            {
                // Add the rights statement
                if (digitalObject.Bib_Info.Access_Condition.Text.Length > 0)
                {
                    return digitalObject.Bib_Info.Access_Condition.Text;
                }
                return String.Empty;
            }
        }

        /// <summary> Returns the temporal decadees for the Solr engine to index for this document </summary>
        [SolrField("temporaldecade")]
        public List<string> Temporaldecade
        {
            get
            {
                List<string> decades = new List<string>();

                return decades;
            }
        }

        /// <summary> Returns the mime types for the Solr engine to index for this document </summary>
        [SolrField("mimetype")]
        public List<string> MimeType
        {
            get
            {
                List<string> mimetypes = new List<string>();

                // Add any download MIME Types
                if (digitalObject.Divisions.Download_Tree.Has_Files)
                {
                    List<SobekCM.Bib_Package.Divisions.SobekCM_File_Info> allDownloads = digitalObject.Divisions.Download_Tree.All_Files;
                    foreach (SobekCM.Bib_Package.Divisions.SobekCM_File_Info thisDownload in allDownloads)
                    {
                        string thisMimeType = thisDownload.MIME_Type(thisDownload.File_Extension);
                        if ((thisMimeType.Length > 0) && (!mimetypes.Contains(thisMimeType)))
                        {
                            mimetypes.Add(thisMimeType);
                        }
                    }
                }

                return mimetypes;
            }
        }

        /// <summary> Returns the table of contents division and page names for the Solr engine to index for this document </summary>
        [SolrField("toc")]
        public List<string> TOC
        {
            get
            {
                List<string> toc = new List<string>();
                return toc;
            }
        }

        /// <summary> Returns the spatial kml for the Solr engine to index for this document </summary>
        [SolrField("spatialkml")]
        public string SpatialKML
        {
            get
            {
                if (digitalObject.Bib_Info.hasCoordinateInformation)
                {
                    return digitalObject.Bib_Info.Coordinates.SobekCM_Main_Spatial_String;
                }
                return null;
            }
        }

        /// <summary> Returns the display list of authors for the Solr engine to store for this document </summary>
        [SolrField("author_display")]
        public string Author_Display
        {
            get
            {
                // Get the authors
                StringBuilder author_builder = new StringBuilder();
                string mainAuthor = String.Empty;
                if (digitalObject.Bib_Info.hasMainEntityName)
                    mainAuthor = digitalObject.Bib_Info.Main_Entity_Name.ToString();
                if ((mainAuthor.Length > 0) && (mainAuthor.IndexOf("unknown") < 0))
                {
                    author_builder.Append(mainAuthor);
                }
                if (digitalObject.Bib_Info.Names_Count > 0)
                {
                    foreach (Bib_Info.Name_Info thisAuthor in digitalObject.Bib_Info.Names)
                    {
                        string thisAuthorString = thisAuthor.ToString();
                        if ((thisAuthorString.Length > 0) && (thisAuthorString.IndexOf("unknown") < 0))
                        {
                            if (author_builder.Length > 0)
                            {
                                author_builder.Append("|" + thisAuthorString);
                            }
                            else
                            {
                                author_builder.Append(thisAuthorString);
                            }
                        }
                    }
                }

                if (author_builder.Length > 0)
                    return author_builder.ToString();
                else
                    return null;
            }
        }

        /// <summary> Returns the display list of publishers for the Solr engine to store for this document </summary>
        [SolrField("publisher_display")]
        public string Publisher_Display
        {
            get
            {
                // Get the publishers
                StringBuilder publisher_builder = new StringBuilder();
                foreach (Bib_Info.Publisher_Info thisPublisher in digitalObject.Bib_Info.Publishers)
                {
                    if (publisher_builder.Length > 0)
                    {
                        publisher_builder.Append("|" + thisPublisher.ToString());
                    }
                    else
                    {
                        publisher_builder.Append(thisPublisher.ToString());
                    }
                }

                if (publisher_builder.Length > 0)
                    return publisher_builder.ToString();
                else
                    return null;
            }
        }

        /// <summary> Returns the main thumbnal for the Solr engine to store for this document </summary>
        [SolrField("mainthumbnail")]
        public string MainThumbnail
        {
            get
            {
                return digitalObject.SobekCM_Web.Main_Thumbnail;
            }
        }

        /// <summary> Returns the text for the first serial hierarchy for the Solr engine to index for this document </summary>
        [SolrField("level1text")]
        public string Level1text
        {
            get
            {
                if (digitalObject.Serial_Info.Count > 0)
                {
                    return digitalObject.Serial_Info[0].Display;
                }
                return null;
            }
        }

        /// <summary> Returns the index for the first serial hierarchy for the Solr engine to index for this document </summary>
        [SolrField("level1index")]
        public Nullable<int> Level1index
        {
            get
            {
                if (digitalObject.Serial_Info.Count > 0)
                {
                    return digitalObject.Serial_Info[0].Order;
                }
                return null;
            }
        }

        /// <summary> Returns the text for the second serial hierarchy for the Solr engine to index for this document </summary>
        [SolrField("level2text")]
        public string Level2text
        {
            get
            {
                if (digitalObject.Serial_Info.Count > 1)
                {
                    return digitalObject.Serial_Info[1].Display;
                }
                return null;
            }
        }

        /// <summary> Returns the index for the second serial hierarchy for the Solr engine to index for this document </summary>
        [SolrField("level2index")]
        public Nullable<int> Level2index
        {
            get
            {
                if (digitalObject.Serial_Info.Count > 1)
                {
                    return digitalObject.Serial_Info[1].Order;
                }
                return null;
            }
        }

        /// <summary> Returns the text for the third serial hierarchy for the Solr engine to index for this document </summary>
        [SolrField("level3text")]
        public string Level3text
        {
            get
            {
                if (digitalObject.Serial_Info.Count > 2)
                {
                    return digitalObject.Serial_Info[2].Display;
                }
                return null;
            }
        }

        /// <summary> Returns the index for the third serial hierarchy for the Solr engine to index for this document </summary>
        [SolrField("level3index")]
        public Nullable<int> Level3index
        {
            get
            {
                if (digitalObject.Serial_Info.Count > 2)
                {
                    return digitalObject.Serial_Info[2].Order;
                }
                return null;
            }
        }

        /// <summary> Returns the ALEPH number for the Solr engine to index for this document </summary>
        [SolrField("aleph")]
        public Nullable<long> ALEPH
        {
            get
            {
                string aleph_string = digitalObject.Bib_Info.ALEPH_Record;
                if ( aleph_string.Length > 0 )
                {
                    foreach( char thisChar in aleph_string )
                    {
                        if ( !char.IsNumber(thisChar))
                            return null;
                    }
                    try
                    {
                        return Convert.ToInt64( aleph_string );
                    }
                    catch
                    {

                    }
                }
                return null;
            }
        }

        /// <summary> Returns the OCLC number for the Solr engine to index for this document </summary>
        [SolrField("oclc")]
        public Nullable<long> OCLC
        {
            get
            {
                string oclc_string = digitalObject.Bib_Info.OCLC_Record;
                if (oclc_string.Length > 0)
                {
                    foreach (char thisChar in oclc_string)
                    {
                        if (!char.IsNumber(thisChar))
                            return null;
                    }
                    try
                    {
                        return Convert.ToInt64(oclc_string);
                    }
                    catch
                    {

                    }
                }
                return null;
            }
        }

        /// <summary> Returns the full text for all the pages within this document for the Solr engine to index for this document </summary>
        [SolrField("fulltext")]
        public string FullText
        {
            get
            {
                if (((solrpages == null) || (solrpages.Count == 0)) && ( additional_text_files.Count == 0 ))
                    return null;

                StringBuilder builder = new StringBuilder(10000);

                // Add the text for each page 
                foreach (SolrPage thisPage in solrpages)
                {
                    builder.Append( thisPage.PageText );
                    builder.Append(" ");
                }

                // Also add the text from any other text files
                foreach( string textFile in additional_text_files )
                {
                    try
                    {
                        System.IO.StreamReader reader = new System.IO.StreamReader(fileLocation + "\\" + textFile);
                        builder.Append(reader.ReadToEnd() + " ");
                        reader.Close();
                    }
                    catch (Exception ee)
                    {
                        bool error = true;
                    }
                }
                   
                return builder.ToString();
            }
        }        
    }
}
