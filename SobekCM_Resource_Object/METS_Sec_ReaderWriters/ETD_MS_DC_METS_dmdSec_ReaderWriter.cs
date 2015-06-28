#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using SobekCM.Resource_Object.Metadata_Modules;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
    /// <summary> Special ETD MS dublin core METS section reader/writer </summary>
	public class ETD_MS_DC_METS_dmdSec_ReaderWriter : DC_METS_dmdSec_ReaderWriter
	{
		/// <summary> Writes the dmdSec for the entire package to the text writer </summary>
		/// <param name="Output_Stream">Stream to which the formatted text is written </param>
		/// <param name="METS_Item">Package with all the metadata to save</param>
		/// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
		/// <returns>TRUE if successful, otherwise FALSE </returns>
		/// <remarks>This utilized the DC writer in the base class and just adds the ETD information </remarks>
		public override bool Write_dmdSec(TextWriter Output_Stream, SobekCM_Item METS_Item, Dictionary<string, object> Options)
		{
			// Write the base stuff
			base.Write_dmdSec(Output_Stream, METS_Item, Options);

			// Ensure this metadata module extension exists and has data
			Thesis_Dissertation_Info thesisInfo = METS_Item.Get_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY) as Thesis_Dissertation_Info;
			if ((thesisInfo == null) || (!thesisInfo.hasData))
				return true;

			// Add the ETD stuff if the ETD metadata module exists
			if (!String.IsNullOrEmpty(thesisInfo.Degree))
				Output_Stream.WriteLine("<thesis.degree.name>" + Convert_String_To_XML_Safe(thesisInfo.Degree) + "</thesis.degree.name>");

			if (thesisInfo.Degree_Level == Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Bachelors)
				Output_Stream.WriteLine("<thesis.degree.level>Bachelors</thesis.degree.level>");
			if (thesisInfo.Degree_Level == Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Masters)
				Output_Stream.WriteLine("<thesis.degree.level>Masters</thesis.degree.level>");
			if (thesisInfo.Degree_Level == Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Doctorate)
				Output_Stream.WriteLine("<thesis.degree.level>Doctorate</thesis.degree.level>");
			if (thesisInfo.Degree_Level == Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.PostDoctorate)
				Output_Stream.WriteLine("<thesis.degree.level>Post-Doctorate</thesis.degree.level>");

			if (thesisInfo.Degree_Disciplines_Count > 0)
			{
				foreach (string thisDiscipline in thesisInfo.Degree_Disciplines)
				{
					Output_Stream.WriteLine("<thesis.degree.discipline>" + Convert_String_To_XML_Safe(thisDiscipline) + "</thesis.degree.discipline>");
				}
			}


			if (!String.IsNullOrEmpty(thesisInfo.Degree_Grantor))
				Output_Stream.WriteLine("<thesis.degree.grantor>" + Convert_String_To_XML_Safe(thesisInfo.Degree_Grantor) + "</thesis.degree.grantor>");



			return true;
		}
	}
}
