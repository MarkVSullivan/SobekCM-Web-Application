#region Using directives

using System;
using System.Text.RegularExpressions;
using System.Web;

using SobekCM.Library.Database;
using System.IO;

#endregion

namespace SobekCM.URL_Rewriter
{
	/// <summary> Rewrites the URL to remove the ugly query string context in many cases  </summary>
	/// <remarks> This extends the IHttpModule class.  This allows for the query string to be constructed
	/// from the longer URL, providing cleaner URLs with less visible query strings.  This class would take an incoming URL 
	/// like http://digital.edu/UF00012345/00002/2j and do an internal rewrite so that within the application the URL is
	/// actually http://digital.edu/sobekcm.aspx?urlquery=UF00012345/00002/2j&amp;portal=digital.edu.</remarks>
	public class Rewriter : IHttpModule
	{
		#region IHttpModule Members

		/// <summary> Dispose of this class  </summary>
		public void Dispose() { }
		
		/// <summary> Initialize this class </summary>
		/// <param name="context"></param>
		public void Init(HttpApplication context) 
		{
			context.BeginRequest += RewriteModule_BeginRequest;
		}

		#endregion

		void RewriteModule_BeginRequest(object sender, EventArgs e)
		{
			string appRelative = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.ToLower();
			string url_authority = HttpContext.Current.Request.Url.Authority;

			// If this is for a FILES request, handle that first
			if ((appRelative.IndexOf("~/files/") == 0) && ( appRelative.Length > 8 ))
			{
				string new_query_string = appRelative.Substring(7);
				HttpContext.Current.RewritePath("~/files.aspx?urlrelative=" + appRelative);
				return;
			}

			// Permit OHPi to run
			if (appRelative.IndexOf("ohpi/") > 0)
			{
				return;
			}
			
			// If a USFLDC PURL, call redirection service method
			if (appRelative.Equals("~/") && HttpContext.Current.Request.RawUrl.StartsWith("/?") && (HttpContext.Current.Request.Url.ToString().Contains("usf.edu") || HttpContext.Current.Request.Url.ToString().Contains("localhost:") || HttpContext.Current.Request.Url.ToString().Contains("usf.sobek.ufl.edu")))
			{
				USFLDC_Redirection_Service();
			}

			// Since the user may be using IIPimage server, skip .fgci extensions
			if (appRelative.IndexOf(".fcgi") > 0)
				return;

			// If this is a direct request for a valid file, skip out immediately
			if ((appRelative.IndexOf(".jpg") > 0) || (appRelative.IndexOf(".gif") > 0) || (appRelative.IndexOf(".css") > 0) || (appRelative.IndexOf(".js") > 0) || (appRelative.IndexOf(".png") > 0) || (appRelative.IndexOf(".html") > 0) || (appRelative.IndexOf(".htm") > 0) || (appRelative.IndexOf(".ashx") > 0))
				return;

			// Special code for the favicon.ico
			if (appRelative.IndexOf("favicon.ico") >= 0)
			{
				string favicon_ico_file = HttpContext.Current.Server.MapPath(appRelative);
				if (System.IO.File.Exists(favicon_ico_file))
					return;
				else
				{
					// is there a URL specific one?
					string new_fav_path = "~/design/favicons/" + HttpContext.Current.Request.Url.Host + "/favicon.ico";
					HttpContext.Current.RewritePath(new_fav_path);
					return;
				}
			}

			// Special code for calls to the data ASPX file
			if (appRelative.IndexOf("sobekcm_data.aspx") >= 0) 
			{
				string current_querystring = HttpContext.Current.Request.QueryString.ToString();
				if (current_querystring.Length > 0)
				{
					HttpContext.Current.RewritePath("~/sobekcm_data.aspx?" + current_querystring + "&portal=" + url_authority, true);
				}
				else
				{
					HttpContext.Current.RewritePath("~/sobekcm_data.aspx?portal=" + url_authority, true);
				}
				return;
			}

			// Special code for calls to the oai ASPX file
			if ((appRelative.IndexOf("sobekcm_oai.aspx") >= 0) || ( HttpContext.Current.Request.QueryString["verb"] != null ))
			{
				string current_querystring = HttpContext.Current.Request.QueryString.ToString();
				if (current_querystring.Length > 0)
				{
					HttpContext.Current.RewritePath("~/sobekcm_oai.aspx?" + current_querystring, true);
				}
				else
				{
					HttpContext.Current.RewritePath("~/sobekcm_oai.aspx", true);
				}
				return;
			}

			// Special code for calls to the upload progress ASPX file
			if (appRelative.IndexOf("uploadprogress.aspx") >= 0)
			{
				string current_querystring = HttpContext.Current.Request.QueryString.ToString();
				if (current_querystring.Length > 0)
				{
					HttpContext.Current.RewritePath("~/UploadProgress.aspx?" + current_querystring, true);
				}
				else
				{
					HttpContext.Current.RewritePath("~/UploadProgress.aspx", true);
				}
				return;
			}

			// If there is a period here ( and this is not a RSS request ) return
			if ((appRelative.IndexOf(".") < 0) || ( appRelative.IndexOf("~/rss") == 0 ) || ( appRelative.IndexOf("sobekcm.aspx") >= 0 ))
			{
				// Remove sobekcm.aspx
				if (appRelative.IndexOf("sobekcm.aspx") > 0)
				{
					appRelative = appRelative.Replace("sobekcm.aspx", "");
				}

				// there is nothing to process            
				if (appRelative.Replace("/", "").Replace("~", "").Length == 0)
				{
					// Special code to handle this if this is going to a virtual directory, and does not have a final '/'
					string url_requested = HttpContext.Current.Request.Url.ToString();
					if (( url_requested.IndexOf("?") < 0 ) && ( url_requested[url_requested.Length - 1] != '/'))
					{
						HttpContext.Current.Response.RedirectPermanent(url_requested + "/");
					}

					string current_querystring2 = HttpContext.Current.Request.QueryString.ToString();
					if (current_querystring2.Length > 0)
					{
						HttpContext.Current.RewritePath("sobekcm.aspx?" + current_querystring2 + "&portal=" + url_authority);
					}
					else
					{
						HttpContext.Current.RewritePath("sobekcm.aspx?portal=" + url_authority);
					}
					return;
				}

				// Remove any double slashes
				appRelative = appRelative.Replace("//", "/");

				// Get rid of first part 
				if (appRelative.IndexOf("~/") == 0)
					appRelative = appRelative.Substring(2);

				// Applies to older web setups
				if (appRelative.IndexOf("ufdc/") == 0)
					appRelative = appRelative.Substring(5);

				// Get rid of leading '/'
				if ((appRelative.Length > 0) && (appRelative[0] == '/'))
				{
					appRelative = appRelative.Length > 1 ? appRelative.Substring(1) : String.Empty;
				}

				// Save the original URL
				HttpContext.Current.Items.Add("Original_URL", HttpContext.Current.Request.Url.ToString());

				// Move any relative url information into a query string variable
				//HttpContext.Current.Request.QueryString.Add("urlrelative", appRelative);
				string current_querystring = HttpContext.Current.Request.QueryString.ToString();
				if (appRelative.Length > 0)
				{
					if ((appRelative.IndexOf("dataset/") == 0) || (appRelative.IndexOf("xml/") == 0) || (appRelative.IndexOf("json/") == 0) || (appRelative.IndexOf("dataprovider/") == 0))
					{
						if (current_querystring.Length > 0)
						{
							HttpContext.Current.RewritePath("~/SobekCM_data.aspx?urlrelative=" + appRelative + "&" + current_querystring + "&portal=" + url_authority, true);
						}
						else
						{
							HttpContext.Current.RewritePath("~/SobekCM_data.aspx?urlrelative=" + appRelative + "&portal=" + url_authority, true);
						}
					}
					else
					{
						// Some special rewrites here
						if (appRelative.IndexOf("rss") == 0)
						{
							if (appRelative == "rss")
							{
								HttpContext.Current.RewritePath("~/data/rss/index.htm", true);
							}
							else
							{
								HttpContext.Current.RewritePath("~/data/" + appRelative, true);
							}
						}
						else
						{
							// Special code to redirect to static pages
							if ((appRelative.Length == 16) && ( appRelative[10] == '/' ))
							{
								string bib_possibly = appRelative.Substring(0, 10);
								string vid_possibly = appRelative.Substring(11, 5);
					
								// Use regular expressions to check format
								string bibid_regex = @"[A-Z]{2}[A-Z|0-9]{4}[0-9]{4}";
								string vid_regex = @"^[0-9]{5}$";
								if ((Regex.Match(bib_possibly.ToUpper(), bibid_regex).Success) && (Regex.Match(vid_possibly, vid_regex).Success))
								{
									// This is for an item, so check for ROBOT here
									if (( !String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["robot"])) || ( Library.Navigation.SobekCM_Navigation_Object.Is_UserAgent_IP_Robot(HttpContext.Current.Request.UserAgent, HttpContext.Current.Request.UserHostAddress)))
									{
										string directory = bib_possibly.Substring(0, 2) + "/" + bib_possibly.Substring(2, 2) + "/" + bib_possibly.Substring(4, 2) + "/" + bib_possibly.Substring(6, 2) + "/" + bib_possibly.Substring(8);
										string redirect_dir = "~/data/" + directory + "/" + bib_possibly + "_" + vid_possibly + ".html";
										HttpContext.Current.RewritePath(redirect_dir, true);
										return;
									}
								}
							}

							// Standard rewrite to the sobek application
							if (current_querystring.Length > 0)
							{
								HttpContext.Current.RewritePath("~/sobekcm.aspx?urlrelative=" + appRelative + "&" + current_querystring + "&portal=" + url_authority, true);
							}
							else
							{
								HttpContext.Current.RewritePath("~/sobekcm.aspx?urlrelative=" + appRelative + "&portal=" + url_authority, true);
							}
						}
					}

				}
				else
				{
					if (current_querystring.Length > 0)
					{
						HttpContext.Current.RewritePath("~/sobekcm.aspx?urlrelative=" + appRelative + "&" + current_querystring + "&portal=" + url_authority, true);
					}
					else
					{
						HttpContext.Current.RewritePath("~/sobekcm.aspx?urlrelative=" + appRelative + "&portal=" + url_authority, true);
					}
				}
			}
		}

