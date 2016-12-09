using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> Top-level table of contents information within the MODS
    /// information about a digital resource </summary>
    public class TableOfContents_Info
    {
        /// <summary> Text of the table of contents </summary>
        public string Text { get; set; }

        /// <summary> Display label associated with this table of contents </summary>
        public string DisplayLabel { get; set; }

        /// <summary> Constructor for a new instance of the TableOfContents_Info object </summary>
        public TableOfContents_Info()
        {
            // Do nothing
        }

        /// <summary> Constructor for a new instance of the TableOfContents_Info object </summary>
        /// <param name="Text"> Text of the table of contents </param>
        public TableOfContents_Info(string Text)
        {
            this.Text = Text;
        }

        /// <summary> Constructor for a new instance of the TableOfContents_Info object </summary>
        /// <param name="Text"> Text of the table of contents </param>
        /// <param name="DisplayLabel"> Display label associated with this table of contents </param>
        public TableOfContents_Info(string Text, string DisplayLabel)
        {
            this.Text = Text;
            this.DisplayLabel = DisplayLabel;
        }

        /// <summary> Adds a new content entry to this table of contents string </summary>
        /// <param name="NewContentData"> New data to add </param>
        public void Add(string NewContentData)
        {
            if (String.IsNullOrWhiteSpace(Text))
                Text = NewContentData;
            else
                Text = Text.Trim() + " ; " + NewContentData.Trim();
        }
    }
}
