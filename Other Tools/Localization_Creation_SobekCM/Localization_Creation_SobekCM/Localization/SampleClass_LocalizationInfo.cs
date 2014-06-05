namespace SobekCM.Library.Localization
{
	/// <summary> Localization class holds all the standard terms utilized by the SampleClass class </summary>
	public class SampleClass_LocalizationInfo : baseLocalizationInfo
	{
		/// <summary> Constructor for a new instance of the SampleClass_Localization class </summary>
		public SampleClass_LocalizationInfo() : base()
		{
			// Do nothing
		}

		/// <summary> Adds a localization string ( with key and value ) to this localization class </summary>
		/// <param name="Key"> Key for the new localization string being saved </param>
		/// <param name="Value"> Value for this localization string </param>
		/// <remarks> This overrides the base class's implementation </remarks>
		public override void Add_Localization_String(string Key, string Value)
		{
			// First, add to the localization string dictionary
			base.Add_Localization_String(Key, Value);

			// Assign to custom properties depending on the key
			switch (Key)
			{
				case "Missing Thumbnail":
					Missing_Thumnbnail = Value;
					break;

				case "Access Restricted":
					Access_Restricted = Value;
					break;
			}
		}

		/// <summary> Text used when displaying an access-restricted item in the results list </summary>
		public string Access_Restricted { get; private set; }

		/// <summary> 'MISSING THUMBNAIL' localization string </summary>
		public string Missing_Thumnbnail { get; private set; }
		
	}
}
