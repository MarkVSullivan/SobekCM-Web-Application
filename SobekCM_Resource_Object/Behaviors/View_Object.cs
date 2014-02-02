#region Using directives

using System;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Behaviors
{
    /// <summary> Objects contains data about a particular view for this item in SobekCM </summary>
    [Serializable]
    public class View_Object : IEquatable<View_Object>
    {
        private string attributes;
        private string filename;
        private string label;
        private View_Enum view_type;

        /// <summary> Constructor for a new instance of the View_Object class </summary>
        /// <param name="View_Type">Standard type of SobekCM View</param>
        public View_Object(View_Enum View_Type)
        {
            view_type = View_Type;
        }

        /// <summary> Constructor for a new instance of the View_Object class </summary>
        /// <param name="View_Type">Standard type of SobekCM View</param>
        /// <param name="Label">Label for this SobekCM View</param>
        /// <param name="Attributes">Any additional attribures needed for thie SobekCM View</param>
        public View_Object(View_Enum View_Type, string Label, string Attributes)
        {
            view_type = View_Type;
            label = Label;
            attributes = Attributes;
        }

        /// <summary> Constructor for a new instance of the View_Object class </summary>
        /// <param name="View_Type">Standard type of SobekCM View</param>
        /// <param name="Label">Label for this SobekCM View</param>
        /// <param name="Attributes">Any additional attribures needed for this SobekCM View</param>
        /// <param name="FileName">Name of the file</param>
        public View_Object(View_Enum View_Type, string Label, string Attributes, string FileName)
        {
            view_type = View_Type;
            label = Label;
            attributes = Attributes;
            filename = FileName;
        }

        /// <summary> Gets and sets the standard type of SobekCM View </summary>
        public View_Enum View_Type
        {
            get { return view_type; }
            set { view_type = value; }
        }

        /// <summary> Gets and sets the label for this SobekCM View </summary>
        public string Label
        {
            get { return label ?? String.Empty; }
            set { label = value; }
        }

        /// <summary> Gets and sets any additional attributes needed for this SobekCM View </summary>
        public string Attributes
        {
            get { return attributes ?? String.Empty; }
            set { attributes = value; }
        }

        /// <summary> Gets and sets the filename needed for this SobekCM View </summary>
        public string FileName
        {
            get { return filename ?? String.Empty; }
            set { filename = value; }
        }

        /// <summary> Gets the SobekCM viewer code, by viewer type </summary>
        public string[] Viewer_Codes
        {
            get { return Viewer_Code_By_Type(view_type); }
        }

        #region IEquatable<View_Object> Members

        /// <summary> Checks to see if this view is equal to another view </summary>
        /// <param name="other"> Other view for comparison </param>
        /// <returns> TRUE if they are equal, otherwise FALSE </returns>
        /// <remarks> Two views are considered equal if they have the view type </remarks>
        public bool Equals(View_Object other)
        {
            if (other.View_Type == View_Type)
                return true;
            else
                return false;
        }

        #endregion

        /// <summary> Gets the SobekCM viewer code, by viewer type </summary>
        public static string[] Viewer_Code_By_Type(View_Enum View_Type)
        {
            switch (View_Type)
            {
                case View_Enum.ALL_VOLUMES:
                    return new string[] {"allvolumes", "allvolumes1", "allvolumes2", "allvolumes3"};

                case View_Enum.CITATION:
                    return new string[] {"citation", "marc", "metadata", "usage"};

				case View_Enum.DATASET_CODEBOOK:
					return new string[] { "dscodebook" };

				case View_Enum.DATASET_REPORTS:
					return new string[] { "dsreports" };

				case View_Enum.DATASET_VIEWDATA:
					return new string[] { "dsview" };

                case View_Enum.TRACKING_SHEET:
                    return new string[]{"ts"};

                case View_Enum.DOWNLOADS:
                    return new string[] {"downloads"};

				case View_Enum.EAD_CONTAINER_LIST:
					return new string[] { "container" };

				case View_Enum.EAD_DESCRIPTION:
					return new string[] { "description" };

				case View_Enum.EMBEDDED_VIDEO:
					return new string[] { "video" };

                case View_Enum.EPHEMERAL_CITIES:
                    return new string[] {"ep"};

                case View_Enum.FEATURES:
                    return new string[] {"features"};

                case View_Enum.FLASH:
                    return new string[] {"flash"};

                case View_Enum.GOOGLE_MAP:
                    return new string[] {"map", "mapsearch"};

                case View_Enum.GOOGLE_COORDINATE_ENTRY:
                    return new string[] { "mapedit" };

                case View_Enum.HTML:
                    return new string[] {"html"};

                case View_Enum.JPEG:
                    return new string[] {"j"};

				case View_Enum.JPEG_TEXT_TWO_UP:
					return new string[] { "u" };

                case View_Enum.JPEG2000:
                    return new string[] {"x"};

				case View_Enum.MANAGE:
					return new string[] { "manage" };

				case View_Enum.PAGE_TURNER:
					return new string[] { "pageturner" };

				case View_Enum.PDF:
					return new string[] { "pdf" };

				case View_Enum.QUALITY_CONTROL:
					return new string[] { "qc" };

                case View_Enum.RELATED_IMAGES:
                    return new string[] {"thumbs"};

                case View_Enum.SEARCH:
                    return new string[] {"search"};

                case View_Enum.SIMPLE_HTML_LINK:
                    return new string[] {""};

                case View_Enum.STREETS:
                    return new string[] {"streets"};

                case View_Enum.TEXT:
                    return new string[] {"t"};

                case View_Enum.TOC:
                    return new string[] {"toc"};

				case View_Enum.TRACKING:
					return new string[] { "milestones", "tracking", "directory", "media", "archive" };

                case View_Enum.YOUTUBE_VIDEO:
                    return new string[] {"youtube"};

                case View_Enum.TEST:
                    return new string[] { "test" };
            }

            return new string[] {""};
        }

        /// <summary> Returns the METS behavior associated with this viewer </summary>
        internal void Add_METS(TextWriter behaviorSec, int view_count)
        {
            if ((View_Type == View_Enum.None) || (View_Type == View_Enum.GOOGLE_MAP) || (View_Type == View_Enum.CITATION) || (View_Type == View_Enum.TOC) || (View_Type == View_Enum.PDF) || (View_Type == View_Enum.FLASH) || (View_Type == View_Enum.YOUTUBE_VIDEO))
            {
                return;
            }

            // Start this behavior
            if (view_count == 1)
            {
                behaviorSec.Write("<METS:behavior GROUPID=\"VIEWS\" ID=\"VIEW1\" STRUCTID=\"STRUCT1\" LABEL=\"Default View\">\r\n");
            }
            else
            {
                behaviorSec.Write("<METS:behavior GROUPID=\"VIEWS\" ID=\"VIEW" + view_count.ToString() + "\" STRUCTID=\"STRUCT1\" LABEL=\"Alternate View\">\r\n");
            }

            // Get the label and title for this behavior mechanism
            string temp_label = String.Empty;
            string temp_title = String.Empty;
            switch (View_Type)
            {
                case View_Enum.JPEG:
                    temp_label = "Viewer for JPEGs";
                    temp_title = "JPEG_Viewer()";
                    break;
                case View_Enum.JPEG2000:
                    temp_label = "Viewer for zoomable JPEG2000s";
                    temp_title = "JP2_Viewer()";
                    break;
                case View_Enum.TEXT:
                    temp_label = "Viewer for text";
                    temp_title = "Text_Viewer()";
                    break;
                case View_Enum.SANBORN:
                    temp_label = "Viewer for Sanborn Map objects";
                    temp_title = "Sanborn_Viewer()";
                    break;
                case View_Enum.RELATED_IMAGES:
                    temp_label = "Related image viewer shows thumbnails for each image";
                    temp_title = "Related_Image_Viewer()";
                    break;
                case View_Enum.EPHEMERAL_CITIES:
                    temp_label = "Viewer for Ephemeral Cities objects";
                    temp_title = "EPC_Viewer()";
                    break;
                case View_Enum.FEATURES:
                    temp_label = "List of Authority Features";
                    temp_title = "Features()";
                    break;
                case View_Enum.STREETS:
                    temp_label = "List of Authority Streets";
                    temp_title = "Streets()";
                    break;
                case View_Enum.HTML:
                    if (Attributes.Length > 0)
                    {
                        temp_title = "HTML_Viewer('" + Attributes + "')";
                    }
                    else
                    {
                        temp_title = "HTML_Viewer()";
                    }
                    temp_label = Label;
                    break;
                case View_Enum.PAGE_TURNER:
                    temp_label = "Page turner style viewer";
                    temp_title = "PageTurner_Viewer()";
                    break;
            }

            // Add the actual behavior mechanism
            behaviorSec.Write("<METS:mechanism LABEL=\"" + temp_label + "\" LOCTYPE=\"OTHER\" OTHERLOCTYPE=\"SobekCM Procedure\" xlink:type=\"simple\" xlink:title=\"" + temp_title + "\" />\r\n");

            // End this behavior
            behaviorSec.Write("</METS:behavior>\r\n");
        }
    }
}