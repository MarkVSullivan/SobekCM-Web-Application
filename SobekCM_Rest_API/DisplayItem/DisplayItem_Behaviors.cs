#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
	/// <summary> Class holds all the non-descriptive behavior information, such as under which
	/// aggregations this item should appear, which web skins, etc..</summary>
	/// <remarks>  Depending on the currently METS-writing profile, much of this data 
	/// can also be written into a METS file, which differentiates this object from the
	/// Web_Info object, which is utilized by the web server to assist with some rendering
	/// and to generally improve performance by calculating and saving several values
	/// when building the resource object.  </remarks>
	[DataContract]
	public class DisplayItem_Behaviors 
	{
        /// <summary>  </summary>
        [DataMember(EmitDefaultValue = false)]
		public List<DisplayItem_Aggregation> aggregations { get; internal set; }

        /// <summary>  </summary>
        [DataMember(EmitDefaultValue = false)]
		public List<string> viewers { get; internal set; }

        /// <summary>  </summary>
        [DataMember(EmitDefaultValue = false)]
		public List<string> webskins { get; internal set; }

        /// <summary>  </summary>
        [DataMember(EmitDefaultValue = false)]
		public List<DisplayItem_Wordmark> wordmarks { get; internal set; }

        /// <summary>  </summary>
        [DataMember(EmitDefaultValue = false)]
		public List<DisplayItem_DescriptiveTag> tags { get; internal set; }

        /// <summary>  </summary>
        [DataMember(EmitDefaultValue = false)]
		public bool canBeDescribed { get; internal set; }

        /// <summary>  </summary>
        [DataMember(EmitDefaultValue = false)]
		public Nullable<bool> darkFlag { get; internal set; }

        /// <summary>  </summary>
        [DataMember(EmitDefaultValue = false)]
		public Nullable<short> ipRestricted { get; internal set; }

        /// <summary>  </summary>
        [DataMember(EmitDefaultValue = false)]
		public string mainThumbnail { get; internal set; }

        /// <summary>  </summary>
        [DataMember(EmitDefaultValue = false)]
		public bool textSearchable { get; internal set; }

        /// <summary>  </summary>
        [DataMember(EmitDefaultValue = false)]
		public string groupTitle { get; internal set; }

        /// <summary>  </summary>
        [DataMember(EmitDefaultValue = false)]
		public string groupType { get; internal set; }

        /// <summary>  </summary>
        [DataMember(EmitDefaultValue = false)]
		public List<string> serialHierarchy { get; internal set; }
	}
}