#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Settings
{
    /// <summary> [DataContract] Class stores the all the settings used by the builder </summary>
    [Serializable, DataContract, ProtoContract]
    public class Builder_Settings
    {
        /// <summary> Constructor for a new instance of the Builder_Settings class </summary>
        public Builder_Settings()
        {
            // Set some defaults
            Send_Usage_Emails = false;
            Add_PageTurner_ItemViewer = false;
            Verbose_Flag = false;
            Seconds_Between_Polls = 60;

            // Initialized the collections
            IncomingFolders = new List<Builder_Source_Folder>();
            PreProcessModulesSettings = new List<Builder_Module_Setting>();
            PostProcessModulesSettings = new List<Builder_Module_Setting>();
            ItemProcessModulesSettings = new List<Builder_Module_Setting>();
            ItemDeleteModulesSettings = new List<Builder_Module_Setting>();
        }
        
        /// <summary> [DataMember] List of all the incoming folders which should be checked for new resources </summary>
        [DataMember(Name = "folders")]
        [XmlArray("folders")]
        [XmlArrayItem("folder", typeof(Builder_Source_Folder))]
        [ProtoMember(1)]
        public List<Builder_Source_Folder> IncomingFolders { get; private set; }

        /// <summary> [DataMember] List of modules to run before doing any additional processing </summary>
        [DataMember(Name = "preProcessModules")]
        [XmlArray("preProcessModules")]
        [XmlArrayItem("module", typeof(Builder_Module_Setting))]
        [ProtoMember(2)]
        public List<Builder_Module_Setting> PreProcessModulesSettings { get; private set; }

        /// <summary> [DataMember] List of modules to run after doing any additional processing </summary>
        [DataMember(Name = "postProcessModules")]
        [XmlArray("postProcessModules")]
        [XmlArrayItem("module", typeof(Builder_Module_Setting))]
        [ProtoMember(3)]
        public List<Builder_Module_Setting> PostProcessModulesSettings { get; private set; }

        /// <summary> [DataMember] List of all the builder modules used for new packages or updates (processed by the builder) </summary>
        [DataMember(Name = "itemProcessModules")]
        [XmlArray("itemProcessModules")]
        [XmlArrayItem("module", typeof(Builder_Module_Setting))]
        [ProtoMember(4)]
        public List<Builder_Module_Setting> ItemProcessModulesSettings { get; private set; }

        /// <summary> [DataMember] List of all the builder modules to use for item deletes (processed by the builder) </summary>
        [DataMember(Name = "itemDeleteModules")]
        [XmlArray("itemDeleteModules")]
        [XmlArrayItem("module", typeof(Builder_Module_Setting))]
        [ProtoMember(5)]
        public List<Builder_Module_Setting> ItemDeleteModulesSettings { get; private set; }

        /// <summary> Clear all these settings </summary>
        public virtual void Clear()
        {
            IncomingFolders.Clear();
            PreProcessModulesSettings.Clear();
            PostProcessModulesSettings.Clear();
            ItemProcessModulesSettings.Clear();
            ItemDeleteModulesSettings.Clear();
        }


        /// <summary> Flag indicates if the page turner should be added automatically </summary>
        [DataMember(Name="addPageTurner")]
        [XmlElement("addPageTurner")]
        [ProtoMember(6)]
        public bool Add_PageTurner_ItemViewer { get; set; }

        /// <summary> Flag indicates if the builder should try to convert office files (Word and Powerpoint) to PDF during load and post-processing </summary>
        [DataMember(Name = "convertOfficeFilesToPdf")]
        [XmlElement("convertOfficeFilesToPdf")]
        [ProtoMember(7)]
        public bool Convert_Office_Files_To_PDF { get; set; }

        /// <summary> IIS web log location (usually a network share) for the builder
        /// to read the logs and add the usage statistics to the database </summary>
        [DataMember(Name = "iisLogsDirectory")]
        [XmlElement("iisLogsDirectory")]
        [ProtoMember(8)]
        public string IIS_Logs_Directory { get; set; }

        /// <summary> Number of days builder logs remain before the builder will try to delete it </summary>
        [DataMember(Name = "logExpirationDays")]
        [XmlElement("logExpirationDays")]
        [ProtoMember(9)]
        public int Log_Expiration_Days { get; set; }

        /// <summary> Number of seconds the builder waits between polls </summary>
        [DataMember(Name = "secondsBetweenPolls")]
        [XmlElement("secondsBetweenPolls")]
        [ProtoMember(10)]
        public int Seconds_Between_Polls { get; set; }

        /// <summary> Flag indicates is usage emails should be sent automatically
        /// after the stats usage has been calculated and added to the database </summary>
        [DataMember(Name = "sendUsageEmails")]
        [XmlElement("sendUsageEmails")]
        [ProtoMember(11)]
        public bool Send_Usage_Emails { get; set; }

        /// <summary> Flag indicates if the builder should be extra verbose in the log files (used for debugging purposes mostly) </summary>
        [DataMember(Name = "verboseFlag")]
        [XmlElement("verboseFlag")]
        [ProtoMember(12)]
        public bool Verbose_Flag { get; set; }

        /// <summary> Flag indicates whether checksums should be verified </summary>
        [XmlIgnore]
        public bool VerifyCheckSum { get; set; }

    }
}
