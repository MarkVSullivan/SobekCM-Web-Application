using System;

namespace SobekCM.Library.CKEditor
{
    /// <summary> Token used to add security, via adding a key to the session
    /// state, for uploading documents using CKEditor through this system </summary>
    public class CKEditor_Security_Token
    {
        /// <summary> Path where the uploaded files should go </summary>
        public readonly string UploadPath;

        /// <summary> URL where the uploaded files go, to return the uploaded file URL </summary>
        public readonly string UploadURL;

        /// <summary> The GUID for this security token </summary>
        public readonly Guid ThisGuid;

        /// <summary> Constructor for a new instance of the CKEditor_Security_Token class </summary>
        /// <param name="UploadPath"> Path where the uploaded files should go </param>
        /// <param name="UploadURL"> URL where the uploaded files go, to return the uploaded file URL </param>
        public CKEditor_Security_Token(string UploadPath, string UploadURL )
        {
            this.UploadPath = UploadPath;
            this.UploadURL = UploadURL;
            ThisGuid = Guid.NewGuid();
        }
    }
}