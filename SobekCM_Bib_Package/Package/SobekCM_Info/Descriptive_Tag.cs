using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.SobekCM_Info
{
    /// <summary> Class holds the information about a single user-entered descriptive tag in relation to this digital object </summary>
    /// <remarks>This data is not stored in the item metadata, but is retrieved from the database when the item is displayed in the digital library </remarks>
    [Serializable]
    public class Descriptive_Tag
    {
        private string username;
        private int userid;
        private string description_tag;
        private DateTime dateAdded;
        private int tagid;

        /// <summary> Constructor for a new instance of the Descriptive_Tag class </summary>
        /// <param name="UserID"> Primary key for the user who entered this tag </param>
        /// <param name="UserName"> Name of the user ( Last Name, Firt Name )</param>
        /// <param name="Description_Tag"> Text of the user-entered descriptive tag </param>
        /// <param name="Date_Added"> Date the tag was added or last modified </param>
        /// <param name="TagID"> Primary key for this tag from the database </param>
        public Descriptive_Tag(int UserID, string UserName, string Description_Tag, DateTime Date_Added, int TagID )
        {
            username = UserName;
            userid = UserID;
            description_tag = Description_Tag;
            dateAdded = Date_Added;
            tagid = TagID;
        }

        /// <summary> Primary key for this tag from the database </summary>
        public int TagID
        {
            get { return tagid; }
            set { tagid = value; }
        }

        /// <summary> Date the tag was added or last modified </summary>
        public DateTime Date_Added
        {
            get { return dateAdded; }
            set { dateAdded = value; }
        }

        /// <summary> Primary key to the user who entered this tag </summary>
        public int UserID
        {
            get { return userid; }
            set { userid = value; }
        }

        /// <summary> Name of the user who entered this tag in the format 'Last Name, First Name' </summary>
        public string UserName
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary> Text of the user-entered descriptive tag </summary>
        public string Description_Tag
        {
            get { return description_tag; }
            set { description_tag = value; }
        }
    }
}
