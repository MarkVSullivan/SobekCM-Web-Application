using System.Collections.Generic;
using System.IO;

namespace SobekCM.Library.Localization
{
	/// <summary> Abstract class which all single class localization files extends </summary>
	public abstract class baseLocalizationInfo
	{
		/// <summary> Dictionary to hold all the values </summary>
		protected Dictionary<string, string> OtherValues;

		/// <summary> Name of the class this localization file serves </summary>
		protected string ClassName;

		/// <summary> Constructor for the abstract class baseLocalizationInfo </summary>
		protected baseLocalizationInfo()
		{
			OtherValues = new Dictionary<string, string>();
		}

		/// <summary> Return any value from the dictionary of terms within this localization class </summary>
		/// <param name="Key"> Key for the value to retrieve from the dictionary </param>
		/// <returns> Either the matching value, or the key itself </returns>
		/// <remarks> This is used for quickly extending the localization strings for a single class.  
		/// Generally, the new string should be exposed in a C# property.  </remarks>
		public string this[string Key]
		{
			get { return OtherValues.ContainsKey(Key) ? OtherValues[Key] : Key; }
		}

		/// <summary> Adds a localization string ( with key and value ) to this localization class </summary>
		/// <param name="Key"> Key for the new localization string being saved </param>
		/// <param name="Value"> Value for this localization string </param>
		public virtual void Add_Localization_String(string Key, string Value)
		{
			OtherValues[Key] = Value;
		}

		/// <summary> Writes the localization string information to the localization XML file being built </summary>
		/// <param name="Writer"> Output stream for the resulting XML </param>
		internal void Write_Localization_XML(TextWriter Writer)
		{
			// Get the list of all the keys, sorted
			SortedList<string, string> sortedKeys = new SortedList<string, string>();
			foreach (string thisKey in OtherValues.Keys)
			{
				sortedKeys[thisKey.ToUpper()] = thisKey;
			}

			// Now, write the localization information 
			Writer.WriteLine("\t<Section class=\"" + ClassName + "\">");
			foreach (KeyValuePair<string,string> thisKey in sortedKeys)
			{
				Writer.WriteLine("\t\t<Text key=\"" + Convert_String_To_XML_Safe_Static(thisKey.Value) + "\" value=\"" + Convert_String_To_XML_Safe_Static(OtherValues[thisKey.Value]) + "\" />");
			}

			// Close this single class
			Writer.WriteLine("\t</Section>");
		}

		/// <summary> Converts a basic string into an XML-safe string </summary>
		/// <param name="Element"> Element data to convert </param>
		/// <returns> Data converted into an XML-safe string</returns>
		private static string Convert_String_To_XML_Safe_Static(string Element)
		{
			if (Element == null)
				return string.Empty;

			string xml_safe = Element;
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
	}
}
