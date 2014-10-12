using System;
using SobekCM.Core.Settings;

namespace SobekCM.Library.Settings
{
    /// <summary> Class provides a static gateway to access and refresh the settings object ( #Singleton ) </summary>
    public static class InstanceWide_Settings_Singleton
    {
        private static InstanceWide_Settings settingsObject;

        private static readonly Object thisLock = new Object();

        /// <summary> Get the settings object (or build the object and return it) </summary>
        public static InstanceWide_Settings Settings
        {
            get
            {
                lock (thisLock)
                {
                    return settingsObject ?? (settingsObject = InstanceWide_Settings_Builder.Build_Settings());
                }
            }
        }

        /// <summary> Force a refresh of the settings objec </summary>
        public static bool Refresh()
        {
            try
            {
                lock (thisLock)
                {
                    if (settingsObject == null)
                        settingsObject = InstanceWide_Settings_Builder.Build_Settings();
                    else
                    {
                        InstanceWide_Settings newSettings = InstanceWide_Settings_Builder.Build_Settings();
                        settingsObject = newSettings;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
