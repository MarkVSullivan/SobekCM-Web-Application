namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the NewPassword_MySobekViewer class </summary>
    public class NewPassword_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the NewPassword_MySobekViewer_Localization class </summary>
        public NewPassword_MySobekViewer_LocalizationInfo() : base()
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
                case "Change Your Password":
                    ChangeYourPassword = Value;
                    break;

                case "Confirm New Password":
                    ConfirmNewPassword = Value;
                    break;

                case "Existing Password":
                    ExistingPassword = Value;
                    break;

                case "New Password":
                    NewPassword = Value;
                    break;

                case "New Passwords Do Not Match":
                    NewPasswordsDoNotMatch = Value;
                    break;

                case "Password Must Be At Least Eight Digits":
                    PasswordMustBeAtLeastEightDigits = Value;
                    break;

                case "Please Enter Your Existing Password And Your New Password":
                    PleaseEnterYourExistingPasswordAndYourNewPassword = Value;
                    break;

                case "Select And Confirm A New Password":
                    SelectAndConfirmANewPassword = Value;
                    break;

                case "The Following Errors Were Detected":
                    TheFollowingErrorsWereDetected = Value;
                    break;

                case "The New Password Cannot Match The Old Password":
                    TheNewPasswordCannotMatchTheOldPassword = Value;
                    break;

                case "Unable To Change Password Verify Current Password":
                    UnableToChangePasswordVerifyCurrentPassword = Value;
                    break;

                case "You Are Required To Change Your Password To Continue":
                    YouAreRequiredToChangeYourPasswordToContinue = Value;
                    break;

            }
        }
        /// <remarks> Title displayed when changing password </remarks>
        public string ChangeYourPassword { get; private set; }

        /// <remarks> Used when a user changes their password </remarks>
        public string ConfirmNewPassword { get; private set; }

        /// <remarks> Used when a user changes their password </remarks>
        public string ExistingPassword { get; private set; }

        /// <remarks> Used when a user changes their password </remarks>
        public string NewPassword { get; private set; }

        /// <remarks> Error message if passwords don't match while changing </remarks>
        public string NewPasswordsDoNotMatch { get; private set; }

        /// <remarks> Error message if password is too short </remarks>
        public string PasswordMustBeAtLeastEightDigits { get; private set; }

        /// <remarks> Prompt when a user opts to change their password </remarks>
        public string PleaseEnterYourExistingPasswordAndYourNewPassword { get; private set; }

        /// <remarks> Prompt for changing password </remarks>
        public string SelectAndConfirmANewPassword { get; private set; }

        /// <remarks> 'The following errors were detected:' localization string </remarks>
        public string TheFollowingErrorsWereDetected { get; private set; }

        /// <remarks> Error message if the user tries to change their password to their existing password </remarks>
        public string TheNewPasswordCannotMatchTheOldPassword { get; private set; }

        /// <remarks> Error message if the current password is incorrect </remarks>
        public string UnableToChangePasswordVerifyCurrentPassword { get; private set; }

        /// <remarks> Prompt when a user has a temporary password </remarks>
        public string YouAreRequiredToChangeYourPasswordToContinue { get; private set; }

    }
}
