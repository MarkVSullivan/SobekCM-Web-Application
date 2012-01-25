#region Using directives

using System;
using System.Data;
using System.Runtime.Serialization;
using SobekCM.Library.MemoryMgmt;

#endregion

namespace SobekCM.Library.Items
{
    /// <summary> Wrapper class that holds the list of all items for a particular volume </summary>
    /// <remarks> This implements the <see cref="ISerializable"/> interface to explicitly handle serialization and deserialization for speed purposes </remarks>
    [Serializable]
    public class SobekCM_Items_In_Title : ISerializable
    {
        private readonly DataTable innerData;

        /// <summary> Constructor for a new instance of the SobekCM_Items_In_Title class </summary>
        /// <remarks> This creates an empty dataset </remarks>
        public SobekCM_Items_In_Title()
        {
            innerData = new DataTable();
            innerData.Columns.Add("ItemID", typeof(Int32));
            innerData.Columns.Add("Title");
            innerData.Columns.Add("Level1_Text");
            innerData.Columns.Add("Level1_Index", typeof(Int32));
            innerData.Columns.Add("Level2_Text");
            innerData.Columns.Add("Level2_Index", typeof(Int32));
            innerData.Columns.Add("Level3_Text");
            innerData.Columns.Add("Level3_Index", typeof(Int32));
            innerData.Columns.Add("Level4_Text");
            innerData.Columns.Add("Level4_Index", typeof(Int32));
            innerData.Columns.Add("Level5_Text");
            innerData.Columns.Add("Level5_Index", typeof(Int32));
            innerData.Columns.Add("MainThumbnail");
            innerData.Columns.Add("VID");
            innerData.Columns.Add("IP_Restriction_Mask", typeof(Int16));

        }

        /// <summary> Constructor for a new instance of the SobekCM_Items_In_Title class </summary>
        /// <param name="Item_Information"> Raw data from the search or browse with item and title information </param>
        public SobekCM_Items_In_Title(DataTable Item_Information)
        {
            innerData = Item_Information;
        }

        /// <summary> Constructor for a new instance of the SobekCM_Items_In_Title class </summary>
        /// <param name="info"> Serialization information object, from which the data of this class is read </param>
        /// <param name="context"> Context of the deserialization request </param>
        /// <remarks> This constructor is used to build this object during a deserialization request, such as when 
        /// an object of this type is pulled from the remote caching server </remarks>
        public SobekCM_Items_In_Title(SerializationInfo info, StreamingContext context)
        {
            innerData = new DataTable();
            AdoNetHelper.DeserializeDataTable(innerData, (byte[])info.GetValue("data", typeof(byte[])));
        }

        /// <summary> Gets the inner table with all of the information for all items within this title </summary>
        public DataTable Item_Table
        {
            get
            {
                return innerData;
            }
        }

        #region ISerializable Members

        /// <summary> Method is used to customize the serialization of this object and returns all the
        /// pertinent serialized data, including the main dataset serialized to binary (rather than XML)</summary>
        /// <param name="info"> Serialization information object, to which the data of this class is added </param>
        /// <param name="context"> Context of the serialization request </param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("data", AdoNetHelper.SerializeDataTable(innerData));
        }

        #endregion
    }
}
