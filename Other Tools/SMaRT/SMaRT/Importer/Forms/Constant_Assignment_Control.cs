using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using UF.Libraries.DLC.TrackingDatabase.Database;

namespace DLC_Importer_Main.Forms
{
    public partial class Constant_Assignment_Control : UserControl
    {
        private string list_value;
       
        protected static string[] possible_maps = { "None", "Aleph", "BibID",   
                "Creator", "NOTIS", "OCLC", "Publisher", "Title", "Abstract", "Alternate Title", 
                "Attribution", "Primary Collection Code", "Alternate Collection Code", "Contributor", 
                "Description", "Donor", "Genre", "Holding Location Code", "Holding Location Statement", 
                "Identifer", "Language", "Source Institution Code", "Source Institution Statement", 
                "SubCollection", "Subject Keyword", "VID", "Comments", "Date", "Edition", 
                "Physical Description", "Note", "Rights", "Series Title", "Material Type", "Uniform Title",
                "LCCN", "ISBN", "ISSN", "Volume", "Issue", "IssueYear", "IssueMonth", "IssueDay", "Section", "Year", "Month", "Day",
                "Coordinates", "Projection", "Scale", "Icon/Wordmarks", "Temporal Coverage", 
                "Affiliation.University", "Affiliation.Campus", "Affiliation.College", "Affiliation.Unit",
                "Affiliation.Department", "Affiliation.Institute", "Affiliation.Center", "Affiliation.Section",
                "Affiliation.Subsection", "Geography.Continent", "Geography.Country", "Geography.Province",  
                "Geography.Region", "Geography.State", "Geography.Territory", "Geography.County", "Geography.City",
                "Geography.Island", "Geography.Area", "Target Technology", "Entity Type"
        };

        protected static string[] tracking_mappable_fields = { "None",                    
                "Primary Collection Code", 
                "Alternate Collection Code",  
                "Holding Location Code",  
                "Source Institution Code",  
                "Material Type",              
                "Entity Type",
                "Target Technology", 
                "Comments"  
        };       

        protected static string[] institution_codes = 
            {                   
                //"AC - Alachua County Public Library",
                //"AM - Florida Agircultural and Mechanical University",
                ////"ANH - Archives Nationales d'Haïti",
                ////"AUF - Agence universitaire de la Francophonie",
                ////"BHFIC - Bibliothèque Haïtienne des Frères de l'Instruction Chrétienne",
                ////"BHPSE - Bibliothèque Haïtienne des Pères du Saint-Esprit",
                //"BN - Bibliotheque Nationale d'Haiti",
                ////"BNH - Bibliothèque Nationale d'Haïti",
                ////"BNPHU - Biblioteca Nacional Pedro Henríquez Ureña",
                //"CA - Digital Library of the Caribbean",
                ////"CARICOM - Caribbean Community Secretariat",
                //"CF - Univeristy of Central Florida",
                //"FA - Florida Atlantic University",
                //"FI - Florida International University",
                //"FS - Florida State University",
                ////"FUNGLODE - Fundacion Global Desarrollo y Democracia",
                //"GC - Florida Gulf Coast University",
                //"JU - Jacksonville University",
                //"MH - Matheson Historical Society",
                //"MM - Monroe County Public Library",
                //"NF - University of North Florida",
                //"NG - Florida National Guard",
                ////"NLG - National Library of Guyana",
                ////"NLJ - National Library of Jamaica",
                ////"PUCMMA - Pontificia Universidad Católica Madre y Maestra - Recinto Santo Tomás de Aquino",
                //"SA - Florida State Archives",
                //"SF - University of South Florida",
                //"SW - Southwest Florida Library Network",
                ////"UASD - Universidad Autónoma de Santo Domingo",
                //"UF - University of Florida",
                //"UM - University of Miami",
                ////"UNPHU - Universidad Nacional Pedro Henríquez Ureña",
                ////"UOV - Universidad de Oriente, Venezuela",
                //"VI - University of Virgin Islands",
                //"WF - University of West Florida",
                //"WT - University of the West Indies"

                "ACPL - Alachua County Public Library",
                "FAMU - Florida Agircultural and Mechanical University",
                "BNH - Bibliotheque Nationale d'Haiti",
                //"BNH - Bibliothèque Nationale d'Haïti",
                "CA - Digital Library of the Caribbean",
                "UCF - Univeristy of Central Florida",
                "FAU - Florida Atlantic University",
                "FIU - Florida International University",
                "FSU - Florida State University",
                "FGCU - Florida Gulf Coast University",
                "JU - Jacksonville University",
                "MATHESON - Matheson Historical Society",
                "MCPL - Monroe County Public Library",
                "UNF - University of North Florida",
                "FNG - Florida National Guard",
                "FSA - Florida State Archives",
                "USF - University of South Florida",
                "SWFLN - Southwest Florida Library Network",
                "UF - University of Florida",
                "UM - University of Miami",
                "UVI - University of Virgin Islands",
                "UWF - University of West Florida",
                "UWI - University of the West Indies"
            };

       
        //protected static string[] material_types = { "NEWSPAPER", "MONOGRAPH", "BOOK", "SERIAL",
        //        "AERIAL", "PHOTOGRAPH", "POSTCARD", "MAP", "TEXT", "AUDIO", "VIDEO",
        //        "ARCHIVE", "ARTIFACT", "IMAGE", "UNDETERMINED" };   
    

