#region Using directives

using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Library.Navigation;
using SobekCM.Tools;
using SobekCM_UI_Library.Navigation;

#endregion

namespace SobekCM.Library.MainWriters
{
    /// <summary> HTML echo writer is generally used just for directing search engine robots to pre-existing 
    /// html pages for indexing items, etc.. </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractMainWriter"/>. </remarks>
    public class Html_Echo_MainWriter : abstractMainWriter
    {
        private readonly string fileToEcho;

        /// <summary> Constructor for a new instance of the Text_MainWriter class </summary>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="HTML_File_To_Echo"> The HTML file to echo </param>
        public Html_Echo_MainWriter(SobekCM_Navigation_Object Current_Mode, string HTML_File_To_Echo)
            : base(Current_Mode, null, null, null, null,  null, null, null)
        {
            fileToEcho = HTML_File_To_Echo;
        }

        /// <summary> Gets the enumeration of the type of main writer </summary>
        /// <value> This property always returns the enumerational value <see cref="SobekCM.Library.Navigation.Writer_Type_Enum.HTML"/>. </value>
        public override Writer_Type_Enum Writer_Type { get { return Writer_Type_Enum.HTML_Echo; } }


        /// <summary> Writes the style references and other data to the HEAD portion of the web page </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_Echo_MainWriter.Write_Within_HTML_Head", "Adding style references to HTML");

            Output.WriteLine("  <meta name=\"robots\" content=\"index, follow\">");

			// Write the style sheet to use 
#if DEBUG
            Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM.css\" rel=\"stylesheet\" type=\"text/css\" />");
#else
			Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM.min.css\" rel=\"stylesheet\" type=\"text/css\" />");

#endif
			// Write the main SobekCM item style sheet to use 
#if DEBUG
			Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM_Item.css\" rel=\"stylesheet\" type=\"text/css\" />");
#else
			Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM_Item.min.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");
#endif

			// Always add jQuery library (changed as of 7/8/2013)
			if ((currentMode.Mode != Display_Mode_Enum.Item_Display) || (currentMode.ViewerCode != "pageturner"))
			{
#if DEBUG
                Output.WriteLine("  <script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-1.10.2.js\"></script>");
				Output.WriteLine("  <script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_full.js\"></script>");
#else
				Output.WriteLine("  <script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-1.10.2.min.js\"></script>");
				Output.WriteLine("  <script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_full.min.js\"></script>");
#endif
			}

			// Include the interface's style sheet if it has one
			Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/" + currentMode.Base_Skin + ".css\" rel=\"stylesheet\" type=\"text/css\" />");
        }

        /// <summary> Perform all the work of adding text directly to the response stream back to the web user </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Html(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_Echo_MainWriter.Write_Html", "Reading the text from the file and echoing back to the output stream");

            try
            {
                FileStream fileStream = new FileStream(fileToEcho, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(fileStream);
                string line = reader.ReadLine();
                while ( line != null )
                {
                    Output.WriteLine(line);
                    line = reader.ReadLine();
                }
                reader.Close();
            }
            catch
            {
                Output.WriteLine("ERROR READING THE SOURCE FILE");
            }
        }
    }
}
