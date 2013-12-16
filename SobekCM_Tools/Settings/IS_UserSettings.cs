#region Using directives

using System;
using System.Data;
using System.IO;
using System.IO.IsolatedStorage;

#endregion

namespace SobekCM.Tools.Settings
{
	/// <summary> IS_UserSettings is an abstract, static class which uses Isolated Storage and XML to
	/// store and retrive the settings for an application's user.
	/// </summary>
	/// <remarks> Object created by Mark V Sullivan (2004) for University of Florida's Digital Library Center. </remarks>
	public abstract class IS_UserSettings
	{
	    /// <summary> DataRow which holds the specific values </summary>
		private static DataRow userSettings;

		/// <summary> Name of the file which stores these settings </summary>
		private static string fileName;

		/// <summary> Static constructor for the IS_UserSettings base class </summary>
		static IS_UserSettings()
		{
			// Create a new, empty dataset
			Create_DataSet();
		}

	    /// <summary> Gets and sets the name of the file which stores the user settings </summary>
	    protected static string FileName
	    {
	        get	{	return fileName;	}
	        set	{	fileName = value.Replace(".xml","");	}
	    }

	    /// <summary> Gets the dataset which contains all the values for this this Isolated Storage saved user setting file </summary>
	    protected static DataSet Setting_DataSet { get; private set; }

	    /// <summary> Method creates a new DataSet to house the user information </summary>
		private static void Create_DataSet()
		{
			// Declare the data set new
			Setting_DataSet = new DataSet( "Settings" );

			// Create a new table to store the row of user information
			DataTable dsTbl = new DataTable( "userSettings" );

			// Add this table to the data set
			Setting_DataSet.Tables.Add( dsTbl );

			// Add a new row to the table
			userSettings = dsTbl.NewRow();
			dsTbl.Rows.Add( userSettings );
		}

		/// <summary> Reads the user settings from a XML file in Isolated Storage </summary>
		/// <returns> TRUE if the XML file already existed, otherwise FALSE </returns>
		/// <remarks> If the XML file does not exist, an empty DataSet is created </remarks>
		protected static bool Read_XML_File( )
		{
			// Create the isolated storage file to load the information from the disk
		    IsolatedStorageFile userSettingFile = IsolatedStorageFile.GetUserStoreForAssembly();

			// Look to see if the file exists
			string[] files = userSettingFile.GetFileNames( fileName + ".xml" );
			if ( files.Length > 0 )
			{
				// Create a stream reader to get the data from the isolated storage for this user
				StreamReader stmReader = new StreamReader( new IsolatedStorageFileStream( fileName + ".xml", FileMode.Open, userSettingFile ) );

				// Read the xml file
				Setting_DataSet = new DataSet();
				Setting_DataSet.ReadXml( stmReader, XmlReadMode.ReadSchema );

				// Fetch the one line of data
				userSettings = Setting_DataSet.Tables[0].Rows[0];

				// Close the stream
				stmReader.Close();

				// Close the connection to the isolated storage
				userSettingFile.Close();

				// Return true since the file already existed
				return true;
			}

		    // The file did not exist yet, so create a data set locally
		    Create_DataSet();

		    // Close the connection to the isolated storage
		    userSettingFile.Close();

		    // Return false since the file did not already exist
		    return false;
		}

		/// <summary> Reads the user settings from a XML file in Isolated Storage </summary>
		/// <param name="IS_FileName"> Name of the file </param>
		/// <returns> TRUE if the XML file already existed, otherwise FALSE </returns>
		/// <remarks> If the XML file does not exist, an empty DataSet is created </remarks>
		protected static bool Read_XML_File( string IS_FileName )
		{
			// Save the file name
			FileName = IS_FileName;

			// Call the 'base' method
			return Read_XML_File();
		}

