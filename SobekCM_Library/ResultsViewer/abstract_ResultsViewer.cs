#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Library.Aggregations;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;
using SobekCM.Library.Results;
using SobekCM.Library.Settings;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.ResultsViewer
{
    /// <summary> Abstract class which implements the <see cref="iResultsViewer"/> interface and which all subsequent
    /// results viewer classes must extend </summary>
    public abstract class abstract_ResultsViewer : iResultsViewer
    {
        /// <summary> Number of results included in each page of results  </summary>
        /// <value> This currently always contains the constant value 20.</value>
        protected int Results_Per_Page = 20;

        /// <summary> Protected field contains the list of valid collection codes, including mapping from the Sobek collections to Greenstone collections </summary>
        protected Aggregation_Code_Manager codeManager;

        /// <summary> Protected field contains the current user information for the current request </summary>
        protected User_Object currentUser;

        /// <summary> Protected field contains the current user IP restriction mask, used to determine
        /// if this use has access to IP restricted items </summary>
        protected int current_user_mask;

        private string imageRedirectStem;

        private string textRedirectStem;

        /// <summary> Constructor for a new instance of the abstract_ResultsViewer class  </summary>
        protected abstract_ResultsViewer()
        {
            // Determine the current user mask
            current_user_mask = 0;
            if ((HttpContext.Current != null) && ( HttpContext.Current.Session["IP_Range_Membership"] != null ))
            {
                current_user_mask = (int)HttpContext.Current.Session["IP_Range_Membership"];
            }
        }

        /// <summary> Sets the list of valid collection codes, including mapping from the Sobek collections to Greenstone collections </summary>
        public Aggregation_Code_Manager Code_Manager
        {
            set	{	codeManager = value;		}
        }

        /// <summary> Calculates the index for the last row to be displayed on the current result page </summary>
        public int LastRow
        {
            get	
            {	
                // Determine which rows to display
                return ( CurrentMode.Page * Results_Per_Page );
            }
        }

        /// <summary> Calculates the redirect string used for image-type materials includes the basic redirect url information for the current request, including current skin, aggregation, and other settings </summary>
        protected string Image_Redirect_Stem
        {
            get { return imageRedirectStem ?? (imageRedirectStem = compute_image_redirect_stem()); }
        }

        /// <summary> Calculates redirect string specifically for textual items, which includes the search terms to pass to the final item viewer </summary>
        protected string Text_Redirect_Stem
        {
            get { return textRedirectStem ?? (textRedirectStem = compute_text_redirect_stem()); }
        }

        //private void recurse_through_sorted_tree(Search_Result_Item_TreeNode myRootNode, System.Web.UI.WebControls.TreeNode rootViewNode)
        //{
        //    foreach (Search_Result_Item_TreeNode childNode in myRootNode.ChildNodes)
        //    {
        //        // Create the related tree view node for this
        //        System.Web.UI.WebControls.TreeNode childViewNode = new System.Web.UI.WebControls.TreeNode();
        //        childViewNode.Value = childNode.Value;
        //        childViewNode.SelectAction = System.Web.UI.WebControls.TreeNodeSelectAction.None;
        //        if (childNode.Link.Length > 0)
        //        {
        //            childViewNode.Text = "<a href=\"" + childNode.Link + "\">" + childNode.Name + "</a>";
        //        }
        //        else
        //        {
        //            childViewNode.Text = childNode.Name;
        //        }
        //        rootViewNode.ChildNodes.Add(childViewNode);

        //        // Step through all the children of this node
        //        recurse_through_sorted_tree(childNode, childViewNode);
        //    }
        //}

        #region iResultsViewer Members

        /// <summary> Sets the lookup object used to pull basic information about any item loaded into this library </summary>
        public Item_Lookup_Object All_Items_Lookup { protected get; set; }

        /// <summary> Gets the total number of results to display </summary>
        /// <value> This value can be override by child classes, but by default this returns the number of titles in the result set, unless this is a date sort, in which case the number of issues is returned </value>
        public virtual int Total_Results
        {
            get {
                return CurrentMode.Sort >= 10 ? Results_Statistics.Total_Items : Results_Statistics.Total_Titles;
            }
        }

        /// <summary> Flag indicates if this result view is sortable </summary>
        /// <value>This value can be override by child classes, but by default this TRUE </value>
        public virtual bool Sortable
        {
            get
            {
                return true;
            }
        }

        /// <summary> Adds the controls for this result viewer to the place holder on the main form </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the result viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles and sorted by serial hierarchy </returns>
        public abstract void Add_HTML(PlaceHolder placeHolder, Custom_Tracer Tracer);

        /// <summary> Set the current mode / navigation information for the current request </summary>
        public SobekCM_Navigation_Object CurrentMode { protected get; set; }

        /// <summary> Sets the single page of results for a search or browse, within the entire set </summary>
        public List<iSearch_Title_Result> Paged_Results { protected get; set; }

        /// <summary> Sets the information about the entire set of results for a search or browse</summary>
        public Search_Results_Statistics Results_Statistics { protected get; set; }

        /// <summary> Sets the current item aggregation under which these results are displayed </summary>
        public Item_Aggregation HierarchyObject { protected get; set; }

        /// <summary> Sets the language support object which handles simple translational duties </summary>
        public Language_Support_Info Translator { get; set; }

        #endregion

        /// <summary> Returns the index for the first row to be displayed on the current page, and performs validation
        /// against the calculated last row to ensure correctness</summary>
        /// <param name="CalculatedLastRow"> Calculated last row to be displayed, to be used for validation purposes</param>
        /// <returns> Index of the first result to display on the current page</returns>
        public int StartRow ( int CalculatedLastRow )
        {
            return CalculatedLastRow - Results_Per_Page - 1;
        }

        private string compute_image_redirect_stem()
        {
            string url_options = CurrentMode.URL_Options();
            if (CurrentMode.Coordinates.Length > 0)
            {
                return (url_options.Length > 0) ?  "?coord=" + CurrentMode.Coordinates + "&" + url_options : "?coord=" + CurrentMode.Coordinates;
            }
            
            return (url_options.Length > 0) ? "?" + url_options : String.Empty;
        }

        private string compute_text_redirect_stem()
        {
            // Split the parts
            List<string> terms = new List<string>();
            List<string> fields = new List<string>();

            // Split the terms correctly
            SobekCM_Assistant.Split_Clean_Search_Terms_Fields(CurrentMode.Search_String, CurrentMode.Search_Fields, CurrentMode.Search_Type, terms, fields, SobekCM_Library_Settings.Search_Stop_Words, CurrentMode.Search_Precision, ',');

            // See about a text search string 
            StringBuilder textSearcher = new StringBuilder();

            // Step through each term and field
            bool text_included_in_search = false;
            for (int i = 0; (i < terms.Count) && (i < fields.Count); i++)
            {
                if ((fields[i].Length > 1) && (terms[i].Length > 1))
                {
                    // If this is either for ANYWHERE or for TEXT, include it
                    if (((fields[i].IndexOf("TX") >= 0) || (fields[i].IndexOf("ZZ") >= 0)) && (fields[i][0] != '-'))
                    {
                        if (textSearcher.Length > 0)
                            textSearcher.Append("+=" + terms[i].Replace("\"", "%22"));
                        else
                            textSearcher.Append(terms[i].Replace("\"", "%22"));
                    }

                    // See if this was explicitly a search against full text
                    if (fields[i].IndexOf("TX") >= 0)
                        text_included_in_search = true;
                }
            }

            string url_options = CurrentMode.URL_Options();
            if (CurrentMode.Coordinates.Length > 0)
            {
                return (url_options.Length > 0) ? "?coord=" + CurrentMode.Coordinates + "&" + url_options : "?coord=" + CurrentMode.Coordinates;
            }

            if (textSearcher.Length > 0)
            {
                if ((CurrentMode.Search_Type == Search_Type_Enum.Full_Text) || (text_included_in_search))
                {
                    return (url_options.Length > 0) ? "/search?search=" + textSearcher + "&" + url_options :  "/search?search=" + textSearcher;
                }
                    
                return (url_options.Length > 0) ? "?search=" + textSearcher + "&" + url_options : "?search=" + textSearcher;
            }
            return (url_options.Length > 0) ?  "?" + url_options :  String.Empty;
        }

 
        /// <summary> Builds the tree view control for all the issues related to a single result title and
        /// adds the tree view to the provided place holder </summary>
        /// <param name="placeHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the results are being built for display</param>
        /// <param name="titleRow"> Title row for this title to be displayed, from the dataset of results </param>
        /// <param name="calculatedTextRedirectStem"> Redirect string specifically for textual items, which includes the search terms to pass to the final item viewer </param>
        /// <param name="base_url"> Writer-adjusted base_url ( i.e., may include /l at the end if logged in currently ) </param>
        /// <param name="currentResultRow"> Counter indicates which result number this is within the current page of results </param>
        /// <remarks> This only adds the root node for the tree.  Any further computations and tree-creation are left for the tree node populate event when the user requests any issue information.</remarks>
        protected void Add_Issue_Tree( PlaceHolder placeHolder, iSearch_Title_Result titleRow, int currentResultRow, string calculatedTextRedirectStem, string base_url )
        {
            // Determine term to use
            string single_item_term = "item";
            string multi_item_term = "items";

            string multiple_type_upper = titleRow.MaterialType.ToUpper();
            switch (multiple_type_upper)
            {
                case "NEWSPAPER":
                    single_item_term = "issue";
                    multi_item_term = "issues";
                    break;

                case "AERIAL":
                case "IMAGEAERIAL":
                    single_item_term = "flight line";
                    multi_item_term = "flight lines";
                    break;

                case "IMAGEMAP":
                case "MAP":
                    single_item_term = "map set";
                    multi_item_term = "map sets";
                    break;

                case "PHOTOGRAPH":
                    single_item_term = "photograph set";
                    multi_item_term = "photograph sets";
                    break;

                case "VIDEO":
                    single_item_term = "video";
                    multi_item_term = "videos";
                    break;

                case "AUDIO":
                    single_item_term = "audio";
                    multi_item_term = "audios";
                    break;

                case "ARTIFACT":
                    single_item_term = "artifact";
                    multi_item_term = "artifacts";
                    break;
            }

            // Set the actual term
            string multi_term = multi_item_term;
            if (titleRow.Item_Count <= 1)
                multi_term = single_item_term;

            // Create the root node first
            TreeNode rootNode = new TreeNode
                                    {
                                        SelectAction = TreeNodeSelectAction.Expand,
                                        Value = currentResultRow + "_" + titleRow.BibID,
                                        Text = (titleRow.GroupTitle.Length < 70) ? titleRow.GroupTitle + " ( " + titleRow.Item_Count + " " + multi_term + " )" : titleRow.GroupTitle.Substring(0, 65) + "... ( " + titleRow.Item_Count + " " + multi_term + " )"
                                    };

            // Build the tree view object and tree view nodes now
            TreeView treeView1 = new TreeView {EnableClientScript = true, PopulateNodesFromClient = true};
            treeView1.TreeNodePopulate += treeView1_TreeNodePopulate;
            rootNode.Expanded = false;
            rootNode.PopulateOnDemand = true;

            treeView1.Nodes.Add(rootNode);

            // Add this tree view to the place holder
            placeHolder.Controls.Add(treeView1);
        }

        /// <summary> Event handler loads the nodes on request to the serial hierarchy trees when the user requests them
        /// by expanding a node </summary>
        /// <param name="sender"> TreeView object that fired this event </param>
        /// <param name="e"> Event arguments includes the tree node which was expanded </param>
        void treeView1_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            // Determine the index of this result within the entire page of results
            string resultsIndex = e.Node.Value;
            string node_value = e.Node.Value;
            if (e.Node.Value.IndexOf("_") > 0)
            {
                resultsIndex = e.Node.Value.Substring(0, e.Node.Value.IndexOf("_"));
                node_value = node_value.Substring(resultsIndex.Length + 1);
            }

            // Get the appropriate title result
            iSearch_Title_Result titleResult = Paged_Results[Convert.ToInt32(resultsIndex)];

            // Is this tree built?
            if (titleResult.Item_Tree == null)
            {
                titleResult.Build_Item_Tree(resultsIndex);
            }

            Search_Result_Item_TreeNode retrieved_node = titleResult.Item_Tree.Get_Node_By_Value(node_value);
            if (retrieved_node == null) return;

            string base_url = CurrentMode.Base_URL;
            if (CurrentMode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn)
                base_url = CurrentMode.Base_URL + "l/";

            foreach (Search_Result_Item_TreeNode childNode in retrieved_node.ChildNodes)
            {
                TreeNode childViewNode = new TreeNode
                                             {
                                                 Value = resultsIndex + "_" + childNode.Value,
                                                 SelectAction = TreeNodeSelectAction.None
                                             };

                string name = Translator.Get_Translation(childNode.Name, CurrentMode.Language);
                string tooltip = String.Empty;
                if (name.Length > 100)
                {
                    tooltip = name;
                    name = name.Substring(0, 100) + "...";
                }

                if (childNode.Link.Length > 0)
                {
                    childViewNode.ToolTip = tooltip;
                    childViewNode.Text = "<a href=\"" + base_url + childNode.Link + textRedirectStem + "\">" + name + "</a>";
                }
                else
                {
                    childViewNode.ToolTip = tooltip;
                    childViewNode.Text = name;
                }
                if (childNode.ChildNodes.Count > 0)
                {
                    childViewNode.PopulateOnDemand = true;
                }
                e.Node.ChildNodes.Add(childViewNode);
            }
        }
    }
}
