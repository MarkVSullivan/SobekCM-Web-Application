using System;
using System.IO;
using System.Web;
using SobekCM.Library.UI;

namespace SobekCM.Library.Helpers.AceEditor
{
    /// <summary> Enumeration for the mode for the Ace Editor </summary>
    public enum AceEditor_Mode : byte
    {
        /// <summary> The mode for this Ace editor has not been defined </summary>
        UNDEFINED,

        /// <summary> This Ace editor will be used for editing CSS stylesheets </summary>
        CSS,
        
        /// <summary> THis Ace editor will be used for editing HTML </summary>
        /// <remarks> This is currently not supported (as of 12/2016), as CKEditor is used for HTML editing </remarks>
        HTML,

        /// <summary> This Ace editor will be used for editing Javascript code </summary>
        Javascript,
        
        /// <summary> This Ace editor will be used for editing XML code </summary>
        XML
    }


    /// <summary> Class is used to write the HTML to allow users to edit content using
    /// the Ace editor library </summary>
    /// <remarks> This is primarily utilized for CSS and Javascript editing currently </remarks>
    public class AceEditor
    {
        /// <summary> Current mode for this editor ( i.e., CSS, Javascript, or HTML editing ) </summary>
        public AceEditor_Mode Mode { get; set; }

        /// <summary> HTML ID used to pull this data back when the page is posted back to the server </summary>
        public string ContentsId { get; set; }

        /// <summary> HTML ID for the editor itself, used to add styling around the editor </summary>
        public string EditorId { get; set; }

        /// <summary> Current base url, used to point to the ACE javascript </summary>
        public string BaseUrl { get; set; }

        /// <summary> Constructor for a new instance of the AceEditor class </summary>
        public AceEditor()
        {
            Mode = AceEditor_Mode.UNDEFINED;
        }

        /// <summary> Constructor for a new instance of the AceEditor class </summary>
        /// <param name="Mode"> Current mode for this editor ( i.e., CSS, Javascript, or HTML editing ) </param>
        public AceEditor(AceEditor_Mode Mode)
        {
            this.Mode = Mode;
        }

        /// <summary> Add the Ace editor and all the necessary html/javascript to enable an Ace editor here on the page </summary>
        /// <param name="Output"> Writer to write to the stream </param>
        /// <param name="Contents"></param>
        /// added to the output stream here (might not if there are multiple AceEditor areas on the same page )</param>
        public void Add_To_Stream(TextWriter Output, string Contents)
        {
            Add_To_Stream(Output, Contents, true );
        }

        /// <summary> Add the Ace editor and all the necessary html/javascript to enable an Ace editor here on the page </summary>
        /// <param name="Output"> Writer to write to the stream </param>
        /// <param name="Contents"></param>
        /// <param name="Include_Script_Reference"> Flag indicates if the Ace script reference should be
        /// added to the output stream here (might not if there are multiple AceEditor areas on the same page )</param>
        public void Add_To_Stream(TextWriter Output, string Contents, bool Include_Script_Reference)
        {
            // If no ContentsID was set, then what is the point of this?
            if (String.IsNullOrEmpty(ContentsId))
            {
                Output.WriteLine("ERROR: ContentsId not set in the Ace Editor object.");
                return;
            }

            // Was an editor id assigned?  If not, create a random one (uses may choose to not apply styling here)
            if (String.IsNullOrEmpty(EditorId))
            {
                EditorId = "AceEditor_" + ContentsId.Replace(" ", "").Replace("_", "");
            }

            Output.WriteLine();
            Output.WriteLine("<!-- ACE Editor HTML -->");
            Output.WriteLine("<textarea style=\"visibility:hidden;position:absolute;\" id=\"" + ContentsId + "\" name=\"" + ContentsId + "\" ></textarea>");
            Output.WriteLine("<div id=\"" + EditorId + "Div\">");
            Output.WriteLine("<pre id=\"" + EditorId + "\">");
            Output.WriteLine(HttpUtility.HtmlEncode(Contents));
            Output.WriteLine("</pre>  ");
            Output.WriteLine("</div>");
            Output.WriteLine();


            Output.WriteLine("<!-- ACE Editor Scripts -->");
            if (Include_Script_Reference)
            {
                if (!String.IsNullOrEmpty(BaseUrl))
                {
                    Output.WriteLine("<script src=\"" + BaseUrl + "default/ace/1.2.5/ace.js\" type=\"text/javascript\" charset=\"utf-8\"></script>  ");
                }
                else
                {
                    Output.WriteLine("<script src=\"" + UI_ApplicationCache_Gateway.Settings.Servers.Application_Server_URL + "default/ace/1.2.5/ace.js\" type=\"text/javascript\" charset=\"utf-8\"></script>  ");
                }
            }

            // Determine the javascript name for the editor in javascript to use
            // This should allow multiple Ace editors on a page (in theory)
            string js_editor_id = EditorId.ToLower();

            Output.WriteLine("<script>  ");
            Output.WriteLine("    var " + js_editor_id + " = ace.edit(\"" + EditorId + "\");  ");

            // Get the theme
            string theme = UI_ApplicationCache_Gateway.Settings.UI.Ace_Editor_Theme;
            if (String.IsNullOrEmpty(theme))
                theme = "chrome";

            Output.WriteLine("    " + js_editor_id + ".setTheme(\"ace/theme/" + theme + "\");  ");

            // Add the mode information
            switch (Mode)
            {
                case AceEditor_Mode.CSS:
                    Output.WriteLine("    " + js_editor_id + ".session.setMode(\"ace/mode/css\");  ");
                    break;

                case AceEditor_Mode.HTML:
                    Output.WriteLine("    " + js_editor_id + ".session.setMode(\"ace/mode/html\");  ");
                    break;

                case AceEditor_Mode.Javascript:
                    Output.WriteLine("    " + js_editor_id + ".session.setMode(\"ace/mode/javascript\");  ");
                    break;

                case AceEditor_Mode.XML:
                    Output.WriteLine("    " + js_editor_id + ".session.setMode(\"ace/mode/xml\");  ");
                    break;
            }

            Output.WriteLine("    " + js_editor_id + ".setOptions({ enableBasicAutocompletion: true, enableSnippets: true, enableLiveAutocompletion: false });");
            Output.WriteLine("    $('textarea[name=\"" + ContentsId + "\"]').val(" + js_editor_id + ".getSession().getValue());");
            Output.WriteLine("    " + js_editor_id + ".getSession().on('change', function () { ");
            Output.WriteLine("             var editorVal = " + js_editor_id + ".getSession().getValue();");
            Output.WriteLine("             $('textarea[name=\"" + ContentsId + "\"]').val(editorVal); });");
            Output.WriteLine("</script>  ");
            Output.WriteLine();
        }
    }

    
}
