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
    /// <summary>
    /// User instantiates an Elsevier_Entitlements_Cache with a csv list of string pii values.
    /// User may access the d_pii_entitlement member to test whether a pii string key is in the dictionary, and
    /// If so, whether it is a pii that a request from the same ip that created the EntitlementCache
    /// would be entitled to view the Elsevier Science Direct article identified by the pii id value.
    /// </summary>
    class Elsevier_Entitlements_Cache
    {
        // SECURITY ISSUE: Remove this line and get api_key from settings table before merging into OpenSource SobekCM!
        static string api_key = "d91051fb976425e3b5f00750cbd33d8b";
        public Dictionary<string, bool> d_pii_entitlement = new Dictionary<string, bool>();
        //public Dictionary<string, Dictionary<string,string>> d_bibid_dict = new Dictionary<string, <Dictionary<string,string>>();
        public List<string> piis_entitled = new List<string>();

        public Elsevier_Entitlements_Cache() 
        {
            // Let it be.
        }

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
        } // end Elsevier_Entitlements_Cache()

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
                // Can add code here to write to a log...
                return null;
            }
        }// end MakeRequest

        // Given an XPathDocument of an Elsevier entitlementResponse, derive and update the dictionary of 
        // items with key (string) pii and value (boolean) entitlement.
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
                    if (n_entitled.Value == "true")
                    {
                        piis_entitled.Add(pii);
                    }                   
                }
            } // end while i_doc.MoveNext()
        } // end update_dict_pii_entitlement()
    }// end class ElsevierEntitlementCache
}
