#region Using directives

using System;

#endregion

namespace SobekCM.Tools.Logs
{
	/// <summary> LogFile_Exception is a custom exception which is thrown for any error
	/// caught while creating, or writing to a log file. <br /> <br /> </summary>
	/// <remarks> This class extends the <see cref="ApplicationException"/> class. </remarks>>
	public class LogFile_Exception : ApplicationException
	{
		/// <summary> Constructor which creates a new LogFile_Exception </summary>
		/// <param name="CustomMessage"> Custom message which explains the error which
		/// occurred to fire this exception. </param>
		public LogFile_Exception( string CustomMessage ) : base ( "Error detected in a Log File class.\n\n" + CustomMessage )
		{
			// Empty constructor
		}
	}
}
