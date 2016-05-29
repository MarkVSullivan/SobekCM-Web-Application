using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.SqlServer.Server;
using ProtoBuf;

namespace SobekCM.Core.Settings.DbItemViewers
{
    /// <summary> Configuration object holds information about a single item viewer 
    /// type from the database, which does not include links to the classes that the UI
    /// may use to display the items for these viewer types </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ItemViewerType")]
    public class DbItemViewerType
    {
        /// <summary> Primary key to this item viewer from the database which is used
        /// to link an item, in the database, to this viewer </summary>
        [DataMember(Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary> Code for this view type, which is returned with each digital resource
        /// and links, via the UI configuration, to the class used to display this item
        /// within the user interface </summary>
        [DataMember(Name = "type")]
        [XmlAttribute("type")]
        [ProtoMember(2)]
        public string ViewType { get; set; }

        /// <summary> Default order, relative to the order on the other item viewers, that this
        /// should be displayed </summary>
        /// <remarks> The only important information from this order is which viewer will be 
        /// displayed by default when the user first comes to the digital resource on the web.<br />
        /// This can be overriden at the individual item level. </remarks>
        [DataMember(Name = "order")]
        [XmlAttribute("order")]
        [ProtoMember(3)]
        public int Order { get; set; }

        /// <summary> Flag indicates if this view should be automatically added to all new items
        /// that are added to the database </summary>
        [DataMember(Name = "default", EmitDefaultValue = false)]
        [XmlAttribute("default")]
        [ProtoMember(4)]
        public bool DefaultView { get; set; }

        /// <summary> Default order this viewer should be displayed on the item viewer menu </summary>
        /// <remarks> This can be overriden at the item level </remarks>
        [DataMember(Name = "menu")]
        [XmlAttribute("menu")]
        [ProtoMember(5)]
        public decimal MenuOrder { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="DbItemViewerType"/> class </summary>
        /// <param name="ID"> Primary key to this item viewer from the database which is used
        /// to link an item, in the database, to this viewer </param>
        /// <param name="ViewType"> Code for this view type, which is returned with each digital resource
        /// and links, via the UI configuration, to the class used to display this item
        /// within the user interface </param>
        /// <param name="Order"> Default order, relative to the order on the other item viewers, that this
        /// should be displayed </param>
        /// <param name="DefaultView"> Flag indicates if this view should be automatically added to all new items
        /// that are added to the database </param>
        /// <param name="MenuOrder"> Default order this viewer should be displayed on the item viewer menu </param>
        public DbItemViewerType(int ID, string ViewType, int Order, bool DefaultView, decimal MenuOrder )
        {
            this.ID = ID;
            this.ViewType = ViewType;
            this.Order = Order;
            this.DefaultView = DefaultView;
            this.MenuOrder = MenuOrder;
        }

        /// <summary> Constructor for a new instance of the <see cref="DbItemViewerType"/> class </summary>
        public DbItemViewerType()
        {
            // Empty constructor
        }
    }
}
