#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.VRACore;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
    /// <summary> Metadata Object Description Standard (MODS) reader that operates against a single METS section  </summary>
    public class MODS_METS_dmdSec_ReaderWriter : XML_Writing_Base_Type, iPackage_dmdSec_ReaderWriter
    {
        #region iPackage_dmdSec_ReaderWriter Members

        /// <summary> Flag indicates if this active reader/writer will write a dmdSec </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns> TRUE if the package has data to be written, otherwise fALSE </returns>
        public bool Include_dmdSec(SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            return METS_Item.Bib_Info.hasData;
        }

        /// <summary> Writes the dmdSec for the entire package to the text writer </summary>
        /// <param name="Output_Stream">Stream to which the formatted text is written </param>
        /// <param name="METS_Item">Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_dmdSec(TextWriter Output_Stream, SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            Write_MODS(Output_Stream, METS_Item.Bib_Info);
            return true;
        }

        /// <summary> Reads the dmdSec at the current position in the XmlTextReader and associates it with the 
        /// entire package  </summary>
        /// <param name="Input_XmlReader"> Open XmlReader from which to read the metadata </param>
        /// <param name="Return_Package"> Package into which to read the metadata</param>
        /// <param name="Options"> Dictionary of any options which this METS section reader may utilize</param>
        /// <returns> TRUE if successful, otherwise FALSE</returns>
        public bool Read_dmdSec(XmlReader Input_XmlReader, SobekCM_Item Return_Package, Dictionary<string, object> Options)
        {
            Read_MODS_Info(Input_XmlReader, Return_Package.Bib_Info, Return_Package);
            return true;
        }

        /// <summary> Flag indicates if this active reader/writer needs to append schema reference information
        /// to the METS XML header by analyzing the contents of the digital resource item </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> TRUE if the schema should be attached, otherwise fALSE </returns>
        public bool Schema_Reference_Required_Package(SobekCM_Item METS_Item)
        {
            // If this reader/writer is being utilized, it will almost certainly have data to write.
            return true;
        }

        /// <summary> Returns the schema namespace (xmlns) information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema namespace info for the METS header</returns>
        public string[] Schema_Namespace(SobekCM_Item METS_Item)
        {
            // If this reader/writer is being utilized, it will almost certainly have data to write.
            return new string[] {"mods=\"http://www.loc.gov/mods/v3\""};
        }

        /// <summary> Returns the schema location information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema location for the METS header</returns>
        public string[] Schema_Location(SobekCM_Item METS_Item)
        {
            // If this reader/writer is being utilized, it will almost certainly have data to write.
            return new string[] {"    http://www.loc.gov/mods/v3\r\n    http://www.loc.gov/mods/v3/mods-3-4.xsd"};
        }

        #endregion

        #region Static method to write the MODS section

        /// <summary> Add the bibliographic information as MODS to the output stream for a given digital resource </summary>
        /// <param name="Output"> Output stream for this metadata to be written to </param>
        /// <param name="thisBibInfo"> Source digital resource bibliographic information </param>
        public static void Write_MODS(TextWriter Output, Bibliographic_Info thisBibInfo)
        {
            thisBibInfo.Add_MODS(Output, null);
        }

        #endregion

        #region Static methods to read the MODS section

        /// <summary> Reads the MODS-compliant section of XML and stores the data in the provided digital resource </summary>
        /// <param name="R"> XmlTextReader from which to read the MODS data </param>
        /// <param name="ThisBibInfo"> Digital resource object to save the data to </param>
        /// <param name="Return_Item"> Return item to have the MODS information populated </param>
        public static void Read_MODS_Info(XmlReader R, Bibliographic_Info ThisBibInfo, SobekCM_Item Return_Item)
        {
            while (R.Read())
            {
                if ((R.NodeType == XmlNodeType.EndElement) && ((R.Name == "METS:mdWrap") || (R.Name == "mdWrap") || (R.Name == "mods")))
                    return;

                if (R.NodeType == XmlNodeType.Element)
                {
                    switch (R.Name)
                    {
                        case "mods:abstract":
                        case "abstract":
                            Abstract_Info thisAbstract = new Abstract_Info();
                            if (R.MoveToAttribute("ID"))
                                thisAbstract.ID = R.Value;
                            if (R.MoveToAttribute("type"))
                                thisAbstract.Type = R.Value;
                            if (R.MoveToAttribute("displayLabel"))
                                thisAbstract.Display_Label = R.Value;
                            if (R.MoveToAttribute("lang"))
                                thisAbstract.Language = R.Value;
                            R.Read();
                            if (R.NodeType == XmlNodeType.Text)
                            {
                                thisAbstract.Abstract_Text = R.Value;
                                ThisBibInfo.Add_Abstract(thisAbstract);
                            }
                            break;

                        case "mods:accessCondition":
                        case "accessCondition":
                            if (R.MoveToAttribute("ID"))
                                ThisBibInfo.Access_Condition.ID = R.Value;
                            if (R.MoveToAttribute("type"))
                                ThisBibInfo.Access_Condition.Type = R.Value;
                            if (R.MoveToAttribute("displayLabel"))
                                ThisBibInfo.Access_Condition.Display_Label = R.Value;
                            if (R.MoveToAttribute("lang"))
                                ThisBibInfo.Access_Condition.Language = R.Value;
                            R.Read();
                            if (R.NodeType == XmlNodeType.Text)
                            {
                                ThisBibInfo.Access_Condition.Text = R.Value;
                            }
                            break;

                        case "mods:classification":
                        case "classification":
                            Classification_Info thisClassification = new Classification_Info();
                            if (R.MoveToAttribute("edition"))
                                thisClassification.Edition = R.Value;
                            if (R.MoveToAttribute("authority"))
                                thisClassification.Authority = R.Value;
                            if (R.MoveToAttribute("displayLabel"))
                                thisClassification.Display_Label = R.Value;
                            R.Read();
                            if (R.NodeType == XmlNodeType.Text)
                            {
                                thisClassification.Classification = R.Value;
                                ThisBibInfo.Add_Classification(thisClassification);
                            }
                            break;

                        case "mods:identifier":
                        case "identifier":
                            Identifier_Info thisIdentifier = new Identifier_Info();
                            if (R.MoveToAttribute("type"))
                                thisIdentifier.Type = R.Value;
                            if (R.MoveToAttribute("displayLabel"))
                                thisIdentifier.Display_Label = R.Value;
                            R.Read();
                            if (R.NodeType == XmlNodeType.Text)
                            {
                                thisIdentifier.Identifier = R.Value;
                                ThisBibInfo.Add_Identifier(thisIdentifier);
                            }
                            break;

                        case "mods:genre":
                        case "genre":
                            Genre_Info thisGenre = new Genre_Info();
                            if (R.MoveToAttribute("ID"))
                                thisGenre.ID = R.Value;
                            if (R.MoveToAttribute("authority"))
                                thisGenre.Authority = R.Value;
                            if (R.MoveToAttribute("lang"))
                                thisGenre.Language = R.Value;
                            R.Read();
                            if (R.NodeType == XmlNodeType.Text)
                            {
                                thisGenre.Genre_Term = R.Value;
                                ThisBibInfo.Add_Genre(thisGenre);
                            }
                            break;

                        case "mods:language":
                        case "language":
                            string language_text = String.Empty;
                            string language_rfc_code = String.Empty;
                            string language_iso_code = String.Empty;
                            string language_id = String.Empty;
                            if (R.MoveToAttribute("ID"))
                                language_id = R.Value;
                            while (R.Read())
                            {
                                if ((R.NodeType == XmlNodeType.Element) && ((R.Name == "mods:languageTerm") || (R.Name == "languageTerm")))
                                {
                                    if (R.MoveToAttribute("type"))
                                    {
                                        switch (R.Value)
                                        {
                                            case "code":
                                                if (R.MoveToAttribute("authority"))
                                                {
                                                    if (R.Value == "rfc3066")
                                                    {
                                                        R.Read();
                                                        if (R.NodeType == XmlNodeType.Text)
                                                        {
                                                            language_rfc_code = R.Value;
                                                        }
                                                    }
                                                    else if (R.Value == "iso639-2b")
                                                    {
                                                        R.Read();
                                                        if (R.NodeType == XmlNodeType.Text)
                                                        {
                                                            language_iso_code = R.Value;
                                                        }
                                                    }
                                                }
                                                break;

                                            case "text":
                                                R.Read();
                                                if (R.NodeType == XmlNodeType.Text)
                                                {
                                                    language_text = R.Value;
                                                }
                                                // Quick check for a change we started in 2010
                                                if (language_text == "governmental publication")
                                                    language_text = "government publication";
                                                break;

                                            default:
                                                R.Read();
                                                if (R.NodeType == XmlNodeType.Text)
                                                {
                                                    language_text = R.Value;
                                                }
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        R.Read();
                                        if (R.NodeType == XmlNodeType.Text)
                                        {
                                            language_text = R.Value;
                                        }
                                    }
                                }

                                if ((R.NodeType == XmlNodeType.EndElement) && ((R.Name == "mods:language") || (R.Name == "language")))
                                {
                                    break;
                                }
                            }

                            if ((language_text.Length > 0) || (language_rfc_code.Length > 0) || (language_iso_code.Length > 0))
                            {
                                ThisBibInfo.Add_Language(language_text, language_iso_code, language_rfc_code);
                            }
                            break;

                        case "mods:location":
                        case "location":
                            while (R.Read())
                            {
                                if ((R.NodeType == XmlNodeType.EndElement) && ((R.Name == "mods:location") || (R.Name == "location")))
                                    break;

                                if (R.NodeType == XmlNodeType.Element)
                                {
                                    if ((R.Name == "mods:physicalLocation") || (R.Name == "physicalLocation"))
                                    {
                                        if (R.MoveToAttribute("type"))
                                        {
                                            if (R.Value == "code")
                                            {
                                                R.Read();
                                                if (R.NodeType == XmlNodeType.Text)
                                                {
                                                    ThisBibInfo.Location.Holding_Code = R.Value;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            R.Read();
                                            if (R.NodeType == XmlNodeType.Text)
                                            {
                                                ThisBibInfo.Location.Holding_Name = R.Value;
                                            }
                                        }
                                    }
                                    if ((R.Name == "mods:url") || (R.Name == "url"))
                                    {
                                        // TEST
                                        if (R.MoveToAttribute("access"))
                                        {
                                            if (R.Value == "object in context")
                                            {
                                                R.Read();
                                                if (R.NodeType == XmlNodeType.Text)
                                                {
                                                    ThisBibInfo.Location.PURL = R.Value;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            string url_displayLabel = R.GetAttribute("displayLabel");
                                            string url_note = R.GetAttribute("note");
                                            R.Read();
                                            if (R.NodeType == XmlNodeType.Text)
                                            {
                                                if ((url_displayLabel != null) && (url_displayLabel == "Finding Guide"))
                                                {
                                                    if (url_note != null)
                                                    {
                                                        ThisBibInfo.Location.EAD_Name = url_note;
                                                    }
                                                    ThisBibInfo.Location.EAD_URL = R.Value;
                                                }
                                                else
                                                {
                                                    if (url_displayLabel != null)
                                                    {
                                                        ThisBibInfo.Location.Other_URL_Display_Label = url_displayLabel;
                                                    }

                                                    if (url_note != null)
                                                    {
                                                        ThisBibInfo.Location.Other_URL_Note = url_note;
                                                    }
                                                    ThisBibInfo.Location.Other_URL = R.Value;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;

                        case "mods:name":
                        case "name":
                            Name_Info tempNewName = read_name_object(R);
                            if (tempNewName.Main_Entity)
                                ThisBibInfo.Main_Entity_Name = tempNewName;
                            else
                            {
                                bool donor = false;
                                foreach (Name_Info_Role role in tempNewName.Roles)
                                {
                                    if ((role.Role == "donor") || (role.Role == "honoree") || (role.Role == "endowment") || (role.Role == "collection"))
                                    {
                                        donor = true;
                                        break;
                                    }
                                }
                                if (donor)
                                {
                                    ThisBibInfo.Donor = tempNewName;
                                }
                                else
                                {
                                    ThisBibInfo.Add_Named_Entity(tempNewName);
                                }
                            }
                            break;

                        case "mods:note":
                        case "note":
                            Note_Info newNote = new Note_Info();
                            if (R.MoveToAttribute("ID"))
                                newNote.ID = R.Value;
                            if (R.MoveToAttribute("type"))
                                newNote.Note_Type_String = R.Value;
                            if (R.MoveToAttribute("displayLabel"))
                                newNote.Display_Label = R.Value;
                            R.Read();
                            if (R.NodeType == XmlNodeType.Text)
                            {
                                newNote.Note = R.Value;
                                ThisBibInfo.Add_Note(newNote);
                            }
                            break;

                        case "mods:Origin_Info":
                        case "Origin_Info":
                        case "mods:originInfo":
                        case "originInfo":
                            while (R.Read())
                            {
                                if ((R.NodeType == XmlNodeType.EndElement) && ((R.Name == "mods:Origin_Info") || (R.Name == "Origin_Info") || (R.Name == "mods:originInfo") || (R.Name == "originInfo")))
                                    break;

                                if (R.NodeType == XmlNodeType.Element)
                                {
                                    switch (R.Name)
                                    {
                                        case "mods:publisher":
                                        case "publisher":
                                            R.Read();
                                            if (R.NodeType == XmlNodeType.Text)
                                            {
                                                ThisBibInfo.Origin_Info.Add_Publisher(R.Value);
                                                ThisBibInfo.Add_Publisher(R.Value);
                                            }
                                            break;

                                        case "mods:place":
                                        case "place":
                                            string place_text = String.Empty;
                                            string place_marc = String.Empty;
                                            string place_iso = String.Empty;
                                            while ((R.Read()) && (!(((R.Name == "mods:place") || (R.Name == "place")) && (R.NodeType == XmlNodeType.EndElement))))
                                            {
                                                if ((R.NodeType == XmlNodeType.Element) && ((R.Name == "mods:placeTerm") || (R.Name == "placeTerm")))
                                                {
                                                    if ((R.MoveToAttribute("type")) && (R.Value == "code"))
                                                    {
                                                        if (R.MoveToAttribute("authority"))
                                                        {
                                                            switch (R.Value)
                                                            {
                                                                case "marccountry":
                                                                    R.Read();
                                                                    if (R.NodeType == XmlNodeType.Text)
                                                                    {
                                                                        place_marc = R.Value;
                                                                    }
                                                                    break;

                                                                case "iso3166":
                                                                    R.Read();
                                                                    if (R.NodeType == XmlNodeType.Text)
                                                                    {
                                                                        place_iso = R.Value;
                                                                    }
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        R.Read();
                                                        if (R.NodeType == XmlNodeType.Text)
                                                        {
                                                            place_text = R.Value;
                                                        }
                                                    }
                                                }
                                            }
                                            if ((place_text.Length > 0) || (place_marc.Length > 0) || (place_iso.Length > 0))
                                            {
                                                ThisBibInfo.Origin_Info.Add_Place(place_text, place_marc, place_iso);
                                            }
                                            break;

                                        case "mods:dateIssued":
                                        case "dateIssued":
                                            if ((R.MoveToAttribute("encoding")) && (R.Value == "marc"))
                                            {
                                                if (R.MoveToAttribute("point"))
                                                {
                                                    if (R.Value == "start")
                                                    {
                                                        R.Read();
                                                        if (R.NodeType == XmlNodeType.Text)
                                                        {
                                                            ThisBibInfo.Origin_Info.MARC_DateIssued_Start = R.Value;
                                                        }
                                                    }
                                                    else if (R.Value == "end")
                                                    {
                                                        R.Read();
                                                        if (R.NodeType == XmlNodeType.Text)
                                                        {
                                                            ThisBibInfo.Origin_Info.MARC_DateIssued_End = R.Value;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    R.Read();
                                                    if (R.NodeType == XmlNodeType.Text)
                                                    {
                                                        ThisBibInfo.Origin_Info.MARC_DateIssued = R.Value;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                R.Read();
                                                if (R.NodeType == XmlNodeType.Text)
                                                {
                                                    ThisBibInfo.Origin_Info.Date_Issued = R.Value;
                                                }
                                            }
                                            break;

                                        case "mods:dateCreated":
                                        case "dateCreated":
                                            R.Read();
                                            if (R.NodeType == XmlNodeType.Text)
                                            {
                                                ThisBibInfo.Origin_Info.Date_Created = R.Value;
                                            }
                                            break;

                                        case "mods:copyrightDate":
                                        case "copyrightDate":
                                            R.Read();
                                            if (R.NodeType == XmlNodeType.Text)
                                            {
                                                ThisBibInfo.Origin_Info.Date_Copyrighted = R.Value;
                                            }
                                            break;

                                        case "mods:dateOther":
                                        case "dateOther":
                                            if ((R.MoveToAttribute("type")) && (R.Value == "reprint"))
                                            {
                                                R.Read();
                                                if (R.NodeType == XmlNodeType.Text)
                                                {
                                                    ThisBibInfo.Origin_Info.Date_Reprinted = R.Value;
                                                }
                                            }
                                            break;

                                        case "mods:edition":
                                        case "edition":
                                            R.Read();
                                            if (R.NodeType == XmlNodeType.Text)
                                            {
                                                ThisBibInfo.Origin_Info.Edition = R.Value;
                                            }
                                            break;

                                        case "mods:frequency":
                                        case "frequency":
                                            string freq_authority = String.Empty;
                                            if (R.MoveToAttribute("authority"))
                                                freq_authority = R.Value;
                                            R.Read();
                                            if (R.NodeType == XmlNodeType.Text)
                                            {
                                                ThisBibInfo.Origin_Info.Add_Frequency(R.Value, freq_authority);
                                            }
                                            break;

                                        case "mods:issuance":
                                        case "issuance":
                                            R.Read();
                                            if (R.NodeType == XmlNodeType.Text)
                                            {
                                                ThisBibInfo.Origin_Info.Add_Issuance(R.Value);
                                            }
                                            break;
                                    }
                                }
                            }

                            break;

                        case "mods:physicalDescription":
                        case "physicalDescription":
                            read_physical_description(R, ThisBibInfo.Original_Description);
                            break;

                        case "mods:recordInfo":
                        case "recordInfo":
                            while (R.Read())
                            {
                                if ((R.NodeType == XmlNodeType.EndElement) && ((R.Name == "mods:recordInfo") || (R.Name == "recordInfo")))
                                {
                                    break;
                                }

                                if (R.NodeType == XmlNodeType.Element)
                                {
                                    switch (R.Name)
                                    {
                                        case "mods:recordCreationDate":
                                        case "recordCreationDate":
                                            if ((R.MoveToAttribute("encoding")) && (R.Value == "marc"))
                                            {
                                                R.Read();
                                                ThisBibInfo.Record.MARC_Creation_Date = R.Value;
                                            }
                                            break;

                                        case "mods:recordIdentifier":
                                        case "recordIdentifier":
                                            string source = String.Empty;
                                            if (R.MoveToAttribute("source"))
                                            {
                                                ThisBibInfo.Record.Main_Record_Identifier.Type = R.Value;
                                            }
                                            R.Read();
                                            ThisBibInfo.Record.Main_Record_Identifier.Identifier = R.Value;
                                            break;

                                        case "mods:recordOrigin":
                                        case "recordOrigin":
                                            R.Read();
                                            ThisBibInfo.Record.Record_Origin = R.Value;
                                            break;

                                        case "mods:descriptionStandard":
                                        case "descriptionStandard":
                                            R.Read();
                                            ThisBibInfo.Record.Description_Standard = R.Value;
                                            break;

                                        case "mods:recordContentSource":
                                        case "recordContentSource":
                                            if (R.MoveToAttribute("authority"))
                                            {
                                                if (R.Value == "marcorg")
                                                {
                                                    R.Read();
                                                    ThisBibInfo.Record.Add_MARC_Record_Content_Sources(R.Value);
                                                }
                                            }
                                            else
                                            {
                                                R.Read();
                                                ThisBibInfo.Source.Statement = R.Value;
                                            }
                                            break;

                                        case "mods:languageOfCataloging":
                                        case "languageOfCataloging":
                                            string cat_language_text = String.Empty;
                                            string cat_language_rfc_code = String.Empty;
                                            string cat_language_iso_code = String.Empty;
                                            string cat_language_id = String.Empty;
                                            while (R.Read())
                                            {
                                                if ((R.NodeType == XmlNodeType.Element) && ((R.Name == "mods:languageTerm") || (R.Name == "languageTerm")))
                                                {
                                                    if (R.MoveToAttribute("ID"))
                                                        cat_language_id = R.Value;

                                                    if (R.MoveToAttribute("type"))
                                                    {
                                                        switch (R.Value)
                                                        {
                                                            case "code":
                                                                if (R.MoveToAttribute("authority"))
                                                                {
                                                                    if (R.Value == "rfc3066")
                                                                    {
                                                                        R.Read();
                                                                        if (R.NodeType == XmlNodeType.Text)
                                                                        {
                                                                            cat_language_rfc_code = R.Value;
                                                                        }
                                                                    }
                                                                    else if (R.Value == "iso639-2b")
                                                                    {
                                                                        R.Read();
                                                                        if (R.NodeType == XmlNodeType.Text)
                                                                        {
                                                                            cat_language_iso_code = R.Value;
                                                                        }
                                                                    }
                                                                }
                                                                break;

                                                            case "text":
                                                                R.Read();
                                                                if (R.NodeType == XmlNodeType.Text)
                                                                {
                                                                    cat_language_text = R.Value;
                                                                }
                                                                break;

                                                            default:
                                                                R.Read();
                                                                if (R.NodeType == XmlNodeType.Text)
                                                                {
                                                                    cat_language_text = R.Value;
                                                                }
                                                                break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        R.Read();
                                                        if (R.NodeType == XmlNodeType.Text)
                                                        {
                                                            cat_language_text = R.Value;
                                                        }
                                                    }
                                                }

                                                if ((R.NodeType == XmlNodeType.EndElement) && ((R.Name == "mods:languageOfCataloging") || (R.Name == "languageOfCataloging")))
                                                {
                                                    break;
                                                }
                                            }

                                            if ((cat_language_text.Length > 0) || (cat_language_rfc_code.Length > 0) || (cat_language_iso_code.Length > 0))
                                            {
                                                Language_Info newCatLanguage = new Language_Info(cat_language_text, cat_language_iso_code, cat_language_rfc_code);
                                                ThisBibInfo.Record.Add_Catalog_Language(newCatLanguage);
                                            }
                                            break;
                                    }
                                }
                            }
                            break;


                        case "mods:relatedItem":
                        case "relatedItem":
                            string relatedItemType = String.Empty;
                            if (R.MoveToAttribute("type"))
                                relatedItemType = R.Value.ToLower();

                            switch (relatedItemType)
                            {
                                case "original":
                                    while (R.Read())
                                    {
                                        if ((R.NodeType == XmlNodeType.EndElement) && ((R.Name == "mods:relatedItem") || (R.Name == "relatedItem")))
                                            break;

                                        if (R.NodeType == XmlNodeType.Element)
                                        {
                                            if ((R.Name == "mods:physicalDescription") || (R.Name == "physicalDescription"))
                                            {
                                                read_physical_description(R, ThisBibInfo.Original_Description);
                                            }
                                        }
                                    }
                                    break;

                                case "series":
                                    string part_type = String.Empty;
                                    string part_caption = String.Empty;
                                    string part_number = String.Empty;

                                    while (R.Read())
                                    {
                                        if ((R.NodeType == XmlNodeType.EndElement) && ((R.Name == "mods:relatedItem") || (R.Name == "mods:relatedItem")))
                                        {
                                            break;
                                        }

                                        if ((R.NodeType == XmlNodeType.EndElement) && ((R.Name == "mods:detail") || (R.Name == "detail")))
                                        {
                                            try
                                            {
                                                switch (part_type)
                                                {
                                                    case "Enum1":
                                                        ThisBibInfo.Series_Part_Info.Enum1 = part_caption;
                                                        ThisBibInfo.Series_Part_Info.Enum1_Index = Convert.ToInt32(part_number);
                                                        break;

                                                    case "Enum2":
                                                        ThisBibInfo.Series_Part_Info.Enum2 = part_caption;
                                                        ThisBibInfo.Series_Part_Info.Enum2_Index = Convert.ToInt32(part_number);
                                                        break;

                                                    case "Enum3":
                                                        ThisBibInfo.Series_Part_Info.Enum3 = part_caption;
                                                        ThisBibInfo.Series_Part_Info.Enum3_Index = Convert.ToInt32(part_number);
                                                        break;

                                                    case "Enum4":
                                                        ThisBibInfo.Series_Part_Info.Enum4 = part_caption;
                                                        ThisBibInfo.Series_Part_Info.Enum4_Index = Convert.ToInt32(part_number);
                                                        break;

                                                    case "Year":
                                                        ThisBibInfo.Series_Part_Info.Year = part_caption;
                                                        ThisBibInfo.Series_Part_Info.Year_Index = Convert.ToInt32(part_number);
                                                        break;

                                                    case "Month":
                                                        ThisBibInfo.Series_Part_Info.Month = part_caption;
                                                        ThisBibInfo.Series_Part_Info.Month_Index = Convert.ToInt32(part_number);
                                                        break;

                                                    case "Day":
                                                        ThisBibInfo.Series_Part_Info.Day = part_caption;
                                                        ThisBibInfo.Series_Part_Info.Day_Index = Convert.ToInt32(part_number);
                                                        break;
                                                }
                                            }
                                            catch
                                            {
                                            }

                                            part_type = String.Empty;
                                            part_caption = String.Empty;
                                            part_number = String.Empty;
                                        }

                                        if (R.NodeType == XmlNodeType.Element)
                                        {
                                            switch (R.Name)
                                            {
                                                case "mods:titleInfo":
                                                case "titleInfo":
                                                    ThisBibInfo.SeriesTitle = read_title_object(R);
                                                    break;

                                                case "mods:detail":
                                                case "detail":
                                                    if (R.MoveToAttribute("type"))
                                                    {
                                                        part_type = R.Value;
                                                    }
                                                    break;

                                                case "mods:caption":
                                                case "caption":
                                                    R.Read();
                                                    if (R.NodeType == XmlNodeType.Text)
                                                    {
                                                        part_caption = R.Value;
                                                    }
                                                    break;

                                                case "mods:number":
                                                case "number":
                                                    R.Read();
                                                    if (R.NodeType == XmlNodeType.Text)
                                                    {
                                                        part_number = R.Value;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    break;

                                default:
                                    Related_Item_Info newRelated = new Related_Item_Info();
                                    ThisBibInfo.Add_Related_Item(newRelated);
                                    switch (relatedItemType)
                                    {
                                        case "preceding":
                                            newRelated.Relationship = Related_Item_Type_Enum.Preceding;
                                            break;

                                        case "succeeding":
                                            newRelated.Relationship = Related_Item_Type_Enum.Succeeding;
                                            break;

                                        case "otherVersion":
                                            newRelated.Relationship = Related_Item_Type_Enum.OtherVersion;
                                            break;

                                        case "otherFormat":
                                            newRelated.Relationship = Related_Item_Type_Enum.OtherFormat;
                                            break;

                                        case "host":
                                            newRelated.Relationship = Related_Item_Type_Enum.Host;
                                            break;
                                    }
                                    if (R.MoveToAttribute("ID"))
                                        newRelated.ID = R.Value;

                                    while (R.Read())
                                    {
                                        if ((R.NodeType == XmlNodeType.EndElement) && ((R.Name == "mods:relatedItem") || (R.Name == "relatedItem")))
                                        {
                                            break;
                                        }

                                        if (R.NodeType == XmlNodeType.Element)
                                        {
                                            switch (R.Name)
                                            {
                                                case "mods:titleInfo":
                                                case "titleInfo":
                                                    newRelated.Set_Main_Title(read_title_object(R));
                                                    break;

                                                case "mods:identifier":
                                                case "identifier":
                                                    Identifier_Info thisRIdentifier = new Identifier_Info();
                                                    if (R.MoveToAttribute("type"))
                                                        thisRIdentifier.Type = R.Value;
                                                    if (R.MoveToAttribute("displayLabel"))
                                                        thisRIdentifier.Display_Label = R.Value;
                                                    R.Read();
                                                    if (R.NodeType == XmlNodeType.Text)
                                                    {
                                                        thisRIdentifier.Identifier = R.Value;
                                                        newRelated.Add_Identifier(thisRIdentifier);
                                                    }
                                                    break;

                                                case "mods:name":
                                                case "name":
                                                    newRelated.Add_Name(read_name_object(R));
                                                    break;

                                                case "mods:note":
                                                case "note":
                                                    Note_Info newRNote = new Note_Info();
                                                    if (R.MoveToAttribute("ID"))
                                                        newRNote.ID = R.Value;
                                                    if (R.MoveToAttribute("type"))
                                                        newRNote.Note_Type_String = R.Value;
                                                    if (R.MoveToAttribute("displayLabel"))
                                                        newRNote.Display_Label = R.Value;
                                                    R.Read();
                                                    if (R.NodeType == XmlNodeType.Text)
                                                    {
                                                        newRNote.Note = R.Value;
                                                        newRelated.Add_Note(newRNote);
                                                    }
                                                    break;

                                                case "mods:url":
                                                case "url":
                                                    if (R.MoveToAttribute("displayLabel"))
                                                        newRelated.URL_Display_Label = R.Value;
                                                    R.Read();
                                                    if (R.NodeType == XmlNodeType.Text)
                                                    {
                                                        newRelated.URL = R.Value;
                                                    }
                                                    break;

                                                case "mods:publisher":
                                                case "publisher":
                                                    R.Read();
                                                    if (R.NodeType == XmlNodeType.Text)
                                                    {
                                                        newRelated.Publisher = R.Value;
                                                    }
                                                    break;

                                                case "mods:recordIdentifier":
                                                case "recordIdentifier":
                                                    if (R.MoveToAttribute("source"))
                                                    {
                                                        if ((R.Value == "ufdc") || (R.Value == "dloc") || (R.Value.ToLower() == "sobekcm"))
                                                        {
                                                            R.Read();
                                                            if (R.NodeType == XmlNodeType.Text)
                                                            {
                                                                newRelated.SobekCM_ID = R.Value;
                                                            }
                                                        }
                                                    }
                                                    break;

                                                case "mods:dateIssued":
                                                case "dateIssued":
                                                    if (R.MoveToAttribute("point"))
                                                    {
                                                        if (R.Value == "start")
                                                        {
                                                            R.Read();
                                                            if (R.NodeType == XmlNodeType.Text)
                                                            {
                                                                newRelated.Start_Date = R.Value;
                                                            }
                                                        }
                                                        else if (R.Value == "end")
                                                        {
                                                            R.Read();
                                                            if (R.NodeType == XmlNodeType.Text)
                                                            {
                                                                newRelated.End_Date = R.Value;
                                                            }
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;

                        case "mods:subject":
                        case "subject":
                            read_subject_object(R, ThisBibInfo);
                            break;

                        case "mods:targetAudience":
                        case "targetAudience":
                            TargetAudience_Info newTarget = new TargetAudience_Info();
                            if (R.MoveToAttribute("ID"))
                                newTarget.ID = R.Value;
                            if (R.MoveToAttribute("authority"))
                                newTarget.Authority = R.Value;
                            R.Read();
                            if (R.NodeType == XmlNodeType.Text)
                            {
                                newTarget.Audience = R.Value;
                                ThisBibInfo.Add_Target_Audience(newTarget);
                            }
                            break;

                        case "mods:tableOfContents":
                        case "tableOfContents":
                            R.Read();
                            if (R.NodeType == XmlNodeType.Text)
                            {
                                ThisBibInfo.TableOfContents = R.Value;
                            }
                            break;

                        case "mods:titleInfo":
                        case "titleInfo":
                            Title_Info thisTitle = read_title_object(R);
                            if (thisTitle.Title_Type == Title_Type_Enum.UNSPECIFIED)
                                ThisBibInfo.Main_Title = thisTitle;
                            else
                                ThisBibInfo.Add_Other_Title(thisTitle);
                            break;

                        case "mods:typeOfResource":
                        case "typeOfResource":
                            R.Read();
                            if (R.NodeType == XmlNodeType.Text)
                            {
                                ThisBibInfo.Type.Add_Uncontrolled_Type(R.Value);
                            }
                            break;

                        case "mods:extension":
                        case "extension":
                            string schema = String.Empty;
                            string alias = String.Empty;
                            if (R.HasAttributes)
                            {
                                for (int i = 0; i < R.AttributeCount; i++)
                                {
                                    R.MoveToAttribute(i);
                                    if (R.Name.IndexOf("xmlns") == 0)
                                    {
                                        alias = R.Name.Replace("xmlns:", "");
                                        schema = R.Value;
                                        break;
                                    }
                                }
                            }
                            if (schema.IndexOf("vra.xsd") > 0)
                            {
                                read_vra_core_extensions(R, ThisBibInfo, alias, Return_Item);
                            }
                            break;
                    }
                }
            }
        }

        private static void read_physical_description(XmlReader r, PhysicalDescription_Info description)
        {
            while (r.Read())
            {
                if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "mods:physicalDescription") || (r.Name == "physicalDescription")))
                    break;

                if (r.NodeType == XmlNodeType.Element)
                {
                    if ((r.Name == "mods:extent") || (r.Name == "extent"))
                    {
                        r.Read();
                        if (r.NodeType == XmlNodeType.Text)
                        {
                            description.Extent = r.Value;
                        }
                    }
                    else if ((r.Name == "mods:note") || (r.Name == "note"))
                    {
                        r.Read();
                        if (r.NodeType == XmlNodeType.Text)
                        {
                            description.Add_Note(r.Value);
                        }
                    }
                    else if ((r.Name == "mods:form") || (r.Name == "form"))
                    {
                        string type = String.Empty;
                        string authority = String.Empty;
                        if (r.MoveToAttribute("type"))
                            type = r.Value;
                        if (r.MoveToAttribute("authority"))
                            authority = r.Value;
                        r.Read();
                        if (r.NodeType == XmlNodeType.Text)
                        {
                            description.Form_Info.Form = r.Value;
                            if (type.Length > 0)
                                description.Form_Info.Type = type;
                            if (authority.Length > 0)
                                description.Form_Info.Authority = authority;
                        }
                    }
                }
            }
        }

        private static void read_vra_core_extensions(XmlReader r, Bibliographic_Info thisBibInfo, string alias, SobekCM_Item Package)
        {
            if (Package == null)
            {
                while (r.Read())
                {
                    if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "mods:extension") || (r.Name == "extension")))
                    {
                        return;
                    }
                }
                return;
            }

            VRACore_Info vraCoreInfo = Package.Get_Metadata_Module( GlobalVar.VRACORE_METADATA_MODULE_KEY ) as VRACore_Info;
            if (vraCoreInfo == null)
            {
                vraCoreInfo = new VRACore_Info();
                Package.Add_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY, vraCoreInfo);
            }

            while (r.Read())
            {
                if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "mods:extension") || (r.Name == "extension")))
                {
                    return;
                }

                if (r.NodeType == XmlNodeType.Element)
                {
                    string nodename = r.Name;
                    if (alias.Length > 0)
                    {
                        nodename = nodename.Replace(alias + ":", "");
                    }
                    switch (nodename)
                    {
                        case "culturalContext":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                if (r.Value.Length > 0)
                                    vraCoreInfo.Add_Cultural_Context(r.Value);
                            }
                            break;

                        case "inscription":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                if (r.Value.Length > 0)
                                    vraCoreInfo.Add_Inscription(r.Value);
                            }
                            break;

                        case "material":
                            string type = String.Empty;
                            if (r.HasAttributes)
                            {
                                if (r.MoveToAttribute("type"))
                                    type = r.Value;
                            }
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                if (r.Value.Length > 0)
                                    vraCoreInfo.Add_Material(r.Value, type);
                            }
                            break;

                        case "measurements":
                            string units = String.Empty;
                            if (r.HasAttributes)
                            {
                                if (r.MoveToAttribute("unit"))
                                    units = r.Value;
                            }
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                if (r.Value.Length > 0)
                                    vraCoreInfo.Add_Measurement(r.Value, units);
                            }
                            break;

                        case "stateEdition":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                if (r.Value.Length > 0)
                                    vraCoreInfo.Add_State_Edition(r.Value);
                            }
                            break;

                        case "stylePeriod":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                if (r.Value.Length > 0)
                                    vraCoreInfo.Add_Style_Period(r.Value);
                            }
                            break;

                        case "technique":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                if (r.Value.Length > 0)
                                    vraCoreInfo.Add_Technique(r.Value);
                            }
                            break;
                    }
                }
            }
        }

        private static void read_subject_object(XmlReader r, Bibliographic_Info thisBibInfo)
        {
            string language = String.Empty;
            string authority = String.Empty;
            string id = String.Empty;

            if (r.MoveToAttribute("ID"))
                id = r.Value;
            if (r.MoveToAttribute("lang"))
                language = r.Value;
            if (r.MoveToAttribute("authority"))
                authority = r.Value;

            // Move to the next element
            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                    break;
            }

            // Determine the subject type
            Subject_Info_Type type = Subject_Info_Type.UNKNOWN;

            // What is the name of this node?
            switch (r.Name)
            {
                case "mods:topic":
                case "mods:geographic":
                case "mods:genre":
                case "mods:temporal":
                case "mods:occupation":
                case "topic":
                case "geographic":
                case "genre":
                case "temporal":
                case "occupation":
                    type = Subject_Info_Type.Standard;
                    break;

                case "mods:hierarchicalGeographic":
                case "hierarchicalGeographic":
                    type = Subject_Info_Type.Hierarchical_Spatial;
                    break;

                case "mods:cartographics":
                case "cartographics":
                    type = Subject_Info_Type.Cartographics;
                    break;

                case "mods:name":
                case "name":
                    type = Subject_Info_Type.Name;
                    break;

                case "mods:titleInfo":
                case "titleInfo":
                    type = Subject_Info_Type.TitleInfo;
                    break;
            }

            // If no type was determined, return null
            if (type == Subject_Info_Type.UNKNOWN)
                return;

            // Was this the standard subject object?
            if (type == Subject_Info_Type.Standard)
            {
                Subject_Info_Standard standardSubject = new Subject_Info_Standard();
                standardSubject.Language = language;
                standardSubject.Authority = authority;
                standardSubject.ID = id;

                do
                {
                    if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "mods:subject") || (r.Name == "subject")))
                    {
                        if ((standardSubject.Topics_Count > 0) || (standardSubject.Geographics_Count > 0) || (standardSubject.Genres_Count > 0) ||
                            (standardSubject.Temporals_Count > 0) || (standardSubject.Occupations_Count > 0))
                        {
                            thisBibInfo.Add_Subject(standardSubject);
                        }
                        return;
                    }

                    if (r.NodeType == XmlNodeType.Element)
                    {
                        switch (r.Name)
                        {
                            case "mods:topic":
                            case "topic":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    standardSubject.Add_Topic(r.Value);
                                break;

                            case "mods:geographic":
                            case "geographic":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    standardSubject.Add_Geographic(r.Value);
                                break;

                            case "mods:genre":
                            case "genre":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    standardSubject.Add_Genre(r.Value);
                                break;

                            case "mods:temporal":
                            case "temporal":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    standardSubject.Add_Temporal(r.Value);
                                break;

                            case "mods:occupation":
                            case "occupation":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    standardSubject.Add_Occupation(r.Value);
                                break;
                        }
                    }
                } while (r.Read());
            }

            // Was this the hierarchical geography subject?
            if (type == Subject_Info_Type.Hierarchical_Spatial)
            {
                Subject_Info_HierarchicalGeographic geoSubject = new Subject_Info_HierarchicalGeographic();
                geoSubject.Language = language;
                geoSubject.Authority = authority;
                geoSubject.ID = id;

                while (r.Read())
                {
                    if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "mods:subject") || (r.Name == "subject")))
                    {
                        thisBibInfo.Add_Subject(geoSubject);
                        return;
                    }

                    if (r.NodeType == XmlNodeType.Element)
                    {
                        switch (r.Name)
                        {
                            case "mods:continent":
                            case "continent":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    geoSubject.Continent = r.Value;
                                break;

                            case "mods:country":
                            case "country":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    geoSubject.Country = r.Value;
                                break;

                            case "mods:province":
                            case "province":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    geoSubject.Province = r.Value;
                                break;

                            case "mods:region":
                            case "region":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    geoSubject.Region = r.Value;
                                break;

                            case "mods:state":
                            case "state":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    geoSubject.State = r.Value;
                                break;

                            case "mods:territory":
                            case "territory":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    geoSubject.Territory = r.Value;
                                break;

                            case "mods:county":
                            case "county":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    geoSubject.County = r.Value;
                                break;

                            case "mods:city":
                            case "city":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    geoSubject.City = r.Value;
                                break;

                            case "mods:citySection":
                            case "citySection":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    geoSubject.CitySection = r.Value;
                                break;

                            case "mods:island":
                            case "island":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    geoSubject.Island = r.Value;
                                break;

                            case "mods:area":
                            case "area":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    geoSubject.Area = r.Value;
                                break;
                        }
                    }
                }
            }

            // Was this the cartographics subject?
            if (type == Subject_Info_Type.Cartographics)
            {
                Subject_Info_Cartographics mapSubject = new Subject_Info_Cartographics();
                mapSubject.Language = language;
                mapSubject.Authority = authority;
                mapSubject.ID = id;

                while (r.Read())
                {
                    if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "mods:subject") || (r.Name == "subject")))
                    {
                        if ((mapSubject.Projection.Length > 0) || (mapSubject.Coordinates.Length > 0) || (mapSubject.Scale.Length > 0))
                        {
                            thisBibInfo.Add_Subject(mapSubject);
                        }
                        return;
                    }

                    if (r.NodeType == XmlNodeType.Element)
                    {
                        switch (r.Name)
                        {
                            case "mods:coordinates":
                            case "coordinates":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    mapSubject.Coordinates = r.Value;
                                break;

                            case "mods:scale":
                            case "scale":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    mapSubject.Scale = r.Value;
                                break;

                            case "mods:projection":
                            case "projection":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    mapSubject.Projection = r.Value;
                                break;
                        }
                    }
                }
            }

            // Was this the name subject?
            if (type == Subject_Info_Type.Name)
            {
                Subject_Info_Name nameSubject = new Subject_Info_Name();
                nameSubject.Language = language;
                nameSubject.Authority = authority;
                nameSubject.ID = id;

                do
                {
                    if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "mods:subject") || (r.Name == "subject")))
                    {
                        thisBibInfo.Add_Subject(nameSubject);
                        return;
                    }

                    if (r.NodeType == XmlNodeType.Element)
                    {
                        switch (r.Name)
                        {
                            case "mods:name":
                            case "name":
                                Name_Info nameInfo = read_name_object(r);
                                nameSubject.Set_Internal_Name(nameInfo);
                                break;

                            case "mods:topic":
                            case "topic":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    nameSubject.Add_Topic(r.Value);
                                break;

                            case "mods:geographic":
                            case "geographic":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    nameSubject.Add_Geographic(r.Value);
                                break;

                            case "mods:genre":
                            case "genre":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    nameSubject.Add_Genre(r.Value);
                                break;

                            case "mods:temporal":
                            case "temporal":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    nameSubject.Add_Temporal(r.Value);
                                break;
                        }
                    }
                } while (r.Read());
            }

            // Was this the title subject?
            if (type == Subject_Info_Type.TitleInfo)
            {
                Subject_Info_TitleInfo titleSubject = new Subject_Info_TitleInfo();
                titleSubject.Language = language;
                titleSubject.Authority = authority;
                titleSubject.ID = id;

                do
                {
                    if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "mods:subject") || (r.Name == "subject")))
                    {
                        thisBibInfo.Add_Subject(titleSubject);
                        return;
                    }

                    if (r.NodeType == XmlNodeType.Element)
                    {
                        switch (r.Name)
                        {
                            case "mods:titleInfo":
                            case "titleInfo":
                                Title_Info titleInfo = read_title_object(r);
                                titleSubject.Set_Internal_Title(titleInfo);
                                break;

                            case "mods:topic":
                            case "topic":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    titleSubject.Add_Topic(r.Value);
                                break;

                            case "mods:geographic":
                            case "geographic":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    titleSubject.Add_Geographic(r.Value);
                                break;

                            case "mods:genre":
                            case "genre":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    titleSubject.Add_Genre(r.Value);
                                break;

                            case "mods:temporal":
                            case "temporal":
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    titleSubject.Add_Temporal(r.Value);
                                break;
                        }
                    }
                } while (r.Read());
            }
        }

        private static Title_Info read_title_object(XmlReader r)
        {
            Title_Info returnVal = new Title_Info();

            if (r.MoveToAttribute("ID"))
                returnVal.ID = r.Value;

            if (r.MoveToAttribute("type"))
            {
                switch (r.Value)
                {
                    case "alternative":
                        returnVal.Title_Type = Title_Type_Enum.Alternative;
                        break;

                    case "translated":
                        returnVal.Title_Type = Title_Type_Enum.Translated;
                        break;

                    case "uniform":
                        returnVal.Title_Type = Title_Type_Enum.Uniform;
                        break;

                    case "abbreviated":
                        returnVal.Title_Type = Title_Type_Enum.Abbreviated;
                        break;
                }
            }

            if (r.MoveToAttribute("displayLabel"))
                returnVal.Display_Label = r.Value;

            if (r.MoveToAttribute("lang"))
                returnVal.Language = r.Value;

            if (r.MoveToAttribute("authority"))
                returnVal.Authority = r.Value;

            while (r.Read())
            {
                if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "mods:titleInfo") || (r.Name == "titleInfo")))
                    return returnVal;

                if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.Name)
                    {
                        case "mods:title":
                        case "title":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                returnVal.Title = r.Value;
                            }
                            break;

                        case "mods:nonSort":
                        case "nonSort":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                returnVal.NonSort = r.Value;
                            }
                            break;

                        case "mods:subTitle":
                        case "subTitle":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                returnVal.Subtitle = r.Value;
                            }
                            break;

                        case "mods:partNumber":
                        case "partNumber":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                returnVal.Add_Part_Number(r.Value);
                            }
                            break;

                        case "mods:partName":
                        case "partName":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                returnVal.Add_Part_Name(r.Value);
                            }
                            break;
                    }
                }
            }

            return returnVal;
        }

        private static Name_Info read_name_object(XmlReader r)
        {
            Name_Info returnValue = new Name_Info();

            if (r.MoveToAttribute("type"))
            {
                if (r.Value == "personal")
                    returnValue.Name_Type = Name_Info_Type_Enum.Personal;
                if (r.Value == "corporate")
                    returnValue.Name_Type = Name_Info_Type_Enum.Corporate;
                if (r.Value == "conference")
                    returnValue.Name_Type = Name_Info_Type_Enum.Conference;
            }

            if (r.MoveToAttribute("ID"))
                returnValue.ID = r.Value;

            while (r.Read())
            {
                if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "mods:name") || (r.Name == "name")))
                    return returnValue;

                if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.Name)
                    {
                        case "mods:displayForm":
                        case "displayForm":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                returnValue.Display_Form = r.Value;
                            }
                            break;

                        case "mods:affiliation":
                        case "affiliation":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                returnValue.Affiliation = r.Value;
                            }
                            break;

                        case "mods:description":
                        case "description":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                returnValue.Description = r.Value;
                            }
                            break;

                        case "mods:namePart":
                        case "namePart":
                            string type = String.Empty;
                            if (r.MoveToAttribute("type"))
                            {
                                type = r.Value;
                            }
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                switch (type)
                                {
                                    case "given":
                                        returnValue.Given_Name = r.Value;
                                        break;

                                    case "family":
                                        returnValue.Family_Name = r.Value;
                                        break;

                                    case "termsOfAddress":
                                        returnValue.Terms_Of_Address = r.Value;
                                        break;

                                    case "date":
                                        returnValue.Dates = r.Value;
                                        break;

                                    default:
                                        returnValue.Full_Name = r.Value;
                                        break;
                                }
                            }
                            break;

                        case "mods:roleTerm":
                        case "roleTerm":
                            string role_type = String.Empty;
                            string role_authority = String.Empty;
                            if (r.MoveToAttribute("type"))
                                role_type = r.Value;
                            if (r.MoveToAttribute("authority"))
                                role_authority = r.Value;
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                switch (role_type)
                                {
                                    case "code":
                                        returnValue.Add_Role(r.Value, role_authority, Name_Info_Role_Type_Enum.Code);
                                        break;

                                    case "text":
                                        returnValue.Add_Role(r.Value, role_authority, Name_Info_Role_Type_Enum.Text);
                                        break;

                                    default:
                                        if (r.Value == "Main Entity")
                                            returnValue.Main_Entity = true;
                                        else
                                            returnValue.Add_Role(r.Value, role_authority, Name_Info_Role_Type_Enum.UNSPECIFIED);
                                        break;
                                }
                            }
                            break;
                    }
                }
            }

            return returnValue;
        }

        #endregion
    }
}