		/// <summary> Writes the user settings to a XML file in Isolated Storage </summary>
		/// <returns> TRUE if the XML file is successfully written, otherwise FALSE </returns>
		protected static bool Write_XML_File( )
		{
			// Only write the XML file if there are columns to write
			if (( Setting_DataSet.Tables.Count > 0 ) && ( Setting_DataSet.Tables[0].Columns.Count > 0 ))
			{
				// Perform this work in a try/catch architecture
				try
				{
					// Create the isolated storage file to write the information to the disk
				    IsolatedStorageFile userSettingFile = IsolatedStorageFile.GetUserStoreForAssembly();

					// Create a stream writer to write to the file in isolated storage
					StreamWriter stmWriter = new StreamWriter( new IsolatedStorageFileStream( fileName + ".xml", FileMode.Create, userSettingFile ) );

					// Write the XML file
					Setting_DataSet.WriteXml( stmWriter, XmlWriteMode.WriteSchema );

					// Close these objects
					stmWriter.Close();
					userSettingFile.Close();

					// Successful, so return true
					return true;
				}
				catch 
				{
					return false;
				}
			}

		    // Since there were no settings to save, return true, but do nothing
		    return true;
		}


		/// <summary> Writes the user settings to a XML file in Isolated Storage </summary>
        /// <param name="IS_FileName"> Name of the file </param>
		/// <returns> TRUE if the XML file is successfully written, otherwise FALSE </returns>
		protected static bool Write_XML_File( string IS_FileName )
		{
			// Save the file name
            FileName = IS_FileName;

			// Call the 'base' method
			return Write_XML_File();
		}

	    /// <summary> Set a value in the current user setting. </summary>
		/// <param name="SettingName"> Name of the setting </param>
		/// <param name="newValue"> New value for the setting </param>
		/// <remarks> If the setting name already exists, the value will be changed
		/// to match the new value. </remarks>
		protected static void Add_Setting( string SettingName, int newValue )
		{
			// Check to see if the setting already exists
			if ( !Setting_DataSet.Tables[0].Columns.Contains( SettingName ) )
			{
				// Add this colume to the table
				Setting_DataSet.Tables[0].Columns.Add( SettingName );
			}

			// Add this value to the only row in the table
			userSettings[ SettingName ] = newValue;
		}

		/// <summary> Set a value in the current user setting. </summary>
		/// <param name="SettingName"> Name of the setting </param>
		/// <param name="newValue"> New value for the setting </param>
		/// <remarks> If the setting name already exists, the value will be changed
		/// to match the new value. </remarks>
		protected static void Add_Setting( string SettingName, string newValue )
		{
			// Check to see if the setting already exists
			if ( !Contains( SettingName ) )
			{
				// Add this colume to the table
				Setting_DataSet.Tables[0].Columns.Add( SettingName );
			}

			// Add this value to the only row in the table
			userSettings[ SettingName ] = newValue;
		}

		/// <summary> Gets a pre-existing integer setting for this user  </summary>
		/// <param name="SettingName"> Name of the setting to fetch </param>
		/// <returns> Value of the integer setting, or -1 if the setting was not found </returns>
		protected static int Get_Int_Setting( string SettingName )
		{
		    if (Contains(SettingName))
            {
                string string_value = userSettings[SettingName].ToString();
                return Convert.ToInt32(string_value);
            }

		    return -1;
		}

	    /// <summary> Gets a pre-existing string setting for this user  </summary>
		/// <param name="SettingName"> Name of the setting to fetch </param>
		/// <returns> Value of the string setting, or an empty string if the setting was not found </returns>
		protected static string Get_String_Setting( string SettingName )
	    {
	        return Contains( SettingName ) ? userSettings[ SettingName ].ToString() : "";
	    }

	    /// <summary> Checks to see if a particular setting already exists for this user </summary>
		/// <param name="SettingName"> Name of the setting to look for </param>
		/// <returns> TRUE if the setting exists, otherwise FALSE </returns>
		protected static bool Contains( string SettingName )
		{
			return ( Setting_DataSet.Tables[0].Columns.Contains( SettingName ) );
		}
	}
}



