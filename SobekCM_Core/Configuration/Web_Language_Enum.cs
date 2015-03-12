#region Using directives

using System;

#endregion

namespace SobekCM.Core.Configuration
{
    /// <summary> Enumeration of all the most commonly supported languages by 
    /// major browsers  </summary>
    public enum Web_Language_Enum : byte 
    {
        /// <summary> No matching language was detected or no detection completed yet </summary>
        UNDEFINED,

        /// <summary> Special value is used when the default language should be used </summary>
        DEFAULT,

        TEMPLATE,

        /// <summary> Afrikaans web language ( af )</summary>
        Afrikaans,

        /// <summary> Albanian web language ( sq )</summary>
        Albanian,

        /// <summary> Arabic web language ( ar )</summary>
        Arabic,

        /// <summary> Aragonese web language ( ar )</summary>
        Aragonese,

        /// <summary> Armenian web language ( hy )</summary>
        Armenian,

        /// <summary> Assamese web language ( as )</summary>
        Assamese,

        /// <summary> Asturian web language ( ast )</summary>
        Asturian,

        /// <summary> Azerbaijani web language ( az )</summary>
        Azerbaijani,

        /// <summary> Basque web language ( eu )</summary>
        Basque,

        /// <summary> Bulgarian web language ( bg )</summary>
        Bulgarian,

        /// <summary> Belarusian web language ( be )</summary>
        Belarusian,

        /// <summary> Bengali web language ( bn )</summary>
        Bengali,

        /// <summary> Bosnian web language ( bs )</summary>
        Bosnian,

        /// <summary> Breton web language ( br )</summary>
        Breton,

        /// <summary> Burmese web language ( my )</summary>
        Burmese,

        /// <summary> Catalan web language ( ca )</summary>
        Catalan,

        /// <summary> Chamorro web language ( ch )</summary>
        Chamorro,

        /// <summary> Chechen web language ( ce )</summary>
        Chechen,

        /// <summary> Chinese web language ( zh )</summary>
        Chinese,

        /// <summary> Chuvash web language ( cv )</summary>
        Chuvash,

        /// <summary> Corsican web language ( co )</summary>
        Corsican,

        /// <summary> Cree web language ( cr )</summary>
        Cree,

        /// <summary> Croatian web language ( hr )</summary>
        Croatian,

        /// <summary> Czech web language ( cs )</summary>
        Czech,

        /// <summary> Danish web language ( da )</summary>
        Danish,

        /// <summary> Dutch web language ( nl )</summary>
        Dutch,

        /// <summary> English web language ( en )</summary>
        English,

        /// <summary> Esperanto web language ( eo )</summary>
        Esperanto,

        /// <summary> Estonian web language ( et )</summary>
        Estonian,

        /// <summary> Faeroese web language ( fo )</summary>
        Faeroese,

        /// <summary> Farsi web language ( fa )</summary>
        Farsi,

        /// <summary> Fijian web language ( fj )</summary>
        Fijian,

        /// <summary> Finnish web language ( fi )</summary>
        Finnish,

        /// <summary> French web language ( fr )</summary>
        French,

        /// <summary> Frisian web language ( fy )</summary>
        Frisian,

        /// <summary> Friulian web language ( fur )</summary>
        Friulian,

        /// <summary> Gaelic web language ( gd )</summary>
        Gaelic,

        /// <summary> Galacian web language ( gl )</summary>
        Galacian,

        /// <summary> Georgian web language ( ka )</summary>
        Georgian,

        /// <summary> German web language ( de )</summary>
        German,

        /// <summary> Greek web language ( el )</summary>
        Greek,

		/// <summary> Gujarati web language ( gu )</summary>
		Gujarati,

        /// <summary> Haitian web language ( ht )</summary>
        Haitian,

        /// <summary> Hebrew web language ( he )</summary>
        Hebrew,

        /// <summary> Hindi web language ( hi )</summary>
        Hindi,

        /// <summary> Hungarian web language ( hu )</summary>
        Hungarian,

        /// <summary> Icelandic web language ( is )</summary>
        Icelandic,

        /// <summary> Indonesian web language ( id )</summary>
        Indonesian,

        /// <summary> Inuktitut web language ( iu )</summary>
        Inuktitut,

        /// <summary> Irish web language ( ga )</summary>
        Irish,

        /// <summary> Italian web language ( it )</summary>
        Italian,

        /// <summary> Japanese web language ( ja )</summary>
        Japanese,

        /// <summary> Kannada web language ( kn )</summary>
        Kannada,

        /// <summary> Kashmiri web language ( ks )</summary>
        Kashmiri,

        /// <summary> Kazakh web language ( kk )</summary>
        Kazakh,

        /// <summary> Khmer web language ( km )</summary>
        Khmer,

        /// <summary> Kinyarwanda language (rw) </summary>
        Kinyarwanda, 

        /// <summary> Kirghiz web language ( ky )</summary>
        Kirghiz,

        /// <summary> Klingon web language ( tlh )</summary>
        Klingon,

        /// <summary> Korean web language ( ko )</summary>
        Korean,

        /// <summary> Latin web language ( la )</summary>
        Latin,

        /// <summary> Latvian web language ( lv )</summary>
        Latvian,

        /// <summary> Lithuanian web language ( lt )</summary>
        Lithuanian,

        /// <summary> Luxembourgish web language ( lb )</summary>
        Luxembourgish,

        /// <summary> FYRO Macedonian web language ( mk )</summary>
        FYRO_Macedonian,

        /// <summary> Malay web language ( ms )</summary>
        Malay,

        /// <summary> Malayalam web language ( ml )</summary>
        Malayalam,

