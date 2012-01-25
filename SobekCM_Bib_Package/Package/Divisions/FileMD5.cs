using System;
using System.IO;
using System.Security.Cryptography;

namespace SobekCM.Bib_Package.Divisions
{
	/// <summary>
	/// FileMD5 is an object used to convert a file to a MD5 Checksum string.  
	/// The name of this file is passed in during the construction of this 
	/// object, or by using the property FileName.  Then the Checksum is retrieved
	/// by using the Checksum property.<br />
	/// <br />
	/// </summary>
	/// <remarks>  Object created by Mark V Sullivan (2003) for University of Florida's Digital Library Center.  </remarks>
	/// <example> Below is a simple example to print the MD5 checksum for any file from a Console application. <code>
	/// <SPAN class="lang">[C#]</SPAN> 
	///	using System;
	///	using System.IO;
	///	using CustomTools.MXF;
	///
	///	namespace CustomTools.Examples
	///	{
	///		public class MXF_Example
	///		{
	///			static void Main() 
	///			{
	///				// Read the name of the file to get the MD5 Checksum for
	///				Console.Write("Enter the name of the file: ");
	///				string file = Console.ReadLine();
	///
	///				// Create the MD5 Checksum object
	///				FileMD5 hasher = new FileMD5( file );
	///
	///				// Print out the checksum result, if no error occurred
	///				if ( !hasher.Error )
	///					Console.WriteLine( "\n" + hasher.Checksum );
	///				else
	///					Console.WriteLine("\nError encountered during checksum");
	///
	///				// Wait for the user to hit enter
	///				Console.WriteLine("\nHit Enter to Continue");
	///				Console.ReadLine();
	///			}
	///		}
	///	}
	/// </code>
	/// <br /> This example will write the following to the Console Window:
	/// <code>
	///
	/// Enter the name of the file: E:\test.xml
	///
	/// 8212ec9bec6d90d6002d256398790b83
	///
	/// Hit Enter to Continue
	///
	/// </code></example>
	public class FileMD5
	{

/*=========================================================================
 *		PRIVATE VARIABLE DECLARATIONS of the FileMD5
 *=========================================================================*/

		/// <summary> Private string variable holds the checksum for the current file </summary>
		private string hashResult;

		/// <summary> Private string variable holds the name of the file for
		/// which the current checksum is valid. </summary>
		private string fileName;

		/// <summary> Private bool variable holds the flag which indicates if an 
		/// error occurred during the checksum process. </summary>
		private bool errorFlag;


/*=========================================================================
 *		CONSTRUCTOR(S) of the FileMD5 Class 
 *=========================================================================*/

		/// <summary> Constructor for a new FileMD5 object which accepts the filename
		/// for the first file to be checked. </summary>
		/// <param name="fileName"> Path and filename for the file to check </param>
		public FileMD5( string fileName )
		{
			// Save the fileName parameter
			this.fileName = fileName;

			// Compute the checksum on the first file
			computeChecksum();
		}

		/// <summary> Constructor for a new FileMD5 object. </summary>
		public FileMD5()
		{
			// Declare the fileName and hashResult as empty strings
			hashResult = "";
			fileName = "";
		}

/*=========================================================================
 *		PUBLIC PROPERTIES of the FileMD5 Class 
 *=========================================================================*/

		/// <summary> Gets the checksum for the current file </summary>
		public string Checksum
		{
			get	{	return hashResult;	}
		}

        /// <summary> Computes the checksum for a new file and returns the MD5 checksum </summary>
        /// <param name="File"> Name of the file </param>
        /// <returns> MD5 Checksum as a string </returns>
        public string Calculate_Checksum(string File)
        {
            this.fileName = File;
            computeChecksum();
            return hashResult;
        }

		/// <summary> Gets the error flag to indicate an error 
		/// occurred during the last checksum computation </summary>
		public bool Error
		{
			get	{	return errorFlag;	}
		}

		/// <summary> Gets or sets the name of the current file. </summary>
		public string FileName
		{
			get	{	return fileName;	}
			set
			{
				// Save the new filename and compute the checksum
				fileName = value;
				computeChecksum();
			}
		}

/*=========================================================================
 *		PRIVATE METHODS of the FileMD5 Class 
 *=========================================================================*/

		/// <summary> Private helper method that computes the checksum for the 
		/// current file and set the private hashResult string to the checksum </summary>
		private void computeChecksum()
		{
			// Perform this in a try/catch
			try
			{
				// Open a connection to the file
				FileStream hashFile = new FileStream( fileName, FileMode.Open, FileAccess.Read );

				// Create the object necessary and compute the hash on the file stream
				MD5 md5 = new MD5CryptoServiceProvider();
				byte[] result = md5.ComputeHash( hashFile );

				// Close the connection to the file
				hashFile.Close();

		
				// Need to convert the byte array into a hex string.  Declare some variables
				// and then iterate through each byte from the 128bit result.
				hashResult = "";
				double byteAsInteger;
				for ( int i = 0 ; i < result.Length ; i++ )
				{
					// Convert the byte to an integer
					byteAsInteger = Convert.ToDouble( result[i] );

					// Convert the integer to hex
					hashResult += GetHex(Math.Floor( byteAsInteger / 16)) + GetHex( byteAsInteger % 16);
				}

				// This result occurred without an error, so set the error flag to false
				errorFlag = false;
			}
			catch
			{
				// There was some error, so set the result to "ERROR" and set the error flag
				hashResult = "ERROR";
				errorFlag = true;
			}
		}

		/// <summary> Private helper method returns the appropriate hex value for the 
		/// double parameter which was passed in. </summary>
		/// <param name="Dec"> Double value of the four-bit value to convert to Hex </param>
		/// <returns> Hex character as a string </returns>
		private static string GetHex(double Dec)
		{
			string Value = "";
			char c = 'a';
			if (Dec >= 10 && Dec <= 15)
			{
				c += (char)(Dec - 10);
				Value += c;
			}
			else Value = "" + Dec;
			return Value;
		}
	}
}
