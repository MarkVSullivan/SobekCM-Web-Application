using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Net;

namespace SobekCM.Library.ResultsViewer
{
    // Elsevier_Entitlement_Cache uses this class to 
    // (1) create these objects and set up a dictionary keyed by bibidstring to these objects
    // (2) look them up via the bibid string.
    class Elsevier_Article
    {
        // Members -------------------------------------------

        static string base_title_url = "http://www.sciencedirect.com/science/article/pii/";
        public string db_item_link;
        public string bibid = "";
        public string pii = "";
        public string title_url;
        public bool is_open_access = false;

        // Member is_entitled should be updated by an Elsevier Entitlement reponse;
        public bool is_entitled = false;
        
        // Methods --------------------------------------------
        // Always return a new Elsevier_Entitlement instance. It is up to the caller
        // to only call this, say, for bibids that start with "LS".
        public Elsevier_Article(string bibid_string, string db_item_link_string)
        {
            bibid = bibid_string;
            db_item_link = db_item_link_string;
            
            // Parse the pii and is_open_access values from the item_link_string
            // Expected db_item_link_string format: anything/pii_value?[oac=t]
            // That is, the pii value is required/expected to be between the last slash and 
            // (a) the next question-mark or (b) end of string.

            string[] parts = db_item_link_string.Split("/".ToCharArray());
            string[] qparts = new string[] {};
            if (parts.Length > 0)
                // Parse the last part, which may be a pii value alone or a pii value followed 
                // by an openaccess value.
                qparts = parts[parts.Length - 1].Split("?".ToCharArray());

                // For pii, remove the officially condoned fluff characters for PII values.
                pii = qparts[0].Replace("(", "").Replace(")", "").Replace("-", "");
                if (qparts.Length > 1)
                {
                    // oac=t is the only specific supported setting now. It is optional and means
                    // this is an OpenAccess article/item.
                    is_open_access = (qparts[1] == "oac=t");
                }
                title_url = base_title_url + pii;
        } // end method Elsevier_Article(string, string)
    }
    
    /// <summary>
    /// User instantiates an Elsevier_Entitlements_Cache with a csv list of string pii values.
    /// User may access the d_pii_entitlement member to test whether a pii string key is in the dictionary, and
    /// If so, whether it is a pii that a request from the same ip that created the EntitlementCache
    /// would be entitled to view the Elsevier Science Direct article identified by the pii id value.
    /// </summary>
    class Elsevier_Entitlements_Cache
    {
        // SECURITY ISSUE: Remove this line and get api_key from settings table before merging into OpenSource SobekCM!
        private string api_key = "d91051fb976425e3b5f00750cbd33d8b";
        public string bib_prefix = "";
        public string pii_csv_string = "";
        // d_pii_entitlement - value is a bool
        public Dictionary<string, bool> d_pii_entitlement = new Dictionary<string, bool>();
        //d_bib_article - value is object elsevier_entitlement
        public Dictionary<string, Elsevier_Article> d_bib_article = new Dictionary<string, Elsevier_Article>();
        // d_pii_article -
        public Dictionary<string, Elsevier_Article> d_pii_article = new Dictionary<string, Elsevier_Article>();

        //public Dictionary<string, Dictionary<string,string>> d_bibid_dict = new Dictionary<string, <Dictionary<string,string>>();
        public List<string> piis_entitled = new List<string>();

        // Add an Elsevier item's entitlement identification and open access info,
        // but only if the bibid starts with the bibid prefix.

        // Prepare the Entitlments Cache
        public Elsevier_Entitlements_Cache(string bib_prefix_string, string api_key_string)
        {
            bib_prefix = bib_prefix_string;
            if (!String.IsNullOrEmpty(api_key_string))
            {
                api_key = api_key_string;
            }
        }
        // Add and return an entitlement object to this cache.
        public Elsevier_Article Add_Article(string bibid_string, string link_string)
        {
            // Do not add an entitlement for a bibid_string that does not start with bib_prefix
            if (bibid_string.IndexOf(bib_prefix) != 0)
            {
                return null;
            }
            // TODO: not add an entitlement for a bibid_string that is already stored in this cache.
            // add code here if needed, but should not happen from SobekCM calling code.

            // Add this entitlement to d_bib_entitlement
            Elsevier_Article article = new Elsevier_Article(bibid_string, link_string);
            d_bib_article[bibid_string] = article;
            d_pii_article[article.pii] = article;

            pii_csv_string += String.IsNullOrEmpty(pii_csv_string)? article.pii : ',' + article.pii ;
            return article;
        } // end Add_Article()

