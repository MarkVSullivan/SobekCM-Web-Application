using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Information about a date, such as creation, published, etc.., for this digital resource </summary>
    [DataContract]
    public class DisplayItem_Date
    {
        /// <summary> Type of this date ( i.e., creation, marc, published, etc..) </summary>
        [DataMember(EmitDefaultValue = false)]
        public string type;

        /// <summary> Point for this date ( i.e., 'start' or 'end' ) </summary>
        [DataMember(EmitDefaultValue = false)]
        public string point;

        /// <summary> Date string </summary>
        [DataMember(EmitDefaultValue = false)]
        public string date;
    }
}
