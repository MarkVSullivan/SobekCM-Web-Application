namespace SobekCM.Builder_Library.Modules
{
    /// <summary> Structure maintains the identifying information for items that had some processing, 
    /// either due to a new incoming package or a database flag to reprocess </summary>
    public struct BibVidStruct
    {
        /// <summary> Bibliographic identifier </summary>
        public readonly string BibID;

        /// <summary> Volume identifier </summary>
        public readonly string VID;

        /// <summary> Constructor for a new instance of the BibVidStruct struct </summary>
        /// <param name="BibID"> Bibliographic identifier </param>
        /// <param name="VID"> Volume identifier </param>
        public BibVidStruct(string BibID, string VID)
        {
            this.BibID = BibID;
            this.VID = VID;
        }
    }
}
