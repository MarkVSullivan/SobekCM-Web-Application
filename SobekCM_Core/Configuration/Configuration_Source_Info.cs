using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Configuration
{
    /// <summary> Basic information on where all the configuration information was read from
    /// as well as any reading errors which may have occurred </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ConfigurationSource")]
    public class Configuration_Source_Info
    {
        public Configuration_Source_Info()
        {
            ReadingLog = new List<string>();
            ErrorEncountered = false;
            LatestDateTimeStamp = new DateTime(2000, 1, 1);
            Files = new List<string>();
        }

        /// <summary> Flag indicates if an error was encountered while reading the configuration
        /// files. </summary>
        /// <remarks> The actual error (and other entries) will appear in the ReadingLog property </remarks>
        [DataMember(Name = "error", EmitDefaultValue = false)]
        [XmlAttribute("error")]
        [ProtoMember(1)]
        public bool ErrorEncountered { get; set; }

        /// <summary> Log file keeps the log of the attempt to read all of the
        /// configuration files, just in case there is an error, or the log is requested </summary>
        [DataMember(Name = "log")]
        [XmlArray("log")]
        [XmlArrayItem("logEntry", typeof(string))]
        [ProtoMember(2)]
        public List<string> ReadingLog { get; set; }


        /// <summary> Source files that were read to build the configuration </summary>
        [DataMember(Name = "files")]
        [XmlArray("files")]
        [XmlArrayItem("file", typeof(string))]
        [ProtoMember(3)]
        public List<string> Files { get; set; }

        /// <summary> Add an empty line to the reading log  </summary>
        public void Add_Log()
        {
            if (ReadingLog == null) ReadingLog = new List<string>();

            ReadingLog.Add(String.Empty);
        }

        /// <summary> Most recent timestamp on any of the configuration files </summary>
        [DataMember(Name = "timestamp", EmitDefaultValue = false)]
        [XmlAttribute("timestamp")]
        [ProtoMember(4)]
        public DateTime LatestDateTimeStamp { get; set; }

        /// <summary> Add a reading log line to the log  </summary>
        /// <param name="LogLine"> Information to add to the log </param>
        public void Add_Log(string LogLine)
        {
            if (ReadingLog == null) ReadingLog = new List<string>();

            ReadingLog.Add(DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Second.ToString().PadLeft(2, '0') + "." + DateTime.Now.Millisecond.ToString().PadLeft(3, '0') + " - " + LogLine);
        }


    }
}
