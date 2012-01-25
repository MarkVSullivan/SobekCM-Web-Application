using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace SobekCM.Bib_Package.Bib_Info
{
    /// <summary> Stores the descriptive metadata for the bibliographic resource </summary>
    /// <remarks> This class extends the <see cref="MODS_Info"/> class and includes additional elements which 
    /// can be expressed in the SobekCM custom schema within a METS file.<br /> <br /> 
    /// Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
    public class Bibliographic_Info : MODS_Info
    {
        #region Private Class Members

        private int sortDate;
        private Source_Info source;

        private string bibID;
        private string vid;
        private string sortTitle;
        private List<Affiliation_Info> affiliations;
        private List<Temporal_Info> temporalSubjects;
        private Coordinates coordinates;
        private List<Publisher_Info> publishers;
        private List<Publisher_Info> manufacturers;

        private string encodingLevel;
        private List<Finding_Guide_Container> containers;

        #endregion

        #region Constructors

        /// <summary> Constructor for a new instance of the Bibliographic_Info class. </summary>
        public Bibliographic_Info()
            : base()
        {
            sortDate = -1;
            source = new Source_Info();
        }

        #endregion

        #region Finding Guide Container properties and methods

        /// <summary> Get the number of finding guide containers associated with this resource </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Containers"/> property.  Even if 
        /// there are no finding guide containers, the Containers property creates a readonly collection to pass back out.</remarks>
        public int Containers_Count
        {
            get
            {
                if (containers == null)
                    return 0;
                else
                    return containers.Count;
            }
        }

        /// <summary> Gets the list of finding guide containers </summary>
        /// <remarks> You should check the count of containers first using the <see cref="Containers_Count"/> property before using this property.
        /// Even if there are no containers, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Finding_Guide_Container> Containers
        {
            get
            {
                if (containers == null)
                    return new ReadOnlyCollection<Finding_Guide_Container>(new List<Finding_Guide_Container>());
                else
                    return new ReadOnlyCollection<Finding_Guide_Container>(containers);
            }
        }

        /// <summary> Adds a new finding-guide container to the container list for this item </summary>
        /// <param name="Type">Type of container this represents (i.e., Box, Folder, etc..)</param>
        /// <param name="Name">Name of this container</param>
        /// <param name="Level">Level within the container list that this container resides</param>
        /// <remarks>This can be used later to construct EADs for single item loads</remarks>
        public void Add_Container(string Type, string Name, int Level)
        {
            if (containers == null)
                containers = new List<Finding_Guide_Container>();

            containers.Add(new Finding_Guide_Container(Type, Name, Level));
        }

        /// <summary> Clears all finding-guide containers associated with tis item </summary>
        public void Clear_Containers()
        {
            if (containers != null)
                containers.Clear();
        }

        #endregion

        #region Public Properties

        /// <summary> Gets or sets the encoding level for this record </summary>
        public string EncodingLevel
        {
            get { return encodingLevel ?? String.Empty; }
            set { encodingLevel = value; }
        }       

        /// <summary> Flag indicates if there is any coordinate information included with this object </summary>
        public bool hasCoordinateInformation
        {
            get
            {
                if (coordinates == null)
                    return false;

                return coordinates.hasData;
            }
        }

        /// <summary> Gets the collection of coordinate information for this resource  </summary>
        public Coordinates Coordinates
        {
            get
            {
                if (coordinates == null)
                    coordinates = new Coordinates();

                return coordinates;
            }
            set
            {
                coordinates = value;
            }
        }

        /// <summary> Gets the records numbers for any and all NOTIS records associated with this item  </summary>
        public string[] NOTIS_Records
        {
            get
            {
                if (identifiers == null)
                    return new string[0];

                ArrayList NOTIS = new ArrayList();
                foreach (Identifier_Info thisIdentifier in identifiers)
                {
                    if (thisIdentifier.Type.Replace("*", "").ToUpper() == "NOTIS")
                    {
                        NOTIS.Add(thisIdentifier.Identifier);
                    }
                }

                string[] returnVal = new string[NOTIS.Count];
                for (int i = 0; i < NOTIS.Count; i++)
                {
                    returnVal[i] = NOTIS[i].ToString();
                }
                return returnVal;
            }
        }

        /// <summary> Gets the ALEPH record number associated with this item </summary>
        public string ALEPH_Record
        {
            get
            {
                if (identifiers == null)
                    return String.Empty;

                foreach (Identifier_Info thisIdentifier in identifiers)
                {
                    if (thisIdentifier.Type.Replace("*", "").ToUpper().IndexOf("ALEPH") >= 0)
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
                        if (thisIdentifier.Type.Replace("*", "").ToUpper().IndexOf("ALEPH") >= 0)
                        {
                            thisIdentifier.Identifier = value;
                            return;
                        }
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
                    if (thisIdentifier.Type.Replace("*","").ToUpper() == "OCLC" )
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
                if (( sobekcm_type == TypeOfResource_SobekCM_Enum.Artifact ) || ( sobekcm_type == TypeOfResource_SobekCM_Enum.Aerial ) || ( sobekcm_type == TypeOfResource_SobekCM_Enum.Map ) || ( sobekcm_type == TypeOfResource_SobekCM_Enum.Photograph ))
                    return true;
                else
                    return false;
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

        /// <summary> Gets the number of affiliations associated with this resource </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Affiliations"/> property.  Even if 
        /// there are no affiliations, the Affiliations property creates a readonly collection to pass back out.</remarks>
        public int Affiliations_Count
        {
            get
            {
                if (affiliations == null)
                    return 0;
                else
                    return affiliations.Count;
            }
        }

        /// <summary> Gets the collection of affiliations associated with this resource </summary>
        /// <remarks> You should check the count of affiliations first using the <see cref="Affiliations_Count"/> property before using this property.
        /// Even if there are no affiliations, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Affiliation_Info> Affiliations
        {
            get
            {
                if (affiliations == null)
                    return new ReadOnlyCollection<Affiliation_Info>(new List<Affiliation_Info>());
                else
                    return new ReadOnlyCollection<Affiliation_Info>(affiliations);
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

        /// <summary> Gets the number of temporal subjects associated with this resource </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="TemporalSubjects"/> property.  Even if 
        /// there are no temporal subjects, the TemporalSubjects property creates a readonly collection to pass back out.</remarks>
        public int TemporalSubjects_Count
        {
            get
            {
                if (temporalSubjects == null)
                    return 0;
                else
                    return temporalSubjects.Count;
            }
        }

        /// <summary> Gets the collection of temporal subjects associated with this resource </summary>
        /// <remarks> You should check the count of temporal subjects first using the <see cref="TemporalSubjects_Count"/> property before using this property.
        /// Even if there are no temporal subjects, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Temporal_Info> TemporalSubjects
        {
            get
            {
                if (temporalSubjects == null)
                    return new ReadOnlyCollection<Temporal_Info>(new List<Temporal_Info>());
                else
                    return new ReadOnlyCollection<Temporal_Info>(temporalSubjects);
            }
        }

        /// <summary> Gets the source insitution information associated with this resource </summary>
        public Source_Info Source
        {
            get { return source; }
        }



        /// <summary> Clear the list of publishers associated with this item </summary>
        public void Clear_Publishers()
        {
            if (publishers != null)
            {
                publishers.Clear();
            }
            originInfo.Clear_Places_And_Publishers();
        }

        /// <summary> Gets the number of publishers associated with this resource </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Publishers"/> property.  Even if 
        /// there are no publishers, the Publishers property creates a readonly collection to pass back out.</remarks>
        public int Publishers_Count
        {
            get
            {
                if (publishers == null)
                    return 0;
                else
                    return publishers.Count;
            }
        }

        /// <summary> Gets the list of publishers associated with this resource </summary>
        /// <remarks> You should check the count of publishers first using the <see cref="Publishers_Count"/> property before using this property.
        /// Even if there are no publishers, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Publisher_Info> Publishers
        {
            get
            {
                if (publishers == null)
                    return new ReadOnlyCollection<Publisher_Info>(new List<Publisher_Info>());
                else
                    return new ReadOnlyCollection<Publisher_Info>(publishers);
            }
        }

        /// <summary> Clear the list of manufacturers associated with this item </summary>
        public void Clear_Manufacturers()
        {
            if (manufacturers != null)
            {
                manufacturers.Clear();
            }
        }

        /// <summary> Gets the number of manufacturers associated with this resource </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Manufacturers"/> property.  Even if 
        /// there are no manufacturers, the Manufacturers property creates a readonly collection to pass back out.</remarks>
        public int Manufacturers_Count
        {
            get
            {
                if (manufacturers == null)
                    return 0;
                else
                    return manufacturers.Count;
            }
        }

        /// <summary> Gets the list of manufacturers associated with this resource </summary>
        /// <remarks> You should check the count of manufacturers first using the <see cref="Manufacturers_Count"/> property before using this property.
        /// Even if there are no manufacturers, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Publisher_Info> Manufacturers
        {
            get
            {
                if (manufacturers == null)
                    return new ReadOnlyCollection<Publisher_Info>(new List<Publisher_Info>());
                else
                    return new ReadOnlyCollection<Publisher_Info>(manufacturers);
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

        /// <summary> Adds a new affiliation object directly to this item </summary>
        /// <param name="New_Affiliation"> Affiliation object to add to this item </param>
        /// <returns> Affiliation object, either the one passed in or one that equals it already in the list </returns>
        public Affiliation_Info Add_Affiliation(Affiliation_Info New_Affiliation)
        {
            if (affiliations == null)
                affiliations = new List<Affiliation_Info>();

            if (!affiliations.Contains(New_Affiliation))
            {
                affiliations.Add(New_Affiliation);
                return New_Affiliation;
            }
            else
            {
                return affiliations.Find(New_Affiliation.Equals);
            }
        }

        /// <summary> Adds a new temporal subject to this object </summary>
        /// <param name="New_Temporal"> Temporral subject object to add to this item </param>
        /// <returns> Temporal_Info object, either the one passed in or one that equals it already in the list </returns>
        public Temporal_Info Add_Temporal_Subject( Temporal_Info New_Temporal  )
        {
            if (temporalSubjects == null)
                temporalSubjects = new List<Temporal_Info>();

            if (!temporalSubjects.Contains(New_Temporal))
            {
                temporalSubjects.Add(New_Temporal);
                return New_Temporal;
            }
            else
            {
                Temporal_Info returnTemporal = temporalSubjects.Find(New_Temporal.Equals);
                if ((New_Temporal.TimePeriod.Length > 0) && ( returnTemporal.TimePeriod.Length == 0 ))
                    returnTemporal.TimePeriod = New_Temporal.TimePeriod;
                return returnTemporal;
            }
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
            else
            {
                Temporal_Info returnTemporal = temporalSubjects.Find(newTemporal.Equals);
                if ((TimePeriod.Length > 0) && (returnTemporal.TimePeriod.Length == 0))
                    returnTemporal.TimePeriod = TimePeriod;
                return returnTemporal;
            }
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
            else
            {
                return publishers.Find(newPublisher.Equals);
            }
        }

        /// <summary> Adds a publisher to this resource </summary>
        /// <param name="New_Publisher">New publisher object </param>
        public Publisher_Info Add_Publisher(Publisher_Info New_Publisher )
        {
            if (publishers == null)
                publishers = new List<Publisher_Info>();

            if (!publishers.Contains(New_Publisher))
            {
                publishers.Add(New_Publisher);
                return New_Publisher;
            }
            else
            {
                return publishers.Find(New_Publisher.Equals);
            }
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
            else
            {
                return manufacturers.Find(newPublisher.Equals);
            }
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
            else
            {
                return manufacturers.Find(New_Manufacturer.Equals);
            }
        }

        /// <summary> Get the citation string, which is all the data fields lumped together
        /// for easy searching. </summary>
        public string Citation_String
        {
            get
            {
                StringBuilder full_citation = new StringBuilder();

                // Add the data from the base class
                full_citation.Append(base.MODS_Citation_String);

                if (affiliations != null)
                {
                    foreach (Affiliation_Info thisAffiliation in affiliations)
                    {
                        full_citation.Append(thisAffiliation.Affiliation_XML + " | ");
                    }
                }

                full_citation.Append(BibID + " | ");

                full_citation.Append(VID + " | ");

                if (source != null)
                {
                    full_citation.Append(source.Code + " | ");

                    full_citation.Append(source.Statement + " | ");
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

        #endregion

        #region Method to clear the old data from this object

        /// <summary> Clears all the bibliographic data associated with this item </summary>
        public void Clear()
        {
            sortDate = -1;
            sortTitle = String.Empty;
            if ( affiliations != null )
                affiliations.Clear();
            if ( temporalSubjects != null )
                temporalSubjects.Clear();
            if ( coordinates != null )
                coordinates.Clear_User_Polygons_And_Lines();

            if ( donor != null ) 
                donor.Clear();
            if ( notes != null )
                notes.Clear();
            if ( main_entity_name != null )
                main_entity_name.Clear();
            if ( names != null )
                names.Clear();
            if ( originInfo != null )
                originInfo.Clear();
            if ( abstracts != null )
                abstracts.Clear();
            if ( languages != null )
                languages.Clear();
            if ( originalPhysicalDesc != null )
                originalPhysicalDesc.Clear();
            if ( mainTitle != null )
                mainTitle.Clear();  
            if ( seriesTitle != null )
                seriesTitle.Clear();
            if ( otherTitles != null )
                otherTitles.Clear();
            if ( identifiers != null )
                identifiers.Clear();
            if ( targetAudiences != null )
                targetAudiences.Clear();
            if ( subjects != null )
                subjects.Clear();
            if ( genres != null )
                genres.Clear();
            if ( locationInfo != null )
                locationInfo.Clear();
            if ( seriesPartInfo != null )
                seriesPartInfo.Clear();
            if ( type != null )
                type.Clear();
            if ( publishers != null )
                publishers.Clear();
            if ( manufacturers != null )
                manufacturers.Clear();
            if ( recordInfo != null )
                recordInfo.Clear();
            if (relatedItems != null )      
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
            set { bibID =value; }
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
        /// <param name="results">StringBuilder to add this XML to </param>
        internal override void Add_MODS( System.IO.TextWriter results)
        {
            // Set some values
            XML_Node_Base_Type.Reset_User_ID_Index();
            if (source != null)
            {
                base.recordInfo.Record_Content_Source = source.Statement;
            }
            if (BibID.Length > 0)
            {
                if ((base.recordInfo.Main_Record_Identifier.Identifier.Length != 0) && (base.recordInfo.Main_Record_Identifier.Type.ToLower() != "ufdc") && (base.recordInfo.Main_Record_Identifier.Type.ToLower() != "dloc") && (base.recordInfo.Main_Record_Identifier.Type.ToLower() != "sobekcm"))
                {
                    Add_Identifier(base.recordInfo.Main_Record_Identifier.Identifier, base.recordInfo.Main_Record_Identifier.Type);
                }
                base.recordInfo.Main_Record_Identifier.Type = "sobekcm";
                base.recordInfo.Main_Record_Identifier.Identifier = BibID + "_" + VID;
                if (VID == "*****")
                    base.recordInfo.Main_Record_Identifier.Identifier = BibID;
            }
            if (publishers != null)
            {
                foreach (Publisher_Info thisPublisher in publishers)
                {
                    base.originInfo.Add_Publisher(thisPublisher.Name);
                }
            }
            base.Add_MODS(results);
        }

        /// <summary> Adds the custom SobekCM bibliographic information in the SobekCM-custom schema XML format</summary>
        /// <param name="results"> Stream to write this source information as SobekCM-formatted XML</param>
        internal void Add_SobekCM_BibDesc( System.IO.TextWriter results)
        {
            string sobekcm_namespace = "sobekcm";
            results.Write("<" + sobekcm_namespace + ":bibDesc>\r\n");

            // Add all the custom SobekCM specific data
            results.Write(toMODS( sobekcm_namespace + ":BibID", BibID));
            results.Write(toMODS( sobekcm_namespace + ":VID", VID));

            // Add affiliation MODS
            if (affiliations != null)
            {
                foreach (Affiliation_Info thisAffiliation in affiliations)
                {
                    thisAffiliation.Add_SobekCM_Metadata( sobekcm_namespace, results);
                }
            }

            // Add the coordinates
            if (coordinates != null)
            {
                coordinates.Add_SobekCM_Metadata(sobekcm_namespace, results);
            }

            // Add the Encoding Level if there is one
            if (!String.IsNullOrEmpty(encodingLevel))
            {
                results.Write(toMODS(sobekcm_namespace + ":EncodingLevel", encodingLevel));
            }

            // Add finding guid section, if there is some
            if ((containers != null) && (containers.Count > 0))
            {
                results.WriteLine("<" + sobekcm_namespace + ":FindingGuidePosition>");
                foreach (Finding_Guide_Container thisContainer in containers)
                {
                    thisContainer.toMETS(results, sobekcm_namespace);
                }
                results.WriteLine("</" + sobekcm_namespace + ":FindingGuidePosition>");
            }

            // Add the manufacturers
            if (manufacturers != null)
            {
                foreach (Publisher_Info thisName in manufacturers)
                {
                    thisName.Add_SobekCM_Metadata( sobekcm_namespace, "Manufacturer", results);
                }
            }

            // Add the publishers
            if (publishers != null)
            {
                foreach (Publisher_Info thisName in publishers)
                {
                    thisName.Add_SobekCM_Metadata( sobekcm_namespace, "Publisher", results);
                }
            }

            // Add the source information
            if (source != null)
            {
                source.Add_SobekCM_Metadata( sobekcm_namespace, results);
            }

            // Add temporal subjects
            if (( temporalSubjects != null ) && ( temporalSubjects.Count > 0))
            {
                // Start this complex data type
                results.Write( "<" + sobekcm_namespace + ":Temporal>\r\n");

                // Step through each element in this type
                foreach (Temporal_Info thisTemporal in temporalSubjects)
                {
                    thisTemporal.Add_SobekCM_Metadata( sobekcm_namespace, results);
                }

                // Close this complete data type out
                results.Write( "</" + sobekcm_namespace + ":Temporal>\r\n");
            }

            //// Add type
            //if (type != null)
            //{
            //    results.Write(toMODS( sobekcm_namespace + ":Type", type.MODS_Type_String));
            //}

            // Add sorting information
            if (sortDate > 0)
            {
                results.Write(toMODS(sobekcm_namespace + ":SortDate", sortDate.ToString()));
            }
            results.Write(toMODS( sobekcm_namespace + ":SortTitle", sortTitle));

            // End the custom SobekCM section
            results.Write( "</" + sobekcm_namespace + ":bibDesc>\r\n");
        }

        #endregion

        #region GSA writing internal properties and methods

        /// <summary> Gets the Greenstone Archival format descriptive metadata for this resource </summary>
        /// <param name="Division_Table_Of_Contents"> String for the table of contents description from the divisions in the MODS file </param>
        /// <param name="Include_SobekCM_Metadata"> Flag indicates whether to include the sobekCM special metadata</param>
        /// <remarks> </remarks>
        internal string GSA_Descriptive_Metadata(string Division_Table_Of_Contents, bool Include_SobekCM_Metadata)
        {
            StringBuilder results = new StringBuilder();
            StringBuilder full_citation = new StringBuilder();

            string indent = "    ";

            // Add dc.Audience
            if (targetAudiences != null)
            {
                foreach (TargetAudience_Info thisAudience in targetAudiences)
                {
                    if (thisAudience.Audience.Length > 0)
                    {
                        results.Append(To_GSA(thisAudience.Audience, "dc.Audience", indent));
                        full_citation.Append(thisAudience.Audience + " ");
                    }
                }
            }

            // Add dc.coverage^temporals
            if (temporalSubjects != null)
            {
                foreach (Temporal_Info thisTemporal in temporalSubjects)
                {
                    results.Append(thisTemporal.toGSA());
                    full_citation.Append(thisTemporal.TimePeriod + " ");
                }
            }

            // Add dc.creator fields from the main entity
            if (( main_entity_name != null ) && (main_entity_name.Full_Name.Length > 0))
            {
                if (main_entity_name.Full_Name.Length > 0)
                {
                    results.Append(indent + "<Metadata name=\"dc.Creator\"> " + base.Convert_String_To_XML_Safe(main_entity_name.Full_Name));
                    if (main_entity_name.Dates.Length > 0)
                        results.Append(", " + base.Convert_String_To_XML_Safe(main_entity_name.Dates));
                    if (main_entity_name.Roles.Count > 0)
                    {
                        string role_combined = String.Empty;
                        foreach (Name_Info_Role thisRole in main_entity_name.Roles)
                        {
                            if (thisRole.Role_Type == Name_Info_Role_Type_Enum.text)
                            {
                                if (role_combined.Length > 0)
                                {
                                    role_combined = role_combined + ", " + thisRole.Role;
                                }
                                else
                                {
                                    role_combined = thisRole.Role;
                                }
                            }
                        }
                        if (role_combined.Length > 0)
                        {
                            results.Append(" ( " + role_combined + " ) ");
                        }
                    }
                    results.Append("</Metadata>\r\n");
                }
                full_citation.Append(main_entity_name.Full_Name + " ");
            }

            // Add dc.creator fields from additional names
            if (names != null)
            {
                foreach (Name_Info thisNameInfo in names)
                {
                    if (thisNameInfo.Full_Name.Length > 0)
                    {
                        results.Append(indent + "<Metadata name=\"dc.Creator\"> " + base.Convert_String_To_XML_Safe(thisNameInfo.Full_Name));
                        if (thisNameInfo.Dates.Length > 0)
                            results.Append(", " + base.Convert_String_To_XML_Safe(thisNameInfo.Dates));
                        if (thisNameInfo.Roles.Count > 0)
                        {
                            string role_combined = String.Empty;
                            foreach (Name_Info_Role thisRole in thisNameInfo.Roles)
                            {
                                if (thisRole.Role_Type == Name_Info_Role_Type_Enum.text)
                                {
                                    if (role_combined.Length > 0)
                                    {
                                        role_combined = role_combined + ", " + thisRole.Role;
                                    }
                                    else
                                    {
                                        role_combined = thisRole.Role;
                                    }
                                }
                            }
                            if (role_combined.Length > 0)
                            {
                                results.Append(" ( " + role_combined + " ) ");
                            }
                        }
                        results.Append("</Metadata>\r\n");
                    }
                    full_citation.Append(thisNameInfo.Full_Name + " ");
                }
            }

            if (originInfo != null)
            {
                // Add dc.date
                results.Append(To_GSA(originInfo.Date_Issued, "dc.Date", indent));
                full_citation.Append(originInfo.Date_Issued + " ");

                // Add dc.date^dateCopyrighted
                results.Append(To_GSA(originInfo.Date_Copyrighted, "dc.Date^dateCopyrighted", indent));
                full_citation.Append(originInfo.Date_Copyrighted + " ");
            }

            // Add dc.description
            if ((originalPhysicalDesc != null) && (originalPhysicalDesc.Notes_Count > 0))
            {
                foreach (string description in originalPhysicalDesc.Notes)
                {
                    results.Append(To_GSA(description, "dc.Description", indent));
                    full_citation.Append(description + " ");
                }
            }
            if (notes != null)
            {
                foreach (Note_Info thisNote in notes)
                {
                    results.Append(To_GSA(thisNote.Note, "dc.Description", indent));
                    full_citation.Append(thisNote.Note + " ");
                }
            }

            // Add dc.description^abstract
            if (abstracts != null)
            {
                foreach (Abstract_Info thisAbstract in abstracts)
                {
                    if (thisAbstract.Abstract_Text.Length > 0)
                    {
                        results.Append(To_GSA(thisAbstract.Abstract_Text, "dc.Description^abstract", indent));
                        full_citation.Append(thisAbstract.Abstract_Text + " ");
                    }
                }
            }

            // Add dc.description^tableOfContents
            if ( !String.IsNullOrEmpty( tableOfContents ))
            {
                results.Append(To_GSA(tableOfContents, "dc.Description^tableOfContents", indent));
                full_citation.Append(tableOfContents + " ");
            }
            else if ( Division_Table_Of_Contents.Length > 0 )
            {
                results.Append(To_GSA(Division_Table_Of_Contents, "dc.Description^tableOfContents", indent));
                full_citation.Append(Division_Table_Of_Contents + " ");
            }


            // Add dc.format
            if (originalPhysicalDesc != null)
            {
                results.Append(To_GSA(originalPhysicalDesc.Extent, "dc.Format^extent", indent));
                full_citation.Append(originalPhysicalDesc.Extent + " ");
            }

            // Add dc.Identifier
            if (identifiers != null)
            {
                foreach (Identifier_Info thisIdentifier in identifiers)
                {
                    if (thisIdentifier.Type.Length > 0)
                    {
                        results.Append(To_GSA("(" + thisIdentifier.Type + ") " + thisIdentifier.Identifier, "dc.Identifier", indent));
                    }
                    else
                    {
                        results.Append(To_GSA(thisIdentifier.Identifier, "dc.Identifier", indent));
                    }
                    full_citation.Append(thisIdentifier.Identifier + " ");
                }
            }

            // Add dc.language
            if (languages != null)
            {
                foreach (Language_Info thisLanguage in languages)
                {
                    results.Append(To_GSA(thisLanguage.Language_Text, "dc.Language", indent));
                    full_citation.Append(thisLanguage.Language_Text + " ");
                }
            }

            // Add dc.publisher
            if ((originInfo != null) && (originInfo.Publishers_Count > 0))
            {
                foreach (string publisher in originInfo.Publishers)
                {
                    results.Append(To_GSA(publisher, "dc.Publisher", indent));
                    full_citation.Append(publisher + " ");
                }
            }

            // Add dc.Rights
            if (accessCondition != null)
            {
                results.Append(To_GSA(accessCondition.Text, "dc.Rights", indent));
                full_citation.Append(accessCondition.Text + " ");
            }

            // Add dc.source ( and holding info to the growing full citation )
            if (source != null)
            {
                results.Append(To_GSA(source.Statement, "dc.Source", indent));
                full_citation.Append(source.Code + " " + source.Statement + " ");
            }

            if (locationInfo != null)
            {
                full_citation.Append(locationInfo.Holding_Code + " " + locationInfo.Holding_Name + " ");
            }

            // Add all dc.subjects ( and some dc.Coverage at the same time)
            if (subjects != null)
            {
                foreach (Subject_Info thisSubject in subjects)
                {
                    results.Append(thisSubject.To_GSA_DublinCore(indent));
                    full_citation.Append(thisSubject.ToString() + " ");
                }
            }

            // Add genres as dc.subjects as well
            if (genres != null)
            {
                foreach (Genre_Info thisGenre in genres)
                {
                    results.Append(To_GSA(thisGenre.Genre_Term, "dc.Subject", indent));
                    full_citation.Append(thisGenre.Genre_Term + " ");
                }
            }

            // Add dc.title
            if (( mainTitle != null ) && (  mainTitle.Title.Length > 0))
            {
                if (mainTitle.Subtitle.Length > 0)
                {
                    results.Append(To_GSA(mainTitle.Title + " " + mainTitle.Subtitle, "dc.Title", indent));
                    full_citation.Append(mainTitle.Title + " " + mainTitle.Subtitle + " ");
                }
                else
                {
                    results.Append(To_GSA(mainTitle.Title, "dc.Title", indent));
                    full_citation.Append(mainTitle.Title + " ");
                }
            }

            // Add dc.title^alternative ( series title )
            if (seriesTitle != null)
            {
                if (seriesTitle.Title.Length > 0)
                {
                    if (seriesTitle.Subtitle.Length > 0)
                    {
                        results.Append(To_GSA(seriesTitle.Title + " " + seriesTitle.Subtitle, "dc.Title^alternative", indent));
                        full_citation.Append(seriesTitle.Title + " " + seriesTitle.Subtitle + " ");
                    }
                    else
                    {
                        results.Append(To_GSA(seriesTitle.Title, "dc.Title^alternative", indent));
                        full_citation.Append(seriesTitle.Title + " ");
                    }
                }
            }

            // Add dc.title^alternative ( other title )
            if (otherTitles != null)
            {
                foreach (Title_Info otherTitle in otherTitles)
                {
                    if (otherTitle.Title.Length > 0)
                    {
                        if (otherTitle.Subtitle.Length > 0)
                        {
                            results.Append(To_GSA(otherTitle.Title + " " + otherTitle.Subtitle, "dc.Title^alternative", indent));
                            full_citation.Append(otherTitle.Title + " " + otherTitle.Subtitle + " ");
                        }
                        else
                        {
                            results.Append(To_GSA(otherTitle.Title, "dc.Title^alternative", indent));
                            full_citation.Append(otherTitle.Title + " ");
                        }
                    }
                }
            }

            // Add dc.type
            if (type != null)
            {
                results.Append(To_GSA(type.MODS_Type_String, "dc.Type", indent));
                full_citation.Append(type.MODS_Type_String + " ");
            }

            // Add sobekCM portion, if requested
            if (Include_SobekCM_Metadata)
            {
                // Add the BibID and VID first
                results.Append(base.To_GSA(bibID, "sobekcm.BibID", indent));
                full_citation.Append(BibID + " ");
                results.Append(base.To_GSA(vid, "sobekcm.VID", indent));
                full_citation.Append(VID + " ");

                // Add sobekcm.affiliation
                if (affiliations != null)
                {
                    foreach (Affiliation_Info thisAffiliation in affiliations)
                    {
                        results.Append(To_GSA(thisAffiliation.Affiliation_XML, "sobekcm.Affiliaton", indent));
                        full_citation.Append(thisAffiliation.Affiliation_XML + " ");
                    }
                }

                // Add sobekcm.coverage
                if (subjects != null)
                {
                    foreach (Subject_Info thisSubject in subjects)
                    {
                        results.Append(thisSubject.To_GSA_SobekCM(indent));
                    }
                }

                // Add sobekcm.donor
                if (donor != null)
                {
                    results.Append(To_GSA(donor.Full_Name, "sobekcm.Donor", indent));
                    full_citation.Append(donor.Full_Name + " ");
                }

                // Add sobekcm.formattype ( for searching format and type at the same time )
                if ((originalPhysicalDesc != null) && (type != null))
                {
                    results.Append(To_GSA(originalPhysicalDesc.Extent + " " + SobekCM_Type_String, "sobekcm.FormatType", indent));
                }

                // Add sobekcm.PubPlace
                if ((originInfo != null) && ( originInfo.Places_Count > 0 ))
                {
                    foreach (Origin_Info_Place pubplace in originInfo.Places)
                    {
                        results.Append(base.To_GSA(pubplace.Place_Text, "sobekcm.PubPlace", indent));
                        full_citation.Append(pubplace.Place_Text + " ");
                    }
                }

                // Add sobekcm.Titles
                if (mainTitle != null)
                {
                    if (mainTitle.Title.Length > 0)
                    {
                        results.Append(To_GSA(mainTitle.Title + " " + mainTitle.Subtitle, "sobekcm.Titles", indent));
                    }
                }

                // Add sobekcm.Titles
                if (seriesTitle != null)
                {
                    if (seriesTitle.Title.Length > 0)
                    {
                        results.Append(To_GSA(seriesTitle.Title + " " + seriesTitle.Subtitle, "sobekcm.Titles", indent));
                    }
                }

                // Add sobekcm.Titles
                if (otherTitles != null)
                {
                    foreach (Title_Info otherTitle in otherTitles)
                    {
                        results.Append(To_GSA(otherTitle.Title + " " + otherTitle.Subtitle, "sobekcm.Titles", indent));
                    }
                }

                // Add the internal metadata for full citation.. ( sobekcm.citation ) 
                results.Append(To_GSA(full_citation.ToString(), "sobekcm.Citation", indent));
            }

            // Return the built information
            return results.ToString();
        }

        #endregion

        #region Methods to create the sort safe title and date

        /// <summary> Calculate the sort title for this resource</summary>
        /// <param name="titleString">Actual title of this resource</param>
        /// <returns>Sortable title value for this resource</returns>
        public string sortSafeTitle(string titleString, bool usePredeterminedSortTitle)
        {
            if (( usePredeterminedSortTitle ) && (SortTitle.Length > 0))
                return sortTitle;

            // Remove all punctuation first
            titleString = titleString.Replace("\"", "").Replace(".", "").Replace("'", "").Replace(",", "");

            // Build the collection of  articles
            string[] articles = new string[9] { "EL", "LA", "LAS", "LE", "LES", "A", "THE", "UNAS", "UNOS" };

            // Step through each article
            string capitalTitle = titleString.ToUpper();
            foreach (string thisArticle in articles)
            {
                if (capitalTitle.IndexOf(thisArticle + " ") == 0)
                {
                    titleString = titleString.Substring(thisArticle.Length + 1).Trim();
                    capitalTitle = titleString.ToUpper();
                }
            }

            // Return this value
            return titleString.ToUpper();
        }

        /// <summary> Calculate the sort date for this resource</summary>
        /// <param name="dateString">Actual date of this resource</param>
        /// <returns>Sortable date value for this resource</returns>
        /// <remarks>This computes the number of days since year January 1, year 1</remarks>
        public int sortSafeDate(string dateString)
        {
            // If there is no date, do nothing
            if (dateString.Trim().Length == 0)
            {
                return -1;
            }

            // If there is already a sort date, use that
            if (sortDate > 0)
                return sortDate;

            // First, check to see if this is a proper date already
            try
            {
                // Try conversion
                DateTime thisDate = Convert.ToDateTime(dateString);

                // Conversion successful, so count days
                TimeSpan timeElapsed = thisDate.Subtract(new DateTime(1, 1, 1));
                sortDate = (int)timeElapsed.TotalDays;
                return sortDate;
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
                    DateTime thisYear = new DateTime(Convert.ToInt16(year), 1, 1);
                    TimeSpan timeElapsed = thisYear.Subtract(new DateTime(1, 1, 1));
                    sortDate = (int)timeElapsed.TotalDays;
                    return sortDate;
                }
            }

            // Return this value, as empty
            return -1;
        }

        #endregion

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
                        if (String.Compare(thisGenre.Authority, "sobekcm", true) == 0)
                        {
                            sobekcm_genre = thisGenre.Genre_Term;

                            // Special code here looking for project
                            if (String.Compare(sobekcm_genre, "project", true) == 0)
                                return TypeOfResource_SobekCM_Enum.Project;

                            // Special code here looking for multivolume
                            if (String.Compare(sobekcm_genre, "multivolume", true) == 0)
                                return TypeOfResource_SobekCM_Enum.Multivolume;
                        }
                        else if (String.Compare(thisGenre.Authority, "marcgt", true) == 0)
                        {
                            if (String.Compare(thisGenre.Genre_Term, "newspaper", true) == 0)
                                sobekcm_genre = "newspaper";
                            if (String.Compare(thisGenre.Genre_Term, "serial", true) == 0)
                                sobekcm_genre = "serial";
                        }
                    }
                }

                // There should always be a mods type, so use that
                switch( type.MODS_Type )
                {
                    case TypeOfResource_MODS_Enum.Cartographic:
                        return TypeOfResource_SobekCM_Enum.Map;

                    case TypeOfResource_MODS_Enum.Mixed_Material:
                        return TypeOfResource_SobekCM_Enum.Archival;

                    case TypeOfResource_MODS_Enum.Moving_Image:
                        return TypeOfResource_SobekCM_Enum.Video;

                    case TypeOfResource_MODS_Enum.Notated_Music:
                        return TypeOfResource_SobekCM_Enum.Archival;

                    case TypeOfResource_MODS_Enum.Sofware_Multimedia:
                        return TypeOfResource_SobekCM_Enum.Archival;

                    case TypeOfResource_MODS_Enum.Sound_Recording:
                    case TypeOfResource_MODS_Enum.Sound_Recording_Musical:
                    case TypeOfResource_MODS_Enum.Sound_Recording_Nonmusical:
                        return TypeOfResource_SobekCM_Enum.Audio;

                    case TypeOfResource_MODS_Enum.Still_Image:
                        if ( String.Compare( sobekcm_genre, "aerial photography", true ) == 0 )
                            return TypeOfResource_SobekCM_Enum.Aerial;
                        else
                            return TypeOfResource_SobekCM_Enum.Photograph;

                    case TypeOfResource_MODS_Enum.Text:
                        switch( sobekcm_genre.ToUpper() )
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
                    List<Genre_Info> sobekcmGenres = new List<Genre_Info>();
                    foreach (Genre_Info thisGenre in genres)
                    {
                        if (String.Compare(thisGenre.Authority, "sobekcm", true) == 0)
                        {
                            sobekcmGenres.Add(thisGenre);
                        }
                    }
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

                    case TypeOfResource_SobekCM_Enum.UNKNOWN:
                        type.MODS_Type = TypeOfResource_MODS_Enum.UNKNOWN;
                        break;

                    case TypeOfResource_SobekCM_Enum.Video:
                        type.MODS_Type = TypeOfResource_MODS_Enum.Moving_Image;
                        break;
                }
            }
        }

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

                    case TypeOfResource_SobekCM_Enum.Map:
                        return "Map";

                    case TypeOfResource_SobekCM_Enum.Multivolume:
                        return "Multivolume";

                    case TypeOfResource_SobekCM_Enum.Newspaper:
                        return "Newspaper";

                    case TypeOfResource_SobekCM_Enum.Photograph:
                        return "Photograph";

                    case TypeOfResource_SobekCM_Enum.Project:
                        return "Project";

                    case TypeOfResource_SobekCM_Enum.Serial:
                        return "Serial";

                    case TypeOfResource_SobekCM_Enum.UNKNOWN:
                        return "Unknown";

                    case TypeOfResource_SobekCM_Enum.Video:
                        return "Video";
                }
                return "Unknown";
            }
            set
            {
                switch (value.ToUpper().Trim())
                {
                    case "AERIAL":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Aerial;
                        return;

                    case "ARCHIVAL":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Archival;
                        return;

                    case "ARTIFACT":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Artifact;
                        return;

                    case "AUDIO":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Audio;
                        return;

                    case "BOOK":
                    case "TEXT":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Book;
                        return;

                    case "MAP":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Map;
                        return;

                    case "MULTIVOLUME":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Multivolume;
                        return;

                    case "NEWSPAPER":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Newspaper;
                        return;

                    case "PHOTOGRAPH":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Photograph;
                        return;

                    case "PROJECT":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Project;
                        return;

                    case "SERIAL":
                        SobekCM_Type = TypeOfResource_SobekCM_Enum.Serial;
                        return;

                    case "VIDEO":
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