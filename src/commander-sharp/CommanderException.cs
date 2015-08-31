using System;
using System.Runtime.Serialization;

namespace Jaja.Commander
{
  [Serializable]
  public class CommanderException : Exception, ISerializable
  {
    public CommanderException() : base() { }

    public CommanderException(string message) : base(message) { }

    public CommanderException(string message, Exception inner) : base(message, inner) { }

    public CommanderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }
}