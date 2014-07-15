using System;
using SobekCM.Resource_Object;

namespace SobekCM.Management_Tool.Importer
{
	/// <summary> Defines the basic methods expected from any bibliographic reader <br /> <br /> </summary>
	/// <remarks> Written by Mark Sullivan (2005) </remarks>
	public interface iBibliographicReader
	{
		/// <summary> Checks the source to make sure it exists, can be connected to, and conforms
		/// to the expected format. </summary>
		/// <returns> TRUE if everything checks out, otherwise FALSE </returns>
		/// <remarks> This can be called before Next(), but is not mandatory. </remarks>
		bool Check_Source();

		/// <summary> Reads in the next bit of bibliographic information </summary>
		/// <returns> Bibliographic information object, or NULL if there are no more to read </returns>
		SobekCM_Item Next();

		/// <summary> This will attempt to return the number of items which are available to be read.  </summary>
		/// <remarks> If it is not possible to determine the number of items, -1 will be returned. </remarks>
		int Count { get; }

		/// <summary> Closes any resources which are being used by this reader </summary>
		/// <remarks> This should always be called after reading </remarks>
		void Close();
	}
}
