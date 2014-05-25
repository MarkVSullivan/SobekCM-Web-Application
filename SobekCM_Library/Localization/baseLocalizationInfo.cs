using System.Collections.Generic;

namespace SobekCM.Library.Localization
{
	/// <summary> Abstract class which all single class localization files extends </summary>
	public abstract class baseLocalizationInfo
	{
		/// <summary> Dictionary to hold all the values </summary>
		protected Dictionary<string, string> otherValues;

		/// <summary> Constructor for the abstract class baseLocalizationInfo </summary>
		protected baseLocalizationInfo()
		{
			otherValues = new Dictionary<string, string>();
		}

		/// <summary> Return any value from the dictionary of terms within this localization class </summary>
		/// <param name="Key"> Key for the value to retrieve from the dictionary </param>
		/// <returns> Either the matching value, or the key itself </returns>
		/// <remarks> This is used for quickly extending the localization strings for a single class.  
		/// Generally, the new string should be exposed in a C# property.  </remarks>
		public string this[string Key]
		{
			get { return otherValues.ContainsKey(Key) ? otherValues[Key] : Key; }
		}

		/// <summary> Adds a localization string ( with key and value ) to this localization class </summary>
		/// <param name="Key"> Key for the new localization string being saved </param>
		/// <param name="Value"> Value for this localization string </param>
		public virtual void Add_Localization_String(string Key, string Value)
		{
			otherValues[Key] = Value;
		}
	}
}