        public Constant_Assignment_Control()
        {
            InitializeComponent();

            this.cboMappedField.Items.AddRange(tracking_mappable_fields);           
        }

        public string Mapped_Name
        {
            get { return cboMappedField.Text; }
            set { cboMappedField.Text = value; }
        }

        public string Mapped_Constant
        {
            get {

                if ((Mapped_Field == UFDC_Bib_Package.Mapped_Fields.Source_Code) && 
                        (cboMappedConstant.Text.Length > 0))
                {
                    list_value = cboMappedConstant.Text.Substring(0, cboMappedConstant.Text.IndexOf(' '));

                    return list_value;
                }
                else
                {

                    return cboMappedConstant.Text;
                }
            }
        }

        public bool hasItemInList( string item )
        {
            return cboMappedField.Items.Contains( item );
        }


        public void Select_List_Item(string columnName)
        {
            int index = -1;

            index = this.cboMappedField.FindString(columnName.Trim());

            if (index > 0)
                this.cboMappedField.SelectedIndex = index;

            else
            {
                string listValue = String.Empty;
                switch (columnName.ToUpper().Trim())
                {
                    case "LTQF":
                    case "LTUF":
                        listValue = "NOTIS";
                        break;

                    case "AUTHOR":
                        listValue = "Creator";
                        break;

                    case "MATERIAL TYPE":
                    case "TYPE":
                        listValue = "Material Type";
                        break;

                    case "FORMAT":
                    case "PHYSICAL DESCRIPTION":
                        listValue = "Physical Description";
                        break;

                    case "PROJECT CODE":
                        listValue = "Primary Collection Code";
                        break;

                    case "ALTERNATE PROJECT CODE":
                        listValue = "Alternate Collection Code";
                        break;

                    case "AFFILIATION UNIVERSITY":
                    case "AFFILIATION#UNIVERSITY":
                    case "AFF#UNIVERSITY":
                    case "UNIVERSITY":
                        listValue = "Affiliation.University";
                        break;

                    case "AFFILIATION CAMPUS":
                    case "AFFILIATION#CAMPUS":
                    case "AFF#CAMPUS":
                    case "CAMPUS":
                        listValue = "Affiliation.Campus";
                        break;

                    case "AFFILIATION COLLEGE":
                    case "AFFILIATION#COLLEGE":
                    case "AFF#COLLEGE":
                    case "COLLEGE":
                        listValue = "Affiliation.College";
                        break;

                    case "AFFILIATION UNIT":
                    case "AFFILIATION#UNIT":
                    case "AFF#UNIT":
                    case "UNIT":
                        listValue = "Affiliation.Unit";
                        break;

                    case "AFFILIATION DEPARTMENT":
                    case "AFFILIATION#DEPARTMENT":
                    case "AFF#DEPARTMENT":
                    case "DEPARTMENT":
                        listValue = "Affiliation.Department";
                        break;

                    case "AFFILIATION INSTITUTE":
                    case "AFFILIATION#INSTITUTE":
                    case "AFF#INSTITUTE":
                    case "INSTITUTE":
                        listValue = "Affiliation.Institute";
                        break;

                    case "AFFILIATION CENTER":
                    case "AFFILIATION#CENTER":
                    case "AFF#CENTER":
                    case "CENTER":
                        listValue = "Affiliation.Center";
                        break;

                    case "AFFILIATION SECTION":
                    case "AFFILIATION#SECTION":
                    case "AFF#SECTION":
                    case "SECTION":
                        listValue = "Affiliation.Section";
                        break;

                    case "AFFILIATION SUBSECTION":
                    case "AFFILIATION#SUBSECTION":
                    case "AFF#SUBSECTION":
                    case "SUBSECTION":
                        listValue = "Affiliation.Subsection";
                        break;

                    case "GEOGRAPHY CONTINENT":
                    case "GEOGRAPHY#CONTINENT":
                    case "GEO#CONTINENT":
                    case "CONTINENT":
                        listValue = "Geography.Continent";
                        break;

                    case "GEOGRAPHY COUNTRY":
                    case "GEOGRAPHY#COUNTRY":
                    case "GEO#COUNTRY":
                    case "COUNTRY":
                        listValue = "Geography.Country";
                        break;

                    case "GEOGRAPHY PROVINCE":
                    case "GEOGRAPHY#PROVINCE":
                    case "GEO#PROVINCE":
                    case "PROVINCE":
                        listValue = "Geography.Province";
                        break;

                    case "GEOGRAPHY REGION":
                    case "GEOGRAPHY#REGION":
                    case "GEO#REGION":
                    case "REGION":
                        listValue = "Geography.Region";
                        break;

                    case "GEOGRAPHY STATE":
                    case "GEOGRAPHY#STATE":
                    case "GEO#STATE":
                    case "STATE":
                        listValue = "Geography.State";
                        break;

                    case "GEOGRAPHY TERRITORY":
                    case "GEOGRAPHY#TERRITORY":
                    case "GEO#TERRITORY":
                    case "TERRITORY":
                        listValue = "Geography.Territory";
                        break;

                    case "GEOGRAPHY COUNTY":
                    case "GEOGRAPHY#COUNTY":
                    case "GEO#COUNTY":
                    case "COUNTY":
                        listValue = "Geography.County";
                        break;

                    case "GEOGRAPHY CITY":
                    case "GEOGRAPHY#CITY":
                    case "GEO#CITY":
                    case "CITY":
                        listValue = "Geography.City";
                        break;

                    case "GEOGRAPHY ISLAND":
                    case "GEOGRAPHY#ISLAND":
                    case "GEO#ISLAND":
                    case "ISLAND":
                        listValue = "Geography.Island";
                        break;

                    case "GEOGRAPHY AREA":
                    case "GEOGRAPHY#AREA":
                    case "GEO#AREA":
                    case "AREA":
                        listValue = "Geography.Area";
                        break;

                    default:
                        listValue = "None";
                        break;
                }

                this.cboMappedField.SelectedIndex = this.cboMappedField.FindStringExact(listValue);
            }

        }
             

