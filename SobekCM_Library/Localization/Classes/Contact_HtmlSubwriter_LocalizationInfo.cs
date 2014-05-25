namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Contact_HtmlSubwriter class </summary>
    public class Contact_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Contact_HtmlSubwriter_Localization class </summary>
        public Contact_HtmlSubwriter_LocalizationInfo() : base()
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
                case "XXX Submission":
                    XXXSubmission = Value;
                    break;

                case "Cancel":
                    Cancel = Value;
                    break;

                case "Click Here To Close This Tab In Your Browser":
                    ClickHereToCloseThisTabInYourBrowser = Value;
                    break;

                case "Click Here To Return To The Digital Collection Home":
                    ClickHereToReturnToTheDigitalCollectionHome = Value;
                    break;

                case "Contact Us":
                    ContactUs = Value;
                    break;

                case "Describe Your Question Or Problem Here":
                    DescribeYourQuestionOrProblemHere = Value;
                    break;

                case "Email":
                    Email = Value;
                    break;

                case "Enter A Subject Here":
                    EnterASubjectHere = Value;
                    break;

                case "Enter Your Email Address Here":
                    EnterYourEmailAddressHere = Value;
                    break;

                case "Enter Your Name Here":
                    EnterYourNameHere = Value;
                    break;

                case "May We Contact You If So Please Provide The Following Information":
                    MayWeContactYouIfSoPleaseProvideTheFollowingInformation = Value;
                    break;

                case "Name":
                    Name = Value;
                    break;

                case "PERSONAL INFORMATION":
                    PERSONALINFORMATION = Value;
                    break;

                case "Please Complete The Following Required Fields":
                    PleaseCompleteTheFollowingRequiredFields = Value;
                    break;

                case "Submit":
                    Submit = Value;
                    break;

                case "The Following Information Is Collected To Allow Us Better Serve Your Needs":
                    TheFollowingInformationIsCollectedToAllowUsBetterServeYourNeeds = Value;
                    break;

                case "Your Email Has Been Sent":
                    YourEmailHasBeenSent = Value;
                    break;

            }
        }
        /// <remarks> "Included at the end of the subject line for emails sent ( i.e., ""[ dLOC Submission ]"")" </remarks>
        public string XXXSubmission { get; private set; }

        /// <remarks> 'Cancel' localization string </remarks>
        public string Cancel { get; private set; }

        /// <remarks> Prompts for the automatic contact us form in the system </remarks>
        public string ClickHereToCloseThisTabInYourBrowser { get; private set; }

        /// <remarks> Prompts for the automatic contact us form in the system </remarks>
        public string ClickHereToReturnToTheDigitalCollectionHome { get; private set; }

        /// <remarks> Prompts for the automatic contact us form in the system </remarks>
        public string ContactUs { get; private set; }

        /// <remarks> Prompts for the automatic contact us form in the system </remarks>
        public string DescribeYourQuestionOrProblemHere { get; private set; }

        /// <remarks> 'Email:' localization string </remarks>
        public string Email { get; private set; }

        /// <remarks> Prompts for the automatic contact us form in the system </remarks>
        public string EnterASubjectHere { get; private set; }

        /// <remarks> Prompts for the automatic contact us form in the system </remarks>
        public string EnterYourEmailAddressHere { get; private set; }

        /// <remarks> Prompts for the automatic contact us form in the system </remarks>
        public string EnterYourNameHere { get; private set; }

        /// <remarks> Prompts for the automatic contact us form in the system </remarks>
        public string MayWeContactYouIfSoPleaseProvideTheFollowingInformation { get; private set; }

        /// <remarks> 'Name:' localization string </remarks>
        public string Name { get; private set; }

        /// <remarks> 'PERSONAL INFORMATION' localization string </remarks>
        public string PERSONALINFORMATION { get; private set; }

        /// <remarks> Prompts for the automatic contact us form in the system </remarks>
        public string PleaseCompleteTheFollowingRequiredFields { get; private set; }

        /// <remarks> 'Submit' localization string </remarks>
        public string Submit { get; private set; }

        /// <remarks> Included in emails sent from the system via the 'Contact Us' feature </remarks>
        public string TheFollowingInformationIsCollectedToAllowUsBetterServeYourNeeds { get; private set; }

        /// <remarks> Prompts for the automatic contact us form in the system </remarks>
        public string YourEmailHasBeenSent { get; private set; }

    }
}
