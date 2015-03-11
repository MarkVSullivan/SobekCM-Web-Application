using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;
using SobekCM.Core.Configuration;

namespace SobekCM.Core.Skins
{
    /// <summary> Complete source information for a web skin object, which determines the header, footer, stylesheet, and other design elements for the rendered HTML </summary>
    /// <remarks> This contains links to all the source files used for every language, but does not contain any of the actual HTML to include in the 
    /// output stream.  During normal use, a language-specific <see cref="Web_Skin_Object" /> instance is used.</remarks>
    [Serializable,DataContract,ProtoContract]
    public class Complete_Web_Skin_Object
    {
        /// <summary> Code for this skin </summary>
        /// <remarks> This also corresponds to the location of the main interface files under the design folder.  (i.e., '\design\skins\[CODE]' ) </remarks>
        [DataMember(Name = "code")]
        [ProtoMember(1)]
        public readonly string Skin_Code;

        /// <summary> Additional CSS Stylesheet to be included for this skin </summary>
        /// <remarks> The standard SobekCM stylesheet is always included, but this stylesheet can override any styles from the standard </remarks>
        [DataMember(Name = "cssStyle")]
        [ProtoMember(2)]
        public readonly string CSS_Style;

        /// <summary> Constructor for a new instance of the Complete_Web_Skin_Object class </summary>
        /// <param name="Skin_Code"> Code for this HTML skin</param>
        /// <param name="Base_Skin_Code"> Code for the base HTML skin which this skin derives from</param>
        /// <param name="CSS_Style"> Additional CSS Stylesheet to be included for this HTML skin</param>
        public Complete_Web_Skin_Object(string Skin_Code, string CSS_Style)
        {
            // Save the parameters
            this.CSS_Style = CSS_Style;
            this.Skin_Code = Skin_Code;
            Override_Banner = false;

            SourceFiles = new Dictionary<Web_Language_Enum, Complete_Web_Skin_Source_Files>();
        }

        /// <summary> Code for the base skin which this skin derives from  </summary>
        /// <remarks> The base skin is used for many of the common design image files which are reused, such as button images, tab images, etc..<br /><br />
        /// This also corresponds to the location of the base skin files under the design folder.  (i.e., '\design\skins\[CODE]' ) </remarks>
        [DataMember(EmitDefaultValue = false, Name = "base")]
        [ProtoMember(3)]
        public string Base_Skin_Code { get; set; }

        /// <summary>  Flag indicates if the top-level aggregation navigation should be suppressed for this web skin ( i.e., is the top-level navigation embedded into the header file already? ) </summary>
        [DataMember(Name = "suppressTopNav")]
        [ProtoMember(4)]
        public bool Suppress_Top_Navigation { get; set; }

        /// <summary> Flag indicates if this skin has a banner which should override any aggregation-specific banner </summary>
        [DataMember(Name = "overrideBanner")]
        [ProtoMember(5)]
        public bool Override_Banner { get; set; }

        /// <summary> Link for the banner, if there is a banner set to override the aggregation-specific banner </summary>
        [DataMember(EmitDefaultValue = false, Name = "bannerLink")]
        [ProtoMember(6)]
        public string Banner_Link { get; set; }

        /// <summary> Notes associated with this web skin </summary>
        [DataMember(EmitDefaultValue = false, Name = "notes")]
        [ProtoMember(7)]
        public string Notes { get; set; }

        /// <summary> Collection of the source files for every language supported by this web skin </summary>
        [DataMember(EmitDefaultValue = false, Name = "sourceByLanguage")]
        [ProtoMember(8)]
        public Dictionary<Web_Language_Enum, Complete_Web_Skin_Source_Files> SourceFiles { get; set; }

    }
}
