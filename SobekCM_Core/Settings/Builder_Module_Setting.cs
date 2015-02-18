using System.Runtime.Serialization;

namespace SobekCM.Core.Settings
{
    /// <summary> Setting information for a single builder module </summary>
    [DataContract]
    public class Builder_Module_Setting
    {
        /// <summary> [DataMember] Name of the assembly DLL (if not a part of the standard SobekCM assembly </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Assembly { get; set; }

        /// <summary> [DataMember] Fully qualified name of the class for this builder module </summary>
        [DataMember]
        public string Class { get; set; }

        /// <summary> [DataMember] First argument necessary for the execution of this interface methods </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Argument1 { get; set; }

        /// <summary> [DataMember] Second argument necessary for the execution of this interface methods </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Argument2 { get; set; }

        /// <summary> [DataMember] Third argument necessary for the execution of this interface methods </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Argument3 { get; set; }

    }
}
