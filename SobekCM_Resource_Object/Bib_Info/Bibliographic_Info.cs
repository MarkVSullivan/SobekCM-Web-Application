#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.VRACore;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> Stores the descriptive metadata for the bibliographic resource </summary>
    /// <remarks> This class extends the <see cref="MODS_Info"/> class and includes additional elements which 
    /// can be expressed in the SobekCM custom schema within a METS file.<br /> <br /> 
    /// Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
    public class Bibliographic_Info : MODS_Info, iMetadata_Module
    {
        private List<Affiliation_Info> affiliations;
        private string bibID;
        private List<Finding_Guide_Container> containers;
 
        private string encodingLevel;
        private List<Publisher_Info> manufacturers;
        private List<Publisher_Info> publishers;
        private int sortDate;
        private string sortTitle;
	    private List<Temporal_Info> temporalSubjects;
        private string vid;

        #region Constructors

        /// <summary> Constructor for a new instance of the Bibliographic_Info class. </summary>
        public Bibliographic_Info()
        {
            sortDate = -1;
            Source = new Source_Info();
        }

        #endregion

        #region Methods/Properties to implement iMetadata_Module

        /// <summary> Name for this metadata module </summary>
        /// <value> This always returns 'VRACore'</value>
        public string Module_Name
        {
            get { return "Main Bibliographic Metadata"; }
        }

        /// <summary> Gets the metadata search terms and values to be saved to the database
        /// to allow searching to occur over the data in this metadata module </summary>
        public List<KeyValuePair<string, string>> Metadata_Search_Terms
        {
            get
            {
                List<KeyValuePair<string, string>> metadataTerms = new List<KeyValuePair<string, string>>();

                // Add abstracts
                if (Abstracts_Count > 0)
                {
                    foreach (Abstract_Info thisAbstract in Abstracts)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Abstract", thisAbstract.Abstract_Text));
                    }
                }

                // Add the rights statement
                if (Access_Condition.Text.Length > 0)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Other Citation", Access_Condition.Text));
                }

                // Add the affiliation information
                if (Affiliations_Count > 0)
                {
                    foreach (Affiliation_Info thisAffiliation in Affiliations)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Affiliation", thisAffiliation.ToString()));
                    }
                }

                // Add the donor information
                if ((hasDonor) && (Donor.hasData))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Donor", Donor.ToString(false)));
                }

                // Add the genre information
                if (Genres_Count > 0)
                {
                    foreach (Genre_Info thisGenre in Genres)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Genre", thisGenre.Genre_Term.ToLower()));
                    }
                }

                // Add the identifiers
                if (Identifiers_Count > 0)
                {
                    foreach (Identifier_Info thisIdentifier in Identifiers)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Identifier", thisIdentifier.Identifier));
                    }
                }

                // Add the languages
                if (Languages_Count > 0)
                {
                    foreach (Language_Info thisLanguage in Languages)
                    {
                        if (thisLanguage.Language_Text.Length > 0)
                        {
                            metadataTerms.Add(new KeyValuePair<string, string>("Language", thisLanguage.Language_Text));
                        }
                    }
                }

                if (hasLocationInformation)
                {
                    // Add any EAD name here
                    if (Location.EAD_Name.Length > 0)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("EAD Name", Location.EAD_Name));
                    }

                    // Add any holding location information here
                    if (Location.Holding_Name.Length > 0)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Holding Location", Location.Holding_Name));
                    }

                    // Add any other URL display label here
                    if (Location.Other_URL_Display_Label.Length > 0)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Other Citation", Location.Other_URL_Display_Label));
                    }

                    // Add any other URL note here
                    if (Location.Other_URL_Note.Length > 0)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Other Citation", Location.Other_URL_Note));
                    }
                }

                // Add the main entry here
                if ((hasMainEntityName) && (Main_Entity_Name.hasData))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Creator", Main_Entity_Name.ToString(true).Replace("<i>", " ").Replace("</i>", " ")));
                }

                // Add the main title here
                if (Main_Title.Title.Length > 0)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Title", Main_Title.Title.Replace("<i>", " ").Replace("</i>", " ")));
                }

                // Add any manufacturers here
                if (Manufacturers_Count > 0)
                {
                    foreach (Publisher_Info thisManufacturer in Manufacturers)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Publisher", thisManufacturer.Name));
                    }
                }

                // Add any other names here
                if (Names_Count > 0)
                {
                    foreach (Name_Info thisName in Names)
                    {
                        if (thisName.hasData)
                        {
                            metadataTerms.Add(new KeyValuePair<string, string>("Creator", thisName.ToString(true).Replace("<i>", " ").Replace("</i>", " ")));
                        }
                    }
                }

                // Add any notes here
                if (Notes_Count > 0)
                {
                    foreach (Note_Info thisNote in Notes)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Notes", thisNote.ToString().Replace("<b>", " ").Replace("</b>", " ")));
                    }
                }

                // Add any frequency here
                if (Origin_Info.Frequencies_Count > 0)
                {
                    foreach (Origin_Info_Frequency thisFrequency in Origin_Info.Frequencies)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Frequency", thisFrequency.Term));
                    }
                }

                // Add the edition here
                if (Origin_Info.Edition.Length > 0)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Edition", Origin_Info.Edition));
                }

                // Add publishers and the display publisher here here
                List<string> places = new List<string>();
                if (Publishers_Count > 0)
                {
                    foreach (Publisher_Info thisPublisher in Publishers)
                    {
                        if ((thisPublisher.Name.ToLower().IndexOf("s.n") < 0) || (thisPublisher.Name.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace(",", "").Replace(".", "").Replace(";", "").Replace(":", "").Replace(">", "").Replace("<", "").Length > 3))
                        {
                            metadataTerms.Add(new KeyValuePair<string, string>("Publisher", thisPublisher.Name));

							metadataTerms.Add(new KeyValuePair<string, string>("Publisher.Display", thisPublisher.ToString().Replace("["," ").Replace("]"," ")));
                        }

                        if (thisPublisher.Places_Count > 0)
                        {
                            foreach (Origin_Info_Place thisPlace in thisPublisher.Places)
                            {
                                if ((thisPlace.Place_Text.Length > 0) && (!places.Contains(thisPlace.Place_Text)))
                                    places.Add(thisPlace.Place_Text);
                            }
                        }


                    }
                }

                // Add any extra places
                if (Origin_Info.Places_Count > 0)
                {
                    foreach (Origin_Info_Place thisPlace in Origin_Info.Places)
                    {
                        if ((thisPlace.Place_Text.Length > 0) && (!places.Contains(thisPlace.Place_Text)))
                            places.Add(thisPlace.Place_Text);
                    }
                }

                // Now, add the publication places
                foreach (string thisPlace in places)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Publication Place", thisPlace));
                }

                // Now, add the format part
                if (Original_Description.Extent.Length > 0)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Format", Original_Description.Extent));
                }

                // ADd the physical description as just a note
                if ((Original_Description != null) && (Original_Description.Notes_Count > 0))
                {
                    foreach (string thisNote in Original_Description.Notes)
                    {
                        if (thisNote.Length > 0)
                        {
                            metadataTerms.Add(new KeyValuePair<string, string>("Notes", thisNote));
                        }
                    }
                }

                // Add any other titles
                if (Other_Titles_Count > 0)
                {
                    foreach (Title_Info thisTitle in Other_Titles)
                    {
                        if (thisTitle.Title.Length > 0)
                        {
                            metadataTerms.Add(new KeyValuePair<string, string>("Title", thisTitle.ToString().Replace("<i>", " ").Replace("</i>", " ")));
                        }
                    }
                }

                // Add the series title
                if ((hasSeriesTitle) && (SeriesTitle.Title.Length > 0))
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Title", SeriesTitle.ToString().Replace("<i>", " ").Replace("</i>", " ")));
                }

                // Add the source statement
                if (Source.Statement.Length > 0)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Source Institution", Source.Statement));
                }

                // Add the subjects
                List<string> country = new List<string>();
                List<string> state = new List<string>();
                List<string> county = new List<string>();
                List<string> city = new List<string>();
                List<string> continent = new List<string>();
                List<string> island = new List<string>();
				List<string> spatials_display = new List<string>();
				List<string> subjects_display = new List<string>();
                if (Subjects_Count > 0)
                {
                    foreach (Subject_Info thisSubject in Subjects)
                    {
                        // Add every subject to the complete list of subjects
                        metadataTerms.Add(new KeyValuePair<string, string>("All Subjects", thisSubject.ToString(false)));

                        // Add name subjects
                        if (thisSubject.Class_Type == Subject_Info_Type.Name)
                        {
                            metadataTerms.Add(new KeyValuePair<string, string>("Name as Subject", thisSubject.ToString(false)));
	                        string complete = thisSubject.ToString(false);
	                        if ((complete.Length > 0) && (!subjects_display.Contains(complete)))
		                        subjects_display.Add(complete);
                        }

                        // Add title subjects
                        if (thisSubject.Class_Type == Subject_Info_Type.TitleInfo)
                        {
                            metadataTerms.Add(new KeyValuePair<string, string>("Title as Subject", thisSubject.ToString(false)));
							string complete = thisSubject.ToString(false);
							if ((complete.Length > 0) && (!subjects_display.Contains(complete)))
								subjects_display.Add(complete);
                        }

                        // Add the subject keywords
                        if ((thisSubject.Class_Type == Subject_Info_Type.Standard) && (thisSubject.ID.IndexOf("690") < 0) && (thisSubject.ID.IndexOf("691") < 0))
                        {
                            // Cast to the hierarchical subject type
                            Subject_Info_Standard standSubj = (Subject_Info_Standard) thisSubject;

                            if (standSubj.Genres_Count > 0)
                            {
                                foreach (string genreTerm in standSubj.Genres)
                                {
                                    metadataTerms.Add(new KeyValuePair<string, string>("Genre", genreTerm.ToLower()));
                                }
                            }
                            if (standSubj.Geographics_Count > 0)
                            {
								StringBuilder thisSpatialBuilder = new StringBuilder();
                                foreach (string geoTerm in standSubj.Geographics)
                                {
	                                if (geoTerm.Length > 0)
	                                {
		                                metadataTerms.Add(new KeyValuePair<string, string>("Spatial Coverage", geoTerm));
		                                if (thisSpatialBuilder.Length == 0)
			                                thisSpatialBuilder.Append(geoTerm);
		                                else
			                                thisSpatialBuilder.Append(" -- " + geoTerm);
	                                }
                                }
	                            string complete2 = thisSpatialBuilder.ToString();
								if ((complete2.Length > 0) && (!spatials_display.Contains(complete2)))
									spatials_display.Add(complete2);
                            }
                            if (standSubj.Topics_Count > 0)
                            {
                                foreach (string topicTerm in standSubj.Topics)
                                {
                                    metadataTerms.Add(new KeyValuePair<string, string>("Subject Keyword", topicTerm));
                                }
                            }

							string complete = thisSubject.ToString(false);
							if ((complete.Length > 0) && (!subjects_display.Contains(complete)))
								subjects_display.Add(complete);
                        }

                        // Add hierarchical spatial info
                        if (thisSubject.Class_Type == Subject_Info_Type.Hierarchical_Spatial)
                        {
                            // Cast to the hierarchical subject type
                            Subject_Info_HierarchicalGeographic hiero = (Subject_Info_HierarchicalGeographic) thisSubject;

                            // Check for existing subfacets and add if not there
                            if ((hiero.Continent.Length > 0) && (!continent.Contains(hiero.Continent)))
                            {
                                metadataTerms.Add(new KeyValuePair<string, string>("Spatial Coverage", hiero.Continent));
                                continent.Add(hiero.Continent);
                            }
                            if ((hiero.Country.Length > 0) && (!country.Contains(hiero.Country)))
                            {
                                metadataTerms.Add(new KeyValuePair<string, string>("Spatial Coverage", hiero.Country));
                                country.Add(hiero.Country);
                            }
                            if ((hiero.State.Length > 0) && (!state.Contains(hiero.State)))
                            {
                                metadataTerms.Add(new KeyValuePair<string, string>("Spatial Coverage", hiero.State));
                                state.Add(hiero.State);
                            }
                            if ((hiero.County.Length > 0) && (!county.Contains(hiero.County)))
                            {
                                metadataTerms.Add(new KeyValuePair<string, string>("Spatial Coverage", hiero.County));
                                county.Add(hiero.County);
                            }
                            if ((hiero.City.Length > 0) && (!city.Contains(hiero.City)))
                            {
                                metadataTerms.Add(new KeyValuePair<string, string>("Spatial Coverage", hiero.City));
                                city.Add(hiero.City);
                            }
                            if ((hiero.Island.Length > 0) && (!island.Contains(hiero.Island)))
                            {
                                metadataTerms.Add(new KeyValuePair<string, string>("Spatial Coverage", hiero.Island));
                                island.Add(hiero.Island);
                            }

							// Add the complete spatial as a display
	                        string complete = hiero.ToString();
							if ((complete.Length > 0) && (!spatials_display.Contains(complete)))
							{
								spatials_display.Add(complete);
				            }
                        }
                    }
                }

                // Add all the subfacets
                foreach (string thisCountry in country)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Country", thisCountry));
                }
                foreach (string thisState in state)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("State", thisState));
                }
                foreach (string thisCounty in county)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("County", thisCounty));
                }
                foreach (string thisCity in city)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("City", thisCity));
                }

				// Add the display spatial coverage
				foreach (string thisSpatial in spatials_display)
				{
					metadataTerms.Add(new KeyValuePair<string, string>("Spatial Coverage.Display", thisSpatial));
				}

				// Add the display subjects
				foreach (string thisSubject in subjects_display)
				{
					metadataTerms.Add(new KeyValuePair<string, string>("Subjects.Display", thisSubject));
				}

                // Add all target audiences
                if (Target_Audiences_Count > 0)
                {
                    foreach (TargetAudience_Info thisAudience in Target_Audiences)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Target Audience", thisAudience.Audience));
                    }
                }

                // Add all the temporal subjects
                if (TemporalSubjects_Count > 0)
                {
                    foreach (Temporal_Info thisTemporal in TemporalSubjects)
                    {
                        if (thisTemporal.TimePeriod.Length > 0)
                        {
                            metadataTerms.Add(new KeyValuePair<string, string>("All Subjects", thisTemporal.TimePeriod));
                            metadataTerms.Add(new KeyValuePair<string, string>("Temporal Subject", thisTemporal.TimePeriod));
                        }
                    }
                }

                // Add the resource type
                string sobekcm_type_string = SobekCM_Type_String;
                if (sobekcm_type_string.Length > 0)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Type", sobekcm_type_string));
                }

                // Add any edition here
                if (Origin_Info.Edition.Length > 0)
                {
                    metadataTerms.Add(new KeyValuePair<string, string>("Edition", Origin_Info.Edition));
                }

				// Get the pub date and year
				string pubdate = Origin_Info.Date_Check_All_Fields;
	            if (pubdate.Length > 0)
	            {
		            metadataTerms.Add(new KeyValuePair<string, string>("Publication Date", pubdate));

		            // Try to get the year
		            int year = -1;

		            if (pubdate.Length == 4)
		            {
			            Int32.TryParse(pubdate, out year);
		            }

		            if (year == -1)
		            {
			            DateTime date;
			            if (DateTime.TryParse(pubdate, out date))
			            {
				            year = date.Year;
			            }
		            }

					if (year != -1)
					{
						metadataTerms.Add(new KeyValuePair<string, string>("Temporal Year", year.ToString()));
					}
	            }

	            return metadataTerms;
            }
        }

        /// <summary> Chance for this metadata module to perform any additional database work
        /// such as saving digital resource data into custom tables </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString"> Connection string for the current database </param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        /// <remarks> This module currently  does no additional processing in this method </remarks>
        public bool Save_Additional_Info_To_Database(int ItemID, string DB_ConnectionString, SobekCM_Item BibObject, out string Error_Message)
        {
            // Set the default error mesasge
            Error_Message = String.Empty;

            return true;
        }

        /// <summary> Chance for this metadata module to load any additional data from the 
        /// database when building this digital resource  in memory </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString">Connection string for the current database</param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        /// <remarks> This module currently  does no additional processing in this method </remarks>
        public bool Retrieve_Additional_Info_From_Database(int ItemID, string DB_ConnectionString, SobekCM_Item BibObject, out string Error_Message)
        {
            // Set the default error mesasge
            Error_Message = String.Empty;

            return true;
        }

        #endregion

        #region Finding Guide Container properties and methods

        /// <summary> Get the number of finding guide containers associated with this resource </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Containers"/> property.  Even if 
        /// there are no finding guide containers, the Containers property creates a readonly collection to pass back out.</remarks>
        public int Containers_Count
        {
            get {
	            return containers == null ? 0 : containers.Count;
            }
        }

        /// <summary> Gets the list of finding guide containers </summary>
        /// <remarks> You should check the count of containers first using the <see cref="Containers_Count"/> property before using this property.
        /// Even if there are no containers, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Finding_Guide_Container> Containers
        {
            get {
	            return containers == null ? new ReadOnlyCollection<Finding_Guide_Container>(new List<Finding_Guide_Container>()) : new ReadOnlyCollection<Finding_Guide_Container>(containers);
            }
        }

        /// <summary> Adds a new finding-guide container to the container list for this item </summary>
		/// <param name="ContainerType">Type of container this represents (i.e., Box, Folder, etc..)</param>
        /// <param name="Name">Name of this container</param>
        /// <param name="Level">Level within the container list that this container resides</param>
        /// <remarks>This can be used later to construct EADs for single item loads</remarks>
        public void Add_Container(string ContainerType, string Name, int Level)
        {
            if (containers == null)
                containers = new List<Finding_Guide_Container>();

			containers.Add(new Finding_Guide_Container(ContainerType, Name, Level));
        }

        /// <summary> Clears all finding-guide containers associated with tis item </summary>
        public void Clear_Containers()
        {
            if (containers != null)
                containers.Clear();
        }

        #endregion

        #region Public Properties

        /// <summary> Flag indicates if this bibliographic section has any data </summary>
        public bool hasData
        {
            get
            {
                return true;
            }
        }

        /// <summary> Gets or sets the encoding level for this record </summary>
        public string EncodingLevel
        {
            get { return encodingLevel ?? String.Empty; }
            set { encodingLevel = value; }
        }

        /// <summary> Gets the records numbers for any and all NOTIS records associated with this item  </summary>
        public string[] NOTIS_Records
        {
            get
            {
	            return identifiers == null ? new string[0] : identifiers.Where(ThisIdentifier => ThisIdentifier.Type.Replace("*", "").ToUpper() == "NOTIS").Select(ThisIdentifier => ThisIdentifier.Identifier).ToArray();
            }
        }

        /// <summary> Gets the ALEPH record number associated with this item </summary>
        public string ALEPH_Record
        {
            get
            {
                if (identifiers == null)
                    return String.Empty;

                foreach (Identifier_Info thisIdentifier in identifiers.Where(ThisIdentifier => ThisIdentifier.Type.Replace("*", "").ToUpper().IndexOf("ALEPH") >= 0))
                {
	                return thisIdentifier.Identifier;
                }
                return String.Empty;
            }
            set
            {
                if (identifiers != null)
                {
                    foreach (Identifier_Info thisIdentifier in identifiers.Where(ThisIdentifier => ThisIdentifier.Type.Replace("*", "").ToUpper().IndexOf("ALEPH") >= 0))
                    {
	                    thisIdentifier.Identifier = value;
	                    return;
                    }
                }
                Add_Identifier(value, "ALEPH");
            }
        }

        /// <summary> Gets the OCLC record number associated with this item </summary>
        public string OCLC_Record
        {
            get
            {
                if (identifiers == null)
                    return String.Empty;

                foreach (Identifier_Info thisIdentifier in identifiers)
                {
                    if (thisIdentifier.Type.Replace("*", "").ToUpper() == "OCLC")
                    {
                        return thisIdentifier.Identifier;
                    }
                }
                return String.Empty;
            }
            set
            {
                if (identifiers != null)
                {
                    foreach (Identifier_Info thisIdentifier in identifiers)
                    {
                        if (thisIdentifier.Type.Replace("*", "").ToUpper().IndexOf("OCLC") >= 0)
                        {
                            thisIdentifier.Identifier = value;
                            return;
                        }
                    }
                }
                Add_Identifier(value, "OCLC");
            }
        }

        /// <summary> Get the flag indicating if this is an image class resource </summary>
        /// <remarks> This is based on the type attribute </remarks>
        public bool ImageClass
        {
            get
            {
	            TypeOfResource_SobekCM_Enum sobekcm_type = SobekCM_Type;
	            return (sobekcm_type == TypeOfResource_SobekCM_Enum.Artifact) || (sobekcm_type == TypeOfResource_SobekCM_Enum.Aerial) || (sobekcm_type == TypeOfResource_SobekCM_Enum.Map) || (sobekcm_type == TypeOfResource_SobekCM_Enum.Photograph);
            }
        }

        /// <summary> Gets the number of affiliations associated with this resource </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Affiliations"/> property.  Even if 
        /// there are no affiliations, the Affiliations property creates a readonly collection to pass back out.</remarks>
        public int Affiliations_Count
        {
            get {
	            return affiliations == null ? 0 : affiliations.Count;
            }
        }

        /// <summary> Gets the collection of affiliations associated with this resource </summary>
        /// <remarks> You should check the count of affiliations first using the <see cref="Affiliations_Count"/> property before using this property.
        /// Even if there are no affiliations, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Affiliation_Info> Affiliations
        {
            get {
	            return affiliations == null ? new ReadOnlyCollection<Affiliation_Info>(new List<Affiliation_Info>()) : new ReadOnlyCollection<Affiliation_Info>(affiliations);
            }
        }

        /// <summary> Gets the number of temporal subjects associated with this resource </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="TemporalSubjects"/> property.  Even if 
        /// there are no temporal subjects, the TemporalSubjects property creates a readonly collection to pass back out.</remarks>
        public int TemporalSubjects_Count
        {
            get {
	            return temporalSubjects == null ? 0 : temporalSubjects.Count;
            }
        }

        /// <summary> Gets the collection of temporal subjects associated with this resource </summary>
        /// <remarks> You should check the count of temporal subjects first using the <see cref="TemporalSubjects_Count"/> property before using this property.
        /// Even if there are no temporal subjects, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Temporal_Info> TemporalSubjects
        {
            get {
	            return temporalSubjects == null ? new ReadOnlyCollection<Temporal_Info>(new List<Temporal_Info>()) : new ReadOnlyCollection<Temporal_Info>(temporalSubjects);
            }
        }

	    /// <summary> Gets the source insitution information associated with this resource </summary>
	    public Source_Info Source { get; private set; }


	    /// <summary> Gets the number of publishers associated with this resource </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Publishers"/> property.  Even if 
        /// there are no publishers, the Publishers property creates a readonly collection to pass back out.</remarks>
        public int Publishers_Count
        {
            get {
	            return publishers == null ? 0 : publishers.Count;
            }
        }

        /// <summary> Gets the list of publishers associated with this resource </summary>
        /// <remarks> You should check the count of publishers first using the <see cref="Publishers_Count"/> property before using this property.
        /// Even if there are no publishers, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Publisher_Info> Publishers
        {
            get {
	            return publishers == null ? new ReadOnlyCollection<Publisher_Info>(new List<Publisher_Info>()) : new ReadOnlyCollection<Publisher_Info>(publishers);
            }
        }

        /// <summary> Gets the number of manufacturers associated with this resource </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Manufacturers"/> property.  Even if 
        /// there are no manufacturers, the Manufacturers property creates a readonly collection to pass back out.</remarks>
        public int Manufacturers_Count
        {
            get {
	            return manufacturers == null ? 0 : manufacturers.Count;
            }
        }

        /// <summary> Gets the list of manufacturers associated with this resource </summary>
        /// <remarks> You should check the count of manufacturers first using the <see cref="Manufacturers_Count"/> property before using this property.
        /// Even if there are no manufacturers, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Publisher_Info> Manufacturers
        {
            get {
	            return manufacturers == null ? new ReadOnlyCollection<Publisher_Info>(new List<Publisher_Info>()) : new ReadOnlyCollection<Publisher_Info>(manufacturers);
            }
        }

        /// <summary> Gets and sets the sort date value for this resources </summary>
        public int SortDate
        {
            get { return sortDate; }
            set { sortDate = value; }
        }

        /// <summary> Gets and sets the sort title value for this resource </summary>
        public string SortTitle
        {
            get { return sortTitle ?? String.Empty; }
            set { sortTitle = value; }
        }

        /// <summary> Get the citation string, which is all the data fields lumped together
        /// for easy searching. </summary>
        public string Citation_String
        {
            get
            {
                StringBuilder full_citation = new StringBuilder();

                // Add the data from the base class
                full_citation.Append(MODS_Citation_String);

                if (affiliations != null)
                {
                    foreach (Affiliation_Info thisAffiliation in affiliations)
                    {
                        full_citation.Append(thisAffiliation.Affiliation_XML + " | ");
                    }
                }

                full_citation.Append(BibID + " | ");

                full_citation.Append(VID + " | ");

                if (Source != null)
                {
                    full_citation.Append(Source.Code + " | ");

                    full_citation.Append(Source.Statement + " | ");
                }

                if (temporalSubjects != null)
                {
                    foreach (Temporal_Info thisTemporal in temporalSubjects)
                    {
                        full_citation.Append(thisTemporal.TimePeriod + " | ");
                    }
                }

                // Return the built information
                return full_citation.ToString();
            }
        }

        /// <summary> Clear the list of affiliations associated with this item </summary>
        public void Clear_Affiliations()
        {
            if (affiliations != null)
            {
                affiliations.Clear();
            }
        }

        /// <summary> Clear the list of temporal subjects associated with this item </summary>
        public void Clear_TemporalSubjects()
        {
            if (temporalSubjects != null)
            {
                temporalSubjects.Clear();
            }
        }

        /// <summary> Clear the list of publishers associated with this item </summary>
        public void Clear_Publishers()
        {
            if (publishers != null)
            {
                publishers.Clear();
            }
            ModsOriginInfo.Clear_Places_And_Publishers();
        }

        /// <summary> Clear the list of manufacturers associated with this item </summary>
        public void Clear_Manufacturers()
        {
            if (manufacturers != null)
            {
                manufacturers.Clear();
            }
        }

        /// <summary> Adds a new affiliation object directly to this item </summary>
        /// <param name="New_Affiliation"> Affiliation object to add to this item </param>
        /// <returns> Affiliation object, either the one passed in or one that equals it already in the list </returns>
        public Affiliation_Info Add_Affiliation(Affiliation_Info New_Affiliation)
        {
	        if (affiliations == null)
                affiliations = new List<Affiliation_Info>();

	        if (affiliations.Contains(New_Affiliation))
	        {
		        return affiliations.Find(New_Affiliation.Equals);
	        }

		    affiliations.Add(New_Affiliation);
		    return New_Affiliation;
        }

	    /// <summary> Adds a new temporal subject to this object </summary>
	    /// <param name="New_Temporal"> Temporral subject object to add to this item </param>
	    /// <returns> Temporal_Info object, either the one passed in or one that equals it already in the list </returns>
	    public Temporal_Info Add_Temporal_Subject(Temporal_Info New_Temporal)
	    {
		    if (temporalSubjects == null)
			    temporalSubjects = new List<Temporal_Info>();

		    if (!temporalSubjects.Contains(New_Temporal))
		    {
			    temporalSubjects.Add(New_Temporal);
			    return New_Temporal;
		    }

		    Temporal_Info returnTemporal = temporalSubjects.Find(New_Temporal.Equals);
		    if ((New_Temporal.TimePeriod.Length > 0) && (returnTemporal.TimePeriod.Length == 0))
			    returnTemporal.TimePeriod = New_Temporal.TimePeriod;
		    return returnTemporal;
	    }

	    /// <summary> Adds a new temporal subject to this object </summary>
	    /// <param name="Start_Year">Start year for the year range</param>
	    /// <param name="End_Year">End year for the year range</param>
	    /// <param name="TimePeriod">Description of the time period (i.e. 'Post-WWII')</param>
	    /// <returns> Temporal_Info object, either the one passed in or one that equals it already in the list </returns>
	    public Temporal_Info Add_Temporal_Subject(int Start_Year, int End_Year, string TimePeriod)
	    {
		    if (temporalSubjects == null)
			    temporalSubjects = new List<Temporal_Info>();

		    Temporal_Info newTemporal = new Temporal_Info(Start_Year, End_Year, TimePeriod);
		    if (!temporalSubjects.Contains(newTemporal))
		    {
			    temporalSubjects.Add(newTemporal);
			    return newTemporal;
		    }

		    Temporal_Info returnTemporal = temporalSubjects.Find(newTemporal.Equals);
		    if ((TimePeriod.Length > 0) && (returnTemporal.TimePeriod.Length == 0))
			    returnTemporal.TimePeriod = TimePeriod;
		    return returnTemporal;
	    }

	    /// <summary> Adds a publisher to this resource </summary>
	    /// <param name="Name">Name of the publisher </param>
	    /// <returns>Object which represents the added publisher</returns>
	    public Publisher_Info Add_Publisher(string Name)
	    {
		    if (publishers == null)
			    publishers = new List<Publisher_Info>();

		    Publisher_Info newPublisher = new Publisher_Info(Name);
		    if (!publishers.Contains(newPublisher))
		    {
			    publishers.Add(newPublisher);
			    return newPublisher;
		    }

		    return publishers.Find(newPublisher.Equals);
	    }

	    /// <summary> Adds a publisher to this resource </summary>
        /// <param name="New_Publisher">New publisher object </param>
        public Publisher_Info Add_Publisher(Publisher_Info New_Publisher)
        {
            if (publishers == null)
                publishers = new List<Publisher_Info>();

            if (!publishers.Contains(New_Publisher))
            {
                publishers.Add(New_Publisher);
                return New_Publisher;
            }

            return publishers.Find(New_Publisher.Equals);
        }

        /// <summary> Removes a publisher linked to this resource  </summary>
        /// <param name="Publisher"> Publisher to remove (if present) </param>
        public void Remove_Publisher(Publisher_Info Publisher)
        {
            if (publishers != null)
                publishers.Remove(Publisher);
        }

        /// <summary> Adds a manufacturer to this resource </summary>
        /// <param name="Name">Name of the manufacturer </param>
        /// <returns>Object which represents the added manufacturer</returns>
        public Publisher_Info Add_Manufacturer(string Name)
        {
            if (manufacturers == null)
                manufacturers = new List<Publisher_Info>();

            Publisher_Info newPublisher = new Publisher_Info(Name);
            if (!manufacturers.Contains(newPublisher))
            {
                manufacturers.Add(newPublisher);
                return newPublisher;
            }

			return manufacturers.Find(newPublisher.Equals);
        }

        /// <summary> Adds a manufacturer to this resource </summary>
        /// <param name="New_Manufacturer">New manufacturer object </param>
        public Publisher_Info Add_Manufacturer(Publisher_Info New_Manufacturer)
        {
            if (manufacturers == null)
                manufacturers = new List<Publisher_Info>();

            if (!manufacturers.Contains(New_Manufacturer))
            {
                manufacturers.Add(New_Manufacturer);
                return New_Manufacturer;
            }

			return manufacturers.Find(New_Manufacturer.Equals);
        }

        #endregion

        #region Method to clear the old data from this object

        /// <summary> Clears all the bibliographic data associated with this item </summary>
        public void Clear()
        {
            sortDate = -1;
            sortTitle = String.Empty;
            if (affiliations != null)
                affiliations.Clear();
            if (temporalSubjects != null)
                temporalSubjects.Clear();

            if (donor != null)
                donor.Clear();
            if (notes != null)
                notes.Clear();
            if (main_entity_name != null)
                main_entity_name.Clear();
            if (names != null)
                names.Clear();
            if (ModsOriginInfo != null)
                ModsOriginInfo.Clear();
            if (abstracts != null)
                abstracts.Clear();
            if (languages != null)
                languages.Clear();
            if (originalPhysicalDesc != null)
                originalPhysicalDesc.Clear();
            if (mainTitle != null)
                mainTitle.Clear();
            if (seriesTitle != null)
                seriesTitle.Clear();
            if (otherTitles != null)
                otherTitles.Clear();
            if (identifiers != null)
                identifiers.Clear();
            if (targetAudiences != null)
                targetAudiences.Clear();
            if (subjects != null)
                subjects.Clear();
            if (genres != null)
                genres.Clear();
            if (locationInfo != null)
                locationInfo.Clear();
            if (seriesPartInfo != null)
                seriesPartInfo.Clear();
            if (type != null)
                type.Clear();
            if (publishers != null)
                publishers.Clear();
            if (manufacturers != null)
                manufacturers.Clear();
            if (recordInfo != null)
                recordInfo.Clear();
            if (relatedItems != null)
                relatedItems.Clear();

            encodingLevel = null;
            if (containers != null)
                containers.Clear();
        }

        #endregion

        #region Internal properties and methods

        /// <summary> Gets and sets the BibID for this material. </summary>
        internal string BibID
        {
            get { return bibID ?? String.Empty; }
            set { bibID = value; }
        }

        /// <summary> Gets and sets the VID for this resource </summary>
        internal string VID
        {
            get { return vid ?? String.Empty; }
            set { vid = value.Replace("VID", ""); }
        }

        #endregion

        #region Methods to write this data in the MODS file

	    /// <summary> Appends this bibliographic description information as MODS to the StringBuilder object </summary>
	    /// <param name="Results">StringBuilder to add this XML to </param>
	    /// <param name="VraCoreInfo"> MODS can also contain the VRA core as an internal extension schema </param>
	    internal override void Add_MODS(TextWriter Results, VRACore_Info VraCoreInfo)
        {
            // Set some values
            XML_Node_Base_Type.Reset_User_ID_Index();
            if (Source != null)
            {
                recordInfo.Record_Content_Source = Source.Statement;
            }
            if (BibID.Length > 0)
            {
                if ((recordInfo.Main_Record_Identifier.Identifier.Length != 0) && (recordInfo.Main_Record_Identifier.Type.ToLower() != "ufdc") && (recordInfo.Main_Record_Identifier.Type.ToLower() != "dloc") && (recordInfo.Main_Record_Identifier.Type.ToLower() != "sobekcm"))
                {
                    Add_Identifier(recordInfo.Main_Record_Identifier.Identifier, recordInfo.Main_Record_Identifier.Type);
                }
                recordInfo.Main_Record_Identifier.Type = "sobekcm";
                recordInfo.Main_Record_Identifier.Identifier = BibID + "_" + VID;
                if (VID == "*****")
                    recordInfo.Main_Record_Identifier.Identifier = BibID;
            }
            if (publishers != null)
            {
                foreach (Publisher_Info thisPublisher in publishers)
                {
                    ModsOriginInfo.Add_Publisher(thisPublisher.Name);
                }
            }
            base.Add_MODS(Results, VraCoreInfo);
        }

        /// <summary> Adds the custom SobekCM bibliographic information in the SobekCM-custom schema XML format</summary>
        /// <param name="Results"> Stream to write this source information as SobekCM-formatted XML</param>
        internal void Add_SobekCM_BibDesc(TextWriter Results)
        {
            string sobekcm_namespace = "sobekcm";
            Results.Write("<" + sobekcm_namespace + ":bibDesc>\r\n");

            // Add all the custom SobekCM specific data
            Results.Write(toMODS(sobekcm_namespace + ":BibID", BibID));
            Results.Write(toMODS(sobekcm_namespace + ":VID", VID));

            // Add affiliation MODS
            if (affiliations != null)
            {
                foreach (Affiliation_Info thisAffiliation in affiliations)
                {
                    thisAffiliation.Add_SobekCM_Metadata(sobekcm_namespace, Results);
                }
            }

            // Add the Encoding Level if there is one
            if (!String.IsNullOrEmpty(encodingLevel))
            {
                Results.Write(toMODS(sobekcm_namespace + ":EncodingLevel", encodingLevel));
            }

            // Add finding guid section, if there is some
            if ((containers != null) && (containers.Count > 0))
            {
                Results.WriteLine("<" + sobekcm_namespace + ":FindingGuidePosition>");
                foreach (Finding_Guide_Container thisContainer in containers)
                {
                    thisContainer.toMETS(Results, sobekcm_namespace);
                }
                Results.WriteLine("</" + sobekcm_namespace + ":FindingGuidePosition>");
            }

            // Add the manufacturers
            if (manufacturers != null)
            {
                foreach (Publisher_Info thisName in manufacturers)
                {
                    thisName.Add_SobekCM_Metadata(sobekcm_namespace, "Manufacturer", Results);
                }
            }

            // Add the publishers 
            if (publishers != null)
            {
                foreach (Publisher_Info thisName in publishers)
                {
                    thisName.Add_SobekCM_Metadata(sobekcm_namespace, "Publisher", Results);
                }
            }


            // Add the source information
            if (Source != null)
            {
                Source.Add_SobekCM_Metadata(sobekcm_namespace, Results);
            }

            // Add temporal subjects
            if ((temporalSubjects != null) && (temporalSubjects.Count > 0))
            {
                // Start this complex data type
                Results.Write("<" + sobekcm_namespace + ":Temporal>\r\n");

                // Step through each element in this type
                foreach (Temporal_Info thisTemporal in temporalSubjects)
                {
                    thisTemporal.Add_SobekCM_Metadata(sobekcm_namespace, Results);
                }

                // Close this complete data type out
                Results.Write("</" + sobekcm_namespace + ":Temporal>\r\n");
            }

            //// Add type
            //if (type != null)
            //{
            //    results.Write(toMODS( sobekcm_namespace + ":Type", type.MODS_Type_String));
            //}

            // Add sorting information
            if (sortDate > 0)
            {
                Results.Write(toMODS(sobekcm_namespace + ":SortDate", sortDate.ToString()));
            }
            Results.Write(toMODS(sobekcm_namespace + ":SortTitle", sortTitle));

            // End the custom SobekCM section
            Results.Write("</" + sobekcm_namespace + ":bibDesc>\r\n");
        }

        #endregion

        #region Methods to create the sort safe title and date

	    /// <summary> Calculate the sort title for this resource</summary>
	    /// <param name="TitleString">Actual title of this resource</param>
	    /// <param name="UsePredeterminedSortTitle"> Flag indicates if there is already a sort title which should be used </param>
	    /// <returns>Sortable title value for this resource</returns>
	    public string SortSafeTitle(string TitleString, bool UsePredeterminedSortTitle)
        {
            if ((UsePredeterminedSortTitle) && (SortTitle.Length > 0))
                return sortTitle;

            // Remove all punctuation first
            TitleString = TitleString.Replace("\"", "").Replace(".", "").Replace("'", "").Replace(",", "");

            // Build the collection of  articles
            string[] articles = new string[] {"EL", "LA", "LAS", "LE", "LES", "A", "THE", "UNAS", "UNOS"};

            // Step through each article
            string capitalTitle = TitleString.ToUpper();
            foreach (string thisArticle in articles)
            {
                if (capitalTitle.IndexOf(thisArticle + " ") == 0)
                {
                    TitleString = TitleString.Substring(thisArticle.Length + 1).Trim();
                    capitalTitle = TitleString.ToUpper();
                }
            }

            // Return this value
            return TitleString.ToUpper();
        }

	    /// <summary> Calculate the sort date for this resource</summary>
	    /// <param name="DateString">Actual date of this resource</param>
	    /// <returns>Sortable date value for this resource</returns>
	    /// <remarks>This computes the number of days since year January 1, year 1</remarks>
	    public int SortSafeDate(string DateString)
	    {
		    // If there is no date, do nothing
		    if (DateString.Trim().Length == 0)
		    {
			    return -1;
		    }

		    // If there is already a sort date, use that
		    if (sortDate > 0)
			    return sortDate;

		    // First, check to see if this is a proper date already

		    // Try conversion
		    DateTime thisDate;
		    if (DateTime.TryParse(DateString, out thisDate))
		    {
			    // Conversion successful, so count days
			    TimeSpan timeElapsed = thisDate.Subtract(new DateTime(1, 1, 1));
			    sortDate = (int) timeElapsed.TotalDays;
			    return sortDate;
		    }
		    else
		    {
			    // Didn't work, so need to try and find a year at least
			    DateString = DateString.Replace("[", "").Replace("]", "").Replace("(", "").Replace(")", "");
			    DateString = DateString.ToUpper();
			    DateString = DateString.Replace("circa", "").Replace("ca", "").Replace("c", "");
			    DateString = DateString.Replace(".", "").Replace(",", "").Replace("-", "").Trim();

			    // Step through looking for first four digits
			    int start = -1;
			    for (int i = 0; i < DateString.Length; i++)
			    {
				    if ((Char.IsNumber(DateString[i])) || (DateString[i] == '-') ||
				        (DateString[i] == 'X') || (DateString[i] == '?') || (DateString[i] == 'U'))
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
					    start = -1;
				    }
			    }

			    // If a start was found, use it
			    if ((start >= 0) && ((DateString.Length - start) >= 4))
			    {
				    string year = DateString.Substring(start, 4).Replace("X", "0").Replace("?", "0").Replace("U", "0").Replace("-", "0");
				    DateTime thisYear = new DateTime(Convert.ToInt16(year), 1, 1);
				    TimeSpan timeElapsed = thisYear.Subtract(new DateTime(1, 1, 1));
				    sortDate = (int) timeElapsed.TotalDays;
				    return sortDate;
			    }
		    }

		    // Return this value, as empty
		    return -1;
	    }

	    #endregion


	    /// <summary> Gets the controlled sobekcm type as an enumeration, based on the 
	    /// type and genres listed within the metadata </summary>
	    public TypeOfResource_SobekCM_Enum SobekCM_Type
        {
            get
            {
                // Look through the genres and get any sobekcm genres
                string sobekcm_genre = String.Empty;
                if (genres != null)
                {
                    foreach (Genre_Info thisGenre in genres)
                    {
                        if (String.Compare(thisGenre.Authority, "sobekcm", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            sobekcm_genre = thisGenre.Genre_Term;

                            // Special code here looking for ead
                            if (String.Compare(sobekcm_genre, "ead", StringComparison.OrdinalIgnoreCase) == 0)
                                return TypeOfResource_SobekCM_Enum.EAD;

                            // Special code here looking for project
                            if (String.Compare(sobekcm_genre, "project", StringComparison.OrdinalIgnoreCase) == 0)
                                return TypeOfResource_SobekCM_Enum.Project;

                            // Special code here looking for multivolume
                            if (String.Compare(sobekcm_genre, "multivolume", StringComparison.OrdinalIgnoreCase) == 0)
                                return TypeOfResource_SobekCM_Enum.Multivolume;

							// Special code here looking for dataset
							if (String.Compare(sobekcm_genre, "dataset", StringComparison.OrdinalIgnoreCase) == 0)
								return TypeOfResource_SobekCM_Enum.Dataset;
                        }
                        else if (String.Compare(thisGenre.Authority, "marcgt", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            if (String.Compare(thisGenre.Genre_Term, "newspaper", StringComparison.OrdinalIgnoreCase) == 0)
                                sobekcm_genre = "newspaper";
                            if (String.Compare(thisGenre.Genre_Term, "serial", StringComparison.OrdinalIgnoreCase) == 0)
                                sobekcm_genre = "serial";
                        }
                    }
                }

                // There should always be a mods type, so use that
                switch (type.MODS_Type)
                {
                    case TypeOfResource_MODS_Enum.Cartographic:
                        return TypeOfResource_SobekCM_Enum.Map;

                    case TypeOfResource_MODS_Enum.Mixed_Material:
                        return String.Compare(sobekcm_genre, "learning object", StringComparison.OrdinalIgnoreCase) == 0 ? TypeOfResource_SobekCM_Enum.Learning_Object : TypeOfResource_SobekCM_Enum.Mixed_Material;

                    case TypeOfResource_MODS_Enum.Moving_Image:
                        return TypeOfResource_SobekCM_Enum.Video;

                    case TypeOfResource_MODS_Enum.Notated_Music:
                        return TypeOfResource_SobekCM_Enum.Notated_Music;

                    case TypeOfResource_MODS_Enum.Sofware_Multimedia:
						return String.Compare(sobekcm_genre, "dataset", StringComparison.OrdinalIgnoreCase) == 0 ? TypeOfResource_SobekCM_Enum.Dataset : TypeOfResource_SobekCM_Enum.Software_Multimedia;

                    case TypeOfResource_MODS_Enum.Sound_Recording:
                    case TypeOfResource_MODS_Enum.Sound_Recording_Musical:
                    case TypeOfResource_MODS_Enum.Sound_Recording_Nonmusical:
                        return TypeOfResource_SobekCM_Enum.Audio;

                    case TypeOfResource_MODS_Enum.Still_Image:
                        return String.Compare(sobekcm_genre, "aerial photography", StringComparison.OrdinalIgnoreCase) == 0 ? TypeOfResource_SobekCM_Enum.Aerial : TypeOfResource_SobekCM_Enum.Photograph;

                    case TypeOfResource_MODS_Enum.Text:
                        switch (sobekcm_genre.ToUpper())
                        {
                            case "NEWSPAPER":
                                return TypeOfResource_SobekCM_Enum.Newspaper;

                            case "SERIAL":
                                return TypeOfResource_SobekCM_Enum.Serial;

                            default:
                                return TypeOfResource_SobekCM_Enum.Book;
                        }

                    case TypeOfResource_MODS_Enum.Three_Dimensional_Object:
                        return TypeOfResource_SobekCM_Enum.Artifact;

                    default:
                        return TypeOfResource_SobekCM_Enum.UNKNOWN;
                }
            }
            set
            {
                // Clear any sobekcm genres
                if (genres != null)
                {
                    List<Genre_Info> sobekcmGenres = genres.Where(ThisGenre => String.Compare(ThisGenre.Authority, "sobekcm", StringComparison.OrdinalIgnoreCase) == 0).ToList();
	                foreach (Genre_Info thisGenre in sobekcmGenres)
                    {
                        Remove_Genre(thisGenre);
                    }
                }

                // Now, set the appropriate values here, in the sobekcm genre, and in the MODS type
                switch (value)
                {
                    case TypeOfResource_SobekCM_Enum.Aerial:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Still_Image;
                        Add_Genre("aerial photography", "sobekcm");
                        break;

                    case TypeOfResource_SobekCM_Enum.Archival:
                    case TypeOfResource_SobekCM_Enum.Mixed_Material:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Mixed_Material;
                        break;

                    case TypeOfResource_SobekCM_Enum.Artifact:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Three_Dimensional_Object;
                        break;

                    case TypeOfResource_SobekCM_Enum.Audio:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Sound_Recording;
                        break;

                    case TypeOfResource_SobekCM_Enum.Book:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Text;
                        break;

					case TypeOfResource_SobekCM_Enum.Dataset:
						type.MODS_Type = TypeOfResource_MODS_Enum.Sofware_Multimedia;
						Add_Genre("dataset", "sobekcm");
						break;

                    case TypeOfResource_SobekCM_Enum.EAD:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Mixed_Material;
                        Add_Genre("ead", "sobekcm");
                        break;

                    case TypeOfResource_SobekCM_Enum.Learning_Object:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Mixed_Material;
                        Add_Genre("learning object", "sobekcm");
                        break;

                    case TypeOfResource_SobekCM_Enum.Map:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Cartographic;
                        break;

                    case TypeOfResource_SobekCM_Enum.Multivolume:
                        type.MODS_Type = TypeOfResource_MODS_Enum.UNKNOWN;
                        Add_Genre("multivolume", "sobekcm");
                        break;

                    case TypeOfResource_SobekCM_Enum.Newspaper:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Text;
                        Add_Genre("newspaper", "sobekcm");
                        break;

                    case TypeOfResource_SobekCM_Enum.Notated_Music:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Notated_Music;
                        break;

                    case TypeOfResource_SobekCM_Enum.Photograph:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Still_Image;
                        break;

                    case TypeOfResource_SobekCM_Enum.Project:
                        type.MODS_Type = TypeOfResource_MODS_Enum.UNKNOWN;
                        Add_Genre("project", "sobekcm");
                        break;

                    case TypeOfResource_SobekCM_Enum.Serial:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Text;
                        Add_Genre("serial", "sobekcm");
                        break;

                    case TypeOfResource_SobekCM_Enum.Software_Multimedia:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Sofware_Multimedia;
                        break;

                    case TypeOfResource_SobekCM_Enum.UNKNOWN:
                        type.MODS_Type = TypeOfResource_MODS_Enum.UNKNOWN;
                        break;

                    case TypeOfResource_SobekCM_Enum.Video:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Moving_Image;
                        break;
                }
            }
        }

		/// <summary> Gets the controlled sobekcm type as a string, based on the 
		/// type and genres listed within the metadata </summary>
        public string SobekCM_Type_String
        {
            get
            {
                TypeOfResource_SobekCM_Enum sobekcmType = SobekCM_Type;
                switch (sobekcmType)
                {
                    case TypeOfResource_SobekCM_Enum.Aerial:
                        return "Aerial Photography";

                    case TypeOfResource_SobekCM_Enum.Archival:
                        return "Archival";

                    case TypeOfResource_SobekCM_Enum.Artifact:
                        return "Artifact";

                    case TypeOfResource_SobekCM_Enum.Audio:
                        return "Audio";

                    case TypeOfResource_SobekCM_Enum.Book:
                        return "Book";

					case TypeOfResource_SobekCM_Enum.Dataset:
						return "Dataset";

                    case TypeOfResource_SobekCM_Enum.EAD:
                        return "Finding Guide (EAD)";

                    case TypeOfResource_SobekCM_Enum.Learning_Object:
                        return "Learning Object";

                    case TypeOfResource_SobekCM_Enum.Map:
                        return "Map";

                    case TypeOfResource_SobekCM_Enum.Mixed_Material:
                        return "Mixed Material";

                    case TypeOfResource_SobekCM_Enum.Multivolume:
                        return "Multivolume";

                    case TypeOfResource_SobekCM_Enum.Newspaper:
                        return "Newspaper";

                    case TypeOfResource_SobekCM_Enum.Notated_Music:
                        return "Notated Music";

                    case TypeOfResource_SobekCM_Enum.Photograph:
                        return "Photograph";

                    case TypeOfResource_SobekCM_Enum.Project:
                        return "Project";

                    case TypeOfResource_SobekCM_Enum.Serial:
                        return "Serial";

                    case TypeOfResource_SobekCM_Enum.Software_Multimedia:
                        return "Software";

                    case TypeOfResource_SobekCM_Enum.UNKNOWN:
                        return "Unknown";

                    case TypeOfResource_SobekCM_Enum.Video:
                        return "Video";
                }
                return "Unknown";
            }
            set
            {
                switch (value.ToUpper().Trim().Replace("_"," "))
                {
                    case "AERIAL":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Aerial;
                        return;

                    case "ARCHIVAL":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Archival;
                        return;

                    case "ARTIFACT":
                    case "THREE DIMENSIONAL OBJECT":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Artifact;
                        return;

                    case "AUDIO":
                    case "SOUND RECORDING":
                    case "SOUND RECORDING MUSICAL":
                    case "SOUND RECORDING NONMUSICAL":
                    case "MUSIC":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Audio;
                        return;

                    case "BOOK":
                    case "TEXT":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Book;
                        return;

					case "DATASET":
						SobekCM_Type = TypeOfResource_SobekCM_Enum.Dataset;
						return;

                    case "EAD":
                    case "FINDING GUIDE":
					case "FINDING GUIDE (EAD)":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.EAD;
                        return;

                    case "LEARNING OBJECT":
                    case "LEARNING RESOURCE":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Learning_Object;
                        return;

                    case "MAP":
                    case "CARTOGRAPHIC":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Map;
                        return;

                    case "MIXED MATERIAL":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Mixed_Material;
                        break;

                    case "MULTIVOLUME":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Multivolume;
                        return;

                    case "NEWSPAPER":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Newspaper;
                        return;

                    case "NOTATED MUSIC":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Notated_Music;
                        return;

                    case "PHOTOGRAPH":
                    case "STILL IMAGE":
					case "IMAGE":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Photograph;
                        return;

                    case "PROJECT":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Project;
                        return;

                    case "SERIAL":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Serial;
                        return;

                    case "SOFTWARE":
                    case "SOFTWARE MULTIMEDIA":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Software_Multimedia;
                        break;

                    case "VIDEO":
                    case "MOVING IMAGE":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Video;
                        return;

                    default:
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.UNKNOWN;
                        return;
                }
            }
        }
    }
}