        /// <summary> Maltese web language ( mt )</summary>
        Maltese,

        /// <summary> Maori web language ( mi )</summary>
        Maori,

        /// <summary> Marathi web language ( mr )</summary>
        Marathi,

        /// <summary> Moldavian web language ( mo )</summary>
        Moldavian,

        /// <summary> Navajo web language ( nv )</summary>
        Navajo,

        /// <summary> Ndonga web language ( ng )</summary>
        Ndonga,

        /// <summary> Nepali web language ( ne )</summary>
        Nepali,

        /// <summary> Norwegian web language ( no )</summary>
        Norwegian,

        /// <summary> Norwegian (Bokmal) web language ( nb )</summary>
        Norwegian_Bokmal,

        /// <summary> Norwegian (Nynorsk) web language ( nn )</summary>
        Norwegian_Nynorsk,

        /// <summary> Occitan web language ( oc )</summary>
        Occitan,

        /// <summary> Oriya web language ( or )</summary>
        Oriya,

        /// <summary> Oromo web language ( om )</summary>
        Oromo,

        /// <summary> Aruban Papiamento (papo - custom code) </summary>
        /// <remarks> No established browser code exists for this </remarks>
        Papiamento,

        /// <summary> Curacao Papiamentu (papu - custom code) </summary>
        /// <remarks> No established browser code exists for this </remarks>
        Papiamentu,

        /// <summary> Persian web language ( fa )</summary>
        Persian,

        /// <summary> Polish web language ( pl )</summary>
        Polish,

        /// <summary> Portuguese web language ( pt )</summary>
        Portuguese,

        /// <summary> Punjabi web language ( pa )</summary>
        Punjabi,

        /// <summary> Quechua web language ( qu )</summary>
        Quechua,

        /// <summary> Rhaeto-Romanic web language ( rm )</summary>
        Rhaeto_Romanic,

        /// <summary> Romanian web language ( ro )</summary>
        Romanian,

        /// <summary> Russian web language ( ru )</summary>
        Russian,

        /// <summary> Sami (Lappish) web language ( sz )</summary>
        Sami_Lappish,

        /// <summary> Sango web language ( sg )</summary>
        Sango,

        /// <summary> Sanskrit web language ( sa )</summary>
        Sanskrit,

        /// <summary> Sardinian web language ( sc )</summary>
        Sardinian,

        /// <summary> Scots Gaelic web language ( gd )</summary>
        Scots_Gaelic,

        /// <summary> Sindhi web language ( sd )</summary>
        Sindhi,

        /// <summary> Singhalese web language ( si )</summary>
        Singhalese,

        /// <summary> Serbian web language ( sr )</summary>
        Serbian,

        /// <summary> Slovak web language ( sk )</summary>
        Slovak,

        /// <summary> Slovenian web language ( sl )</summary>
        Slovenian,

        /// <summary> Somani web language ( so )</summary>
        Somani,

        /// <summary> Sorbian web language ( sb )</summary>
        Sorbian,

        /// <summary> Spanish web language ( es )</summary>
        Spanish,

        /// <summary> Sutu web language ( sx )</summary>
        Sutu,

        /// <summary> Swahili web language ( sw )</summary>
        Swahili,

        /// <summary> Swedish web language ( sv )</summary>
        Swedish,

        /// <summary> Tamil web language ( ta )</summary>
        Tamil,

        /// <summary> Tatar web language ( tt )</summary>
        Tatar,

        /// <summary> Telugu web language ( te )</summary>
        Telugu,

        /// <summary> Thai web language ( th )</summary>
        Thai,

        /// <summary> Tigre web language ( tig )</summary>
        Tigre,

        /// <summary> Tsonga web language ( ts )</summary>
        Tsonga,

        /// <summary> Tswana web language ( tn )</summary>
        Tswana,

        /// <summary> Turkish web language ( tr )</summary>
        Turkish,

        /// <summary> Turkmen web language ( tk )</summary>
        Turkmen,

        /// <summary> Ukrainian web language ( uk )</summary>
        Ukrainian,

        /// <summary> Upper Sorbian web language ( hsb )</summary>
        Upper_Sorbian,

        /// <summary> Urdu web language ( ur )</summary>
        Urdu,

        /// <summary> Venda web language ( ve )</summary>
        Venda,

        /// <summary> Vietnamese web language ( vi )</summary>
        Vietnamese,

        /// <summary> Volapuk web language ( vo )</summary>
        Volapuk,

        /// <summary> Walloon web language ( wa )</summary>
        Walloon,

        /// <summary> Welsh web language ( cy )</summary>
        Welsh,

        /// <summary> Xhosa web language ( xh )</summary>
        Xhosa,

        /// <summary> Yiddish web language ( ji )</summary>
        Yiddish,

        /// <summary> Zulu web language ( zu )</summary>
        Zulu
    }

