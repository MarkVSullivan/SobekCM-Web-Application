using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Library.OAI
{
    public class OAI_Record
    {
        public DateTime Last_Modified_Date { get; set; }

        public string Record { get; set;  }

        public string BibID { get; set;  }

        public OAI_Record( string BibID, string Record, DateTime Last_Modified_Date )
        {
            this.BibID = BibID;
            this.Record = Record;
            this.Last_Modified_Date = Last_Modified_Date;
        }

        public OAI_Record(string BibID, DateTime Last_Modified_Date)
        {
            this.BibID = BibID;
            this.Last_Modified_Date = Last_Modified_Date;
        }
    }
}
