#region Using directives

using System;
using System.Collections.Generic;

#endregion

namespace SobekCM.Resource_Object.Divisions
{
    /// <summary> Page node in a strucutral map tree associated with a digital resource</summary>
    /// <remarks> This class extends the <see cref="abstract_TreeNode"/> class. <br /> <br /> 
    /// Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center. </remarks>
    [Serializable]
    public class Page_TreeNode : abstract_TreeNode
    {
        private List<SobekCM_File_Info> files;

        /// <summary> Constructor creates an empty instance of the Page_TreeNode class </summary>
        public Page_TreeNode() : base("Page", String.Empty)
        {
            files = new List<SobekCM_File_Info>();
        }

        /// <summary> Constructor creates a new instance of the Page_TreeNode class </summary>
        /// <param name="Label">Node label</param>
        public Page_TreeNode(string Label) : base("Page", Label)
        {
            files = new List<SobekCM_File_Info>();

            // If the label is just 'page'
            if (Label.ToUpper() == "PAGE")
            {
                label = String.Empty;
            }
        }

        /// <summary> Gets the flag indicating if this is a page node or not </summary>
        /// <value>Always returns 'TRUE'</value>
        public override bool Page
        {
            get { return true; }
        }

        /// <summary> Gets the collection of files under this page </summary>
        /// <remarks> This returns a generic list which is a collection of <see cref="SobekCM_File_Info"/> objects. </remarks>
        public List<SobekCM_File_Info> Files
        {
            get { return files; }
        }
    }
}