    /// <summary> Static class is used to change between codes and language names to the language enumeration  </summary>
    public static class Web_Language_Enum_Converter
    {
        /// <summary> Gets the array of different language names supported by this system </summary>
        public static string[] Language_Name_Array
        {
            get
            {
				return new[] { "Afrikaans", "Albanian", "Arabic", "Aragonese", "Armenian", "Assamese", "Asturian", "Azerbaijani", "Basque", "Bulgarian", "Belarusian", "Bengali", "Bosnian", "Breton", "Bulgarian", "Burmese", "Catalan", "Chamorro", "Chechen", "Chinese", "Chuvash", "Corsican", "Cree", "Croatian", "Czech", "Danish", "Dutch", "English", "Esperanto", "Estonian", "Faeroese", "Farsi", "Fijian", "Finnish", "French", "Frisian", "Friulian", "Gaelic", "Galacian", "Georgian", "German", "Greek", "Gujarati", "Haitian", "Hebrew", "Hindi", "Hungarian", "Icelandic", "Indonesian", "Inuktitut", "Irish", "Italian", "Japanese", "Kannada", "Kashmiri", "Kazakh", "Khmer", "Kinyarwanda", "Kirghiz", "Klingon", "Korean", "Latin", "Latvian", "Lithuanian", "Luxembourgish", "FYRO Macedonian", "Malay", "Malayalam", "Maltese", "Maori", "Marathi", "Moldavian", "Navajo", "Ndonga", "Nepali", "Norwegian", "Norwegian (Bokmal)", "Norwegian (Nynorsk)", "Occitan", "Oriya", "Oromo", "Papiamento", "Papiamentu", "Persian", "Polish", "Portuguese", "Punjabi", "Quechua", "Rhaeto-Romanic", "Romanian", "Russian", "Sami (Lappish)", "Sango", "Sanskrit", "Sardinian", "Scots Gaelic", "Sindhi", "Singhalese", "Serbian", "Slovak", "Slovenian", "Somani", "Sorbian", "Spanish", "Sutu", "Swahili", "Swedish", "Tamil", "Tatar", "Telugu", "Thai", "Tigre", "Tsonga", "Tswana", "Turkish", "Turkmen", "Ukrainian", "Upper Sorbian", "Urdu", "Venda", "Vietnamese", "Volapuk", "Walloon", "Welsh", "Xhosa", "Yiddish", "Zulu" };
            }
        }

