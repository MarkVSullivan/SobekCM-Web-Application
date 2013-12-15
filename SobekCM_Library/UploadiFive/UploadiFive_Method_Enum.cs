
namespace SobekCM.Library.UploadiFive
{
	/// <summary> Enumeration for the type of method to use when submitting the form </summary>
	public enum UploadiFive_Method_Enum : byte
	{
		/// <summary> FormData is sents as a HTML post to the server </summary>
		Post,

		/// <summary> FormData values are sent via the querystring </summary>
		/// <remarks> This is not supported by the related ASP.net classes </remarks>
		Get
	}
}