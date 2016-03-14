using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Resource_Object.Behaviors;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Shortcuts that are used just for display of this item within the web </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("web")]
    public class BriefItem_Web
    {
        private Dictionary<string, string> fileExtensionLookupDictionary;


        /// <summary> Base source URL for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "sourceUrl")]
        [XmlElement("sourceUrl")]
        [ProtoMember(1)]
        public string Source_URL { get; set; }

        /// <summary> List of all the file extensions included in this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "fileExtensions")]
        [XmlArray("fileExtensions")]
        [XmlArrayItem("extension", typeof(string))]
        [ProtoMember(2)]
        public List<string> File_Extensions { get; set; }

        /// <summary> Number of siblings which this item has ( i.e., is this a multi-volume set? ) </summary>
        [DataMember(EmitDefaultValue = false, Name = "siblings")]
        [XmlElement("siblings")]
        [ProtoMember(3)]
        public int? Siblings { get; set; }

        /// <summary> Primary key (ItemID) for this individual item/volume from the SobekCM Web database </summary>
        [DataMember(Name = "itemid")]
        [XmlAttribute("itemid")]
        [ProtoMember(4)]
        public int ItemID { get; set; }

        /// <summary> Primary key (GroupID) for the BibiD / title from the SobekCM Web database </summary>
        [DataMember(Name = "groupid")]
        [XmlAttribute("groupid")]
        [ProtoMember(5)]
        public int GroupID { get; set; }

        /// <summary> List of all the file extensions included in this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "relatedTitles")]
        [XmlArray("relatedTitles")]
        [XmlArrayItem("relatedTitle", typeof(BriefItem_Related_Titles))]
        [ProtoMember(6)]
        public List<BriefItem_Related_Titles> Related_Titles { get; set; }

        /// <summary> Main issue date for this item </summary>
        [DataMember(Name = "date")]
        [XmlAttribute("date")]
        [ProtoMember(7)]
        public string Date { get; set; }

        /// <summary> Internal comments linked to this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "internalComments")]
        [XmlElement("internalComments")]
        [ProtoMember(8)]
        public string Internal_Comments { get; set; }

        /// <summary> Additional HTML (usually a link) to display in the title box, just below the title </summary>
        /// <remarks> This highlights some additional piece of information, such as an external link </remarks>
        [DataMember(EmitDefaultValue = false, Name = "titleBoxAdditionalLink")]
        [XmlElement("titleBoxAdditionalLink")]
        [ProtoMember(9)]
        public string Title_Box_Additional_Link { get; set; }

        /// <summary> Type (usually 'Related Link') of the additional HTML to display in the title box, just below the title </summary>
        /// <remarks> This highlights some additional piece of information, such as an external link </remarks>
        [DataMember(EmitDefaultValue = false, Name = "titleBoxAdditionalType")]
        [XmlElement("titleBoxAdditionalType")]
        [ProtoMember(10)]
        public string Title_Box_Additional_Link_Type { get; set; }

        /// <summary> List of all the user entered tags in this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "userTags")]
        [XmlArray("userTags")]
        [XmlArrayItem("userTag", typeof(BriefItem_UserTag))]
        [ProtoMember(11)]
        public List<BriefItem_UserTag> User_Tags { get; set; }


        /// <summary> Add a new user tag to this item </summary>
        /// <param name="UserID"> Primary key for the user who entered this tag </param>
        /// <param name="UserName"> Name of the user ( Last Name, Firt Name )</param>
        /// <param name="Description_Tag"> Text of the user-entered descriptive tag </param>
        /// <param name="Date_Added"> Date the tag was added or last modified </param>
        /// <param name="TagID"> Primary key for this tag from the database </param>
        public void Add_User_Tag(int UserID, string UserName, string Description_Tag, DateTime Date_Added, int TagID)
        {
            if (User_Tags == null)
                User_Tags = new List<BriefItem_UserTag>();

            foreach (BriefItem_UserTag thisTag in User_Tags.Where(ThisTag => ThisTag.TagID == TagID))
            {
                thisTag.Description_Tag = Description_Tag;
                thisTag.UserID = UserID;
                thisTag.Date_Added = DateTime.Now;
                return;
            }

            User_Tags.Add(new BriefItem_UserTag(UserID, UserName, Description_Tag, Date_Added, TagID));
        }


        /// <summary> Delete a user tag from this object, by TagID and UserID </summary>
        /// <param name="TagID"> Primary key for this tag from the database </param>
        /// <param name="UserID">  Primary key for the user who entered this tag </param>
        /// <returns> Returns TRUE is successful, otherwise FALSE </returns>
        /// <remarks> This only deletes the user tag if the UserID for the tag matches the provided userid </remarks>
        public bool Delete_User_Tag(int TagID, int UserID)
        {
            if (User_Tags == null)
                return false;

            BriefItem_UserTag tag_to_delete = User_Tags.FirstOrDefault(ThisTag => (ThisTag.TagID == TagID) && (ThisTag.UserID == UserID));
            if (tag_to_delete == null)
            {
                return false;
            }

            User_Tags.Remove(tag_to_delete);
            return true;
        }

        /// <summary> Add a related title to this web information </summary>
        /// <param name="Relationship"> Relationship between the main title and the related title</param>
        /// <param name="Title"> Title of the related title within this SobekCM library</param>
        /// <param name="Link"> Link for the related title within this SobekCM library</param>
        public void Add_Related_Title(string Relationship, string Title, string Link)
        {
            // Make sure not null
            if (Related_Titles == null)
                Related_Titles = new List<BriefItem_Related_Titles>();

            // Add this
            Related_Titles.Add(new BriefItem_Related_Titles(Relationship, Title, Link));
        }

        /// <summary> Checks to see if a file extension exists in the file extensions lists </summary>
        /// <param name="Extension"> File extension to check </param>
        /// <returns> TRUE if the file extension exists, otherwise FALSE </returns>
        /// <remarks> The file extension check is not caps sensitive and is tolerant of a period at the beginning, but it is
        /// not necessary either.  </remarks>
        public bool Contains_File_Extension(string Extension)
        {
            // If dictionary is NULL, define it
            if (fileExtensionLookupDictionary == null)
                fileExtensionLookupDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // If the file extensions list is NULL, than probably no files attached
            if ((File_Extensions == null) || (File_Extensions.Count == 0))
                return false;

            // If the count doesn't match the file extensions, than add them all
            if (fileExtensionLookupDictionary.Count != File_Extensions.Count)
            {
                fileExtensionLookupDictionary.Clear();
                foreach (string thisExtention in File_Extensions)
                {
                    if ((thisExtention.Length > 1) && (thisExtention[0] == '.'))
                        fileExtensionLookupDictionary.Add(thisExtention.Substring(1), thisExtention);
                    else
                        fileExtensionLookupDictionary.Add(thisExtention, thisExtention);
                }
            }

            // Now, just look to see if it exists (but check first to see if a period was passed in)
            if ((Extension.Length > 1) && (Extension[0] == '.'))
                return fileExtensionLookupDictionary.ContainsKey(Extension.Substring(1));
            
            // No period, so simple test
            return fileExtensionLookupDictionary.ContainsKey(Extension);
        }


    }
}