        /// <summary> Converts the web browser code to the web language enumeration </summary>
        /// <param name="Code"> Code for the browser </param>
        /// <returns> Enumeration for the language, as specified by the browser code</returns>
        /// <remarks> If this is a sub-language, or refinement code, like 'en-us', and there is no match
        /// this will call itself recursively to see if the base language ('en' for example) is
        /// recognized.  </remarks>
        public static Web_Language_Enum Code_To_Enum( string Code )
        {
            switch( Code.ToLower() )
            {
                case "af":
	                return Web_Language_Enum.Afrikaans;

                case "sq":
	                return Web_Language_Enum.Albanian;

                case "ar":
	                return Web_Language_Enum.Arabic;

                case "hy":
	                return Web_Language_Enum.Armenian;

                case "as":
	                return Web_Language_Enum.Assamese;

                case "ast":
	                return Web_Language_Enum.Asturian;

                case "az":
	                return Web_Language_Enum.Azerbaijani;

                case "eu":
	                return Web_Language_Enum.Basque;

                case "bg":
	                return Web_Language_Enum.Bulgarian;

                case "be":
	                return Web_Language_Enum.Belarusian;

                case "bn":
	                return Web_Language_Enum.Bengali;

                case "bs":
	                return Web_Language_Enum.Bosnian;

                case "br":
	                return Web_Language_Enum.Breton;

                case "my":
	                return Web_Language_Enum.Burmese;

                case "ca":
	                return Web_Language_Enum.Catalan;

                case "ch":
	                return Web_Language_Enum.Chamorro;

                case "ce":
	                return Web_Language_Enum.Chechen;

                case "zh":
	                return Web_Language_Enum.Chinese;

                case "cv":
	                return Web_Language_Enum.Chuvash;

                case "co":
	                return Web_Language_Enum.Corsican;

                case "cr":
	                return Web_Language_Enum.Cree;

                case "hr":
	                return Web_Language_Enum.Croatian;

                case "cs":
	                return Web_Language_Enum.Czech;

                case "da":
	                return Web_Language_Enum.Danish;

                case "nl":
	                return Web_Language_Enum.Dutch;

                case "en":
	                return Web_Language_Enum.English;

                case "eo":
	                return Web_Language_Enum.Esperanto;

                case "et":
	                return Web_Language_Enum.Estonian;

                case "fo":
	                return Web_Language_Enum.Faeroese;

                case "fa":
	                return Web_Language_Enum.Farsi;

                case "fj":
	                return Web_Language_Enum.Fijian;

                case "fi":
	                return Web_Language_Enum.Finnish;

                case "fr":
	                return Web_Language_Enum.French;

                case "fy":
	                return Web_Language_Enum.Frisian;

                case "fur":
	                return Web_Language_Enum.Friulian;

                case "gd":
	                return Web_Language_Enum.Gaelic;

                case "gl":
	                return Web_Language_Enum.Galacian;

                case "ka":
	                return Web_Language_Enum.Georgian;

                case "de":
	                return Web_Language_Enum.German;

                case "el":
	                return Web_Language_Enum.Greek;

                case "gu":
					return Web_Language_Enum.Gujarati;

                case "ht":
	                return Web_Language_Enum.Haitian;

                case "he":
	                return Web_Language_Enum.Hebrew;

                case "hi":
	                return Web_Language_Enum.Hindi;

                case "hu":
	                return Web_Language_Enum.Hungarian;

                case "is":
	                return Web_Language_Enum.Icelandic;

                case "id":
	                return Web_Language_Enum.Indonesian;

                case "iu":
	                return Web_Language_Enum.Inuktitut;

                case "ga":
	                return Web_Language_Enum.Irish;

                case "it":
	                return Web_Language_Enum.Italian;

                case "ja":
	                return Web_Language_Enum.Japanese;

                case "kn":
	                return Web_Language_Enum.Kannada;

                case "ks":
	                return Web_Language_Enum.Kashmiri;

                case "kk":
	                return Web_Language_Enum.Kazakh;

                case "km":
	                return Web_Language_Enum.Khmer;

                case "rw":
                    return Web_Language_Enum.Kinyarwanda;

                case "ky":
	                return Web_Language_Enum.Kirghiz;

                case "tlh":
	                return Web_Language_Enum.Klingon;

                case "ko":
	                return Web_Language_Enum.Korean;

                case "la":
	                return Web_Language_Enum.Latin;

                case "lv":
	                return Web_Language_Enum.Latvian;

                case "lt":
	                return Web_Language_Enum.Lithuanian;

                case "lb":
	                return Web_Language_Enum.Luxembourgish;

                case "mk":
	                return Web_Language_Enum.FYRO_Macedonian;

                case "ms":
	                return Web_Language_Enum.Malay;

                case "ml":
	                return Web_Language_Enum.Malayalam;

                case "mt":
	                return Web_Language_Enum.Maltese;

                case "mi":
	                return Web_Language_Enum.Maori;

                case "mr":
	                return Web_Language_Enum.Marathi;

                case "mo":
	                return Web_Language_Enum.Moldavian;

                case "nv":
	                return Web_Language_Enum.Navajo;

                case "ng":
	                return Web_Language_Enum.Ndonga;

                case "ne":
	                return Web_Language_Enum.Nepali;

                case "no":
	                return Web_Language_Enum.Norwegian;

                case "nb":
	                return Web_Language_Enum.Norwegian_Bokmal;

                case "nn":
	                return Web_Language_Enum.Norwegian_Nynorsk;

                case "oc":
	                return Web_Language_Enum.Occitan;

                case "or":
	                return Web_Language_Enum.Oriya;

                case "om":
	                return Web_Language_Enum.Oromo;

                case "pl":
	                return Web_Language_Enum.Polish;

                case "pt":
	                return Web_Language_Enum.Portuguese;

                case "pa":
	                return Web_Language_Enum.Punjabi;

                case "papo":
                    return Web_Language_Enum.Papiamento;

                case "papu":
                    return Web_Language_Enum.Papiamentu;

                case "qu":
	                return Web_Language_Enum.Quechua;

                case "rm":
	                return Web_Language_Enum.Rhaeto_Romanic;

                case "ro":
	                return Web_Language_Enum.Romanian;

                case "ru":
	                return Web_Language_Enum.Russian;

                case "sz":
	                return Web_Language_Enum.Sami_Lappish;

                case "sg":
	                return Web_Language_Enum.Sango;

                case "sa":
	                return Web_Language_Enum.Sanskrit;

                case "sc":
	                return Web_Language_Enum.Sardinian;

                case "sd":
	                return Web_Language_Enum.Sindhi;

                case "si":
	                return Web_Language_Enum.Singhalese;

                case "sr":
	                return Web_Language_Enum.Serbian;

                case "sk":
	                return Web_Language_Enum.Slovak;

                case "sl":
	                return Web_Language_Enum.Slovenian;

                case "so":
	                return Web_Language_Enum.Somani;

                case "sb":
	                return Web_Language_Enum.Sorbian;

                case "es":
                case "sp":
	                return Web_Language_Enum.Spanish;

                case "sx":
	                return Web_Language_Enum.Sutu;

                case "sw":
	                return Web_Language_Enum.Swahili;

                case "sv":
	                return Web_Language_Enum.Swedish;

                case "ta":
	                return Web_Language_Enum.Tamil;

                case "tt":
	                return Web_Language_Enum.Tatar;

                case "te":
	                return Web_Language_Enum.Telugu;

                case "th":
	                return Web_Language_Enum.Thai;

                case "tig":
	                return Web_Language_Enum.Tigre;

                case "ts":
	                return Web_Language_Enum.Tsonga;

                case "tn":
	                return Web_Language_Enum.Tswana;

                case "tr":
	                return Web_Language_Enum.Turkish;

                case "tk":
	                return Web_Language_Enum.Turkmen;

                case "uk":
	                return Web_Language_Enum.Ukrainian;

                case "hsb":
	                return Web_Language_Enum.Upper_Sorbian;

                case "ur":
	                return Web_Language_Enum.Urdu;

                case "ve":
	                return Web_Language_Enum.Venda;

                case "vi":
	                return Web_Language_Enum.Vietnamese;

                case "vo":
	                return Web_Language_Enum.Volapuk;

                case "wa":
	                return Web_Language_Enum.Walloon;

                case "cy":
	                return Web_Language_Enum.Welsh;

                case "xh":
	                return Web_Language_Enum.Xhosa;

                case "ji":
	                return Web_Language_Enum.Yiddish;

                case "zu":
	                return Web_Language_Enum.Zulu;

                default:
                    // Is this perhaps a sub-language?
                    if (Code.IndexOf("-") > 0)
                    {
                        string[] splitter = Code.Split("-".ToCharArray());
                        return Code_To_Enum(splitter[0]);
                    }
                    return Web_Language_Enum.UNDEFINED;
            }
        }

