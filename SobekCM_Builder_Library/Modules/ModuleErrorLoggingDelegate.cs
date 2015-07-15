
namespace SobekCM.Builder_Library.Modules
{
    /// <summary> Delegate is used for events that store detailed error information to the builder log </summary>
    /// <param name="LogStatement"> Statement for the error log entry </param>
    /// <param name="BibID_VID"> BibID and VID, if this error occurred while looking at a single digital resource folder </param>
    /// <param name="MetsType"> Incoming METS type, if identified </param>
    /// <param name="RelatedLogID"> Primary key for a related log entry, if this is a log entry related to another </param>
    /// <returns> Primary key for this related log entry, in case other errors should be attached to this, or -1 if no delegates attached </returns>
    public delegate long ModuleErrorLoggingDelegate(string LogStatement, string BibID_VID, string MetsType, long RelatedLogID);
}
