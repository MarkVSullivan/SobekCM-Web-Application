#region Using directives

using System;
using System.Runtime.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Message
{
    /// <summary> Enumeration of possible exceptions and errors caught during execution of a REST API microservice endpoint </summary>
    public enum ErrorRestTypeEnum : byte
    {
        /// <summary> No error was encountered - initial state </summary>
        NONE,

        /// <summary> An exception was caught during execution </summary>
        Exception,

        /// <summary> Necessary input values were not provided or were incorrect </summary>
        InputError,

        /// <summary> The request successfully completed </summary>
        Successful
    }

    /// <summary> Error message returned via REST messages, in the case a problem
    /// is encountered  </summary>
    [Serializable, DataContract, ProtoContract]
    public class ErrorRestMessage
    {
        /// <summary> Constructor for a new instance of the ErrorRestMessage class </summary>
        /// <remarks> Parameterless constructor required for deserialization </remarks>
        public ErrorRestMessage()
        {
            // Do nothing
        }

        /// <summary> Constructor for a new instance of the ErrorRestMessage class </summary>
        /// <param name="ErrorTypeEnum"> Error type ( NONE, exception, input error, etc.. )</param>
        /// <param name="Message"> Message about this error (or possible success) assuming an error was encountered</param>
        public ErrorRestMessage(ErrorRestTypeEnum ErrorTypeEnum, string Message)
        {
            this.ErrorTypeEnum = ErrorTypeEnum;
            this.Message = Message;
        }

        /// <summary> Error type ( NONE, exception, input error, etc.. ) </summary>
        [DataMember(Name = "type")]
        [ProtoMember(1)]
        public ErrorRestTypeEnum ErrorTypeEnum { get; private set; }

        /// <summary> Message about this error (or possible success) assuming an error was encountered </summary>
        [DataMember(EmitDefaultValue = false, Name = "message")]
        [ProtoMember(2)]
        public string Message { get; private set; }


    }
}
