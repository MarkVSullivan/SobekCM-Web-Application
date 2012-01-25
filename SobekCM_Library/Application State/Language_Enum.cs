#region Using directives

using System;

#endregion

namespace SobekCM.Library.Application_State
{
    /// <summary> Enumeration is used to indicate the current skin language </summary>
    public enum Language_Enum : byte
    {
        /// <summary> This is a template space holder, which is used to write the different language
        /// links in the header.</summary>
        /// <remarks> This maps to "ZZZZZZZ..." and then this is replaced in the resultant URL with the code
        /// for each individual language</remarks>
        Template = 0,

        /// <summary> English language </summary>
        English = 1,

        /// <summary> French language </summary>
        French,

        /// <summary> Spanish language </summary>
        Spanish,

        /// <summary> Dutch language </summary>
        Dutch,

        /// <summary> Haitian-French creole language </summary>
        Kreyol,

        /// <summary> Papiemento language of Aruba  </summary>
        Papiemento,

        /// <summary> German language </summary>
        German,

        /// <summary> Portuguese language </summary>
        Portuguese,

        /// <summary> Italian language </summary>
        Italian,

        /// <summary> Default language is used when no language is indicated </summary>
        DEFAULT
    }


    /// <summary> Static class is used to change between codes and language names to the language enumeration  </summary>
    public class Language_Enum_Converter
    {
        /// <summary> Gets the array of different language names supported by this system </summary>
        public static string[] Language_Name_Array
        {
            get
            {
                return new[] { "Dutch", "English", "French", "German", "Italian", "Kreyol (Haitian)", "Papiemento", "Portuguese", "Spanish" };
            }
        }

        /// <summary> Converts a string (code or language name) to the language enumeration </summary>
        /// <param name="Language_Code">Code or name of the language </param>
        /// <returns> Associated language enumeration </returns>
        public static Language_Enum Code_To_Language_Enum( string Language_Code )
        {
            switch( Language_Code.ToLower() )
            {
                case "eng":
                case "en":
                case "english":
                    return Language_Enum.English;

                case "fra":
                case "fre":
                case "fr":
                case "french":
                case "francais":
                case "français":
                case "francaise":
                case "française":
                    return Language_Enum.French;

                case "spa":
                case "esp":
                case "sp":
                case "es":
                case "spanish":
                case "espanol":
                case "español":
                    return Language_Enum.Spanish;

                case "nl":
                case "nld":
                case "dut":
                case "dutch":
                    return Language_Enum.Dutch;

                case "ht":
                case "hat":
                case "kreyol":
                case "kreyol (haitian)":
                case "haiti":
                case "haitian":
                    return Language_Enum.Kreyol;

                case "pap":
                case "papiemento":   // We are aware these are two distinct languages, but the 
                case "papiemanta":   // similarities merit auto-detection here. 
                    return Language_Enum.Papiemento;

                case "de":
                case "deu":
                case "ge":
                case "ger":
                case "german":
                case "deutsch":
                    return Language_Enum.German;

                case "pt":
                case "por":
                case "portuguese":
                    return Language_Enum.Portuguese;

                case "it":
                case "ita":
                case "italian":
                    return Language_Enum.Italian;

                case "template":
                    return Language_Enum.Template;

                default:
                    return Language_Enum.DEFAULT;
            }
        }

        /// <summary> Converts the language enumeration to the language code  </summary>
        /// <param name="Language">Language enumeration </param>
        /// <returns> Code for the language indicated by enumeration </returns>
        public static string Language_Enum_To_Code( Language_Enum Language )
        {
            switch( Language )
            {
                case Language_Enum.DEFAULT:
                    return String.Empty;

                case Language_Enum.Dutch:
                    return "dutch";

                case Language_Enum.English:
                    return "en";

                case Language_Enum.French:
                    return "fr";

                case Language_Enum.German:
                    return "german";

                case Language_Enum.Italian:
                    return "italian";

                case Language_Enum.Kreyol:
                    return "kreyol";

                case Language_Enum.Papiemento:
                    return "papiemento";

                case Language_Enum.Portuguese:
                    return "portuguese";

                case Language_Enum.Spanish:
                    return "es";

                case Language_Enum.Template:
                    return "XXXXX";

                default:
                    return String.Empty;
            }
        }
    }
}
