using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Library.UploadiFive
{
	/// <summary> Token used to add security, via adding a key to the session
	/// state, for uploading documents through this system </summary>
	public class UploadiFive_Security_Token
	{
		/// <summary> Path where the uploaded files should go </summary>
		public readonly string UploadPath;

		/// <summary> List of file extensions allowed </summary>
		public readonly string AllowedFileExtensions;

		/// <summary> The name of the file object to use in your server-side script </summary>
		public readonly string FileObjName;

		/// <summary> The GUID for this security token </summary>
		public readonly Guid ThisGuid;



		/// <summary> Constructor for a new instance of the UploadiFive_Security_Token class </summary>
		/// <param name="UploadPath"> Path where the uploaded files should go </param>
		/// <param name="AllowedFileExtensions"> List of file extensions allowed </param>
		/// <param name="FileObjName"> The name of the file object to use in your server-side script </param>
		public UploadiFive_Security_Token(string UploadPath, string AllowedFileExtensions, string FileObjName )
		{
			this.UploadPath = UploadPath;
			this.AllowedFileExtensions = AllowedFileExtensions;
			this.FileObjName = FileObjName;
			ThisGuid = Guid.NewGuid();

		}
	}
}
