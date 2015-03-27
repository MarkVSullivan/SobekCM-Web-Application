#region Using directives

using System.IO;
using SobekCM.Core.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Tools;

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
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="HTML_File_To_Echo"> The HTML file to echo </param>
        public Html_Echo_MainWriter(RequestCache RequestSpecificValues, string HTML_File_To_Echo) : base(RequestSpecificValues)
        {
            fileToEcho = HTML_File_To_Echo;
        }

        /// <summary> Gets the enumeration of the type of main writer </summary>
        /// <value> This property always returns the enumerational value <see cref="SobekCM.UI_Library.Navigation.Writer_Type_Enum.HTML"/>. </value>
        public override Writer_Type_Enum Writer_Type { get { return Writer_Type_Enum.HTML_Echo; } }


        /// <summary> Writes the style references and other data to the HEAD portion of the web page </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_Echo_MainWriter.Write_Within_HTML_Head", "Adding style references to HTML");

            Output.WriteLine("  <meta name=\"robots\" content=\"index, follow\">");

			// Write the style sheet to use 
            Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");

			// Write the main SobekCM item style sheet to use 
			Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Item_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");

			// Always add jQuery library (changed as of 7/8/2013)
			if ((RequestSpecificValues.Current_Mode.Mode != Display_Mode_Enum.Item_Display) || (RequestSpecificValues.Current_Mode.ViewerCode != "pageturner"))
			{
                Output.WriteLine("  <script type=\"text/javascript\" src=\"" + Static_Resources.Jquery_1_10_2_Js + "\"></script>");
				Output.WriteLine("  <script type=\"text/javascript\" src=\"" + Static_Resources.Sobekcm_Full_Js + "\"></script>");
			}

			// Include the interface's style sheet if it has one
			Output.WriteLine("  <link href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "design/skins/" + RequestSpecificValues.Current_Mode.Base_Skin + "/" + RequestSpecificValues.Current_Mode.Base_Skin + ".css\" rel=\"stylesheet\" type=\"text/css\" />");
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
