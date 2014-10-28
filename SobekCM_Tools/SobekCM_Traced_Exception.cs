#region Using directives

using System;

#endregion

namespace SobekCM.Tools
{
    /// <summary> Custom exception which includes the trace router from the current HTTP request, which functions
    /// as a mini-trace route/stack trace and can be included in the error email </summary>
    public class SobekCM_Traced_Exception : ApplicationException
    {

        private readonly Custom_Tracer tracer;

        /// <summary> Constructor for a new instance of the SobekCM_Traced_Exception class </summary>
        /// <param name="Message"> The error message that explains the reason for the exception </param>
        /// <param name="Inner_Exception"> The exception which is the cause of the current exception </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public SobekCM_Traced_Exception(string Message, Exception Inner_Exception, Custom_Tracer Tracer ) : base( Message, Inner_Exception )
        {
            tracer = Tracer;
            if (tracer != null)
            {
                tracer.Add_Trace("SobekCM_Traced_Exception.Constructor", "Exception caught and bundled in the custom traced exception");
            }
        }

        /// <summary> Returns the trace route (from the stored tracer object) as html </summary>
        public string Trace_Route_HTML
        {
            get
            {
                return tracer != null ? tracer.Complete_Trace : String.Empty;
            }
        }

        /// <summary> Returns the trace route (from the stored tracer object) as text </summary>
        public string Trace_Route
        {
            get {
                return tracer != null ? tracer.Text_Trace : String.Empty;
            }
        }
    }
}
