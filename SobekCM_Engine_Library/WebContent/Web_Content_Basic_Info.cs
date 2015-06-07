namespace SobekCM.Engine_Library.WebContent
{
    /// <summary> Basic information, from the database, for a web content page </summary>
    public class Web_Content_Basic_Info
    {
        /// <summary> Primary key for this web content page, from the database </summary>
        public readonly int WebContentID;

        /// <summary> Title for this web content page, from the database </summary>
        /// <remarks> Generally, this is used for auto-creating sitemaps and reporting purposes </remarks>
        public readonly string Title;

        /// <summary> Summary for this web content page, from the database </summary>
        /// <remarks> Generally, this is used for auto-creating sitemaps </remarks>
        public readonly string Summary;

        /// <summary> Flag indicates if this web content page is currently deleted within the database </summary>
        public readonly bool Deleted;

        /// <summary> Constructor for a new instance of the Web_Content_Basic_Info class </summary>
        /// <param name="WebContentID"> Primary key for this web content page, from the database </param>
        /// <param name="Title"> Title for this web content page, from the database </param>
        /// <param name="Summary"> Summary for this web content page, from the database </param>
        /// <param name="Deleted"> Flag indicates if this web content page is currently deleted within the database </param>
        public Web_Content_Basic_Info(int WebContentID, string Title, string Summary, bool Deleted)
        {
            this.WebContentID = WebContentID;
            this.Title = Title;
            this.Summary = Summary;
            this.Deleted = Deleted;
        }
    }
}
