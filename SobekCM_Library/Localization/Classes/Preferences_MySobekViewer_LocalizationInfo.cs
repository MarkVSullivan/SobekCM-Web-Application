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
                case "Register For XXX":
                    RegisterForXXX = Value;
                    break;

                case "Edit Your Account Preferences":
                    EditYourAccountPreferences = Value;
                    break;

                case "Account Information":
                    AccountInformation = Value;
                    break;

                case "Username":
                    Username = Value;
                    break;

                case "Personal Information":
                    PersonalInformation = Value;
                    break;

                case "Family Names":
                    FamilyNames = Value;
                    break;

                case "Given Names":
                    GivenNames = Value;
                    break;

                case "Lastfamily Names":
                    LastfamilyNames = Value;
                    break;

                case "Firstgiven Names":
                    FirstgivenNames = Value;
                    break;

                case "Default Metadata":
                    DefaultMetadata = Value;
                    break;

                case "Nickname":
                    Nickname = Value;
                    break;

                case "Email":
                    Email = Value;
                    break;

                case "Send Me Monthly Usage Statistics For My Items":
                    SendMeMonthlyUsageStatisticsForMyItems = Value;
                    break;

                case "Current Affiliation Information":
                    CurrentAffiliationInformation = Value;
                    break;

                case "Organizationuniversity":
                    Organizationuniversity = Value;
                    break;

                case "College":
                    College = Value;
                    break;

                case "Department":
                    Department = Value;
                    break;

                case "Unit":
                    Unit = Value;
                    break;

                case "Selfsubmittal Preferences":
                    SelfsubmittalPreferences = Value;
                    break;

                case "Send Me An Email When I Submit New Items":
                    SendMeAnEmailWhenISubmitNewItems = Value;
                    break;

                case "Template":
                    Template = Value;
                    break;

                case "Project":
                    Project = Value;
                    break;

                case "Default Rights":
                    DefaultRights = Value;
                    break;

                case "These Are The Default Rights You Give For Sharing Repurposing Or Remixing Your Item To Other Users You Can Set This With Each New Item You Submit But This Will Be The Default That Appears":
                    TheseAreTheDefaultRightsYouGiveForSharing = Value;
                    break;

                case "You May Also Select A A Title Explanation Of Different Creative Commons Licenses Href Httpcreativecommonsorgaboutlicenses Creative Commons License A Option Below":
                    YouMayAlsoSelectAATitleExplanationOfDiffe = Value;
                    break;

                case "Other Preferences":
                    OtherPreferences = Value;
                    break;

                case "Language":
                    Language = Value;
                    break;

                case "Password":
                    Password = Value;
                    break;

                case "Confirm Password":
                    ConfirmPassword = Value;
                    break;

                case "Username Must Be At Least Eight Digits":
                    UsernameMustBeAtLeastEightDigits = Value;
                    break;

                case "Select And Confirm A Password":
                    SelectAndConfirmAPassword = Value;
                    break;

                case "Passwords Do Not Match":
                    PasswordsDoNotMatch = Value;
                    break;

                case "Password Must Be At Least Eight Digits":
                    PasswordMustBeAtLeastEightDigits = Value;
                    break;

                case "Ufids Are Always Eight Digits":
                    UfidsAreAlwaysEightDigits = Value;
                    break;

                case "Ufids Are Always Numeric":
                    UfidsAreAlwaysNumeric = Value;
                    break;

                case "A Valid Email Is Required":
                    AValidEmailIsRequired = Value;
                    break;

                case "Rights Statement Truncated To 1000 Characters":
                    RightsStatementTruncatedTo1000Characters = Value;
                    break;

                case "An Account For That Email Address Already Exists":
                    AnAccountForThatEmailAddressAlreadyExists = Value;
                    break;

                case "That Username Is Taken Please Choose Another":
                    ThatUsernameIsTakenPleaseChooseAnother = Value;
                    break;

                case "Registration For XXX Is Free And Open To The Public Enter Your Information Below To Be Instantly Registered":
                    RegistrationForXXXIsFreeAndOpenToThePubli = Value;
                    break;

                case "Account Information Name And Email Are Required For Each New Account":
                    AccountInformationNameAndEmailAreRequiredFo = Value;
                    break;

                case "Already Registered Xxxlog Onxxx":
                    AlreadyRegisteredXxxlogOnxxx = Value;
                    break;

                case "The Following Errors Were Detected":
                    TheFollowingErrorsWereDetected = Value;
                    break;

                case "XXX Optionally Provides Access Through Gatorlink":
                    XXXOptionallyProvidesAccessThroughGatorlink = Value;
                    break;

                case "I Would Like To Be Able To Submit Materials Online Once Your Application To Submit Has Been Approved You Will Receive Email Notification":
                    IWouldLikeToBeAbleToSubmitMaterialsOnline = Value;
                    break;

                case "CANCEL":
                    CANCEL = Value;
                    break;

            }
        }
        /// <remarks> 'Register for {0}' localization string </remarks>
        public string RegisterForXXX { get; private set; }

        /// <remarks> 'Edit Your Account Preferences' localization string </remarks>
        public string EditYourAccountPreferences { get; private set; }

        /// <remarks> 'Account Information' localization string </remarks>
        public string AccountInformation { get; private set; }

        /// <remarks> 'UserName' localization string </remarks>
        public string Username { get; private set; }

        /// <remarks> 'Personal Information' localization string </remarks>
        public string PersonalInformation { get; private set; }

        /// <remarks> 'Family Name(s)' localization string </remarks>
        public string FamilyNames { get; private set; }

        /// <remarks> 'Given Name(s)' localization string </remarks>
        public string GivenNames { get; private set; }

        /// <remarks> 'Last/Family Name(s)' localization string </remarks>
        public string LastfamilyNames { get; private set; }

        /// <remarks> 'First/Given Name(s)' localization string </remarks>
        public string FirstgivenNames { get; private set; }

        /// <remarks> 'Default Metadata' localization string </remarks>
        public string DefaultMetadata { get; private set; }

        /// <remarks> 'Nickname' localization string </remarks>
        public string Nickname { get; private set; }

        /// <remarks> 'Email' localization string </remarks>
        public string Email { get; private set; }

        /// <remarks> 'Send me monthly usage statistics for my items' localization string </remarks>
        public string SendMeMonthlyUsageStatisticsForMyItems { get; private set; }

        /// <remarks> 'Current Affiliation Information' localization string </remarks>
        public string CurrentAffiliationInformation { get; private set; }

        /// <remarks> 'Organization/University' localization string </remarks>
        public string Organizationuniversity { get; private set; }

        /// <remarks> 'College' localization string </remarks>
        public string College { get; private set; }

        /// <remarks> 'Department' localization string </remarks>
        public string Department { get; private set; }

        /// <remarks> 'Unit' localization string </remarks>
        public string Unit { get; private set; }

        /// <remarks> 'Self-Submittal Preferences' localization string </remarks>
        public string SelfsubmittalPreferences { get; private set; }

        /// <remarks> 'Send me an email when I submit new items' localization string </remarks>
        public string SendMeAnEmailWhenISubmitNewItems { get; private set; }

        /// <remarks> 'Template' localization string </remarks>
        public string Template { get; private set; }

        /// <remarks> 'Project' localization string </remarks>
        public string Project { get; private set; }

        /// <remarks> 'Default Rights' localization string </remarks>
        public string DefaultRights { get; private set; }

        /// <remarks> '"(These are the default rights you give for sharing, repurposing, or remixing your item to other users. You can set this with each new item you submit, but this will be the default that appears.)"' localization string </remarks>
        public string TheseAreTheDefaultRightsYouGiveForSharing { get; private set; }

        /// <remarks> '"You may also select a a title Explanation of different creative commons licenses. href=http://creativecommons.org/about/licenses/  Creative Commons License  option below."' localization string </remarks>
        public string YouMayAlsoSelectAATitleExplanationOfDiffe { get; private set; }

        /// <remarks> 'Other Preferences' localization string </remarks>
        public string OtherPreferences { get; private set; }

        /// <remarks> 'Language' localization string </remarks>
        public string Language { get; private set; }

        /// <remarks> 'Password' localization string </remarks>
        public string Password { get; private set; }

        /// <remarks> 'Confirm Password' localization string </remarks>
        public string ConfirmPassword { get; private set; }

        /// <remarks> 'Username must be at least eight digits' localization string </remarks>
        public string UsernameMustBeAtLeastEightDigits { get; private set; }

        /// <remarks> 'Select and confirm a password' localization string </remarks>
        public string SelectAndConfirmAPassword { get; private set; }

        /// <remarks> 'Passwords do not match' localization string </remarks>
        public string PasswordsDoNotMatch { get; private set; }

        /// <remarks> 'Password must be at least eight digits' localization string </remarks>
        public string PasswordMustBeAtLeastEightDigits { get; private set; }

        /// <remarks> 'UFIDs are always eight digits' localization string </remarks>
        public string UfidsAreAlwaysEightDigits { get; private set; }

        /// <remarks> 'UFIDs are always numeric' localization string </remarks>
        public string UfidsAreAlwaysNumeric { get; private set; }

        /// <remarks> 'A valid email is required' localization string </remarks>
        public string AValidEmailIsRequired { get; private set; }

        /// <remarks> 'Rights statement truncated to 1000 characters.' localization string </remarks>
        public string RightsStatementTruncatedTo1000Characters { get; private set; }

        /// <remarks> 'An account for that email address already exists.' localization string </remarks>
        public string AnAccountForThatEmailAddressAlreadyExists { get; private set; }

        /// <remarks> 'That username is taken.  Please choose another.' localization string </remarks>
        public string ThatUsernameIsTakenPleaseChooseAnother { get; private set; }

        /// <remarks> 'Registration for {0} is free and open to the public.  Enter your information below to be instantly registered.' localization string </remarks>
        public string RegistrationForXXXIsFreeAndOpenToThePubli { get; private set; }

        /// <remarks> '"Account information, name, and email are required for each new account."' localization string </remarks>
        public string AccountInformationNameAndEmailAreRequiredFo { get; private set; }

        /// <remarks> 'Already registered?  {0}Log on{1}.' localization string </remarks>
        public string AlreadyRegisteredXxxlogOnxxx { get; private set; }

        /// <remarks> 'The following errors were detected:' localization string </remarks>
        public string TheFollowingErrorsWereDetected { get; private set; }

        /// <remarks> '%1 (optionally provides access through Gatorlink)' localization string </remarks>
        public string XXXOptionallyProvidesAccessThroughGatorlink { get; private set; }

        /// <remarks> '"I would like to be able to submit materials online. (Once your application to submit has been approved, you will receive email notification)"' localization string </remarks>
        public string IWouldLikeToBeAbleToSubmitMaterialsOnline { get; private set; }

        /// <remarks> 'CANCEL' localization string </remarks>
        public string CANCEL { get; private set; }

    }
}
