#region Using directives

using System;
using System.Runtime.Serialization;
using System.Text;

#endregion

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

        /// <summary> Key for this setting, used to lookup for duplicate references to the same module </summary>
        [IgnoreDataMember]
        public string Key
        {
            get
            {
                // Get the assembly/class/arguments string for lookup
                StringBuilder builder = new StringBuilder(1000);
                if (!String.IsNullOrEmpty(Assembly))
                    builder.Append(Assembly + "|");
                else
                    builder.Append("|");
                builder.Append(Class);
                if (!String.IsNullOrEmpty(Argument1))
                    builder.Append(Argument1 + "|");
                else
                    builder.Append("|");
                if (!String.IsNullOrEmpty(Argument2))
                    builder.Append(Argument2 + "|");
                else
                    builder.Append("|");
                if (!String.IsNullOrEmpty(Argument3))
                    builder.Append(Argument3);
                return builder.ToString();
            }
        }

    }
}
