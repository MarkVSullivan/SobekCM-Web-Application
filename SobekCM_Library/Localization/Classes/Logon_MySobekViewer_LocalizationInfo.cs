namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Logon_MySobekViewer class </summary>
    public class Logon_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Logon_MySobekViewer_Localization class </summary>
        public Logon_MySobekViewer_LocalizationInfo() : base()
        {
            // Do nothing
        }

        /// <summary> Adds a localization string ( with key and value ) to this localization class </summary>
        /// <param name="Key"> Key for the new localization string being saved </param>
        /// <param name="Value"> Value for this localization string </param>
        /// <remarks> This overrides the base class's implementation </remarks>
        public override void Add_Localization_String(string Key, string Value)
        {
            // First, add to the localization string dictionary
            base.Add_Localization_String(Key, Value);

            // Assign to custom properties depending on the key
            switch (Key)
            {
                case "If You Have A Valid XXX Logon Xxxsign On With XXX Authentication Herexxx":
                    IfYouHaveAValidXXXLogonXxxsignOnWithXXXAuthenticationHerexxx = Value;
                    break;

                case "Not Registered Yet Xxxregister Nowxxx Or Xxxcontact Usxxx":
                    NotRegisteredYetXxxregisterNowxxxOrXxxcontactUsxxx = Value;
                    break;

                case "Forgot Your Username Or Password Please Xxxcontact Usxxx":
                    ForgotYourUsernameOrPasswordPleaseXxxcontactUsxxx = Value;
                    break;

                case "LOG IN":
                    LOGIN = Value;
                    break;

                case "Logon To XXX":
                    LogonToXXX = Value;
                    break;

                case "Not Registered Yet Xxxregister Nowxxx":
                    NotRegisteredYetXxxregisterNowxxx = Value;
                    break;

                case "Password":
                    Password = Value;
                    break;

                case "Please Choose The Appropriate Logon Directly Below":
                    PleaseChooseTheAppropriateLogonDirectlyBelow = Value;
                    break;

                case "Remember Me":
                    RememberMe = Value;
                    break;

                case "The Feature You Are Trying To Access Requires A Valid Logon":
                    TheFeatureYouAreTryingToAccessRequiresAValidLogon = Value;
                    break;

                case "Username Or Email":
                    UsernameOrEmail = Value;
                    break;

            }
        }
        /// <remarks> Line for user to logon with current system authentication ( {1} and {2} are replaced with link-enabling HTML) </remarks>
        public string IfYouHaveAValidXXXLogonXxxsignOnWithXXXAuthenticationHerexxx { get; private set; }

        /// <remarks> "Line for users to either register or send an email (in case of problems).  {0}, {1}, {2} are all replaced with link-enabling HTML" </remarks>
        public string NotRegisteredYetXxxregisterNowxxxOrXxxcontactUsxxx { get; private set; }

        /// <remarks> Shorter version of not-registered help used in the pop-up login form </remarks>
        public string ForgotYourUsernameOrPasswordPleaseXxxcontactUsxxx { get; private set; }

        /// <remarks> Title for pop-up login form </remarks>
        public string LOGIN { get; private set; }

        /// <remarks> Title when the user opts to log onto the system </remarks>
        public string LogonToXXX { get; private set; }

        /// <remarks> Shorter version of not-registered help used in the pop-up login form </remarks>
        public string NotRegisteredYetXxxregisterNowxxx { get; private set; }

        /// <remarks> Used in the pop-up login form </remarks>
        public string Password { get; private set; }

        /// <remarks> 'Please choose the appropriate logon directly below.' localization string </remarks>
        public string PleaseChooseTheAppropriateLogonDirectlyBelow { get; private set; }

        /// <remarks> Checkbox text used in the pop-up login form to use cookies to retain user informatio </remarks>
        public string RememberMe { get; private set; }

        /// <remarks> Prompt for the user to logon to access the system features </remarks>
        public string TheFeatureYouAreTryingToAccessRequiresAValidLogon { get; private set; }

        /// <remarks> Used in the pop-up login form </remarks>
        public string UsernameOrEmail { get; private set; }

    }
}
