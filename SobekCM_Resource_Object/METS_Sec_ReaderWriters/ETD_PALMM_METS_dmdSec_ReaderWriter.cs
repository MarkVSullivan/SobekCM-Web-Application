#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using SobekCM.Resource_Object.Metadata_Modules;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
	/// <summary> Class is used to read and write Electronic Theses and Dissertations (ETD) metadata
	/// in the PALMM schema, utilized by the State Universities in Florida </summary>
	/// <remarks> This could likely be moved to a seperate plug-in library </remarks>
	public class ETD_PALMM_METS_dmdSec_ReaderWriter : ETD_SobekCM_METS_dmdSec_ReaderWriter
    {
        #region iPackage_dmdSec_ReaderWriter Members

        /// <summary> Writes the dmdSec for the entire package to the text writer </summary>
        /// <param name="Output_Stream">Stream to which the formatted text is written </param>
        /// <param name="METS_Item">Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public override bool Write_dmdSec(TextWriter Output_Stream, SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            Thesis_Dissertation_Info thesisInfo = METS_Item.Get_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY) as Thesis_Dissertation_Info;
            if ((thesisInfo == null) || (!thesisInfo.hasData))
                return true;

            Output_Stream.WriteLine("<palmm:thesis>");
            if (!String.IsNullOrEmpty(thesisInfo.Committee_Chair))
                Output_Stream.WriteLine("<palmm:committeeChair>" + Convert_String_To_XML_Safe(thesisInfo.Committee_Chair) + "</palmm:committeeChair>");
            if (!String.IsNullOrEmpty(thesisInfo.Committee_Co_Chair))
                Output_Stream.WriteLine("<palmm:committeeCoChair>" + Convert_String_To_XML_Safe(thesisInfo.Committee_Co_Chair) + "</palmm:committeeCoChair>");
            if (thesisInfo.Committee_Members_Count > 0)
            {
                foreach (string thisCommitteeMember in thesisInfo.Committee_Members)
                {
                    Output_Stream.WriteLine("<palmm:committeeMember>" + Convert_String_To_XML_Safe(thisCommitteeMember) + "</palmm:committeeMember>");
                }
            }
            if (thesisInfo.Graduation_Date.HasValue)
            {
                string encoded_date = thesisInfo.Graduation_Date.Value.Year + "-" + thesisInfo.Graduation_Date.Value.Month.ToString().PadLeft(2, '0') + "-" + thesisInfo.Graduation_Date.Value.Day.ToString().PadLeft(2, '0');
                Output_Stream.WriteLine("<palmm:graduationDate>" + encoded_date + "</palmm:graduationDate>");
            }
            if (!String.IsNullOrEmpty(thesisInfo.Degree))
                Output_Stream.WriteLine("<palmm:degree>" + Convert_String_To_XML_Safe(thesisInfo.Degree) + "</palmm:degree>");
			if (thesisInfo.Degree_Disciplines_Count > 0)
			{
				if ( thesisInfo.Degree_Disciplines_Count == 1 )
					Output_Stream.WriteLine("<palmm:degreeDiscipline>" + Convert_String_To_XML_Safe(thesisInfo.Degree_Disciplines[0]) + "</palmm:degreeDiscipline>");
				else
				{
					Output_Stream.Write("<palmm:degreeDiscipline>");
					bool first = true;
					foreach (string thisDiscipline in thesisInfo.Degree_Disciplines)
					{
						if ( !first )
							Output_Stream.Write(";");
						else
							first = false;

						Output_Stream.Write( Convert_String_To_XML_Safe(thisDiscipline));
					}
					Output_Stream.WriteLine("</palmm:degreeDiscipline>");
				}
			}
            if (!String.IsNullOrEmpty(thesisInfo.Degree_Grantor))
                Output_Stream.WriteLine("<palmm:degreeGrantor>" + Convert_String_To_XML_Safe(thesisInfo.Degree_Grantor) + "</palmm:degreeGrantor>");
			if (thesisInfo.Degree_Level == Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Bachelors)
				Output_Stream.WriteLine("<palmm:degreeLevel>Bachelors</palmm:degreeLevel>");
            if (thesisInfo.Degree_Level == Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Masters)
                Output_Stream.WriteLine("<palmm:degreeLevel>Masters</palmm:degreeLevel>");
            if (thesisInfo.Degree_Level == Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Doctorate)
                Output_Stream.WriteLine("<palmm:degreeLevel>Doctorate</palmm:degreeLevel>");
            Output_Stream.WriteLine("</palmm:thesis>");
            return true;
        }

        /// <summary> Returns the schema namespace (xmlns) information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema namespace info for the METS header</returns>
        public override string[] Schema_Namespace(SobekCM_Item METS_Item)
        {
            return new string[] {"palmm=\"http://www.fcla.edu/dls/md/palmm/\""};
        }

        /// <summary> Returns the schema location information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema location for the METS header</returns>
        public override string[] Schema_Location(SobekCM_Item METS_Item)
        {
            return new string[] {"    http://www.fcla.edu/dls/md/palmm/\r\n    http://www.fcla.edu/dls/md/palmm.xsd"};
        }

        #endregion
    }
}