        /// <summary> Given a language name, returns the associated browser code </summary>
        /// <param name="Language_Name"> Name of the language </param>
        /// <returns> Code " the language </returns>
        public static string Name_To_Code(string Language_Name)
        {
            switch ( Language_Name.ToLower() )
            {
                case "afrikaans":
                    return "af";

                case "albanian":
                    return "sq";

                case "arabic":
                    return "ar";

                case "aragonese":
                    return "ar";

                case "armenian":
                    return "hy";

                case "assamese":
                    return "as";

                case "asturian":
                    return "ast";

                case "azerbaijani":
                    return "az";

                case "basque":
                    return "eu";

                case "bulgarian":
                    return "bg";

                case "belarusian":
                    return "be";

                case "bengali":
                    return "bn";

                case "bosnian":
                    return "bs";

                case "breton":
                    return "br";

                case "burmese":
                    return "my";

                case "catalan":
                    return "ca";

                case "chamorro":
                    return "ch";

                case "chechen":
                    return "ce";

                case "chinese":
                    return "zh";

                case "chuvash":
                    return "cv";

                case "corsican":
                    return "co";

                case "cree":
                    return "cr";

                case "croatian":
                    return "hr";

                case "czech":
                    return "cs";

                case "danish":
                    return "da";

                case "dutch":
                    return "nl";

                case "english":
                    return "en";

                case "esperanto":
                    return "eo";

                case "estonian":
                    return "et";

                case "faeroese":
                    return "fo";

                case "farsi":
                    return "fa";

                case "fijian":
                    return "fj";

                case "finnish":
                    return "fi";

                case "french":
                    return "fr";

                case "frisian":
                    return "fy";

                case "friulian":
                    return "fur";

                case "gaelic":
                    return "gd";

                case "galacian":
                    return "gl";

                case "georgian":
                    return "ka";

                case "german":
                    return "de";

                case "greek":
                    return "el";

				case "gujarati":
                    return "gu";

                case "haitian":
                    return "ht";

                case "hebrew":
                    return "he";

                case "hindi":
                    return "hi";

                case "hungarian":
                    return "hu";

                case "icelandic":
                    return "is";

                case "indonesian":
                    return "id";

                case "inuktitut":
                    return "iu";

                case "irish":
                    return "ga";

                case "italian":
                    return "it";

                case "japanese":
                    return "ja";

                case "kannada":
                    return "kn";

                case "kashmiri":
                    return "ks";

                case "kazakh":
                    return "kk";

                case "khmer":
                    return "km";

                case "kinyarwanda":
                    return "rw";

                case "kirghiz":
                    return "ky";

                case "klingon":
                    return "tlh";

                case "korean":
                    return "ko";

                case "latin":
                    return "la";

                case "latvian":
                    return "lv";

                case "lithuanian":
                    return "lt";

                case "luxembourgish":
                    return "lb";

                case "fyro macedonian":
                    return "mk";

                case "malay":
                    return "ms";

                case "malayalam":
                    return "ml";

                case "maltese":
                    return "mt";

                case "maori":
                    return "mi";

                case "marathi":
                    return "mr";

                case "moldavian":
                    return "mo";

                case "navajo":
                    return "nv";

                case "ndonga":
                    return "ng";

                case "nepali":
                    return "ne";

                case "norwegian":
                    return "no";

                case "norwegian (bokmal)":
                    return "nb";

                case "norwegian (nynorsk)":
                    return "nn";

                case "occitan":
                    return "oc";

                case "oriya":
                    return "or";

                case "oromo":
                    return "om";

                case "papiamento":
                    return "papo";

                case "papiamentu":
                    return "papu";

                case "persian":
                    return "fa";

                case "polish":
                    return "pl";

                case "portuguese":
                    return "pt";

                case "punjabi":
                    return "pa";

                case "quechua":
                    return "qu";

                case "rhaeto-romanic":
                    return "rm";

                case "romanian":
                    return "ro";

                case "russian":
                    return "ru";

                case "sami (lappish)":
                    return "sz";

                case "sango":
                    return "sg";

                case "sanskrit":
                    return "sa";

                case "sardinian":
                    return "sc";

                case "scots gaelic":
                    return "gd";

                case "sindhi":
                    return "sd";

                case "singhalese":
                    return "si";

                case "serbian":
                    return "sr";

                case "slovak":
                    return "sk";

                case "slovenian":
                    return "sl";

                case "somani":
                    return "so";

                case "sorbian":
                    return "sb";

                case "spanish":
                    return "es";

                case "sutu":
                    return "sx";

                case "swahili":
                    return "sw";

                case "swedish":
                    return "sv";

                case "tamil":
                    return "ta";

                case "tatar":
                    return "tt";

                case "telugu":
                    return "te";

                case "thai":
                    return "th";

                case "tigre":
                    return "tig";

                case "tsonga":
                    return "ts";

                case "tswana":
                    return "tn";

                case "turkish":
                    return "tr";

                case "turkmen":
                    return "tk";

                case "ukrainian":
                    return "uk";

                case "upper sorbian":
                    return "hsb";

                case "urdu":
                    return "ur";

                case "venda":
                    return "ve";

                case "vietnamese":
                    return "vi";

                case "volapuk":
                    return "vo";

                case "walloon":
                    return "wa";

                case "welsh":
                    return "cy";

                case "xhosa":
                    return "xh";

                case "yiddish":
                    return "ji";

                case "zulu":
                    return "zu";

                default:
                    return String.Empty;
            }
        }

