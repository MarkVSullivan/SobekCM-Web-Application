namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Preferences_MySobekViewer class </summary>
    public class Preferences_MySobekViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Preferences_MySobekViewer_Localization class </summary>
        public Preferences_MySobekViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Preferences_MySobekViewer";
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
                case "XXX Optionally Provides Access Through Gatorlink":
                    XXXOptionallyProvidesAccessThroughGatorlink = Value;
                    break;

                case "These Are The Default Rights You Give For Sharing Repurposing Or Remixing Your Item To Other Users You Can Set This With Each New Item You Submit But This Will Be The Default That Appears":
                    TheseAreTheDefaultRightsYouGiveForSharingRepurposingOrRemixingYourItemToOtherUsersYouCanSetThisWithEachNewItemYouSubmitButThisWillBeTheDefaultThatAppears = Value;
                    break;

                case "A Valid Email Is Required":
                    AValidEmailIsRequired = Value;
                    break;

                case "Account Information":
                    AccountInformation = Value;
                    break;

                case "Account Information Name And Email Are Required For Each New Account":
                    AccountInformationNameAndEmailAreRequiredForEachNewAccount = Value;
                    break;

                case "Already Registered Xxxlog Onxxx":
                    AlreadyRegisteredXxxlogOnxxx = Value;
                    break;

                case "An Account For That Email Address Already Exists":
                    AnAccountForThatEmailAddressAlreadyExists = Value;
                    break;

                case "CANCEL":
                    CANCEL = Value;
                    break;

                case "College":
                    College = Value;
                    break;

                case "Confirm Password":
                    ConfirmPassword = Value;
                    break;

                case "Current Affiliation Information":
                    CurrentAffiliationInformation = Value;
                    break;

                case "Default Metadata":
                    DefaultMetadata = Value;
                    break;

                case "Default Rights":
                    DefaultRights = Value;
                    break;

                case "Department":
                    Department = Value;
                    break;

                case "Edit Your Account Preferences":
                    EditYourAccountPreferences = Value;
                    break;

                case "Email":
                    Email = Value;
                    break;

                case "Family Names":
                    FamilyNames = Value;
                    break;

                case "Firstgiven Names":
                    FirstgivenNames = Value;
                    break;

                case "Given Names":
                    GivenNames = Value;
                    break;

                case "I Would Like To Be Able To Submit Materials Online Once Your Application To Submit Has Been Approved You Will Receive Email Notification":
                    IWouldLikeToBeAbleToSubmitMaterialsOnlineOnceYourApplicationToSubmitHasBeenApprovedYouWillReceiveEmailNotification = Value;
                    break;

                case "Language":
                    Language = Value;
                    break;

                case "Lastfamily Names":
                    LastfamilyNames = Value;
                    break;

                case "Nickname":
                    Nickname = Value;
                    break;

                case "Organizationuniversity":
                    Organizationuniversity = Value;
                    break;

                case "Other Preferences":
                    OtherPreferences = Value;
                    break;

                case "Password":
                    Password = Value;
                    break;

                case "Password Must Be At Least Eight Digits":
                    PasswordMustBeAtLeastEightDigits = Value;
                    break;

                case "Passwords Do Not Match":
                    PasswordsDoNotMatch = Value;
                    break;

                case "Personal Information":
                    PersonalInformation = Value;
                    break;

                case "Project":
                    Project = Value;
                    break;

                case "Register For XXX":
                    RegisterForXXX = Value;
                    break;

                case "Registration For XXX Is Free And Open To The Public Enter Your Information Below To Be Instantly Registered":
                    RegistrationForXXXIsFreeAndOpenToThePublicEnterYourInformationBelowToBeInstantlyRegistered = Value;
                    break;

                case "Rights Statement Truncated To 1000 Characters":
                    RightsStatementTruncatedTo1000Characters = Value;
                    break;

                case "Select And Confirm A Password":
                    SelectAndConfirmAPassword = Value;
                    break;

                case "Selfsubmittal Preferences":
                    SelfsubmittalPreferences = Value;
                    break;

                case "Send Me An Email When I Submit New Items":
                    SendMeAnEmailWhenISubmitNewItems = Value;
                    break;

                case "Send Me Monthly Usage Statistics For My Items":
                    SendMeMonthlyUsageStatisticsForMyItems = Value;
                    break;

                case "Template":
                    Template = Value;
                    break;

                case "That Username Is Taken Please Choose Another":
                    ThatUsernameIsTakenPleaseChooseAnother = Value;
                    break;

                case "The Following Errors Were Detected":
                    TheFollowingErrorsWereDetected = Value;
                    break;

                case "Ufids Are Always Eight Digits":
                    UfidsAreAlwaysEightDigits = Value;
                    break;

                case "Ufids Are Always Numeric":
                    UfidsAreAlwaysNumeric = Value;
                    break;

                case "Unit":
                    Unit = Value;
                    break;

                case "Username":
                    Username = Value;
                    break;

                case "Username Must Be At Least Eight Digits":
                    UsernameMustBeAtLeastEightDigits = Value;
                    break;

                case "You May Also Select A A Title Explanation Of Different Creative Commons Licenses Href Httpcreativecommonsorgaboutlicenses Creative Commons License A Option Below":
                    YouMayAlsoSelectAATitleExplanationOfDifferentCreativeCommonsLicensesHrefHttpcreativecommonsorgaboutlicensesCreativeCommonsLicenseAOptionBelow = Value;
                    break;

            }
        }
        /// <remarks> '%1 (optionally provides access through Gatorlink)' localization string </remarks>
        public string XXXOptionallyProvidesAccessThroughGatorlink { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string TheseAreTheDefaultRightsYouGiveForSharingRepurposingOrRemixingYourItemToOtherUsersYouCanSetThisWithEachNewItemYouSubmitButThisWillBeTheDefaultThatAppears { get; private set; }

        /// <remarks> Error if the user does not provide a valid email </remarks>
        public string AValidEmailIsRequired { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string AccountInformation { get; private set; }

        /// <remarks> Instructions on fields required during registration </remarks>
        public string AccountInformationNameAndEmailAreRequiredForEachNewAccount { get; private set; }

        /// <remarks> Link to logon if the user accidently ends up in registration </remarks>
        public string AlreadyRegisteredXxxlogOnxxx { get; private set; }

        /// <remarks> Error if an account already exists with the same email address </remarks>
        public string AnAccountForThatEmailAddressAlreadyExists { get; private set; }

        /// <remarks> 'CANCEL' localization string </remarks>
        public string CANCEL { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string College { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string ConfirmPassword { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string CurrentAffiliationInformation { get; private set; }

        /// <remarks> 'Default Metadata' localization string </remarks>
        public string DefaultMetadata { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string DefaultRights { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string Department { get; private set; }

        /// <remarks> Title when a registered/logged on user edits their account </remarks>
        public string EditYourAccountPreferences { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string Email { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string FamilyNames { get; private set; }

        /// <remarks> 'First/Given Name(s)' localization string </remarks>
        public string FirstgivenNames { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string GivenNames { get; private set; }

        /// <remarks> '"I would like to be able to submit materials online. (Once your application to submit has been approved, you will receive email notification)"' localization string </remarks>
        public string IWouldLikeToBeAbleToSubmitMaterialsOnlineOnceYourApplicationToSubmitHasBeenApprovedYouWillReceiveEmailNotification { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string Language { get; private set; }

        /// <remarks> 'Last/Family Name(s)' localization string </remarks>
        public string LastfamilyNames { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string Nickname { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string Organizationuniversity { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string OtherPreferences { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string Password { get; private set; }

        /// <remarks> Error if the password is too short </remarks>
        public string PasswordMustBeAtLeastEightDigits { get; private set; }

        /// <remarks> Error message if passwords don't match </remarks>
        public string PasswordsDoNotMatch { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string PersonalInformation { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string Project { get; private set; }

        /// <remarks> Title when a user is registering with the system initially </remarks>
        public string RegisterForXXX { get; private set; }

        /// <remarks> Prompt for registration with the system </remarks>
        public string RegistrationForXXXIsFreeAndOpenToThePublicEnterYourInformationBelowToBeInstantlyRegistered { get; private set; }

        /// <remarks> Warning if a very long rights statement is entered </remarks>
        public string RightsStatementTruncatedTo1000Characters { get; private set; }

        /// <remarks> Error message if a user does not enter any password information </remarks>
        public string SelectAndConfirmAPassword { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string SelfsubmittalPreferences { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string SendMeAnEmailWhenISubmitNewItems { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string SendMeMonthlyUsageStatisticsForMyItems { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string Template { get; private set; }

        /// <remarks> Error if the user chooses a username which already exists </remarks>
        public string ThatUsernameIsTakenPleaseChooseAnother { get; private set; }

        /// <remarks> 'The following errors were detected:' localization string </remarks>
        public string TheFollowingErrorsWereDetected { get; private set; }

        /// <remarks> 'UFIDs are always eight digits' localization string </remarks>
        public string UfidsAreAlwaysEightDigits { get; private set; }

        /// <remarks> 'UFIDs are always numeric' localization string </remarks>
        public string UfidsAreAlwaysNumeric { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string Unit { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string Username { get; private set; }

        /// <remarks> Error if the username is too short </remarks>
        public string UsernameMustBeAtLeastEightDigits { get; private set; }

        /// <remarks> Used for registration/account editing </remarks>
        public string YouMayAlsoSelectAATitleExplanationOfDifferentCreativeCommonsLicensesHrefHttpcreativecommonsorgaboutlicensesCreativeCommonsLicenseAOptionBelow { get; private set; }

    }
}
