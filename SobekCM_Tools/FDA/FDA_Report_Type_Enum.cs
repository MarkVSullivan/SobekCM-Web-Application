namespace SobekCM.Tools.FDA
{
    /// <summary> Enumeration indicates the type of FDA Ingest Report </summary>
    public enum FDA_Report_Type
    {
        /// <summary> FDA Ingest Report when a SIP is successfully ingested into the digital archive </summary>
        INGEST = 1,

        /// <summary> Report is created when an item is withdrawn from the digital archive </summary>
        WITHDRAWAL,

        /// <summary> Dissemination Report is written when an item is re-ingested into the archive </summary>
        DISSEMINATION,

        /// <summary> FDA Ingest Error Report when a SIP is not ingested into the digital archive </summary>
        ERROR,

        /// <summary> Used to indicate an unrecognized FDA report type </summary>
        INVALID
    }
}
