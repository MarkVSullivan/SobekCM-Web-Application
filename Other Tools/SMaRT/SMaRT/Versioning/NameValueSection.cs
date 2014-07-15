#region Using directives

using System.Configuration;

#endregion

namespace SobekCM.Management_Tool.Versioning
{
    /// <summary> Custom version check configuration section which is essentially a name value collection </summary>
    public class VersionCheckerConfigSection : ConfigurationSection
    {   
        /// <summary> Gets the collection of name-value pairs from this custom configuration section </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public NameValueConfigurationCollection Settings
        {
            get
            {   
                return (NameValueConfigurationCollection)base[""];   
            }
        }
    }
}
