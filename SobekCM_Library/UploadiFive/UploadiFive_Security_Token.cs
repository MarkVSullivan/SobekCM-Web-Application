#region Using directives

using System;

#endregion

namespace SobekCM.Library.UploadiFive
{
	/// <summary> Token used to add security, via adding a key to the session
	/// state, for uploading documents using UploadiFive through this system </summary>
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

		/// <summary> Name for the final server-side file, which allows overriding the default name,
		/// which would otherwise match the uploaded name </summary>
		/// <remarks> This can be used to avoid having to manually rename the file after upload </remarks>
		public readonly string ServerSideFileName;

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

		/// <summary> Constructor for a new instance of the UploadiFive_Security_Token class </summary>
		/// <param name="UploadPath"> Path where the uploaded files should go </param>
		/// <param name="AllowedFileExtensions"> List of file extensions allowed </param>
		/// <param name="FileObjName"> The name of the file object to use in your server-side script </param>
		/// <param name="ServerSideFileName"> Name for the final server-side file, which allows overriding the default name,
		/// which would otherwise match the uploaded name</param>
		public UploadiFive_Security_Token(string UploadPath, string AllowedFileExtensions, string FileObjName, string ServerSideFileName )
		{
			this.UploadPath = UploadPath;
			this.AllowedFileExtensions = AllowedFileExtensions;
			this.FileObjName = FileObjName;
			this.ServerSideFileName = ServerSideFileName;
			ThisGuid = Guid.NewGuid();
		}
	}
}