		void USFLDC_Redirection_Service()
		{
			String packageid=null;
			String aggregationCode = null;
			String purl_handle=null;
			String url_error = "http://guides.lib.usf.edu/content.php?pid=87781&sid=744350";

			try
			{
				purl_handle = HttpContext.Current.Request.RawUrl.Substring(2);
			}
			catch (Exception e)
			{
				purl_handle = "";
			}

			if (purl_handle == "m1" || purl_handle.StartsWith("m1."))
			{
				// Courtesy permanently moved redirect for former partner MCPL for the MCPLHPC (CID=M01)

				HttpContext.Current.Response.StatusCode = 301;
				HttpContext.Current.Response.Redirect("http://cdm16681.contentdm.oclc.org");
			}
			else if (purl_handle.Contains(".") && !purl_handle.Contains("browse") && !purl_handle.Contains("search"))
			{
				// item purl

				packageid = SobekCM_Database.Get_BibID_VID_From_Identifier(purl_handle);

				if (packageid != null)
				{
					packageid = packageid.ToUpper();
					HttpContext.Current.Response.StatusCode = 307;
					HttpContext.Current.Response.Redirect(packageid, true);
				}
				else
				{
					HttpContext.Current.Response.StatusCode = 307;
					HttpContext.Current.Response.Redirect(url_error);
				}
			}
			else if (purl_handle.Contains(".browse") || purl_handle.Contains(".search"))
			{
				// browse or search purl

				String purl_handle_original = purl_handle;
				int pos1 = purl_handle.IndexOf(".");
				String action = "";
				purl_handle=purl_handle.Substring(0,pos1);

				if (purl_handle.Length==2)
				{
					purl_handle = purl_handle.Substring(0, 1) + "0" + purl_handle.Substring(1);
				}

				aggregationCode = SobekCM_Database.Get_AggregationCode_From_CID(purl_handle.ToUpper());

				if (aggregationCode != null)
				{
					if (purl_handle_original.Contains(".browse"))
					{
						action = "/all";
					}
					else
					{
						action = "/advanced";
					}

					HttpContext.Current.Response.StatusCode = 307;
					HttpContext.Current.Response.Redirect(aggregationCode.ToLower() + action, true);
				}
				else
				{
					HttpContext.Current.Response.StatusCode = 307;
					HttpContext.Current.Response.Redirect(url_error);
				}
			}
			else if (purl_handle.Length == 2 || purl_handle.Length == 3)
			{
				// collection purl

				if (purl_handle.Length == 2)
				{
					purl_handle = purl_handle.Substring(0, 1) + "0" + purl_handle.Substring(1);
				}
				
				aggregationCode = SobekCM_Database.Get_AggregationCode_From_CID(purl_handle.ToUpper());

				if (aggregationCode != null)
				{
					HttpContext.Current.Response.StatusCode = 307;
					HttpContext.Current.Response.Redirect(aggregationCode.ToLower(), true);
				}
				else
				{
					HttpContext.Current.Response.StatusCode = 307;
					HttpContext.Current.Response.Redirect(url_error);
				}
			}
			else
			{
				HttpContext.Current.Response.StatusCode = 307;
				HttpContext.Current.Response.Redirect(url_error);
			}
	   }
	}
}
