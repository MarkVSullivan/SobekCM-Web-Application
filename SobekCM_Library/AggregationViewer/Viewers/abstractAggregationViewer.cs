#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Navigation;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.AggregationViewer.Viewers
{
    /// <summary> Abstract class which all collection viewer classes extend </summary>
    /// <remarks> This class implements the <see cref="iAggregationViewer"/> interface.<br /><br />
    /// Collection viewers are used when displaying collection home pages, searches, browses, and information pages.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the Application_State_Builder </li>
    /// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  For a collection-level request, an instance of the  <see cref="Aggregation_HtmlSubwriter"/> class is created. </li>
    /// <li>To display the requested collection view, the collection subwriter will create one or more collection viewers ( implementing this class )</li>
    /// </ul></remarks>
    public abstract class abstractAggregationViewer : iAggregationViewer
    {
        /// <summary> Protected field contains the information specific to the current request </summary>
        protected RequestCache RequestSpecificValues;

        /// <summary> Protected field contains the aggregation-specific request information </summary>
        protected AggregationViewBag ViewBag;

        /// <summary> Empty list of behaviors, returned by default </summary>
        /// <remarks> This just prevents an empty set from having to be created over and over </remarks>
        protected static List<HtmlSubwriter_Behaviors_Enum> emptybehaviors = new List<HtmlSubwriter_Behaviors_Enum>();

        /// <summary> Constructor for objects which implement this abstract class  </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="ViewBag"> Aggregation-specific request information, such as aggregation object and any browse object requested </param>
        protected abstractAggregationViewer(RequestCache RequestSpecificValues, AggregationViewBag ViewBag)
        {
            this.RequestSpecificValues = RequestSpecificValues;
            this.ViewBag = ViewBag;
        }

        #region iAggregationViewer Members

        /// <summary> Reference to the javascript used to initiate the search </summary>
        public string Search_Script_Reference { get; protected set; }

        /// <summary> Reference to the javascript method to be called </summary>
        public string Search_Script_Action { get; protected set; }

        /// <summary> Gets the type of collection view or search supported by this collection viewer </summary>
        public abstract Item_Aggregation_Views_Searches_Enum Type { get; }

        /// <summary> Flag indicates whether the secondary text requires controls </summary>
        /// <value> This defaults to FALSE but is overwritten by most collection viewers </value>
        public virtual bool Secondary_Text_Requires_Controls
        {
            get { return false; }
        }

        /// <summary> Gets the collection of special behaviors which this aggregation viewer
        /// requests from the main HTML subwriter. </summary>
        /// <remarks> By default, this returns an empty list </remarks>
        public virtual List<HtmlSubwriter_Behaviors_Enum> AggregationViewer_Behaviors
        {
            get { return emptybehaviors; }
        }

        /// <summary> Flag which indicates whether the selection panel should be displayed </summary>
        /// <value> This defaults to <see cref="Selection_Panel_Display_Enum.Selectable"/> but is overwritten by most collection viewers </value>
        public virtual Selection_Panel_Display_Enum Selection_Panel_Display
        {
            get { return Selection_Panel_Display_Enum.Selectable; }
        }

        /// <summary> Gets flag which indicates whether this is an internal view, which may have a 
        /// slightly different design feel </summary>
        /// <remarks> This returns FALSE by default, but can be overriden by individual viewer implementations</remarks>
        public virtual bool Is_Internal_View
        {
            get { return false; }
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <remarks> This returns NULL by default, but can be override by individual viewer implementations </remarks>
        public virtual string Viewer_Title
        {
            get { return null;  }
        }

        /// <summary> Gets the URL for the icon related to this aggregational viewer task </summary>
        /// <remarks> This returns NULL by default, but can be override by individual viewer implementations </remarks>
        public virtual string Viewer_Icon
        {
            get { return null;  }
        }


        /// <summary> Add the HTML to be displayed in the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public abstract void Add_Search_Box_HTML(TextWriter Output, Custom_Tracer Tracer);
        
        /// <summary> Add the HTML to be displayed below the search box </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        ///  <remarks> No html is added here, although some children class override this virtual method to add HTML </remarks>
        public virtual void Add_Secondary_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstractAggregationViewer.Add_Secondary_HTML", "No html added");
            }

            // No html to be added here
        }

        /// <summary> Add the HTML and controls to the section below the search box </summary>
        /// <param name="MainPlaceHolder">Place holder to add html and controls to</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> No controls are added here, although some children class override this virtual method to add controls </remarks>
        public virtual void Add_Secondary_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstractAggregationViewer.Add_Secondary_Controls", "No controls added");
            }

            // No controls to be added here
        }

        #endregion

        /// <summary> Writes the simple search tips out to the HTTP text writer </summary>
        /// <param name="Output"> TextWriter writes to the HTTP Response stream </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        protected void Add_Simple_Search_Tips(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstractAggregationViewer.Add_Simple_Search_Tips", "Adding simple search tips");
            }

            // Write the quick tips
            Output.WriteLine("<!-- Add quick tips ( abstractAggregationViewer ) -->");

            switch ( RequestSpecificValues.Current_Mode.Language )
            {
                case Web_Language_Enum.French:
					Output.WriteLine("<div id=\"sbk_QuickTips\">");
                    Output.WriteLine("  <h1>Conseils rapides</h1>");
                    Output.WriteLine("  <ul>");
                    Output.WriteLine("    <li>La Recherche Booléenne");
                    Output.WriteLine("      <p class=\"tagline\"> Utilisez <b>+</b> ou <i><b>et</b></i> des termes et de trouver des documents avec <b>tous</b> les termes.<br />");
                    Output.WriteLine("      Utilisez <b>-</b> ou <i><b>ou</b></i> entre les termes ou pour rechercher des enregistrements avec <b>l'un</b> des termes.<br />");
                    Output.WriteLine("      Utilisez <b>!</b> ou <i><b>et not</b></i> entre les termes à exclure des enregistrements avec des termes.<br />");
                    Output.WriteLine("      Si rien n'est indiqué, <i><b>et</b></i> est la valeur par défaut.<br />");
                    Output.WriteLine("      EXEMPLE: naturelle et non histoire");
                    Output.WriteLine("      </p>");
                    Output.WriteLine("    </li>");
                    Output.WriteLine("    <li>Recherche d'une Expression");
                    Output.WriteLine("      <p class=\"tagline\"> Placer des guillemets autour d'une phrase recherchera la phrase exacte.<br />");
                    Output.WriteLine("      EXEMPLE: &laquo;histoire naturelle&raquo;</p>");
                    Output.WriteLine("    </li>");
                    Output.WriteLine("    <li>Capitalisation");
                    Output.WriteLine("      <p class=\"tagline\"> Les recherches ne sont pas la capitalisation sensible.<br />");
                    Output.WriteLine("      EXEMPLE: La recherche de <i>NATUREL</i> rendra les mêmes résultats que la recherche de naturel <i>naturel</i></p>");
                    Output.WriteLine("    </li>");
                    Output.WriteLine("    <li>Les Signes Diacritiques");
                    Output.WriteLine("      <p class=\"tagline\"> Pour rechercher des mots avec des signes diacritiques, le caractère doit être entré dans la zone de recherche.<br />");
                    Output.WriteLine("      EXEMPLE: La recherche de <i>Précédent</i> est différente de <i>Precedent</i></p>");
                    Output.WriteLine("    </li>");
                    Output.WriteLine("  </ul>");
                    Output.WriteLine("</div>");
                    Output.WriteLine("<br />");
                    Output.WriteLine();
                    break;

                case Web_Language_Enum.Spanish:
					Output.WriteLine("<div id=\"sbk_QuickTips\">");
                    Output.WriteLine("  <h1>Consejos Rápidos:</h1>");
                    Output.WriteLine("  <ul>");
                    Output.WriteLine("    <li>Búsqueda Binaria");
                    Output.WriteLine("      <p class=\"tagline\"> Utilice <b>+</b> o <i><b>y</b></i>entre los términos para encontrar registros con <b>todos</b> los términos.<br />");
                    Output.WriteLine("      Utilice <b>-</b> o <i><b>o</b></i> entre los términos para encontrar registros con <b>cualquiera</b> de los términos.<br />");
                    Output.WriteLine("      Utilice <b>!</b> o <i><b>y no</b></i>  entre los términos para excluir registros con los términos.<br />");
                    Output.WriteLine("      Si no se indica nada , <i><b>y</b></i> es el predeterminado.<br />");
                    Output.WriteLine("      EJEMPLO: natural y no historia ");
                    Output.WriteLine("      </p>");
                    Output.WriteLine("    </li>");
                    Output.WriteLine("    <li>Búsqueda de Frases");
                    Output.WriteLine("      <p class=\"tagline\"> Colocar comillas a una frase buscará la frase exacta.<br />");
                    Output.WriteLine("      EJEMPLO: &quot;historio natural&quot;</p>");
                    Output.WriteLine("    </li>");
                    Output.WriteLine("    <li>Uso de Mayúsculas");
                    Output.WriteLine("      <p class=\"tagline\"> Las búsquedas no distinguen las mayúsculas de las minúsculas.<br />");
                    Output.WriteLine("      EJEMPLO: Buscar <i>NATURAL</i> dará el mismo resultado que buscar <i>natural</i></p>");
                    Output.WriteLine("    </li>");
                    Output.WriteLine("    <li>Tilde Diacrítica");
                    Output.WriteLine("      <p class=\"tagline\"> Para buscar palabras con tilde diacrítica, el símbolo debe ser puesto en el cuadro de búsqueda.<br />");
                    Output.WriteLine("      EJEMPLO: Buscar <i>Précédent</i> es una búsqueda diferente a <i>Precedent</i></p>");
                    Output.WriteLine("    </li>");
                    Output.WriteLine("  </ul>");
                    Output.WriteLine("</div>");
                    Output.WriteLine("  <br />");
                    Output.WriteLine();
                    break;

                default:
					Output.WriteLine("<div id=\"sbk_QuickTips\">");
                    Output.WriteLine("  <h1>Quick Tips</h1>");
                    Output.WriteLine("  <ul>");
                    Output.WriteLine("    <li>Boolean Searching");
                    Output.WriteLine("      <p class=\"tagline\"> Use <b>+</b> or <i><b>and</b></i> between terms to find records with <b>all</b> the terms.<br />");
                    Output.WriteLine("      Use <b>-</b> or <i><b>or</b></i> between terms to find records with <b>any</b> of the terms.<br />");
                    Output.WriteLine("      Use <b>!</b> or <i><b>and not</b></i> between terms to exclude records with terms.<br />");
                    Output.WriteLine("      If nothing is indicated, <b><i>and</i></b> is the default.<br />");
                    Output.WriteLine("      EXAMPLE: natural and not history");
                    Output.WriteLine("      </p>");
                    Output.WriteLine("    </li>");
                    Output.WriteLine("    <li>Phrase Searching");
                    Output.WriteLine("      <p class=\"tagline\"> Placing quotes around a phrase will search for the exact phrase.<br />");
                    Output.WriteLine("      EXAMPLE: &quot;natural history&quot;</p>");
                    Output.WriteLine("    </li>");
                    Output.WriteLine("    <li>Capitalization");
                    Output.WriteLine("      <p class=\"tagline\"> Searches are not capitalization sensitive.<br />");
                    Output.WriteLine("      EXAMPLE: Searching for <i>NATURAL</i> will return the same results as searching for <i>natural</i></p>");
                    Output.WriteLine("    </li>");
                    Output.WriteLine("    <li>Diacritics");
                    Output.WriteLine("      <p class=\"tagline\"> To search for words with diacritics, the character must be entered into the search box.<br />");
                    Output.WriteLine("      EXAMPLE: Searching <i>Précédent</i> is a different search than <i>Precedent</i></p>");
                    Output.WriteLine("    </li>");
                    Output.WriteLine("  </ul>");
                    Output.WriteLine("</div>");
                    Output.WriteLine("  <br />");
                    Output.WriteLine();
                    break;
            }
        }
    }
}
