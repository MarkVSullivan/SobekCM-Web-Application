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
		/// <param name="customMessage"> Custom message which explains the error which
		/// occurred to fire this exception. </param>
		public LogFile_Exception( string customMessage ) : base ( "Error detected in a Log File class.\n\n" + customMessage )
		{
			// Empty constructor
		}
	}
}
