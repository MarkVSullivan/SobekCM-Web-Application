#region Using directives

using System;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.OAI
{
    /// <summary> Basic information about an OAI record to be display  </summary>
    [DataContract]
    public class OAI_Record
    {
        /// <summary> Last date this item was modified in some way </summary>
        [DataMember]
        public DateTime Last_Modified_Date { get; set; }

        /// <summary> Full record to be provided via OAI-PMH </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Record { get; set;  }

        /// <summary> Bibliographic identifier of this item within this instance  </summary>
        [DataMember]
        public string BibID { get; set;  }

        /// <summary> Volume identifier of this item within this instance  </summary>
        [DataMember]
        public string VID { get; set; }

        /// <summary> Constructor for a new instance of the OAI_Record class  </summary>
        /// <param name="BibID"> Bibliographic identifier of this item within this instance </param>
        /// <param name="VID"> Volume identifier of this item within this instance </param>
        /// <param name="Record"> Full record to be provided via OAI-PMH </param>
        /// <param name="Last_Modified_Date"></param>
        public OAI_Record( string BibID, string VID, string Record, DateTime Last_Modified_Date )
        {
            this.BibID = BibID;
            this.VID = VID;
            this.Record = Record;
            this.Last_Modified_Date = Last_Modified_Date;
        }

        /// <summary> Constructor for a new instance of the OAI_Record class  </summary>
        /// <param name="BibID"> Bibliographic identifier of this item within this instance </param>
        /// <param name="VID"> Volume identifier of this item within this instance </param>
        /// <param name="Last_Modified_Date"> Last date this item was modified in some way </param>
        public OAI_Record(string BibID, string VID, DateTime Last_Modified_Date)
        {
            this.BibID = BibID;
            this.VID = VID;
            this.Last_Modified_Date = Last_Modified_Date;
        }
    }
}
