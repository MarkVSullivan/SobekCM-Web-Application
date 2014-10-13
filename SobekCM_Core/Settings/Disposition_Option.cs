#region Using directives

using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.Settings
{
    /// <summary> Stores display information for a single possible disposition, or how physical material should be
    /// handled after digitization completes </summary>
    [DataContract]
    public class Disposition_Option
    {
        /// <summary> This disposition in a future tense (default language) </summary>
        [DataMember]
        public readonly string Future;

        /// <summary> This disposition in a past tense (default language) </summary>
        [DataMember]
        public readonly string Past;

        /// <summary> Key to this disposition </summary>
        [DataMember]
        public readonly int Key;

        /// <summary> Constructor for a new instance of the Disposition_Option class </summary>
        /// <param name="Key"> Key to this disposition </param>
        /// <param name="Past"> This disposition in a past tense (default language) </param>
        /// <param name="Future"> This disposition in a future tense (default language)</param>
        public Disposition_Option(int Key, string Past, string Future)
        {
            this.Future = Future;
            this.Past = Past;
            this.Key = Key;
        }
    }
}
