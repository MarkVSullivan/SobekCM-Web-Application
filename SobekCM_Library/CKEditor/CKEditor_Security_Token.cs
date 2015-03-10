using System;

namespace SobekCM.Library.CKEditor
{
    /// <summary> Token used to add security, via adding a key to the session
    /// state, for uploading documents using CKEditor through this system </summary>
    public class CKEditor_Security_Token
    {
        /// <summary> Path where the uploaded files should go </summary>
        public readonly string UploadPath;

        /// <summary> The GUID for this security token </summary>
        public readonly Guid ThisGuid;

        /// <summary> Constructor for a new instance of the CKEditor_Security_Token class </summary>
        /// <param name="UploadPath"> Path where the uploaded files should go </param>
        public CKEditor_Security_Token(string UploadPath )
        {
            this.UploadPath = UploadPath;
            ThisGuid = Guid.NewGuid();
        }
    }
}