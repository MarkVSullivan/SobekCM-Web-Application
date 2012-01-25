#region Using directives

using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Library.Navigation;

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

        /// <summary> Perform all the work of adding text directly to the response stream back to the web user </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Add_Text_To_Page(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_Echo_MainWriter.Add_Text_To_Page", "Reading the text from the file and echoing back to the output stream");

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

            Tracer.Add_Trace("Html_Echo_MainWriter.Add_Text_To_Page", "Finished reading and writing the file");

            Output.WriteLine("<br /><br /><b>TRACE ROUTE</b>");
            Output.WriteLine("<br /><br />Total Execution Time: " + Tracer.Milliseconds + " Milliseconds<br /><br />");
            Output.WriteLine(Tracer.Complete_Trace + "<br />");
        }

        /// <summary> Perform all the work of adding to the response stream back to the web user </summary>
        /// <param name="Navigation_Place_Holder"> Place holder is used to add more complex server-side objects during execution</param>
        /// <param name="TOC_Place_Holder"> Place holder is used to add more complex server-side objects during execution</param>
        /// <param name="Main_Place_Holder"> Place holder is used to add more complex server-side objects during execution</param>
        /// <param name="myUfdcUploadPlaceHolder"> Place holder is used to add more complex server-side objects during execution </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Since this class writes all the output directly to the response stream, this method simply returns, without doing anything</remarks>
        public override void Add_Controls(PlaceHolder Navigation_Place_Holder,
            PlaceHolder TOC_Place_Holder,
            PlaceHolder Main_Place_Holder,
            PlaceHolder myUfdcUploadPlaceHolder,
            Custom_Tracer Tracer)
        {
            return;
        }

                /// <summary> Writes the style references and other data to the HEAD portion of the web page </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public void Add_Style_References(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Html_Echo_MainWriter.Add_Style_References", "Adding style references to HTML");

            Output.WriteLine("  <meta name=\"robots\" content=\"index, nofollow\">");
            Output.WriteLine("  <link href=\"" + currentMode.Base_URL + "default/SobekCM.css\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");

            // Include the interface's style sheet if it has one
            Output.WriteLine("  <style type=\"text/css\" media=\"screen\">");
            Output.WriteLine("    @import url( " + currentMode.Base_URL + "design/skins/" + currentMode.Base_Skin + "/" + currentMode.Base_Skin + ".css );");
            Output.WriteLine("  </style>");
        }
    }
}
