namespace SobekCM.Library.Aggregations
{
    /// <summary> Data about a single thematic heading, used to organize item aggregations on the main home page </summary>
    public class Thematic_Heading
    {
        /// <summary> Primary key for this thematic heading in the database </summary>
        public readonly int ThematicHeadingID;

        /// <summary> Display name for this thematic heading </summary>
        public readonly string ThemeName;

        /// <summary> Constructor for a new instance of the Thematic_Heading class </summary>
        /// <param name="ThematicHeadingID"> Primary key for this thematic heading in the database</param>
        /// <param name="ThemeName"> Display name for this thematic heading</param>
        public Thematic_Heading(int ThematicHeadingID, string ThemeName)
        {
            this.ThematicHeadingID = ThematicHeadingID;
            this.ThemeName = ThemeName;
        }
    }
}
