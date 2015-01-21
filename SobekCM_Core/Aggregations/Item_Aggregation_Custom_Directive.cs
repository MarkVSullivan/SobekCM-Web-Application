#region Using directives

using System;
using System.Runtime.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Aggregations
{
    /// <summary> Custom derivative information which allows a custom derivative
    /// to be replaced with HTML read from a source file, in the home page, info
    /// pages, and browse pages </summary>
    [Serializable, DataContract, ProtoContract]
    public class Item_Aggregation_Custom_Directive
    {
        /// <summary> Custom derivative code to be replaced with the replacement HTML </summary>
        [DataMember(Name = "code"), ProtoMember(1)]
        public readonly string Code;

        /// <summary> Replacement HTML from the source file </summary>
        [DataMember(Name = "replacement"), ProtoMember(2)]
        public readonly string Replacement_HTML;

        /// <summary> Source file for the replacement HTML </summary>
        [DataMember(Name = "file"), ProtoMember(3)]
        public readonly string Source_File;

        /// <summary> Constructor for a new instance of the Item_Aggregation_Custom_Derivative class </summary>
        /// <param name="Code"> Custom derivative code to be replaced with the replacement HTML </param>
        /// <param name="Source_File"> Source file for the replacement HTML </param>
        /// <param name="Replacement_HTML"> Replacement HTML from the source file </param>
        public Item_Aggregation_Custom_Directive(string Code, string Source_File, string Replacement_HTML)
        {
            this.Code = Code;
            this.Source_File = Source_File;
            this.Replacement_HTML = Replacement_HTML;
        }
    }
}
