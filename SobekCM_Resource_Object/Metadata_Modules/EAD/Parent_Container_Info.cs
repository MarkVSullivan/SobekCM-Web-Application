#region Using directives

using System;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.EAD
{
    /// <summary> Class stores information about the containers which hold a component object from this EAD.</summary>
    /// <remarks> This is an abstraction which replaced the strict box, folder, item structure.  Those types of containers are still the most commonly encountered though.<br /><br />
    /// This class contains information about a single level of the container hierarchy, and is generally used within a list of different types (i.e., one for the box information, one for the folder information, etc.. )</remarks>
    [Serializable]
    public class Parent_Container_Info
    {
        /// <summary> Title or label for this container </summary>
        public readonly string Container_Title;

        /// <summary> General type of this container ( usually 'box', 'folder', etc.. ) </summary>
        public readonly string Container_Type;

        /// <summary> Constructor for a new instance of the Parent_Container_Info class </summary>
        /// <param name="Container_Type"> General type of this container ( usually 'box', 'folder', etc.. )</param>
        /// <param name="Container_Title"> Title or label for this container</param>
        public Parent_Container_Info(string Container_Type, string Container_Title)
        {
            this.Container_Title = Container_Title;
            this.Container_Type = Container_Type;
        }
    }
}