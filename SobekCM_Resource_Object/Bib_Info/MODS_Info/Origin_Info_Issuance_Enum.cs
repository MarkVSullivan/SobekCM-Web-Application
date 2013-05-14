namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> Enumeration indicates the number of issues related to a digital resource or the original
    /// resource </summary>
    public enum Origin_Info_Issuance_Enum
    {
        /// <summary> Unknown (default) issuance information </summary>
        UNKNOWN = -1,

        /// <summary> Resource is a continuing resource with multiple, continuous issues  </summary>
        Continuing = 1,

        /// <summary> Single monographic resource, without additional issues </summary>
        Monographic,

        /// <summary> Single unit resource, without additional issues </summary>
        Single_Unit,

        /// <summary> Monographic resource with multiple parts issued </summary>
        Multipart_Monograph,

        /// <summary> Serial resource with mutiple parts issued </summary>
        Serial,

        /// <summary> Integrating resource which may have multiple related parts issued </summary>
        Integrating_Resource
    }
}