
namespace SobekCM.Engine_Library.Microservices
{
    /// <summary> A single IP address (or one continuous range of IP addresses) which make up one 
    /// part of a restriction range for restricting access to microservices </summary>
    public class Microservice_IpRange
    {
        /// <summary> Descriptive label for this particular IP address(es) </summary>
        public string Label { get; internal set; }

        /// <summary> IP address, or the beginning of a range of IP addresses </summary>
        public string StartIp { get; internal set;  }

        /// <summary> Ending IP address, in the case this is a range of IP addresses </summary>
        public string EndIp { get; internal set; }
    }
}