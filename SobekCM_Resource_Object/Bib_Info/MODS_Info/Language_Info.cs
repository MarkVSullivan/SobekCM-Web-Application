#region Using directives

using System;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary>A designation of the language in which the content of a resource is expressed</summary>
    /// <remarks>This class extends the <see cref="XML_Node_Base_Type"/> class for writing to XML </remarks>
    [Serializable]
    public class Language_Info : XML_Writing_Base_Type, IEquatable<Language_Info>
    {
        private string language_iso_code;
        private string language_rfc_code;
        private string language_text;

        /// <summary> Constructor creates an empty instance of the Language_Info class </summary>
        public Language_Info()
        {
            // Do nothing by default
        }

        /// <summary> Constructor creates an empty instance of the Language_Info class </summary>
        /// <param name="Language_Text">Language term for this language</param>
        /// <param name="Language_ISO_Code">Iso639-2b code for this language</param>
        /// <param name="Language_RFC_Code">Rfc3066 code for this language</param>       
        public Language_Info(string Language_Text, string Language_ISO_Code, string Language_RFC_Code)
        {
            language_text = Language_Text;
            language_iso_code = Language_ISO_Code;
            language_rfc_code = Language_RFC_Code;

            if ((language_iso_code.Length == 0) && (language_text.Length > 0))
            {
                string code = Get_Code_By_Language(language_text);
                if (code.Length > 0)
                    language_iso_code = code;
            }

            if ((language_iso_code.Length > 0) && (language_text.Length == 0))
            {
                string text = Get_Language_By_Code(language_iso_code);
                if (text.Length > 0)
                    language_text = text;
            }

            // The code may be appearing in the text portion
            if ((language_iso_code.Length == 0) && (language_text.Length == 3))
            {
                string possible_text = Get_Language_By_Code(language_text);
                if (possible_text.Length > 0)
                {
                    language_iso_code = language_text;
                    language_text = possible_text;
                }
            }
        }

        /// <summary> Gets or sets the Language term for this language  </summary>
        public string Language_Text
        {
            get { return language_text ?? String.Empty; }
            set
            {
                language_text = value;

                if ((String.IsNullOrEmpty(language_iso_code)) && (!String.IsNullOrEmpty(language_text)))
                {
                    string code = Get_Code_By_Language(language_text);
                    if (code.Length > 0)
                        language_iso_code = code;
                }
            }
        }

        /// <summary> Gets or sets the Iso639-2b code for this language </summary>
        public string Language_ISO_Code
        {
            get { return language_iso_code ?? String.Empty; }
            set
            {
                language_iso_code = value;

                if ((!String.IsNullOrEmpty(language_iso_code)) && (String.IsNullOrEmpty(language_text)))
                {
                    string text = Get_Language_By_Code(language_iso_code);
                    if (text.Length > 0)
                        language_text = text;
                }
            }
        }

        /// <summary> Gets or sets the Rfc3066 code for this language  </summary>
        public string Language_RFC_Code
        {
            get { return language_rfc_code ?? String.Empty; }
            set { language_rfc_code = value; }
        }

        /// <summary> Flag indicates if this language object has data, or is an empty language </summary>
        internal bool hasData
        {
            get
            {
                return (!String.IsNullOrEmpty(language_text)) || (!String.IsNullOrEmpty(language_iso_code)) || (!String.IsNullOrEmpty(language_rfc_code));
            }
        }

        #region IEquatable<Language_Info> Members

        /// <summary> Compares this object with another similarly typed object </summary>
        /// <param name="Other">Similarly types object </param>
        /// <returns>TRUE if the two objects are sufficiently similar</returns>
        public bool Equals(Language_Info Other)
        {
            if (( !String.IsNullOrEmpty(Language_Text)) && ( String.Compare(Language_Text, Other.Language_Text, StringComparison.Ordinal) == 0 ))
                return true;

            if ((!String.IsNullOrEmpty(Language_RFC_Code)) && (String.Compare(Language_RFC_Code, Other.Language_RFC_Code, StringComparison.Ordinal) == 0))
                return true;

            return (!String.IsNullOrEmpty(Language_ISO_Code)) && (String.Compare(Language_ISO_Code, Other.Language_ISO_Code, StringComparison.Ordinal) == 0);
        }

        #endregion

        /// <summary> Writes this language as MODS to a writer writing to a stream ( either a file or web response stream )</summary>
        /// <param name="ReturnValue"> Writer to the MODS building stream </param>
        internal void Add_MODS(TextWriter ReturnValue)
        {
            if (!hasData)
                return;

            ReturnValue.Write("<mods:language");
            ReturnValue.Write(">\r\n");

            if (!String.IsNullOrEmpty(language_text))
            {
                ReturnValue.Write("<mods:languageTerm type=\"text\">" + Convert_String_To_XML_Safe(language_text) + "</mods:languageTerm>\r\n");
            }

            if (!String.IsNullOrEmpty(language_iso_code))
            {
                ReturnValue.Write("<mods:languageTerm type=\"code\" authority=\"iso639-2b\">" + Convert_String_To_XML_Safe(language_iso_code) + "</mods:languageTerm>\r\n");
            }

            if (!String.IsNullOrEmpty(language_rfc_code))
            {
                ReturnValue.Write("<mods:languageTerm type=\"code\" authority=\"rfc3066\">" + Convert_String_To_XML_Safe(language_rfc_code) + "</mods:languageTerm>\r\n");
            }
            ReturnValue.Write("</mods:language>\r\n");
        }

        #region Code to get the language text from the ISO code

        /// <summary> Get the language text from the ISO code </summary>
        /// <param name="CodeCheck"> ISO code to convert to language name </param>
        /// <returns> The associated language name or an empty string </returns>
        public string Get_Language_By_Code(string CodeCheck)
        {
            return Get_Language_By_Code_Static(CodeCheck);
        }

        /// <summary> Get the language text from the ISO code </summary>
        /// <param name="CodeCheck"> ISO code to convert to language name </param>
        /// <returns> The associated language name or an empty string </returns>
        public static string Get_Language_By_Code_Static(string CodeCheck)
        {
            string code_upper = CodeCheck.ToUpper();
            string text = String.Empty;
            switch (code_upper)
            {
                case "AAR":
                case "AA":
                    text = "Afar";
                    break;
                case "ABK":
                case "AB":
                    text = "Abkhaz";
                    break;
                case "ACE":
                    text = "Achinese";
                    break;
                case "ACH":
                    text = "Acoli";
                    break;
                case "ADA":
                    text = "Adangme";
                    break;
                case "ADY":
                    text = "Adygei";
                    break;
                case "AFA":
                    text = "Afroasiatic (Other)";
                    break;
                case "AFH":
                    text = "Afrihili (Artificial language)";
                    break;
                case "AFR":
                case "AF":
                    text = "Afrikaans";
                    break;
                case "AJM":
                    text = "Aljamía";
                    break;
                case "AKA":
                case "AK":
                    text = "Akan";
                    break;
                case "AKK":
                    text = "Akkadian";
                    break;
                case "ALB":
                case "SQI":
                case "SQ":
                    text = "Albanian";
                    break;
                case "ALE":
                    text = "Aleut";
                    break;
                case "ALG":
                    text = "Algonquian (Other)";
                    break;
                case "AMH":
                case "AM":
                    text = "Amharic";
                    break;
                case "ANG":
                    text = "English, Old (ca. 450-1100)";
                    break;
                case "APA":
                    text = "Apache languages";
                    break;
                case "ARA":
                case "AR":
                    text = "Arabic";
                    break;
                case "ARC":
                    text = "Aramaic";
                    break;
                case "ARG":
                case "AN":
                    text = "Aragonese Spanish";
                    break;
                case "ARM":
                case "HYE":
                case "HY":
                    text = "Armenian";
                    break;
                case "ARN":
                    text = "Mapuche";
                    break;
                case "ARP":
                    text = "Arapaho";
                    break;
                case "ART":
                    text = "Artificial (Other)";
                    break;
                case "ARW":
                    text = "Arawak";
                    break;
                case "ASM":
                case "AS":
                    text = "Assamese";
                    break;
                case "AST":
                    text = "Bable";
                    break;
                case "ATH":
                    text = "Athapascan (Other)";
                    break;
                case "AUS":
                    text = "Australian languages";
                    break;
                case "AVA":
                case "AV":
                    text = "Avaric";
                    break;
                case "AVE":
                case "AE":
                    text = "Avestan";
                    break;
                case "AWA":
                    text = "Awadhi";
                    break;
                case "AYM":
                case "AY":
                    text = "Aymara";
                    break;
                case "AZE":
                case "AZ":
                    text = "Azerbaijani";
                    break;
                case "BAD":
                    text = "Banda";
                    break;
                case "BAI":
                    text = "Bamileke languages";
                    break;
                case "BAK":
                case "BA":
                    text = "Bashkir";
                    break;
                case "BAL":
                    text = "Baluchi";
                    break;
                case "BAM":
                case "BM":
                    text = "Bambara";
                    break;
                case "BAN":
                    text = "Balinese";
                    break;
                case "BAQ":
                case "EUS":
                case "EU":
                    text = "Basque";
                    break;
                case "BAS":
                    text = "Basa";
                    break;
                case "BAT":
                    text = "Baltic (Other)";
                    break;
                case "BEJ":
                    text = "Beja";
                    break;
                case "BEL":
                case "BE":
                    text = "Belarusian";
                    break;
                case "BEM":
                    text = "Bemba";
                    break;
                case "BEN":
                case "BN":
                    text = "Bengali";
                    break;
                case "BER":
                    text = "Berber (Other)";
                    break;
                case "BHO":
                    text = "Bhojpuri";
                    break;
                case "BIH":
                case "BH":
                    text = "Bihari";
                    break;
                case "BIK":
                    text = "Bikol";
                    break;
                case "BIN":
                    text = "Edo";
                    break;
                case "BIS":
                case "BI":
                    text = "Bislama";
                    break;
                case "BLA":
                    text = "Siksika";
                    break;
                case "BNT":
                    text = "Bantu (Other)";
                    break;
                case "BOS":
                case "BS":
                    text = "Bosnian";
                    break;
                case "BRA":
                    text = "Braj";
                    break;
                case "BRE":
                case "BR":
                    text = "Breton";
                    break;
                case "BTK":
                    text = "Batak";
                    break;
                case "BUA":
                    text = "Buriat";
                    break;
                case "BUG":
                    text = "Bugis";
                    break;
                case "BUL":
                case "BG":
                    text = "Bulgarian";
                    break;
                case "BUR":
                case "MYA":
                case "MY":
                    text = "Burmese";
                    break;
                case "CAD":
                    text = "Caddo";
                    break;
                case "CAI":
                    text = "Central American Indian (Other)";
                    break;
                case "CAM":
                    text = "Khmer";
                    break;
                case "CAR":
                    text = "Carib";
                    break;
                case "CAT":
                case "CA":
                    text = "Catalan";
                    break;
                case "CAU":
                    text = "Caucasian (Other)";
                    break;
                case "CEB":
                    text = "Cebuano";
                    break;
                case "CEL":
                    text = "Celtic (Other)";
                    break;
                case "CHA":
                case "CH":
                    text = "Chamorro";
                    break;
                case "CHB":
                    text = "Chibcha";
                    break;
                case "CHE":
                case "CE":
                    text = "Chechen";
                    break;
                case "CHG":
                    text = "Chagatai";
                    break;
                case "CHI":
                case "ZHO":
                case "ZH":
                    text = "Chinese";
                    break;
                case "CHK":
                    text = "Truk";
                    break;
                case "CHM":
                    text = "Mari";
                    break;
                case "CHN":
                    text = "Chinook jargon";
                    break;
                case "CHO":
                    text = "Choctaw";
                    break;
                case "CHP":
                    text = "Chipewyan";
                    break;
                case "CHR":
                    text = "Cherokee";
                    break;
                case "CHU":
                case "CU":
                    text = "Church Slavic";
                    break;
                case "CHV":
                case "CV":
                    text = "Chuvash";
                    break;
                case "CHY":
                    text = "Cheyenne";
                    break;
                case "CMC":
                    text = "Chamic languages";
                    break;
                case "COP":
                    text = "Coptic";
                    break;
                case "COR":
                case "KW":
                    text = "Cornish";
                    break;
                case "COS":
                case "CO":
                    text = "Corsican";
                    break;
                case "CPE":
                    text = "Creoles and Pidgins, English-based (Other)";
                    break;
                case "CPF":
                    text = "Creoles and Pidgins, French-based (Other)";
                    break;
                case "CPP":
                    text = "Creoles and Pidgins, Portuguese-based (Other)";
                    break;
                case "CRE":
                case "CR":
                    text = "Cree";
                    break;
                case "CRH":
                    text = "Crimean Tatar";
                    break;
                case "CRP":
                    text = "Creoles and Pidgins (Other)";
                    break;
                case "CUS":
                    text = "Cushitic (Other)";
                    break;
                case "CZE":
                case "CES":
                case "CS":
                    text = "Czech";
                    break;
                case "DAK":
                    text = "Dakota";
                    break;
                case "DAN":
                case "DA":
                    text = "Danish";
                    break;
                case "DAR":
                    text = "Dargwa";
                    break;
                case "DAY":
                    text = "Dayak";
                    break;
                case "DEL":
                    text = "Delaware";
                    break;
                case "DEN":
                    text = "Slave";
                    break;
                case "DGR":
                    text = "Dogrib";
                    break;
                case "DIN":
                    text = "Dinka";
                    break;
                case "DIV":
                    text = "Divehi";
                    break;
                case "DOI":
                    text = "Dogri";
                    break;
                case "DRA":
                    text = "Dravidian (Other)";
                    break;
                case "DUA":
                    text = "Duala";
                    break;
                case "DUM":
                    text = "Dutch, Middle (ca. 1050-1350)";
                    break;
                case "DUT":
                    text = "Dutch";
                    break;
                case "DYU":
                    text = "Dyula";
                    break;
                case "DZO":
                    text = "Dzongkha";
                    break;
                case "EFI":
                    text = "Efik";
                    break;
                case "EGY":
                    text = "Egyptian";
                    break;
                case "EKA":
                    text = "Ekajuk";
                    break;
                case "ELX":
                    text = "Elamite";
                    break;
                case "ENG":
                    text = "English";
                    break;
                case "ENM":
                    text = "English, Middle (1100-1500)";
                    break;
                case "EPO":
                    text = "Esperanto";
                    break;
                case "ESK":
                    text = "Eskimo languages";
                    break;
                case "ESP":
                    text = "Esperanto";
                    break;
                case "EST":
                    text = "Estonian";
                    break;
                case "ETH":
                    text = "Ethiopic";
                    break;
                case "EWE":
                    text = "Ewe";
                    break;
                case "EWO":
                    text = "Ewondo";
                    break;
                case "FAN":
                    text = "Fang";
                    break;
                case "FAO":
                    text = "Faroese";
                    break;
                case "FAR":
                    text = "Faroese";
                    break;
                case "FAT":
                    text = "Fanti";
                    break;
                case "FIJ":
                    text = "Fijian";
                    break;
                case "FIN":
                    text = "Finnish";
                    break;
                case "FIU":
                    text = "Finno-Ugrian (Other)";
                    break;
                case "FON":
                    text = "Fon";
                    break;
                case "FRE":
                    text = "French";
                    break;
                case "FRI":
                    text = "Frisian";
                    break;
                case "FRM":
                    text = "French, Middle (ca. 1400-1600)";
                    break;
                case "FRO":
                    text = "French, Old (ca. 842-1400)";
                    break;
                case "FRY":
                    text = "Frisian";
                    break;
                case "FUL":
                    text = "Fula";
                    break;
                case "FUR":
                    text = "Friulian";
                    break;
                case "GAA":
                    text = "Gã";
                    break;
                case "GAE":
                    text = "Scottish Gaelic";
                    break;
                case "GAG":
                    text = "Galician";
                    break;
                case "GAL":
                    text = "Oromo";
                    break;
                case "GAY":
                    text = "Gayo";
                    break;
                case "GBA":
                    text = "Gbaya";
                    break;
                case "GEM":
                    text = "Germanic (Other)";
                    break;
                case "GEO":
                    text = "Georgian";
                    break;
                case "GER":
                    text = "German";
                    break;
                case "GEZ":
                    text = "Ethiopic";
                    break;
                case "GIL":
                    text = "Gilbertese";
                    break;
                case "GLA":
                    text = "Scottish Gaelic";
                    break;
                case "GLE":
                    text = "Irish";
                    break;
                case "GLG":
                    text = "Galician";
                    break;
                case "GLV":
                    text = "Manx";
                    break;
                case "GMH":
                    text = "German, Middle High (ca. 1050-1500)";
                    break;
                case "GOH":
                    text = "German, Old High (ca. 750-1050)";
                    break;
                case "GON":
                    text = "Gondi";
                    break;
                case "GOR":
                    text = "Gorontalo";
                    break;
                case "GOT":
                    text = "Gothic";
                    break;
                case "GRB":
                    text = "Grebo";
                    break;
                case "GRC":
                    text = "Greek, Ancient (to 1453)";
                    break;
                case "GRE":
                    text = "Greek, Modern (1453- )";
                    break;
                case "GRN":
                    text = "Guarani";
                    break;
                case "GUA":
                    text = "Guarani";
                    break;
                case "GUJ":
                    text = "Gujarati";
                    break;
                case "GWI":
                    text = "Gwich'in";
                    break;
                case "HAI":
                    text = "Haida";
                    break;
                case "HAT":
                    text = "Haitian French Creole";
                    break;
                case "HAU":
                    text = "Hausa";
                    break;
                case "HAW":
                    text = "Hawaiian";
                    break;
                case "HEB":
                    text = "Hebrew";
                    break;
                case "HER":
                    text = "Herero";
                    break;
                case "HIL":
                    text = "Hiligaynon";
                    break;
                case "HIM":
                    text = "Himachali";
                    break;
                case "HIN":
                    text = "Hindi";
                    break;
                case "HIT":
                    text = "Hittite";
                    break;
                case "HMN":
                    text = "Hmong";
                    break;
                case "HMO":
                    text = "Hiri Motu";
                    break;
                case "HUN":
                    text = "Hungarian";
                    break;
                case "HUP":
                    text = "Hupa";
                    break;
                case "IBA":
                    text = "Iban";
                    break;
                case "IBO":
                    text = "Igbo";
                    break;
                case "ICE":
                    text = "Icelandic";
                    break;
                case "IDO":
                    text = "Ido";
                    break;
                case "III":
                    text = "Sichuan Yi";
                    break;
                case "IJO":
                    text = "Ijo";
                    break;
                case "IKU":
                    text = "Inuktitut";
                    break;
                case "ILE":
                    text = "Interlingue";
                    break;
                case "ILO":
                    text = "Iloko";
                    break;
                case "INA":
                    text = "Interlingua (International Auxiliary Language Association";
                    break;
                case "INC":
                    text = "Indic (Other)";
                    break;
                case "IND":
                    text = "Indonesian";
                    break;
                case "INE":
                    text = "Indo-European (Other)";
                    break;
                case "INH":
                    text = "Ingush";
                    break;
                case "INT":
                    text = "Interlingua (International Auxiliary Language Association)";
                    break;
                case "IPK":
                    text = "Inupiaq";
                    break;
                case "IRA":
                    text = "Iranian (Other)";
                    break;
                case "IRI":
                    text = "Irish";
                    break;
                case "IRO":
                    text = "Iroquoian (Other)";
                    break;
                case "ITA":
                    text = "Italian";
                    break;
                case "JAV":
                    text = "Javanese";
                    break;
                case "JPN":
                    text = "Japanese";
                    break;
                case "JPR":
                    text = "Judeo-Persian";
                    break;
                case "JRB":
                    text = "Judeo-Arabic";
                    break;
                case "KAA":
                    text = "Kara-Kalpak";
                    break;
                case "KAB":
                    text = "Kabyle";
                    break;
                case "KAC":
                    text = "Kachin";
                    break;
                case "KAL":
                    text = "Kalâtdlisut";
                    break;
                case "KAM":
                    text = "Kamba";
                    break;
                case "KAN":
                    text = "Kannada";
                    break;
                case "KAR":
                    text = "Karen";
                    break;
                case "KAS":
                    text = "Kashmiri";
                    break;
                case "KAU":
                    text = "Kanuri";
                    break;
                case "KAW":
                    text = "Kawi";
                    break;
                case "KAZ":
                    text = "Kazakh";
                    break;
                case "KBD":
                    text = "Kabardian";
                    break;
                case "KHA":
                    text = "Khasi";
                    break;
                case "KHI":
                    text = "Khoisan (Other)";
                    break;
                case "KHM":
                    text = "Khmer";
                    break;
                case "KHO":
                    text = "Khotanese";
                    break;
                case "KIK":
                    text = "Kikuyu";
                    break;
                case "KIN":
                    text = "Kinyarwanda";
                    break;
                case "KIR":
                    text = "Kyrgyz";
                    break;
                case "KMB":
                    text = "Kimbundu";
                    break;
                case "KOK":
                    text = "Konkani";
                    break;
                case "KOM":
                    text = "Komi";
                    break;
                case "KON":
                    text = "Kongo";
                    break;
                case "KOR":
                    text = "Korean";
                    break;
                case "KOS":
                    text = "Kusaie";
                    break;
                case "KPE":
                    text = "Kpelle";
                    break;
                case "KRO":
                    text = "Kru";
                    break;
                case "KRU":
                    text = "Kurukh";
                    break;
                case "KUA":
                    text = "Kuanyama";
                    break;
                case "KUM":
                    text = "Kumyk";
                    break;
                case "KUR":
                    text = "Kurdish";
                    break;
                case "KUS":
                    text = "Kusaie";
                    break;
                case "KUT":
                    text = "Kutenai";
                    break;
                case "LAD":
                    text = "Ladino";
                    break;
                case "LAH":
                    text = "Lahnda";
                    break;
                case "LAM":
                    text = "Lamba";
                    break;
                case "LAN":
                    text = "Occitan (post-1500)";
                    break;
                case "LAO":
                    text = "Lao";
                    break;
                case "LAP":
                    text = "Sami";
                    break;
                case "LAT":
                    text = "Latin";
                    break;
                case "LAV":
                    text = "Latvian";
                    break;
                case "LEZ":
                    text = "Lezgian";
                    break;
                case "LIM":
                    text = "Limburgish";
                    break;
                case "LIN":
                    text = "Lingala";
                    break;
                case "LIT":
                    text = "Lithuanian";
                    break;
                case "LOL":
                    text = "Mongo-Nkundu";
                    break;
                case "LOZ":
                    text = "Lozi";
                    break;
                case "LTZ":
                    text = "Letzeburgesch";
                    break;
                case "LUA":
                    text = "Luba-Lulua";
                    break;
                case "LUB":
                    text = "Luba-Katanga";
                    break;
                case "LUG":
                    text = "Ganda";
                    break;
                case "LUI":
                    text = "Luiseño";
                    break;
                case "LUN":
                    text = "Lunda";
                    break;
                case "LUO":
                    text = "Luo (Kenya and Tanzania)";
                    break;
                case "LUS":
                    text = "Lushai";
                    break;
                case "MAC":
                    text = "Macedonian";
                    break;
                case "MAD":
                    text = "Madurese";
                    break;
                case "MAG":
                    text = "Magahi";
                    break;
                case "MAH":
                    text = "Marshallese";
                    break;
                case "MAI":
                    text = "Maithili";
                    break;
                case "MAK":
                    text = "Makasar";
                    break;
                case "MAL":
                    text = "Malayalam";
                    break;
                case "MAN":
                    text = "Mandingo";
                    break;
                case "MAO":
                    text = "Maori";
                    break;
                case "MAP":
                    text = "Austronesian (Other)";
                    break;
                case "MAR":
                    text = "Marathi";
                    break;
                case "MAS":
                    text = "Masai";
                    break;
                case "MAX":
                    text = "Manx";
                    break;
                case "MAY":
                    text = "Malay";
                    break;
                case "MDR":
                    text = "Mandar";
                    break;
                case "MEN":
                    text = "Mende";
                    break;
                case "MGA":
                    text = "Irish, Middle (ca. 1100-1550)";
                    break;
                case "MIC":
                    text = "Micmac";
                    break;
                case "MIN":
                    text = "Minangkabau";
                    break;
                case "MIS":
                    text = "Miscellaneous languages";
                    break;
                case "MKH":
                    text = "Mon-Khmer (Other)";
                    break;
                case "MLA":
                    text = "Malagasy";
                    break;
                case "MLG":
                    text = "Malagasy";
                    break;
                case "MLT":
                    text = "Maltese";
                    break;
                case "MNC":
                    text = "Manchu";
                    break;
                case "MNI":
                    text = "Manipuri";
                    break;
                case "MNO":
                    text = "Manobo languages";
                    break;
                case "MOH":
                    text = "Mohawk";
                    break;
                case "MOL":
                    text = "Moldavian";
                    break;
                case "MON":
                    text = "Mongolian";
                    break;
                case "MOS":
                    text = "Mooré";
                    break;
                case "MUL":
                    text = "Multiple languages";
                    break;
                case "MUN":
                    text = "Munda (Other)";
                    break;
                case "MUS":
                    text = "Creek";
                    break;
                case "MWR":
                    text = "Marwari";
                    break;
                case "MYN":
                    text = "Mayan languages";
                    break;
                case "NAH":
                    text = "Nahuatl";
                    break;
                case "NAI":
                    text = "North American Indian (Other)";
                    break;
                case "NAP":
                    text = "Neapolitan Italian";
                    break;
                case "NAU":
                    text = "Nauru";
                    break;
                case "NAV":
                    text = "Navajo";
                    break;
                case "NBL":
                    text = "Ndebele (South Africa)";
                    break;
                case "NDE":
                    text = "Ndebele (Zimbabwe)";
                    break;
                case "NDO":
                    text = "Ndonga";
                    break;
                case "NDS":
                    text = "Low German";
                    break;
                case "NEP":
                    text = "Nepali";
                    break;
                case "NEW":
                    text = "Newari";
                    break;
                case "NIA":
                    text = "Nias";
                    break;
                case "NIC":
                    text = "Niger-Kordofanian (Other)";
                    break;
                case "NIU":
                    text = "Niuean";
                    break;
                case "NNO":
                    text = "Norwegian (Nynorsk)";
                    break;
                case "NOB":
                    text = "Norwegian (Bokmål)";
                    break;
                case "NOG":
                    text = "Nogai";
                    break;
                case "NON":
                    text = "Old Norse";
                    break;
                case "NOR":
                    text = "Norwegian";
                    break;
                case "NSO":
                    text = "Northern Sotho";
                    break;
                case "NUB":
                    text = "Nubian languages";
                    break;
                case "NYA":
                    text = "Nyanja";
                    break;
                case "NYM":
                    text = "Nyamwezi";
                    break;
                case "NYN":
                    text = "Nyankole";
                    break;
                case "NYO":
                    text = "Nyoro";
                    break;
                case "NZI":
                    text = "Nzima";
                    break;
                case "OCI":
                    text = "Occitan (post-1500)";
                    break;
                case "OJI":
                    text = "Ojibwa";
                    break;
                case "ORI":
                    text = "Oriya";
                    break;
                case "ORM":
                    text = "Oromo";
                    break;
                case "OSA":
                    text = "Osage";
                    break;
                case "OSS":
                    text = "Ossetic";
                    break;
                case "OTA":
                    text = "Turkish, Ottoman";
                    break;
                case "OTO":
                    text = "Otomian languages";
                    break;
                case "PAA":
                    text = "Papuan (Other)";
                    break;
                case "PAG":
                    text = "Pangasinan";
                    break;
                case "PAL":
                    text = "Pahlavi";
                    break;
                case "PAM":
                    text = "Pampanga";
                    break;
                case "PAN":
                    text = "Panjabi";
                    break;
                case "PAP":
                    text = "Papiamento";
                    break;
                case "PAU":
                    text = "Palauan";
                    break;
                case "PEO":
                    text = "Old Persian (ca. 600-400 B.C.)";
                    break;
                case "PER":
                    text = "Persian";
                    break;
                case "PHI":
                    text = "Philippine (Other)";
                    break;
                case "PHN":
                    text = "Phoenician";
                    break;
                case "PLI":
                    text = "Pali";
                    break;
                case "POL":
                    text = "Polish";
                    break;
                case "PON":
                    text = "Ponape";
                    break;
                case "POR":
                    text = "Portuguese";
                    break;
                case "PRA":
                    text = "Prakrit languages";
                    break;
                case "PRO":
                    text = "Provençal (to 1500)";
                    break;
                case "PUS":
                    text = "Pushto";
                    break;
                case "QUE":
                    text = "Quechua";
                    break;
                case "RAJ":
                    text = "Rajasthani";
                    break;
                case "RAP":
                    text = "Rapanui";
                    break;
                case "RAR":
                    text = "Rarotongan";
                    break;
                case "ROA":
                    text = "Romance (Other)";
                    break;
                case "ROH":
                    text = "Raeto-Romance";
                    break;
                case "ROM":
                    text = "Romani";
                    break;
                case "RUM":
                    text = "Romanian";
                    break;
                case "RUN":
                    text = "Rundi";
                    break;
                case "RUS":
                    text = "Russian";
                    break;
                case "SAD":
                    text = "Sandawe";
                    break;
                case "SAG":
                    text = "Sango (Ubangi Creole)";
                    break;
                case "SAH":
                    text = "Yakut";
                    break;
                case "SAI":
                    text = "South American Indian (Other)";
                    break;
                case "SAL":
                    text = "Salishan languages";
                    break;
                case "SAM":
                    text = "Samaritan Aramaic";
                    break;
                case "SAN":
                    text = "Sanskrit";
                    break;
                case "SAO":
                    text = "Samoan";
                    break;
                case "SAS":
                    text = "Sasak";
                    break;
                case "SAT":
                    text = "Santali";
                    break;
                case "SCC":
                    text = "Serbian";
                    break;
                case "SCO":
                    text = "Scots";
                    break;
                case "SCR":
                    text = "Croatian";
                    break;
                case "SEL":
                    text = "Selkup";
                    break;
                case "SEM":
                    text = "Semitic (Other)";
                    break;
                case "SGA":
                    text = "Irish, Old (to 1100)";
                    break;
                case "SGN":
                    text = "Sign languages";
                    break;
                case "SHN":
                    text = "Shan";
                    break;
                case "SHO":
                    text = "Shona";
                    break;
                case "SID":
                    text = "Sidamo";
                    break;
                case "SIN":
                    text = "Sinhalese";
                    break;
                case "SIO":
                    text = "Siouan (Other)";
                    break;
                case "SIT":
                    text = "Sino-Tibetan (Other)";
                    break;
                case "SLA":
                    text = "Slavic (Other)";
                    break;
                case "SLO":
                    text = "Slovak";
                    break;
                case "SLV":
                    text = "Slovenian";
                    break;
                case "SMA":
                    text = "Southern Sami";
                    break;
                case "SME":
                    text = "Northern Sami";
                    break;
                case "SMI":
                    text = "Sami";
                    break;
                case "SMJ":
                    text = "Lule Sami";
                    break;
                case "SMN":
                    text = "Inari Sami";
                    break;
                case "SMO":
                    text = "Samoan";
                    break;
                case "SMS":
                    text = "Skolt Sami";
                    break;
                case "SNA":
                    text = "Shona";
                    break;
                case "SND":
                    text = "Sindhi";
                    break;
                case "SNH":
                    text = "Sinhalese";
                    break;
                case "SNK":
                    text = "Soninke";
                    break;
                case "SOG":
                    text = "Sogdian";
                    break;
                case "SOM":
                    text = "Somali";
                    break;
                case "SON":
                    text = "Songhai";
                    break;
                case "SOT":
                    text = "Sotho";
                    break;
                case "SPA":
                    text = "Spanish";
                    break;
                case "SRD":
                    text = "Sardinian";
                    break;
                case "SRR":
                    text = "Serer";
                    break;
                case "SSA":
                    text = "Nilo-Saharan (Other)";
                    break;
                case "SSO":
                    text = "Sotho";
                    break;
                case "SSW":
                    text = "Swazi";
                    break;
                case "SUK":
                    text = "Sukuma";
                    break;
                case "SUN":
                    text = "Sundanese";
                    break;
                case "SUS":
                    text = "Susu";
                    break;
                case "SUX":
                    text = "Sumerian";
                    break;
                case "SWA":
                    text = "Swahili";
                    break;
                case "SWE":
                    text = "Swedish";
                    break;
                case "SWZ":
                    text = "Swazi";
                    break;
                case "SYR":
                    text = "Syriac";
                    break;
                case "TAG":
                    text = "Tagalog";
                    break;
                case "TAH":
                    text = "Tahitian";
                    break;
                case "TAI":
                    text = "Tai (Other)";
                    break;
                case "TAJ":
                    text = "Tajik";
                    break;
                case "TAM":
                    text = "Tamil";
                    break;
                case "TAR":
                    text = "Tatar";
                    break;
                case "TAT":
                    text = "Tatar";
                    break;
                case "TEL":
                    text = "Telugu";
                    break;
                case "TEM":
                    text = "Temne";
                    break;
                case "TER":
                    text = "Terena";
                    break;
                case "TET":
                    text = "Tetum";
                    break;
                case "TGK":
                    text = "Tajik";
                    break;
                case "TGL":
                    text = "Tagalog";
                    break;
                case "THA":
                    text = "Thai";
                    break;
                case "TIB":
                case "BOD":
                case "BO":
                    text = "Tibetan";
                    break;
                case "TIG":
                    text = "Tigré";
                    break;
                case "TIR":
                    text = "Tigrinya";
                    break;
                case "TIV":
                    text = "Tiv";
                    break;
                case "TKL":
                    text = "Tokelauan";
                    break;
                case "TLI":
                    text = "Tlingit";
                    break;
                case "TMH":
                    text = "Tamashek";
                    break;
                case "TOG":
                    text = "Tonga (Nyasa)";
                    break;
                case "TON":
                    text = "Tongan";
                    break;
                case "TPI":
                    text = "Tok Pisin";
                    break;
                case "TRU":
                    text = "Truk";
                    break;
                case "TSI":
                    text = "Tsimshian";
                    break;
                case "TSN":
                    text = "Tswana";
                    break;
                case "TSO":
                    text = "Tsonga";
                    break;
                case "TSW":
                    text = "Tswana";
                    break;
                case "TUK":
                    text = "Turkmen";
                    break;
                case "TUM":
                    text = "Tumbuka";
                    break;
                case "TUP":
                    text = "Tupi languages";
                    break;
                case "TUR":
                    text = "Turkish";
                    break;
                case "TUT":
                    text = "Altaic (Other)";
                    break;
                case "TVL":
                    text = "Tuvaluan";
                    break;
                case "TWI":
                    text = "Twi";
                    break;
                case "TYV":
                    text = "Tuvinian";
                    break;
                case "UDM":
                    text = "Udmurt";
                    break;
                case "UGA":
                    text = "Ugaritic";
                    break;
                case "UIG":
                    text = "Uighur";
                    break;
                case "UKR":
                    text = "Ukrainian";
                    break;
                case "UMB":
                    text = "Umbundu";
                    break;
                case "UND":
                    text = "Undetermined";
                    break;
                case "URD":
                    text = "Urdu";
                    break;
                case "UZB":
                    text = "Uzbek";
                    break;
                case "VAI":
                    text = "Vai";
                    break;
                case "VEN":
                    text = "Venda";
                    break;
                case "VIE":
                    text = "Vietnamese";
                    break;
                case "VOL":
                    text = "Volapük";
                    break;
                case "VOT":
                    text = "Votic";
                    break;
                case "WAK":
                    text = "Wakashan languages";
                    break;
                case "WAL":
                    text = "Walamo";
                    break;
                case "WAR":
                    text = "Waray";
                    break;
                case "WAS":
                    text = "Washo";
                    break;
                case "WEL":
                case "CYM":
                case "CY":
                    text = "Welsh";
                    break;
                case "WEN":
                    text = "Sorbian languages";
                    break;
                case "WLN":
                    text = "Walloon";
                    break;
                case "WOL":
                    text = "Wolof";
                    break;
                case "XAL":
                    text = "Kalmyk";
                    break;
                case "XHO":
                    text = "Xhosa";
                    break;
                case "YAO":
                    text = "Yao (Africa)";
                    break;
                case "YAP":
                    text = "Yapese";
                    break;
                case "YID":
                    text = "Yiddish";
                    break;
                case "YOR":
                    text = "Yoruba";
                    break;
                case "YPK":
                    text = "Yupik languages";
                    break;
                case "ZAP":
                    text = "Zapotec";
                    break;
                case "ZEN":
                    text = "Zenaga";
                    break;
                case "ZHA":
                    text = "Zhuang";
                    break;
                case "ZND":
                    text = "Zande";
                    break;
                case "ZUL":
                    text = "Zulu";
                    break;
                case "ZUN":
                    text = "Zuni";
                    break;
            }

            return text;
        }

        #endregion

        #region Code to get the ISO code from the language text

        /// <summary> Gets the ISO code from the language text </summary>
        /// <param name="LanguageCheck"> Name of the language to check for ISO code </param>
        /// <returns> Associated ISO code, or an empty string </returns>
        private string Get_Code_By_Language(string LanguageCheck)
        {
            return Get_Code_By_Language_Static(LanguageCheck);
        }

        /// <summary> Gets the ISO code from the language text </summary>
        /// <param name="LanguageCheck"> Name of the language to check for ISO code </param>
        /// <returns> Associated ISO code, or an empty string </returns>
        private static string Get_Code_By_Language_Static(string LanguageCheck)
        {
            string language_upper = LanguageCheck.ToUpper();
            string code = String.Empty;
            switch (language_upper)
            {
                case "AFAR":
                    code = "aar";
                    break;
                case "ABKHAZ":
                    code = "abk";
                    break;
                case "ACHINESE":
                    code = "ace";
                    break;
                case "ACOLI":
                    code = "ach";
                    break;
                case "ADANGME":
                    code = "ada";
                    break;
                case "ADYGEI":
                    code = "ady";
                    break;
                case "AFROASIATIC (OTHER)":
                case "AFROASIATIC":
                    code = "afa";
                    break;
                case "AFRIHILI (ARTIFICIAL LANGUAGE)":
                case "AFRIHILI":
                    code = "afh";
                    break;
                case "AFRIKAANS":
                    code = "afr";
                    break;
                case "ALJAMÍA":
                    code = "ajm";
                    break;
                case "AKAN":
                    code = "aka";
                    break;
                case "AKKADIAN":
                    code = "akk";
                    break;
                case "ALBANIAN":
                    code = "alb";
                    break;
                case "ALEUT":
                    code = "ale";
                    break;
                case "ALGONQUIAN (OTHER)":
                case "ALGONQUIAN":
                    code = "alg";
                    break;
                case "AMHARIC":
                    code = "amh";
                    break;
                case "ENGLISH, OLD (CA. 450-1100)":
                case "ENGLISH, OLD":
                    code = "ang";
                    break;
                case "APACHE LANGUAGES":
                    code = "apa";
                    break;
                case "ARABIC":
                    code = "ara";
                    break;
                case "ARAMAIC":
                    code = "arc";
                    break;
                case "ARAGONESE SPANISH":
                    code = "arg";
                    break;
                case "ARMENIAN":
                    code = "arm";
                    break;
                case "MAPUCHE":
                    code = "arn";
                    break;
                case "ARAPAHO":
                    code = "arp";
                    break;
                case "ARTIFICIAL (OTHER)":
                case "ARTIFICIAL":
                    code = "art";
                    break;
                case "ARAWAK":
                    code = "arw";
                    break;
                case "ASSAMESE":
                    code = "asm";
                    break;
                case "BABLE":
                    code = "ast";
                    break;
                case "ATHAPASCAN (OTHER)":
                case "ATHAPASCAN":
                    code = "ath";
                    break;
                case "AUSTRALIAN LANGUAGES":
                    code = "aus";
                    break;
                case "AVARIC":
                    code = "ava";
                    break;
                case "AVESTAN":
                    code = "ave";
                    break;
                case "AWADHI":
                    code = "awa";
                    break;
                case "AYMARA":
                    code = "aym";
                    break;
                case "AZERBAIJANI":
                    code = "aze";
                    break;
                case "BANDA":
                    code = "bad";
                    break;
                case "BAMILEKE LANGUAGES":
                case "BAMILEKE":
                    code = "bai";
                    break;
                case "BASHKIR":
                    code = "bak";
                    break;
                case "BALUCHI":
                    code = "bal";
                    break;
                case "BAMBARA":
                    code = "bam";
                    break;
                case "BALINESE":
                    code = "ban";
                    break;
                case "BASQUE":
                    code = "baq";
                    break;
                case "BASA":
                    code = "bas";
                    break;
                case "BALTIC (OTHER)":
                case "BALTIC":
                    code = "bat";
                    break;
                case "BEJA":
                    code = "bej";
                    break;
                case "BELARUSIAN":
                    code = "bel";
                    break;
                case "BEMBA":
                    code = "bem";
                    break;
                case "BENGALI":
                    code = "ben";
                    break;
                case "BERBER (OTHER)":
                case "BERBER":
                    code = "ber";
                    break;
                case "BHOJPURI":
                    code = "bho";
                    break;
                case "BIHARI":
                    code = "bih";
                    break;
                case "BIKOL":
                    code = "bik";
                    break;
                case "EDO":
                    code = "bin";
                    break;
                case "BISLAMA":
                    code = "bis";
                    break;
                case "SIKSIKA":
                    code = "bla";
                    break;
                case "BANTU (OTHER)":
                case "BANTU":
                    code = "bnt";
                    break;
                case "BOSNIAN":
                    code = "bos";
                    break;
                case "BRAJ":
                    code = "bra";
                    break;
                case "BRETON":
                    code = "bre";
                    break;
                case "BATAK":
                    code = "btk";
                    break;
                case "BURIAT":
                    code = "bua";
                    break;
                case "BUGIS":
                    code = "bug";
                    break;
                case "BULGARIAN":
                    code = "bul";
                    break;
                case "BURMESE":
                    code = "bur";
                    break;
                case "CADDO":
                    code = "cad";
                    break;
                case "CENTRAL AMERICAN INDIAN (OTHER)":
                case "CENTRAL AMERICAN INDIAN)":
                    code = "cai";
                    break;
                case "KHMER":
                    code = "cam";
                    break;
                case "CARIB":
                    code = "car";
                    break;
                case "CATALAN":
                    code = "cat";
                    break;
                case "CAUCASIAN (OTHER)":
                case "CAUCASIAN":
                    code = "cau";
                    break;
                case "CEBUANO":
                    code = "ceb";
                    break;
                case "CELTIC (OTHER)":
                case "CELTIC":
                    code = "cel";
                    break;
                case "CHAMORRO":
                    code = "cha";
                    break;
                case "CHIBCHA":
                    code = "chb";
                    break;
                case "CHECHEN":
                    code = "che";
                    break;
                case "CHAGATAI":
                    code = "chg";
                    break;
                case "CHINESE":
                    code = "chi";
                    break;
                case "TRUK":
                    code = "chk";
                    break;
                case "MARI":
                    code = "chm";
                    break;
                case "CHINOOK JARGON":
                    code = "chn";
                    break;
                case "CHOCTAW":
                    code = "cho";
                    break;
                case "CHIPEWYAN":
                    code = "chp";
                    break;
                case "CHEROKEE":
                    code = "chr";
                    break;
                case "CHURCH SLAVIC":
                    code = "chu";
                    break;
                case "CHUVASH":
                    code = "chv";
                    break;
                case "CHEYENNE":
                    code = "chy";
                    break;
                case "CHAMIC LANGUAGES":
                    code = "cmc";
                    break;
                case "COPTIC":
                    code = "cop";
                    break;
                case "CORNISH":
                    code = "cor";
                    break;
                case "CORSICAN":
                    code = "cos";
                    break;
                case "CREOLES AND PIDGINS, ENGLISH-BASED (OTHER)":
                case "CREOLES AND PIDGINS, ENGLISH-BASED":
                    code = "cpe";
                    break;
                case "CREOLES AND PIDGINS, FRENCH-BASED (OTHER)":
                case "CREOLES AND PIDGINS, FRENCH-BASED":
                    code = "cpf";
                    break;
                case "CREOLES AND PIDGINS, PORTUGUESE-BASED (OTHER)":
                case "CREOLES AND PIDGINS, PORTUGUESE-BASED":
                    code = "cpp";
                    break;
                case "CREE":
                    code = "cre";
                    break;
                case "CRIMEAN TATAR":
                    code = "crh";
                    break;
                case "CREOLES AND PIDGINS (OTHER)":
                case "CREOLES AND PIDGINS":
                    code = "crp";
                    break;
                case "CUSHITIC (OTHER)":
                case "CUSHITIC":
                    code = "cus";
                    break;
                case "CZECH":
                    code = "cze";
                    break;
                case "DAKOTA":
                    code = "dak";
                    break;
                case "DANISH":
                    code = "dan";
                    break;
                case "DARGWA":
                    code = "dar";
                    break;
                case "DAYAK":
                    code = "day";
                    break;
                case "DELAWARE":
                    code = "del";
                    break;
                case "SLAVE":
                    code = "den";
                    break;
                case "DOGRIB":
                    code = "dgr";
                    break;
                case "DINKA":
                    code = "din";
                    break;
                case "DIVEHI":
                    code = "div";
                    break;
                case "DOGRI":
                    code = "doi";
                    break;
                case "DRAVIDIAN (OTHER)":
                case "DRAVIDIAN":
                    code = "dra";
                    break;
                case "DUALA":
                    code = "dua";
                    break;
                case "DUTCH, MIDDLE (CA. 1050-1350)":
                case "DUTCH, MIDDLE":
                    code = "dum";
                    break;
                case "DUTCH":
                    code = "dut";
                    break;
                case "DYULA":
                    code = "dyu";
                    break;
                case "DZONGKHA":
                    code = "dzo";
                    break;
                case "EFIK":
                    code = "efi";
                    break;
                case "EGYPTIAN":
                    code = "egy";
                    break;
                case "EKAJUK":
                    code = "eka";
                    break;
                case "ELAMITE":
                    code = "elx";
                    break;
                case "ENGLISH":
                    code = "eng";
                    break;
                case "ENGLISH, MIDDLE (1100-1500)":
                case "ENGLISH, MIDDLE":
                    code = "enm";
                    break;
                case "ESKIMO LANGUAGES":
                case "ESKIMO":
                    code = "esk";
                    break;
                case "ESPERANTO":
                    code = "esp";
                    break;
                case "ESTONIAN":
                    code = "est";
                    break;
                case "ETHIOPIC":
                    code = "eth";
                    break;
                case "EWE":
                    code = "ewe";
                    break;
                case "EWONDO":
                    code = "ewo";
                    break;
                case "FANG":
                    code = "fan";
                    break;
                case "FAROESE":
                    code = "far";
                    break;
                case "FANTI":
                    code = "fat";
                    break;
                case "FIJIAN":
                    code = "fij";
                    break;
                case "FINNISH":
                    code = "fin";
                    break;
                case "FINNO-UGRIAN (OTHER)":
                case "FINNO-UGRIAN":
                    code = "fiu";
                    break;
                case "FON":
                    code = "fon";
                    break;
                case "FRENCH":
                    code = "fre";
                    break;
                case "FRISIAN":
                    code = "fri";
                    break;
                case "FRENCH, MIDDLE (CA. 1400-1600)":
                case "FRENCH, MIDDLE)":
                    code = "frm";
                    break;
                case "FRENCH, OLD (CA. 842-1400)":
                case "FRENCH, OLD":
                    code = "fro";
                    break;
                case "FULA":
                    code = "ful";
                    break;
                case "FRIULIAN":
                    code = "fur";
                    break;
                case "GÃ":
                    code = "gaa";
                    break;
                case "SCOTTISH GAELIC":
                    code = "gae";
                    break;
                case "GALICIAN":
                    code = "gag";
                    break;
                case "OROMO":
                    code = "gal";
                    break;
                case "GAYO":
                    code = "gay";
                    break;
                case "GBAYA":
                    code = "gba";
                    break;
                case "GERMANIC (OTHER)":
                case "GERMANIC":
                    code = "gem";
                    break;
                case "GEORGIAN":
                    code = "geo";
                    break;
                case "GERMAN":
                    code = "ger";
                    break;
                case "GILBERTESE":
                    code = "gil";
                    break;
                case "IRISH":
                    code = "gle";
                    break;

                case "MANX":
                    code = "glv";
                    break;
                case "GERMAN, MIDDLE HIGH (CA. 1050-1500)":
                case "GERMAN, MIDDLE HIGH":
                    code = "gmh";
                    break;
                case "GERMAN, OLD HIGH (CA. 750-1050)":
                case "GERMAN, OLD HIGH":
                    code = "goh";
                    break;
                case "GONDI":
                    code = "gon";
                    break;
                case "GORONTALO":
                    code = "gor";
                    break;
                case "GOTHIC":
                    code = "got";
                    break;
                case "GREBO":
                    code = "grb";
                    break;
                case "GREEK, ANCIENT (TO 1453)":
                case "GREEK, ANCIENT":
                    code = "grc";
                    break;
                case "GREEK, MODERN (1453- )":
                case "GREEK, MODERN":
                case "GREEK":
                    code = "gre";
                    break;
                case "GUARANI":
                    code = "grn";
                    break;
                case "GUJARATI":
                    code = "guj";
                    break;
                case "GWICH'IN":
                    code = "gwi";
                    break;
                case "HAIDA":
                    code = "hai";
                    break;
                case "HAITIAN FRENCH CREOLE":
                case "HAITIAN CREOLE":
                case "KREYOL":
                    code = "hat";
                    break;
                case "HAUSA":
                    code = "hau";
                    break;
                case "HAWAIIAN":
                    code = "haw";
                    break;
                case "HEBREW":
                    code = "heb";
                    break;
                case "HERERO":
                    code = "her";
                    break;
                case "HILIGAYNON":
                    code = "hil";
                    break;
                case "HIMACHALI":
                    code = "him";
                    break;
                case "HINDI":
                    code = "hin";
                    break;
                case "HITTITE":
                    code = "hit";
                    break;
                case "HMONG":
                    code = "hmn";
                    break;
                case "HIRI MOTU":
                    code = "hmo";
                    break;
                case "HUNGARIAN":
                    code = "hun";
                    break;
                case "HUPA":
                    code = "hup";
                    break;
                case "IBAN":
                    code = "iba";
                    break;
                case "IGBO":
                    code = "ibo";
                    break;
                case "ICELANDIC":
                    code = "ice";
                    break;
                case "IDO":
                    code = "ido";
                    break;
                case "SICHUAN YI":
                    code = "iii";
                    break;
                case "IJO":
                    code = "ijo";
                    break;
                case "INUKTITUT":
                    code = "iku";
                    break;
                case "INTERLINGUE":
                    code = "ile";
                    break;
                case "ILOKO":
                    code = "ilo";
                    break;
                case "INTERLINGUA (INTERNATIONAL AUXILIARY LANGUAGE ASSOCIATION":
                    code = "ina";
                    break;
                case "INDIC (OTHER)":
                    code = "inc";
                    break;
                case "INDONESIAN":
                    code = "ind";
                    break;
                case "INDO-EUROPEAN (OTHER)":
                    code = "ine";
                    break;
                case "INGUSH":
                    code = "inh";
                    break;
                case "INTERLINGUA (INTERNATIONAL AUXILIARY LANGUAGE ASSOCIATION)":
                    code = "int";
                    break;
                case "INUPIAQ":
                    code = "ipk";
                    break;
                case "IRANIAN (OTHER)":
                case "IRANIAN":
                    code = "ira";
                    break;
                case "IROQUOIAN (OTHER)":
                case "IROQUOIAN":
                    code = "iro";
                    break;
                case "ITALIAN":
                    code = "ita";
                    break;
                case "JAVANESE":
                    code = "jav";
                    break;
                case "JAPANESE":
                    code = "jpn";
                    break;
                case "JUDEO-PERSIAN":
                    code = "jpr";
                    break;
                case "JUDEO-ARABIC":
                    code = "jrb";
                    break;
                case "KARA-KALPAK":
                    code = "kaa";
                    break;
                case "KABYLE":
                    code = "kab";
                    break;
                case "KACHIN":
                    code = "kac";
                    break;
                case "KALÂTDLISUT":
                    code = "kal";
                    break;
                case "KAMBA":
                    code = "kam";
                    break;
                case "KANNADA":
                    code = "kan";
                    break;
                case "KAREN":
                    code = "kar";
                    break;
                case "KASHMIRI":
                    code = "kas";
                    break;
                case "KANURI":
                    code = "kau";
                    break;
                case "KAWI":
                    code = "kaw";
                    break;
                case "KAZAKH":
                    code = "kaz";
                    break;
                case "KABARDIAN":
                    code = "kbd";
                    break;
                case "KHASI":
                    code = "kha";
                    break;
                case "KHOISAN (OTHER)":
                case "KHOISAN":
                    code = "khi";
                    break;
                case "KHOTANESE":
                    code = "kho";
                    break;
                case "KIKUYU":
                    code = "kik";
                    break;
                case "KINYARWANDA":
                    code = "kin";
                    break;
                case "KYRGYZ":
                    code = "kir";
                    break;
                case "KIMBUNDU":
                    code = "kmb";
                    break;
                case "KONKANI":
                    code = "kok";
                    break;
                case "KOMI":
                    code = "kom";
                    break;
                case "KONGO":
                    code = "kon";
                    break;
                case "KOREAN":
                    code = "kor";
                    break;
                case "KUSAIE":
                    code = "kos";
                    break;
                case "KPELLE":
                    code = "kpe";
                    break;
                case "KRU":
                    code = "kro";
                    break;
                case "KURUKH":
                    code = "kru";
                    break;
                case "KUANYAMA":
                    code = "kua";
                    break;
                case "KUMYK":
                    code = "kum";
                    break;
                case "KURDISH":
                    code = "kur";
                    break;
                case "KUTENAI":
                    code = "kut";
                    break;
                case "LADINO":
                    code = "lad";
                    break;
                case "LAHNDA":
                    code = "lah";
                    break;
                case "LAMBA":
                    code = "lam";
                    break;
                case "OCCITAN (POST-1500)":
                    code = "lan";
                    break;
                case "LAO":
                    code = "lao";
                    break;
                case "SAMI":
                    code = "lap";
                    break;
                case "LATIN":
                    code = "lat";
                    break;
                case "LATVIAN":
                    code = "lav";
                    break;
                case "LEZGIAN":
                    code = "lez";
                    break;
                case "LIMBURGISH":
                    code = "lim";
                    break;
                case "LINGALA":
                    code = "lin";
                    break;
                case "LITHUANIAN":
                    code = "lit";
                    break;
                case "MONGO-NKUNDU":
                    code = "lol";
                    break;
                case "LOZI":
                    code = "loz";
                    break;
                case "LETZEBURGESCH":
                    code = "ltz";
                    break;
                case "LUBA-LULUA":
                    code = "lua";
                    break;
                case "LUBA-KATANGA":
                    code = "lub";
                    break;
                case "GANDA":
                    code = "lug";
                    break;
                case "LUISEÑO":
                    code = "lui";
                    break;
                case "LUNDA":
                    code = "lun";
                    break;
                case "LUO (KENYA AND TANZANIA)":
                case "LUO":
                    code = "luo";
                    break;
                case "LUSHAI":
                    code = "lus";
                    break;
                case "MACEDONIAN":
                    code = "mac";
                    break;
                case "MADURESE":
                    code = "mad";
                    break;
                case "MAGAHI":
                    code = "mag";
                    break;
                case "MARSHALLESE":
                    code = "mah";
                    break;
                case "MAITHILI":
                    code = "mai";
                    break;
                case "MAKASAR":
                    code = "mak";
                    break;
                case "MALAYALAM":
                    code = "mal";
                    break;
                case "MANDINGO":
                    code = "man";
                    break;
                case "MAORI":
                    code = "mao";
                    break;
                case "AUSTRONESIAN (OTHER)":
                case "AUSTRONESIAN":
                    code = "map";
                    break;
                case "MARATHI":
                    code = "mar";
                    break;
                case "MASAI":
                    code = "mas";
                    break;
                case "MALAY":
                    code = "may";
                    break;
                case "MANDAR":
                    code = "mdr";
                    break;
                case "MENDE":
                    code = "men";
                    break;
                case "IRISH, MIDDLE (CA. 1100-1550)":
                case "IRISH, MIDDLE":
                    code = "mga";
                    break;
                case "MICMAC":
                    code = "mic";
                    break;
                case "MINANGKABAU":
                    code = "min";
                    break;
                case "MISCELLANEOUS LANGUAGES":
                    code = "mis";
                    break;
                case "MON-KHMER (OTHER)":
                case "MON-KHMER":
                    code = "mkh";
                    break;
                case "MALAGASY":
                    code = "mla";
                    break;
                case "MALTESE":
                    code = "mlt";
                    break;
                case "MANCHU":
                    code = "mnc";
                    break;
                case "MANIPURI":
                    code = "mni";
                    break;
                case "MANOBO LANGUAGES":
                    code = "mno";
                    break;
                case "MOHAWK":
                    code = "moh";
                    break;
                case "MOLDAVIAN":
                    code = "mol";
                    break;
                case "MONGOLIAN":
                    code = "mon";
                    break;
                case "MOORÉ":
                    code = "mos";
                    break;
                case "MULTIPLE LANGUAGES":
                    code = "mul";
                    break;
                case "MUNDA (OTHER)":
                case "MUNDA":
                    code = "mun";
                    break;
                case "CREEK":
                    code = "mus";
                    break;
                case "MARWARI":
                    code = "mwr";
                    break;
                case "MAYAN LANGUAGES":
                    code = "myn";
                    break;
                case "NAHUATL":
                    code = "nah";
                    break;
                case "NORTH AMERICAN INDIAN (OTHER)":
                case "NORTH AMERICAN INDIAN":
                    code = "nai";
                    break;
                case "NEAPOLITAN ITALIAN":
                    code = "nap";
                    break;
                case "NAURU":
                    code = "nau";
                    break;
                case "NAVAJO":
                    code = "nav";
                    break;
                case "NDEBELE (SOUTH AFRICA)":
                    code = "nbl";
                    break;
                case "NDEBELE (ZIMBABWE)":
                    code = "nde";
                    break;
                case "NDONGA":
                    code = "ndo";
                    break;
                case "LOW GERMAN":
                    code = "nds";
                    break;
                case "NEPALI":
                    code = "nep";
                    break;
                case "NEWARI":
                    code = "new";
                    break;
                case "NIAS":
                    code = "nia";
                    break;
                case "NIGER-KORDOFANIAN (OTHER)":
                case "NIGER-KORDOFANIAN":
                    code = "nic";
                    break;
                case "NIUEAN":
                    code = "niu";
                    break;
                case "NORWEGIAN (NYNORSK)":
                case "NYNORSK":
                    code = "nno";
                    break;
                case "NORWEGIAN (BOKMÅL)":
                case "BOKMÅL":
                    code = "nob";
                    break;
                case "NOGAI":
                    code = "nog";
                    break;
                case "OLD NORSE":
                    code = "non";
                    break;
                case "NORWEGIAN":
                    code = "nor";
                    break;
                case "NORTHERN SOTHO":
                    code = "nso";
                    break;
                case "NUBIAN LANGUAGES":
                    code = "nub";
                    break;
                case "NYANJA":
                    code = "nya";
                    break;
                case "NYAMWEZI":
                    code = "nym";
                    break;
                case "NYANKOLE":
                    code = "nyn";
                    break;
                case "NYORO":
                    code = "nyo";
                    break;
                case "NZIMA":
                    code = "nzi";
                    break;
                case "OCCITAN":
                    code = "oci";
                    break;
                case "OJIBWA":
                    code = "oji";
                    break;
                case "ORIYA":
                    code = "ori";
                    break;
                case "OSAGE":
                    code = "osa";
                    break;
                case "OSSETIC":
                    code = "oss";
                    break;
                case "TURKISH, OTTOMAN":
                case "OTTOMAN":
                    code = "ota";
                    break;
                case "OTOMIAN LANGUAGES":
                    code = "oto";
                    break;
                case "PAPUAN (OTHER)":
                case "PAPUAN":
                    code = "paa";
                    break;
                case "PANGASINAN":
                    code = "pag";
                    break;
                case "PAHLAVI":
                    code = "pal";
                    break;
                case "PAMPANGA":
                    code = "pam";
                    break;
                case "PANJABI":
                    code = "pan";
                    break;
                case "PAPIAMENTO":
                    code = "pap";
                    break;
                case "PALAUAN":
                    code = "pau";
                    break;
                case "OLD PERSIAN (CA. 600-400 B.C.)":
                case "OLD PERSIAN":
                    code = "peo";
                    break;
                case "PERSIAN":
                    code = "per";
                    break;
                case "PHILIPPINE (OTHER)":
                case "PHILIPPINE":
                    code = "phi";
                    break;
                case "PHOENICIAN":
                    code = "phn";
                    break;
                case "PALI":
                    code = "pli";
                    break;
                case "POLISH":
                    code = "pol";
                    break;
                case "PONAPE":
                    code = "pon";
                    break;
                case "PORTUGUESE":
                    code = "por";
                    break;
                case "PRAKRIT LANGUAGES":
                case "PRAKRIT":
                    code = "pra";
                    break;
                case "PROVENÇAL (TO 1500)":
                case "PROVENÇAL":
                    code = "pro";
                    break;
                case "PUSHTO":
                    code = "pus";
                    break;
                case "QUECHUA":
                    code = "que";
                    break;
                case "RAJASTHANI":
                    code = "raj";
                    break;
                case "RAPANUI":
                    code = "rap";
                    break;
                case "RAROTONGAN":
                    code = "rar";
                    break;
                case "ROMANCE (OTHER)":
                    code = "roa";
                    break;
                case "RAETO-ROMANCE":
                    code = "roh";
                    break;
                case "ROMANI":
                    code = "rom";
                    break;
                case "ROMANIAN":
                    code = "rum";
                    break;
                case "RUNDI":
                    code = "run";
                    break;
                case "RUSSIAN":
                    code = "rus";
                    break;
                case "SANDAWE":
                    code = "sad";
                    break;
                case "SANGO (UBANGI CREOLE)":
                case "SANGO":
                    code = "sag";
                    break;
                case "YAKUT":
                    code = "sah";
                    break;
                case "SOUTH AMERICAN INDIAN (OTHER)":
                case "SOUTH AMERICAN INDIAN":
                    code = "sai";
                    break;
                case "SALISHAN LANGUAGES":
                    code = "sal";
                    break;
                case "SAMARITAN ARAMAIC":
                    code = "sam";
                    break;
                case "SANSKRIT":
                    code = "san";
                    break;
                case "SAMOAN":
                    code = "sao";
                    break;
                case "SASAK":
                    code = "sas";
                    break;
                case "SANTALI":
                    code = "sat";
                    break;
                case "SERBIAN":
                    code = "scc";
                    break;
                case "SCOTS":
                    code = "sco";
                    break;
                case "CROATIAN":
                    code = "scr";
                    break;
                case "SELKUP":
                    code = "sel";
                    break;
                case "SEMITIC (OTHER)":
                case "SEMITIC":
                    code = "sem";
                    break;
                case "IRISH, OLD (TO 1100)":
                case "IRISH, OLD":
                    code = "sga";
                    break;
                case "SIGN LANGUAGES":
                    code = "sgn";
                    break;
                case "SHAN":
                    code = "shn";
                    break;
                case "SHONA":
                    code = "sho";
                    break;
                case "SIDAMO":
                    code = "sid";
                    break;
                case "SINHALESE":
                    code = "sin";
                    break;
                case "SIOUAN (OTHER)":
                case "SIOUAN":
                    code = "sio";
                    break;
                case "SINO-TIBETAN (OTHER)":
                case "SINO-TIBETAN":
                    code = "sit";
                    break;
                case "SLAVIC (OTHER)":
                case "SLAVIC":
                    code = "sla";
                    break;
                case "SLOVAK":
                    code = "slo";
                    break;
                case "SLOVENIAN":
                    code = "slv";
                    break;
                case "SOUTHERN SAMI":
                    code = "sma";
                    break;
                case "NORTHERN SAMI":
                    code = "sme";
                    break;
                case "LULE SAMI":
                    code = "smj";
                    break;
                case "INARI SAMI":
                    code = "smn";
                    break;
                case "SKOLT SAMI":
                    code = "sms";
                    break;
                case "SINDHI":
                    code = "snd";
                    break;
                case "SONINKE":
                    code = "snk";
                    break;
                case "SOGDIAN":
                    code = "sog";
                    break;
                case "SOMALI":
                    code = "som";
                    break;
                case "SONGHAI":
                    code = "son";
                    break;
                case "SOTHO":
                    code = "sot";
                    break;
                case "SPANISH":
                    code = "spa";
                    break;
                case "SARDINIAN":
                    code = "srd";
                    break;
                case "SERER":
                    code = "srr";
                    break;
                case "NILO-SAHARAN (OTHER)":
                case "NILO-SAHARAN":
                    code = "ssa";
                    break;
                case "SWAZI":
                    code = "ssw";
                    break;
                case "SUKUMA":
                    code = "suk";
                    break;
                case "SUNDANESE":
                    code = "sun";
                    break;
                case "SUSU":
                    code = "sus";
                    break;
                case "SUMERIAN":
                    code = "sux";
                    break;
                case "SWAHILI":
                    code = "swa";
                    break;
                case "SWEDISH":
                    code = "swe";
                    break;
                case "SYRIAC":
                    code = "syr";
                    break;
                case "TAGALOG":
                    code = "tag";
                    break;
                case "TAHITIAN":
                    code = "tah";
                    break;
                case "TAI (OTHER)":
                case "TAI":
                    code = "tai";
                    break;
                case "TAJIK":
                    code = "taj";
                    break;
                case "TAMIL":
                    code = "tam";
                    break;
                case "TATAR":
                    code = "tar";
                    break;
                case "TELUGU":
                    code = "tel";
                    break;
                case "TEMNE":
                    code = "tem";
                    break;
                case "TERENA":
                    code = "ter";
                    break;
                case "TETUM":
                    code = "tet";
                    break;
                case "THAI":
                    code = "tha";
                    break;
                case "TIBETAN":
                    code = "tib";
                    break;
                case "TIGRÉ":
                    code = "tig";
                    break;
                case "TIGRINYA":
                    code = "tir";
                    break;
                case "TIV":
                    code = "tiv";
                    break;
                case "TOKELAUAN":
                    code = "tkl";
                    break;
                case "TLINGIT":
                    code = "tli";
                    break;
                case "TAMASHEK":
                    code = "tmh";
                    break;
                case "TONGA (NYASA)":
                case "TONGA":
                    code = "tog";
                    break;
                case "TONGAN":
                    code = "ton";
                    break;
                case "TOK PISIN":
                    code = "tpi";
                    break;
                case "TSIMSHIAN":
                    code = "tsi";
                    break;
                case "TSWANA":
                    code = "tsn";
                    break;
                case "TSONGA":
                    code = "tso";
                    break;
                case "TURKMEN":
                    code = "tuk";
                    break;
                case "TUMBUKA":
                    code = "tum";
                    break;
                case "TUPI LANGUAGES":
                    code = "tup";
                    break;
                case "TURKISH":
                    code = "tur";
                    break;
                case "ALTAIC (OTHER)":
                case "ALTAIC":
                    code = "tut";
                    break;
                case "TUVALUAN":
                    code = "tvl";
                    break;
                case "TWI":
                    code = "twi";
                    break;
                case "TUVINIAN":
                    code = "tyv";
                    break;
                case "UDMURT":
                    code = "udm";
                    break;
                case "UGARITIC":
                    code = "uga";
                    break;
                case "UIGHUR":
                    code = "uig";
                    break;
                case "UKRAINIAN":
                    code = "ukr";
                    break;
                case "UMBUNDU":
                    code = "umb";
                    break;
                case "UNDETERMINED":
                    code = "und";
                    break;
                case "URDU":
                    code = "urd";
                    break;
                case "UZBEK":
                    code = "uzb";
                    break;
                case "VAI":
                    code = "vai";
                    break;
                case "VENDA":
                    code = "ven";
                    break;
                case "VIETNAMESE":
                    code = "vie";
                    break;
                case "VOLAPÜK":
                    code = "vol";
                    break;
                case "VOTIC":
                    code = "vot";
                    break;
                case "WAKASHAN LANGUAGES":
                    code = "wak";
                    break;
                case "WALAMO":
                    code = "wal";
                    break;
                case "WARAY":
                    code = "war";
                    break;
                case "WASHO":
                    code = "was";
                    break;
                case "WELSH":
                    code = "wel";
                    break;
                case "SORBIAN LANGUAGES":
                    code = "wen";
                    break;
                case "WALLOON":
                    code = "wln";
                    break;
                case "WOLOF":
                    code = "wol";
                    break;
                case "KALMYK":
                    code = "xal";
                    break;
                case "XHOSA":
                    code = "xho";
                    break;
                case "YAO (AFRICA)":
                case "YAO":
                    code = "yao";
                    break;
                case "YAPESE":
                    code = "yap";
                    break;
                case "YIDDISH":
                    code = "yid";
                    break;
                case "YORUBA":
                    code = "yor";
                    break;
                case "YUPIK LANGUAGES":
                    code = "ypk";
                    break;
                case "ZAPOTEC":
                    code = "zap";
                    break;
                case "ZENAGA":
                    code = "zen";
                    break;
                case "ZHUANG":
                    code = "zha";
                    break;
                case "ZANDE":
                    code = "znd";
                    break;
                case "ZULU":
                    code = "zul";
                    break;
                case "ZUNI":
                    code = "zun";
                    break;
            }

            return code;
        }

        #endregion
    }
}