using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SobekCM.Library.Users;

namespace SobekCM.Library.Configuration
{
	/// <summary> Static class contains all the Shibboleth configuration details from the configuration file
	/// as well as methods to do read the configuration file </summary>
	public static class Shibboleth_Configuration
	{
		private static Dictionary<string, User_Object_Attribute_Mapping_Enum> attributeMapping;

		private static List<KeyValuePair<User_Object_Attribute_Mapping_Enum, string>> constants;

		private static List<KeyValuePair<string, string>> canSubmitIndicators;

		static Shibboleth_Configuration()
		{
			attributeMapping = new Dictionary<string, User_Object_Attribute_Mapping_Enum>();
			constants = new List<KeyValuePair<User_Object_Attribute_Mapping_Enum, string>>();
			canSubmitIndicators = new List<KeyValuePair<string, string>>();

			Config_File_Read = false;
			UserIdentityAttribute = String.Empty;
			ShibbolethURL = String.Empty;
			Label = String.Empty;
		}

		/// <summary> FLag indicates if a configuration file was read (or an attempt to read, even if 
		/// the file does not exists, meaning Shibboleth is not configured ) </summary>
		public static bool Config_File_Read { get; private set;  }

		/// <summary> Primary attribute from the Shibboleth cookie which identifies the user uniquely </summary>
		/// <remarks> For example, this is the UFID attribute from the cookie for UFDC </remarks>
		public static string UserIdentityAttribute { get; private set; }

		/// <summary> URL for the instance of Shibboleth </summary>
		/// <remarks> This can contain "[%TARGET%]", in which case the system will set the target programmatically </remarks>
		public static string ShibbolethURL { get; private set; }

		/// <summary> Label for this type of authentication, to be displayed to user during logon </summary>
		/// <remarks> For example, 'Gatorlink' for UFDC, 'UK Federation', etc.. </remarks>
		public static string Label { get; private set; }




	}
}
