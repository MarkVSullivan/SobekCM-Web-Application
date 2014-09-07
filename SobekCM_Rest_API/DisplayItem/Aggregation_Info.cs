#region Using directives

using System;

#endregion

namespace SobekCM.Resource_Object.Behaviors
{
    /// <summary> Stores basic information (code and name) of all aggregations linked to a digital object </summary>
    [Serializable]
    public class Aggregation_Info : IEquatable<Aggregation_Info>
    {
        private string code;
        private string name;

        /// <summary> Constructor for a new instance of the Aggregation_Info object </summary>
        /// <param name="Code"> Aggregation code </param>
        /// <param name="Name"> Full name of this aggregation </param>
        public Aggregation_Info(string Code, string Name)
        {
            code = Code.ToUpper();
            name = Name;
        }

        /// <summary> Gets the code associated with this aggregation </summary>
        public string Code
        {
            get { return code; }
        }

        /// <summary> Gets the full name associated with this aggregation </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        #region IEquatable<Aggregation_Info> Members

        /// <summary> Determines if this aggregation is equal to another aggregation object </summary>
        /// <param name="other"> Object to compare this aggregation to </param>
        /// <returns> TRUE if the aggregation codes are the same, otherwise FALSE </returns>
        public bool Equals(Aggregation_Info other)
        {
            if (code == other.Code)
                return true;
            else
                return false;
        }

        #endregion
    }
}