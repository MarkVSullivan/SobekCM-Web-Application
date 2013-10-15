using System.Collections.Generic;
using System.IO;

namespace SobekCM.Tools
{
	/// <summary> Class contains static methods used for working with files, or
	/// directories of files </summary>
	public static class SobekCM_File_Utilities
	{
		/// <summary> Returns file names from given top folder that comply to given filters </summary>
		/// <param name="SourceFolder">Folder with files to retrieve</param>
		/// <param name="Filters">Multiple file filters separated by | character</param>
		/// <returns>Array of string file names that meet given filters</returns>
		public static string[] GetFiles(string SourceFolder, string Filters)
		{
			return GetFiles(SourceFolder, Filters, SearchOption.TopDirectoryOnly);
		}


		/// <summary> Returns file names from given folder that comply to given filters </summary>
		/// <param name="SourceFolder">Folder with files to retrieve</param>
		/// <param name="Filters">Multiple file filters separated by | character</param>
		/// <param name="SearchOption">File.IO.SearchOption, could be AllDirectories or TopDirectoryOnly</param>
		/// <returns>Array of string file names that meet given filters</returns>
		public static string[] GetFiles(string SourceFolder, string Filters, SearchOption SearchOption)
		{
			// List will hold all file names
			List<string> alFiles = new List<string>();

			// Create an array of filter string
			string[] multipleFilters = Filters.Split('|');

			// for each filter find mathing file names
			foreach (string fileFilter in multipleFilters)
			{
				// add found file names to array list
				alFiles.AddRange(Directory.GetFiles(SourceFolder, fileFilter, SearchOption));
			}

			// returns string array of relevant file names
			return alFiles.ToArray();
		}


	}
}
