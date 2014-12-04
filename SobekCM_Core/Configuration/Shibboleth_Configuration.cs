#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Core.Configuration
{
 
	/// <summary> Static class contains all the Shibboleth configuration details from the configuration file
	/// as well as methods to do read the configuration file </summary>
	public class Shibboleth_Configuration
	{
        
		private Dictionary<string, User_Object_Attribute_Mapping_Enum> attributeMapping;

	    private List<KeyValuePair<string, string>> canSubmitIndicators;

        /// <summary> Constructor for a new instance of the Shibboleth_Configuration class </summary>
		public Shibboleth_Configuration()
		{
			attributeMapping = new Dictionary<string, User_Object_Attribute_Mapping_Enum>();
			Constants = new List<KeyValuePair<User_Object_Attribute_Mapping_Enum, string>>();
			canSubmitIndicators = new List<KeyValuePair<string, string>>();

			Config_File_Read = false;
			UserIdentityAttribute = String.Empty;
			ShibbolethURL = String.Empty;
			Label = String.Empty;
            Debug = false;
            Enabled = true;
		}

        /// <summary> Flag indicates if this is set to DEBUG mode, in which case data is written to the trace route
        /// during each check, or Shibboleth authentication </summary>
        public bool Debug { get; set;  }

        /// <summary> Flag indicates if Shibboleth authentication is currently enabled </summary>
        public bool Enabled { get; set; }

        /// <summary> Error message in case there was an issue pulling this configuration from the configuration files </summary>
        public string Error { get; set; }

		/// <summary> FLag indicates if a configuration file was read (or an attempt to read, even if 
		/// the file does not exists, meaning Shibboleth is not configured ) </summary>
		public bool Config_File_Read { get; set;  }

		/// <summary> Primary attribute from the Shibboleth cookie which identifies the user uniquely </summary>
		/// <remarks> For example, this is the UFID attribute from the cookie for UFDC </remarks>
		public string UserIdentityAttribute { get; set; }

		/// <summary> URL for the instance of Shibboleth </summary>
		/// <remarks> This can contain "[%TARGET%]", in which case the system will set the target programmatically </remarks>
		public string ShibbolethURL { get; set; }

		/// <summary> Label for this type of authentication, to be displayed to user during logon </summary>
		/// <remarks> For example, 'Gatorlink' for UFDC, 'UK Federation', etc.. </remarks>
		public string Label { get; set; }

        /// <summary> Add a new mapping between a server variable returned from Shibboleth and a user attribute within the SobekCM user object </summary>
        /// <param name="ServerVariable"> Server variable from the Shibboleth response </param>
        /// <param name="UserAttribute"> Attribute within the SobekCM user object </param>
	    public void Add_Attribute_Mapping(string ServerVariable, User_Object_Attribute_Mapping_Enum UserAttribute)
	    {
	        attributeMapping[ServerVariable] = UserAttribute;
	    }

        /// <summary> Add a new constant mapping for all new users that are established using this Shibboleth authentication system </summary>
        /// <param name="UserAttribute"> Attribute within the SobekCM user object </param>
        /// <param name="ConstantValue"> Constant value to apply for all new Shibboleth users established using this Shibboleth authenticaion system </param>
	    public void Add_Constant(User_Object_Attribute_Mapping_Enum UserAttribute, string ConstantValue)
	    {
	        Constants.Add(new KeyValuePair<User_Object_Attribute_Mapping_Enum, string>(UserAttribute, ConstantValue ));
	    }

        /// <summary> Add a new indicator that a new user established using this Shibboleth authenticaion system can submit items </summary>
        /// <param name="ServerVariable"> Server variable from the Shibboleth response </param>
        /// <param name="RequiredValue"> Value to match - if the value matches, then the new user should be granted submit rights </param>
        public void Add_CanSubmit_Indicator(string ServerVariable, string RequiredValue)
	    {
            canSubmitIndicators.Add(new KeyValuePair<string, string>(ServerVariable, RequiredValue));
	    }

        /// <summary> Get the mapping from the server variable into the new user object </summary>
        /// <param name="ServerVariable"> Name from the server variable </param>
        /// <returns> Mapping into the user object ( or NONE ) </returns>
	    public User_Object_Attribute_Mapping_Enum Get_User_Object_Mapping(string ServerVariable)
	    {
	        if (attributeMapping.ContainsKey(ServerVariable))
	            return attributeMapping[ServerVariable];
	        return User_Object_Attribute_Mapping_Enum.NONE;
	    }

        /// <summary> List of all the constants to assign to a new user </summary>
	    public List<KeyValuePair<User_Object_Attribute_Mapping_Enum, string>> Constants { get; private set; }

	    #region Code to save this shibboleth configuration to a XML file

        /// <summary> Save this quality control configuration to a XML config file </summary>
        /// <param name="FilePath"> File/path for the resulting XML config file </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_To_Config_File(string FilePath)
        {
            bool returnValue = true;
            StreamWriter writer = null;
            try
            {
                // Start the output file
                writer = new StreamWriter(FilePath, false, Encoding.UTF8);
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                writer.WriteLine("<SobekCM_Config xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
                writer.WriteLine("\txmlns=\"http://sobekrepository.org/schemas/sobekcm_config\" ");
                writer.WriteLine("\txsi:schemaLocation=\"http://sobekrepository.org/schemas/sobekcm_config ");
                writer.WriteLine("\t\thttp://sobekrepository.org/schemas/sobekcm_config.xsd\">");
                writer.WriteLine("\t<Authentication>");
                writer.Write("\t\t<Shibboleth");
                if ( !String.IsNullOrEmpty(UserIdentityAttribute)) writer.Write(" UserIdentityAttribute=\"" + UserIdentityAttribute + "\"");
                if (!String.IsNullOrEmpty(ShibbolethURL)) writer.Write(" URL=\"" + ShibbolethURL + "\"");
                if (!String.IsNullOrEmpty(Label)) writer.Write(" Label=\"" + Label + "\"");
                writer.Write(" Enabled=\"" + Enabled.ToString().ToLower() + "\"");
                if (Debug) writer.Write(" Debug=\"true\"");
                writer.WriteLine(">");

                // Add the attribute matching
                if (( attributeMapping != null ) && ( attributeMapping.Count > 0))
                {
                    writer.WriteLine("\t\t\t<AttributeMatching>");

                    foreach (KeyValuePair<string, User_Object_Attribute_Mapping_Enum> thisMatch in attributeMapping)
                    {
                        writer.WriteLine("\t\t\t\t<Mapping ServerVariable=\"" + thisMatch.Key + "\" UserAttribute=\"" + User_Object_Attribute_Mapping_Enum_Converter.ToString( thisMatch.Value ) + "\" />");
                    }

                    writer.WriteLine("\t\t\t</AttributeMatching>");
                }

                // Add the constants
                if ((Constants != null) && (Constants.Count > 0))
                {
                    writer.WriteLine("\t\t\t<Constants>");

                    foreach (KeyValuePair<User_Object_Attribute_Mapping_Enum, string> thisConstant in Constants)
                    {
                        writer.WriteLine("\t\t\t\t<Constant UserAttribute=\"" + User_Object_Attribute_Mapping_Enum_Converter.ToString(thisConstant.Key) + "\">" + Convert_String_To_XML_Safe(thisConstant.Value) + "</Constant>");
                    }

                    writer.WriteLine("\t\t\t</Constants>");
                }

                // Add the logic portions
                if ((canSubmitIndicators != null) && (canSubmitIndicators.Count > 0))
                {
                    writer.WriteLine("\t\t\t<Logic>");

                    foreach (KeyValuePair<string, string> canSubmitIndicator in canSubmitIndicators)
                    {
                        writer.WriteLine("\t\t\t\t<CanSubmit ServerVariable=\"" + canSubmitIndicator.Key + "\" Value=\"" + canSubmitIndicator.Value + "\" />");
                    }

                    writer.WriteLine("\t\t\t</Logic>");
                }

                writer.WriteLine("\t\t</Shibboleth>");
                writer.WriteLine("\t</Authentication>");
                writer.WriteLine("</SobekCM_Config>");
                writer.Flush();
                writer.Close();
            }
            catch (Exception ee)
            {
                returnValue = false;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }

            return returnValue;
        }

        /// <summary> Converts a basic string into an XML-safe string </summary>
        /// <param name="element"> Element data to convert </param>
        /// <returns> Data converted into an XML-safe string</returns>
        private static string Convert_String_To_XML_Safe(string element)
        {
            if (element == null)
                return string.Empty;

            string xml_safe = element;
            int i = xml_safe.IndexOf("&");
            while (i >= 0)
            {
                if ((i != xml_safe.IndexOf("&amp;", i)) && (i != xml_safe.IndexOf("&quot;", i)) &&
                    (i != xml_safe.IndexOf("&gt;", i)) && (i != xml_safe.IndexOf("&lt;", i)))
                {
                    xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                }

                i = xml_safe.IndexOf("&", i + 1);
            }
            return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        #endregion
	}
}
