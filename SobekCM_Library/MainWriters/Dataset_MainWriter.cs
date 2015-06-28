#region Using directives

using System.IO;
using SobekCM.Core.Navigation;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.MainWriters
{
    /// <summary> Main writer writes search results and item browses as a dataset represented in
    /// XML format to the response stream.  This is the native Microsoft.NET format, easily read into
    /// a remote dataset by using DataSet.ReadXML() </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractMainWriter"/>. </remarks>
    public class Dataset_MainWriter : abstractMainWriter
    {
        /// <summary> Constructor for a new instance of the Dataset_MainWriter class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Dataset_MainWriter(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
   
        {
            // All work done in base class
        }

        /// <summary> Gets the enumeration of the type of main writer </summary>
        /// <value> This property always returns the enumerational value <see cref="Writer_Type_Enum.DataSet"/>. </value>
        public override Writer_Type_Enum Writer_Type { get { return Writer_Type_Enum.DataSet; } }

        /// <summary> Perform all the work of adding text directly to the response stream back to the web user </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Html(TextWriter Output, Custom_Tracer Tracer)
        {
            switch (RequestSpecificValues.Current_Mode.Mode)
            {
                case Display_Mode_Enum.Results:
                case Display_Mode_Enum.Aggregation:
                    if (RequestSpecificValues.Paged_Results != null)
                        display_search_results();
                     break;

                default:
                    Output.Write("DataSet Writer - Unknown Mode");
                    break;
            }
        }

        private void display_search_results()
        {
            // Write this information
            //search_results.WriteXml(Output, XmlWriteMode.WriteSchema);
        }
    }
}
