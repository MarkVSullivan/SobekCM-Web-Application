namespace SobekCM.Core.ApplicationState
{
    /// <summary> Basic information about a template from the database </summary>
    /// <remarks> This does not contain the actual template element objects, just the fields from the database </remarks>
    public class Template
    {
        /// <summary> Code for this template, which must match the filename for the template XML file </summary>
        public readonly string Code;

        /// <summary> Name for this template </summary>
        public readonly string Name;

        /// <summary> Description for this template </summary>
        public readonly string Description;

        /// <summary> Constructor for a new instance of the Template class </summary>
        /// <param name="Code"> Code for this template, which must match the filename for the template XML file </param>
        /// <param name="Name"> Name for this template </param>
        /// <param name="Description"> Description for this template </param>
        public Template(string Code, string Name, string Description )
        {
            this.Code = Code;
            this.Name = Name;
            this.Description = Description;
        }
    }
}
