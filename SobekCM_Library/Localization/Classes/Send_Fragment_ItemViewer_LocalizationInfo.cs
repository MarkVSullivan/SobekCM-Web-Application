namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Send_Fragment_ItemViewer class </summary>
    public class Send_Fragment_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Send_Fragment_ItemViewer_Localization class </summary>
        public Send_Fragment_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Send_Fragment_ItemViewer";
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
                case "Send This Item To A Friend":
                    SendThisItemToAFriend = Value;
                    break;

                case "Enter The Email Information Below":
                    EnterTheEmailInformationBelow = Value;
                    break;

                case "To":
                    To = Value;
                    break;

                case "Comments":
                    Comments = Value;
                    break;

                case "Format":
                    Format = Value;
                    break;

                case "HTML":
                    HTML = Value;
                    break;

                case "Text":
                    Text = Value;
                    break;

                case "Send":
                    Send = Value;
                    break;

                case "You Must Wait XXX Seconds Between Sending Emails":
                    YouMustWaitXXXSecondsBetweenSendingEmails = Value;
                    break;

                case "You Cannot Send To More Than XXX Email Addresses Simultaneously":
                    YouCannotSendToMoreThanXXXEmailAddressesS = Value;
                    break;

                case "You Have Reached Your Daily Quota For Emails":
                    YouHaveReachedYourDailyQuotaForEmails = Value;
                    break;

                case "Close":
                    Close = Value;
                    break;

                case "Plain Text":
                    PlainText = Value;
                    break;

                case "Cancel":
                    Cancel = Value;
                    break;

            }
        }
        /// <remarks> 'Send this Item to a Friend' localization string </remarks>
        public string SendThisItemToAFriend { get; private set; }

        /// <remarks> 'Enter the email information below' localization string </remarks>
        public string EnterTheEmailInformationBelow { get; private set; }

        /// <remarks> 'To:' localization string </remarks>
        public string To { get; private set; }

        /// <remarks> 'Comments:' localization string </remarks>
        public string Comments { get; private set; }

        /// <remarks> 'Format:' localization string </remarks>
        public string Format { get; private set; }

        /// <remarks> 'HTML:' localization string </remarks>
        public string HTML { get; private set; }

        /// <remarks> 'Text:' localization string </remarks>
        public string Text { get; private set; }

        /// <remarks> 'Send' localization string </remarks>
        public string Send { get; private set; }

        /// <remarks> 'You must wait %1 seconds between sending emails.' localization string </remarks>
        public string YouMustWaitXXXSecondsBetweenSendingEmails { get; private set; }

        /// <remarks> 'You cannot send to more than %1 email addresses simultaneously.' localization string </remarks>
        public string YouCannotSendToMoreThanXXXEmailAddressesS { get; private set; }

        /// <remarks> 'You have reached your daily quota for emails.' localization string </remarks>
        public string YouHaveReachedYourDailyQuotaForEmails { get; private set; }

        /// <remarks> 'Close' localization string </remarks>
        public string Close { get; private set; }

        /// <remarks> 'Plain Text' localization string </remarks>
        public string PlainText { get; private set; }

        /// <remarks> 'Cancel' localization string </remarks>
        public string Cancel { get; private set; }

    }
}
