namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Text_MainWriter class </summary>
    public class Text_MainWriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Text_MainWriter_Localization class </summary>
        public Text_MainWriter_LocalizationInfo() : base()
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
                case "Bibid":
                    Bibid = Value;
                    break;

                case "Default_Collection":
                    Default_Collection = Value;
                    break;

                case "File_Root":
                    File_Root = Value;
                    break;

                case "Greenstone_Code":
                    Greenstone_Code = Value;
                    break;

                case "Group":
                    Group = Value;
                    break;

                case "GROUP Table":
                    GROUPTable = Value;
                    break;

                case "Grouptitle":
                    Grouptitle = Value;
                    break;

                case "ICON Table":
                    ICONTable = Value;
                    break;

                case "Icon_Name":
                    Icon_Name = Value;
                    break;

                case "Icon_URL":
                    Icon_URL = Value;
                    break;

                case "Icons":
                    Icons = Value;
                    break;

                case "INTERFACE Table":
                    INTERFACETable = Value;
                    break;

                case "Invalid Group":
                    InvalidGroup = Value;
                    break;

                case "INVALID ITEM INDICATED":
                    INVALIDITEMINDICATED = Value;
                    break;

                case "Itemid":
                    Itemid = Value;
                    break;

                case "Items":
                    Items = Value;
                    break;

                case "Level1_Index":
                    Level1_Index = Value;
                    break;

                case "Level1_Text":
                    Level1_Text = Value;
                    break;

                case "Level2_Index":
                    Level2_Index = Value;
                    break;

                case "Level2_Text":
                    Level2_Text = Value;
                    break;

                case "Level3_Index":
                    Level3_Index = Value;
                    break;

                case "Level3_Text":
                    Level3_Text = Value;
                    break;

                case "Level4_Index":
                    Level4_Index = Value;
                    break;

                case "Level4_Text":
                    Level4_Text = Value;
                    break;

                case "Level5_Index":
                    Level5_Index = Value;
                    break;

                case "Level5_Text":
                    Level5_Text = Value;
                    break;

                case "Link":
                    Link = Value;
                    break;

                case "Skins":
                    Skins = Value;
                    break;

                case "TEXT WRITER UNKNOWN MODE":
                    TEXTWRITERUNKNOWNMODE = Value;
                    break;

                case "Title":
                    Title = Value;
                    break;

                case "Type":
                    Type = Value;
                    break;

            }
        }
        /// <remarks> 'BibID' localization string </remarks>
        public string Bibid { get; private set; }

        /// <remarks> 'Default_Collection' localization string </remarks>
        public string Default_Collection { get; private set; }

        /// <remarks> 'File_Root' localization string </remarks>
        public string File_Root { get; private set; }

        /// <remarks> 'Greenstone_Code' localization string </remarks>
        public string Greenstone_Code { get; private set; }

        /// <remarks> 'Group' localization string </remarks>
        public string Group { get; private set; }

        /// <remarks> 'GROUP table' localization string </remarks>
        public string GROUPTable { get; private set; }

        /// <remarks> 'GroupTitle' localization string </remarks>
        public string Grouptitle { get; private set; }

        /// <remarks> 'ICON table' localization string </remarks>
        public string ICONTable { get; private set; }

        /// <remarks> 'Icon_Name' localization string </remarks>
        public string Icon_Name { get; private set; }

        /// <remarks> 'Icon_URL' localization string </remarks>
        public string Icon_URL { get; private set; }

        /// <remarks> 'Icons' localization string </remarks>
        public string Icons { get; private set; }

        /// <remarks> 'INTERFACE table' localization string </remarks>
        public string INTERFACETable { get; private set; }

        /// <remarks> 'Invalid Group' localization string </remarks>
        public string InvalidGroup { get; private set; }

        /// <remarks> 'INVALID ITEM INDICATED' localization string </remarks>
        public string INVALIDITEMINDICATED { get; private set; }

        /// <remarks> 'ItemID' localization string </remarks>
        public string Itemid { get; private set; }

        /// <remarks> 'Items' localization string </remarks>
        public string Items { get; private set; }

        /// <remarks> 'Level1_Index' localization string </remarks>
        public string Level1_Index { get; private set; }

        /// <remarks> 'Level1_Text' localization string </remarks>
        public string Level1_Text { get; private set; }

        /// <remarks> 'Level2_Index' localization string </remarks>
        public string Level2_Index { get; private set; }

        /// <remarks> 'Level2_Text' localization string </remarks>
        public string Level2_Text { get; private set; }

        /// <remarks> 'Level3_Index' localization string </remarks>
        public string Level3_Index { get; private set; }

        /// <remarks> 'Level3_Text' localization string </remarks>
        public string Level3_Text { get; private set; }

        /// <remarks> 'Level4_Index' localization string </remarks>
        public string Level4_Index { get; private set; }

        /// <remarks> 'Level4_Text' localization string </remarks>
        public string Level4_Text { get; private set; }

        /// <remarks> 'Level5_Index' localization string </remarks>
        public string Level5_Index { get; private set; }

        /// <remarks> 'Level5_Text' localization string </remarks>
        public string Level5_Text { get; private set; }

        /// <remarks> 'Link' localization string </remarks>
        public string Link { get; private set; }

        /// <remarks> 'Skins' localization string </remarks>
        public string Skins { get; private set; }

        /// <remarks> 'TEXT WRITER - UNKNOWN MODE' localization string </remarks>
        public string TEXTWRITERUNKNOWNMODE { get; private set; }

        /// <remarks> 'Title' localization string </remarks>
        public string Title { get; private set; }

        /// <remarks> 'Type' localization string </remarks>
        public string Type { get; private set; }

    }
}
