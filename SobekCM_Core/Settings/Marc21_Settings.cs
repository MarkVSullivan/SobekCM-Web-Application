#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Settings
{
    /// <summary> Settings related to the generation of Marc21 files within the system and the MarcXML feed </summary>
    [Serializable, DataContract, ProtoContract]
    public class Marc21_Settings
    {
        /// <summary> Cataloging source code for the 040 field, ( for example FUG for University of Florida ) </summary>
        [DataMember(Name="catalogingSourceCode", EmitDefaultValue = false)]
        [XmlElement("catalogingSourceCode")]
        [ProtoMember(1)]
        public string Cataloging_Source_Code { get; set; }

        /// <summary> Place of reproduction, or primary location associated with the SobekCM instance ( for the added 533 |b field ) </summary>
        /// <remarks> This 533 is not added for born digital items </remarks>
        [DataMember(Name = "reproductionPlace", EmitDefaultValue = false)]
        [XmlElement("reproductionPlace")]
        [ProtoMember(2)]
        public string Reproduction_Place { get; set; }

        /// <summary> Agency responsible for reproduction, or primary agency associated with the SobekCM instance ( for the added 533 |c field )</summary>
        /// <remarks> This 533 is not added for born digital items </remarks>
        [DataMember(Name = "reproductionAgency", EmitDefaultValue = false)]
        [XmlElement("reproductionAgency")]
        [ProtoMember(3)]
        public string Reproduction_Agency { get; set; }

        /// <summary> Location code for the 852 |a - if none is given the system abbreviation will be used. Otherwise, the system abbreviation will be put in the 852 |b field. </summary>
        [DataMember(Name = "locationCode", EmitDefaultValue = false)]
        [XmlElement("locationCode")]
        [ProtoMember(4)]
        public string Location_Code { get; set; }

        /// <summary> XSLT file to use as a final transform, after the standard MarcXML file is written </summary>
        /// <remarks> This only affects generated MarcXML ( for the feeds and OAI ) not the dispayed in-system MARC ( as of January 2015 ).  This file should appear in the config/users folder. </remarks>
        [DataMember(Name = "xsltFile", EmitDefaultValue = false)]
        [XmlElement("xsltFile")]
        [ProtoMember(5)]
        public string XSLT_File{ get; set; }
        
        /// <summary> Location where the MarcXML feeds should be placed </summary>
        [DataMember(Name = "marcXmlFeedLocation", EmitDefaultValue = false)]
        [XmlAttribute("marcXmlFeedLocation")]
        [ProtoMember(6)]
        public string MarcXML_Feed_Location { get; set; }

        /// <summary> Flag indicates if the MARC feed should be built by default by the bulk loader </summary>
        [DataMember(Name = "buildMarcFeed")]
        [XmlAttribute("buildMarcFeed")]
        [ProtoMember(7)]
        public bool Build_MARC_Feed_By_Default { get; set; }

        #region Methods that controls XML serialization

        /// <summary> Method suppresses XML Serialization of the Cataloging_Source_Code property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeCataloging_Source_Code()
        {
            return (!String.IsNullOrEmpty(Cataloging_Source_Code));
        }

        /// <summary> Method suppresses XML Serialization of the Reproduction_Place property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeReproduction_Place()
        {
            return (!String.IsNullOrEmpty(Reproduction_Place));
        }

        /// <summary> Method suppresses XML Serialization of the Reproduction_Agency property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeReproduction_Agency()
        {
            return (!String.IsNullOrEmpty(Reproduction_Agency));
        }

        /// <summary> Method suppresses XML Serialization of the Location_Code property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeLocation_Code()
        {
            return (!String.IsNullOrEmpty(Location_Code));
        }

        /// <summary> Method suppresses XML Serialization of the XSLT_File property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeXSLT_File()
        {
            return (!String.IsNullOrEmpty(XSLT_File));
        }

        #endregion

    }
}
