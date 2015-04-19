namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the NewPassword_MySobekViewer class </summary>
    public class NewPassword_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the NewPassword_MySobekViewer_Localization class </summary>
        public NewPassword_MySobekViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "NewPassword_MySobekViewer";
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

                case "Select And Confirm A New Password":
                    SelectAndConfirmANewPassword = Value;
                    break;

                case "New Passwords Do Not Match":
                    NewPasswordsDoNotMatch = Value;
                    break;

                case "Password Must Be At Least Eight Digits":
                    PasswordMustBeAtLeastEightDigits = Value;
                    break;

                case "The New Password Cannot Match The Old Password":
                    TheNewPasswordCannotMatchTheOldPassword = Value;
                    break;

                case "Unable To Change Password Verify Current Password":
                    UnableToChangePasswordVerifyCurrentPassword = Value;
                    break;

                case "You Are Required To Change Your Password To Continue":
                    YouAreRequiredToChangeYourPasswordToContin = Value;
                    break;

                case "Please Enter Your Existing Password And Your New Password":
                    PleaseEnterYourExistingPasswordAndYourNewP = Value;
                    break;

                case "Existing Password":
                    ExistingPassword = Value;
                    break;

                case "New Password":
                    NewPassword = Value;
                    break;

                case "Confirm New Password":
                    ConfirmNewPassword = Value;
                    break;

                case "The Following Errors Were Detected":
                    TheFollowingErrorsWereDetected = Value;
                    break;

            }
        }
        /// <remarks> 'Change your password' localization string </remarks>
        public string ChangeYourPassword { get; private set; }

        /// <remarks> 'Select and confirm a new password' localization string </remarks>
        public string SelectAndConfirmANewPassword { get; private set; }

        /// <remarks> 'New passwords do not match' localization string </remarks>
        public string NewPasswordsDoNotMatch { get; private set; }

        /// <remarks> 'Password must be at least eight digits' localization string </remarks>
        public string PasswordMustBeAtLeastEightDigits { get; private set; }

        /// <remarks> 'The new password cannot match the old password' localization string </remarks>
        public string TheNewPasswordCannotMatchTheOldPassword { get; private set; }

        /// <remarks> 'Unable to change password.  Verify current password.' localization string </remarks>
        public string UnableToChangePasswordVerifyCurrentPassword { get; private set; }

        /// <remarks> 'You are required to change your password to continue.' localization string </remarks>
        public string YouAreRequiredToChangeYourPasswordToContin { get; private set; }

        /// <remarks> 'Please enter your existing password and your new password.' localization string </remarks>
        public string PleaseEnterYourExistingPasswordAndYourNewP { get; private set; }

        /// <remarks> 'Existing Password:' localization string </remarks>
        public string ExistingPassword { get; private set; }

        /// <remarks> 'New Password:' localization string </remarks>
        public string NewPassword { get; private set; }

        /// <remarks> 'Confirm New Password:' localization string </remarks>
        public string ConfirmNewPassword { get; private set; }

        /// <remarks> 'The following errors were detected:' localization string </remarks>
        public string TheFollowingErrorsWereDetected { get; private set; }

    }
}
