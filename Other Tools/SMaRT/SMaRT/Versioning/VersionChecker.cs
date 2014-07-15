#region Using directives

using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;

#endregion

namespace SobekCM.Management_Tool.Versioning
{
	/// <summary>
	/// VersionChecker is a class used to check against a XML versioning database file on the network
	/// drives to determine if a newer version of an application is available. <br /> <br /> </summary>
	/// <remarks>  This object should be used as an application is launched to check against the XML
	/// versioning database on the network.  If there is a later version of the calling application, then this class
	/// will tell whether the update is mandatory.  If the update is requested, another Process
	/// can be launched to do the update. <br /> <br />
	/// To use this class, a custom versioning section (shown below) must exist in the Application Config file.  An example
	/// file is <a href="example.exe.config.html">here</a>, and below are just the sections needed.  This section must include 
	/// information on where the XML versioning database sits, as well as the application name and current version.
	/// <code>
	///	&lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
	///	&lt;configuration&gt;
	///
	///		&lt;!-- Define a custom section for VersionChecking                            --&gt;
	///		&lt;configSections&gt;
	///			&lt;section name=&quot;VersionChecker&quot;  type=&quot;System.Configuration.NameValueSectionHandler&quot; /&gt;
	///		&lt;/configSections&gt;
	///   
	///		&lt;!-- Below is information to allow for VersionChecking prior to execution.  --&gt;
	///		&lt;!-- XML_Directory holds all the centralized version information and then   --&gt;
	///		&lt;!-- the AppName and Version allow for checking this particular app.        --&gt;
	///		&lt;VersionChecker&gt;
	///			&lt;add key=&quot;XML_Directory&quot; value=&quot;\\Smathersnt12\DLCdocs\ScanQC\People\Mark\Applications\Versioning&quot; /&gt;
	///			&lt;add key=&quot;AppName&quot; value=&quot;Aerial Image Collector&quot; /&gt;
	///			&lt;add key=&quot;Version&quot; value=&quot;2.1.0&quot; /&gt;
	///		&lt;/VersionChecker&gt;
	///   
	///	&lt;/configuration&gt; 
	/// </code>
	/// <br /> Object created by Mark V Sullivan (2003) for University of Florida's Digital Library Center. </remarks>
	/// <example> EXAMPLE 1: Example <a href="VersioningDB.xml">Versioning XML database</a> can be found below and 
	/// <a href="VersioningDB.xml">here</a>.  The <a href="VersioningDB.xsd">Versioning XSD file</a> can be 
	/// found <a href="VersioningDB.xsd">here</a>:
	/// <code>
	///	&lt;VersionChecker xmlns=&quot;DLC&quot;&gt;
	///		&lt;Application&gt;
	///			&lt;Name&gt;DLC CD Manager&lt;/Name&gt;
	///			&lt;NewVersion&gt;1.1.2&lt;/NewVersion&gt;
	///			&lt;LastOldVersion&gt;1.0.2&lt;/LastOldVersion&gt;
	///			&lt;SetupFile&gt;\\Smathersnt12\DLCdocs\ScanQC\People\Mark\SetupFiles\DLC CD Manager\Upgrade Only\Setup.exe&lt;/SetupFile&gt;
	///			&lt;Mandatory&gt;true&lt;/Mandatory&gt;
	///		&lt;/Application&gt;
	///		&lt;Application&gt;
	///			&lt;Name&gt;DLC FTP Application&lt;/Name&gt;
	///			&lt;NewVersion&gt;2.1.3&lt;/NewVersion&gt;
	///			&lt;LastOldVersion&gt;2.0.0&lt;/LastOldVersion&gt;
	///			&lt;SetupFile&gt;\\Smathersnt12\DLCdocs\ScanQC\People\Mark\SetupFiles\DLC FTP Application\Upgrade Only\Setup.exe&lt;/SetupFile&gt;
	///			&lt;Mandatory&gt;true&lt;/Mandatory&gt;
	///		&lt;/Application&gt;
	///		&lt;Application&gt;
	///			&lt;Name&gt;DLC TimeTracker&lt;/Name&gt;
	///			&lt;NewVersion&gt;1.1.5&lt;/NewVersion&gt;
	///			&lt;LastOldVersion&gt;1.0.0&lt;/LastOldVersion&gt;
	///			&lt;SetupFile&gt;\\Smathersnt12\DLCdocs\ScanQC\People\Mark\SetupFiles\DLC TimeTracker\Upgrade Only\Setup.exe&lt;/SetupFile&gt;
	///			&lt;Mandatory&gt;false&lt;/Mandatory&gt;
	///		&lt;/Application&gt;
	///	&lt;/VersionChecker&gt;
	/// </code>
	/// <br /> <br /> EXAMPLE 2: Below is an example of a program which performs this version check.
	/// <code>
	/// <SPAN class="lang">[C#]</SPAN> 
	///	using System;
	///	using UF.Libraries.DLC.CustomTools;
	///
	///	namespace UF.Libraries.DLC.CustomTools
	///	{
	///		public class Startup
	///		{
	///			static void Main() 
	///			{
	///				// Create a version checker to see if this is the latest version
	///				VersionChecker versionChecker = new VersionChecker();
	///
	///				// Update this application, as necessary
	///				versionChecker.UpdateAsNecessary();
	///
	///				// Only continue if this application is not being updated
	///				if ( !versionChecker.Updating )
	///				{
	///					// Now, if an error was encountered anywhere, show an error message
	///					if ( versionChecker.Error )
	///						MessageBox.Show("An error was encountered while performing the routine Version check.              \n\n" +
	///							"Your application may not be the most recent version.","Version Check Error", MessageBoxButtons.OK, 
	///							MessageBoxIcon.Warning  );
	///
	///					// Now, launch the actual work
	///					Application.Run( new MainForm() );
	///				}
	///			}
	///		}
	///	} 
	/// </code> </example>
	public class VersionChecker
	{
	    /// <summary> Private boolean flag indicates if an error occurred during any of 
		/// the processed. </summary>
		private bool error;

