#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SobekCM.Core.Configuration;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;

#endregion

namespace SobekCM.Library.Aggregations
{
    /// <summary> Holds all the information about an item aggregation highlight, including text, image, tooltip, and link </summary>
    [Serializable]
    public class Item_Aggregation_Highlights
    {
        private readonly Dictionary<Web_Language_Enum, string> text;
        private readonly Dictionary<Web_Language_Enum, string> tooltips;

        /// <summary> Constructor for a new instance of the Item_Aggregation_Highlights class </summary>
        public Item_Aggregation_Highlights()
        {
            tooltips = new Dictionary<Web_Language_Enum, string>();
            text = new Dictionary<Web_Language_Enum, string>();
            Link = String.Empty;
            Image = String.Empty;
        }

        /// <summary> Gets the dictionary of languages to text </summary>
        public Dictionary<Web_Language_Enum, string> Text_Dictionary
        {
            get { return text; }
        }

        /// <summary> Gets the dictionary of languages to tooltips </summary>
        public Dictionary<Web_Language_Enum, string> Tooltip_Dictionary
        {
            get { return tooltips; }
        }

        /// <summary> Gets and set the link that the user goes to when they click on this image </summary>
        public string Link { get; set; }

        /// <summary> Gets and sets the image to display as the highlight </summary>
        public string Image { get; set; }

        internal void Write_In_Configuration_XML_File( StreamWriter Writer )
        {
            Writer.WriteLine("    <hi:highlight>");
            Writer.WriteLine("      <hi:source>" + Image + "</hi:source>");
            if (Link.Length > 0)
            {
                Writer.WriteLine("      <hi:link>" + Link + "</hi:link>");
            }
            foreach (KeyValuePair<Web_Language_Enum, string> thisTooltip in tooltips)
            {
                if (thisTooltip.Key == Web_Language_Enum.UNDEFINED)
                    Writer.WriteLine("      <hi:tooltip>" + thisTooltip.Value + "</hi:tooltip>");
                else
                    Writer.WriteLine("      <hi:tooltip lang=\"" + Web_Language_Enum_Converter.Enum_To_Code(thisTooltip.Key) + "\">" + thisTooltip.Value + "</hi:tooltip>");
            }
            foreach (KeyValuePair<Web_Language_Enum, string> thisText in text)
            {
                if (thisText.Key == Web_Language_Enum.UNDEFINED)
                    Writer.WriteLine("      <hi:text>" + thisText.Value + "</hi:text>");
                else
                    Writer.WriteLine("      <hi:text lang=\"" + Web_Language_Enum_Converter.Enum_To_Code(thisText.Key) + "\">" + thisText.Value + "</hi:text>");
            }
            Writer.WriteLine("    </hi:highlight>");
        }

        /// <summary> Add a language tooltip to this highlight </summary>
        /// <param name="Language">Language enumeration for this tooltip </param>
        /// <param name="Tooltip"> Tooltip </param>
        public void Add_Tooltip(Web_Language_Enum Language, string Tooltip)
        {
            tooltips[Language] = Tooltip;
        }

        /// <summary> Gets the language-specific tooltip, if one exists </summary>
        /// <param name="Language"> Language of the tooltip to retrieve </param>
        /// <returns> Language-specific tooltip </returns>
        public string Get_Tooltip(Web_Language_Enum Language)
        {
            if (tooltips.ContainsKey(Language))
                return tooltips[Language];

            if (tooltips.ContainsKey(Web_Language_Enum.DEFAULT))
                return tooltips[Web_Language_Enum.DEFAULT];

            if (tooltips.ContainsKey(Web_Language_Enum.English))
                return tooltips[Web_Language_Enum.English];

            if (tooltips.Count > 0)
                return tooltips.ElementAt(0).Value;

            return string.Empty;
        }

        /// <summary> Add a language text to this highlight </summary>
        /// <param name="Language">Language enumeration for this text </param>
        /// <param name="Text"> Text </param>
        public void Add_Text(Web_Language_Enum Language, string Text)
        {
            text[Language] = Text;
        }

        /// <summary> Gets the language-specific text, if one exists </summary>
        /// <param name="Language"> Language of the text to retrieve </param>
        /// <returns> Language-specific text </returns>
        public string Get_Text(Web_Language_Enum Language)
        {
            if (text.ContainsKey(Language))
                return text[Language];

            if (text.ContainsKey(Web_Language_Enum.DEFAULT))
                return text[Web_Language_Enum.DEFAULT];

            if (text.ContainsKey(Web_Language_Enum.English))
                return text[Web_Language_Enum.English];

            if (text.Count > 0)
                return text.ElementAt(0).Value;

            return string.Empty;
        }

        /// <summary> Outputs this highlight information in the standard HTML format, per the indicated language </summary>
        /// <param name="Language"> Language of the current interface </param>
        /// <param name="Directory"> Directory within which the image will be found for the image </param>
        /// <returns> HTML for this highlight </returns>
        public string ToHTML( Web_Language_Enum Language, string Directory )
        {
            StringBuilder highlightBldr = new StringBuilder(500);
            highlightBldr.Append("<span id=\"SobekHighlight\">" + Environment.NewLine );
            highlightBldr.Append("  <table>" + Environment.NewLine );
            highlightBldr.Append("    <tr><td>" + Environment.NewLine );

            if (Link.Length > 0)
            {
                if (Link.IndexOf("?") > 0)
                {
                    highlightBldr.Append("      <a href=\"" + Link + "<%&URLOPTS%>\" title=\"" + Get_Tooltip(Language) + "\">" + Environment.NewLine );
                }
                else
                {
                    highlightBldr.Append("      <a href=\"" + Link + "<%?URLOPTS%>\" title=\"" + Get_Tooltip(Language) + "\">" + Environment.NewLine );
                }
                highlightBldr.Append("        <img src=\"" + Directory + Image + "\" alt=\"" + Get_Tooltip(Language) + "\"/>" + Environment.NewLine );
                highlightBldr.Append("      </a>" + Environment.NewLine );
            }
            else
            {
                highlightBldr.Append("      <img src=\"" + Directory + Image + "\" alt=\"" + Get_Tooltip(Language) + "\"/>" + Environment.NewLine );
            }

            highlightBldr.Append("    </td></tr>" + Environment.NewLine );

            string textForCurrentLanguage = Get_Text(Language);
            if (textForCurrentLanguage.Length > 0)
            {
                highlightBldr.Append("    <tr><td>" + Environment.NewLine );
                if (Link.Length > 0)
                {
                    if (Link.IndexOf("?") > 0)
                    {
                        highlightBldr.Append("      <a href=\"" + Link + "<%&URLOPTS%>\" title=\"" + Get_Tooltip(Language) + "\">" + Environment.NewLine );
                    }
                    else
                    {
                        highlightBldr.Append("      <a href=\"" + Link + "<%?URLOPTS%>\" title=\"" + Get_Tooltip(Language) + "\">" + Environment.NewLine );
                    }

                    highlightBldr.Append("        <span class=\"SobekHighlightText\"> " + textForCurrentLanguage + " </span>" + Environment.NewLine );
                    highlightBldr.Append("      </a>" + Environment.NewLine );
                }
                else
                {
                    highlightBldr.Append("      <span class=\"SobekHighlightText\"> " + textForCurrentLanguage + " </span>" + Environment.NewLine );
                }
                highlightBldr.Append("    </td></tr>" + Environment.NewLine );
            }

            highlightBldr.Append("  </table>" + Environment.NewLine );
            highlightBldr.Append("</span>");

            return highlightBldr.ToString();

        }
    }
}



