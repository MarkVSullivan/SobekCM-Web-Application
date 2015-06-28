#region Using directives

using System;

#endregion

namespace SobekCM.Resource_Object.Behaviors
{
    /// <summary> Class represents the information about a single wordmark </summary>
    [Serializable]
    public class Wordmark_Info : IEquatable<Wordmark_Info>
    {
        private string code;
        private string html;
        private string link;
        private string title;

        /// <summary> Constructor for a new instance of the Wordmark_Info class </summary>
        public Wordmark_Info()
        {
            // Do nothing
        }

        /// <summary> Constructor for a new instance of the Wordmark_Info class </summary>
        /// <param name="Code">Code for this icon</param>
        public Wordmark_Info(string Code)
        {
            code = Code;
        }

        /// <summary> Gets and sets the code for this wordmark </summary>
        public string Code
        {
            get { return code ?? String.Empty; }
            set { code = value; }
        }

        /// <summary> Gets and sets the title for this wordmark </summary>
        public string Title
        {
            get { return title ?? String.Empty; }
            set { title = value; }
        }

        /// <summary> Gets and sets the html for this wordmark </summary>
        public string HTML
        {
            get { return html ?? String.Empty; }
            set { html = value; }
        }

        /// <summary> Gets and sets the link for this wordmark </summary>
        public string Link
        {
            get { return link ?? String.Empty; }
            set { link = value; }
        }

        #region IEquatable<Wordmark_Info> Members

        /// <summary> Checks to see if this wordmark/icon is equal to another wordmark/icon </summary>
        /// <param name="Other"> Other wordmark/icon to verify equality with </param>
        /// <returns> TRUE if they equal, otherwise FALSE</returns>
        /// <remarks> Two wordmark/icons are considered equal if their codes are identical </remarks>
        public bool Equals(Wordmark_Info Other)
        {
            return String.Compare(Other.Code, Code, StringComparison.OrdinalIgnoreCase) == 0;
        }

        #endregion
    }
}