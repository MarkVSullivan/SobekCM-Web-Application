using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.UI_Configuration.Viewers
{
    /// <summary> Configuration information for the special item HTML writer, including
    /// the viewers configuration </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ItemWriterConfig")]
    public class ItemWriterConfig
    {
        /// <summary> Fully qualified (including namespace) name of the main class used
        /// as the item HTML writer </summary>
        /// <remarks> By default, this would be 'SobekCM.Library.HTML.Item_HtmlSubwriter' </remarks>
        [DataMember(Name = "class")]
        [XmlAttribute("class")]
        [ProtoMember(1)]
        public string Class { get; set; }

        /// <summary> Name of the assembly within which this class resides, unless this
        /// is one of the default subviewers included in the core code </summary>
        /// <remarks> By default, this would be blank </remarks>
        [DataMember(Name = "assembly", EmitDefaultValue = false)]
        [XmlAttribute("assembly")]
        [ProtoMember(2)]
        public string Assembly { get; set; }

        /// <summary> Collection of item viewers mapped to viewer codes </summary>
        [DataMember(Name = "viewers")]
        [XmlArray("viewers")]
        [XmlArrayItem("viewer", typeof(ItemSubViewerConfig))]
        [ProtoMember(3)]
        public List<ItemSubViewerConfig> Viewers { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="ItemWriterConfig"/> class </summary>
        public ItemWriterConfig()
        {
            Class = "SobekCM.Library.HTML.Item_HtmlSubwriter";
            Viewers = new List<ItemSubViewerConfig>();

            set_default();
        }

        private void set_default()
        {
            Assembly = null;
            Class = "SobekCM.Library.HTML.Item_HtmlSubwriter";
            Viewers.Clear();

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "MARC",
                ViewerCode = "marc",
                Class = "SobekCM.Library.ItemViewer.Viewers.Citation_MARC_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "CITATION",
                ViewerCode = "citation",
                Class = "SobekCM.Library.ItemViewer.Viewers.Citation_Standard_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "DOWNLOADS",
                ViewerCode = "downloads",
                Class = "SobekCM.Library.ItemViewer.Viewers.Downloads_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "EMBEDDED_VIDEO",
                ViewerCode = "evideo",
                Class = "SobekCM.Library.ItemViewer.Viewers.EmbeddedVideo_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "FLASH",
                ViewerCode = "swf",
                Class = "SobekCM.Library.ItemViewer.Viewers.Flash_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true,
                FileExtensions = new string[] { "SWF"}
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "GOOGLE_MAP",
                ViewerCode = "map",
                Class = "SobekCM.Library.ItemViewer.Viewers.Google_Map_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "HTML",
                ViewerCode = "html",
                Class = "SobekCM.Library.ItemViewer.Viewers.HTML_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true,
                FileExtensions = new string[] { "HTML", "HTM" }
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "JPEG",
                ViewerCode = "#j",
                Class = "SobekCM.Library.ItemViewer.Viewers.JPEG_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true,
                PageExtensions = new string[] { "JPG", "JPEG" }
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "JPEG2000",
                ViewerCode = "#x",
                Class = "SobekCM.Library.ItemViewer.Viewers.JPEG2000_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true,
                PageExtensions = new string[] { "JP2" }
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "MANAGE_MENU",
                ViewerCode = "manage",
                Class = "SobekCM.Library.ItemViewer.Viewers.ManageMenu_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "METADATA",
                ViewerCode = "metadata",
                Class = "SobekCM.Library.ItemViewer.Viewers.Metadata_Links_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "ALL_VOLUMES",
                ViewerCode = "allvolumes",
                Class = "SobekCM.Library.ItemViewer.Viewers.MultiVolumes_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "PDF",
                ViewerCode = "pdf",
                Class = "SobekCM.Library.ItemViewer.Viewers.PDF_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true,
                FileExtensions = new string[] { "PDF" }
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "RELATED_IMAGES",
                ViewerCode = "thumbs",
                Class = "SobekCM.Library.ItemViewer.Viewers.Related_Images_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "USAGE",
                ViewerCode = "usage",
                Class = "SobekCM.Library.ItemViewer.Viewers.Usage_Stats_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "VIDEO",
                ViewerCode = "video",
                Class = "SobekCM.Library.ItemViewer.Viewers.Video_ItemViewer_Prototyper",
                Enabled = true,
                AlwaysAdd = true,
                FileExtensions = new string[] { "WEBM", "OGG", "MP4" }
            });
        }
    }
}