        /// <summary> Given the language enumeration, return the associated language name </summary>
        /// <param name="Web_Language"> Enumeration for the web language </param>
        /// <returns> Name for the language </returns>
        public static string Enum_To_Name( Web_Language_Enum Web_Language )
        {
            switch( Web_Language )
            {
                case Web_Language_Enum.Afrikaans:
                    return "Afrikaans";

                case Web_Language_Enum.Albanian:
                    return "Albanian";

                case Web_Language_Enum.Arabic:
                    return "Arabic";

                case Web_Language_Enum.Aragonese:
                    return "Aragonese";

                case Web_Language_Enum.Armenian:
                    return "Armenian";

                case Web_Language_Enum.Assamese:
                    return "Assamese";

                case Web_Language_Enum.Asturian:
                    return "Asturian";

                case Web_Language_Enum.Azerbaijani:
                    return "Azerbaijani";

                case Web_Language_Enum.Basque:
                    return "Basque";

                case Web_Language_Enum.Bulgarian:
                    return "Bulgarian";

                case Web_Language_Enum.Belarusian:
                    return "Belarusian";

                case Web_Language_Enum.Bengali:
                    return "Bengali";

                case Web_Language_Enum.Bosnian:
                    return "Bosnian";

                case Web_Language_Enum.Breton:
                    return "Breton";

                case Web_Language_Enum.Burmese:
                    return "Burmese";

                case Web_Language_Enum.Catalan:
                    return "Catalan";

                case Web_Language_Enum.Chamorro:
                    return "Chamorro";

                case Web_Language_Enum.Chechen:
                    return "Chechen";

                case Web_Language_Enum.Chinese:
                    return "Chinese";

                case Web_Language_Enum.Chuvash:
                    return "Chuvash";

                case Web_Language_Enum.Corsican:
                    return "Corsican";

                case Web_Language_Enum.Cree:
                    return "Cree";

                case Web_Language_Enum.Croatian:
                    return "Croatian";

                case Web_Language_Enum.Czech:
                    return "Czech";

                case Web_Language_Enum.Danish:
                    return "Danish";

                case Web_Language_Enum.Dutch:
                    return "Dutch";

                case Web_Language_Enum.English:
                    return "English";

                case Web_Language_Enum.Esperanto:
                    return "Esperanto";

                case Web_Language_Enum.Estonian:
                    return "Estonian";

                case Web_Language_Enum.Faeroese:
                    return "Faeroese";

                case Web_Language_Enum.Farsi:
                    return "Farsi";

                case Web_Language_Enum.Fijian:
                    return "Fijian";

                case Web_Language_Enum.Finnish:
                    return "Finnish";

                case Web_Language_Enum.French:
                    return "French";

                case Web_Language_Enum.Frisian:
                    return "Frisian";

                case Web_Language_Enum.Friulian:
                    return "Friulian";

                case Web_Language_Enum.Gaelic:
                    return "Gaelic";

                case Web_Language_Enum.Galacian:
                    return "Galacian";

                case Web_Language_Enum.Georgian:
                    return "Georgian";

                case Web_Language_Enum.German:
                    return "German";

                case Web_Language_Enum.Greek:
                    return "Greek";

				case Web_Language_Enum.Gujarati:
					return "Gujarati";

                case Web_Language_Enum.Haitian:
                    return "Haitian";

                case Web_Language_Enum.Hebrew:
                    return "Hebrew";

                case Web_Language_Enum.Hindi:
                    return "Hindi";

                case Web_Language_Enum.Hungarian:
                    return "Hungarian";

                case Web_Language_Enum.Icelandic:
                    return "Icelandic";

                case Web_Language_Enum.Indonesian:
                    return "Indonesian";

                case Web_Language_Enum.Inuktitut:
                    return "Inuktitut";

                case Web_Language_Enum.Irish:
                    return "Irish";

                case Web_Language_Enum.Italian:
                    return "Italian";

                case Web_Language_Enum.Japanese:
                    return "Japanese";

                case Web_Language_Enum.Kannada:
                    return "Kannada";

                case Web_Language_Enum.Kashmiri:
                    return "Kashmiri";

                case Web_Language_Enum.Kazakh:
                    return "Kazakh";

                case Web_Language_Enum.Khmer:
                    return "Khmer";

                case Web_Language_Enum.Kinyarwanda:
                    return "Kinyarwanda";

                case Web_Language_Enum.Kirghiz:
                    return "Kirghiz";

                case Web_Language_Enum.Klingon:
                    return "Klingon";

                case Web_Language_Enum.Korean:
                    return "Korean";

                case Web_Language_Enum.Latin:
                    return "Latin";

                case Web_Language_Enum.Latvian:
                    return "Latvian";

                case Web_Language_Enum.Lithuanian:
                    return "Lithuanian";

                case Web_Language_Enum.Luxembourgish:
                    return "Luxembourgish";

                case Web_Language_Enum.FYRO_Macedonian:
                    return "FYRO Macedonian";

                case Web_Language_Enum.Malay:
                    return "Malay";

                case Web_Language_Enum.Malayalam:
                    return "Malayalam";

                case Web_Language_Enum.Maltese:
                    return "Maltese";

                case Web_Language_Enum.Maori:
                    return "Maori";

                case Web_Language_Enum.Marathi:
                    return "Marathi";

                case Web_Language_Enum.Moldavian:
                    return "Moldavian";

                case Web_Language_Enum.Navajo:
                    return "Navajo";

                case Web_Language_Enum.Ndonga:
                    return "Ndonga";

                case Web_Language_Enum.Nepali:
                    return "Nepali";

                case Web_Language_Enum.Norwegian:
                    return "Norwegian";

                case Web_Language_Enum.Norwegian_Bokmal:
                    return "Norwegian (Bokmal)";

                case Web_Language_Enum.Norwegian_Nynorsk:
                    return "Norwegian (Nynorsk)";

                case Web_Language_Enum.Occitan:
                    return "Occitan";

                case Web_Language_Enum.Oriya:
                    return "Oriya";

                case Web_Language_Enum.Oromo:
                    return "Oromo";

                case Web_Language_Enum.Papiamento:
                    return "Papiamento";

                case Web_Language_Enum.Papiamentu:
                    return "Papiamentu";

                case Web_Language_Enum.Persian:
                    return "Persian";

                case Web_Language_Enum.Polish:
                    return "Polish";

                case Web_Language_Enum.Portuguese:
                    return "Portuguese";

                case Web_Language_Enum.Punjabi:
                    return "Punjabi";

                case Web_Language_Enum.Quechua:
                    return "Quechua";

                case Web_Language_Enum.Rhaeto_Romanic:
                    return "Rhaeto-Romanic";

                case Web_Language_Enum.Romanian:
                    return "Romanian";

                case Web_Language_Enum.Russian:
                    return "Russian";

                case Web_Language_Enum.Sami_Lappish:
                    return "Sami (Lappish)";

                case Web_Language_Enum.Sango:
                    return "Sango";

                case Web_Language_Enum.Sanskrit:
                    return "Sanskrit";

                case Web_Language_Enum.Sardinian:
                    return "Sardinian";

                case Web_Language_Enum.Scots_Gaelic:
                    return "Scots Gaelic";

                case Web_Language_Enum.Sindhi:
                    return "Sindhi";

                case Web_Language_Enum.Singhalese:
                    return "Singhalese";

                case Web_Language_Enum.Serbian:
                    return "Serbian";

                case Web_Language_Enum.Slovak:
                    return "Slovak";

                case Web_Language_Enum.Slovenian:
                    return "Slovenian";

                case Web_Language_Enum.Somani:
                    return "Somani";

                case Web_Language_Enum.Sorbian:
                    return "Sorbian";

                case Web_Language_Enum.Spanish:
                    return "Spanish";

                case Web_Language_Enum.Sutu:
                    return "Sutu";

                case Web_Language_Enum.Swahili:
                    return "Swahili";

                case Web_Language_Enum.Swedish:
                    return "Swedish";

                case Web_Language_Enum.Tamil:
                    return "Tamil";

                case Web_Language_Enum.Tatar:
                    return "Tatar";

                case Web_Language_Enum.Telugu:
                    return "Telugu";

                case Web_Language_Enum.Thai:
                    return "Thai";

                case Web_Language_Enum.Tigre:
                    return "Tigre";

                case Web_Language_Enum.Tsonga:
                    return "Tsonga";

                case Web_Language_Enum.Tswana:
                    return "Tswana";

                case Web_Language_Enum.Turkish:
                    return "Turkish";

                case Web_Language_Enum.Turkmen:
                    return "Turkmen";

                case Web_Language_Enum.Ukrainian:
                    return "Ukrainian";

                case Web_Language_Enum.Upper_Sorbian:
                    return "Upper Sorbian";

                case Web_Language_Enum.Urdu:
                    return "Urdu";

                case Web_Language_Enum.Venda:
                    return "Venda";

                case Web_Language_Enum.Vietnamese:
                    return "Vietnamese";

                case Web_Language_Enum.Volapuk:
                    return "Volapuk";

                case Web_Language_Enum.Walloon:
                    return "Walloon";

                case Web_Language_Enum.Welsh:
                    return "Welsh";

                case Web_Language_Enum.Xhosa:
                    return "Xhosa";

                case Web_Language_Enum.Yiddish:
                    return "Yiddish";

                case Web_Language_Enum.Zulu:
                    return "Zulu";

                default:
                    return "Unknown Language";

            }
        }

