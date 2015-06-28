namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Usage_Stats_Email_Helper class </summary>
    public class Usage_Stats_Email_Helper_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Usage_Stats_Email_Helper_Localization class </summary>
        public Usage_Stats_Email_Helper_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Usage_Stats_Email_Helper";
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
                case "Usage Statistics For Your Materials XXX":
                    UsageStatisticsForYourMaterialsXXX = Value;
                    break;

                case "Below Are The Details For Your Top 10 Items See The Link Below To View Usage Statistics For All XXX Of Your Items":
                    BelowAreTheDetailsForYourTop10ItemsSeeTh = Value;
                    break;

                case "XXX Views":
                    XXXViews = Value;
                    break;

                case "Total To Date Since XXX":
                    TotalToDateSinceXXX = Value;
                    break;

                case "You Are Receiving This Message Because You Are A Contributor To A Digital Library Or A Collection Supported By The UF Libraries Including The Institutional Repository IRUF The UF Digital Collections UFDC The Digital Library Of The Caribbean Dloc And Many Others If You Do Not Wish To Receive Future Messages Please A Href Httpufdcufledumypreferences Edit Your Account Preferences A Online Or Send An Email To A Href Mailtoufdcuflibufledu Ufdcuflibufledu A P P Strong Usage Statistics For Your Materials DATE Strong P P NAME P P Thank You For Sharing Materials That Will Be Accessible Online And For Supporting Worldwide Open Access To Scholarly Creative And Other Works This Is A Usage Report For The Shared Materials P P Your Items Have Been Viewed TOTAL Times Since They Were Added And Were Viewed MONTHLY Times This Month P ITEMS P Em A Href Httpufdcufledumystats YEAR MONTH D Click Here To See The Usage Statistics For All Of Your Items Gtgt A Em P P Thank You For Sharing These Materials Please Contact Us With Any Questions A Href Mailtoufdcuflibufledu Ufdcuflibufledu A Or 3522732900":
                    YouAreReceivingThisMessageBecauseYouAreAC = Value;
                    break;

            }
        }

        /// <remarks> 'Usage statistics for your materials ( %1 )' localization string </remarks>
        public string UsageStatisticsForYourMaterialsXXX { get; private set; }

        /// <remarks> 'Below are the details for your top 10 items.  See the link below to view usage statistics for all %1 of your items.' localization string </remarks>
        public string BelowAreTheDetailsForYourTop10ItemsSeeTh { get; private set; }

        /// <remarks> '%1 views' localization string </remarks>
        public string XXXViews { get; private set; }

        /// <remarks> 'Total to date ( since %1 )' localization string </remarks>
        public string TotalToDateSinceXXX { get; private set; }

        /// <remarks> '"You are receiving this message because you are a contributor to a digital library or a collection supported by the UF Libraries, including the Institutional Repository (IR@UF), the UF Digital Collections (UFDC), the Digital Library of the Caribbean (dLOC), and many others. If you do not wish to receive future messages, please ... localization string </remarks>
        public string YouAreReceivingThisMessageBecauseYouAreAC { get; private set; }

    }
}
