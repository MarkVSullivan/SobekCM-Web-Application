#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.Navigation;
using SobekCM.Core.Results;
using SobekCM.Library.UI;
using SobekCM.Tools;

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

        /// <summary> Protected field contains the current user IP restriction mask, used to determine
        /// if this use has access to IP restricted items </summary>
        protected int CurrentUserMask;

        private string imageRedirectStem;

        private string textRedirectStem;

        /// <summary> Values specific to the current HTML request </summary>
        protected RequestCache RequestSpecificValues;

        /// <summary> Constructor for a new instance of the abstract_ResultsViewer class  </summary>
        protected abstract_ResultsViewer( RequestCache RequestSpecificValues )
        {
            this.RequestSpecificValues = RequestSpecificValues;

            // Determine the current user mask
            CurrentUserMask = 0;
            if ((HttpContext.Current != null) && ( HttpContext.Current.Session["IP_Range_Membership"] != null ))
            {
                CurrentUserMask = (int)HttpContext.Current.Session["IP_Range_Membership"];
            }
        }

        /// <summary> Calculates the index for the last row to be displayed on the current result page </summary>
        public int LastRow
        {
            get
            {
                // Determine which rows to display
                if (RequestSpecificValues.Current_Mode.Page.HasValue)
                    return (RequestSpecificValues.Current_Mode.Page.Value * Results_Per_Page);
                
                return Results_Per_Page;
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

        /// <summary> Gets the total number of results to display </summary>
        /// <value> This value can be override by child classes, but by default this returns the number of titles in the result set, unless this is a date sort, in which case the number of issues is returned </value>
        public virtual int Total_Results
        {
            get {
                return RequestSpecificValues.Current_Mode.Sort >= 10 ? RequestSpecificValues.Results_Statistics.Total_Items : RequestSpecificValues.Results_Statistics.Total_Titles;
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
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the the bulk of the result viewer's output is displayed</param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Sorted tree with the results in hierarchical structure with volumes and issues under the titles and sorted by serial hierarchy </returns>
        public abstract void Add_HTML(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer);

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
            string url_options = UrlWriterHelper.URL_Options(RequestSpecificValues.Current_Mode);
            if ( !String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Coordinates))
            {
                return (url_options.Length > 0) ? "?coord=" + RequestSpecificValues.Current_Mode.Coordinates + "&" + url_options : "?coord=" + RequestSpecificValues.Current_Mode.Coordinates;
            }
            
            return (url_options.Length > 0) ? "?" + url_options : String.Empty;
        }

        private string compute_text_redirect_stem()
        {
            // Split the parts
            List<string> terms = new List<string>();
            List<string> fields = new List<string>();

            // Split the terms correctly
            SobekCM_Assistant.Split_Clean_Search_Terms_Fields(RequestSpecificValues.Current_Mode.Search_String, RequestSpecificValues.Current_Mode.Search_Fields, RequestSpecificValues.Current_Mode.Search_Type, terms, fields, UI_ApplicationCache_Gateway.Search_Stop_Words, RequestSpecificValues.Current_Mode.Search_Precision, ',');

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

            string url_options = UrlWriterHelper.URL_Options(RequestSpecificValues.Current_Mode);
            if (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.Coordinates))
            {
                return (url_options.Length > 0) ? "?coord=" + RequestSpecificValues.Current_Mode.Coordinates + "&" + url_options : "?coord=" + RequestSpecificValues.Current_Mode.Coordinates;
            }

            if (textSearcher.Length > 0)
            {
                if ((RequestSpecificValues.Current_Mode.Search_Type == Search_Type_Enum.Full_Text) || (text_included_in_search))
                {
                    return (url_options.Length > 0) ? "/search?search=" + textSearcher + "&" + url_options :  "/search?search=" + textSearcher;
                }
                    
                return (url_options.Length > 0) ? "?search=" + textSearcher + "&" + url_options : "?search=" + textSearcher;
            }
            return (url_options.Length > 0) ?  "?" + url_options :  String.Empty;
        }

 
        /// <summary> Builds the tree view control for all the issues related to a single result title and
        /// adds the tree view to the provided place holder </summary>
        /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form into which the results are being built for display</param>
        /// <param name="TitleRow"> Title row for this title to be displayed, from the dataset of results </param>
        /// <param name="CalculatedTextRedirectStem"> Redirect string specifically for textual items, which includes the search terms to pass to the final item viewer </param>
        /// <param name="BaseURL"> Writer-adjusted base_url ( i.e., may include /l at the end if logged in currently ) </param>
        /// <param name="CurrentResultRow"> Counter indicates which result number this is within the current page of results </param>
        /// <remarks> This only adds the root node for the tree.  Any further computations and tree-creation are left for the tree node populate event when the user requests any issue information.</remarks>
        protected void Add_Issue_Tree( PlaceHolder MainPlaceHolder, iSearch_Title_Result TitleRow, int CurrentResultRow, string CalculatedTextRedirectStem, string BaseURL )
        {
            // Determine term to use
            string single_item_term = "item";
            string multi_item_term = "items";

            string multiple_type_upper = TitleRow.MaterialType.ToUpper();
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
            if (TitleRow.Item_Count <= 1)
                multi_term = single_item_term;

            // Create the root node first
            TreeNode rootNode = new TreeNode
                                    {
                                        SelectAction = TreeNodeSelectAction.Expand,
                                        Value = CurrentResultRow + "_" + TitleRow.BibID,
                                        Text = (TitleRow.GroupTitle.Length < 70) ? TitleRow.GroupTitle + " ( " + TitleRow.Item_Count + " " + multi_term + " )" : TitleRow.GroupTitle.Substring(0, 65) + "... ( " + TitleRow.Item_Count + " " + multi_term + " )"
                                    };

            // Build the tree view object and tree view nodes now
            TreeView treeView1 = new TreeView {EnableClientScript = true, PopulateNodesFromClient = true};
            treeView1.TreeNodePopulate += treeView1_TreeNodePopulate;
            rootNode.Expanded = false;
            rootNode.PopulateOnDemand = true;

            treeView1.Nodes.Add(rootNode);

            // Add this tree view to the place holder
            MainPlaceHolder.Controls.Add(treeView1);
        }

        /// <summary> Event handler loads the nodes on request to the serial hierarchy trees when the user requests them
        /// by expanding a node </summary>
        /// <param name="Sender"> TreeView object that fired this event </param>
        /// <param name="E"> Event arguments includes the tree node which was expanded </param>
        void treeView1_TreeNodePopulate(object Sender, TreeNodeEventArgs E)
        {
            // Determine the index of this result within the entire page of results
            string resultsIndex = E.Node.Value;
            string node_value = E.Node.Value;
            if (E.Node.Value.IndexOf("_") > 0)
            {
                resultsIndex = E.Node.Value.Substring(0, E.Node.Value.IndexOf("_"));
                node_value = node_value.Substring(resultsIndex.Length + 1);
            }

            // Get the appropriate title result
            iSearch_Title_Result titleResult = RequestSpecificValues.Paged_Results[Convert.ToInt32(resultsIndex)];

            // Is this tree built?
            if (titleResult.Item_Tree == null)
            {
                titleResult.Build_Item_Tree(resultsIndex);
            }

            Search_Result_Item_TreeNode retrieved_node = titleResult.Item_Tree.Get_Node_By_Value(node_value);
            if (retrieved_node == null) return;

            string base_url = RequestSpecificValues.Current_Mode.Base_URL;
            if (RequestSpecificValues.Current_Mode.Writer_Type == Writer_Type_Enum.HTML_LoggedIn)
                base_url = RequestSpecificValues.Current_Mode.Base_URL + "l/";

            foreach (Search_Result_Item_TreeNode childNode in retrieved_node.ChildNodes)
            {
                TreeNode childViewNode = new TreeNode
                                             {
                                                 Value = resultsIndex + "_" + childNode.Value,
                                                 SelectAction = TreeNodeSelectAction.None
                                             };

                string name = UI_ApplicationCache_Gateway.Translation.Get_Translation(childNode.Name, RequestSpecificValues.Current_Mode.Language);
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
                E.Node.ChildNodes.Add(childViewNode);
            }
        }
    }
}
