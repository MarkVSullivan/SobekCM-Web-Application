namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Logon_MySobekViewer class </summary>
    public class Logon_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Logon_MySobekViewer_Localization class </summary>
        public Logon_MySobekViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Logon_MySobekViewer";
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
                case "Logon To XXX":
                    LogonToXXX = Value;
                    break;

                case "The Feature You Are Trying To Access Requires A Valid Logon":
                    TheFeatureYouAreTryingToAccessRequiresAVa = Value;
                    break;

                case "Please Choose The Appropriate Logon Directly Below":
                    PleaseChooseTheAppropriateLogonDirectlyBelow = Value;
                    break;

                case "If You Have A Valid XXX Logon Xxxsign On With XXX Authentication Herexxx":
                    IfYouHaveAValidXXXLogonXxxsignOnWithXXX = Value;
                    break;

                case "Not Registered Yet Xxxregister Nowxxx Or Xxxcontact Usxxx":
                    NotRegisteredYetXxxregisterNowxxxOrXxxcontac = Value;
                    break;

                case "LOG IN":
                    LOGIN = Value;
                    break;

                case "Username Or Email":
                    UsernameOrEmail = Value;
                    break;

                case "Password":
                    Password = Value;
                    break;

                case "Remember Me":
                    RememberMe = Value;
                    break;

                case "Not Registered Yet Xxxregister Nowxxx":
                    NotRegisteredYetXxxregisterNowxxx = Value;
                    break;

                case "Forgot Your Username Or Password Please Xxxcontact Usxxx":
                    ForgotYourUsernameOrPasswordPleaseXxxcontact = Value;
                    break;

            }
        }
        /// <remarks> 'Logon to {0}' localization string </remarks>
        public string LogonToXXX { get; private set; }

        /// <remarks> 'The feature you are trying to access requires a valid logon.' localization string </remarks>
        public string TheFeatureYouAreTryingToAccessRequiresAVa { get; private set; }

        /// <remarks> 'Please choose the appropriate logon directly below.' localization string </remarks>
        public string PleaseChooseTheAppropriateLogonDirectlyBelow { get; private set; }

        /// <remarks> '"<b>If you have a valid {0} logon</b>, {1}Sign on with {0} authentication here{2}."' localization string </remarks>
        public string IfYouHaveAValidXXXLogonXxxsignOnWithXXX { get; private set; }

        /// <remarks> '<b>Not registered yet?</b> {0}Register now{1} or {2}Contact Us{1}.' localization string </remarks>
        public string NotRegisteredYetXxxregisterNowxxxOrXxxcontac { get; private set; }

        /// <remarks> 'LOG IN' localization string </remarks>
        public string LOGIN { get; private set; }

        /// <remarks> 'Username or email:' localization string </remarks>
        public string UsernameOrEmail { get; private set; }

        /// <remarks> 'Password:' localization string </remarks>
        public string Password { get; private set; }

        /// <remarks> 'Remember me' localization string </remarks>
        public string RememberMe { get; private set; }

        /// <remarks> 'Not registered yet?  {0}Register now{1}' localization string </remarks>
        public string NotRegisteredYetXxxregisterNowxxx { get; private set; }

        /// <remarks> 'Forgot your username or password?  Please {0}contact us{1}.' localization string </remarks>
        public string ForgotYourUsernameOrPasswordPleaseXxxcontact { get; private set; }

    }
}
