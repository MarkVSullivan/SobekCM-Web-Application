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

        /// <summary> Constructor for a new instance of the View_Object class </summary>
        /// <param name="View_Type">Standard type of SobekCM View</param>
        public View_Object(string View_Type)
        {
            this.View_Type = View_Type;
        }

        /// <summary> Constructor for a new instance of the View_Object class </summary>
        /// <param name="View_Type">Standard type of SobekCM View</param>
        /// <param name="Label">Label for this SobekCM View</param>
        /// <param name="Attributes">Any additional attribures needed for thie SobekCM View</param>
        public View_Object(string View_Type, string Label, string Attributes)
        {
            this.View_Type = View_Type;
            label = Label;
            attributes = Attributes;
        }

        /// <summary> Gets and sets the standard type of SobekCM View </summary>
        public string View_Type { get; set; }

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

        /// <summary> Order this displays in the item main menu </summary>
        public float MenuOrder { get; set; }

        /// <summary> Flag indicates this viewers is explicitly excluded from this item </summary>
        public bool Exclude { get; set; }


        #region IEquatable<View_Object> Members

        /// <summary> Checks to see if this view is equal to another view </summary>
        /// <param name="Other"> Other view for comparison </param>
        /// <returns> TRUE if they are equal, otherwise FALSE </returns>
        /// <remarks> Two views are considered equal if they have the view type </remarks>
        public bool Equals(View_Object Other)
        {
            return Other.View_Type == View_Type;
        }

        #endregion

        /// <summary> Returns the METS behavior associated with this viewer </summary>
        /// <param name="Results"> Results stream to write the METS-encoded serial information </param>
        /// <param name="ViewCount"> Number of this view, as they are added </param>
        internal void Add_METS(TextWriter Results, int ViewCount)
        {
            if ( String.IsNullOrEmpty(View_Type))
            {
                return;
            }

            // Start this behavior
            if (ViewCount == 1)
            {
                Results.Write("<METS:behavior GROUPID=\"VIEWS\" ID=\"VIEW1\" STRUCTID=\"STRUCT1\" LABEL=\"Default View\">\r\n");
            }
            else
            {
                Results.Write("<METS:behavior GROUPID=\"VIEWS\" ID=\"VIEW" + ViewCount.ToString() + "\" STRUCTID=\"STRUCT1\" LABEL=\"Alternate View\">\r\n");
            }

            // Add the actual behavior mechanism
            Results.Write("<METS:mechanism LOCTYPE=\"OTHER\" OTHERLOCTYPE=\"SobekCM Procedure\" xlink:type=\"simple\" />\r\n");

            // End this behavior
            Results.Write("</METS:behavior>\r\n");
        }
    }
}