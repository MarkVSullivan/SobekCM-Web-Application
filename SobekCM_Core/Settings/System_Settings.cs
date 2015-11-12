using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.Configuration;

namespace SobekCM.Core.Settings
{
    /// <summary> Top-level settings that control basic operation and appearance of the entire SobekCM instance </summary>
    [Serializable, DataContract, ProtoContract]
    public class System_Settings
    {
        /// <summary> Constructor for a new instance of the System_Settings class </summary>
        public System_Settings()
        {
            Include_Partners_On_System_Home = false;
            Include_TreeView_On_System_Home = false;
            Default_UI_Language = Web_Language_Enum.English;
        }

        /// <summary> Returns the default user interface language </summary>
        [DataMember]
        public Web_Language_Enum Default_UI_Language { get; set; }

        /// <summary> Flag determines if the detailed view of user permissions for items in an aggregation should show </summary>
        [DataMember]
        public bool Detailed_User_Aggregation_Permissions { get; set; }

        /// <summary> Flag indicates if logon has been restricted to system admins </summary>
        [DataMember]
        public bool Disable_Standard_User_Logon_Flag { get; set; }

        /// <summary> Message to go with the logon restriction </summary>
        [DataMember]
        public string Disable_Standard_User_Logon_Message { get; set; }

        /// <summary> Flag indicates if the partners browse should be displayed on the home page </summary>
        [DataMember]
        public bool Include_Partners_On_System_Home { get; set; }

        /// <summary> Flag indicates if the tree view should be displayed on the home page </summary>
        [DataMember]
        public bool Include_TreeView_On_System_Home { get; set; }





        /// <summary> Gets the abbrevation used to refer to this digital library </summary>
        [DataMember]
        public string System_Abbreviation { get; set; }


        /// <summary> Gets the base name for this system </summary>
        [DataMember]
        public string System_Name { get; set; }

        /// <summary> Set the default UI language, by passing in a string </summary>
        [XmlIgnore]
        public string Default_UI_Language_String
        {
            set
            {
                Default_UI_Language = Web_Language_Enum_Converter.Code_To_Enum(Web_Language_Enum_Converter.Name_To_Code(value));
            }
            get { return Web_Language_Enum_Converter.Enum_To_Name(Default_UI_Language); }
        }
    }
}
