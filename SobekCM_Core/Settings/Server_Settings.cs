using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Settings
{
    /// <summary> Settings regarding the server architecture, include URLs and network locations </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ServerSettings")]
    public class Server_Settings
    {
        private string inProcessLocationOverride;

        /// <summary> Constructor for a new instance of the Server_Settings class </summary>
        public Server_Settings()
        {
            Base_URL = String.Empty;
            Image_URL = String.Empty;
            SobekCM_ImageServer = String.Empty;
            JP2ServerUrl = String.Empty;
            JP2ServerType = String.Empty;
            Web_Output_Caching_Minutes = 0;
            Static_Resources_Config_File = "CDN";
            Base_SobekCM_Location_Relative = String.Empty;
            isHosted = false;
        }

        /// <summary> Network directory for the SobekCM web application server </summary>
        [DataMember(Name = "applicationServerNetwork")]
        [XmlElement("applicationServerNetwork")]
        [ProtoMember(1)]
        public string Application_Server_Network { get; set; }

        /// <summary> Primary URL for this instance of the SobekCM web application server </summary>
        [DataMember(Name = "applicationServerUrl")]
        [XmlElement("applicationServerUrl")]
        [ProtoMember(2)]
        public string Application_Server_URL { get; set; }

        /// <summary> URL for the Solr/Lucene index for the document metadata and text </summary>
        [DataMember(Name = "documentSolrIndexUrl", EmitDefaultValue = false)]
        [XmlElement("documentSolrIndexUrl")]
        [ProtoMember(3)]
        public string Document_Solr_Index_URL { get; set; }

        /// <summary> Gets the URL for the SobekCM engine for this instance </summary>
        [DataMember(Name = "engineUrl")]
        [XmlElement("engineUrl")]
        [ProtoMember(4)]
        public string Engine_URL { get { return System_Base_URL + "engine/"; } }

        /// <summary> Network directory for the image server which holds all the resource files </summary>
        [DataMember(Name = "imageServerNetwork")]
        [XmlElement("imageServerNetwork")]
        [ProtoMember(5)]
        public string Image_Server_Network { get; set; }

        /// <summary> Network directory for the image server which holds all the resource files </summary>
        /// <remarks> Add by Keven for FIU dPanther separate image server </remarks>
        [DataMember(Name = "imageServerRoot")]
        [XmlElement("imageServerRoot")]
        [ProtoMember(6)]
        public string Image_Server_Root { get; set; }

        /// <summary> Base image URL for all digital resource images </summary>
        [DataMember(Name = "imageUrl")]
        [XmlElement("imageUrl")]
        [ProtoMember(7)]
        public string Image_URL { get; set; }

        /// <summary> Gets the TYPE of the JPEG2000 server - allowing the system to support different
        /// types of the zoomable server ( i.e., Aware, Djatoka, etc.. ) </summary>
        [DataMember(Name = "jp2ServerType", EmitDefaultValue = false)]
        [XmlElement("jp2ServerType")]
        [ProtoMember(8)]
        public string JP2ServerType { get; set; }

        /// <summary> URL for an external JPEG2000 zoomable image server </summary>
        [DataMember(Name = "jp2ServerUrl", EmitDefaultValue = false)]
        [XmlElement("jp2ServerUrl")]
        [ProtoMember(9)]
        public string JP2ServerUrl { get; set; }

        /// <summary> URL for the Solr/Lucene index for the page text </summary>
        [DataMember(Name = "pageSolrIndexUrl", EmitDefaultValue = false)]
        [XmlElement("pageSolrIndexUrl")]
        [ProtoMember(10)]
        public string Page_Solr_Index_URL { get; set; }

        /// <summary> URL to the SobekCM Image Server, initially used just when features need to be drawn on images </summary>
        [DataMember(Name = "sobekCustomImageServer", EmitDefaultValue = false)]
        [XmlElement("sobekCustomImageServer")]
        [ProtoMember(11)]
        public string SobekCM_ImageServer { get; set; }

        /// <summary> IP address for the SobekCM web server </summary>
        [DataMember(Name = "webServerIp", EmitDefaultValue = false)]
        [XmlElement("webServerIp")]
        [ProtoMember(12)]
        public string SobekCM_Web_Server_IP { get; set; }

        /// <summary> Location where all the item-level page exist for search engine indexing </summary>
        [DataMember(Name = "staticPagesLocation", EmitDefaultValue = false)]
        [XmlElement("staticPagesLocation")]
        [ProtoMember(13)]
        public string Static_Pages_Location { get; set; }

        /// <summary> Gets the complete url to this instance of SobekCM library software </summary>
        /// <value> Currently this always returns 'http://ufdc.ufl.edu/' </value>
        [DataMember(Name = "systemBaseUrl", EmitDefaultValue = false)]
        [XmlElement("systemBaseUrl")]
        [ProtoMember(14)]
        public string System_Base_URL { get; set; }

        /// <summary> Gets the error web page to send users to when a catastrophic error occurs </summary>
        /// <value> For example, for UFDC this always returns 'http://ufdc.ufl.edu/error.html' </value>
        [DataMember(Name = "systemErrorUrl", EmitDefaultValue = false)]
        [XmlElement("systemErrorUrl")]
        [ProtoMember(15)]
        public string System_Error_URL { get; set; }

        /// <summary> Relative location to the folders on the web server </summary>
        /// <remarks> This is only used when building pages in the SobekCM Builder, which allows for
        /// the replacement of all the relative references ( i.e., '/design/skins/dloc/dloc.css') with the full
        /// link ( i.e., 'http://example.edu/design/skins/dloc/dloc.css' ) </remarks>
        [DataMember(Name = "baseLocationRelative", EmitDefaultValue = false)]
        [XmlElement("baseLocationRelative")]
        [ProtoMember(16)]
        public string Base_SobekCM_Location_Relative { get; set; }

        /// <summary> Indicates which static resources configuration file to use </summary>
        [DataMember(Name = "staticResourcesConfigFile", EmitDefaultValue = false)]
        [XmlElement("staticResourcesConfigFile")]
        [ProtoMember(17)]
        public string Static_Resources_Config_File { get; set; }

        /// <summary> Flag indicates if the statistics information should be cached for very quick 
        /// retrieval for search engine robots. </summary>
        [DataMember(Name = "statisticsCachingEnabled")]
        [XmlElement("statisticsCachingEnabled")]
        [ProtoMember(18)]
        public bool Statistics_Caching_Enabled { get; set; }

        /// <summary> Number of minutes clients are suggested to cache the web output </summary>
        [DataMember(Name = "webOutputCachingMinutes")]
        [XmlElement("webOutputCachingMinutes")]
        [ProtoMember(19)]
        public int Web_Output_Caching_Minutes { get; set; }

        /// <summary> Gets the base URL for this instance, without the application name </summary>
        [DataMember(Name = "baseUrl")]
        [XmlElement("baseUrl")]
        [ProtoMember(20)]
        public string Base_URL { get; set; }

        /// <summary> Flag indicates if this is a 'hosted' solution of SobekCM, in which case
        /// certain fields should not be made available, even to "system admins" </summary>
        [DataMember(Name = "isHosted")]
        [XmlElement("isHosted")]
        [ProtoMember(21)]
        public bool isHosted { get; set; }

        /// <summary> Folder where files bound for archiving are placed </summary>
        [DataMember(Name = "packageArchivalFolder")]
        [XmlElement("packageArchivalFolder")]
        [ProtoMember(22)]
        public string Package_Archival_Folder { get; set; }


        #region Derivative properties which return the base directory or base url with a constant ending to indicate the SobekCM standard subfolders

        /// <summary> Base directory where the ASP.net application is running on the application server </summary>
        [XmlIgnore]
        public string Base_Directory { get { return Application_Server_Network; } set { Application_Server_Network = value; } }

        /// <summary> Directory for this application's mySobek folder, where the template and project files reside for online submittal and editing</summary>
        /// <value> [Base_Directory] + 'mySobek\' </value>
        [XmlIgnore]
        public string Base_MySobek_Directory
        {
            get { return Base_Directory + "mySobek\\"; }
        }

        /// <summary> Directory for this application's DATA folder, where the OAI source files reside </summary>
        /// <value> [Base_Dir] + 'data\' </value>
        [XmlIgnore]
        public string Base_Data_Directory
        {
            get { return Base_Directory + "data\\"; }
        }

        /// <summary> Directory for this application's TEMP folder, where some slow-changing data is stored in XML format </summary>
        /// <value> [Base_Dir] + 'temp\' </value>
        [XmlIgnore]
        public string Base_Temporary_Directory
        {
            get { return Base_Directory + "temp\\"; }
        }

        /// <summary> Directory for this application's DESIGN folder, where all the aggregation and interface folders reside </summary>
        /// <value> [Base_Directory] + 'design\' </value>
        [XmlIgnore]
        public string Base_Design_Location
        {
            get { return Base_Directory + "design\\"; }
        }

        /// <summary> Gets the location that submission packets are built before being submitted into the regular
        /// digital resource location </summary>
        public string In_Process_Submission_Location
        {
            get
            {
                if (String.IsNullOrEmpty(inProcessLocationOverride))
                    return Base_Directory + "\\mySobek\\InProcess";
                return inProcessLocationOverride;
            }
            set { inProcessLocationOverride = value; }
        }



        /// <summary> Network location of the recycle bin, where deleted items and
        /// files are placed for a while, in case of accidental deletion </summary>
        [XmlIgnore]
        public string Recycle_Bin
        {
            get { return Image_Server_Network + "\\RECYCLE BIN"; }
        }

        #endregion

    }
}
