namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> Holds information about an additional date on this record </summary>
    public class Origin_Info_Other_Date
    {
        /// <summary> Date value for this other date </summary>
        public string Value { get; set; }

        /// <summary> Type of date ( or display label ) </summary>
        public string Type { get; set; }

        /// <summary> Indicates this is the key date for this resource </summary>
        public bool KeyDate { get; set; }

        /// <summary> Constructor for a new instance of the Origin_Info_Other_Date class </summary>
        /// <param name="Value"> Date value for this other date </param>
        public Origin_Info_Other_Date(string Value)
        {
            this.Value = Value;
            KeyDate = false;
        }

        /// <summary> Constructor for a new instance of the Origin_Info_Other_Date class </summary>
        /// <param name="Value"> Date value for this other date </param>
        /// <param name="Type"> Type of date ( or display label ) </param>
        public Origin_Info_Other_Date(string Value, string Type)
        {
            this.Value = Value;
            this.Type = Type;
            KeyDate = false;
        }
    }
}