        public UFDC_Bib_Package.Mapped_Fields Mapped_Field
        {
            get
            {
                switch (cboMappedField.Text)
                {
                    case "None":
                        return UFDC_Bib_Package.Mapped_Fields.None;

                    case "Aleph":
                        return UFDC_Bib_Package.Mapped_Fields.Aleph;

                    case "BibID":
                        return UFDC_Bib_Package.Mapped_Fields.BibID;

                    case "Collection":
                        return UFDC_Bib_Package.Mapped_Fields.Collection_Primary;

                    case "Creator":
                        return UFDC_Bib_Package.Mapped_Fields.Creator;

                    case "NOTIS":
                        return UFDC_Bib_Package.Mapped_Fields.NOTIS;

                    case "OCLC":
                        return UFDC_Bib_Package.Mapped_Fields.OCLC;

                    case "Publisher":
                        return UFDC_Bib_Package.Mapped_Fields.Publisher;

                    case "Title":
                        return UFDC_Bib_Package.Mapped_Fields.Title;

                    case "Abstract":
                        return UFDC_Bib_Package.Mapped_Fields.Abstract;

                    case "Alternate Title":
                        return UFDC_Bib_Package.Mapped_Fields.Alternate_Title;

                    case "Attribution":
                        return UFDC_Bib_Package.Mapped_Fields.Attribution;

                    case "Primary Collection Code":
                        return UFDC_Bib_Package.Mapped_Fields.Collection_Primary;

                    case "Alternate Collection Code":
                        return UFDC_Bib_Package.Mapped_Fields.Collection_Alternate;

                    case "Contributor":
                        return UFDC_Bib_Package.Mapped_Fields.Contributor;

                    case "Description":
                        return UFDC_Bib_Package.Mapped_Fields.Description;

                    case "Donor":
                        return UFDC_Bib_Package.Mapped_Fields.Donor;

                    case "Genre":
                        return UFDC_Bib_Package.Mapped_Fields.Genre;

                    case "Holding Location Code":
                        return UFDC_Bib_Package.Mapped_Fields.Holding_Code;

                    case "Holding Location Statement":
                        return UFDC_Bib_Package.Mapped_Fields.Holding_Statement;

                    case "Identifer":
                        return UFDC_Bib_Package.Mapped_Fields.Identifier;

                    case "Language":
                        return UFDC_Bib_Package.Mapped_Fields.Language;

                    case "Source Institution Code":
                        return UFDC_Bib_Package.Mapped_Fields.Source_Code;

                    case "Source Institution Statement":
                        return UFDC_Bib_Package.Mapped_Fields.Source_Statement;

                    case "SubCollection":
                        return UFDC_Bib_Package.Mapped_Fields.SubCollection;

                    case "Subject Keyword":
                        return UFDC_Bib_Package.Mapped_Fields.Subject_Keyword;

                    case "VID":
                        return UFDC_Bib_Package.Mapped_Fields.VID;

                    case "Comments":
                        return UFDC_Bib_Package.Mapped_Fields.Comments;

                    case "Date":
                        return UFDC_Bib_Package.Mapped_Fields.Date;

                    case "Edition":
                        return UFDC_Bib_Package.Mapped_Fields.Edition;

                    case "Physical Description":
                        return UFDC_Bib_Package.Mapped_Fields.Format;

                    case "Rights":
                        return UFDC_Bib_Package.Mapped_Fields.Rights;

                    case "Series Title":
                        return UFDC_Bib_Package.Mapped_Fields.Series_Title;

                    case "Material Type":
                        return UFDC_Bib_Package.Mapped_Fields.Type;

                    case "Note":
                        return UFDC_Bib_Package.Mapped_Fields.Note;

                    case "Uniform Title":
                        return UFDC_Bib_Package.Mapped_Fields.Uniform_Title;

                    case "LCCN":
                        return UFDC_Bib_Package.Mapped_Fields.LCCN;

                    case "ISBN":
                        return UFDC_Bib_Package.Mapped_Fields.ISBN;

                    case "ISSN":
                        return UFDC_Bib_Package.Mapped_Fields.ISSN;

                    case "Volume":
                        return UFDC_Bib_Package.Mapped_Fields.Volume;

                    case "Issue":
                        return UFDC_Bib_Package.Mapped_Fields.Issue;

                    case "Section":
                        return UFDC_Bib_Package.Mapped_Fields.Section;

                    case "Year":
                    case "IssueYear":
                        return UFDC_Bib_Package.Mapped_Fields.Year;

                    case "Month":
                    case "IssueMonth":
                        return UFDC_Bib_Package.Mapped_Fields.Month;

                    case "Day":
                    case "IssueDay":
                        return UFDC_Bib_Package.Mapped_Fields.Day;

                    case "Coordinates":
                        return UFDC_Bib_Package.Mapped_Fields.Coordinates;

                    case "Projection":
                        return UFDC_Bib_Package.Mapped_Fields.Projection;

                    case "Scale":
                        return UFDC_Bib_Package.Mapped_Fields.Scale;

                    //case "Spatial Coverage":
                    //    return UFDC_Bib_Package.Mapped_Fields.Spatial_Coverage;

                    case "Icon/Wordmarks":
                        return UFDC_Bib_Package.Mapped_Fields.Icon_Wordmarks;

                    case "Temporal Coverage":
                        return UFDC_Bib_Package.Mapped_Fields.Temporal_Coverage;

                    case "Affiliation.University":
                        return UFDC_Bib_Package.Mapped_Fields.Affiliation_University;

                    case "Affiliation.Campus":
                        return UFDC_Bib_Package.Mapped_Fields.Affiliation_Campus;

                    case "Affiliation.College":
                        return UFDC_Bib_Package.Mapped_Fields.Affiliation_College;

                    case "Affiliation.Unit":
                        return UFDC_Bib_Package.Mapped_Fields.Affiliation_Unit;

                    case "Affiliation.Department":
                        return UFDC_Bib_Package.Mapped_Fields.Affiliation_Department;

                    case "Affiliation.Institute":
                        return UFDC_Bib_Package.Mapped_Fields.Affiliation_Institute;

                    case "Affiliation.Center":
                        return UFDC_Bib_Package.Mapped_Fields.Affiliation_Center;

                    case "Affiliation.Section":
                        return UFDC_Bib_Package.Mapped_Fields.Affiliation_Section;

                    case "Affiliation.Subsection":
                        return UFDC_Bib_Package.Mapped_Fields.Affiliation_Subsection;

                    case "Geography.Continent":
                        return UFDC_Bib_Package.Mapped_Fields.Geography_Continent;

                    case "Geography.Country":
                        return UFDC_Bib_Package.Mapped_Fields.Geography_Country;

                    case "Geography.Province":
                        return UFDC_Bib_Package.Mapped_Fields.Geography_Province;

                    case "Geography.Region":
                        return UFDC_Bib_Package.Mapped_Fields.Geography_Region;

                    case "Geography.State":
                        return UFDC_Bib_Package.Mapped_Fields.Geography_State;

                    case "Geography.Territory":
                        return UFDC_Bib_Package.Mapped_Fields.Geography_Territory;

                    case "Geography.County":
                        return UFDC_Bib_Package.Mapped_Fields.Geography_County;

                    case "Geography.City":
                        return UFDC_Bib_Package.Mapped_Fields.Geography_City;

                    case "Geography.Island":
                        return UFDC_Bib_Package.Mapped_Fields.Geography_Island;

                    case "Geography.Area":
                        return UFDC_Bib_Package.Mapped_Fields.Geography_Area;

                    default:
                        return UFDC_Bib_Package.Mapped_Fields.None;
                }
            }
        }

