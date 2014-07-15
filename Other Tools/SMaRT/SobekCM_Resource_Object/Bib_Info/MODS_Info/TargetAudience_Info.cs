#region Using directives

using System;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary>A description of the intellectual level of the audience for which the resource is intended.</summary>
    [Serializable]
    public class TargetAudience_Info : XML_Node_Base_Type, IEquatable<TargetAudience_Info>
    {
        private string audience;
        private string authority;

        /// <summary> Constructor for a new instance of the TargetAudience_Info class </summary>
        public TargetAudience_Info()
        {
            // DO nothing
        }

        /// <summary> Constructor for a new instance of the TargetAudience_Info class </summary>
        /// <param name="Audience">Description of targeted audience </param>
        public TargetAudience_Info(string Audience)
        {
            audience = Audience;

            string audience_caps = audience.ToUpper();
            if ((audience_caps == "ADOLESCENT") || (audience_caps == "ADULT") || (audience_caps == "GENERAL") || (audience_caps == "PRIMARY") || (audience_caps == "PRE-ADOLESCENT") || (audience_caps == "JUVENILE") || (audience_caps == "PRESCHOOL") || (audience_caps == "SPECIALIZED"))
            {
                authority = "marctarget";
            }
        }

        /// <summary> Constructor for a new instance of the TargetAudience_Info class </summary>
        /// <param name="Audience">Description of targeted audience </param>
        /// <param name="Authority">Authority from which this audience came from</param>
        public TargetAudience_Info(string Audience, string Authority)
        {
            audience = Audience;
            authority = Authority;

            if (authority.Length == 0)
            {
                string audience_caps = audience.ToUpper();
                if ((audience_caps == "ADOLESCENT") || (audience_caps == "ADULT") || (audience_caps == "GENERAL") || (audience_caps == "PRIMARY") || (audience_caps == "PRE-ADOLESCENT") || (audience_caps == "JUVENILE") || (audience_caps == "PRESCHOOL") || (audience_caps == "SPECIALIZED"))
                {
                    authority = "marctarget";
                }
            }
        }

        /// <summary> Gets or sets the description of the targeted audience </summary>
        public string Audience
        {
            get { return audience ?? String.Empty; }
            set { audience = value; }
        }

        /// <summary> Gets or sets the authority for the targeted audience </summary>
        /// <remarks>This comes from a controlled list</remarks>
        public string Authority
        {
            get { return authority ?? String.Empty; }
            set { authority = value; }
        }

        #region IEquatable<TargetAudience_Info> Members

        /// <summary> Compares this object with another similarly typed object </summary>
        /// <param name="other">Similarly types object </param>
        /// <returns>TRUE if the two objects are sufficiently similar</returns>
        public bool Equals(TargetAudience_Info other)
        {
            if (Audience == other.Audience)
                return true;
            else
                return false;
        }

        #endregion

        internal void Add_MODS(TextWriter results)
        {
            if (String.IsNullOrEmpty(audience))
                return;

            results.Write("<mods:targetAudience");
            base.Add_ID(results);
            if (!String.IsNullOrEmpty(authority))
                results.Write(" authority=\"" + authority + "\"");
            results.Write(">" + base.Convert_String_To_XML_Safe(audience) + "</mods:targetAudience>\r\n");
        }
    }
}