using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace DLC_Importer_Main.Forms
{
    public partial class Column_Assignment_Control : UserControl
    {
        private bool empty;

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
                "Geography.Island", "Geography.Area", "Entity Type"
        };
                                           
        public Column_Assignment_Control()
        {
            InitializeComponent();

            this.comboBox1.Items.AddRange(possible_maps);
                      
            empty = false;
        }

        public bool Empty
        {
            get
            {
                return empty;
            }
            set
            {
                empty = value;
                if (empty)
                {
                    this.textBox1.ReadOnly = false;
                    this.textBox1.Text = "{Empty}";
                }
                else
                {
                    this.textBox1.ReadOnly = true;
                }
            }
        }

        public string Column_Name
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public string Mapped_Name
        {
            get { return comboBox1.Text; }
        }

        public bool hasItemInList( string item )
        {
            return comboBox1.Items.Contains( item );
        }

        public void Select_List_Item(string columnName)
        {            
            int index = -1;            

            index = this.comboBox1.FindString(columnName.Trim());
            
            if (index > 0)
                this.comboBox1.SelectedIndex = index;
           
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

                    case "ENTITY TYPE":
                    case "SUB TYPE":
                        listValue = "Entity Type";
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

                this.comboBox1.SelectedIndex = this.comboBox1.FindStringExact(listValue);
            }
           
        }

        public UFDC_Bib_Package.Mapped_Fields Mapped_Field
        {
            get
            {
                switch (comboBox1.Text)
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

                    case "Entity Type":
                        return UFDC_Bib_Package.Mapped_Fields.Sub_Type;

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
    }
}
