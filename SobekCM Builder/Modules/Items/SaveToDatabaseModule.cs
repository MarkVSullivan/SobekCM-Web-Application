﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Library.Builder;

namespace SobekCM.Builder.Modules.Items
{
    public class SaveToDatabaseModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            // Determine total size on the disk
            string[] all_files_final = Directory.GetFiles(Resource.Resource_Folder);
            double size = all_files_final.Sum(ThisFile => (double)(((new FileInfo(ThisFile)).Length) / 1024));
            Resource.DiskSpaceMb = size;

            // Save this package to the database
            if (!Resource.Save_to_Database(Resource.NewPackage))
            {
                OnError("Error saving data to SobekCM database.  The database may not reflect the most recent data in the METS.", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
            }
        }
    }
}
