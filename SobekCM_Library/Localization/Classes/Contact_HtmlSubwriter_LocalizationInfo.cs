namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Contact_HtmlSubwriter class </summary>
    public class Contact_HtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Contact_HtmlSubwriter_Localization class </summary>
        public Contact_HtmlSubwriter_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Contact_HtmlSubwriter";
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
                case "The Following Information Is Collected To Allow Us Better Serve Your Needs":
                    TheFollowingInformationIsCollectedToAllowUs = Value;
                    break;

                case "PERSONAL INFORMATION":
                    PERSONALINFORMATION = Value;
                    break;

                case "Name":
                    Name = Value;
                    break;

                case "Email":
                    Email = Value;
                    break;

                case "XXX Submission":
                    XXXSubmission = Value;
                    break;

                case "Submit":
                    Submit = Value;
                    break;

                case "Cancel":
                    Cancel = Value;
                    break;

                case "Your Email Has Been Sent":
                    YourEmailHasBeenSent = Value;
                    break;

                case "Click Here To Return To The Digital Collection Home":
                    ClickHereToReturnToTheDigitalCollectionHom = Value;
                    break;

                case "Click Here To Close This Tab In Your Browser":
                    ClickHereToCloseThisTabInYourBrowser = Value;
                    break;

                case "Contact Us":
                    ContactUs = Value;
                    break;

                case "Please Complete The Following Required Fields":
                    PleaseCompleteTheFollowingRequiredFields = Value;
                    break;

                case "Enter A Subject Here":
                    EnterASubjectHere = Value;
                    break;

                case "Describe Your Question Or Problem Here":
                    DescribeYourQuestionOrProblemHere = Value;
                    break;

                case "May We Contact You If So Please Provide The Following Information":
                    MayWeContactYouIfSoPleaseProvideTheFollow = Value;
                    break;

                case "Enter Your Name Here":
                    EnterYourNameHere = Value;
                    break;

                case "Enter Your Email Address Here":
                    EnterYourEmailAddressHere = Value;
                    break;

            }
        }
        /// <remarks> 'The following information is collected to allow us better serve your needs.' localization string </remarks>
        public string TheFollowingInformationIsCollectedToAllowUs { get; private set; }

        /// <remarks> 'PERSONAL INFORMATION' localization string </remarks>
        public string PERSONALINFORMATION { get; private set; }

        /// <remarks> 'Name:' localization string </remarks>
        public string Name { get; private set; }

        /// <remarks> 'Email:' localization string </remarks>
        public string Email { get; private set; }

        /// <remarks> '[ %1 Submission ]' localization string </remarks>
        public string XXXSubmission { get; private set; }

        /// <remarks> 'Submit' localization string </remarks>
        public string Submit { get; private set; }

        /// <remarks> 'Cancel' localization string </remarks>
        public string Cancel { get; private set; }

        /// <remarks> 'Your email has been sent.' localization string </remarks>
        public string YourEmailHasBeenSent { get; private set; }

        /// <remarks> 'Click here to return to the digital collection home' localization string </remarks>
        public string ClickHereToReturnToTheDigitalCollectionHom { get; private set; }

        /// <remarks> 'Click here to close this tab in your browser' localization string </remarks>
        public string ClickHereToCloseThisTabInYourBrowser { get; private set; }

        /// <remarks> 'Contact Us' localization string </remarks>
        public string ContactUs { get; private set; }

        /// <remarks> 'Please complete the following required fields:' localization string </remarks>
        public string PleaseCompleteTheFollowingRequiredFields { get; private set; }

        /// <remarks> 'Enter a subject here:' localization string </remarks>
        public string EnterASubjectHere { get; private set; }

        /// <remarks> 'Describe your question or problem here:' localization string </remarks>
        public string DescribeYourQuestionOrProblemHere { get; private set; }

        /// <remarks> '"May we contact you?  If so, please provide the following information:"' localization string </remarks>
        public string MayWeContactYouIfSoPleaseProvideTheFollow { get; private set; }

        /// <remarks> 'Enter your name here:' localization string </remarks>
        public string EnterYourNameHere { get; private set; }

        /// <remarks> 'Enter your e-mail address here:' localization string </remarks>
        public string EnterYourEmailAddressHere { get; private set; }

    }
}
