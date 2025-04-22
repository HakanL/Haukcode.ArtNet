using System;

namespace Haukcode
{
    public class InvalidPacketException : InvalidOperationException
    {
        public InvalidPacketException(string message)
            : base(message)
        {
        }
    }
}
