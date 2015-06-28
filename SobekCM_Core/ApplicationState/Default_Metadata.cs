namespace SobekCM.Core.ApplicationState
{
    /// <summary> Very basic information about a default metadata set (global or otherwise) </summary>
    public class Default_Metadata
    {
        /// <summary> Code for this default metadata set </summary>
        public readonly string Code;

        /// <summary> Name for this default metadata set </summary>
        public readonly string Name;

        /// <summary> Full description of this default metadata set </summary>
        public readonly string Description;

        /// <summary> User for this default metadata set, if this is a personal default metadata set, and not a global set </summary>
        public readonly string User;

        /// <summary> Constructor for a new instance of the Default_Metadata class </summary>
        /// <param name="Code"> Code for this default metadata set </param>
        /// <param name="Name"> Name of this default metadata set </param>
        /// <param name="Description"> Description of this default metadata set </param>
        public Default_Metadata(string Code, string Name, string Description)
        {
            this.Code = Code;
            this.Name = Name;
            this.Description = Description;
        }

        /// <summary> Constructor for a new instance of the Default_Metadata class </summary>
        /// <param name="Code"> Code for this default metadata set </param>
        /// <param name="Name"> Name of this default metadata set </param>
        /// <param name="Description"> Description of this default metadata set </param>
        /// <param name="User"> User for this default metadata set, if this is a personal default metadata set, and not a global set </param>
        public Default_Metadata(string Code, string Name, string Description, string User)
        {
            this.Code = Code;
            this.Name = Name;
            this.Description = Description;
            this.User = User;
        }
    }
}
