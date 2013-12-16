#region Using directives

using System;

#endregion

namespace SobekCM.Resource_Object.Divisions
{
    /// <summary> Class holds information about any outer divisions which wrap around the 
    /// rest of the structure map and explain how this resource relates to the larger series or 
    /// set of resources  </summary>
    public class Outer_Division_Info
    {
        /// <summary> Constructor for a new instance of the Outer_Division_Info class </summary>
        public Outer_Division_Info()
        {
            Label = String.Empty;
            OrderLabel = 0;
            Type = String.Empty;
        }

        /// <summary> Constructor for a new instance of the Outer_Division_Info class </summary>
        /// <param name="Label">Textual label for this outer division ( i.e., 'Volume 4', 'Issue 2')</param>
        /// <param name="OrderLabel">Numeric order information associated with the label</param>
        /// <param name="Type">Type of division ( i.e., volume, issue, etc.. )</param>
        public Outer_Division_Info(string Label, int OrderLabel, string Type)
        {
            this.Label = Label;
            this.OrderLabel = OrderLabel;
            this.Type = Type;
        }

        /// <summary> Textual label for this outer division ( i.e., 'Volume 4', 'Issue 2') </summary>
        public string Label { get; set; }

        /// <summary> Numeric order information associated with the label  </summary>
        public int OrderLabel { get; set; }

        /// <summary> Type of division ( i.e., volume, issue, etc.. ) </summary>
        public string Type { get; set; }
    }
}