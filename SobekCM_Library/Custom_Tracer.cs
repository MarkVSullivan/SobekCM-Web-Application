#region Using directives

using System;
using System.Diagnostics;
using System.Text;

#endregion

namespace SobekCM.Library
{
    /// <summary> Enumeration tells the type of trace message </summary>
    public enum Custom_Trace_Type_Enum : byte
    {
        /// <summary> This is a NORMAL trace message </summary>
        Normal = 1,

        /// <summary> This is an ERROR trace message </summary>
        Error
    }

    /// <summary> Traces execution of a SobekCM query through construction of the various classes and rendering of HTML and controls </summary>
    /// <remarks> This class allows a trace route to be written at the bottom of the server HTML page </remarks>
    public class Custom_Tracer
    {
        private readonly Stopwatch elapsedTimer;
        private readonly StringBuilder traceBuilder;

        /// <summary> Constructor for a new instance of the Custom_Tracer class </summary>
        public Custom_Tracer()
        {
            Enabled = true;
            traceBuilder = new StringBuilder();
            traceBuilder.Append("<table class=\"Traceroute\"><tr><th>MILLISECOND &nbsp; </th><th align=\"left\">CLASS.METHOD</th><th align=\"left\">MESSAGE</th></tr>");
            elapsedTimer = new Stopwatch();
            elapsedTimer.Start();
        }

        /// <summary> Number of milliseconds since execution began </summary>
        public long Milliseconds
        {
            get
            {
                return elapsedTimer.ElapsedMilliseconds;
            }
        }

        /// <summary> Flag which indicates whether this class is enabled </summary>
        public bool Enabled { get; set; }

        /// <summary> Gets the complete trace route, as an HTML table </summary>
        public string Complete_Trace
        {
            get
            {
                return traceBuilder + "</table>";
            }
        }

        /// <summary> Returns the complete trace route as simple text </summary>
        /// <remarks> Since the trace is actually built as HTML, this must remove the html tags from the trace route</remarks>
        public string Text_Trace
        {
            get
            {
                return traceBuilder.ToString().Replace("<table class=\"Traceroute\"><tr><th>MILLISECOND &nbsp; </th><th align=\"left\">CLASS.METHOD</th><th align=\"left\">MESSAGE</th></tr>", "").Replace("<tr><td>", "").Replace("</td><td>", "      ").Replace("<font color=\"red\">", "").Replace("</font>", "").Replace("</td></tr>", "" + Environment.NewLine );
            }
        }

        /// <summary> Adds a trace message to the building list of traces </summary>
        /// <param name="Method">Method from which this trace call was executed</param>
        public void Add_Trace(string Method)
        {
            Add_Trace(Method, String.Empty, Custom_Trace_Type_Enum.Normal);
        }

        /// <summary> Adds a trace message to the building list of traces </summary>
        /// <param name="Method">Method from which this trace call was executed</param>
        /// <param name="Message">Message to add to trace</param>
        public void Add_Trace(string Method, string Message)
        {
            Add_Trace(Method, Message, Custom_Trace_Type_Enum.Normal);
        }

        /// <summary> Adds a trace message to the building list of traces </summary>
        /// <param name="Method">Method from which this trace call was executed</param>
        /// <param name="Message">Message to add to trace</param>
        /// <param name="Message_Type">Type of message</param>
        public void Add_Trace(string Method, string Message, Custom_Trace_Type_Enum Message_Type )
        {
                if (Message_Type == Custom_Trace_Type_Enum.Normal)
                {
                    traceBuilder.Append("<tr><td>" + Milliseconds + "</td><td>" + Method.ToLower() + "</td><td>" + Message + "</td></tr>\n");
                }
                else
                {
                    traceBuilder.Append("<tr><td>" + Milliseconds + "</td><td>" + Method.ToLower() + "</td><td><font color=\"red\">" + Message + "</font></td></tr>\n");
                }
        }

        /// <summary> Clears this trace route and resets the elapsed timer </summary>
        /// <remarks> This is generally called when creating static HTML pages through the Builder application, to prevent
        /// the trace object from getting astronomically large </remarks>
        public void Clear()
        {
            if (traceBuilder.Length > 0)
                traceBuilder.Remove(0, traceBuilder.Length);
            elapsedTimer.Reset();
        }
    }
}
