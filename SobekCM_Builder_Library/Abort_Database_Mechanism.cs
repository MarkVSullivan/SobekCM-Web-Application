#region Using directives

using System.Collections.Generic;
using SobekCM.Library.Database;

#endregion

namespace SobekCM.Builder_Library
{
	/// <summary> Enumeration which reflects a possible state for the builder operation flag </summary>
	public enum Builder_Operation_Flag_Enum : byte
	{
		/// <summary> Standard operation </summary>
		STANDARD_OPERATION = 1,

		/// <summary> Pause has been requested, so don't abort, but don't do anything either </summary>
		PAUSE_REQUESTED,

		/// <summary> Abort has been requested, but not yet fulfilled </summary>
		ABORT_REQUESTED,

		/// <summary> An abort request is currently being handled </summary>
		ABORTING,

		/// <summary> Building is temporarily suspended </summary>
		NO_BUILDING_REQUESTED,

		/// <summary> The previous exection was aborted </summary>
		LAST_EXECUTION_ABORTED
	}

	/// <summary> Class is used to check for a builder abort request via a database flag </summary>
	public static class Abort_Database_Mechanism
	{
		private static readonly string setting_key;

		/// <summary> Static constructor for the Abort_Database_Mechanism class  </summary>
		/// <remarks> This constructor sets the settings key to check for, but does no other preparation </remarks>
		static Abort_Database_Mechanism()
		{
			 setting_key = "Builder Operation Flag";
		}

		/// <summary> Checks the database for a new abort flag or sets the flag to a corresponding enumerational value </summary>
		public static Builder_Operation_Flag_Enum Builder_Operation_Flag
		{
			get
			{
				Dictionary<string, string> builder_settings = SobekCM_Database.Get_Settings(null);
				if (builder_settings.ContainsKey(setting_key))
				{
					switch( builder_settings[setting_key].ToUpper().Replace("_"," "))
					{
						case "ABORT REQUESTED":
							return Builder_Operation_Flag_Enum.ABORT_REQUESTED;
						case "PAUSE REQUESTED":
							return Builder_Operation_Flag_Enum.PAUSE_REQUESTED;
						case "ABORTING":
							return Builder_Operation_Flag_Enum.ABORTING;
						case "NO BUILDING REQUESTED":
							return Builder_Operation_Flag_Enum.NO_BUILDING_REQUESTED;
						case "LAST EXECUTION ABORTED":
							return Builder_Operation_Flag_Enum.LAST_EXECUTION_ABORTED;
						default:
							return Builder_Operation_Flag_Enum.STANDARD_OPERATION;
					}
				}
				return Builder_Operation_Flag_Enum.STANDARD_OPERATION;
			}
			set
			{
				string newValue = "STANDARD OPERATION";
				switch (value)
				{
					case Builder_Operation_Flag_Enum.ABORT_REQUESTED:
						newValue = "ABORT REQUESTED";
						break;

					case Builder_Operation_Flag_Enum.PAUSE_REQUESTED:
						newValue = "PAUSE REQUESTED";
						break;

					case Builder_Operation_Flag_Enum.ABORTING:
						newValue = "ABORTING";
						break;

					case Builder_Operation_Flag_Enum.NO_BUILDING_REQUESTED:
						newValue = "NO BUILDING REQUESTED";
						break;

					case Builder_Operation_Flag_Enum.LAST_EXECUTION_ABORTED:
						newValue = "LAST EXECUTION ABORTED";
						break;
				}
				SobekCM_Database.Set_Setting(setting_key, newValue);

			}
		}

		/// <summary> Returns a flag indicating if an abort was requested </summary>
		/// <returns> TRUE if the flag is currently ABORT REQUESTED, ABORTING, or NO BUILDER REQUESTED</returns>
		public static bool Abort_Requested()
		{
			Dictionary<string, string> builder_settings = SobekCM_Database.Get_Settings(null);
			if (builder_settings.ContainsKey(setting_key))
			{
				switch (builder_settings[setting_key].ToUpper().Replace("_", " "))
				{
					case "ABORT REQUESTED":
					case "ABORTING":
					case "NO BUILDING REQUESTED":
						return true;
				}
			}

			return false;
		}
	}
}