		/// <summary> Flag indicates if the update() function has been called. </summary>
		private bool updating;

	    /// <summary> Private DataTable class holds the information from the XML versioning
	    /// database to verify versioning against. </summary>
	    private readonly DataTable versioning;

	    /// <summary> Constructor for a new VersionChecker object. </summary>
		/// <example> To see examples, look at the examples listed under the main <see cref="VersionChecker"/> class. </example>
		public VersionChecker()
		{
            // Perform in try/catch
            try
            {
                if (VersionConfigSettings.VersionCheckingXML.IndexOf("http:") >= 0)
                {
                    // Get this HTML page
                    string html = GetHtmlPage(VersionConfigSettings.VersionCheckingXML).Trim();

                    DataSet versionSet = new DataSet();
                    versionSet.ReadXml(new StringReader(html));
                    versioning = versionSet.Tables[0];
                }
                else
                {
                    // POPULATE THE PRIVATE DATA TABLE
                    // Create the XmlDataDocument to read the data from the XML database
                    DataSet versionSet = new DataSet();
                    versionSet.ReadXml(VersionConfigSettings.VersionCheckingXML + "\\VersioningDB.xml");

                    // Pull out the Data Table and assign it to the local variable
                    versioning = versionSet.Tables[0];
                }

                // SET ERROR FLAG TO DEFAULT OF FALSE
                error = false;
                updating = false;
            }
            catch
            {
                // Set the error flag to true
                error = true;
                updating = false;
            }
		}

	    /// <summary> Returns TRUE if an error occurred anywhere, otherwise FALSE. </summary>
		/// <example> To see examples, look at the examples listed under the main <see cref="VersionChecker"/> class. </example>
		public bool Error
		{
			get	{	return error;	}
		}

