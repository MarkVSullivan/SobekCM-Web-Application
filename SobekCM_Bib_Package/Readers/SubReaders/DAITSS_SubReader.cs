using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Bib_Package.SobekCM_Info;
using SobekCM.Bib_Package.Divisions;

namespace SobekCM.Bib_Package.Readers.SubReaders
{
    /// <summary> Subreader reads a DAITSS-compliant section of XML and stores the data in the provided digital resource object </summary>
    public class DAITSS_SubReader
    {
        /// <summary> Reads the DAITSS-compliant section of XML and stores the data in the provided digital resource </summary>
        /// <param name="r"> XmlTextReader from which to read the daitss data </param>
        /// <param name="package"> Digital resource object to save the data to </param>
        public static void Read_Daitss_Sec(XmlTextReader r, SobekCM_Item package)
        {
            // Loop through reading each XML node
            do
            {
                // If this is the end of this section, return
                if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "METS:mdWrap") || (r.Name == "mdWrap")))
                    return;

                // get the right division information based on node type
                switch (r.NodeType)
                {
                    case XmlNodeType.Element:
                        if ((r.Name.ToLower() == "daitss:agreement_info") && (r.HasAttributes))
                        {
                            if (r.MoveToAttribute("ACCOUNT"))
                                package.DAITSS.Account = r.Value;

                            if (r.MoveToAttribute("SUB_ACCOUNT"))
                                package.DAITSS.SubAccount = r.Value;

                            if (r.MoveToAttribute("PROJECT"))
                                package.DAITSS.Project = r.Value;
                        }
                        break;
                }
            } while (r.Read());
        }
    }
}
