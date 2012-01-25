#region Using directives

using System;
using System.IO;
using System.Web.UI.WebControls;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Enumeration indicates the size of a button to render </summary>
    public enum Button_Size : byte
    { 
        /// <summary> Normal-sized button to be rendered </summary>
        Normal = 1, 

        /// <summary> Larger-sized button to be rendered </summary>
        Large 
    };

    /// <summary> Enumeration indicates the type of button to render </summary>
    public enum Button_Type : byte
    { 
        /// <summary> Standard button, with no arrow </summary>
        Standard = 0, 

        /// <summary> Start button includes a left arrow and line indicating to go all the way to the beginning </summary>
        Start, 

        /// <summary> Previous button includes a left arrow indicating to go back one step  </summary>
        Previous,

        /// <summary> Previous button includes a right arrow indicating to go forward one step  </summary>
        Next,

        /// <summary> Start button includes a right arrow and line indicating to go all the way to the end </summary>
        Last 
    };

    /// <summary> [UNDER DEVELOPMENT] Static class assists with rendering the buttons required as HTML </summary>
    public class button_html_writer
    {
        /// <summary> Write the requrested button to the output stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Button_Text"> Text to include in the button </param>
        /// <param name="Size"> Size of the button to render </param>
        /// <param name="Type"> Type of button to render </param>
        /// <param name="Base_Skin"> Code the base web html skin, indicating which button images to use </param>
        /// <param name="Browser"> Type of browser from the original request since button rendering is somewhat browser specific</param>
        /// <param name="Link"> Link to include for the button </param>
        /// <param name="Base_URL"> Base URL for the images </param>
        public static void Write_Button( TextWriter Output, string Button_Text, Button_Size Size, Button_Type Type, string Base_Skin, string Browser, string Link, string Base_URL )
        {
            Output.WriteLine("<br /><br />");
            Output.WriteLine(" This button works for IE: <a href=\"index2.html\"><img src=\"" + Base_URL + "design/skins/" + Base_Skin + "/buttons/largbuttonw_left.jpg\" /><span class=\"largbutton_text_ie\">SUBMIT</span><img src=\"" + Base_URL + "design/skins/" + Base_Skin + "/buttons/largbuttonw_right.jpg\" /></a>");
            Output.WriteLine("<br /><br />");
            Output.WriteLine(" This button works for Mozilla: <a href=\"index2.html\"><img src=\"" + Base_URL + "design/skins/" + Base_Skin + "/buttons/largbuttonw_left.jpg\" /><span class=\"largbutton_text_mozilla\">SUBMIT</span><img src=\"" + Base_URL + "design/skins/" + Base_Skin + "/buttons/largbuttonw_right.jpg\" /></a>");
            Output.WriteLine("<br /><br />");
        }

        /// <summary> Create a button as a control </summary>
        /// <param name="Button_Text"> Text to include in the button </param>
        /// <param name="Size"> Size of the button to render </param>
        /// <param name="Type"> Type of button to render </param>
        /// <param name="Base_Skin"> Code the base web html skin, indicating which button images to use </param>
        /// <param name="Browser"> Type of browser from the original request since button rendering is somewhat browser specific</param>
        /// <returns></returns>
        public static ImageButton Create_Button( string Button_Text, Button_Size Size, Button_Type Type, string Base_Skin, string Browser )
        {
            return new ImageButton();
        }

        /// <summary> Return the HTML for a button as a string </summary>
        /// <param name="Button_Text"> Text to include in the button </param>
        /// <param name="Size"> Size of the button to render </param>
        /// <param name="Type"> Type of button to render </param>
        /// <param name="Base_Skin"> Code the base web html skin, indicating which button images to use </param>
        /// <param name="Browser"> Type of browser from the original request since button rendering is somewhat browser specific</param>
        /// <param name="Link"> Link to include for the button </param>
        /// <returns> String including the HTML for the butotn to render</returns>
        public static string Button_HTML(string Button_Text, Button_Size Size, Button_Type Type, string Base_Skin, string Browser, string Link)
        {
            return String.Empty;
        }
    }
}