		/// <summary> Returns TRUE if the application is updating, otherwise FALSE.  </summary>
		/// <example> To see examples, look at the examples listed under the main <see cref="VersionChecker"/> class. </example>
		public bool Updating
		{
			get	{	return updating;	}
		}

	    private String GetHtmlPage(string strURL)
	    {
	        // the html retrieved from the page
	        string strResult = String.Empty;
	        WebRequest objRequest = WebRequest.Create(strURL);
	        WebResponse objResponse = objRequest.GetResponse();
	        Stream objStream = objResponse.GetResponseStream();
            if (objStream != null)
            {
                // the using keyword will automatically dispose the object once complete
                using (StreamReader sr = new StreamReader(objStream))
                {
                    strResult = sr.ReadToEnd();
                    // Close and clean up the StreamReader
                    sr.Close();
                }
            }
            return strResult;
	    }

	    /// <summary> Checks to see if a later version of the calling application exists. </summary>
		/// <returns> TRUE if this is not the latest version </returns>
		/// <example> To see examples, look at the examples listed under the main <see cref="VersionChecker"/> class. </example>
		public bool UpdateExists( )
		{
			// Perform in try/catch
			try
			{
			    // Pull out the matching row from the data table. Very little error checking
				// is done here, since any error should count as a general error.
				DataRow[] selected = versioning.Select("Name = '" + VersionConfigSettings.AppName + "'");
			    return !VersionConfigSettings.AppVersion.Equals( selected[0]["NewVersion"].ToString() );
			}
			catch
			{
				// Set the error flag
				error = true;
				return false;
			}
		}

		/// <summary> Checks to see if any update for this is mandatory or not. </summary>
		/// <returns> TRUE if there is a mandatory update available </returns>
		/// <example> To see examples, look at the examples listed under the main <see cref="VersionChecker"/> class. </example>
		public bool UpdateMandatory( )
		{
			// Perform in try/catch
			try
			{
				// Pull out the matching row from the data table. Very little error checking
				// is done here, since any error should count as a general error.
				DataRow[] selected = versioning.Select("Name = '" + VersionConfigSettings.AppName + "'");
				bool mandatory = Convert.ToBoolean( selected[0]["Mandatory"] );

				// If the upgrade is mandatory, no point performing any more version checking
				if ( mandatory )
					return true;

				// Now, ensure the current version is not even older than the Last Old Version.
				// In this case, it WILL be necessary to upgrade
				double thisVersion = 0, lastVersion = 0;
				string[] parser = VersionConfigSettings.AppVersion.Split(".".ToCharArray());
				for ( int i = 0 ; i < parser.Length ; i++ )
					thisVersion += (Convert.ToInt32(parser[i]) * ( 1000^i ));
				parser = selected[0]["LastOldVersion"].ToString().Split(".".ToCharArray());
				for ( int i = 0 ; i < parser.Length ; i++ )
					lastVersion += (Convert.ToInt32(parser[i]) * ( 1000^i ));

				// Now see if this version is older than the last version
				return thisVersion < lastVersion;
			}
			catch
			{
				// Set the error flag
				error = true;
				return false;
			}
		}

		/// <summary> Performs all of the querying to determine if this application needs to be
		/// updated, queries the user, and updates as necessary. </summary>
		/// <returns> TRUE if it is updating, otherwise FALSE </returns>
		/// <example> To see examples, look at the examples listed under the main <see cref="VersionChecker"/> class. </example>
		public bool UpdateAsNecessary()
		{
			// Check to see if an update exists
			if ( UpdateExists() )
			{
				// LDetermine if the update is mandatory or not 
				if ( UpdateMandatory() )
				{
					MessageBox.Show("There is a new version of this application which must be installed.      \n\n"
						+ "Please stand by as the installation software is launched.        \n\nIf you are not an administrator on this machine, please ask your supervisor to update this.       ", "New Version Needed", 
						MessageBoxButtons.OK, MessageBoxIcon.Information);

					// Start the Setup files to update this application and then exit
					Update();
				}
				else
				{
					// A non-mandatory update exists
					DialogResult update = MessageBox.Show("A newer version of this software is available.            \n\nWould you like to upgrade now?",
						"New Version Available", MessageBoxButtons.YesNo, MessageBoxIcon.Question );
					if ( update.Equals(DialogResult.Yes ) )
					{					
						// Start the Setup files to update this application and then exit
						Update();
					}
				}
			}

			// Return the updating flag
			return updating;
		}


