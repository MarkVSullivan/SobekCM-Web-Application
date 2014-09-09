#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Main display object represents a single digital resource including the metadata and structure information 
    /// necessary for a simple display of this item </summary>
    [Serializable]
    public class DisplayItem 
    {

        /// <summary> Creation and modification information for this resource </summary>
        /// <remarks> This falls short of the full tracking informatio, and is just the basic 
        /// information on when the item was created and last modified with some data from the METS header </remarks>
        public DisplayItem_CreationInfo creationInfo { get; set; }

        /// <summary> Behavior information about how this item behaves in a digital repository </summary>
        public DisplayItem_Behaviors behaviors { get; set; }

        public DisplayItem_Description description { get; set; }


 
        /// <summary> Gets the primary key information associated with the item/group in the database </summary>
        public DisplayItem_Keys keys { get; set; }

        ///// <summary> Gets and sets the source directory, where all the files can be found </summary>
        //public string Source_Directory
        //{
        //    get { return divInfo.Source_Directory; }
        //    set { divInfo.Source_Directory = value; }
        //}

        /// <summary> Gets and sets the division and file information for this resource </summary>
        //public Division_Info Divisions
        //{
        //    get { return divInfo; }
        //    set { divInfo = value; }
        //}


        /// <summary> Gets and sets the Bibliographic Identifier (BibID) associated with this resource </summary>
        public string BibID { get; internal set; }

        /// <summary> Gets and sets the Volume Identifier (VID) associated with this resource </summary>
        public string VID { get; internal set; }


    }
}