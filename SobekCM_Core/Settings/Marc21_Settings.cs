using System.Runtime.Serialization;

namespace SobekCM.Core.Settings
{
    /// <summary> Settings related to the generation of Marc21 files within the system and the MarcXML feed </summary>
    [DataContract]
    public class Marc21_Settings
    {
        /// <summary> Cataloging source code for the 040 field, ( for example FUG for University of Florida ) </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Cataloging_Source_Code { get; set; }

        /// <summary> Place of reproduction, or primary location associated with the SobekCM instance ( for the added 533 |b field ) </summary>
        /// <remarks> This 533 is not added for born digital items </remarks>
        [DataMember(EmitDefaultValue = false)]
        public string Reproduction_Place { get; set; }

        /// <summary> Agency responsible for reproduction, or primary agency associated with the SobekCM instance ( for the added 533 |c field )</summary>
        /// <remarks> This 533 is not added for born digital items </remarks>
        [DataMember(EmitDefaultValue = false)]
        public string Reproduction_Agency { get; set; }

        /// <summary> Location code for the 852 |a - if none is given the system abbreviation will be used. Otherwise, the system abbreviation will be put in the 852 |b field. </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Location_Code { get; set; }

        /// <summary> XSLT file to use as a final transform, after the standard MarcXML file is written </summary>
        /// <remarks> This only affects generated MarcXML ( for the feeds and OAI ) not the dispayed in-system MARC ( as of January 2015 ).  This file should appear in the config/users folder. </remarks>
        [DataMember(EmitDefaultValue = false)]
        public string XSLT_File{ get; set; }
        
        /// <summary> Location where the MarcXML feeds should be placed </summary>
        [DataMember(EmitDefaultValue = false)]
        public string MarcXML_Feed_Location { get; set; }

        /// <summary> Flag indicates if the MARC feed should be built by default by the bulk loader </summary>
        [DataMember]
        public bool Build_MARC_Feed_By_Default { get; set; }
    }
}
