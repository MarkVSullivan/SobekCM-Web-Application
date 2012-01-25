using System;
using System.Text;

namespace SobekCM.Bib_Package
{
	/// <summary> Stores information about the Digital Archive [DAITSS] associated with this resource </summary>
	/// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
	public class DAITSS_Info
	{
		private bool archive;
		private string account, subaccount, project;

		/// <summary> Constructor for a new instance of the DAITSS_Info class </summary>
		public DAITSS_Info()
		{
			archive = true;
		}

		/// <summary> Gets and sets the flag which indicates this material will be sent to DAITSS </summary>
		public bool toArchive
		{
			get		{		return archive;			}
			set		{		archive = value;		}
		}

		/// <summary> Gets and sets the DAITSS account from the signed contract </summary>
        /// <value> This defaults to 'UF' if there is no value</value>
		public string Account
		{
			get		{		return account ?? "UF";			}
			set		{		account = value;		}
		}

		/// <summary> Gets and sets the DAITSS sub-account, used for internal book-keeping </summary>
		public string SubAccount
		{
			get		{		return subaccount ?? String.Empty;		}
			set		{		subaccount = value;		}
		}

		/// <summary> Gets and sets the DAITSS project </summary>
		public string Project
		{
			get		{		return project ?? String.Empty;			}
			set		{		project = value;		}
		}

		/// <summary> Gets the METS Administrative section for the DAITSS information </summary>
		public string METS_Administrative_Metadata
		{
			get
			{
                if ((String.IsNullOrEmpty(Account)) || (String.IsNullOrEmpty(project)))
                    return String.Empty;

				StringBuilder results = new StringBuilder();

                string indent = String.Empty;



				// Add the entire administrative section
				results.Append("<METS:amdSec>\r\n");
				results.Append( indent + "<METS:digiprovMD ID=\"AMD_DAITSS\">\r\n");
				results.Append( indent + indent + "<METS:mdWrap MDTYPE=\"OTHER\" OTHERMDTYPE=\"DAITSS\">\r\n" );
				results.Append( indent + indent + indent + "<METS:xmlData>\r\n" );
				results.Append( indent + indent + indent + indent + "<daitss:daitss>\r\n");

                if (!String.IsNullOrEmpty(subaccount))
				{
                    results.Append(indent + indent + indent + indent + indent + "<daitss:AGREEMENT_INFO ACCOUNT=\"" + Account + "\" SUB_ACCOUNT=\"" + subaccount + "\" PROJECT=\"" + project + "\"/>\r\n"); 
				}
				else
				{
                    results.Append(indent + indent + indent + indent + indent + "<daitss:AGREEMENT_INFO ACCOUNT=\"" + Account + "\" PROJECT=\"" + project + "\"/>\r\n"); 
				}

				results.Append( indent + indent + indent + indent + "</daitss:daitss>\r\n");
				results.Append( indent + indent + indent + "</METS:xmlData>\r\n");
				results.Append( indent + indent + "</METS:mdWrap>\r\n");
				results.Append( indent + "</METS:digiprovMD>\r\n");
				results.Append( "</METS:amdSec>\r\n");

				// Return this
				return results.ToString();
			}
		}
	}
}