		/// <summary> Runs the MSI file for this application from the network location. </summary>
		/// <remarks> The calling application should quit immediately after calling this. </remarks>
		/// <example> To see examples, look at the examples listed under the main <see cref="VersionChecker"/> class. </example>
		public void Update( )
		{
			// Set the updating flag to true
			updating = true;

			// Perform in try/catch
			try
			{
				// Pull out the matching row from the data table. Very little error checking
				// is done here, since any error should count as a general error.
				DataRow[] selected = versioning.Select("Name = '" + VersionConfigSettings.AppName + "'");
				string setupApp = selected[0]["SetupFile"].ToString();

				// Create the process to run the Setup files
				Process toRun = new Process();
				ProcessStartInfo psI = new ProcessStartInfo( setupApp );
				toRun.StartInfo = psI;
				toRun.Start();
			}
			catch
			{
				// Set the error flag
				error = true;
			}
		}

		/// <summary> Method used to build the initial XML and XML schemas. </summary>
		/// <remarks> This remains here just in case it is ever needed again </remarks>
		/// <example> To see examples, look at the examples listed under the main <see cref="VersionChecker"/> class. </example>
		private void CreateXML( )
		{
			// Create a DataSet, namespace, and version table
		    DataSet ds = new DataSet("VersionChecker") {Namespace = "DLC"};
		    DataTable onlyTable = new DataTable("Application");
			onlyTable.Columns.Add( new DataColumn("Name") );
			onlyTable.Columns.Add( new DataColumn("NewVersion") );
			onlyTable.Columns.Add( new DataColumn("LastOldVersion") );
			onlyTable.Columns.Add( new DataColumn("SetupFile") );
			onlyTable.Columns.Add( new DataColumn("Mandatory", Type.GetType( "System.Boolean") ) );
			ds.Tables.Add( onlyTable );

			// Add row 1 to the table
			DataRow newRow = onlyTable.NewRow();
			newRow["Name"] = "DLC CD Manager";
			newRow["NewVersion"] = "1.0.1";
			newRow["LastOldVersion"] = "1.0.0";
			newRow["SetupFile"] = @"\\Smathersnt12\DLCdocs\ScanQC\People\Mark\SetupFiles\DLC CD Manager\Setup.exe";
			newRow["Mandatory"] = true;
			onlyTable.Rows.Add( newRow );

			// Add a 2nd row to the table
			newRow = onlyTable.NewRow();
			newRow["Name"] = "DLC FTP Application";
			newRow["NewVersion"] = "1.1.1";
			newRow["LastOldVersion"] = "1.1.0";
			newRow["SetupFile"] = @"\\Smathersnt12\DLCdocs\ScanQC\People\Mark\SetupFiles\DLC FTP\Setup.exe";
			newRow["Mandatory"] = false;
			onlyTable.Rows.Add( newRow );

			// Accept the changes to the dataset
			ds.AcceptChanges();

			// Create a StreamWriter to write out the XML, write it out, and close
			StreamWriter myXmlWriter = new StreamWriter( VersionConfigSettings.VersionCheckingXML + "\\VersioningDB.xml" );
			ds.WriteXml( myXmlWriter );
			myXmlWriter.Close();

			// Create a StreamWriter to write out the XML Schema, write it out, and close
			StreamWriter schemaWriter = new StreamWriter( VersionConfigSettings.VersionCheckingXML + "\\VersioningDB.xsd");
			ds.WriteXmlSchema( schemaWriter );
			schemaWriter.Close();
		}
	}
}
