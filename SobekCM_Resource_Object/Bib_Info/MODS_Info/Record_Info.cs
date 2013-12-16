#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> Class stores information about the metadata record and source of the metadata record </summary>
    [Serializable]
    public class Record_Info : XML_Writing_Base_Type
    {
        private List<Language_Info> catalog_languages;
        private string descriptionStandard;

        private Identifier_Info main_record_identifier;
        private string marc_creation_date;
        private List<string> marc_record_content_sources;
        private string record_content_source;
        private string record_origin;

        /// <summary> Constructor for a new instance of the Record_Info class </summary>
        public Record_Info()
        {
        }

        /// <summary> Gets and sets the MARC Creation date information </summary>
        public string MARC_Creation_Date
        {
            get { return marc_creation_date ?? String.Empty; }
            set { marc_creation_date = value; }
        }

        /// <summary> Gets and sets the information about the origin of this record </summary>
        public string Record_Origin
        {
            get { return record_origin ?? String.Empty; }
            set { record_origin = value; }
        }

        /// <summary> Gets the number of catalog languages associated with this item  </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Catalog_Languages"/> property.  Even if 
        /// there are no catalog languages, the Catalog_Languages property creates a readonly collection to pass back out.</remarks>
        public int Catalog_Languages_Count
        {
            get
            {
                if (catalog_languages == null)
                    return 0;
                else
                    return catalog_languages.Count;
            }
        }

        /// <summary> Gets the collection of the languages of the cataloging </summary>
        /// <remarks> You should check the count of catalog languages first using the <see cref="Catalog_Languages_Count"/> property before using this property.
        /// Even if there are no catalog languages, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Language_Info> Catalog_Languages
        {
            get
            {
                if (catalog_languages == null)
                    return new ReadOnlyCollection<Language_Info>(new List<Language_Info>());
                else
                    return new ReadOnlyCollection<Language_Info>(catalog_languages);
            }
        }

        /// <summary> Gets the count of record content sources for this item (in MARC encoding) </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="MARC_Record_Content_Sources"/> property.  Even if 
        /// there are no MARC record content sources, the MARC_Record_Content_Sources property creates a readonly collection to pass back out.</remarks>
        public int MARC_Record_Content_Sources_Count
        {
            get
            {
                if (marc_record_content_sources == null)
                    return 0;
                else
                    return marc_record_content_sources.Count;
            }
        }

        /// <summary> Gets the list of record content sources for this item (in MARC encoding) </summary>
        /// <remarks> You should check the count of catalog languages first using the <see cref="MARC_Record_Content_Sources_Count"/> property before using this property.
        /// Even if there are no catalog sources, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> MARC_Record_Content_Sources
        {
            get
            {
                if (marc_record_content_sources == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(marc_record_content_sources);
            }
        }

        /// <summary> Main Record identifier ( which is generally the BibID + '_' + VID ) </summary>
        public Identifier_Info Main_Record_Identifier
        {
            get
            {
                if (main_record_identifier == null)
                    main_record_identifier = new Identifier_Info();

                return main_record_identifier;
            }
        }

        /// <summary> Source for this record  </summary>
        internal string Record_Content_Source
        {
            get { return record_content_source ?? String.Empty; }
            set { record_content_source = value; }
        }

        /// <summary> Description standard for this record </summary>
        public string Description_Standard
        {
            get { return descriptionStandard ?? String.Empty; }
            set { descriptionStandard = value; }
        }

        /// <summary> Flag indicates if this record information obejct contains data </summary>
        public bool hasData
        {
            get
            {
                if ((!String.IsNullOrEmpty(marc_creation_date)) || (!String.IsNullOrEmpty(record_origin)) ||
                    (!String.IsNullOrEmpty(record_content_source)) || (!String.IsNullOrEmpty(descriptionStandard)) ||
                    ((marc_record_content_sources != null) && (marc_record_content_sources.Count > 0)) ||
                    ((catalog_languages != null) && (catalog_languages.Count > 0)))
                    return true;

                if ((main_record_identifier != null) && (main_record_identifier.Identifier.Length > 0))
                    return true;

                return false;
            }
        }

        /// <summary> Clear the list of all catalog languages </summary>
        public void Clear_Catalog_Languages()
        {
            if (catalog_languages != null)
                catalog_languages.Clear();
        }

        /// <summary> Adds a new language to the list of catalog languages  </summary>
        /// <param name="New_Catalog_Language"> New catalog language to add </param>
        public void Add_Catalog_Language(Language_Info New_Catalog_Language)
        {
            if (catalog_languages == null)
                catalog_languages = new List<Language_Info>();

            catalog_languages.Add(New_Catalog_Language);
        }

        /// <summary> Clear the list of record content sources for this item (in MARC encoding) </summary>
        public void Clear_MARC_Record_Content_Sources()
        {
            if (marc_record_content_sources != null)
                marc_record_content_sources.Clear();
        }

        /// <summary> Adds a new MARC record content source to this item </summary>
        /// <param name="Record_Source"> New MARC record content source to add </param>
        public void Add_MARC_Record_Content_Sources(string Record_Source)
        {
            if (marc_record_content_sources == null)
                marc_record_content_sources = new List<string>();

            if (!marc_record_content_sources.Contains(Record_Source))
                marc_record_content_sources.Add(Record_Source);
        }

        /// <summary> Clear all of the pertinent record information </summary>
        public void Clear()
        {
            if (marc_record_content_sources != null) marc_record_content_sources.Clear();
            if (catalog_languages != null) catalog_languages.Clear();
            if (main_record_identifier != null) main_record_identifier.Clear();
        }

        /// <summary> Add the MODS information for this record information to the stream writer </summary>
        /// <param name="results"> Writer to the MODS building stream </param>
        internal void Add_MODS(TextWriter results)
        {
            results.WriteLine("<mods:recordInfo>");
            if ((main_record_identifier != null) && (main_record_identifier.Identifier.Length > 0))
            {
                if (main_record_identifier.Type.Length > 0)
                {
                    results.WriteLine("<mods:recordIdentifier source=\"" + main_record_identifier.Type + "\">" + main_record_identifier.Identifier + "</mods:recordIdentifier>");
                }
                else
                {
                    results.WriteLine("<mods:recordIdentifier>" + main_record_identifier.Identifier + "</mods:recordIdentifier>");
                }
            }

            if (!String.IsNullOrEmpty(marc_creation_date))
            {
                results.WriteLine("<mods:recordCreationDate encoding=\"marc\">" + marc_creation_date + "</mods:recordCreationDate>");
            }

            if (!String.IsNullOrEmpty(record_origin))
            {
                results.WriteLine("<mods:recordOrigin>" + base.Convert_String_To_XML_Safe(record_origin) + "</mods:recordOrigin>");
            }

            if (!String.IsNullOrEmpty(record_content_source))
            {
                results.WriteLine("<mods:recordContentSource>" + base.Convert_String_To_XML_Safe(record_content_source) + "</mods:recordContentSource>");
            }

            if (marc_record_content_sources != null)
            {
                foreach (string marcSource in marc_record_content_sources)
                {
                    results.WriteLine("<mods:recordContentSource authority=\"marcorg\">" + marcSource + "</mods:recordContentSource>");
                }
            }

            if (catalog_languages != null)
            {
                foreach (Language_Info thisLanguage in catalog_languages)
                {
                    if (thisLanguage.hasData)
                    {
                        results.Write("<mods:languageOfCataloging");
                        results.WriteLine(">");
                        if (thisLanguage.Language_Text.Length > 0)
                        {
                            results.Write("<mods:languageTerm type=\"text\">" + base.Convert_String_To_XML_Safe(thisLanguage.Language_Text) + "</mods:languageTerm>\r\n");
                        }

                        if (thisLanguage.Language_ISO_Code.Length > 0)
                        {
                            results.Write("<mods:languageTerm type=\"code\" authority=\"iso639-2b\">" + base.Convert_String_To_XML_Safe(thisLanguage.Language_ISO_Code) + "</mods:languageTerm>\r\n");
                        }

                        if (thisLanguage.Language_RFC_Code.Length > 0)
                        {
                            results.Write("<mods:languageTerm type=\"code\" authority=\"rfc3066\">" + base.Convert_String_To_XML_Safe(thisLanguage.Language_RFC_Code) + "</mods:languageTerm>\r\n");
                        }
                        results.WriteLine("</mods:languageOfCataloging>");
                    }
                }
            }

            if (!String.IsNullOrEmpty(descriptionStandard))
            {
                results.WriteLine("<mods:descriptionStandard>" + base.Convert_String_To_XML_Safe(descriptionStandard) + "</mods:descriptionStandard>");
            }

            results.WriteLine("</mods:recordInfo>");
        }
    }
}