#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Client;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Items;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Core.UI_Configuration;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.Database;
using SobekCM.Library.Database;
using SobekCM.Library.Email;
using SobekCM.Library.ItemViewer;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.UI;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.EAD;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> Item html subwriter renders views on a single digital resource </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. </remarks>
    public class Item_HtmlSubwriter : abstractHtmlSubwriter
    {
        #region Private class members 
        private readonly bool isEadTypeItem;
        private bool itemCheckedOutByOtherUser;
        private readonly bool itemRestrictedFromUserByIp;
        private readonly int searchResultsCount;
        private readonly bool showToc;
        private readonly bool showZoomable;
        private bool tocSelectedComplete;
        private TreeView treeView1;
        private readonly bool userCanEditItem;
        private readonly List<HtmlSubwriter_Behaviors_Enum> behaviors;
        private string buttonsHtml;
        private string pageLinksHtml;

        private BriefItemInfo currentItem;
        private SobekCM_Items_In_Title itemsInTitle;

        #endregion

        #region Constructor(s)

        /// <summary> Constructor for a new instance of the Item_HtmlSubwriter class </summary>
        /// <param name="ShowToc"> Flag indicates whether to show the table of contents open for this item </param>
        /// <param name="Show_Zoomable"> Flag indicates if the zoomable server is available </param>
        /// <param name="Item_Restricted_Message"> Message to be shown because this item is restriced from the current user by IP address </param>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Item_HtmlSubwriter(bool ShowToc, bool Show_Zoomable,
            string Item_Restricted_Message,
            RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {
            showToc = ShowToc;
            showZoomable = Show_Zoomable;
            itemCheckedOutByOtherUser = false;
            userCanEditItem = false;
            searchResultsCount = 0;

            // Try to get the current item
            currentItem = SobekEngineClient.Items.Get_Item_Brief(RequestSpecificValues.Current_Mode.BibID, RequestSpecificValues.Current_Mode.VID, true, RequestSpecificValues.Tracer);
        }

        #endregion


        /// <summary> Gets and sets the page viewer used to display the current item </summary>
        public iItemViewer PageViewer { get; set; }

        /// <summary> Flag indicates this item is currently checked out by another user </summary>
        public bool Item_Checked_Out_By_Other_User 
        {
            set                                  
            {
                //// Override the page viewer at this point
                //if ((value) && (PageViewer.Override_On_Checked_Out))
                //{
                //    PageViewer = new Checked_Out_ItemViewer();
                //}
                itemCheckedOutByOtherUser = value; 

            }
            get { return itemCheckedOutByOtherUser; }
        }

        /// <summary> Gets the collection of special behaviors which this subwriter
        /// requests from the main HTML subwriter. </summary>
        /// <remarks> By default, this returns an empty list </remarks>
        public override List<HtmlSubwriter_Behaviors_Enum> Subwriter_Behaviors
        {
            get
            {
                return behaviors;
            }
        }


        /// <summary> Adds the internal header HTML for this specific HTML writer </summary>
        /// <param name="Output"> Stream to which to write the HTML for the internal header information </param>
        /// <param name="Current_User"> Currently logged on user, to determine specific rights </param>
        public override void Write_Internal_Header_HTML(TextWriter Output, User_Object Current_User)
        {

        }

        //public void Set_Text_Language(Application_State.Language_Enum Language)
        //{
        //    switch (Language)
        //    {
        //        case Application_State.Language_Enum.French:
        //            search = "RECHERCHE";
        //            search_doc = "Recherche Ce Document";
        //            view = "VUE";
        //            full_citation = "Notice"; // "Citation Complètes";
        //            browse_images = "Revue des Images";
        //            view_image = "Revue l'Image";
        //            browse_text = "Revue la Texte";
        //            language = "LANGUE";
        //            english = "Anglais";
        //            french = "Français";
        //            spanish = "Espagñol";
        //            download = "TÉLÉCHARGEMENT";
        //            help = "AIDE";
        //            using_site = "Navigation";
        //            contact = "Assistance";
        //            contents = "TABLE DES MATIÈRES";
        //            break;

        //        case Application_State.Language_Enum.Spanish:
        //            search = "BUSCAR";
        //            search_all = "Busque Todas las Colecciones";
        //            search_this = "Busque Esta Colección";
        //            last_search = "Resultados Anteriores";
        //            search_doc = "Busque Este Documento";
        //            view = "VER";
        //            full_citation = "Cita Completa";
        //            browse_images = "Navegar Imagenes";
        //            view_image = "Ver Imagen";
        //            browse_text = "Navegar Texto";
        //            language = "IDIOMA";
        //            english = "Inglés";
        //            french = "Francés";
        //            spanish = "Español";
        //            download = "TRANSFERENCIA DIRECTA";
        //            help = "AYUDA";
        //            using_site = "Usando este sitio";
        //            contact = "Contacto";
        //            contents = "INDICE";
        //            break;
        //    }
        //}

	    /// <summary> Writes the HTML generated by this item html subwriter directly to the response stream </summary>
	    /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
	    /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
	    /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
	    /// <remarks> This begins writing this page, up to the item-level main menu</remarks>
	    public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
	    {
		    Tracer.Add_Trace("Item_HtmlSubwriter.Write_HTML", "Begin writing the item viewer, up to the item-level main menu");
            return true;
	    }


	    /// <summary> Writes the html to the output stream open the itemNavForm, which appears just before the TocPlaceHolder </summary>
        /// <param name="Output">Stream to directly write to</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Write_ItemNavForm_Opening", "Start the left navigational bar");

        }

        /// <summary> Writes the HTML generated by this item html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        /// <remarks> This continues writing this item from finishing the left navigation bar to the popup forms to the page navigation controls at the top of the item viewer's main area</remarks>
        public override void Write_Additional_HTML(TextWriter Output, Custom_Tracer Tracer)
        {

        }

        /// <summary> Performs the final HTML writing which completes the item table and adds the final page navigation buttons at the bottom of the page </summary>
        /// <param name="Main_PlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public void Add_Main_Viewer_Section(PlaceHolder Main_PlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Add_Main_Viewer_Section", "Rendering HTML ( add any controls which the item viewer needs to add )");

        }

        /// <summary> Writes final HTML to the output stream after all the placeholders and just before the itemNavForm is closed.  </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Write_ItemNavForm_Closing", "Close the item viewer and add final pagination");


        }

        /// <summary> Spot to write any final HTML to the response stream  </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer )
        {
            Tracer.Add_Trace("Item_HtmlSubwriter.Write_Final_Html", "Add reference to draggable jquery ui");
	        if (!behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Full_JQuery_UI))
	        {
                Output.WriteLine("<script type=\"text/javascript\" src=\"" + Static_Resources.Jquery_Ui_1_10_3_Draggable_Js + "\"></script>");
	        }
        }

        #region Methods to create the treeview control for the table of contents

        /// <summary> Populates the tree view with the divisions from the current digital resource item </summary>
        /// <param name="TreeViewArg"> Tree view control to populate </param>
        public void Create_TreeView_From_Divisions(TreeView TreeViewArg )
        {
            tocSelectedComplete = false;

            // Get the current mode page
            List<TreeNode> nodes = new List<TreeNode>();
            List<TreeNode> selectedNodes = new List<TreeNode>();

            int sequence = 0;
            //foreach (abstract_TreeNode absNode in currentItem.Divisions.Physical_Tree.Roots)
            //{
            //    Division_TreeNode divNode = (Division_TreeNode) absNode;
            //    TreeNode treeViewNode = new TreeNode { Text = string.Format("<span class=\"sbkIsw_TocTreeViewItem\" Title=\"{0}\">{1}</span>", divNode.Display_Label, divNode.Display_Short_Label) };
            //    TreeViewArg.Nodes.Add( treeViewNode );
            //    nodes.Add(treeViewNode);
            //    List<TreeNode> pathNodes = new List<TreeNode> {treeViewNode};
            //    recurse_through_tree(divNode, treeViewNode, nodes, selectedNodes, pathNodes, ref sequence );
            //}

            //foreach (TreeNode selectedNode in selectedNodes)
            //{
            //    selectedNode.Text = selectedNode.Text.Replace("sbkIsw_TocTreeViewItem", "sbkIsw_SelectedTocTreeViewItem");
            //    TreeNode selectedNodeExpander = selectedNode;
            //    while (selectedNodeExpander.Parent != null) 
            //    {
            //        (selectedNodeExpander.Parent).Expand();
            //        selectedNodeExpander = selectedNodeExpander.Parent;
            //    }
            //}
        }

        private void recurse_through_tree(Division_TreeNode ParentNode, TreeNode ParentViewNode, List<TreeNode> Nodes, List<TreeNode> SelectedNodes, List<TreeNode> PathNodes, ref int Sequence)
        {
            foreach (abstract_TreeNode absNode in ParentNode.Nodes)
            {
                if (absNode.Page)
                {
                    Sequence++;

                    foreach (TreeNode thisNode in Nodes)
                    {
                        thisNode.Value = Sequence.ToString();
                    }
                    if (Sequence >= RequestSpecificValues.Current_Mode.Page)
                    {
                        if (!tocSelectedComplete)
                        {
                            SelectedNodes.AddRange(PathNodes);
                            tocSelectedComplete = true;
                        }
                        else
                        {
                            if (Sequence == RequestSpecificValues.Current_Mode.Page)
                            {
                                SelectedNodes.AddRange(PathNodes);
                            }
                        }
                    }
                    Nodes.Clear();
                }
                else
                {
                    Division_TreeNode divNode = (Division_TreeNode)absNode;
                    TreeNode treeViewNode = new TreeNode { Text = string.Format("<span class=\"SobekTocTreeViewItem\" Title='{0}'>{1}</span>", divNode.Display_Label, divNode.Display_Short_Label) };
                    ParentViewNode.ChildNodes.Add(treeViewNode);
                    Nodes.Add(treeViewNode);
                    List<TreeNode> pathNodes2 = new List<TreeNode> { treeViewNode };
                    recurse_through_tree(divNode, treeViewNode, Nodes, SelectedNodes, pathNodes2, ref Sequence);
                }
            }
        }

        #endregion

        /// <summary> Gets the collection of body attributes to be included 
        /// within the HTML body tag (usually to add events to the body) </summary>
        public override List<Tuple<string, string>> Body_Attributes
        {
            get
            {
                List<Tuple<string, string>> returnValue = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("onload", "itemwriter_load();"), 
                        new Tuple<string, string>("onresize", "itemwriter_load();"),
						new Tuple<string, string>("id", "itembody")
                    };

                // Add default script attachments

                // Add any viewer specific body attributes
                if (PageViewer != null)
                    PageViewer.Add_ViewerSpecific_Body_Attributes(returnValue);
                return returnValue;
            }
        }

        /// <summary> Title for this web page </summary>
        public override string WebPage_Title
        {
            get
            {
                return currentItem != null ? currentItem.Title : "{0} Item";
            }
        }

        /// <summary> Write any additional values within the HTML Head of the
        /// final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {

        }

		/// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
		/// <value> Always returns an empty string </value>
		public override string Container_CssClass
		{
			get { return String.Empty; }
		}
    }
}
