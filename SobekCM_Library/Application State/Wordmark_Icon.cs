namespace SobekCM.Library.Application_State
{
    /// <summary> Lookup information about a single wordmark/icon including image, link, and title </summary>
    public class Wordmark_Icon
    {
        /// <summary> Metadata code for this wordmark/icon</summary>
        public readonly string Code;

        /// <summary> Name of the image file for this wordmark/icon</summary>
        public readonly string Image_FileName;

        /// <summary> Link related to this wordmark/icon</summary>
        public readonly string Link;

        /// <summary> Title for this wordmark/icon which appears when you hover over the image</summary>
        public readonly string Title;

        /// <summary> Constructor for a new instance of the Wordmark_Icon class </summary>
        /// <param name="Code"> Metadata code for this wordmark/icon</param>
        /// <param name="Image_FileName"> Name of the image file for this wordmark/icon</param>
        /// <param name="Link"> Link related to this wordmark/icon</param>
        /// <param name="Title"> Title for this wordmark/icon which appears when you hover over the image</param>
        public Wordmark_Icon(string Code, string Image_FileName, string Link, string Title)
        {
            this.Code = Code;
            this.Image_FileName = Image_FileName;
            this.Link = Link;
            this.Title = Title;
        }
    }
}
