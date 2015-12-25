using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.Configuration.Authentication;
using SobekCM.Core.Configuration.Engine;
using SobekCM.Core.Configuration.Extensions;
using SobekCM.Core.Configuration.OAIPMH;
using SobekCM.Core.UI_Configuration;
using SobekCM.Resource_Object.Configuration;

namespace SobekCM.Core.Configuration
{
    /// <summary> Class provides access to all the configuration information loaded from the
    /// engine-side configuration files </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("Configurations")]
    public class InstanceWide_Configuration
    {
        /// <summary> Constructor for a new instance of the InstanceWide_Configuration class </summary>
        public InstanceWide_Configuration()
        {
            // Create some of the configuration stuff
            Authentication = new Authentication_Configuration();
            BriefItemMapping = new BriefItemMapping_Configuration();
            ContactForm = new ContactForm_Configuration();
            Engine = new Engine_Server_Configuration();
            Extensions = new Extension_Configuration();
            QualityControlTool = new QualityControl_Configuration();
            Metadata = new Metadata_Configuration();
            OAI_PMH = new OAI_PMH_Configuration();
            UI = new InstanceWide_UI_Configuration();

            // Set some defaults
            HasData = false;
            ReadingLog = new List<string>();
            ErrorEncountered = false;
            LatestDateTimeStamp = new DateTime(2000,1,1);
        }

        /// <summary> Flag indicates if the data has been pulled into this </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public bool HasData { get; set; }

        /// <summary> Log file keeps the log of the attempt to read all of the
        /// configuration files, just in case there is an error, or the log is requested </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public List<string>  ReadingLog { get; set; }

        /// <summary> Flag indicates if an error was encountered while reading the configuration
        /// files. </summary>
        /// <remarks> The actual error (and other entries) will appear in the ReadingLog property </remarks>
        [XmlIgnore]
        [IgnoreDataMember]
        public bool ErrorEncountered { get; set; }

        /// <summary> Add an empty line to the reading log  </summary>
        public void Add_Log()
        {
            if (ReadingLog == null) ReadingLog = new List<string>();

            ReadingLog.Add(String.Empty);
        }

        /// <summary> Add a reading log line to the log  </summary>
        /// <param name="LogLine"> Information to add to the log </param>
        public void Add_Log(string LogLine)
        {
            if (ReadingLog == null) ReadingLog = new List<string>();

            ReadingLog.Add( DateTime.Now.Hour.ToString().PadLeft(2,'0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2,'0') + ":" + DateTime.Now.Second.ToString().PadLeft(2,'0') + "." + DateTime.Now.Millisecond.ToString().PadLeft(3,'0') + " - " + LogLine);
        }

        /// <summary> Most recent timestamp on any of the configuration files </summary>
        [DataMember(Name = "timestamp", EmitDefaultValue = false)]
        [XmlAttribute("timestamp")]
        [ProtoMember(1)]
        public DateTime LatestDateTimeStamp { get; set; }

        /// <summary> Configuration for authentication for this instance </summary>
        [DataMember(Name = "authentication", EmitDefaultValue = false)]
        [XmlElement("authentication")]
        [ProtoMember(2)]
        public Authentication_Configuration Authentication { get; set; }

        /// <summary> Configuration for the mapping between the main SobekCM_Item object
        /// and the BriefItem object used by the item viewers and passed over the REST API </summary>
        [DataMember(Name = "briefItemMapping", EmitDefaultValue = false)]
        [XmlElement("briefItemMapping")]
        [ProtoMember(3)]
        public BriefItemMapping_Configuration BriefItemMapping { get; set; }

        /// <summary> Configuration for the default contact form for this instance </summary>
        [DataMember(Name = "contactForm", EmitDefaultValue = false)]
        [XmlElement("contactForm")]
        [ProtoMember(4)]
        public ContactForm_Configuration ContactForm { get; set; }

        /// <summary> Configuration for the engine endpoints exposed for this instance </summary>
        [DataMember(Name = "engine", EmitDefaultValue = false)]
        [XmlElement("engine")]
        [ProtoMember(5)]
        public Engine_Server_Configuration Engine { get; set; }

        /// <summary> Configuration for the engine endpoints exposed for this instance </summary>
        [DataMember(Name = "extensions", EmitDefaultValue = false)]
        [XmlElement("extensions")]
        [ProtoMember(6)]
        public Extension_Configuration Extensions { get; set; }

        /// <summary> Configuration information regarding how to read and write metadata files in the system </summary>
        [DataMember(Name = "metadata", EmitDefaultValue = false)]
        [XmlElement("metadata")]
        [ProtoMember(7)]
        public Metadata_Configuration Metadata { get; set; }

        /// <summary> Configuration for instance-wide OAI-PMH settings for this instance </summary>
        [DataMember(Name = "oai-pmh", EmitDefaultValue = false)]
        [XmlElement("oai-pmh")]
        [ProtoMember(8)]
        public OAI_PMH_Configuration OAI_PMH { get; set; }

        /// <summary> Configuration for the quality control tool for this instance </summary>
        [DataMember(Name = "qcConfig", EmitDefaultValue = false)]
        [XmlElement("qcConfig")]
        [ProtoMember(9)]
        public QualityControl_Configuration QualityControlTool { get; set; }

        /// <summary> Configuration for the user-inteface specific configurations for this instance </summary>
        /// <remarks> This property is not serialized </remarks>
        [XmlIgnore]
        [IgnoreDataMember]
        public InstanceWide_UI_Configuration UI { get; set; }

    }
}
