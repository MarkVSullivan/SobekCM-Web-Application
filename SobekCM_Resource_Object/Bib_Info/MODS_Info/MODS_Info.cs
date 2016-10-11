#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using SobekCM.Resource_Object.Metadata_Modules.VRACore;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> Contains all the data elements which are expressed in MODS format.  </summary>
    /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center. </remarks>
    [Serializable]
    public class MODS_Info : XML_Writing_Base_Type
    {
        /// <summary> Protected field contains the collection of summary/abstracts associated with this digital resource </summary>
        protected List<Abstract_Info> abstracts;

        /// <summary> Protected field contains the rights associated with the use of this digital resource </summary>
        protected AccessCondition_Info accessCondition;

        /// <summary> Protected field contains the collection of classifications associated with this digital resource </summary>
        protected List<Classification_Info> classifications;

        /// <summary> Protected field contains the donor object for this material  </summary>
        protected Name_Info donor;

        ///// <summary> Protected field contains the physical description (extend and notes) contained within the resource itself, rather than refering to the original resource </summary>
        //protected PhysicalDescription_Info physicalDesc;

        /// <summary> Protected field contains the collection of genre terms associated with this digital resource </summary>
        protected List<Genre_Info> genres;

        /// <summary> Protected field contains the collection of subjects associated with this digital resource </summary>
        protected List<Identifier_Info> identifiers;

        /// <summary> Protected field contains the collection of languages associated with this digital resource </summary>
        protected List<Language_Info> languages;

        /// <summary> Protected field contains the location object that holds information about the physical location of the original document and
        /// any URL information for the item or related EADs  </summary>
        protected Location_Info locationInfo;

        /// <summary> Protected field contains the main title associated with this digital resource </summary>
        protected Title_Info mainTitle;

        /// <summary> Protected field contains the main entity information (main author, etc..) for this digital resource </summary>
        protected Name_Info main_entity_name;

        /// <summary> Protected field contains the collection of names associated with this digital resource </summary>
        protected List<Name_Info> names;

        /// <summary> Protected field contains the collection of notes associated with this digital resource </summary>
        protected List<Note_Info> notes;

        /// <summary> Protected field contains the origination information ( publisher, years, frequency, etc..) associated with this digital resource </summary>
        protected MODS_Origin_Info ModsOriginInfo;

        /// <summary> Protected field contains the physical description (extent and notes) of the original resource, which is encoded as a related item in the MODS </summary>
        protected PhysicalDescription_Info originalPhysicalDesc;

        /// <summary> Protected field contains the collection of subjects associated with this digital resource </summary>
        protected List<Title_Info> otherTitles;

        /// <summary> Protected field contains the information about the actual record which describes the digital resource, including original source, cataloging language, main identifier, etc... </summary>
        protected Record_Info recordInfo;

        /// <summary> Protected field contains the collection of related items associated with this digital resource </summary>
        protected List<Related_Item_Info> relatedItems;

        /// <summary> Protected field contains the series part information for this resource </summary>
        protected Part_Info seriesPartInfo;

        /// <summary> Protected field contains the series title associated with this digital resource </summary>
        protected Title_Info seriesTitle;

        /// <summary> Protected field contains the collection of subjects associated with this digital resource </summary>
        protected List<Subject_Info> subjects;

        /// <summary> Protected field contains the table of contents value in this MODS portion of the metadata file </summary>
        protected string tableOfContents;

        /// <summary> Protected field contains the collection of subjects associated with this digital resource </summary>
        protected List<TargetAudience_Info> targetAudiences;

        /// <summary> Protected field contains the original resource type such as map, aerial photography, book, serial, etc..   </summary>
        protected TypeOfResource_Info type;

        #region Constructors

        /// <summary> Constructor for a new instance of the MODS_Info class. </summary>
        public MODS_Info()
        {
            // Perform some preliminary configuration
            ModsOriginInfo = new MODS_Origin_Info();
            accessCondition = new AccessCondition_Info();
            originalPhysicalDesc = new PhysicalDescription_Info();
            //physicalDesc = new PhysicalDescription_Info();
            mainTitle = new Title_Info();
            type = new TypeOfResource_Info();
            recordInfo = new Record_Info();
        }

        #endregion

        #region Public Properties

        /// <summary> Original resource type such as map, aerial photography, book, serial, etc..  </summary>
        public TypeOfResource_Info Type
        {
            get { return type; }
        }

        /// <summary> Information about the actual record which describes the digital resource, including original source, cataloging language, main identifier, etc... </summary>
        public Record_Info Record
        {
            get { return recordInfo; }
        }

        /// <summary> Origination information ( publisher, years, frequency, etc..) associated with this digital resource </summary>
        public MODS_Origin_Info Origin_Info
        {
            get { return ModsOriginInfo; }
        }

        /// <summary> Physical description (extent, notes, and form) of the original resource, encoded as a related item </summary>
        public PhysicalDescription_Info Original_Description
        {
            get { return originalPhysicalDesc; }
        }

        ///// <summary> Physical description (extent, notes, and form) of the digital resource </summary>
        //public PhysicalDescription_Info Physical_Description
        //{
        //    get { return physicalDesc; }
        //}

        /// <summary> Main title associated with this digital resource </summary>
        public Title_Info Main_Title
        {
            get { return mainTitle; }
            set { mainTitle = value; }
        }

        /// <summary> Rights associated with the use of this digital resource </summary>
        public AccessCondition_Info Access_Condition
        {
            get { return accessCondition; }
        }

        /// <summary> Gets or sets the table of contents value in this MODS portion of the metadata file </summary>
        /// <remarks>This is used for retaining the original TOC from an existing catalog record.</remarks>
        public string TableOfContents
        {
            get { return tableOfContents ?? String.Empty; }
            set { tableOfContents = value; }
        }

        /// <summary> The number of abstracts associated with this digital resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Abstracts"/> property.  Even if 
        /// there are no abstracts, the Abstracts property creates a readonly collection to pass back out.</remarks>
        public int Abstracts_Count
        {
            get {
                return abstracts == null ? 0 : abstracts.Count;
            }
        }

        /// <summary> Collection of all the abstracts/summaries associated with this digital resource </summary>
        /// <remarks> You should check the count of abstracts first using the <see cref="Abstracts_Count"/> property before using this property.
        /// Even if there are no abstracts, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Abstract_Info> Abstracts
        {
            get {
                return abstracts == null ? new ReadOnlyCollection<Abstract_Info>(new List<Abstract_Info>()) : new ReadOnlyCollection<Abstract_Info>(abstracts);
            }
        }

        /// <summary> The number of described items which are related to this digital resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="RelatedItems"/> property.  Even if 
        /// there are no related items, the RelatedItems property creates a readonly collection to pass back out.</remarks>
        public int RelatedItems_Count
        {
            get {
                return relatedItems == null ? 0 : relatedItems.Count;
            }
        }

        /// <summary> Collection of all the information about items related to this digital resource </summary>
        /// <remarks> You should check the count of related items first using the <see cref="RelatedItems_Count"/> property before using this property.
        /// Even if there are no related items, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Related_Item_Info> RelatedItems
        {
            get {
                return relatedItems == null ? new ReadOnlyCollection<Related_Item_Info>(new List<Related_Item_Info>()) : new ReadOnlyCollection<Related_Item_Info>(relatedItems);
            }
        }

        /// <summary> The number of genre types associated with this digital resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Genres"/> property.  Even if 
        /// there are no genres, the Genres property creates a readonly collection to pass back out.</remarks>
        public int Genres_Count
        {
            get {
                return genres == null ? 0 : genres.Count;
            }
        }

        /// <summary> Collection of genre types associated with this digital resource </summary>
        /// <remarks> You should check the count of genres first using the <see cref="Genres_Count"/> property before using this property.
        /// Even if there are no genres, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Genre_Info> Genres
        {
            get {
                return genres == null ? new ReadOnlyCollection<Genre_Info>(new List<Genre_Info>()) : new ReadOnlyCollection<Genre_Info>(genres);
            }
        }

        /// <summary> The number of alternate titles associated with this digital resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Other_Titles"/> property.  Even if 
        /// there are no other titles, the Other_Titles property creates a readonly collection to pass back out.</remarks>
        public int Other_Titles_Count
        {
            get {
                return otherTitles == null ? 0 : otherTitles.Count;
            }
        }

        /// <summary> Collection of alternate titles associated with this digital resource </summary>
        /// <remarks> You should check the count of other titles first using the <see cref="Other_Titles_Count"/> property before using this property.
        /// Even if there are no other titles, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Title_Info> Other_Titles
        {
            get {
                return otherTitles == null ? new ReadOnlyCollection<Title_Info>(new List<Title_Info>()) : new ReadOnlyCollection<Title_Info>(otherTitles);
            }
        }


        /// <summary> The number of languages associated with this digital resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Languages"/> property.  Even if 
        /// there are no languages, the Languages property creates a readonly collection to pass back out.</remarks>
        public int Languages_Count
        {
            get {
                return languages == null ? 0 : languages.Count;
            }
        }

        /// <summary> Collection of languages associated with this digital resource </summary>
        /// <remarks> You should check the count of languages first using the <see cref="Languages_Count"/> property before using this property.
        /// Even if there are no languages, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Language_Info> Languages
        {
            get {
                return languages == null ? new ReadOnlyCollection<Language_Info>(new List<Language_Info>()) : new ReadOnlyCollection<Language_Info>(languages);
            }
        }

        /// <summary> The number of classifications associated with this digital resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Classifications"/> property.  Even if 
        /// there are no classifications, the Classifications property creates a readonly collection to pass back out.</remarks>
        public int Classifications_Count
        {
            get {
                return classifications == null ? 0 : classifications.Count;
            }
        }

        /// <summary> Collection of classifications associated with this digital resource </summary>
        /// <remarks> You should check the count of classifications first using the <see cref="Classifications_Count"/> property before using this property.
        /// Even if there are no classifications, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Classification_Info> Classifications
        {
            get {
                return classifications == null ? new ReadOnlyCollection<Classification_Info>(new List<Classification_Info>()) : new ReadOnlyCollection<Classification_Info>(classifications);
            }
        }

        /// <summary> The number of identifiers associated with this digital resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Identifiers"/> property.  Even if 
        /// there are no identifiers, the Identifiers property creates a readonly collection to pass back out.</remarks>
        public int Identifiers_Count
        {
            get {
                return identifiers == null ? 0 : identifiers.Count;
            }
        }

        /// <summary> Collection of identifiers associated with this digital resource </summary>
        /// <remarks> You should check the count of identifiers first using the <see cref="Identifiers_Count"/> property before using this property.
        /// Even if there are no identifiers, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Identifier_Info> Identifiers
        {
            get {
                return identifiers == null ? new ReadOnlyCollection<Identifier_Info>(new List<Identifier_Info>()) : new ReadOnlyCollection<Identifier_Info>(identifiers);
            }
        }

        /// <summary> The number of subject keywords associated with this digital resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Subjects"/> property.  Even if 
        /// there are no subject keywords, the Subjects property creates a readonly collection to pass back out.</remarks>
        public int Subjects_Count
        {
            get {
                return subjects == null ? 0 : subjects.Count;
            }
        }

        /// <summary> Collection of subject keywords associated with this digital resource </summary>
        /// <remarks> You should check the count of subject keywords first using the <see cref="Subjects_Count"/> property before using this property.
        /// Even if there are no subject keywords, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Subject_Info> Subjects
        {
            get {
                return subjects == null ? new ReadOnlyCollection<Subject_Info>(new List<Subject_Info>()) : new ReadOnlyCollection<Subject_Info>(subjects);
            }
        }

        /// <summary> The number of notes associated with this digital resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Notes"/> property.  Even if 
        /// there are no notes, the Notes property creates a readonly collection to pass back out.</remarks>
        public int Notes_Count
        {
            get {
                return notes == null ? 0 : notes.Count;
            }
        }

        /// <summary> Collection of notes associated with this digital resource </summary>
        /// <remarks> You should check the count of notes first using the <see cref="Notes_Count"/> property before using this property.
        /// Even if there are no notes, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Note_Info> Notes
        {
            get {
                return notes == null ? new ReadOnlyCollection<Note_Info>(new List<Note_Info>()) : new ReadOnlyCollection<Note_Info>(notes);
            }
        }

        /// <summary> The number of names (i.e., creators, contributors) associated with this digital resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Names"/> property.  Even if 
        /// there are no names, the Names property creates a readonly collection to pass back out.</remarks>
        public int Names_Count
        {
            get {
                return names == null ? 0 : names.Count;
            }
        }

        /// <summary> Collection of names (i.e., creators, contributors) associated with this digital resource </summary>
        /// <remarks> You should check the count of names first using the <see cref="Names_Count"/> property before using this property.
        /// Even if there are no names, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Name_Info> Names
        {
            get {
                return names == null ? new ReadOnlyCollection<Name_Info>(new List<Name_Info>()) : new ReadOnlyCollection<Name_Info>(names);
            }
        }

        /// <summary> The number of audiences targeted (i.e., juvenile, adult, etc..) by the content of this digital resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Target_Audiences"/> property.  Even if 
        /// there are no target audiences, the Target_Audiences property creates a readonly collection to pass back out.</remarks>
        public int Target_Audiences_Count
        {
            get {
                return targetAudiences == null ? 0 : targetAudiences.Count;
            }
        }

        /// <summary> Collection of audiences targeted (i.e., juvenile, adult, etc..) by the content of this digital resource </summary>
        /// <remarks> You should check the count of target audiences first using the <see cref="Target_Audiences_Count"/> property before using this property.
        /// Even if there are no target audiences, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<TargetAudience_Info> Target_Audiences
        {
            get {
                return targetAudiences == null ? new ReadOnlyCollection<TargetAudience_Info>(new List<TargetAudience_Info>()) : new ReadOnlyCollection<TargetAudience_Info>(targetAudiences);
            }
        }

        /// <summary> Flag indicates if there is the possibility of some location information
        /// being present for this digital resource </summary>
        public bool hasLocationInformation
        {
            get {
                return locationInfo != null;
            }
        }

        /// <summary> Location object holds information about the physical location of the original document and
        /// any URL information for the item or related EADs </summary>
        public Location_Info Location
        {
            get { return locationInfo ?? (locationInfo = new Location_Info()); }
        }

        /// <summary> Flag indicates if there is the possibility of some donor information
        /// being present for this digital resource </summary>
        public bool hasDonor
        {
            get {
                return donor != null;
            }
        }

        /// <summary> Gets and sets the donor object for this material </summary>
        public Name_Info Donor
        {
            get { return donor ?? (donor = new Name_Info()); }
            set { donor = value; }
        }

        /// <summary> Flag indicates if there is the main entity information (main author, etc..) for this digital resource </summary>
        public bool hasMainEntityName
        {
            get {
                return main_entity_name != null && main_entity_name.hasData;
            }
        }

        /// <summary> Gets and sets the main entity information (main author, etc..) for this digital resource </summary>
        public Name_Info Main_Entity_Name
        {
            get { return main_entity_name ?? (main_entity_name = new Name_Info()); }
            set { main_entity_name = value; }
        }

        /// <summary> Flag indicates if there is a series title associated with this digital resource </summary>
        public bool hasSeriesTitle
        {
            get {
                return (seriesTitle != null) && (seriesTitle.Title.Length > 0);
            }
        }

        /// <summary> Gets or sets the series title associated with this digital resource </summary>
        public Title_Info SeriesTitle
        {
            get { return seriesTitle ?? (seriesTitle = new Title_Info()); }
            set { seriesTitle = value; }
        }

        /// <summary> Flag indicates if there is series part information for this resource  </summary>
        public bool hasSeriesPartInfo
        {
            get {
                return (seriesPartInfo != null) && (seriesPartInfo.hasData);
            }
        }

        /// <summary> Gets and sets the series part information for this resource </summary>
        public Part_Info Series_Part_Info
        {
            get { return seriesPartInfo ?? (seriesPartInfo = new Part_Info()); }
            set { seriesPartInfo = value; }
        }

        #endregion

        #region Public Methods

        #region Methods to add named entities to this resource 

        /// <summary> Clears all of the named entities which were associated with this digital resource </summary>
        public void Clear_Names()
        {
            if (names != null)
                names.Clear();
        }

        /// <summary> Removes a named entity from the collection of names associated with this digital resource </summary>
        /// <remarks> Named entity to remove</remarks>
        public void Remove_Name(Name_Info Name)
        {
            if ((names != null) && (names.Contains(Name)))
                names.Remove(Name);
        }

        /// <summary> Add a new named entity (i.e., creator, contributor) associated with this digital resource  </summary>
        /// <param name="New_Name"> Named entity object </param>
        /// <returns>Newly built name subject</returns>
        public void Add_Named_Entity(Name_Info New_Name)
        {
            if (names == null)
                names = new List<Name_Info>();

            names.Add(New_Name);
        }

        /// <summary> Add a new named entity (i.e., creator, contributor) associated with this digital resource  </summary>
        /// <param name="Name"> Name of the new named entity to associate</param>
        /// <returns>Newly built and added name entity</returns>
        public Name_Info Add_Named_Entity(string Name)
        {
            if (names == null)
                names = new List<Name_Info>();

            Name_Info newName = new Name_Info(Name, String.Empty);
            names.Add(newName);
            return newName;
        }

        /// <summary> Add a new named entity (i.e., creator, contributor) associated with this digital resource  </summary>
        /// <param name="Name"> Name of the new named entity to associate</param>
        /// <param name="Text_Role"> Role of this named entity </param>
        /// <returns>Newly built and added name entity</returns>
        public Name_Info Add_Named_Entity(string Name, string Text_Role)
        {
            if (names == null)
                names = new List<Name_Info>();

            Name_Info newName = new Name_Info(Name, Text_Role);
            names.Add(newName);
            return newName;
        }

        /// <summary> Add a new named entity (i.e., creator, contributor) associated with this digital resource  </summary>
        /// <param name="Name"> Name of the new named entity to associate</param>
        /// <param name="Text_Role"> Role of this named entity </param>
        /// <param name="MARC_Role"> ROle of thie entity encoded in MARC </param>
        /// <returns>Newly built and added name entity</returns>
        public Name_Info Add_Named_Entity(string Name, string Text_Role, string MARC_Role)
        {
            if (names == null)
                names = new List<Name_Info>();

            Name_Info newName = new Name_Info(Name, Text_Role);
            newName.Add_Role(MARC_Role, "marcrelator", Name_Info_Role_Type_Enum.Code);
            names.Add(newName);
            return newName;
        }

        #endregion

        #region Methods to add genres to this resource

        /// <summary> Clears any existing genre terms from this item  </summary>
        public void Clear_Genres()
        {
            if (genres != null)
                genres.Clear();
        }

        /// <summary> Removes a single genre from the list of genre terms associated with this item </summary>
        /// <param name="Genre"> Genre term to remove </param>
        public void Remove_Genre(Genre_Info Genre)
        {
            if ((genres != null) && (genres.Contains(Genre)))
                genres.Remove(Genre);
        }

        /// <summary> Add a new genre to this item </summary>
        /// <param name="Genre">Genre for this item</param>
        public void Add_Genre(Genre_Info Genre)
        {
            if (genres == null)
                genres = new List<Genre_Info>();

            genres.Add(Genre);
        }

        /// <summary> Add a new genre term to this item </summary>
        /// <param name="Genre_Term">Genre term for this item</param>
        /// <returns>Newly built and added genre object </returns>
        public Genre_Info Add_Genre(string Genre_Term)
        {
            return Add_Genre(Genre_Term, String.Empty);
        }

        /// <summary> Add a new genre term to this item </summary>
        /// <param name="Genre_Term">Genre term for this item</param>
        /// <param name="Authority">Authority for this genre term</param>
        /// <returns>Newly built and added genre object </returns>
        public Genre_Info Add_Genre(string Genre_Term, string Authority)
        {
            if (genres == null)
                genres = new List<Genre_Info>();

            // Create the new genre object
            Genre_Info newGenre = new Genre_Info(Genre_Term, Authority);

            // If this is unique, add to the collection 
            if (!genres.Contains(newGenre))
            {
                genres.Add(newGenre);
                return newGenre;
            }
            else
            {
                Genre_Info returnGenre = genres.Find(newGenre.Equals);
                if (String.Compare(Authority, returnGenre.Authority, true) != 0)
                {
                    genres.Add(newGenre);
                    return newGenre;
                }
                return returnGenre;
            }
        }

        #endregion

        #region Methods to add notes to this resource

        /// <summary> Clears all of the notes which were associated with this digital resource </summary>
        public void Clear_Notes()
        {
            if (notes != null)
                notes.Clear();
        }

        /// <summary> Removes a single note from the collection of notes associated with this digital resource </summary>
        /// <param name="Note"> Note to remove </param>
        public void Remove_Note(Note_Info Note)
        {
            if ((notes != null) && (notes.Contains(Note)))
                notes.Remove(Note);
        }

        /// <summary> Add a new note which describes either this digital resource or the original item </summary>
        /// <param name="New_Note">Note object</param>
        public void Add_Note(Note_Info New_Note)
        {
            if (notes == null)
                notes = new List<Note_Info>();

            if (!notes.Contains(New_Note))
            {
                notes.Add(New_Note);
            }
        }

        /// <summary> Add a new note which describes either this digital resource or the original item </summary>
        /// <param name="Note">Text of the note</param>
        /// <returns>Newly built and added note object</returns>
        public Note_Info Add_Note(string Note)
        {
            if (notes == null)
                notes = new List<Note_Info>();

            Note_Info newNote = new Note_Info(Note);
            if (!notes.Contains(newNote))
            {
                notes.Add(newNote);
                return newNote;
            }
            else
            {
                return notes.Find(newNote.Equals);
            }
        }

        /// <summary> Add a new note which describes either this digital resource or the original item </summary>
        /// <param name="Note">Text of the note</param>
        /// <param name="Note_Type">Type of Note</param>
        /// <returns>Newly built and added note object</returns>
        public Note_Info Add_Note(string Note, string Note_Type)
        {
            if (notes == null)
                notes = new List<Note_Info>();

            Note_Info newNote = new Note_Info(Note, Note_Type);
            if (!notes.Contains(newNote))
            {
                notes.Add(newNote);
                return newNote;
            }
            else
            {
                return notes.Find(newNote.Equals);
            }
        }

        /// <summary> Add a new note which describes either this digital resource or the original item </summary>
        /// <param name="Note">Text of the note</param>
        /// <param name="Note_Type">Type of Note</param>
        /// <returns>Newly built and added note object</returns>
        public Note_Info Add_Note(string Note, Note_Type_Enum Note_Type)
        {
            if (notes == null)
                notes = new List<Note_Info>();

            Note_Info newNote = new Note_Info(Note, Note_Type);
            if (!notes.Contains(newNote))
            {
                notes.Add(newNote);
                return newNote;
            }
            else
            {
                return notes.Find(newNote.Equals);
            }
        }

        /// <summary> Add a new note which describes either this digital resource or the original item </summary>
        /// <param name="Note">Text of the note</param>
        /// <param name="Note_Type">Type of Note</param>
        /// <param name="Display_Label">Display Label for this note</param>
        /// <returns>Newly built and added note object</returns>
        public Note_Info Add_Note(string Note, Note_Type_Enum Note_Type, string Display_Label)
        {
            if (notes == null)
                notes = new List<Note_Info>();

            Note_Info newNote = new Note_Info(Note, Note_Type, Display_Label);
            if (!notes.Contains(newNote))
            {
                notes.Add(newNote);
                return newNote;
            }
            else
            {
                Note_Info returnNote = notes.Find(newNote.Equals);
                if (Display_Label.Length > 0)
                {
                    returnNote.Display_Label = Display_Label;
                }
                return returnNote;
            }
        }

        /// <summary> Add a new note which describes either this digital resource or the original item </summary>
        /// <param name="Note">Text of the note</param>
        /// <param name="Note_Type">Type of Note</param>
        /// <param name="Display_Label">Display Label for this note</param>
        /// <returns>Newly built and added note object</returns>
        public Note_Info Add_Note(string Note, string Note_Type, string Display_Label)
        {
            if (notes == null)
                notes = new List<Note_Info>();

            Note_Info newNote = new Note_Info(Note, Note_Type, Display_Label);
            if (!notes.Contains(newNote))
            {
                notes.Add(newNote);
                return newNote;
            }
            else
            {
                Note_Info returnNote = notes.Find(newNote.Equals);
                if (Display_Label.Length > 0)
                {
                    returnNote.Display_Label = Display_Label;
                }
                return returnNote;
            }
        }

        #endregion

        #region Methods to add abstracts to this resource

        /// <summary> Clears all the abstracts (summary, content scope, etc.. ) which were related to this item </summary>
        public void Clear_Abstracts()
        {
            if (abstracts != null)
                abstracts.Clear();
        }

        /// <summary> Checks to see if an abstract is part of the collection of abstracts associated with this item </summary>
        /// <param name="Abstract"> Abstract/summary to check </param>
        /// <returns> TRUE if this abstract is already in the collection, otherwise FALSE </returns>
        public bool Contains_Abstract(Abstract_Info Abstract)
        {
            if (abstracts == null)
                return false;
            else
                return abstracts.Contains(Abstract);
        }

        /// <summary> Adds a new abstract (summary, content scope, etc.. ) to this item </summary>
        /// <param name="New_Abstract">New abstract to add to this item </param>
        /// <returns>Either the abstract object passed in, or if there is a similar abstract added already, the matching abstract</returns>
        public Abstract_Info Add_Abstract(Abstract_Info New_Abstract)
        {
            if (abstracts == null)
                abstracts = new List<Abstract_Info>();

            if (!abstracts.Contains(New_Abstract))
            {
                abstracts.Add(New_Abstract);
                return New_Abstract;
            }
            else
            {
                return abstracts.Find(New_Abstract.Equals);
            }
        }

        /// <summary> Adds a new abstract (summary, content scope, etc.. ) to this item </summary>
        /// <param name="Abstract">Text of the abstract</param>
        /// <returns>Newly built and added abstract object</returns>
        public Abstract_Info Add_Abstract(string Abstract)
        {
            if (abstracts == null)
                abstracts = new List<Abstract_Info>();

            Abstract_Info newAbstract = new Abstract_Info(Abstract, String.Empty);
            if (!abstracts.Contains(newAbstract))
            {
                abstracts.Add(newAbstract);
                return newAbstract;
            }
            else
            {
                return abstracts.Find(newAbstract.Equals);
            }
        }


        /// <summary> Adds a new abstract (summary, content scope, etc.. ) to this item </summary>
        /// <param name="Abstract">Text of the abstract</param>
        /// <param name="Language">Language of the abstract</param>
        /// <returns>Newly built and added abstract object</returns>
        public Abstract_Info Add_Abstract(string Abstract, string Language)
        {
            if (abstracts == null)
                abstracts = new List<Abstract_Info>();

            Abstract_Info newAbstract = new Abstract_Info(Abstract, Language);
            if (!abstracts.Contains(newAbstract))
            {
                abstracts.Add(newAbstract);
                return newAbstract;
            }
            else
            {
                Abstract_Info returnAbstract = abstracts.Find(newAbstract.Equals);
                if (Language.Length > 0)
                {
                    returnAbstract.Language = Language;
                }
                return returnAbstract;
            }
        }

        /// <summary> Adds a new abstract (summary, content scope, etc.. ) to this item </summary>
        /// <param name="Abstract">Text of the abstract</param>
        /// <param name="Language">Language of the abstract</param>
        /// <param name="DisplayLabel">Display label for this abstract</param>
        /// <param name="Type">Type of abstract</param>
        /// <returns>Newly built and added abstract object</returns>
        public Abstract_Info Add_Abstract(string Abstract, string Language, string Type, string DisplayLabel)
        {
            if (abstracts == null)
                abstracts = new List<Abstract_Info>();

            Abstract_Info newAbstract = new Abstract_Info(Abstract, Language);
            newAbstract.Display_Label = DisplayLabel;
            newAbstract.Type = Type;
            if (!abstracts.Contains(newAbstract))
            {
                abstracts.Add(newAbstract);
                return newAbstract;
            }
            else
            {
                Abstract_Info returnAbstract = abstracts.Find(newAbstract.Equals);
                if (Language.Length > 0)
                {
                    returnAbstract.Language = Language;
                }
                return returnAbstract;
            }
        }

        #endregion

        #region Methods to add classifications to this resource

        /// <summary> Clears all the classifications which were related to this item </summary>
        public void Clear_Classifications()
        {
            if (classifications != null)
                classifications.Clear();
        }

        /// <summary> Adds a new classifications to this item </summary>
        /// <param name="Classification">Classification</param>
        /// <returns>Newly built and added classification object, or or if there is a similar classification added already, the matching classification</returns>
        public Classification_Info Add_Classification(Classification_Info Classification)
        {
            if (classifications == null)
                classifications = new List<Classification_Info>();

            if (!classifications.Contains(Classification))
            {
                classifications.Add(Classification);
                return Classification;
            }
            else
            {
                Classification_Info returnClassification = classifications.Find(Classification.Equals);
                if (Classification.Authority.Length > 0)
                    returnClassification.Authority = Classification.Authority;
                if (Classification.Display_Label.Length > 0)
                    returnClassification.Display_Label = Classification.Display_Label;
                if (Classification.Edition.Length > 0)
                    returnClassification.Edition = Classification.Edition;
                return returnClassification;
            }
        }

        /// <summary> Adds a new classification to this item </summary>
        /// <param name="Classification">Classification</param>
        /// <returns>Newly built and added classification object, or or if there is a similar classification added already, the matching classification</returns>
        public Classification_Info Add_Classification(string Classification)
        {
            return Add_Classification(Classification, String.Empty);
        }

        /// <summary> Adds a new classification to this item </summary>
        /// <param name="Classification">Classification</param>
        /// <param name="Authority">Authority</param>
        /// <returns>Newly built and added classification object, or or if there is a similar classification added already, the matching classification</returns>
        public Classification_Info Add_Classification(string Classification, string Authority)
        {
            if (classifications == null)
                classifications = new List<Classification_Info>();

            Classification_Info newClassification = new Classification_Info(Classification, Authority);
            if (!classifications.Contains(newClassification))
            {
                classifications.Add(newClassification);
                return newClassification;
            }
            else
            {
                Classification_Info returnClassification = classifications.Find(newClassification.Equals);
                if (newClassification.Authority.Length > 0)
                    returnClassification.Authority = newClassification.Authority;
                if (newClassification.Display_Label.Length > 0)
                    returnClassification.Display_Label = newClassification.Display_Label;
                if (newClassification.Edition.Length > 0)
                    returnClassification.Edition = newClassification.Edition;
                return returnClassification;
            }
        }

        /// <summary> Remove an existing classification from this digital resource </summary>
        /// <param name="Classification"> Classification to remove </param>
        public void Remove_Classification(Classification_Info Classification)
        {
            if (classifications == null)
                return;
            else
                classifications.Remove(Classification);
        }

        #endregion

        #region Methods to add identifiers to this resource

        /// <summary> Clears all the identifiers which were related to this item </summary>
        public void Clear_Identifiers()
        {
            if (identifiers != null)
                identifiers.Clear();
        }

        /// <summary> Adds a new identifier to this item </summary>
        /// <param name="Identifier">Identifier</param>
        /// <returns>Newly built and added identifier object, or or if there is a similar identifier added already, the matching identifier</returns>
        public Identifier_Info Add_Identifier(Identifier_Info Identifier)
        {
            if (identifiers == null)
                identifiers = new List<Identifier_Info>();

            if (!identifiers.Contains(Identifier))
            {
                identifiers.Add(Identifier);
                return Identifier;
            }
            else
            {
                Identifier_Info returnIdentifier = identifiers.Find(Identifier.Equals);
                if (Identifier.Type.Length > 0)
                    returnIdentifier.Type = Identifier.Type;
                if (Identifier.Display_Label.Length > 0)
                    returnIdentifier.Display_Label = Identifier.Display_Label;
                return returnIdentifier;
            }
        }

        /// <summary> Adds a new identifier to this item </summary>
        /// <param name="Identifier">Identifier</param>
        /// <returns>Newly built and added identifier object, or or if there is a similar identifier added already, the matching identifier</returns>
        public Identifier_Info Add_Identifier(string Identifier)
        {
            return Add_Identifier(Identifier, String.Empty, String.Empty);
        }

        /// <summary> Adds a new identifier to this item </summary>
        /// <param name="Type">Uncontrolled Identifier Type</param>
        /// <param name="Identifier">Identifier</param>
        /// <returns>Newly built and added identifier object, or or if there is a similar identifier added already, the matching identifier</returns>
        public Identifier_Info Add_Identifier(string Identifier, string Type)
        {
            return Add_Identifier(Identifier, Type, String.Empty);
        }

        /// <summary> Adds a new identifier to this item </summary>
        /// <param name="Type">Uncontrolled Identifier Type</param>
        /// <param name="Identifier">Identifier</param>
        /// <param name="DisplayLabel">Additional text associated with the identifier necessary for display.</param>
        /// <returns>Newly built and added identifier object, or or if there is a similar identifier added already, the matching identifier</returns>
        public Identifier_Info Add_Identifier(string Identifier, string Type, string DisplayLabel)
        {
            if (identifiers == null)
                identifiers = new List<Identifier_Info>();

            Identifier_Info newIdentifier = new Identifier_Info(Identifier, Type, DisplayLabel);
            if (!identifiers.Contains(newIdentifier))
            {
                identifiers.Add(newIdentifier);
                return newIdentifier;
            }
            else
            {
                Identifier_Info returnIdentifier = identifiers.Find(newIdentifier.Equals);
                if (Type.Length > 0)
                    returnIdentifier.Type = Type;
                if (DisplayLabel.Length > 0)
                    returnIdentifier.Display_Label = DisplayLabel;
                return returnIdentifier;
            }
        }

        /// <summary> Remove an existing identifier from this digital resource </summary>
        /// <param name="Identifier"> Identifier to remove </param>
        public void Remove_Identifier(Identifier_Info Identifier)
        {
            if (identifiers == null)
                return;
            else
                identifiers.Remove(Identifier);
        }

        #endregion

        #region Methods to add new target audiences to this resource

        /// <summary> Clears all the targeted audiences which were related to this item </summary>
        public void Clear_Target_Audiences()
        {
            if (targetAudiences != null)
                targetAudiences.Clear();
        }

        /// <summary> Removes an existing target audience  </summary>
        /// <param name="Audience"> Target audience to remove </param>
        public void Remove_Target_Audience(TargetAudience_Info Audience)
        {
            if ((targetAudiences != null) && (targetAudiences.Contains(Audience)))
            {
                targetAudiences.Remove(Audience);
            }
        }

        /// <summary> Adds a new targeted audience to this item </summary>
        /// <param name="Audience"> Built target audience object </param>
        public void Add_Target_Audience(TargetAudience_Info Audience)
        {
            if (targetAudiences == null)
                targetAudiences = new List<TargetAudience_Info>();

            if (!targetAudiences.Contains(Audience))
            {
                targetAudiences.Add(Audience);
            }
        }

        /// <summary> Adds a new targeted audience to this item </summary>
        /// <param name="Audience">Description of targeted audience </param>
        /// <returns>Newly built and added target audience object</returns>
        public TargetAudience_Info Add_Target_Audience(string Audience)
        {
            if (targetAudiences == null)
                targetAudiences = new List<TargetAudience_Info>();

            if (Audience.Trim().Length == 0)
                return null;

            TargetAudience_Info newTarget = new TargetAudience_Info(Audience);
            if (!targetAudiences.Contains(newTarget))
            {
                targetAudiences.Add(newTarget);
                return newTarget;
            }
            else
            {
                return targetAudiences.Find(newTarget.Equals);
            }
        }

        /// <summary> Inserts a new targeted audience to this item at the provided index </summary>
        /// <param name="Index"> Index where this target audience should be inserted </param>
        /// <param name="Audience">Description of targeted audience </param>
        /// <param name="Authority">Authority from which this audience came from</param>
        /// <returns>Newly built and added target audience object</returns>
        public TargetAudience_Info Insert_Target_Audience(int Index, string Audience, string Authority)
        {
            if (targetAudiences == null)
                targetAudiences = new List<TargetAudience_Info>();

            TargetAudience_Info newTarget = new TargetAudience_Info(Audience, Authority);
            if (!targetAudiences.Contains(newTarget))
            {
                targetAudiences.Remove(newTarget);
            }

            targetAudiences.Insert(0, newTarget);
            return newTarget;
        }

        /// <summary> Adds a new targeted audience to this item </summary>
        /// <param name="Audience">Description of targeted audience </param>
        /// <param name="Authority">Authority from which this audience came from</param>
        /// <returns>Newly built and added target audience object</returns>
        public TargetAudience_Info Add_Target_Audience(string Audience, string Authority)
        {
            if (targetAudiences == null)
                targetAudiences = new List<TargetAudience_Info>();

            if (Audience.Trim().Length == 0)
                return null;

            TargetAudience_Info newTarget = new TargetAudience_Info(Audience, Authority);
            if (!targetAudiences.Contains(newTarget))
            {
                targetAudiences.Add(newTarget);
                return newTarget;
            }
            else
            {
                TargetAudience_Info returnAudience = targetAudiences.Find(newTarget.Equals);
                if (Authority.Length > 0)
                {
                    returnAudience.Authority = Authority;
                }
                return returnAudience;
            }
        }

        #endregion

        #region Methods to add new subjects to this resource

        /// <summary> Clears all the subjects which were related to this item </summary>
        public void Clear_Subjects()
        {
            if (subjects != null)
                subjects.Clear();
        }

        /// <summary> Removes a subject from the list of subjects related to this item </summary>
        /// <param name="Subject"> Subject to remove </param>
        public void Remove_Subject(Subject_Info Subject)
        {
            if ((subjects != null) && (subjects.Contains(Subject)))
                subjects.Remove(Subject);
        }

        /// <summary> Add a new empty cartographics subject and return it  </summary>
        /// <returns>Newly built cartographics subject</returns>
        public Subject_Info_Cartographics Add_Cartographics_Subject()
        {
            if (subjects == null)
                subjects = new List<Subject_Info>();

            Subject_Info_Cartographics returnValue = new Subject_Info_Cartographics();
            subjects.Add(returnValue);
            return returnValue;
        }

        /// <summary> Add scale information to this resource </summary>
        /// <param name="Scale">Scale text</param>
        /// <param name="ID">ID for the new cartographic subject (used for MARC mappings)</param>
        /// <returns>Newly built cartographic subject</returns>
        public Subject_Info_Cartographics Add_Scale(string Scale, string ID)
        {
            if (subjects == null)
                subjects = new List<Subject_Info>();

            foreach (Subject_Info thisSubject in subjects)
            {
                if (thisSubject.Class_Type == Subject_Info_Type.Cartographics)
                {
                    Subject_Info_Cartographics cartoSubj = (Subject_Info_Cartographics) thisSubject;
                    if (cartoSubj.Scale == Scale)
                    {
                        if ((ID.Length > 0) && (cartoSubj.ID.Length == 0))
                        {
                            cartoSubj.ID = ID;
                        }
                        return cartoSubj;
                    }
                }
            }

            // Add a new subject then
            Subject_Info_Cartographics newCarto = new Subject_Info_Cartographics();
            newCarto.ID = ID;
            newCarto.Scale = Scale;
            subjects.Add(newCarto);
            return newCarto;
        }

        /// <summary> Add scale information to this resource </summary>
        /// <param name="Scale">Scale text</param>
        /// <param name="Projection">Statement of Projection</param>
        /// <param name="Coordinates">Statement of Coordinates</param>
        /// <param name="ID">ID for the new cartographic subject (used for MARC mappings)</param>
        /// <returns>Newly built cartographic subject</returns>
        public Subject_Info_Cartographics Add_Scale(string Scale, string Projection, string Coordinates, string ID)
        {
            if (subjects == null)
                subjects = new List<Subject_Info>();

            foreach (Subject_Info thisSubject in subjects)
            {
                if (thisSubject.Class_Type == Subject_Info_Type.Cartographics)
                {
                    Subject_Info_Cartographics cartoSubj = (Subject_Info_Cartographics) thisSubject;
                    if (cartoSubj.Scale == Scale)
                    {
                        if ((ID.Length > 0) && (cartoSubj.ID.Length == 0))
                        {
                            cartoSubj.ID = ID;
                        }
                        cartoSubj.Projection = Projection;
                        cartoSubj.Coordinates = Coordinates;
                        return cartoSubj;
                    }
                }
            }

            // Add a new subject then
            Subject_Info_Cartographics newCarto = new Subject_Info_Cartographics();
            newCarto.ID = ID;
            newCarto.Scale = Scale;
            newCarto.Projection = Projection;
            newCarto.Coordinates = Coordinates;
            subjects.Add(newCarto);
            return newCarto;
        }

        /// <summary> Add a new empty hierarchical geographic subject and return it  </summary>
        /// <returns>Newly built hierarchical geographic subject</returns>
        public Subject_Info_HierarchicalGeographic Add_Hierarchical_Geographic_Subject()
        {
            if (subjects == null)
                subjects = new List<Subject_Info>();

            Subject_Info_HierarchicalGeographic returnValue = new Subject_Info_HierarchicalGeographic();
            subjects.Add(returnValue);
            return returnValue;
        }

        /// <summary> Add a new empty name subject and return it  </summary>
        /// <returns>Newly built name subject</returns>
        public Subject_Info_Name Add_Name_Subject()
        {
            if (subjects == null)
                subjects = new List<Subject_Info>();

            Subject_Info_Name returnValue = new Subject_Info_Name();
            subjects.Add(returnValue);
            return returnValue;
        }

        /// <summary> Add a new empty subject and return it  </summary>
        /// <returns>Newly built subject</returns>
        public Subject_Info_Standard Add_Subject()
        {
            if (subjects == null)
                subjects = new List<Subject_Info>();

            Subject_Info_Standard returnValue = new Subject_Info_Standard();
            subjects.Add(returnValue);
            return returnValue;
        }

        /// <summary> Add an existing subject to this digital resource </summary>
        /// <param name="Subject"> Subject to add to this resource </param>
        public void Add_Subject(Subject_Info Subject)
        {
            if (subjects == null)
                subjects = new List<Subject_Info>();

            subjects.Add(Subject);
        }

        /// <summary> Add a new subject keyword and return it  </summary>
        /// <param name="Topic">Topical subject</param>
        /// <param name="Authority">Authority for this topic subject keyword</param>
        /// <returns>Newly built subject</returns>
        public Subject_Info_Standard Add_Subject(string Topic, string Authority)
        {
            if (subjects == null)
                subjects = new List<Subject_Info>();

            if (Topic.Trim().Length > 0)
            {
                Subject_Info_Standard returnValue = new Subject_Info_Standard(Topic, Authority);
                subjects.Add(returnValue);
                return returnValue;
            }
            else
            {
                return null;
            }
        }

        /// <summary> Add a new empty title info subject and return it  </summary>
        /// <returns>Newly built title info subject</returns>
        public Subject_Info_TitleInfo Add_Title_Subject()
        {
            if (subjects == null)
                subjects = new List<Subject_Info>();

            Subject_Info_TitleInfo returnValue = new Subject_Info_TitleInfo();
            subjects.Add(returnValue);
            return returnValue;
        }

        #endregion

        #region Methods to add languages to this resource

        /// <summary> Clears all the languages which were related to this item </summary>
        public void Clear_Languages()
        {
            if (languages != null)
                languages.Clear();
        }

        /// <summary> Removes an existing language object from this item </summary>
        /// <param name="Language"> Language to remove </param>
        public void Remove_Language(Language_Info Language)
        {
            if ((languages != null) && (languages.Contains(Language)))
                languages.Remove(Language);
        }

        /// <summary> Indicate another language associated with this material </summary>
        /// <param name="Language_Text">Language term for this language</param>
        public Language_Info Add_Language(string Language_Text)
        {
            return Add_Language(Language_Text, String.Empty, String.Empty);
        }

        /// <summary> Indicate another language associated with this material </summary>
        /// <param name="Language_Text">Language term for this language</param>
        /// <param name="Language_ISO_Code">Iso639-2b code for this language</param>
        /// <param name="Language_RFC_Code">Rfc3066 code for this language</param>       
        public Language_Info Add_Language(string Language_Text, string Language_ISO_Code, string Language_RFC_Code)
        {
            if (languages == null)
                languages = new List<Language_Info>();

            Language_Info newLanguage = new Language_Info(Language_Text, Language_ISO_Code, Language_RFC_Code);
            if (!languages.Contains(newLanguage))
            {
                languages.Add(newLanguage);
                return newLanguage;
            }
            else
            {
                Language_Info returnLanguage = languages.Find(newLanguage.Equals);
                if (newLanguage.Language_Text.Length > 0)
                    returnLanguage.Language_Text = newLanguage.Language_Text;
                if (newLanguage.Language_ISO_Code.Length > 0)
                    returnLanguage.Language_ISO_Code = newLanguage.Language_ISO_Code;
                if (newLanguage.Language_RFC_Code.Length > 0)
                    returnLanguage.Language_RFC_Code = newLanguage.Language_RFC_Code;
                return returnLanguage;
            }
        }

        #endregion

        #region Method to add other titles to this resource 

        /// <summary> Clears all the other titles which were related to this item </summary>
        public void Clear_Other_Titles()
        {
            if (otherTitles != null)
                otherTitles.Clear();
        }

        /// <summary> Removes an existing other title from the other title collection </summary>
        /// <param name="Title"> Title to remove </param>
        public void Remove_Other_Title(Title_Info Title)
        {
            if ((otherTitles != null) && (otherTitles.Contains(Title)))
                otherTitles.Remove(Title);
        }

        /// <summary> Adds a new title to this item  </summary>
        /// <param name="Title">String for this title</param>
        /// <param name="Type">Type of title</param>
        /// <returns>Built title object for any other additiona</returns>
        public Title_Info Add_Other_Title(string Title, Title_Type_Enum Type)
        {
            if (otherTitles == null)
                otherTitles = new List<Title_Info>();

            // Make sure this doesn't already exist as the main title
            if (String.Compare(mainTitle.Title, Title, StringComparison.OrdinalIgnoreCase) == 0)
                return mainTitle;

            // Make sure this doesn't already exist as an other title
            foreach (Title_Info thisOtherTitle in otherTitles)
            {
                if (String.Compare(thisOtherTitle.Title, Title, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    thisOtherTitle.Title_Type = Type;
                    return thisOtherTitle;
                }
            }

            Title_Info newTitle = new Title_Info(Title, Type);
            otherTitles.Add(newTitle);
            return newTitle;
        }

        /// <summary> Adds a new title to this item  </summary>
        /// <param name="NewTitle">New title to add</param>
        public void Add_Other_Title(Title_Info NewTitle)
        {
            if (otherTitles == null)
                otherTitles = new List<Title_Info>();

            otherTitles.Add(NewTitle);
        }

        #endregion

        /// <summary> Clear all the items which are associated with this resource as related items </summary>
        public void Clear_Related_Items()
        {
            if (relatedItems != null)
                relatedItems.Clear();
        }

        /// <summary> Adds a new related item to this digital resource </summary>
        /// <param name="Related_Item"> Related item to remove </param>
        public void Add_Related_Item(Related_Item_Info Related_Item)
        {
            if (relatedItems == null)
                relatedItems = new List<Related_Item_Info>();

            relatedItems.Add(Related_Item);
        }

        /// <summary> Removes a related item from this digital resource </summary>
        /// <param name="Related_Item"> Related item to remove </param>
        public void Remove_Related_Item(Related_Item_Info Related_Item)
        {
            if (relatedItems != null)
                relatedItems.Remove(Related_Item);
        }

        #endregion

        #region Method for returning all of the MODS information as a single citation string (for indexing)

        /// <summary> Get the citation string, which is all the data fields lumped together
        /// for easy searching. </summary>
        internal string MODS_Citation_String
        {
            get
            {
                StringBuilder full_citation = new StringBuilder();

                if (locationInfo != null)
                {
                    full_citation.Append(locationInfo.Holding_Code + " | ");
                    full_citation.Append(locationInfo.Holding_Name + " | ");
                }

                full_citation.Append(ModsOriginInfo.Date_Issued + " | ");
                if ((originalPhysicalDesc != null) && (originalPhysicalDesc.Notes_Count > 0))
                {
                    foreach (string description in originalPhysicalDesc.Notes)
                    {
                        full_citation.Append(description + " | ");
                    }
                }
                if (notes != null)
                {
                    foreach (Note_Info thisNote in notes)
                    {
                        full_citation.Append(thisNote.Note + " | ");
                    }
                }

                if (( originalPhysicalDesc != null ) && ( !String.IsNullOrEmpty(originalPhysicalDesc.Extent)))
                    full_citation.Append(originalPhysicalDesc.Extent + " | ");
                full_citation.Append(type.MODS_Type_String + " | ");

                if (languages != null)
                {
                    foreach (Language_Info thisLanguage in languages)
                    {
                        full_citation.Append(thisLanguage.Language_Text + " | ");
                    }
                }

                if (ModsOriginInfo.Publishers_Count > 0)
                {
                    foreach (string publisher in ModsOriginInfo.Publishers)
                    {
                        full_citation.Append(publisher + " | ");
                    }
                }

                if (ModsOriginInfo.Places_Count > 0)
                {
                    foreach (Origin_Info_Place pubplace in ModsOriginInfo.Places)
                    {
                        full_citation.Append(pubplace.Place_Text + " | ");
                    }
                }

                if (classifications != null)
                {
                    foreach (Classification_Info thisClassification in classifications)
                    {
                        if (thisClassification.Authority.Length > 0)
                        {
                            full_citation.Append(thisClassification.Classification + " ( " + thisClassification.Authority + " ) | ");
                        }
                        else
                        {
                            full_citation.Append(thisClassification.Classification + " | ");
                        }
                    }
                }

                if (identifiers != null)
                {
                    foreach (Identifier_Info thisIdentifier in identifiers)
                    {
                        full_citation.Append(thisIdentifier.Identifier + " | ");
                    }
                }

                if (genres != null)
                {
                    foreach (Genre_Info thisGenre in genres)
                    {
                        full_citation.Append(thisGenre.Genre_Term + " | ");
                    }
                }

                if (subjects != null)
                {
                    foreach (Subject_Info thisSubject in subjects)
                    {
                        full_citation.Append(thisSubject + " | ");
                    }
                }

                full_citation.Append(accessCondition.Text + " | ");

                if (mainTitle.Title.Length > 0)
                {
                    if (mainTitle.Subtitle.Length > 0)
                    {
                        full_citation.Append(mainTitle.Title + " | " + mainTitle.Subtitle + " | ");
                    }
                    else
                    {
                        full_citation.Append(mainTitle.Title + " | ");
                    }
                }

                if (seriesTitle != null)
                {
                    if (seriesTitle.Title.Length > 0)
                    {
                        if (seriesTitle.Subtitle.Length > 0)
                        {
                            full_citation.Append(seriesTitle.Title + " | " + seriesTitle.Subtitle + " | ");
                        }
                        else
                        {
                            full_citation.Append(seriesTitle.Title + " | ");
                        }
                    }
                }

                if (otherTitles != null)
                {
                    foreach (Title_Info otherTitle in otherTitles)
                    {
                        if (otherTitle.Title.Length > 0)
                        {
                            if (otherTitle.Subtitle.Length > 0)
                            {
                                full_citation.Append(otherTitle.Title + " | " + otherTitle.Subtitle + " | ");
                            }
                            else
                            {
                                full_citation.Append(otherTitle.Title + " | ");
                            }
                        }
                    }
                }

                if (donor != null)
                {
                    full_citation.Append(donor.Full_Name + " | ");
                }

                if (main_entity_name != null)
                {
                    if (main_entity_name.Full_Name.Length > 0)
                    {
                        full_citation.Append(main_entity_name.Full_Name + " | ");
                    }
                }

                if (names != null)
                {
                    foreach (Name_Info thisNameInfo in names)
                    {
                        full_citation.Append(thisNameInfo.Full_Name + " | ");
                    }
                }

                // Return the built information
                return full_citation.ToString();
            }
        }

        #endregion

        #region  MODS writing sections

        /// <summary> Appends this bibliographic description information as MODS to the StringBuilder object </summary>
        /// <param name="Results">StringBuilder to add this XML to </param>
        /// <param name="VRACoreInfo"> VRA core information to include here </param>
        internal virtual void Add_MODS(TextWriter Results, VRACore_Info VRACoreInfo)
        {
            // Start the MODS section
            Results.Write("<mods:mods>\r\n");

            // Add the abstracts
            if (abstracts != null)
            {
                foreach (Abstract_Info thisAbstract in abstracts)
                {
                    thisAbstract.Add_MODS(Results);
                }
            }

            // Add the classifications
            if (classifications != null)
            {
                foreach (Classification_Info thisClassification in classifications)
                {
                    thisClassification.Add_MODS(Results);
                }
            }

            // Add the rights
            accessCondition.Add_MODS(Results);

            // Add the genres
            if (genres != null)
            {
                foreach (Genre_Info thisGenre in genres)
                {
                    thisGenre.Add_MODS(Results);
                }
            }

            // Add the identifiers
            if (identifiers != null)
            {
                foreach (Identifier_Info thisIdentifier in identifiers)
                {
                    thisIdentifier.Add_MODS(Results);
                }
            }


            // Add the languages
            if (languages != null)
            {
                foreach (Language_Info thisLanguage in languages)
                {
                    thisLanguage.Add_MODS(Results);
                }
            }

            // Write the location
            if (locationInfo != null)
            {
                locationInfo.Add_MODS(Results);
            }

            write_all_names(Results);

            // Add the notes
            if (notes != null)
            {
                foreach (Note_Info thisNote in notes)
                {
                    thisNote.Add_MODS(Results);
                }
            }

            ModsOriginInfo.Add_MODS(Results);

            //// Add the physical description of this resource
            //if (physicalDesc.hasData)
            //{
            //    physicalDesc.Add_MODS(results);
            //}

            // Write the record information
            if (recordInfo.hasData)
            {
                recordInfo.Add_MODS(Results);
            }

            // Write description of the related, original resource
            if (originalPhysicalDesc.hasData)
            {
                Results.Write("<mods:relatedItem type=\"original\">\r\n");
                originalPhysicalDesc.Add_MODS(Results);
                Results.Write("</mods:relatedItem>\r\n");
            }

            // Write the volume and issue and edition, if they exist
            if (((seriesPartInfo != null) && (seriesPartInfo.hasData)) || ((seriesTitle != null) && (seriesTitle.Title.Length > 0)))
            {
                Results.Write("<mods:relatedItem type=\"series\">\r\n");

                // Write the series title
                if ((seriesTitle != null) && (seriesTitle.Title.Length > 0))
                {
                    seriesTitle.Add_MODS(Results);
                }

                if ((seriesPartInfo != null) && (seriesPartInfo.hasData))
                {
                    seriesPartInfo.Add_MODS(Results);
                }
                Results.Write("</mods:relatedItem>\r\n");
            }

            // Add any additional related items
            if (relatedItems != null)
            {
                foreach (Related_Item_Info relatedItem in relatedItems)
                {
                    relatedItem.Add_MODS(Results);
                }
            }

            // Add the subjects
            if (subjects != null)
            {
                foreach (Subject_Info thisSubject in subjects)
                {
                    thisSubject.Add_MODS(Results);
                }
            }

            if (!String.IsNullOrEmpty(tableOfContents))
            {
                Results.Write("<mods:tableOfContents type=\"original catalog\">" + Convert_String_To_XML_Safe(tableOfContents) + "</mods:tableOfContents>\r\n");
            }

            // Write the target audiences
            if (targetAudiences != null)
            {
                foreach (TargetAudience_Info thisAudience in targetAudiences)
                {
                    thisAudience.Add_MODS(Results);
                }
            }

            // Write the main title
            if (mainTitle.Title.Length > 0)
                mainTitle.Add_MODS(Results);

            // Write all the other titles
            if (otherTitles != null)
            {
                foreach (Title_Info otherTitle in otherTitles)
                {
                    if (otherTitle.Title.Length > 0)
                    {
                        otherTitle.Add_MODS(Results);
                    }
                }
            }

            // Write the resource type
            type.Add_MODS_MODS(Results);
            
            // End the MODS section
            Results.Write("</mods:mods>\r\n");
        }

        /// <summary> Helper method to create a simple MODS line </summary>
        /// <param name="METSTag">Tag for this XML value</param>
        /// <param name="METSValue">Inner text for this XML value</param>
        /// <returns>Built simple MODS string </returns>
        protected string toMODS(string METSTag, string METSValue)
        {
            if (!String.IsNullOrEmpty(METSValue))
            {
                return "<" + METSTag + ">" + METSValue + "</" + METSTag + ">\r\n";
            }
            
            return String.Empty;
        }

        private void write_all_names(TextWriter Results)
        {
            // Add the main entity first
            if ((main_entity_name != null) && ((main_entity_name.Full_Name.Length > 0) || (main_entity_name.Family_Name.Length > 0) || (main_entity_name.Given_Name.Length > 0)))
            {
                main_entity_name.Add_MODS(true, Results);
            }

            // Now, add all the other names
            if (names != null)
            {
                foreach (Name_Info thisName in names)
                {
                    thisName.Add_MODS(false, Results);
                }
            }

            // Was there a donor?
            if ((donor != null) && (donor.Full_Name.Length > 0) && (donor.Full_Name.ToUpper() != "UNKNOWN"))
            {
                if (donor.Roles.Count == 0)
                {
                    if ((donor.Name_Type == Name_Info_Type_Enum.Personal) || (donor.Name_Type == Name_Info_Type_Enum.UNKNOWN))
                        donor.Add_Role("donor", String.Empty);
                    else
                        donor.Add_Role("endowment", String.Empty);
                }
                donor.Add_MODS(false, Results);
            }
        }

        #endregion
    }
}