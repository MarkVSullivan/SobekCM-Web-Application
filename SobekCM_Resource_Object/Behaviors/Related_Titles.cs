namespace SobekCM.Resource_Object.Behaviors
{
    /// <summary> Class represents titles which are related to the current title in the
    /// SobekCM database </summary>
    public class Related_Titles
    {
        /// <summary> String represents this relationship between the main title and the related title </summary>
        public readonly string Relationship;

        /// <summary> Title of the related title within this SobekCM library </summary>
        public readonly string Title;

        /// <summary> Link for the related title within this SobekCM library </summary>
        public readonly string Link;

        /// <summary> Constructor for a new instance of the Related_Titles class </summary>
        /// <param name="Relationship"> String represents this relationship between the main title and the related title</param>
        /// <param name="Title"> Title of the related title within this SobekCM library</param>
        /// <param name="Link"> Link for the related title within this SobekCM library</param>
        public Related_Titles(string Relationship, string Title, string Link)
        {
            this.Relationship = Relationship;
            this.Title = Title;
            this.Link = Link;
        }
    }
}