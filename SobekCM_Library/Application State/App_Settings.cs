using System;

namespace SobekCM_Library.Application_State
{
	/// <summary> Class stores basic information about this instance of the application and server information </summary>
	public class App_Settings
	{
        /// <summary> Base directory where the ASP.net application is running on the application server </summary>
		public readonly string Base_Dir;

        /// <summary> Base URL which points to the ASP.net application on the application server </summary>
		public readonly string Base_URL;

        /// <summary> URL to the Aware JPEG2000 zoomable image server </summary>
		public readonly string JP2_Server;

        /// <summary> Temporary location used while online submittors build a digital resource for submission </summary>
        public readonly string In_Process_Submission_Location;

		private string greenstone_base_url;
		private string image_url;

        /// <summary> Constructor for a new instance of the App_Settings class </summary>
        /// <param name="Base_Dir"> Base directory where the ASP.net application is running on the application server </param>
        /// <param name="Base_URL"> Base URL which points to the ASP.net application on the application server </param>
        /// <param name="Database_String"> Database connection string is built from the application config file </param>
        /// <param name="App_Name"> Name of this application </param>
        /// <param name="JP2_Server"> URL to the Aware JPEG2000 zoomable image server </param>
        /// <param name="In_Process_Submission_Location"> Temporary location used while online submittors build a digital resource for submission </param>
        public App_Settings(string Base_Dir, string Base_URL, string Database_String, string App_Name, string JP2_Server, string In_Process_Submission_Location)
		{
			// Save all of these values
			this.Base_Dir = Base_Dir;
			this.Base_URL = Base_URL;
			this.Database_String = Database_String;
			this.App_Name = App_Name;
			this.JP2_Server = JP2_Server;
            this.In_Process_Submission_Location = In_Process_Submission_Location;

			// Set some defaults
			greenstone_base_url = String.Empty;
			image_url = String.Empty;
		}

        /// <summary> Directory for this application's DESIGN folder, where all the aggregation and interface folders reside </summary>
        /// <value> [Base_Dir] + 'design\' </value>
		public string Base_Design_Location
		{
			get	{	return Base_Dir + "design\\";  }
		}

        /// <summary> Directory for this application's myUFDC folder, where the template and project files reside for online submittal and editing</summary>
        /// <value> [Base_Dir] + 'myUFDC\' </value>
        public string Base_MyUFDC_Location
        {
            get { return Base_Dir + "myUFDC\\"; }
        }

        /// <summary> Directory for this application's DATA folder, where the OAI source files reside </summary>
        /// <value> [Base_Dir] + 'design\' </value>
        public string Base_Data_Location
        {
            get { return Base_Dir + "data\\"; }
        }

        /// <summary> Base greenstone URL for all searches </summary>
		public string Greenstone_Base_URL
		{
			get	{	return greenstone_base_url;		}
		}

        /// <summary> Base image URL for all digital resource images </summary>
		public string Image_URL
		{
			get	{	return image_url;		}
		}

        /// <summary> Sets the greenstone root and image root for this application server </summary>
        /// <param name="Greenstone_Root"> Root URL for the greenstone server for all searches </param>
        /// <param name="Image_Root"> Root URL for the image server with all the digital resource images</param>
		public void Set_Server_URLs( string Greenstone_Root, string Image_Root )
		{
            if (Greenstone_Root.Length > 0)
            {
                if (Greenstone_Root[Greenstone_Root.Length - 1] != '/')
                    greenstone_base_url = Greenstone_Root + "/cgi-bin/library?";
                else
                    greenstone_base_url = Greenstone_Root + "cgi-bin/library?";
            }
            image_url = Image_Root;
		}
	}
}
