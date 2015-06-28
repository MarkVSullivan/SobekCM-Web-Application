#region Using directives

using System;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary>Designation of physical parts of a resource in a detailed form 
    /// (particularly in relation to the original host document or larger body of work)</summary>
    [Serializable]
    public class Part_Info : XML_Writing_Base_Type
    {
        private string day;
        private int day_index;
        private string enum1;
        private int enum1_index;
        private string enum2;
        private int enum2_index;
        private string enum3;
        private int enum3_index;
        private string enum4;
        private int enum4_index;
        private string month;
        private int month_index;
        private string year;
        private int year_index;

        /// <summary> Constructor for a new instance of the Part_Info class </summary>
        public Part_Info()
        {
            enum1_index = -1;
            enum2_index = -1;
            enum3_index = -1;
            enum4_index = -1;

            year_index = -1;
            month_index = -1;
            day_index = -1;
        }

        /// <summary> Clear all the data associated with this part information object </summary>
        public void Clear()
        {
            enum1 = null;
            enum2 = null;
            enum3 = null;
            enum4 = null;
            year = null;
            month = null;
            day = null;

            enum1_index = -1;
            enum2_index = -1;
            enum3_index = -1;
            enum4_index = -1;

            year_index = -1;
            month_index = -1;
            day_index = -1;
        }

        /// <summary> return a copy of this part information object  </summary>
        /// <returns></returns>
        public Part_Info Copy()
        {
            Part_Info copy = new Part_Info
            {
                Day = day, 
                Day_Index = Day_Index, 
                Enum1 = Enum1, 
                Enum1_Index = Enum1_Index, 
                Enum2 = Enum2, 
                Enum2_Index = Enum2_Index, 
                Enum3 = Enum3, 
                Enum3_Index = Enum3_Index, 
                Enum4 = Enum4, 
                Enum4_Index = Enum4_Index, 
                Month = Month, 
                Month_Index = Month_Index, 
                Year = Year, 
                Year_Index = Year_Index
            };
            return copy;
        }

        #region Internal methods and properties

        /// <summary> Returns flag which indicates this part object has data </summary>
        internal bool hasData
        {
            get
            {
                return (!String.IsNullOrEmpty(enum1)) || (!String.IsNullOrEmpty(enum2)) || (!String.IsNullOrEmpty(enum3)) || (!String.IsNullOrEmpty(enum4)) ||
                       (!String.IsNullOrEmpty(year)) || (!String.IsNullOrEmpty(month)) || (!String.IsNullOrEmpty(day));
            }
        }


        internal void Add_MODS(TextWriter Results)
        {
            if (!hasData)
                return;

            Results.Write("<mods:part>\r\n");

            if (!String.IsNullOrEmpty(enum1))
            {
                Results.Write("<mods:detail type=\"Enum1\">\r\n");
                Results.Write("<mods:caption>" + Convert_String_To_XML_Safe(enum1) + "</mods:caption>\r\n");
                if (enum1_index >= 0)
                {
                    Results.Write("<mods:number>" + enum1_index + "</mods:number>\r\n");
                }
                Results.Write("</mods:detail>\r\n");
            }

            if (!String.IsNullOrEmpty(enum2))
            {
                Results.Write("<mods:detail type=\"Enum2\">\r\n");
                Results.Write("<mods:caption>" + Convert_String_To_XML_Safe(enum2) + "</mods:caption>\r\n");
                if (enum2_index >= 0)
                {
                    Results.Write("<mods:number>" + enum2_index + "</mods:number>\r\n");
                }
                Results.Write("</mods:detail>\r\n");
            }

            if (!String.IsNullOrEmpty(enum3))
            {
                Results.Write("<mods:detail type=\"Enum3\">\r\n");
                Results.Write("<mods:caption>" + Convert_String_To_XML_Safe(enum3) + "</mods:caption>\r\n");
                if (enum3_index >= 0)
                {
                    Results.Write("<mods:number>" + enum3_index + "</mods:number>\r\n");
                }
                Results.Write("</mods:detail>\r\n");
            }

            if (!String.IsNullOrEmpty(enum4))
            {
                Results.Write("<mods:detail type=\"Enum1\">\r\n");
                Results.Write("<mods:caption>" + Convert_String_To_XML_Safe(enum4) + "</mods:caption>\r\n");
                if (enum4_index >= 0)
                {
                    Results.Write("<mods:number>" + enum4_index + "</mods:number>\r\n");
                }
                Results.Write("</mods:detail>\r\n");
            }

            if (!String.IsNullOrEmpty(year))
            {
                Results.Write("<mods:detail type=\"Year\">\r\n");
                Results.Write("<mods:caption>" + Convert_String_To_XML_Safe(year) + "</mods:caption>\r\n");
                if (year_index >= 0)
                {
                    Results.Write("<mods:number>" + year_index + "</mods:number>\r\n");
                }
                Results.Write("</mods:detail>\r\n");
            }

            if (!String.IsNullOrEmpty(month))
            {
                Results.Write("<mods:detail type=\"Month\">\r\n");
                Results.Write("<mods:caption>" + Convert_String_To_XML_Safe(month) + "</mods:caption>\r\n");
                if (month_index >= 0)
                {
                    Results.Write("<mods:number>" + month_index + "</mods:number>\r\n");
                }
                Results.Write("</mods:detail>\r\n");
            }

            if (!String.IsNullOrEmpty(day))
            {
                Results.Write("<mods:detail type=\"Day\">\r\n");
                Results.Write("<mods:caption>" + Convert_String_To_XML_Safe(day) + "</mods:caption>\r\n");
                if (day_index >= 0)
                {
                    Results.Write("<mods:number>" + day_index + "</mods:number>\r\n");
                }
                Results.Write("</mods:detail>\r\n");
            }

            Results.Write("</mods:part>\r\n");
        }

        #endregion

        #region Public Properties

        /// <summary> Gets and sets the text for the first level of enumeration </summary>
        public string Enum1
        {
            get { return enum1 ?? String.Empty; }
            set { enum1 = value; }
        }

        /// <summary> Gets and sets the text for the second level of enumeration </summary>
        public string Enum2
        {
            get { return enum2 ?? String.Empty; }
            set { enum2 = value; }
        }

        /// <summary> Gets and sets the text for the third level of enumeration </summary>
        public string Enum3
        {
            get { return enum3 ?? String.Empty; }
            set { enum3 = value; }
        }

        /// <summary> Gets and sets the text for the fourth level of enumeration </summary>
        public string Enum4
        {
            get { return enum4 ?? String.Empty; }
            set { enum4 = value; }
        }

        /// <summary> Gets and sets the text for the year/first level of chronology </summary>
        public string Year
        {
            get { return year ?? String.Empty; }
            set { year = value; }
        }

        /// <summary> Gets and sets the text for the month/second level of chronology </summary>
        public string Month
        {
            get { return month ?? String.Empty; }
            set { month = value; }
        }

        /// <summary> Gets and sets the text for the day/third level of chronology </summary>
        public string Day
        {
            get { return day ?? String.Empty; }
            set { day = value; }
        }

        /// <summary> Gets and sets the index for the first level of enumeration </summary>
        public int Enum1_Index
        {
            get { return enum1_index; }
            set { enum1_index = value; }
        }

        /// <summary> Gets and sets the index for the second level of enumeration </summary>
        public int Enum2_Index
        {
            get { return enum2_index; }
            set { enum2_index = value; }
        }

        /// <summary> Gets and sets the index for the third level of enumeration </summary>
        public int Enum3_Index
        {
            get { return enum3_index; }
            set { enum3_index = value; }
        }

        /// <summary> Gets and sets the index for the fourth level of enumeration </summary>
        public int Enum4_Index
        {
            get { return enum4_index; }
            set { enum4_index = value; }
        }


        /// <summary> Gets and sets the index for the year/first level of chronology </summary>
        public int Year_Index
        {
            get { return year_index; }
            set { year_index = value; }
        }

        /// <summary> Gets and sets the index for the month/second level of chronology </summary>
        public int Month_Index
        {
            get { return month_index; }
            set { month_index = value; }
        }

        /// <summary> Gets and sets the index for the day/third level of chronology </summary>
        public int Day_Index
        {
            get { return day_index; }
            set { day_index = value; }
        }

        #endregion
    }
}