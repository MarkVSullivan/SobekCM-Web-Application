#region Using directives

using System;
using System.Runtime.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Aggregations
{
    /// <summary> Data about a single thematic heading, used to organize item aggregationPermissions on the main home page </summary>
    [Serializable, DataContract, ProtoContract]
    public class Thematic_Heading
    {
        /// <summary> Primary key for this thematic heading in the database </summary>
        [DataMember(Name = "id"), ProtoMember(3)]
        public readonly int ID;

        /// <summary> Display name for this thematic heading </summary>
        [DataMember(Name = "name"), ProtoMember(3)]
        public readonly string Text;

        /// <summary> Constructor for a new instance of the Thematic_Heading class </summary>
        /// <param name="ID"> Primary key for this thematic heading in the database</param>
        /// <param name="Text"> Display name for this thematic heading</param>
        public Thematic_Heading(int ID, string Text)
        {
            this.ID = ID;
            this.Text = Text;
        }
    }
}
