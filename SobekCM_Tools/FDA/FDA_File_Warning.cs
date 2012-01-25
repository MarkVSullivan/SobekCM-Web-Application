#region Using directives

using System;

#endregion

namespace SobekCM.Tools.FDA
{
    /// <summary> Class stores the basic information about a file-level warning in a FDA report </summary>
    /// <remarks> This class is used within a single <see cref="FDA_File"/> object within a complete <see cref="FDA_Report_Data" /> object and is not generally used alone.</remarks>
    /// <example> For examples, see the example under the <see cref="FDA_Report_Data" /> class.</example>
    public class FDA_File_Warning
    {
        /// <summary> Constructor creates a new instance of the FDA_File_Warning class </summary>
        public FDA_File_Warning()
        {
            Code = String.Empty;
            Text = String.Empty;
        }

        /// <summary> Constructor creates a new instance of the FDA_File_Warning class </summary>
        /// <param name="Code"> Warning code for this file-level warning </param>
        /// <param name="Text"> Warning text for this file-level warning </param>
        public FDA_File_Warning(string Code, string Text)
        {
            this.Code = Code;
            this.Text = Text;
        }

        /// <summary> Gets or sets the code for this file-level warning </summary>
        public string Code { get; set; }

        /// <summary> Gets or sets the text for this file-level warning </summary>
        public string Text { get; set; }
    }
}
