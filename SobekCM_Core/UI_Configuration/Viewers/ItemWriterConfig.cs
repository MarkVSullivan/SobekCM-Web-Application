using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.UI.WebControls;
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
        private Dictionary<string, ItemSubViewerConfig> viewersByCode;
        private Dictionary<string, ItemSubViewerConfig> viewersByType;

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


        public ClassAssemblyConfig MainMenu { get; set; }



        /// <summary> Constructor for a new instance of the <see cref="ItemWriterConfig"/> class </summary>
        public ItemWriterConfig()
        {
            Class = "SobekCM.Library.HTML.Item_HtmlSubwriter";
            Viewers = new List<ItemSubViewerConfig>();

            set_default();
        }

        /// <summary> Add a new item viewer for the writer to use </summary>
        /// <param name="NewViewer"> New viewer to add </param>
        /// <remarks> If a viewer config already exists for the viewer type or viewer code, this 
        /// will replace the existing one </remarks>
        public void Add_Viewer(ItemSubViewerConfig NewViewer)
        {
            // Ensure the dictionaries are built
            if (viewersByCode == null) viewersByCode = new Dictionary<string, ItemSubViewerConfig>(StringComparer.InvariantCultureIgnoreCase);
            if (viewersByType == null) viewersByType = new Dictionary<string, ItemSubViewerConfig>(StringComparer.InvariantCultureIgnoreCase);

            // Check for the count of items in the dictionaries
            if (viewersByCode.Count != Viewers.Count)
            {
                viewersByCode.Clear();
                foreach (ItemSubViewerConfig existingConfig in Viewers)
                    viewersByCode[existingConfig.ViewerCode] = existingConfig;
            }
            if (viewersByType.Count != Viewers.Count)
            {
                viewersByType.Clear();
                foreach (ItemSubViewerConfig existingConfig in Viewers)
                    viewersByType[existingConfig.ViewerType] = existingConfig;
            }

            // Look for a match by code - remove any existing matches
            if (viewersByCode.ContainsKey(NewViewer.ViewerCode))
            {
                if (Viewers.Contains(viewersByCode[NewViewer.ViewerCode]))
                    Viewers.Remove(viewersByCode[NewViewer.ViewerCode]);
            }

            // Look for a match by type - remove any existing matches
            if (viewersByType.ContainsKey(NewViewer.ViewerType))
            {
                if (Viewers.Contains(viewersByType[NewViewer.ViewerType]))
                    Viewers.Remove(viewersByType[NewViewer.ViewerType]);
            }

            // Now, add the new viewer
            viewersByCode[NewViewer.ViewerCode] = NewViewer;
            viewersByType[NewViewer.ViewerType] = NewViewer;
            Viewers.Add(NewViewer);

        }

        /// <summary> Clears all the previously loaded information, such as the default values </summary>
        /// <remarks> This clears all the item viewer information, clears the assembly, and sets the class to the
        /// default item html subwriter class. </remarks>
        public void Clear()
        {
            Viewers.Clear();
            Assembly = String.Empty;
            Class = "SobekCM.Library.HTML.Item_HtmlSubwriter";
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
                Enabled = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "CITATION",
                ViewerCode = "citation",
                Class = "SobekCM.Library.ItemViewer.Viewers.Citation_Standard_ItemViewer_Prototyper",
                Enabled = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "DOWNLOADS",
                ViewerCode = "downloads",
                Class = "SobekCM.Library.ItemViewer.Viewers.Downloads_ItemViewer_Prototyper",
                Enabled = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "EMBEDDED_VIDEO",
                ViewerCode = "evideo",
                Class = "SobekCM.Library.ItemViewer.Viewers.EmbeddedVideo_ItemViewer_Prototyper",
                Enabled = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "FLASH",
                ViewerCode = "swf",
                Class = "SobekCM.Library.ItemViewer.Viewers.Flash_ItemViewer_Prototyper",
                Enabled = true,
                FileExtensions = new string[] { "SWF"}
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "GOOGLE_MAP",
                ViewerCode = "map",
                Class = "SobekCM.Library.ItemViewer.Viewers.Google_Map_ItemViewer_Prototyper",
                Enabled = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "HTML",
                ViewerCode = "html",
                Class = "SobekCM.Library.ItemViewer.Viewers.HTML_ItemViewer_Prototyper",
                Enabled = true,
                FileExtensions = new string[] { "HTML", "HTM" }
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "JPEG",
                ViewerCode = "#j",
                Class = "SobekCM.Library.ItemViewer.Viewers.JPEG_ItemViewer_Prototyper",
                Enabled = true,
                PageExtensions = new string[] { "JPG", "JPEG" }
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "JPEG2000",
                ViewerCode = "#x",
                Class = "SobekCM.Library.ItemViewer.Viewers.JPEG2000_ItemViewer_Prototyper",
                Enabled = true,
                PageExtensions = new string[] { "JP2" }
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "MANAGE_MENU",
                ViewerCode = "manage",
                Class = "SobekCM.Library.ItemViewer.Viewers.ManageMenu_ItemViewer_Prototyper",
                Enabled = true,
                ManagementViewer = true,
                ManagementOrder = 1000
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "METADATA",
                ViewerCode = "metadata",
                Class = "SobekCM.Library.ItemViewer.Viewers.Metadata_Links_ItemViewer_Prototyper",
                Enabled = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "ALL_VOLUMES",
                ViewerCode = "allvolumes",
                Class = "SobekCM.Library.ItemViewer.Viewers.MultiVolumes_ItemViewer_Prototyper",
                Enabled = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "PDF",
                ViewerCode = "pdf",
                Class = "SobekCM.Library.ItemViewer.Viewers.PDF_ItemViewer_Prototyper",
                Enabled = true,
                FileExtensions = new string[] { "PDF" }
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "RELATED_IMAGES",
                ViewerCode = "thumbs",
                Class = "SobekCM.Library.ItemViewer.Viewers.Related_Images_ItemViewer_Prototyper",
                Enabled = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "USAGE",
                ViewerCode = "usage",
                Class = "SobekCM.Library.ItemViewer.Viewers.Usage_Stats_ItemViewer_Prototyper",
                Enabled = true
            });

            Viewers.Add(new ItemSubViewerConfig
            {
                ViewerType = "VIDEO",
                ViewerCode = "video",
                Class = "SobekCM.Library.ItemViewer.Viewers.Video_ItemViewer_Prototyper",
                Enabled = true,
                FileExtensions = new string[] { "WEBM", "OGG", "MP4" }
            });
        }

        /// <summary> Sets the main menu provider for this item writer </summary>
        /// <param name="Class"> Fully qualified (including namespace) name of the class used to display the item main menu </param>
        /// <param name="Assembly"> Name of the assembly within which this class resides, unless this 
        /// is one of the default class/assembly included in the core code </param>
        public void SetMainMenu(string Class, string Assembly)
        {
            MainMenu = new ClassAssemblyConfig
            {
                Class = Class,
                Assembly = Assembly
            };
        }
    }
}
