namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Usage_Stats_Email_Helper class </summary>
    public class Usage_Stats_Email_Helper_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Usage_Stats_Email_Helper_Localization class </summary>
        public Usage_Stats_Email_Helper_LocalizationInfo() : base()
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
                case "XXX Views":
                    XXXViews = Value;
                    break;

                case "Below Are The Details For Your Top 10 Items See The Link Below To View Usage Statistics For All XXX Of Your Items":
                    BelowAreTheDetailsForYourTop10ItemsSeeTheLinkBelowToViewUsageStatisticsForAllXXXOfYourItems = Value;
                    break;

                case "Total To Date Since XXX":
                    TotalToDateSinceXXX = Value;
                    break;

                case "Usage Statistics For Your Materials XXX":
                    UsageStatisticsForYourMaterialsXXX = Value;
                    break;

                case "You Are Receiving This Message Because You Are A Contributor To A Digital Library Or A Collection Supported By The UF Libraries Including The Institutional Repository IRUF The UF Digital Collections UFDC The Digital Library Of The Caribbean Dloc And Many Others If You Do Not Wish To Receive Future Messages Please A Href Httpufdcufledumypreferences Edit Your Account Preferences A Online Or Send An Email To A Href Mailtoufdcuflibufledu Ufdcuflibufledu A P P Strong Usage Statistics For Your Materials DATE Strong P P NAME P P Thank You For Sharing Materials That Will Be Accessible Online And For Supporting Worldwide Open Access To Scholarly Creative And Other Works This Is A Usage Report For The Shared Materials P P Your Items Have Been Viewed TOTAL Times Since They Were Added And Were Viewed MONTHLY Times This Month P ITEMS P Em A Href Httpufdcufledumystats YEAR MONTH D Click Here To See The Usage Statistics For All Of Your Items Gtgt A Em P P Thank You For Sharing These Materials Please Contact Us With Any Questions A Href Mailtoufdcuflibufledu Ufdcuflibufledu A Or 3522732900":
                    YouAreReceivingThisMessageBecauseYouAreAContributorToADigitalLibraryOrACollectionSupportedByTheUFLibrariesIncludingTheInstitutionalRepositoryIRUFTheUFDigitalCollectionsUFDCTheDigitalLibraryOfTheCaribbeanDlocAndManyOthersIfYouDoNotWishToReceiveFutureMessagesPleaseAHrefHttpufdcufledumypreferencesEditYourAccountPreferencesAOnlineOrSendAnEmailToAHrefMailtoufdcuflibufleduUfdcuflibufleduAPPStrongUsageStatisticsForYourMateria = Value;
                    break;

            }
        }
        /// <remarks> %1 = number of views that month </remarks>
        public string XXXViews { get; private set; }

        /// <remarks> %1 = total number of items submitted by this person/institution </remarks>
        public string BelowAreTheDetailsForYourTop10ItemsSeeTheLinkBelowToViewUsageStatisticsForAllXXXOfYourItems { get; private set; }

        /// <remarks> %1=date added to repository.  Label for the total number of views since the item was added to the repository. </remarks>
        public string TotalToDateSinceXXX { get; private set; }

        /// <remarks> "%1 = Date (i.e., ""February 2012""), Subject of the email when emailing mothly usage reports." </remarks>
        public string UsageStatisticsForYourMaterialsXXX { get; private set; }

        /// <remarks> '"You are receiving this message because you are a contributor to a digital library or a collection supported by the UF Libraries, including the Institutional Repository (IR@UF), the UF Digital Collections (UFDC), the Digital Library of the Caribbean (dLOC), and many others. If you do not wish to receive future messages, please <a href=\""http://ufdc.ufl.edu/my/preferences\"">edit your account preferences</a> online or send an email to <a href=\""mailto:ufdc@uflib.ufl.edu\"">ufdc@uflib.ufl.edu</a>. </p>"" + ""<p><strong>Usage statistics for your materials ( <%DATE%> )</strong></p>"" + ""<p><%NAME%>,</p>"" + ""<p>Thank you for sharing materials that will be accessible online and for supporting worldwide open access to scholarly, creative, and other works.  This is a usage report for the shared materials.</p>"" + ""<p>Your items have been viewed <%TOTAL%> times since they were added and were viewed <%MONTHLY%> times this month</p>"" + ""<%ITEMS%>"" + ""<p><em><a href=\""http://ufdc.ufl.edu/my/stats/<%YEAR%><%MONTH%>d\"">Click here to see the usage statistics for all of your items. &gt;&gt;</a></em></p>"" + ""<p>Thank you for sharing these materials.  Please contact us with any questions ( <a href=\""mailto:ufdc@uflib.ufl.edu\"">ufdc@uflib.ufl.edu</a> or 352-273-2900)."' localization string </remarks>
        public string YouAreReceivingThisMessageBecauseYouAreAContributorToADigitalLibraryOrACollectionSupportedByTheUFLibrariesIncludingTheInstitutionalRepositoryIRUFTheUFDigitalCollectionsUFDCTheDigitalLibraryOfTheCaribbeanDlocAndManyOthersIfYouDoNotWishToReceiveFutureMessagesPleaseAHrefHttpufdcufledumypreferencesEditYourAccountPreferencesAOnlineOrSendAnEmailToAHrefMailtoufdcuflibufleduUfdcuflibufleduAPPStrongUsageStatisticsForYourMateria { get; private set; }

    }
}
