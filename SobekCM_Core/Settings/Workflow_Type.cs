#region Using directives

using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.Settings
{
    /// <summary> Information about a single workflow type used for tracking all work that occurs against
    /// a single digital resource  </summary>
    [DataContract]
    public class Workflow_Type
    {
        /// <summary> Workflow type name, such as 'Scanned', 'Online Metadata Edit', etc.. (default language) </summary>
        [DataMember]
        public readonly string Name;

        /// <summary> Key to this workflow type </summary>
        [DataMember]
        public readonly int Key;

        /// <summary> Constructor for a new instance of the Workflow_Type class </summary>
        /// <param name="Key"> Key to this workflow type</param>
        /// <param name="Name">Workflow type name, such as 'Scanned', 'Online Metadata Edit', etc.. (default language)</param>
        public Workflow_Type(int Key, string Name)
        {
            this.Key = Key;
            this.Name = Name;
        }
    }
}
