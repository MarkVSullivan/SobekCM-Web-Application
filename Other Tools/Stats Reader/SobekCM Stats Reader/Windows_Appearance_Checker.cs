#region Using directives

using Microsoft.Win32;

#endregion

namespace SobekCM_Stats_Reader
{
    /// <summary> Static class is used to check if the current window's theme is XP-style, or complies
    /// with the older Win95-type display </summary>
    /// <remarks> This class is often used when constructing a form to determine which style to apply to 
    /// some of the controls within the form, such as text boxes, combo boxes, and buttons.  This check is performed
    /// by checking the CurrentUser's Control Panel\Appearance's Current registry value. <br /><br />
    /// The first time the <see cref="Windows_Appearance_Checker.is_XP_Theme" /> method is called, the
    /// registry is checked and the value is stored for the remainder of checks.  Although this is promotes
    /// high-performance and does not repeatadly check the registry, this means if the style is modified 
    /// during execution of the application, the changed value is not read and the internal value of this
    /// class is not updated.</remarks>
    public class Windows_Appearance_Checker
    {
        private static readonly bool checkedTheme;
        private static bool isXp;

        /// <summary> Static constructor for the Windows_Appearance_Checker static class </summary>
        static Windows_Appearance_Checker()
        {
            checkedTheme = false;
            isXp = false;
        }

        /// <summary> Returns flag indicating if the current user's appearance is set to XP-style, or the 
        /// older Win-95 type appearance </summary>
        /// <remarks> If this is the first time this check has been performed, this checks the CurrentUser's
        /// Control Panel\Appearance's Current registry value. </remarks>
        public static bool is_XP_Theme
        {
            get
            {
                if (checkedTheme)
                {
                    return isXp;
                }

                try
                {
                    const string keyName = @"Control Panel\Appearance";
                    RegistryKey regKey = Registry.CurrentUser.CreateSubKey(keyName);
                    if (regKey != null)
                    {
                        string valueFromKey = regKey.GetValue("Current", "Windows Standard").ToString();
                        if ((valueFromKey == "Windows Standard") || (valueFromKey == "@themeui.dll,-854"))
                        {
                            isXp = false;
                        }
                        else
                        {
                            isXp = true;
                        }
                    }
                }
                catch
                {
                    isXp = false;
                }

                return isXp;
            }
        }
    }
}