        private void cboMappedField_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboMappedConstant.Items.Clear();
            cboMappedConstant.DropDownStyle = ComboBoxStyle.Simple;

            int index = 0;
            switch (cboMappedField.Text)
            {
                case "None":
                    break;               

                case "Source Institution Code":
                case "Holding Location Code":
                    cboMappedConstant.Items.AddRange(institution_codes);                    
                    cboMappedConstant.DropDownStyle = ComboBoxStyle.DropDownList;

                    // select default value
                    index = cboMappedConstant.FindString("UF");

                    if (index > 0)
                        cboMappedConstant.SelectedIndex = index;
                    break;                              

                case "Material Type":
                    //cboMappedConstant.Items.AddRange(material_types);
                    cboMappedConstant.Items.AddRange(Bibliographic_Type_Mill.ToStringArray());
                    cboMappedConstant.SelectedIndex = -1;

                    cboMappedConstant.DropDownStyle = ComboBoxStyle.DropDownList;                   
                    break;

                case "Primary Collection Code":
                case "Alternate Collection Code":
                    // Populate all the project codes
			        foreach( DataRow thisRow in CS_TrackingDatabase.Project_Codes.Rows )
			        {
                        this.cboMappedConstant.Items.Add(thisRow["itemcode"].ToString());
			        }

                    cboMappedConstant.DropDownStyle = ComboBoxStyle.DropDownList;                  
                    break;

                case "Target Technology":
                    // Populate all the project codes
                    foreach (DataRow thisRow in CS_TrackingDatabase.Target_Technologies.Rows)
                    {
                        this.cboMappedConstant.Items.Add(thisRow["technologytype"].ToString());
                    }

                    cboMappedConstant.DropDownStyle = ComboBoxStyle.DropDownList;

                    // select default value
                    index = cboMappedConstant.FindString("UFDC");
                    if (index > 0)
                        cboMappedConstant.SelectedIndex = index;
                    break;

                case "Entity Type":
                    // Populate all the entity types
                    foreach (DataRow thisRow in CS_TrackingDatabase.Get_All_EntityType.Rows)
                    {
                        this.cboMappedConstant.Items.Add(thisRow["entityname"].ToString());
                    }

                    cboMappedConstant.DropDownStyle = ComboBoxStyle.DropDownList;                  
                    break;  

                default:
                    break;
            }

        }

        # region Public Properties

        /// <summary> Gets the collection of ProjectCodes. </summary>
        public static DataTable Project_Codes
        {
            get { return UF.Libraries.DLC.TrackingDatabase.Database.CS_TrackingDatabase.Project_Codes; }
        }

        #endregion

        private void cboMappedConstant_SelectedIndexChanged(object sender, EventArgs e)
        {
            //cboMappedConstant.Parent.Parent.Refresh();
        }
    }
}
