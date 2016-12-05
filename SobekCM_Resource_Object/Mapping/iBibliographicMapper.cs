using System.Collections.Generic;

namespace SobekCM.Resource_Object.Mapping
{
    /// <summary> Interface is for all bibliographic mapping classes which takes data and field name and maps that 
    /// data into the SobekCM field within the main SobekCM Item package </summary>
    public interface iBibliographicMapper
    {
        /// <summary> Name of this bibliographic mapper, used for logging mapping </summary>
        string Name { get; }

        /// <summary> Returns the list of preferred mappings for the elements handled
        /// by this mapper </summary>
        /// <remarks> For example this might just return 'title', 'creator', 'subject', etc.. </remarks>
        List<string> Preferred_Mappings { get; }

        /// <summary> Adds a bit of data to a bibliographic package using the mapping </summary>
        /// <param name="Package">Bibliographic package to receive the data</param>
        /// <param name="Data">Text of the data</param>
        /// <param name="Field">Mapped field</param>
        /// <returns> TRUE if the field was mapped, FALSE if there was data and no mapping was found </returns>
        bool Add_Data(SobekCM_Item Package, string Data, string Field);

    }
}
