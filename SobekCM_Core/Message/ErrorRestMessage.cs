using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Message
{
    public enum ErrorRestType : byte
    {
        NONE,
    
        Exception,

        InputError,

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
        /// <param name="ErrorType"> Error type ( NONE, exception, input error, etc.. )</param>
        /// <param name="Message"> Message about this error (or possible success) assuming an error was encountered</param>
        public ErrorRestMessage(ErrorRestType ErrorType, string Message)
        {
            this.ErrorType = ErrorType;
            this.Message = Message;
        }

        /// <summary> Error type ( NONE, exception, input error, etc.. ) </summary>
        [DataMember(Name = "type")]
        [ProtoMember(1)]
        public ErrorRestType ErrorType { get; private set; }

        /// <summary> Message about this error (or possible success) assuming an error was encountered </summary>
        [DataMember(EmitDefaultValue = false, Name = "message")]
        [ProtoMember(2)]
        public string Message { get; private set; }


    }
}
