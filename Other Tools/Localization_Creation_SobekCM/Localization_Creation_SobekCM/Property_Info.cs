using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SobekCM.Library.Localization
{
    class Property_Info
    {
        public string Name { get; set; }

        public string English_Term { get; set; }

        public string Remarks { get; set; }

	    public string Name_To_Use_For_Property
	    {
		    get
		    {
				TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
				string cleaned = Name.Replace("~", "").Replace("|", "").Replace("<i>", "").Replace("</i>", "").Replace("<b>", "").Replace("</b>", "").Replace("[", "").Replace("]", "").Replace("+", "").Replace("@", "").Replace("%1", "XXX").Replace("%2", "XXX").Replace("%3", "XXX").Replace("%4", "XXX").Replace(":", "").Replace(",", " ").Replace(".", "").Replace("(", "").Replace(")", "").Replace("%", "").Replace("=", " ").Replace("\\", "").Replace("/", "").Replace("\"", "").Replace("'", "").Replace("<", " ").Replace(">", " ").Replace(";", "").Replace("&", "").Replace("-", "").Replace("?", "").Replace("!", "").Replace("{0}", "XXX").Replace("{1}", "XXX").Replace("{2}", "XXX").Replace("{3}", "XXX").Replace("{4}", "XXX").Replace("  ", " ").Replace("  ", " ").Trim();
			    if (Char.IsNumber(cleaned[0]))
				    cleaned = cleaned.Substring(1).Trim();
			    if (cleaned.Length > 500)
				    cleaned = cleaned.Substring(0, 500);
			    return textInfo.ToTitleCase(cleaned).Replace(" ", "");
		    }
	    }

		public string Name_To_Use_In_XML
		{
			get
			{
				TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
				string cleaned = Name.Replace("~", "").Replace("|", "").Replace("<b>", "").Replace("</b>", "").Replace("[", "").Replace("]", "").Replace("+", "").Replace("@", "").Replace("%1", "XXX").Replace("%2", "XXX").Replace("%3", "XXX").Replace("%4", "XXX").Replace(":", "").Replace(",", " ").Replace(".", "").Replace("(", "").Replace(")", "").Replace("%", "").Replace("=", " ").Replace("\\", "").Replace("/", "").Replace("\"", "").Replace("'", "").Replace("<", " ").Replace(">", " ").Replace(";", "").Replace("&", "").Replace("-", "").Replace("?", "").Replace("!", "").Replace("{0}", "XXX").Replace("{1}", "XXX").Replace("{2}", "XXX").Replace("{3}", "XXX").Replace("{4}", "XXX").Replace("  ", " ").Replace("  ", " ").Trim();
				if (Char.IsNumber(cleaned[0]))
					cleaned = cleaned.Substring(1).Trim();
				return textInfo.ToTitleCase(cleaned);
			}
		}
    }
}
