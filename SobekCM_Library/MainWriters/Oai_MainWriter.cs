#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.OAIPMH;
using SobekCM.Core.Navigation;
using SobekCM.Core.OAI;
using SobekCM.Library.Database;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.MainWriters
{
    /// <summary> Main writer writes browses in OAI-PMH format </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractMainWriter"/>. </remarks>
    public class Oai_MainWriter: abstractMainWriter
    {
        private readonly DataTable oaiSets;
        private readonly NameValueCollection queryString;
        private readonly string url = UI_ApplicationCache_Gateway.Settings.Servers.System_Base_URL;
        private readonly List<string> validArgs;

        private readonly string oai_resource_identifier_base;
        private readonly string oai_repository_name;
        private readonly string oai_repository_identifier;

        private const int RECORDS_PER_PAGE = 100;
        private const int IDENTIFIERS_PER_PAGE = 250;

        private const int WAY_FUTURE_YEAR = 2025;
        private const int WAY_PAST_YEAR = 1900;

        private readonly OAI_PMH_Configuration config;
        private List<string> metadataPrefixes;

        /// <summary> Constructor for a new instance of the Oai_MainWriter class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="Query_String"> URL Query string to parse for OAI-PMH verbs and other values </param>
        public Oai_MainWriter(NameValueCollection Query_String, RequestCache RequestSpecificValues) : base(RequestSpecificValues)
   
        {
            // Build list of valid arguments
            validArgs = new List<string>
                            {
                                "from",
                                "until",
                                "metadataPrefix",
                                "set",
                                "resumptionToken",
                                "identifier",
                                "verb",
                                "portal",
                                "urlrelative"
                            };

            // Load the list of OAI sets
            oaiSets = SobekCM_Database.Get_OAI_Sets();
            queryString = Query_String;

            // Set the response type
            HttpContext.Current.Response.ContentType = "text/xml";

            // Determine some global settings
            if (UI_ApplicationCache_Gateway.Configuration.OAI_PMH != null)
            {
                config = UI_ApplicationCache_Gateway.Configuration.OAI_PMH;
                oai_resource_identifier_base = UI_ApplicationCache_Gateway.Configuration.OAI_PMH.Identifier_Base;
                oai_repository_name = UI_ApplicationCache_Gateway.Configuration.OAI_PMH.Name;
                oai_repository_identifier = UI_ApplicationCache_Gateway.Configuration.OAI_PMH.Identifier;
            }
            else
            {
                config = new OAI_PMH_Configuration();
                config.Set_Default();
                config.Enabled = true;
            }
            if (String.IsNullOrEmpty(oai_resource_identifier_base)) oai_resource_identifier_base = "oai:" + UI_ApplicationCache_Gateway.Settings.System.System_Abbreviation + ":";
            if (String.IsNullOrEmpty(oai_repository_name)) oai_repository_name = UI_ApplicationCache_Gateway.Settings.System.System_Name;
            if (String.IsNullOrEmpty(oai_repository_identifier)) oai_repository_identifier = UI_ApplicationCache_Gateway.Settings.System.System_Abbreviation;

            // Get the list of metadata prefixes permissiable by the system
            metadataPrefixes = new List<string>();
            foreach (OAI_PMH_Metadata_Format thisConfig in config.Metadata_Prefixes)
            {
                metadataPrefixes.Add(thisConfig.Prefix);
            }
        }

        /// <summary> Gets the enumeration of the type of main writer </summary>
        /// <value> This property always returns the enumerational value <see cref="SobekCM.Core.Navigation.Writer_Type_Enum.OAI"/>. </value>
        public override Writer_Type_Enum Writer_Type { get { return Writer_Type_Enum.OAI; } }

        /// <summary> Perform all the work of adding text directly to the response stream back to the web user </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Html(TextWriter Output, Custom_Tracer Tracer)
        {
            // Parse the request
            string verb = queryString["verb"];
            if (string.IsNullOrEmpty(verb))
            {
                Write_Error(Output, "verb=\"\"", "badVerb", "The verb argument is missing.");
                return;
            }

            // Make sure the verb is valid first
            if ((verb != "GetRecord") && (verb != "Identify") && (verb != "ListIdentifiers") &&
                (verb != "ListMetadataFormats") && (verb != "ListMetadataFormats") &&
                (verb != "ListRecords") && (verb != "ListSets"))
            {
                Write_Error(Output, "verb=\"\"", "badVerb", verb + " is not a legal OAI-PMH 2.0 verb.");
                return;
            }

            // If OAI-PMH is currently disbled, return a (custom) error and stop
            if (!config.Enabled)
            {
                Write_Error(Output, "verb=\"" + verb + "\"", "disabled", "OAI-PMH is disabled");
                return;
            }

            // Check each argument for basic validity
            foreach (string thisArg in queryString.AllKeys)
            {
                if (!validArgs.Contains(thisArg))
                {
                    Write_Error(Output, "verb=\"\"", "badArgument", thisArg + " is not a legal OAI-PMH 2.0 argument.");
                    return;
                }
            }

            // Switch result by verb
            switch (verb)
            {
                case "GetRecord":
                    string identifier = "";
                    string metadata = "";
                    foreach (string thisKey in queryString.Keys)
                    {
                        // If this is invalid, throw the error
                        if ((thisKey != "verb") && (thisKey != "identifier") && (thisKey != "metadataPrefix") && (thisKey != "portal") && (thisKey != "urlrelative"))
                        {
                            Write_Error(Output, "verb=\"\"", "badArgument", thisKey + " is not legal in this context.");
                            return;
                        }

                        // If this is identifier, save it
                        if (thisKey == "identifier")
                            identifier = queryString["identifier"];

                        // If this is identifier, save it
                        if (thisKey == "metadataPrefix")
                            metadata = queryString["metadataPrefix"];
                    }

                    // Make sure both the identifier and metadata have data
                    if (identifier.Length == 0)
                    {
                        Write_Error(Output, "verb=\"\"", "badArgument", "identifier required argument is missing.");
                        return;
                    }

                    // Make sure both the identifier and metadata have data
                    if (metadata.Length == 0)
                    {
                        Write_Error(Output, "verb=\"\"", "badArgument", "metadataPrefix required argument is missing.");
                        return;
                    }

                    // Display the record and metadata
                    GetRecord(Output, identifier, metadata);
                    break;

                case "Identify":
                    foreach (string thisKey in queryString.Keys)
                    {
                        // If this is invalid, throw the error
                        if ((thisKey != "verb") && (thisKey != "portal") && ( thisKey != "urlrelative" ))
                        {
                            Write_Error(Output, "verb=\"\"", "badArgument", thisKey + " is not legal in this context.");
                            return;
                        }
                    }
                    Identify(Output);
                    break;

                case "ListIdentifiers":
                    string fromLi = "";
                    string untilLi = "";
                    string setNameLi = "";
                    string metadataPrefixLi = "";
                    string tokenLi = "";
                    foreach (string thisKey in queryString.Keys)
                    {
                        // If this is invalid, throw the error
                        if ((thisKey != "verb") && (thisKey != "from") && (thisKey != "until") &&
                            (thisKey != "metadataPrefix") && (thisKey != "set") && (thisKey != "resumptionToken") && 
                            (thisKey != "portal") && (thisKey != "urlrelative"))
                        {
                            Write_Error(Output, "verb=\"\"", "badArgument", thisKey + " is not legal in this context.");
                            return;
                        }

                        // If this is FROM, save it
                        if (thisKey == "from")
                            fromLi = queryString["from"];

                        // If this is UNTIL, save it
                        if (thisKey == "until")
                            untilLi = queryString["until"];

                        // If this is SET, save it
                        if (thisKey == "set")
                            setNameLi = queryString["set"];

                        // If this is METADATAPREFIX, save it
                        if (thisKey == "metadataPrefix")
                            metadataPrefixLi = queryString["metadataPrefix"];

                        // Check the resumption token
                        if (thisKey == "resumptionToken")
                        {
                            tokenLi = queryString["resumptionToken"];
                            if (tokenLi.Length < 8)
                            {
                                Write_Error(Output, "resumptionToken=\"" + tokenLi + "\" verb=\"ListIdentifiers\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + tokenLi);
                                return;
                            }
                        }
                    }

                    ListIdentifiers(Output, setNameLi, fromLi, untilLi, metadataPrefixLi, tokenLi);
                    break;

                case "ListMetadataFormats":
                    string identifierLmf = "";
                    foreach (string thisKey in queryString.Keys)
                    {
                        // If this is invalid, throw the error
                        if ((thisKey != "verb") && (thisKey != "identifier") && (thisKey != "portal") && (thisKey != "urlrelative"))
                        {
                            Write_Error(Output, "verb=\"\"", "badArgument", thisKey + " is not legal in this context.");
                            return;
                        }

                        // If this is identifier, save it
                        if (thisKey == "identifier")
                            identifierLmf = queryString["identifier"];
                    }

                    ListMetadataFormats(Output, identifierLmf);
                    break;

                case "ListRecords":
                    string fromLr = "";
                    string untilLr = "";
                    string setNameLr = "";
                    string metadataPrefixLr = "";
                    string tokenLr = "";
                    foreach (string thisKey in queryString.Keys)
                    {
                        // If this is invalid, throw the error
                        if ((thisKey != "verb") && (thisKey != "from") && (thisKey != "until") &&
                            (thisKey != "metadataPrefix") && (thisKey != "set") && (thisKey != "resumptionToken") && 
                            (thisKey != "portal") && (thisKey != "urlrelative"))
                        {
                            Write_Error(Output, "verb=\"\"", "badArgument", thisKey + " is not legal in this context.");
                            return;
                        }

                        // If this is FROM, save it
                        if (thisKey == "from")
                            fromLr = queryString["from"];

                        // If this is UNTIL, save it
                        if (thisKey == "until")
                            untilLr = queryString["until"];

                        // If this is SET, save it
                        if (thisKey == "set")
                            setNameLr = queryString["set"];

                        // If this is METADATAPREFIX, save it
                        if (thisKey == "metadataPrefix")
                            metadataPrefixLr = queryString["metadataPrefix"];

                        // Check the resumption token
                        if (thisKey == "resumptionToken")
                        {
                            tokenLr = queryString["resumptionToken"];
                            if (tokenLr.Length < 8)
                            {
                                Write_Error(Output, "resumptionToken=\"" + tokenLr + "\" verb=\"ListRecords\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + tokenLr);
                                return;
                            }
                        }
                    }

                    ListRecords(Output, setNameLr, fromLr, untilLr, metadataPrefixLr, tokenLr);
                    break;

                case "ListSets":
                    foreach (string thisKey in queryString.Keys)
                    {
                        // If this is invalid, throw the error
                        if ((thisKey != "verb") && (thisKey != "resumptionToken") && (thisKey != "portal") && (thisKey != "urlrelative"))
                        {
                            Write_Error(Output, "verb=\"\"", "badArgument", thisKey + " is not legal in this context.");
                            return;
                        }

                        // We will not be using resumptionToken
                        if (thisKey == "resumptionToken")
                        {
                            string tokenLs = queryString["resumptionToken"];
                            Write_Error(Output, "resumptionToken=\"" + tokenLs + "\" verb=\"ListSets\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + tokenLs);
                            return;
                        }
                    }
                    ListSets(Output);
                    break;

                default:
                    Write_Error(Output, "verb=\"" + verb + "\"", "badVerb", verb + " is not a legal OAI-PMH 2.0 verb.");
                    break;
            }
        }

        /// <summary> Lists all of the sets available for OAI-PMH harvesting </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        protected internal void ListSets(TextWriter Output)
        {
            // Start the response
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?> ");
            Output.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"oai2.xsl\" ?>");
            Output.WriteLine("<OAI-PMH xmlns=\"http://www.openarchives.org/OAI/2.0/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd\">");
            Output.WriteLine("<responseDate>" + date_in_utc(DateTime.Now) + "</responseDate>");
            Output.WriteLine("<request verb=\"ListSets\">" + url + "</request>");
            Output.WriteLine("<ListSets>");

            // Display each set information
            foreach (DataRow thisRow in oaiSets.Rows)
            {
                Output.WriteLine("<set>");
                Output.WriteLine("<setSpec>" + thisRow["Code"].ToString().ToLower() + "</setSpec>");
                Output.WriteLine("<setName>" + thisRow["Name"].ToString().Replace("&","&amp;").Replace("\"", "&quot;") + "</setName>");
                Output.WriteLine("<setDescription>");
                Output.WriteLine("<oai_dc:dc xmlns:oai_dc=\"http://www.openarchives.org/OAI/2.0/oai_dc/\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/oai_dc/ http://www.openarchives.org/OAI/2.0/oai_dc.xsd\">");
                Output.WriteLine("\t<dc:title>" + thisRow["Name"].ToString().Replace("&", "&amp;").Replace("\"", "&quot;") + "</dc:title> ");
                Output.WriteLine("\t<dc:identifier>" + oai_resource_identifier_base + ":" + thisRow["Code"].ToString().ToLower() + "</dc:identifier>");
                Output.WriteLine("\t<dc:identifier>" + UI_ApplicationCache_Gateway.Settings.Servers.System_Base_URL + "/" + thisRow["Code"].ToString().ToLower() + "</dc:identifier>");
                if ( thisRow["Description"].ToString().Length > 0 )
                    Output.WriteLine("\t<dc:description>" + thisRow["Description"].ToString().Replace("&", "&amp;").Replace("\"", "&quot;") + "</dc:description>");
                if (thisRow["OAI_Metadata"].ToString().Length > 0)
                {
                    Output.WriteLine("\t" + thisRow["OAI_Metadata"].ToString().Replace("&", "&amp;").Replace("\"", "&quot;"));
                }
                Output.WriteLine("</oai_dc:dc>");
                Output.WriteLine("</setDescription>");
                Output.WriteLine("</set>");
            }

            // Close the response
            Output.WriteLine("</ListSets>");
            Output.WriteLine("</OAI-PMH>");
        }
        /// <summary> Lists all the records associated with a certain OAI-PMH set </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="SetCode"> Code for the OAI-PMH set </param>
        /// <param name="From"> Date from which to return all the identifiers </param>
        /// <param name="Until"> Date to which to return all the identifiers </param>
        /// <param name="ResumptionToken"> Resumption token from the original query string, if one was provided </param>
        /// <param name="MetadataPrefix"> Prefix indicates the format for the metadata </param>
        protected internal void ListRecords(TextWriter Output, string SetCode, string From, 
            string Until, string MetadataPrefix, string ResumptionToken )
        {
            // Set the default dates and page first
            DateTime from_date = new DateTime(WAY_PAST_YEAR, 1, 1);
            DateTime until_date = new DateTime(WAY_FUTURE_YEAR, 1, 1);
            int current_page = 1;

            // Start to build the request XML
            StringBuilder request = new StringBuilder();
            
            // If there is a resumption token, that should be used to pull all information
            if (ResumptionToken.Length > 0)
            {
                // Add to the request
                request.Append("resumptionToken=\"" + ResumptionToken + "\" ");


                // Try to get the MetadataPrefix from the token
                if (ResumptionToken.IndexOf(":") < 0)
                {
                    Write_Error(Output, "resumptionToken=\"" + ResumptionToken + "\" verb=\"ListRecords\"", "badResumptionToken", "no metadata specified in resumption token:" + ResumptionToken);
                    return;
                }

                // Compute the state from the token
                try
                {
                    MetadataPrefix = ResumptionToken.Substring(ResumptionToken.IndexOf(":")+1);
                    string modifiedToken = ResumptionToken.Substring(0, ResumptionToken.IndexOf(":"));

                    string page_string = modifiedToken.Substring(0, 6);
                    SetCode = modifiedToken.Substring(6 + oai_repository_identifier.Length);

                    // Try to parse the page and dates now
                    if (!Int32.TryParse(page_string, out current_page))
                    {
                        Write_Error(Output, "resumptionToken=\"" + ResumptionToken + "\" verb=\"ListRecords\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + ResumptionToken);
                        return;
                    }

                    // Was there a from and until date included in the token?
                    if (SetCode.IndexOf(".") >= 0)
                    {
                        int period_index = SetCode.IndexOf(".");
                        string from_string = SetCode.Substring(period_index + 1, 8);
                        string until_string = SetCode.Substring(period_index + 9, 8);
                        SetCode = SetCode.Substring(0, period_index);

                        // Before we can parse those dates, we need to format a bit for the .NET framework
                        from_string = from_string.Substring(4, 2) + "/" + from_string.Substring(6) + "/" + from_string.Substring(0, 4);
                        until_string = until_string.Substring(4, 2) + "/" + until_string.Substring(6) + "/" + until_string.Substring(0, 4);

                        // Try to parse the dates now
                        if ((!DateTime.TryParse(from_string, out from_date)) || (!DateTime.TryParse(until_string, out until_date)))
                        {
                            Write_Error(Output, "resumptionToken=\"" + ResumptionToken + "\" verb=\"ListRecords\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + ResumptionToken);
                            return;
                        }
                    }
                }
                catch
                {
                    Write_Error(Output, "resumptionToken=\"" + ResumptionToken + "\" verb=\"ListRecords\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + ResumptionToken);
                    return;
                }
            }
            else
            {
                // Add to the request 
                if (From.Length > 0)
                    request.Append("from=\"" + From + "\" ");
                request.Append("metadataPrefix=\"" + MetadataPrefix + "\" ");
                if (SetCode.Length > 0)
                    request.Append("set=\"" + SetCode + "\" ");
                if (Until.Length > 0)
                    request.Append("until=\"" + Until + "\" ");

                // Check the format of FROM and UNTIL, if they are provided
                const string REGEX_MATCH_STRING = "([0-9]{4})(-([0-9]{2})(-([0-9]{2})))";
                if (From.Length > 0)
                {
                    if ((From.Length != 10) || (!Regex.Match(From, REGEX_MATCH_STRING).Success) || (!DateTime.TryParse(From, out from_date)))
                    {
                        Write_Error(Output, "verb=\"\"", "badArgument", "bad OAI date format:" + From);
                        return;
                    }
                }
                if (Until.Length > 0)
                {
                    if ((Until.Length != 10) || (!Regex.Match(Until, REGEX_MATCH_STRING).Success) || (!DateTime.TryParse(Until, out until_date)))
                    {
                        Write_Error(Output, "verb=\"\"", "badArgument", "bad OAI date format:" + Until);
                        return;
                    }
                }
            }

            // Make sure both the identifier and metadata have data
            if (MetadataPrefix.Length == 0)
            {
                Write_Error(Output, "verb=\"\"", "badArgument", "metadataPrefix required argument is missing.");
                return;
            }

            // Metadata prefix must be in list of acceptable
            if (!metadataPrefixes.Contains(MetadataPrefix))
            {
                Write_Error(Output, "verb=\"\"", "cannotDisseminateFormat", MetadataPrefix + " is not supported by this repository.");
                return;
            }

            // Finish the request echo portion
            request.Append("verb=\"ListRecords\"");

            // Get the records
            List<OAI_Record> results = SobekCM_Database.Get_OAI_Data(SetCode, MetadataPrefix, from_date, until_date, RECORDS_PER_PAGE, current_page, true);
            if (results.Count == 0)
            {
                Write_Error(Output, request.ToString(), "noRecordsMatch", "There are no records matching the criteria indicated by this request's arguments.");
            }
            else
            {
                // Determine if a resumption token should be submitted after this.  This is easily
                // done since the database will attempt to return one additional record, if more records
                // exist
                bool more_records = false;
                if (results.Count > RECORDS_PER_PAGE)
                {
                    more_records = true;
                    results.RemoveAt(results.Count - 1);
                }

                // Write the list of records/identifiers
                Write_OAI_ListRecords(SetCode, from_date, until_date, ResumptionToken, request.ToString(), false, current_page, more_records, MetadataPrefix, results, Output);
            }
        }

        /// <summary> Lists all the records associated with a single OAI-PMH set </summary>
        /// <param name="SetCode"> Code for the OAI-PMH set </param>
        /// <param name="From"> Date from which to return all the identifiers </param>
        /// <param name="Until"> Date to which to return all the identifiers </param>
        /// <param name="ResumptionToken"> Resumption token from the original query string, if one was provided </param>
        /// <param name="Request"> String that represents the original request </param>
        /// <param name="HeadersOnly"> Flag indicates to just return the headers of each digital resource </param>
        /// <param name="Current_Page">Current page number within the set of results</param>
        /// <param name="More_Records"> Flag indicates if a resumption token should be issued</param>
        /// <param name="MetadataPrefix"> MetadataPrefix requested </param>
        /// <param name="Records"> List of all the records to return to the user </param>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <returns> TRUE if successfuly, otherwise FALSE</returns>
        protected internal void Write_OAI_ListRecords(string SetCode, DateTime From, DateTime Until,
            string ResumptionToken, string Request, bool HeadersOnly, int Current_Page, bool More_Records, string MetadataPrefix, List<OAI_Record> Records, TextWriter Output)
        {
            // If the first read was successful then write the header
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?> ");
            Output.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"oai2.xsl\" ?>");
            Output.WriteLine("<OAI-PMH xmlns=\"http://www.openarchives.org/OAI/2.0/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd\">");
            Output.WriteLine("<responseDate>" + date_in_utc(DateTime.Now) + "</responseDate>");
            Output.WriteLine("<request " + Request + ">" + url + "</request>");
            Output.WriteLine(HeadersOnly ? "<ListIdentifiers>" : "<ListRecords>");

            // Step through each record
            foreach (OAI_Record thisRecord in Records)
            {
                if (!HeadersOnly)
                    Output.Write("<record>");
                Output.Write("<header><identifier>" + oai_resource_identifier_base + thisRecord.BibID + "_" + thisRecord.VID + "</identifier><datestamp>" + thisRecord.Last_Modified_Date.Year + "-" + thisRecord.Last_Modified_Date.Month.ToString().PadLeft(2, '0') + "-" + thisRecord.Last_Modified_Date.Day.ToString().PadLeft(2, '0') + "</datestamp>");
                if (SetCode.Length > 0)
                    Output.Write("<setSpec>" + oai_resource_identifier_base + SetCode + "</setSpec>");
                Output.Write("</header>");
                if (!HeadersOnly)
                {
                    Output.WriteLine("<metadata>" + thisRecord.Record + "</metadata></record>");
                }
            }

            // Should we provide a resumption token for possibly more records?
            if ( More_Records )
            {
                DateTime expDate = DateTime.Now.AddDays(1).ToUniversalTime();
                string expirationDateString = expDate.Year + "-" + expDate.Month.ToString().PadLeft(2, '0') + "-" + expDate.Day.ToString().PadLeft(2, '0') + "T" + expDate.Hour.ToString().PadLeft(2, '0') + ":" + expDate.Minute.ToString().PadLeft(2, '0') + ":00Z";
                string newResumptionToken = (Current_Page + 1).ToString().PadLeft(6, '0') + oai_repository_identifier + SetCode;
                if ((From.Year != WAY_PAST_YEAR) || (Until.Year != WAY_FUTURE_YEAR))
                {
                    newResumptionToken = newResumptionToken + "." + From.Year + From.Month.ToString().PadLeft(2, '0') + From.Day.ToString().PadLeft(2, '0') + Until.Year + Until.Month.ToString().PadLeft(2, '0') + Until.Day.ToString().PadLeft(2, '0');
                }
                Output.WriteLine("<resumptionToken expirationDate=\"" + expirationDateString + "\">" + newResumptionToken + ":" + MetadataPrefix + "</resumptionToken>");
            }

            // Write the response
            Output.WriteLine(HeadersOnly ? "</ListIdentifiers>" : "</ListRecords>");
            Output.WriteLine("</OAI-PMH>");
        }

        /// <summary> Lists all of the identifiers within a particular OAI-PMH set </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="SetCode"> Code for the OAI-PMH set </param>
        /// <param name="From"> Date from which to return all the identifiers </param>
        /// <param name="Until"> Date to which to return all the identifiers </param>
        /// <param name="MetadataPrefix"> Prefix of the metadata fomat to return the identifier information</param>
        /// <param name="ResumptionToken"> Resumption token from the original query string, if one was provided </param>
        protected internal void ListIdentifiers(TextWriter Output, string SetCode, string From, string Until, string MetadataPrefix, string ResumptionToken )
        {

            // Set the default dates and page first
            DateTime from_date = new DateTime(1900, 1, 1);
            DateTime until_date = DateTime.Now.AddDays(1);
            int current_page = 1;

            // Start to build the request XML
            StringBuilder request = new StringBuilder();

            // If there is a resumption token, that should be used to pull all information
            if (ResumptionToken.Length > 0)
            {
                // Add to the request
                request.Append("resumptionToken=\"" + ResumptionToken + "\" ");

                // Try to get the MetadataPrefix from the token
                if (ResumptionToken.IndexOf(":") < 0)
                {
                    Write_Error(Output, "resumptionToken=\"" + ResumptionToken + "\" verb=\"ListRecords\"", "badResumptionToken", "no metadata specified in resumption token:" + ResumptionToken);
                    return;
                }
                
                // Compute the state from the token
                try
                {

                    MetadataPrefix = ResumptionToken.Substring(ResumptionToken.IndexOf(":") + 1);
                    string modifiedToken = ResumptionToken.Substring(0, ResumptionToken.IndexOf(":"));

                    string page_string = modifiedToken.Substring(0, 6);
                    SetCode = modifiedToken.Substring(6 + oai_repository_identifier.Length);

                    // Try to parse the page and dates now
                    if (!Int32.TryParse(page_string, out current_page))
                    {
                        Write_Error(Output, "resumptionToken=\"" + ResumptionToken + "\" verb=\"ListRecords\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + ResumptionToken);
                        return;
                    }

                    // Was there a from and until date included in the token?
                    if (SetCode.IndexOf(".") > 0)
                    {
                        int period_index = SetCode.IndexOf(".");
                        string from_string = SetCode.Substring(period_index + 1, 8);
                        string until_string = SetCode.Substring(period_index + 9, 8);
                        SetCode = SetCode.Substring(0, period_index);

                        // Before we can parse those dates, we need to format a bit for the .NET framework
                        from_string = from_string.Substring(4, 2) + "/" + from_string.Substring(6) + "/" + from_string.Substring(0, 4);
                        until_string = until_string.Substring(4, 2) + "/" + until_string.Substring(6) + "/" + until_string.Substring(0, 4);

                        // Try to parse the dates now
                        if ((!DateTime.TryParse(from_string, out from_date)) || (!DateTime.TryParse(until_string, out until_date)))
                        {
                            Write_Error(Output, "resumptionToken=\"" + ResumptionToken + "\" verb=\"ListRecords\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + ResumptionToken);
                            return;
                        }
                    }
                }
                catch
                {
                    Write_Error(Output, "resumptionToken=\"" + ResumptionToken + "\" verb=\"ListIdentifiers\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + ResumptionToken);
                    return;
                }
            }
            else
            {
                // Add to the request 
                if (From.Length > 0)
                    request.Append("from=\"" + From + "\" ");
                request.Append("metadataPrefix=\"" + MetadataPrefix + "\" ");
                if (SetCode.Length > 0)
                    request.Append("set=\"" + SetCode + "\" ");
                if (Until.Length > 0)
                    request.Append("until=\"" + Until + "\" ");

                // Check the format of FROM and UNTIL, if they are provided
                const string REGEX_MATCH_STRING = "([0-9]{4})(-([0-9]{2})(-([0-9]{2})))";
                if (From.Length > 0)
                {
                    if ((From.Length != 10) || (!Regex.Match(From, REGEX_MATCH_STRING).Success) || (!DateTime.TryParse(From, out from_date)))
                    {
                        Write_Error(Output, "verb=\"\"", "badArgument", "bad OAI date format:" + From);
                        return;
                    }
                }
                if (Until.Length > 0)
                {
                    if ((Until.Length != 10) || (!Regex.Match(Until, REGEX_MATCH_STRING).Success) || (!DateTime.TryParse(Until, out until_date)))
                    {
                        Write_Error(Output, "verb=\"\"", "badArgument", "bad OAI date format:" + Until);
                        return;
                    }
                }
            }

            // Make sure both the identifier and metadata have data
            if (MetadataPrefix.Length == 0)
            {
                Write_Error(Output, "verb=\"\"", "badArgument", "metadataPrefix required argument is missing.");
                return;
            }


            // Finish the request echo portion
            request.Append("verb=\"ListIdentifiers\"");

            // Metadata prefix must be in list of acceptable
            if (!metadataPrefixes.Contains(MetadataPrefix))
            {
                Write_Error(Output, request.ToString(), "cannotDisseminateFormat", MetadataPrefix + " is not supported by this repository.");
                return;
            }

            // Get the records
            List<OAI_Record> results = SobekCM_Database.Get_OAI_Data(SetCode, MetadataPrefix, from_date, until_date, IDENTIFIERS_PER_PAGE, current_page, false );
            if (results.Count == 0)
            {
                Write_Error(Output, request.ToString(), "noRecordsMatch", "There are no records matching the criteria indicated by this request's arguments.");
            }
            else
            {
                // Determine if a resumption token should be submitted after this.  This is easily
                // done since the database will attempt to return one additional record, if more records
                // exist
                bool more_records = false;
                if (results.Count > IDENTIFIERS_PER_PAGE)
                {
                    more_records = true;
                    results.RemoveAt(results.Count - 1);
                }

                // Write the list of records/identifiers
                Write_OAI_ListRecords(SetCode, from_date, until_date, ResumptionToken, request.ToString(), true, current_page, more_records, MetadataPrefix, results, Output);
            }
        }

        /// <summary> Identifies the information about this OAI-PMH server </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        protected internal void Identify(TextWriter Output)
        {
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?> ");
            Output.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"oai2.xsl\" ?>");
            Output.WriteLine("<OAI-PMH xmlns=\"http://www.openarchives.org/OAI/2.0/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd\">");
            Output.WriteLine("<responseDate>" + date_in_utc(DateTime.Now) + "</responseDate>");
            Output.WriteLine("<request verb=\"Identify\">" + url + "</request>");
            Output.WriteLine("<Identify>");
            Output.WriteLine("<repositoryName>" + oai_repository_name + "</repositoryName>");
            Output.WriteLine("\t<baseURL>" + url + "</baseURL>");
            Output.WriteLine("\t<protocolVersion>2.0</protocolVersion>");
            if ((config.Admin_Emails != null) && (config.Admin_Emails.Count > 0))
            {
                foreach (string thisEmail in config.Admin_Emails)
                {
                    Output.WriteLine("\t<adminEmail>" + thisEmail + "</adminEmail>");
                }
            }
            else
            {
                Output.WriteLine("\t<adminEmail>" + UI_ApplicationCache_Gateway.Settings.Email.System_Email + "</adminEmail>");
            }
            Output.WriteLine("\t<earliestDatestamp>2005-12-15</earliestDatestamp>");
            Output.WriteLine("\t<deletedRecord>transient</deletedRecord>");
            Output.WriteLine("\t<granularity>YYYY-MM-DD</granularity>");
            Output.WriteLine("<description>");
            Output.WriteLine("\t<oai-identifier xmlns=\"http://www.openarchives.org/OAI/2.0/oai-identifier\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/oai-identifier http://www.openarchives.org/OAI/2.0/oai-identifier.xsd\">");
            Output.WriteLine("\t\t<scheme>oai</scheme>");
            Output.WriteLine("\t\t<repositoryIdentifier>" + oai_repository_identifier + "</repositoryIdentifier>");
            Output.WriteLine("\t\t<delimiter>:</delimiter>");
            Output.WriteLine("\t\t<sampleIdentifier>" + oai_resource_identifier_base + "AB12345678_12345</sampleIdentifier>");
            Output.WriteLine("\t</oai-identifier>");
            Output.WriteLine("</description>");

            Output.WriteLine("<description>");
            Output.WriteLine("\t<oai_dc:dc xmlns:oai_dc=\"http://www.openarchives.org/OAI/2.0/oai_dc/\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/oai_dc/ http://www.openarchives.org/OAI/2.0/oai_dc.xsd\">");
            Output.WriteLine("\t\t<dc:title>" + oai_repository_name + "</dc:title> ");
            Output.WriteLine("\t\t<dc:identifier>" + oai_resource_identifier_base + "</dc:identifier>");
            Output.WriteLine("\t\t<dc:identifier>" + UI_ApplicationCache_Gateway.Settings.Servers.System_Base_URL + "</dc:identifier>");

            if ((config.Descriptions != null) && (config.Descriptions.Count > 0))
            {
                foreach (string thisDesc in config.Descriptions)
                {
                    Output.WriteLine("\t\t<dc:description>" + thisDesc + "</dc:description>");
                }
            }

            Output.WriteLine("\t</oai_dc:dc>");
            Output.WriteLine("</description>");

            Output.WriteLine("</Identify>");
            Output.WriteLine("</OAI-PMH>");
        }

        /// <summary> Gets the OAI-PMH m,etadata for a single digital resource, by identifier </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Identifier"> Identifier for the record to retrieve the metadata for </param>
        /// <param name="MetadataPrefix"> Prefix for the metadata format to return the record </param>
        protected internal void GetRecord(TextWriter Output, string Identifier, string MetadataPrefix)
        {
            // Perform check that the identifier is valid
            bool valid = true;

            // Metadata prefix must be in the list of acceptable metadata prefixes
            if ( !metadataPrefixes.Contains(MetadataPrefix))
            {
                Write_Error(Output, "identifier=\"" + Identifier + "\" metadataPrefix=\"" + MetadataPrefix + "\" verb=\"GetRecord\"", "cannotDisseminateFormat", "Item " + Identifier + " does not have metadata for " + MetadataPrefix);
                return;
            }

            // Ensure the identifier is in basic correct form
            OAI_Record thisTitle = null;
            if (Identifier.IndexOf(oai_resource_identifier_base) != 0)
            {
                valid = false;
            }
            else
            {
                // Get the bib id and vid
                string bibid_vid = Identifier.Substring( oai_resource_identifier_base.Length);
                if (bibid_vid.Length != 16)
                    valid = false;
                else
                {
                    // Pull the information for this item
                    string bibid = bibid_vid.Substring(0, 10);
                    string vid = bibid_vid.Substring(11);
                    thisTitle = SobekCM_Database.Get_OAI_Record(bibid, vid, MetadataPrefix);
                }
            }              

            // Throw error if invalid
            if ((!valid) || (thisTitle == null ))
            {
                Write_Error(Output, "identifier=\"" + Identifier + "\" metadataPrefix=\"" + MetadataPrefix + "\" verb=\"GetRecord\"", "idDoesNotExist", "identifier is not valid URI: " + Identifier);
                return;
            }
            
            // Display the information
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?> ");
            Output.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"oai2.xsl\" ?>");
            Output.WriteLine("<OAI-PMH xmlns=\"http://www.openarchives.org/OAI/2.0/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd\">");
            Output.WriteLine("<responseDate>" + date_in_utc(DateTime.Now) + "</responseDate>");
            Output.WriteLine("<request identifier=\"" + Identifier + "\" metadataPrefix=\"" + MetadataPrefix + "\" verb=\"GetRecord\">" + url + "</request>");
            Output.WriteLine("<GetRecord>");

            Output.Write("<record><header><identifier>" + oai_resource_identifier_base + thisTitle.BibID + "_" + thisTitle.VID + "</identifier><datestamp>" + thisTitle.Last_Modified_Date.Year + "-" + thisTitle.Last_Modified_Date.Month.ToString().PadLeft(2, '0') + "-" + thisTitle.Last_Modified_Date.Day.ToString().PadLeft(2, '0') + "</datestamp></header>");
            Output.WriteLine("<metadata>" + thisTitle.Record + "</metadata></record>");

            Output.WriteLine("</GetRecord>");
            Output.WriteLine("</OAI-PMH>");
        }

        /// <summary> Gets the metadata formats available for either the entire set or a single item, by identifier. </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Identifier"> Identifier to check for different possible identifiers </param>
        protected internal void ListMetadataFormats(TextWriter Output, string Identifier)
        {
            // Prepare response with DC as metadata format
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?> ");
            Output.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"oai2.xsl\" ?>");
            Output.WriteLine("<OAI-PMH xmlns=\"http://www.openarchives.org/OAI/2.0/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd\">");
            Output.WriteLine("<responseDate>" + date_in_utc(DateTime.Now) + "</responseDate>");
            if (Identifier.Length > 0)
                Output.WriteLine("<request identifier=\"" + Identifier + "\" verb=\"ListMetadataFormats\">" + url + "</request>");
            else
                Output.WriteLine("<request verb=\"ListMetadataFormats\">" + url + "</request>");
            Output.WriteLine("<ListMetadataFormats>");

            foreach (OAI_PMH_Metadata_Format format in config.Metadata_Prefixes)
            {
                Output.WriteLine("<metadataFormat>");
                Output.WriteLine("<metadataPrefix>" + format.Prefix + "</metadataPrefix>");
                Output.WriteLine("<schema>" + format.Schema + "</schema>");
                Output.WriteLine("<metadataNamespace>" + format.MetadataNamespace + "</metadataNamespace>");
                Output.WriteLine("</metadataFormat>");  
            }

            Output.WriteLine("</ListMetadataFormats>");
            Output.WriteLine("</OAI-PMH>");
        }

        /// <summary> Write the OAI-PMH error in the correct (OAI-PMH) format </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="RequestVerb"> Verb from the original request </param>
        /// <param name="ErrorCode"> OAI-PMH Code for the error encountered </param>
        /// <param name="ErrorString"> String describes the problem encountered </param>
        protected internal void Write_Error(TextWriter Output, string RequestVerb, string ErrorCode, string ErrorString)
        {
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            Output.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"oai2.xsl\" ?>");
            Output.WriteLine("<OAI-PMH xmlns=\"http://www.openarchives.org/OAI/2.0/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd\">");
            Output.WriteLine("<responseDate>" + date_in_utc(DateTime.Now) + "</responseDate>");
            Output.WriteLine("<request " + RequestVerb + ">" + url + "</request>");
            Output.WriteLine("<error code=\"" + ErrorCode + "\">" + ErrorString + "</error>");
            Output.WriteLine("</OAI-PMH>");
        }

        private static string date_in_utc(DateTime Date)
        {
            DateTime dateUtc = Date.ToUniversalTime();
            return dateUtc.Year.ToString() + "-" + dateUtc.Month.ToString().PadLeft(2, '0') + "-" + dateUtc.Day.ToString().PadLeft(2, '0') + "T" + dateUtc.Hour.ToString().PadLeft(2, '0') + ":" + dateUtc.Minute.ToString().PadLeft(2, '0') + ":" + dateUtc.Second.ToString().PadLeft(2, '0') + "Z";
        }
    }
}