        // Using the member string of csv pii values, get entitlement from Elsevier API 
        // and update the member dictionary where key is pii and value is entitlement. 
        public void update_from_entitlements_api()
        {
            if (pii_csv_string.Length == 0)
            {
                return;
            }
            try
            {
                //Create the entitlement request
                string entitlementRequest = CreateRequest(pii_csv_string);

                //Console.WriteLine(entitlementRequest);
                XPathDocument xpdResponse = MakeRequest(entitlementRequest);
                update_dict_pii_entitlement(xpdResponse);
            }
            catch (Exception)
            {
                //pass
            }
        } // end method

        // Given a string of csv pii values, get entitlement from Elsevier API 
        // and update the member dictionary where key is pii and value is entitlement. 
        public void cache_pii_csv_string(string pii_csv_string)
        {
            if (pii_csv_string.Length == 0)
            {
                return;
            }
            try
            {
                //Create the entitlement request
                string entitlementRequest = CreateRequest(pii_csv_string);

                //Console.WriteLine(entitlementRequest);
                XPathDocument xpdResponse = MakeRequest(entitlementRequest);
                update_dict_pii_entitlement(xpdResponse);
            }
            catch (Exception)
            {
                //pass
            }
        } // end method

        // Return the Elsevier Entitlement request URL given a csv querystring of Elsevier 
        // PII identifier codes.
        // The codes may optionally contain the 'official fluff characters' allowed in PII codes.
        private string CreateRequest(string queryString)
        {
            string UrlRequest = "http://api.elsevier.com/content/article/entitlement/pii/" +
                                            queryString + "?httpAccept=text/xml&apiKey=" + api_key;
            // Parse the list of individual pii values in the query
            string[] parts = queryString.Split(",".ToCharArray());
            // Clear the pii dictionary to prepare for a new one.
            d_pii_entitlement.Clear();
            // Set up new dictionary of items of pii to boolean entitlement value
            for (int i = 0; i < parts.Length; i++)
            {
                string pii = parts[i];
                // Clean pii fluff characters before inserting key into dictionary
                string pii_norm = pii.Replace("(", "").Replace(")", "").Replace("-", "");
                // Add pii value to dictionary with initial false value. Let API prove it is true.
                if (!d_pii_entitlement.ContainsKey(pii_norm))
                {
                    d_pii_entitlement.Add(pii_norm, false);
                }
            }
            return (UrlRequest);
        } // end CreateRequest()

        // Submit the given HTTP Request and return the xmlDoc of the response or null
        private XPathDocument MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                XPathDocument xpDoc = new XPathDocument(response.GetResponseStream());
                return (xpDoc);
            }
            catch (Exception)
            {
                // Just return
                return null;
            }
        }// end MakeRequest

        // Given an XPathDocument of an Elsevier entitlementResponse, derive and update 
        // (1) the dictionary of items with key (string) pii and value (boolean) entitlement, and
        // (2) simple list of pii values that the requestor is entitled to view for free.
        // and .... resume d_bib_article
        private void update_dict_pii_entitlement(XPathDocument entitlementResponse)
        {
            XPathNavigator xpnav = entitlementResponse.CreateNavigator();
            XPathNodeIterator i_doc = xpnav.Select("//document-entitlement");
            while (i_doc.MoveNext())
            {
                XPathNavigator n_pii = i_doc.Current.SelectSingleNode(".//pii");
                XPathNavigator n_entitled = i_doc.Current.SelectSingleNode(".//entitled");

                // Remove officially allowed PII fluff characters.
                string pii = n_pii.Value.Replace("(", "").Replace(")", "").Replace("-", "");
                if (pii.Length > 0)
                {
                    d_pii_entitlement[pii] = n_entitled.Value == "true" ? true : false;
                    Elsevier_Article article;
                    if (d_pii_article.TryGetValue(pii, out article))
                    {
                        article.is_entitled = n_entitled.Value == "true" ? true : false;
                    }
                    if (n_entitled.Value == "true")
                    {
                        piis_entitled.Add(pii);
                    }                   
                }
            } // end while i_doc.MoveNext()
        } // end method update_dict_pii_entitlement()
    }// end class ElsevierEntitlementCache
}
