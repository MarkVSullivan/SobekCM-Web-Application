using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Settings.DbItemViewers
{
    /// <summary> All the item viewer information from the database,
    /// which includes which viewers are added by default, default orders, and the primary key 
    /// to which each individual digital resource is attached </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("ItemViewerTypes")] 
    public class DbItemViewerTypes
    {
        private Dictionary<string, DbItemViewerType> lookupDictionary;

        /// <summary> List of item viewer types from the database </summary>
        [DataMember(Name = "viewers")]
        [XmlArray("viewer")]
        [XmlArrayItem("viewer", typeof(DbItemViewerType))]
        [ProtoMember(1)]
        public List<DbItemViewerType> ViewerTypes { set; get; }

        /// <summary> Constructor for a new instance of the <see cref="DbItemViewerTypes"/> class </summary>
        public DbItemViewerTypes()
        {
            ViewerTypes = new List<DbItemViewerType>();
        }

        /// <summary> Get the viewer code information from the database, by viewer code </summary>
        /// <param name="ViewerCode">The viewer code.</param>
        /// <returns></returns>
        public DbItemViewerType Get_ViewerType(string ViewerCode)
        {
            // Ensure the dictionary is built
            if (lookupDictionary == null) lookupDictionary = new Dictionary<string, DbItemViewerType>(StringComparer.OrdinalIgnoreCase);

            // Ensure the dictionary is populated
            if (lookupDictionary.Count != ViewerTypes.Count)
            {
                lookupDictionary.Clear();
                foreach (DbItemViewerType thisType in ViewerTypes)
                {
                    lookupDictionary[thisType.ViewType] = thisType;
                }
            }

            // If this viewer code exists, return it
            if (lookupDictionary.ContainsKey(ViewerCode)) return lookupDictionary[ViewerCode];

            // Not found
            return null;
        }

        /// <summary> Adds a new item viewer config, from the database, to the list of possible
        /// item viewers </summary>
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
        public void Add_ViewerType(int ID, string ViewType, int Order, bool DefaultView, decimal MenuOrder)
        {
            ViewerTypes.Add(new DbItemViewerType(ID, ViewType, Order, DefaultView, MenuOrder ));
        }
    }
}
