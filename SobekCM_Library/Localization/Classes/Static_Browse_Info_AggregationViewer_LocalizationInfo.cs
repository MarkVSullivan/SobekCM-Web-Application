namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Static_Browse_Info_AggregationViewer class </summary>
    public class Static_Browse_Info_AggregationViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Static_Browse_Info_AggregationViewer_Localization class </summary>
        public Static_Browse_Info_AggregationViewer_LocalizationInfo() : base()
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
                case "Author":
                    Author = Value;
                    break;

                case "Date":
                    Date = Value;
                    break;

                case "Description":
                    Description = Value;
                    break;

                case "Edit Content":
                    EditContent = Value;
                    break;

                case "Edit This Home Text":
                    EditThisHomeText = Value;
                    break;

                case "HTML Head Info":
                    HTMLHeadInfo = Value;
                    break;

                case "Keywords":
                    Keywords = Value;
                    break;

                case "Show Header Data Advanced":
                    ShowHeaderDataAdvanced = Value;
                    break;

                case "The Data Below Describes The Content Of This Static Child Page And Is Used By Some Search Engine Indexing Algorithms By Default It Will Not Show In Text Of The Page But Will Be Included In The Head Tag Of The Page":
                    TheDataBelowDescribesTheContentOfThisStaticChildPageAndIsUsedBySomeSearchEngineIndexingAlgorithmsByDefaultItWillNotShowInTextOfThePageButWillBeIncludedInTheHeadTagOfThePage = Value;
                    break;

                case "Title":
                    Title = Value;
                    break;

            }
        }
        /// <remarks> 'Author:' localization string </remarks>
        public string Author { get; private set; }

        /// <remarks> 'Date:' localization string </remarks>
        public string Date { get; private set; }

        /// <remarks> 'Description:' localization string </remarks>
        public string Description { get; private set; }

        /// <remarks> 'edit content' localization string </remarks>
        public string EditContent { get; private set; }

        /// <remarks> 'Edit this home text' localization string </remarks>
        public string EditThisHomeText { get; private set; }

        /// <remarks> 'HTML Head Info:' localization string </remarks>
        public string HTMLHeadInfo { get; private set; }

        /// <remarks> 'Keywords:' localization string </remarks>
        public string Keywords { get; private set; }

        /// <remarks> 'show header data (advanced)' localization string </remarks>
        public string ShowHeaderDataAdvanced { get; private set; }

        /// <remarks> '"The data below describes the content of this static child page and is used by some search engine indexing algorithms.  By default, it will not show in text of the page, but will be included in the head tag of the page."' localization string </remarks>
        public string TheDataBelowDescribesTheContentOfThisStaticChildPageAndIsUsedBySomeSearchEngineIndexingAlgorithmsByDefaultItWillNotShowInTextOfThePageButWillBeIncludedInTheHeadTagOfThePage { get; private set; }

        /// <remarks> 'Title:' localization string </remarks>
        public string Title { get; private set; }

    }
}
