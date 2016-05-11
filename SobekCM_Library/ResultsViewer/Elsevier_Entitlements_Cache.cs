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
        public bool is_obsolete = false;

        // Member is_entitled should be updated by an Elsevier Entitlement reponse;
        public bool is_entitled = false;
        
        // Methods --------------------------------------------
        // Always return a new Elsevier_Entitlement instance. It is up to the caller
        // to only call this, say, for bibids that start with "LS".
        public Elsevier_Article(string bibid_string, string db_item_link_string)
        {
            bibid = bibid_string;
            db_item_link = db_item_link_string;
            // workaround for builder failure to delete obsolete records.
            if (bibid_string.IndexOf("LS005") != 0)
            {
                is_obsolete = true;
            }
            else
            {
                is_obsolete = false;
            }
            
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
        #region svg_access_symbols
        public const string svg_access_symbols = @"
<svg width='0' height='0' ><defs>
<symbol viewbox='00 0 700 1000' id='access-open' >
  <rect width='640' height='1000' fill='white'/>
  <g stroke='#f68212' stroke-width='104.764' fill='none'>
    <path d='M111.387,308.135V272.408A209.21,209.214 0 0,1 529.807,272.408V530.834'/>
    <circle cx='320.004' cy='680.729' r='256.083'/>
  </g>
  <circle fill='#f68212' cx='321.01' cy='681.659' r='86.4287'/>
</symbol>

<symbol id='access-check' >
	<path fill='#E87224' d='M27.817,19.998c-0.256,0-1.137,0-1.665-0.08v4.146h-1.265V13.531l0.593-0.032
		c0.832-0.064,2.337-0.064,2.705-0.064c1.776,0,3.281,1.201,3.281,3.217C31.467,18.637,29.626,19.998,27.817,19.998z M27.753,14.491
		l-1.601,0.016v4.434h1.185c1.425,0,2.689-0.512,2.689-2.289C30.026,15.164,29.226,14.491,27.753,14.491z'/>
	<path fill='#E87224' d='M38.153,24.063l-0.191-1.057c-0.688,0.945-1.281,1.265-2.449,1.265c-1.297,0-2.32-0.448-2.32-1.905v-5.554
		h1.232v4.69c0,0.896,0.08,1.697,1.2,1.697c0.448,0,0.88-0.096,1.28-0.288c0.416-0.352,0.881-0.977,1.009-1.281v-4.818h1.232v5.25
		c0,0.656,0.032,1.345,0.128,2.001H38.153z'/>
	<path fill='#E87224' d='M44.376,24.271c-0.656,0-1.504-0.08-2.48-0.272V12.378h1.232v4.69c0.592-0.24,1.328-0.448,2.241-0.448
		c1.648,0,2.833,0.976,2.833,3.666C48.202,22.718,46.777,24.271,44.376,24.271z M45.146,17.725c-0.593,0-0.945,0.016-1.025,0.048
		c-0.224,0.064-0.656,0.208-0.992,0.384v4.946c0.496,0.096,0.849,0.096,1.185,0.096c1.601,0,2.545-0.977,2.545-2.817
		C46.857,18.829,46.441,17.725,45.146,17.725z'/>
	<path fill='#E87224' d='M50.183,24.063V12.378h1.232v11.685H50.183z'/>
	<path fill='#E87224' d='M54.712,15.067c-0.416,0-0.784-0.448-0.784-0.913c0-0.448,0.368-0.864,0.784-0.864
		c0.448,0,0.832,0.417,0.832,0.864C55.544,14.603,55.16,15.067,54.712,15.067z M54.12,24.063v-7.251h1.232v7.251H54.12z'/>
	<path fill='#E87224' d='M59.735,24.271c-0.88,0-1.744-0.208-2.305-0.592l0.145-1.152c0.752,0.48,1.152,0.736,2.129,0.736
		c0.96,0,1.553-0.464,1.553-1.072c0-1.361-3.857-1.393-3.857-3.554c0-1.136,0.88-2.017,2.688-2.017c1.024,0,1.776,0.272,2.306,0.544
		l-0.129,1.153c-0.704-0.577-1.536-0.705-2.272-0.705c-0.656,0-1.36,0.256-1.36,0.944c0,1.329,3.857,1.345,3.857,3.554
		C62.489,23.215,61.624,24.271,59.735,24.271z'/>
	<path fill='#E87224' d='M70.409,24.063h-1.232c0.049-1.633,0.049-2.065,0.049-3.137c0-0.336,0-0.96-0.033-1.537
		c-0.063-1.104-0.144-1.665-1.216-1.665c-0.433,0-0.993,0.128-1.104,0.192c-0.176,0.096-0.848,0.8-1.185,1.281v4.866h-1.232V12.378
		h1.232v5.458c0.608-0.72,1.28-1.216,2.401-1.216c1.28,0,2.192,0.416,2.305,1.905c0.048,0.72,0.048,1.44,0.048,2.161
		C70.441,21.838,70.441,22.415,70.409,24.063z'/>
	<path fill='#E87224' d='M73.735,20.045c-0.032,0.176-0.032,0.208-0.032,0.4c0,1.761,0.864,2.753,2.369,2.753
		c0.864,0,1.841-0.368,2.272-0.752l0.097,1.121c-0.641,0.4-1.377,0.704-2.753,0.704c-1.825,0-3.266-1.393-3.266-3.81
		c0-2.481,1.377-3.841,3.233-3.841c2.161,0,2.913,1.136,2.913,3.425H73.735z M75.576,17.66c-0.337,0-0.88,0.145-0.96,0.224
		c-0.097,0.08-0.545,0.48-0.721,1.201h3.553C77.385,17.805,76.521,17.66,75.576,17.66z'/>
	<path fill='#E87224' d='M83.816,18.252c0-0.096-0.032-0.512-0.514-0.512c-0.689,0-1.199,1.121-1.455,1.713v4.61h-1.234v-5.266
		c0-0.769-0.047-1.265-0.126-1.985h1.137l0.207,1.152c0.239-0.624,0.864-1.329,1.647-1.329c0.674,0,1.058,0.368,1.138,1.057
		c0.017,0.144,0.049,0.288,0.049,0.432L83.816,18.252z'/>
	<path fill='#E87224' d='M92.583,24.271h-0.959l-2.882-7.459h1.313l2.112,5.586l1.89-5.586h1.217L92.583,24.271z'/>
	<path fill='#E87224' d='M97.335,20.045c-0.032,0.176-0.032,0.208-0.032,0.4c0,1.761,0.864,2.753,2.369,2.753
		c0.864,0,1.842-0.368,2.271-0.752l0.098,1.121c-0.641,0.4-1.377,0.704-2.753,0.704c-1.825,0-3.267-1.393-3.267-3.81
		c0-2.481,1.377-3.841,3.232-3.841c2.161,0,2.913,1.136,2.913,3.425H97.335z M99.176,17.66c-0.337,0-0.88,0.145-0.96,0.224
		c-0.097,0.08-0.545,0.48-0.721,1.201h3.553C100.984,17.805,100.12,17.66,99.176,17.66z'/>
	<path fill='#E87224' d='M107.416,18.252c0-0.096-0.032-0.512-0.514-0.512c-0.688,0-1.199,1.121-1.455,1.713v4.61h-1.233v-5.266
		c0-0.769-0.047-1.265-0.127-1.985h1.138l0.207,1.152c0.239-0.624,0.864-1.329,1.647-1.329c0.673,0,1.058,0.368,1.137,1.057
		c0.017,0.144,0.05,0.288,0.05,0.432L107.416,18.252z'/>
	<path fill='#E87224' d='M111.255,24.271c-0.881,0-1.743-0.208-2.305-0.592l0.146-1.152c0.752,0.48,1.151,0.736,2.129,0.736
		c0.959,0,1.551-0.464,1.551-1.072c0-1.361-3.855-1.393-3.855-3.554c0-1.136,0.88-2.017,2.688-2.017
		c1.024,0,1.776,0.272,2.307,0.544l-0.128,1.153c-0.705-0.577-1.537-0.705-2.272-0.705c-0.656,0-1.359,0.256-1.359,0.944
		c0,1.329,3.855,1.345,3.855,3.554C114.008,23.215,113.145,24.271,111.255,24.271z'/>
	<path fill='#E87224' d='M116.646,15.067c-0.416,0-0.785-0.448-0.785-0.913c0-0.448,0.369-0.864,0.785-0.864
		c0.446,0,0.832,0.417,0.832,0.864C117.479,14.603,117.096,15.067,116.646,15.067z M116.055,24.063v-7.251h1.231v7.251H116.055z'/>
	<path fill='#E87224' d='M122.711,24.271c-2.018,0-3.361-1.489-3.361-3.826c0-2.128,1.232-3.825,3.537-3.825
		c2.033,0,3.379,1.473,3.379,3.825C126.266,22.687,124.902,24.271,122.711,24.271z M122.729,17.676c-0.576,0-1.09,0.256-1.231,0.4
		c-0.16,0.16-0.815,1.041-0.815,2.289c0,2.305,1.231,2.833,2.191,2.833c0.496,0,1.089-0.24,1.249-0.4
		c0.176-0.176,0.813-0.992,0.813-2.241c0-0.112,0-0.224-0.016-0.336C124.76,18.221,123.831,17.676,122.729,17.676z'/>
	<path fill='#E87224' d='M134.393,24.063h-1.217c0.032-1.633,0.032-2.065,0.032-3.137v-0.672c0-0.272,0-0.576-0.017-0.864
		c-0.063-1.104-0.159-1.665-1.232-1.665c-0.416,0-0.977,0.128-1.089,0.192c-0.177,0.096-0.849,0.8-1.199,1.281v4.866h-1.233V18.83
		c0-0.833-0.063-1.441-0.127-2.017h1.137l0.191,1.072c0.608-0.72,1.328-1.264,2.434-1.264c1.298,0,2.193,0.416,2.306,1.905
		c0.063,0.72,0.063,1.44,0.063,2.161C134.439,21.838,134.439,22.415,134.393,24.063z'/>
	<path fill='#505150' d='M16.843,48.255c-2.609,0-5.41-1.92-5.41-5.426c0-3.073,2.353-5.49,5.457-5.49
		c1.617,0,2.338,0.192,3.218,0.608l-0.097,1.201c-0.959-0.528-1.969-0.736-3.281-0.736c-2.08,0-3.857,1.648-3.857,4.322
		c0,2.529,1.809,4.434,4.082,4.434c1.041,0,2.081-0.208,3.057-0.736l0.097,1.217C19.243,48.079,18.155,48.255,16.843,48.255z'/>
	<path fill='#505150' d='M27.915,48.063h-1.232c0.047-1.633,0.047-2.063,0.047-3.137c0-0.336,0-0.96-0.031-1.537
		c-0.064-1.104-0.145-1.664-1.217-1.664c-0.432,0-0.992,0.127-1.104,0.191c-0.177,0.096-0.849,0.8-1.185,1.281v4.865H21.96V36.378
		h1.232v5.458c0.608-0.721,1.281-1.216,2.4-1.216c1.281,0,2.193,0.416,2.306,1.905c0.048,0.72,0.048,1.438,0.048,2.16
		C27.946,45.838,27.946,46.415,27.915,48.063z'/>
	<path fill='#505150' d='M31.24,44.045c-0.032,0.176-0.032,0.208-0.032,0.4c0,1.761,0.865,2.752,2.369,2.752
		c0.864,0,1.841-0.367,2.273-0.752l0.096,1.121c-0.641,0.399-1.377,0.705-2.754,0.705c-1.824,0-3.265-1.395-3.265-3.812
		c0-2.479,1.376-3.841,3.233-3.841c2.16,0,2.913,1.136,2.913,3.425L31.24,44.045L31.24,44.045z M33.081,41.66
		c-0.336,0-0.881,0.145-0.961,0.224c-0.096,0.08-0.544,0.479-0.72,1.201h3.554C34.89,41.805,34.025,41.66,33.081,41.66z'/>
	<path fill='#505150' d='M40.585,48.271c-1.921,0-3.297-1.344-3.297-3.892c0-2.655,1.824-3.761,3.297-3.761
		c1.328,0,2.32,0.336,2.417,1.296c0.017,0.146,0.017,0.353,0.017,0.526h-1.153c0-0.625-0.353-0.769-1.216-0.769
		c-0.496,0-0.77,0.063-0.881,0.128c-0.128,0.064-1.137,0.592-1.137,2.529c0,2.033,1.217,2.865,2.289,2.865
		c0.608,0,1.553-0.336,2.049-0.736l0.096,1.121C42.313,48.079,41.433,48.271,40.585,48.271z'/>
	<path fill='#505150' d='M48.985,48.063l-2.674-3.281h-0.496v3.281h-1.232V36.378h1.232v7.507l2.882-3.072h1.648l-3.104,3.232
		l3.265,4.018H48.985z'/>
	<path fill='#505150' d='M59.704,48.063c-0.096-0.383-0.224-0.623-0.304-1.041c-0.479,0.688-1.12,1.25-2.272,1.25
		c-1.105,0-1.793-0.721-1.793-1.633c0-0.673,0.128-1.121,0.288-1.328c0.416-0.592,2.961-1.186,3.681-1.313v-1.01
		c0-1.071-0.303-1.313-1.344-1.313c-0.688,0-1.168,0.063-1.168,0.625l-1.152,0.098c0-0.416,0.047-0.785,0.144-1.088
		c0.032-0.129,0.8-0.688,2.353-0.688c1.585,0,2.401,0.752,2.401,1.92v3.362c0,0.736,0.159,1.632,0.319,2.16h-1.153V48.063z
		 M59.304,44.958c-0.768,0.128-1.936,0.544-2.641,0.88c-0.08,0.305-0.08,0.448-0.08,0.592c0,0.448,0.129,0.771,0.881,0.771
		c0.944,0,1.84-1.058,1.84-1.187V44.958z'/>
	<path fill='#505150' d='M65.72,48.271c-1.92,0-3.297-1.344-3.297-3.892c0-2.655,1.824-3.761,3.297-3.761
		c1.329,0,2.321,0.336,2.418,1.296c0.016,0.146,0.016,0.353,0.016,0.526h-1.152c0-0.625-0.353-0.769-1.217-0.769
		c-0.496,0-0.769,0.063-0.881,0.128c-0.127,0.064-1.137,0.592-1.137,2.529c0,2.033,1.217,2.865,2.289,2.865
		c0.609,0,1.553-0.336,2.049-0.736l0.097,1.121C67.448,48.079,66.568,48.271,65.72,48.271z'/>
	<path fill='#505150' d='M72.376,48.271c-1.92,0-3.297-1.344-3.297-3.892c0-2.655,1.824-3.761,3.297-3.761
		c1.329,0,2.321,0.336,2.418,1.296c0.016,0.146,0.016,0.353,0.016,0.526h-1.152c0-0.625-0.353-0.769-1.217-0.769
		c-0.496,0-0.769,0.063-0.881,0.128c-0.127,0.064-1.137,0.592-1.137,2.529c0,2.033,1.217,2.865,2.289,2.865
		c0.609,0,1.553-0.336,2.049-0.736l0.097,1.121C74.104,48.079,73.225,48.271,72.376,48.271z'/>
	<path fill='#505150' d='M77.047,44.045c-0.032,0.176-0.032,0.208-0.032,0.4c0,1.761,0.865,2.752,2.369,2.752
		c0.864,0,1.841-0.367,2.273-0.752l0.096,1.121c-0.642,0.399-1.377,0.705-2.754,0.705c-1.824,0-3.265-1.395-3.265-3.812
		c0-2.479,1.376-3.841,3.233-3.841c2.16,0,2.913,1.136,2.913,3.425L77.047,44.045L77.047,44.045z M78.888,41.66
		c-0.336,0-0.881,0.145-0.961,0.224c-0.096,0.08-0.544,0.479-0.72,1.201h3.555C80.695,41.805,79.832,41.66,78.888,41.66z'/>
	<path fill='#505150' d='M85.415,48.271c-0.88,0-1.744-0.209-2.306-0.592l0.146-1.152c0.752,0.479,1.152,0.736,2.129,0.736
		c0.96,0,1.554-0.466,1.554-1.072c0-1.361-3.856-1.395-3.856-3.555c0-1.137,0.88-2.018,2.688-2.018c1.023,0,1.775,0.271,2.307,0.544
		l-0.129,1.151c-0.704-0.576-1.536-0.705-2.272-0.705c-0.655,0-1.36,0.258-1.36,0.945c0,1.328,3.857,1.344,3.857,3.553
		C88.169,47.215,87.305,48.271,85.415,48.271z'/>
	<path fill='#505150' d='M91.814,48.271c-0.881,0-1.744-0.209-2.306-0.592l0.146-1.152c0.752,0.479,1.152,0.736,2.129,0.736
		c0.96,0,1.552-0.466,1.552-1.072c0-1.361-3.856-1.395-3.856-3.555c0-1.137,0.881-2.018,2.688-2.018
		c1.024,0,1.776,0.271,2.306,0.544l-0.127,1.151c-0.705-0.576-1.537-0.705-2.273-0.705c-0.655,0-1.36,0.258-1.36,0.945
		c0,1.328,3.856,1.344,3.856,3.553C94.566,47.215,93.704,48.271,91.814,48.271z'/>
	<path fill='#505150' d='M103.256,48.271c-2.018,0-3.361-1.49-3.361-3.826c0-2.129,1.232-3.825,3.537-3.825
		c2.033,0,3.379,1.474,3.379,3.825C106.811,46.688,105.447,48.271,103.256,48.271z M103.271,41.676c-0.576,0-1.088,0.256-1.231,0.4
		c-0.16,0.16-0.815,1.041-0.815,2.289c0,2.305,1.231,2.832,2.191,2.832c0.496,0,1.089-0.239,1.249-0.399
		c0.176-0.176,0.815-0.991,0.815-2.241c0-0.111,0-0.224-0.018-0.336C105.305,42.221,104.376,41.676,103.271,41.676z'/>
	<path fill='#505150' d='M111.752,48.271c-0.593,0-1.12-0.112-1.602-0.369v4.146h-1.231v-9.22c0-0.734-0.032-1.219-0.129-2.018
		h1.138l0.191,1.071c0.77-0.88,1.456-1.264,2.48-1.264c1.473,0,2.625,0.96,2.625,3.521
		C115.227,46.639,113.801,48.271,111.752,48.271z M112.313,41.725c-0.353,0-0.849,0.129-0.961,0.192
		c-0.144,0.097-0.849,0.784-1.199,1.296v3.568c0.336,0.209,0.929,0.418,1.425,0.418c1.567,0,2.305-1.168,2.305-2.913
		C113.88,43.245,113.688,41.725,112.313,41.725z'/>
	<path fill='#505150' d='M118.936,48.271c-0.865,0-1.567-0.545-1.567-1.811v-4.674h-1.313v-0.96h1.313v-2.466h1.198v2.449h1.906
		v0.959h-1.906v4.291c0,0.785,0.209,1.137,0.754,1.137c0.353,0,0.703-0.256,1.072-0.418l0.096,1.025
		C120.104,48.047,119.479,48.271,118.936,48.271z'/>
	<path fill='#505150' d='M122.822,39.066c-0.416,0-0.784-0.447-0.784-0.912c0-0.448,0.368-0.864,0.784-0.864
		c0.447,0,0.832,0.417,0.832,0.864C123.654,38.604,123.271,39.066,122.822,39.066z M122.229,48.063v-7.25h1.232v7.25H122.229z'/>
	<path fill='#505150' d='M128.887,48.271c-2.017,0-3.361-1.49-3.361-3.826c0-2.129,1.232-3.825,3.537-3.825
		c2.033,0,3.379,1.474,3.379,3.825C132.439,46.688,131.079,48.271,128.887,48.271z M128.902,41.676c-0.575,0-1.088,0.256-1.231,0.4
		c-0.159,0.16-0.815,1.041-0.815,2.289c0,2.305,1.231,2.832,2.191,2.832c0.496,0,1.09-0.239,1.249-0.399
		c0.177-0.176,0.815-0.991,0.815-2.241c0-0.111,0-0.224-0.016-0.336C130.936,42.221,130.007,41.676,128.902,41.676z'/>
	<path fill='#505150' d='M140.566,48.063h-1.215c0.03-1.633,0.03-2.063,0.03-3.137v-0.672c0-0.271,0-0.576-0.017-0.863
		c-0.063-1.104-0.159-1.666-1.232-1.666c-0.416,0-0.976,0.129-1.089,0.192c-0.176,0.097-0.849,0.8-1.198,1.28v4.867h-1.232V42.83
		c0-0.833-0.063-1.441-0.129-2.018h1.138l0.19,1.071c0.608-0.72,1.33-1.264,2.435-1.264c1.297,0,2.192,0.416,2.306,1.903
		c0.063,0.722,0.063,1.44,0.063,2.162C140.616,45.838,140.616,46.415,140.566,48.063z'/>
	<path fill='#505150' d='M144.919,48.271c-0.881,0-1.745-0.209-2.306-0.592l0.145-1.152c0.752,0.479,1.152,0.736,2.129,0.736
		c0.961,0,1.553-0.466,1.553-1.072c0-1.361-3.856-1.395-3.856-3.555c0-1.137,0.881-2.018,2.688-2.018
		c1.023,0,1.776,0.271,2.306,0.544l-0.127,1.151c-0.705-0.576-1.537-0.705-2.272-0.705c-0.656,0-1.361,0.258-1.361,0.945
		c0,1.328,3.856,1.344,3.856,3.553C147.671,47.215,146.809,48.271,144.919,48.271z'/>
</symbol>

<symbol id='access-public' >
	<path fill='#E87224' d='M28.007,19.942c-0.256,0-1.137,0-1.665-0.08v4.146h-1.264V13.475l0.592-0.032
		c0.832-0.064,2.337-0.064,2.705-0.064c1.776,0,3.281,1.201,3.281,3.217C31.657,18.581,29.816,19.942,28.007,19.942z M27.943,14.436
		l-1.601,0.016v4.434h1.185c1.425,0,2.689-0.512,2.689-2.289C30.216,15.108,29.416,14.436,27.943,14.436z'/>
	<path fill='#E87224' d='M38.344,24.007l-0.192-1.057c-0.688,0.945-1.281,1.265-2.449,1.265c-1.296,0-2.321-0.448-2.321-1.905
		v-5.554h1.232v4.69c0,0.896,0.08,1.697,1.201,1.697c0.448,0,0.88-0.096,1.28-0.288c0.417-0.352,0.88-0.977,1.009-1.281v-4.818
		h1.232v5.25c0,0.656,0.032,1.345,0.128,2.001H38.344z'/>
	<path fill='#E87224' d='M44.566,24.215c-0.656,0-1.504-0.08-2.481-0.272v-11.62h1.232v4.69c0.592-0.24,1.329-0.448,2.241-0.448
		c1.648,0,2.833,0.976,2.833,3.666C48.392,22.663,46.967,24.215,44.566,24.215z M45.335,17.669c-0.592,0-0.944,0.016-1.024,0.048
		c-0.224,0.064-0.656,0.208-0.993,0.384v4.946c0.496,0.096,0.849,0.096,1.185,0.096c1.601,0,2.545-0.977,2.545-2.817
		C47.047,18.773,46.631,17.669,45.335,17.669z'/>
	<path fill='#E87224' d='M50.373,24.007V12.323h1.232v11.685L50.373,24.007L50.373,24.007z'/>
	<path fill='#E87224' d='M54.902,15.012c-0.416,0-0.784-0.448-0.784-0.913c0-0.448,0.368-0.864,0.784-0.864
		c0.448,0,0.833,0.417,0.833,0.864C55.734,14.547,55.35,15.012,54.902,15.012z M54.31,24.007v-7.251h1.232v7.251H54.31z'/>
	<path fill='#E87224' d='M59.926,24.215c-0.88,0-1.745-0.208-2.305-0.592l0.144-1.152c0.752,0.48,1.152,0.736,2.129,0.736
		c0.96,0,1.552-0.464,1.552-1.072c0-1.361-3.857-1.393-3.857-3.554c0-1.136,0.88-2.017,2.689-2.017c1.024,0,1.777,0.272,2.305,0.544
		l-0.128,1.153c-0.705-0.577-1.537-0.705-2.273-0.705c-0.656,0-1.361,0.256-1.361,0.944c0,1.329,3.857,1.345,3.857,3.554
		C62.679,23.159,61.814,24.215,59.926,24.215z'/>
	<path fill='#E87224' d='M70.6,24.007h-1.232c0.048-1.633,0.048-2.065,0.048-3.137c0-0.336,0-0.96-0.032-1.537
		c-0.064-1.104-0.144-1.665-1.216-1.665c-0.432,0-0.993,0.128-1.104,0.192c-0.176,0.096-0.848,0.8-1.185,1.281v4.866h-1.232V12.323
		h1.232v5.458c0.608-0.72,1.281-1.216,2.401-1.216c1.281,0,2.193,0.416,2.305,1.905c0.048,0.72,0.048,1.44,0.048,2.161
		C70.631,21.782,70.631,22.359,70.6,24.007z'/>
	<path fill='#E87224' d='M73.925,19.99c-0.032,0.176-0.032,0.208-0.032,0.4c0,1.761,0.865,2.753,2.369,2.753
		c0.864,0,1.841-0.368,2.273-0.752l0.096,1.121c-0.641,0.4-1.377,0.704-2.753,0.704c-1.825,0-3.265-1.393-3.265-3.81
		c0-2.481,1.376-3.841,3.233-3.841c2.161,0,2.913,1.136,2.913,3.425H73.925z M75.766,17.604c-0.336,0-0.88,0.145-0.96,0.224
		c-0.096,0.08-0.544,0.48-0.72,1.201h3.553C77.575,17.749,76.71,17.604,75.766,17.604z'/>
	<path fill='#E87224' d='M84.006,18.197c0-0.096-0.031-0.512-0.512-0.512c-0.688,0-1.201,1.121-1.457,1.713v4.61h-1.232v-5.266
		c0-0.769-0.049-1.265-0.129-1.985h1.137l0.209,1.152c0.24-0.624,0.863-1.329,1.647-1.329c0.672,0,1.058,0.368,1.138,1.057
		c0.016,0.144,0.047,0.288,0.047,0.432L84.006,18.197z'/>
	<path fill='#E87224' d='M92.773,24.215h-0.961l-2.881-7.459h1.313l2.112,5.586l1.89-5.586h1.217L92.773,24.215z'/>
	<path fill='#E87224' d='M97.525,19.99c-0.032,0.176-0.032,0.208-0.032,0.4c0,1.761,0.864,2.753,2.368,2.753
		c0.865,0,1.842-0.368,2.273-0.752l0.096,1.121c-0.641,0.4-1.377,0.704-2.752,0.704c-1.826,0-3.266-1.393-3.266-3.81
		c0-2.481,1.376-3.841,3.232-3.841c2.162,0,2.914,1.136,2.914,3.425H97.525z M99.366,17.604c-0.337,0-0.88,0.145-0.96,0.224
		c-0.096,0.08-0.545,0.48-0.721,1.201h3.553C101.174,17.749,100.311,17.604,99.366,17.604z'/>
	<path fill='#E87224' d='M107.605,18.197c0-0.096-0.031-0.512-0.512-0.512c-0.688,0-1.201,1.121-1.457,1.713v4.61h-1.232v-5.266
		c0-0.769-0.049-1.265-0.129-1.985h1.138l0.208,1.152c0.24-0.624,0.863-1.329,1.648-1.329c0.672,0,1.057,0.368,1.137,1.057
		c0.016,0.144,0.048,0.288,0.048,0.432L107.605,18.197z'/>
	<path fill='#E87224' d='M111.445,24.215c-0.881,0-1.745-0.208-2.305-0.592l0.144-1.152c0.752,0.48,1.151,0.736,2.129,0.736
		c0.96,0,1.552-0.464,1.552-1.072c0-1.361-3.857-1.393-3.857-3.554c0-1.136,0.881-2.017,2.689-2.017
		c1.023,0,1.777,0.272,2.305,0.544l-0.127,1.153c-0.705-0.577-1.537-0.705-2.273-0.705c-0.656,0-1.361,0.256-1.361,0.944
		c0,1.329,3.857,1.345,3.857,3.554C114.197,23.159,113.333,24.215,111.445,24.215z'/>
	<path fill='#E87224' d='M116.836,15.012c-0.416,0-0.783-0.448-0.783-0.913c0-0.448,0.367-0.864,0.783-0.864
		c0.448,0,0.833,0.417,0.833,0.864C117.669,14.547,117.285,15.012,116.836,15.012z M116.244,24.007v-7.251h1.232v7.251H116.244z'/>
	<path fill='#E87224' d='M122.9,24.215c-2.017,0-3.36-1.489-3.36-3.826c0-2.128,1.231-3.825,3.537-3.825
		c2.032,0,3.377,1.473,3.377,3.825C126.454,22.631,125.094,24.215,122.9,24.215z M122.917,17.621c-0.576,0-1.088,0.256-1.231,0.4
		c-0.16,0.16-0.816,1.041-0.816,2.289c0,2.305,1.232,2.833,2.193,2.833c0.496,0,1.088-0.24,1.248-0.4
		c0.176-0.176,0.816-0.992,0.816-2.241c0-0.112,0-0.224-0.016-0.336C124.95,18.165,124.021,17.621,122.917,17.621z'/>
	<path fill='#E87224' d='M134.582,24.007h-1.216c0.032-1.633,0.032-2.065,0.032-3.137v-0.672c0-0.271,0-0.576-0.017-0.864
		c-0.063-1.104-0.159-1.665-1.231-1.665c-0.416,0-0.977,0.128-1.088,0.192c-0.176,0.096-0.849,0.8-1.201,1.281v4.866h-1.232v-5.234
		c0-0.833-0.064-1.441-0.128-2.017h1.138l0.191,1.072c0.607-0.72,1.329-1.264,2.434-1.264c1.295,0,2.192,0.416,2.305,1.905
		c0.063,0.72,0.063,1.44,0.063,2.161C134.63,21.782,134.63,22.359,134.582,24.007z'/>
	<path fill='#505150' d='M33.199,43.589v4.418h-1.265v-4.418l-3.618-6.13h1.393l2.929,4.914l2.849-4.914h1.281L33.199,43.589z'/>
	<path fill='#505150' d='M39.791,48.215c-2.017,0-3.361-1.488-3.361-3.826c0-2.127,1.232-3.824,3.537-3.824
		c2.033,0,3.377,1.473,3.377,3.824C43.344,46.631,41.983,48.215,39.791,48.215z M39.807,41.621c-0.576,0-1.088,0.256-1.232,0.4
		c-0.16,0.16-0.816,1.041-0.816,2.289c0,2.305,1.232,2.832,2.193,2.832c0.496,0,1.088-0.24,1.249-0.399
		c0.176-0.177,0.816-0.992,0.816-2.241c0-0.111,0-0.225-0.016-0.336C41.839,42.165,40.911,41.621,39.807,41.621z'/>
	<path fill='#505150' d='M50.287,48.007l-0.192-1.057c-0.688,0.944-1.281,1.265-2.449,1.265c-1.296,0-2.321-0.447-2.321-1.904
		v-5.555h1.232v4.689c0,0.896,0.08,1.697,1.201,1.697c0.448,0,0.88-0.096,1.28-0.287c0.417-0.353,0.88-0.978,1.009-1.281v-4.818
		h1.232v5.25c0,0.656,0.032,1.346,0.128,2.001H50.287z'/>
	<path fill='#505150' d='M63.984,48.007h-1.232C62.8,46.374,62.8,45.941,62.8,44.87c0-0.336,0-0.96-0.032-1.537
		c-0.064-1.104-0.144-1.665-1.216-1.665c-0.432,0-0.992,0.128-1.104,0.191c-0.176,0.097-0.848,0.801-1.185,1.281v4.866h-1.232
		V36.322h1.232v5.459c0.608-0.721,1.281-1.217,2.401-1.217c1.281,0,2.193,0.416,2.305,1.906c0.048,0.719,0.048,1.439,0.048,2.16
		C64.016,45.781,64.016,46.359,63.984,48.007z'/>
	<path fill='#505150' d='M70.447,48.007c-0.096-0.384-0.224-0.624-0.304-1.041c-0.48,0.688-1.121,1.249-2.273,1.249
		c-1.104,0-1.792-0.72-1.792-1.633c0-0.672,0.128-1.12,0.288-1.328c0.416-0.592,2.961-1.186,3.681-1.313v-1.01
		c0-1.072-0.304-1.313-1.344-1.313c-0.688,0-1.168,0.064-1.168,0.625l-1.152,0.096c0-0.416,0.048-0.783,0.144-1.088
		c0.032-0.128,0.8-0.688,2.353-0.688c1.585,0,2.401,0.752,2.401,1.92v3.361c0,0.736,0.16,1.633,0.32,2.161H70.447z M70.046,44.902
		c-0.768,0.127-1.937,0.543-2.641,0.879c-0.08,0.305-0.08,0.449-0.08,0.593c0,0.448,0.128,0.769,0.88,0.769
		c0.944,0,1.84-1.057,1.84-1.185L70.046,44.902L70.046,44.902z'/>
	<path fill='#505150' d='M76.414,48.215h-0.96l-2.881-7.459h1.313l2.113,5.586l1.889-5.586h1.217L76.414,48.215z'/>
	<path fill='#505150' d='M81.166,43.99c-0.032,0.176-0.032,0.207-0.032,0.4c0,1.76,0.865,2.752,2.369,2.752
		c0.864,0,1.841-0.367,2.272-0.752l0.097,1.121c-0.642,0.4-1.377,0.704-2.753,0.704c-1.825,0-3.265-1.394-3.265-3.81
		c0-2.481,1.376-3.842,3.233-3.842c2.161,0,2.913,1.137,2.913,3.426H81.166z M83.006,41.604c-0.336,0-0.88,0.146-0.96,0.225
		c-0.096,0.08-0.544,0.48-0.72,1.201h3.553C84.814,41.749,83.951,41.604,83.006,41.604z'/>
	<path fill='#505150' d='M95.663,48.007c-0.097-0.384-0.224-0.624-0.304-1.041c-0.48,0.688-1.121,1.249-2.273,1.249
		c-1.104,0-1.792-0.72-1.792-1.633c0-0.672,0.128-1.12,0.288-1.328c0.416-0.592,2.961-1.186,3.682-1.313v-1.01
		c0-1.072-0.305-1.313-1.345-1.313c-0.688,0-1.168,0.064-1.168,0.625L91.6,42.34c0-0.416,0.047-0.783,0.144-1.088
		c0.032-0.128,0.8-0.688,2.353-0.688c1.586,0,2.401,0.752,2.401,1.92v3.361c0,0.736,0.16,1.633,0.319,2.161H95.663z M95.262,44.902
		c-0.768,0.127-1.937,0.543-2.641,0.879c-0.08,0.305-0.08,0.449-0.08,0.593c0,0.448,0.128,0.769,0.88,0.769
		c0.944,0,1.841-1.057,1.841-1.185V44.902L95.262,44.902z'/>
	<path fill='#505150' d='M101.678,48.215c-1.921,0-3.297-1.344-3.297-3.89c0-2.657,1.825-3.761,3.297-3.761
		c1.329,0,2.321,0.336,2.418,1.295c0.016,0.145,0.016,0.353,0.016,0.529h-1.153c0-0.625-0.353-0.77-1.216-0.77
		c-0.496,0-0.77,0.064-0.881,0.128c-0.127,0.063-1.137,0.592-1.137,2.528c0,2.033,1.217,2.865,2.289,2.865
		c0.608,0,1.553-0.336,2.049-0.736l0.097,1.121C103.407,48.023,102.525,48.215,101.678,48.215z'/>
	<path fill='#505150' d='M108.334,48.215c-1.921,0-3.297-1.344-3.297-3.89c0-2.657,1.824-3.761,3.297-3.761
		c1.329,0,2.32,0.336,2.417,1.295c0.017,0.145,0.017,0.353,0.017,0.529h-1.152c0-0.625-0.352-0.77-1.217-0.77
		c-0.496,0-0.769,0.064-0.879,0.128c-0.129,0.063-1.138,0.592-1.138,2.528c0,2.033,1.218,2.865,2.289,2.865
		c0.608,0,1.554-0.336,2.05-0.736l0.096,1.121C110.063,48.023,109.182,48.215,108.334,48.215z'/>
	<path fill='#505150' d='M113.005,43.99c-0.032,0.176-0.032,0.207-0.032,0.4c0,1.76,0.864,2.752,2.369,2.752
		c0.864,0,1.842-0.367,2.273-0.752l0.096,1.121c-0.641,0.4-1.377,0.704-2.753,0.704c-1.825,0-3.265-1.394-3.265-3.81
		c0-2.481,1.375-3.842,3.232-3.842c2.161,0,2.913,1.137,2.913,3.426H113.005z M114.846,41.604c-0.336,0-0.88,0.146-0.959,0.225
		c-0.097,0.08-0.545,0.48-0.721,1.201h3.553C116.654,41.749,115.79,41.604,114.846,41.604z'/>
	<path fill='#505150' d='M121.373,48.215c-0.88,0-1.745-0.208-2.305-0.592l0.144-1.152c0.752,0.48,1.151,0.736,2.129,0.736
		c0.96,0,1.552-0.464,1.552-1.072c0-1.361-3.856-1.393-3.856-3.554c0-1.136,0.88-2.017,2.688-2.017c1.024,0,1.777,0.271,2.305,0.543
		l-0.127,1.154c-0.705-0.578-1.537-0.705-2.273-0.705c-0.656,0-1.361,0.256-1.361,0.943c0,1.329,3.857,1.346,3.857,3.555
		C124.126,47.159,123.262,48.215,121.373,48.215z'/>
	<path fill='#505150' d='M127.773,48.215c-0.881,0-1.746-0.208-2.306-0.592l0.144-1.152c0.752,0.48,1.152,0.736,2.13,0.736
		c0.96,0,1.552-0.464,1.552-1.072c0-1.361-3.857-1.393-3.857-3.554c0-1.136,0.881-2.017,2.689-2.017
		c1.023,0,1.777,0.271,2.305,0.543l-0.128,1.154c-0.704-0.578-1.536-0.705-2.272-0.705c-0.656,0-1.361,0.256-1.361,0.943
		c0,1.329,3.857,1.346,3.857,3.555C130.525,47.159,129.662,48.215,127.773,48.215z'/>
</symbol>
</defs>
</svg>            
                ";
        #endregion
        #region javascript_cors_entitlement 
        public const string javascript_cors_entitlement = @"
<script>
// Create the XHR object.
function createCORSRequest(method, url) {
  var xhr = new XMLHttpRequest();
  if ('withCredentials' in xhr) {
    // XHR for Chrome/Firefox/Opera/Safari.
    xhr.open(method, url, true);
  } else if (typeof XDomainRequest != 'undefined') {
    // XDomainRequest for IE.
    xhr = new XDomainRequest();
    xhr.open(method, url);
  } else {
    // CORS not supported.
    xhr = null;
  }
  return xhr;
} // end createCORSRequest()

// Make the actual CORS request.
function makeCorsRequest() {

    // -------- On Button-click: First, Set Green-Yellow Demo Title -----------
    elements = document.getElementsByClassName('S000634951104968X');
    for (var i = 0; i < elements.length; i++) {
       elements[i].style.backgroundColor = 'yellow';
       elements[i].style.color = 'green';
       }
    // CREATE ENTITLEMENT API REQUEST URL FOR ACCESS REQUESTS
    accesses = document.getElementsByClassName('elsevier_access');
    var url = 'http://api.elsevier.com/content/article/entitlement/pii/';
    if (accesses.length < 1)
    {
        return;
    }

    for (var i = 0; i < accesses.length; i++) {
       if(i) {
           url += ',';
       }
       url += accesses[i].getAttribute('id');
    }
    url += '?httpAccept=text/xml&apikey=d91051fb976425e3b5f00750cbd33d8b';
    //url += '?apikey=d91051fb976425e3b5f00750cbd33d8b&httpAccept=text/xml';
    
    // alert(url);
    
    // CREATE XML PARSER
    var parseXml;
    if (typeof window.DOMParser != 'undefined'){
       parseXml = function(xmlStr) {
          return ( new window.DOMParser() ).parseFromString(xmlStr, 'text/xml');
          }
    } else if (typeof window.ActiveXObject != 'undefined' &&
          new window.ActiveXObject('Microsoft.XMLDOM')){
          parseXML = function(xmlStr) {   
              var xmlDoc = new window.ActiveXObject('Microsoft.XMLDOM');
              xmlDoc.async = 'false';
              xmlDOc.loadXML(xmlStr);
              return xmlDoc;
        };
    } else {
       throw new Error('No XML parser found');
    }

  var xhr = createCORSRequest('GET', url);
  if (!xhr) {
    alert('CORS not supported');
    return;
  }

  // Response handlers.
  xhr.onload = function() {
    var text = xhr.responseText;
    var response_doc = parseXml(text);
   
    // alert('Response from CORS request to ' + url + ' has text: ' + text);
   
    // RE-SET SVG ACCESS MESSAGE IMAGE FOR ENTITLED PII REQUESTS

    elts_entitlements = response_doc.getElementsByTagName('document-entitlement');
    // alert(elts_entitlements.length);


    for (var i = 0; i < elts_entitlements.length; i++) {
    
        pii =      elts_entitlements[i].getElementsByTagName('pii-norm')[0].textContent;
        entitled = elts_entitlements[i].getElementsByTagName('entitled')[0].textContent;
        // alert(i + ':' +  pii + ' = '  + entitled);
        
        // CHANGE THE ACCESS MESSAGES FOR ENTITLED ARTICLES        
        if (document.getElementById(pii) && entitled == 'true'){
            if (document.getElementById(pii).getAttribute('type') == 'message')
            {
                // Set text message (used by table results viewer) to nothing 
                // to indicate normal access is OK
                document.getElementById(pii).innerHTML = '';
            } else {
                // This is the brief or thumbnail viewer which uses an svg image as an access message.
                document.getElementById(pii).innerHTML = '<use xlink:href=#access-public />';
            }
        }
    }

  }; // end setting xhr.onload

  xhr.onerror = function() {
    // alert('Whoops, there was an error making the Elsevier Entitlement API request.');
  };

  xhr.send();
} // end makeCorsRequest

function addLoadEvent(func) { 
  var oldonload = window.onload; 
  if (typeof window.onload != 'function') { 
    window.onload = func; 
  } else { 
    window.onload = function() { 
      if (oldonload) { 
        oldonload(); 
      } 
      func(); 
    } 
  } 
} 

addLoadEvent(makeCorsRequest); 


</script> ";
        #endregion
        #region iconOpenAccess
        public string iconOpenAccess = @"
<svg xmlns='http://www.w3.org/2000/svg' 
x='0px' y='0px' viewbox='0 0 1000 1000'  
width='80px' height='80px'
xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#' xmlns:cc='http://creativecommons.org/ns#' xmlns:dc='http://purl.org/dc/elements/1.1/'>
  <metadata><rdf:RDF><cc:Work rdf:about=''>
    <dc:format>image/svg+xml</dc:format>
    <dc:type rdf:resource='http://purl.org/dc/dcmitype/StillImage'/>
    <dc:creator>art designer at PLoS, modified by Wikipedia users Nina, Beao, JakobVoss, and AnonMoos</dc:creator>
    <dc:description>Open Access logo, converted into svg, designed by PLoS. This version with transparent background.</dc:description>
    <dc:source>http://commons.wikimedia.org/wiki/File:Open_Access_logo_PLoS_white.svg</dc:source>
    <dc:license rdf:resource='http://creativecommons.org/publicdomain/zero/1.0/'/>
    <cc:license rdf:resource='http://creativecommons.org/publicdomain/zero/1.0/'/>
    <cc:attributionName>art designer at PLoS, modified by Wikipedia users Nina, Beao, JakobVoss, and AnonMoos</cc:attributionName>
    <cc:attributionURL>http://www.plos.org/</cc:attributionURL>
  </cc:Work></rdf:RDF></metadata>
  <rect width='640' height='1000' fill='#ffffff'/>
  <g stroke='#f68212' stroke-width='104.764' fill='none'>
    <path d='M111.387,308.135V272.408A209.21,209.214 0 0,1 529.807,272.408V530.834'/>
    <circle cx='320.004' cy='680.729' r='256.083'/>
  </g>
  <circle fill='#f68212' cx='321.01' cy='681.659' r='86.4287'/>
</svg>
";
        /// <summary> String literal for html svg for External (guest paygate) icon</summary>

        public string iconGuestPays = @"
<svg id='icon-external' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' 
width='60px' height='60px'
x='0px' y='0px' viewBox='0 0 130 130' xml:space='preserve'>
<polygon fill='#008000' points='120.088,16.696 60.256,16.697 60.257,0.095 120.092,0.091 '/>
<rect x='55.91' y='24.562' 
 transform='matrix(0.7071 -0.7071 0.7071 0.7071 1.0877 70.8061)' 
 fill='#008000' width='60.209' height='19.056'/>
<polygon fill='#008000' points='119.975,0.107 119.996,59.938 103.408,59.95 103.393,0.104 '/>
<rect x='3' y='23.5' fill='#008000' width='17' height='87'/>
<rect x='86.49' y='76.059' fill='#008000' width='17' height='36.941'/>
<rect x='3' y='16.692' fill='#008000' width='40.655' height='17'/>
<rect x='3' y='96' fill='#008000' width='100.49' height='17'/>
</svg>
";
#endregion
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
