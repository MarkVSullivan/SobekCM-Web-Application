#region Using directives

using System.Collections.Generic;
using System.Collections.ObjectModel;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Metadata_Modules;

#endregion

namespace SobekCM.Resource_Object
{
    public interface iMetadataDescribable
    {
        #region Code to cover metadata module extensions

        /// <summary> Gets the collection of all included metadata module extensions </summary>
        /// <remarks> These methods allows extensibility since any metadata plug-in can implement
        /// the iMetadata_Module interface and be added here. </remarks>
        ReadOnlyCollection<iMetadata_Module> Metadata_Modules { get; }

        /// <summary> Gets a metadata module extension by name from this digital resource </summary>
        /// <param name="Module_Name"> Name of the module to retrieve </param>
        /// <returns>Requested metadata module, or NULL if it doesn't exist </returns>
        /// <remarks>These methods allows extensibility since any metadata plug-in can implement
        /// the iMetadata_Module interface and be added here. </remarks>
        iMetadata_Module Get_Metadata_Module(string Module_Name);

        /// <summary> Adds a new metadata module extension to this digital resource </summary>
        /// <param name="Module_Name"> Name of this module </param>
        /// <param name="New_Module">New metadata module to add to this digital resource</param>
        /// <remarks> These methods allows extensibility since any metadata plug-in can implement
        /// the iMetadata_Module interface and be added here. </remarks>
        void Add_Metadata_Module(string Module_Name, iMetadata_Module New_Module);

        #endregion

        #region Code to cover any unanalyzed DMDSEC or AMDSEC portions of the original METS file

        /// <summary> Gets the collection of unanalyzed DMDSECs (descriptive metadata sections) in the original METS file </summary>
        List<Unanalyzed_METS_Section> Unanalyzed_DMDSECs { get; }

        /// <summary> Gets the collection of unanalyzed AMDSECs (administrative metadata sections) in the original METS file </summary>
        List<Unanalyzed_METS_Section> Unanalyzed_AMDSECs { get; }

        /// <summary> Adds information about an unanalyzed DMDSEC (descriptive metadata section) in the METS file, to be preserved as is for later writing </summary>
        /// <param name="Section_Attributes"> List of attributes in the top-level definition of this section </param>
        /// <param name="ID"> ID for the top-level section (also included in the attribute list) </param>
        /// <param name="Inner_XML"> Complete XML include in this unanalyzed METS section </param>
        void Add_Unanalyzed_DMDSEC(List<KeyValuePair<string, string>> Section_Attributes, string ID, string Inner_XML);

        /// <summary> Adds information about an unanalyzed DMDSEC (descriptive metadata section) in the METS file, to be preserved as is for later writing </summary>
        /// <param name="METS_Section"> Fully built unanalyzed section object </param>
        void Add_Unanalyzed_DMDSEC(Unanalyzed_METS_Section METS_Section);

        /// <summary> Adds information about an unanalyzed AMDSEC (administrative metadata section) in the METS file, to be preserved as is for later writing </summary>
        /// <param name="Section_Attributes"> List of attributes in the top-level definition of this section </param>
        /// <param name="ID"> ID for the top-level section (also included in the attribute list) </param>
        /// <param name="Inner_XML"> Complete XML include in this unanalyzed METS section </param>
        void Add_Unanalyzed_AMDSEC(List<KeyValuePair<string, string>> Section_Attributes, string ID, string Inner_XML);

        /// <summary> Adds information about an unanalyzed AMDSEC (administrative metadata section) in the METS file, to be preserved as is for later writing </summary>
        /// <param name="METS_Section"> Fully built unanalyzed section object </param>
        void Add_Unanalyzed_AMDSEC(Unanalyzed_METS_Section METS_Section);

        #endregion

        #region Code to address bibliographic information associated with thie node

        /// <summary> Flag indicates if the bibliographic data object has been built here and contains data</summary>
        bool hasBibliographicData { get; }

        /// <summary> Gets the bibliographic information associated with this node  </summary>
        /// <remarks> NOTE!! Even if no bib info exists for this item, calling this method will
        /// create the object, which is fairly memory intensive.  You should always call the
        /// <see cref="hasBibliographicData" /> method first. </remarks>
        Bibliographic_Info Bib_Info { get; }

        #endregion
    }
}