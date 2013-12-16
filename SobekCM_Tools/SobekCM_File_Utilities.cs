using System;
using System.Collections.Generic;
using System.IO;

namespace SobekCM.Tools
{
	/// <summary> Class contains static methods used for working with files, or
	/// directories of files </summary>
	public static class SobekCM_File_Utilities
	{
		/// <summary> Dletes a set of folders and files recursively </summary>
		/// <param name="Folder"> Top folder to delete </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		public static bool Delete_Folders_Recursively(string Folder)
		{
			try
			{
				List<string> subdirs = new List<string>();
				recurse_folders(Folder, subdirs);

				foreach (string thisSubFolder in subdirs)
				{
					// Delete all files
					foreach( string thisFile in Directory.GetFiles(thisSubFolder))
						File.Delete(thisFile);

					Directory.Delete(thisSubFolder);
				}
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		private static void recurse_folders(string Folder, List<string> nodes)
		{
			foreach (string subFolder in Directory.GetDirectories(Folder))
			{
				recurse_folders(subFolder, nodes);
			}

			nodes.Add(Folder);
		}

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