        public static string Enum_To_Code(Web_Language_Enum Enum )
        {
            switch(Enum)
            {
                case Web_Language_Enum.Afrikaans:
                    return "af";

                case Web_Language_Enum.Albanian:
                    return "sq";

                case Web_Language_Enum.Arabic:
                    return "ar";

                case Web_Language_Enum.Aragonese:
                    return "ar";

                case Web_Language_Enum.Armenian:
                    return "hy";

                case Web_Language_Enum.Assamese:
                    return "as";

                case Web_Language_Enum.Asturian:
                    return "ast";

                case Web_Language_Enum.Azerbaijani:
                    return "az";

                case Web_Language_Enum.Basque:
                    return "eu";

                case Web_Language_Enum.Belarusian:
                    return "be";

                case Web_Language_Enum.Bengali:
                    return "bn";

                case Web_Language_Enum.Bosnian:
                    return "bs";

                case Web_Language_Enum.Breton:
                    return "br";

                case Web_Language_Enum.Bulgarian:
                    return "bg";

                case Web_Language_Enum.Burmese:
                    return "my";

                case Web_Language_Enum.Catalan:
                    return "ca";

                case Web_Language_Enum.Chamorro:
                    return "ch";

                case Web_Language_Enum.Chechen:
                    return "ce";

                case Web_Language_Enum.Chinese:
                    return "zh";

                case Web_Language_Enum.Chuvash:
                    return "cv";

                case Web_Language_Enum.Corsican:
                    return "co";

                case Web_Language_Enum.Cree:
                    return "cr";

                case Web_Language_Enum.Croatian:
                    return "hr";

                case Web_Language_Enum.Czech:
                    return "cs";

                case Web_Language_Enum.Danish:
                    return "da";

                case Web_Language_Enum.Dutch:
                    return "nl";

                case Web_Language_Enum.English:
                    return "en";

                case Web_Language_Enum.Esperanto:
                    return "eo";

                case Web_Language_Enum.Estonian:
                    return "et";

                case Web_Language_Enum.Faeroese:
                    return "fo";

                case Web_Language_Enum.Farsi:
                    return "fa";

                case Web_Language_Enum.Fijian:
                    return "fj";

                case Web_Language_Enum.Finnish:
                    return "fi";

                case Web_Language_Enum.French:
                    return "fr";

                case Web_Language_Enum.Frisian:
                    return "fy";

                case Web_Language_Enum.Friulian:
                    return "fur";

                case Web_Language_Enum.Gaelic:
                    return "gd";

                case Web_Language_Enum.Galacian:
                    return "gl";

                case Web_Language_Enum.Georgian:
                    return "ka";

                case Web_Language_Enum.German:
                    return "de";

                case Web_Language_Enum.Greek:
                    return "el";

				case Web_Language_Enum.Gujarati:
                    return "gu";

                case Web_Language_Enum.Haitian:
                    return "ht";

                case Web_Language_Enum.Hebrew:
                    return "he";

                case Web_Language_Enum.Hindi:
                    return "hi";

                case Web_Language_Enum.Hungarian:
                    return "hu";

                case Web_Language_Enum.Icelandic:
                    return "is";

                case Web_Language_Enum.Indonesian:
                    return "id";

                case Web_Language_Enum.Inuktitut:
                    return "iu";

                case Web_Language_Enum.Irish:
                    return "ga";

                case Web_Language_Enum.Italian:
                    return "it";

                case Web_Language_Enum.Japanese:
                    return "ja";

                case Web_Language_Enum.Kannada:
                    return "kn";

                case Web_Language_Enum.Kashmiri:
                    return "ks";

                case Web_Language_Enum.Kazakh:
                    return "kk";

                case Web_Language_Enum.Khmer:
                    return "km";

                case Web_Language_Enum.Kinyarwanda:
                    return "rw";

                case Web_Language_Enum.Kirghiz:
                    return "ky";

                case Web_Language_Enum.Klingon:
                    return "tlh";

                case Web_Language_Enum.Korean:
                    return "ko";

                case Web_Language_Enum.Latin:
                    return "la";

                case Web_Language_Enum.Latvian:
                    return "lv";

                case Web_Language_Enum.Lithuanian:
                    return "lt";

                case Web_Language_Enum.Luxembourgish:
                    return "lb";

                case Web_Language_Enum.FYRO_Macedonian:
                    return "mk";

                case Web_Language_Enum.Malay:
                    return "ms";

                case Web_Language_Enum.Malayalam:
                    return "ml";

                case Web_Language_Enum.Maltese:
                    return "mt";

                case Web_Language_Enum.Maori:
                    return "mi";

                case Web_Language_Enum.Marathi:
                    return "mr";

                case Web_Language_Enum.Moldavian:
                    return "mo";

                case Web_Language_Enum.Navajo:
                    return "nv";

                case Web_Language_Enum.Ndonga:
                    return "ng";

                case Web_Language_Enum.Nepali:
                    return "ne";

                case Web_Language_Enum.Norwegian:
                    return "no";

                case Web_Language_Enum.Norwegian_Bokmal:
                    return "nb";

                case Web_Language_Enum.Norwegian_Nynorsk:
                    return "nn";

                case Web_Language_Enum.Occitan:
                    return "oc";

                case Web_Language_Enum.Oriya:
                    return "or";

                case Web_Language_Enum.Oromo:
                    return "om";

                case Web_Language_Enum.Papiamento:
                    return "papo";
            
                case Web_Language_Enum.Papiamentu:
                    return "papu";

                case Web_Language_Enum.Persian:
                    return "fa";

                case Web_Language_Enum.Polish:
                    return "pl";

                case Web_Language_Enum.Portuguese:
                    return "pt";

                case Web_Language_Enum.Punjabi:
                    return "pa";

                case Web_Language_Enum.Quechua:
                    return "qu";

                case Web_Language_Enum.Rhaeto_Romanic:
                    return "rm";

                case Web_Language_Enum.Romanian:
                    return "ro";

                case Web_Language_Enum.Russian:
                    return "ru";

                case Web_Language_Enum.Sami_Lappish:
                    return "sz";

                case Web_Language_Enum.Sango:
                    return "sg";

                case Web_Language_Enum.Sanskrit:
                    return "sa";

                case Web_Language_Enum.Sardinian:
                    return "sc";

                case Web_Language_Enum.Scots_Gaelic:
                    return "gd";

                case Web_Language_Enum.Sindhi:
                    return "sd";

                case Web_Language_Enum.Singhalese:
                    return "si";

                case Web_Language_Enum.Serbian:
                    return "sr";

                case Web_Language_Enum.Slovak:
                    return "sk";

                case Web_Language_Enum.Slovenian:
                    return "sl";

                case Web_Language_Enum.Somani:
                    return "so";

                case Web_Language_Enum.Sorbian:
                    return "sb";

                case Web_Language_Enum.Spanish:
                    return "es";

                case Web_Language_Enum.Sutu:
                    return "sx";

                case Web_Language_Enum.Swahili:
                    return "sw";

                case Web_Language_Enum.Swedish:
                    return "sv";

                case Web_Language_Enum.Tamil:
                    return "ta";

                case Web_Language_Enum.Tatar:
                    return "tt";

                case Web_Language_Enum.Telugu:
                    return "te";

                case Web_Language_Enum.Thai:
                    return "th";

                case Web_Language_Enum.Tigre:
                    return "tig";

                case Web_Language_Enum.Tsonga:
                    return "ts";

                case Web_Language_Enum.Tswana:
                    return "tn";

                case Web_Language_Enum.Turkish:
                    return "tr";

                case Web_Language_Enum.Turkmen:
                    return "tk";

                case Web_Language_Enum.Ukrainian:
                    return "uk";

                case Web_Language_Enum.Upper_Sorbian:
                    return "hsb";

                case Web_Language_Enum.Urdu:
                    return "ur";

                case Web_Language_Enum.Venda:
                    return "ve";

                case Web_Language_Enum.Vietnamese:
                    return "vi";

                case Web_Language_Enum.Volapuk:
                    return "vo";

                case Web_Language_Enum.Walloon:
                    return "wa";

                case Web_Language_Enum.Welsh:
                    return "cy";

                case Web_Language_Enum.Xhosa:
                    return "xh";

                case Web_Language_Enum.Yiddish:
                    return "ji";

                case Web_Language_Enum.Zulu:
                    return "zu";

                
                default:
                    return String.Empty;
            }
        }
    }
}
