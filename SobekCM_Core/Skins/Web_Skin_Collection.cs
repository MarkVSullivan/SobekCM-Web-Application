#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using ProtoBuf;
using SobekCM.Resource_Object.Database;

#endregion

namespace SobekCM.Core.Skins
{
	/// <summary> Collection of all the HTML skin data needed, including the fully built default skins and 
    /// the raw data to create the other HTML skins required to fulfil a user's request </summary>
    [Serializable, DataContract, ProtoContract]
	public class Web_Skin_Collection
	{
        [DataMember(Name = "orderedCodes")]
        [ProtoMember(1)]
        public List<string> Ordered_Skin_Codes { get; set; }

	    /// <summary> Constructor for a new instance of the Web_Skin_Collection class  </summary>
		public Web_Skin_Collection()
		{
	        Ordered_Skin_Codes = new List<string>();
		}

        /// <summary> Constructor for a new instance of the Web_Skin_Collection class  </summary>
        /// <param name="Interface_Table"> Datatable with the interface details for all valid HTML skins from the database </param>
        /// <remarks> This datatable is retrieved from the database by calling 
        /// the <see cref="Engine_Database.Get_All_Web_Skins"/> method</remarks>
        public Web_Skin_Collection(DataTable Interface_Table)
        {
            Ordered_Skin_Codes = new List<string>();

            Skin_Table = Interface_Table;
        }

        /// <summary> Initialize the web skin collection, by clearing it and 
        /// passing in the table of web skins </summary>
        /// <param name="Interface_Table"> Datatable with all the web skin information </param>
	    public void Initialize(DataTable Interface_Table)
	    {
	        Clear();
            Skin_Table = Interface_Table;

            Ordered_Skin_Codes = (from DataRow thisRow in Skin_Table.Rows select thisRow[0].ToString().ToLower()).ToList();
	    }

	    /// <summary> Datatable which has the information about every valid HTML skin from the database </summary>
	    /// <remarks> This is passed in during construction of this object.  This datatable was retrieved by calling 
	    /// the <see cref="SobekCM_Database.Get_All_Web_Skins"/> method during 
	    /// application startup when the <see cref="SobekCM_Skin_Collection_Builder.Populate_Default_Skins"/> method is called. </remarks>
	    [DataMember]
	    public DataTable Skin_Table { get; set; }

	    /// <summary> Clears all the default interfaces </summary>
	    public void Clear()
	    {
            Skin_Table = null;
	    }

	    /// <summary> Datarow matching the provided skin code </summary>
	    /// <param name="Skin_Code"> Code for the HTML skin information to retrieve </param>
	    /// <returns> Row from a database query with basic information about the skin to build ( codes, override flags, banner link ), or NULL </returns>
	    /// <remarks> The datarow for this method is from the datatable passed in during construction of this object.  This datatable was retrieved by calling 
	    /// the <see cref="Database.SobekCM_Database.Get_All_Web_Skins"/> method during 
	    /// application startup when the <see cref="SobekCM_Skin_Collection_Builder.Populate_Default_Skins"/> method is called. </remarks>
	    public DataRow Skin_Row(string Skin_Code )
	    {
	        DataRow[] selectedRows = Skin_Table.Select("WebSkinCode = '" + Skin_Code + "'");
	        return selectedRows.Length > 0 ? selectedRows[0] : null;
	    }
	}
}
