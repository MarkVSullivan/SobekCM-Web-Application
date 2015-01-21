#region Using directives

using System;

#endregion

namespace SobekCM.Resource_Object.Behaviors
{
    /// <summary> Stores basic information (code and name) of all aggregations linked to a digital object </summary>
    [Serializable]
    public class Aggregation_Info : IEquatable<Aggregation_Info>
    {
        /// <summary> Constructor for a new instance of the Aggregation_Info object </summary>
        /// <param name="Code"> Aggregation code </param>
        /// <param name="Name"> Full name of this aggregation </param>
        public Aggregation_Info(string Code, string Name)
        {
            this.Code = Code.ToUpper();
            this.Name = Name;
        }

        /// <summary> Gets the code associated with this aggregation </summary>
        public string Code { get; private set; }

        /// <summary> Gets the full name associated with this aggregation </summary>
        public string Name { get; set; }

        /// <summary> Type of aggregation </summary>
        public string Type { get; set;  }

        #region IEquatable<Aggregation_Info> Members

        /// <summary> Determines if this aggregation is equal to another aggregation object </summary>
        /// <param name="other"> Object to compare this aggregation to </param>
        /// <returns> TRUE if the aggregation codes are the same, otherwise FALSE </returns>
        public bool Equals(Aggregation_Info other)
        {
            if (Code == other.Code)
                return true;
            else
                return false;
        }

        #endregion
    }
}