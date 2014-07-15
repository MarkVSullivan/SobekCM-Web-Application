using System;
using System.Collections.Generic;
using System.Text;

namespace SobekCM.Management_Tool.Importer
{
    public enum Importer_Type_Enum
    {
        METS = 0,
        MARC,
        Spreadsheet
    }

    interface iImporter_Process
    {
        Importer_Type_Enum Importer_Type { get; }
    }

}
