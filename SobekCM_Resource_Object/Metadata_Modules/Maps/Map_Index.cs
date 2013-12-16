#region Using directives

using System;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.Maps
{
    /// <summary> Contains all the information about a graphical index to a map set, such as the Sanborn maps </summary>
    [Serializable]
    public class Map_Index
    {
        private string html_file;
        private string image_file;
        private long indexid;
        private string title;
        private string type;

        /// <summary> Constructor for a new instance of the Map_Index class </summary>
        public Map_Index()
        {
            indexid = -1;
            title = String.Empty;
            image_file = String.Empty;
            html_file = String.Empty;
            type = String.Empty;
        }

        /// <summary> Constructor for a new instance of the Map_Index class </summary>
        /// <param name="IndexID"> Primary key for this map index </param>
        /// <param name="Title"> Title of this map index  </param>
        /// <param name="Image_File"> Name of the image file for this map index </param>
        /// <param name="HTML_File"> Name of the html map file related to the image file </param>
        /// <param name="Type"> Type of index </param>
        public Map_Index(long IndexID, string Title, string Image_File, string HTML_File, string Type)
        {
            indexid = IndexID;
            title = Title;
            image_file = Image_File;
            html_file = HTML_File;
            type = Type;
        }

        /// <summary> Primary key for this map index </summary>
        public long IndexID
        {
            get { return indexid; }
            set { indexid = value; }
        }

        /// <summary>  Title of this map index  </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>  Name of the image file for this map index  </summary>
        public string Image_File
        {
            get { return image_file; }
            set { image_file = value; }
        }

        /// <summary>  Name of the html map file related to the image file  </summary>
        public string HTML_File
        {
            get { return html_file; }
            set { html_file = value; }
        }

        /// <summary>  Type of index  </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
    }
}