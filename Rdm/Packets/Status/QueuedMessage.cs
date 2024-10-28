#region Copyright © 2011 Oliver Waits
//______________________________________________________________________________________________________________
//  
// Copyright © 2011 Oliver Waits
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//______________________________________________________________________________________________________________
#endregion
   
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Status
{
    /// <summary>
    /// Obtains a message from the responders message queue.
    /// </summary>
    /// <remarks>
    /// The Message Count field of all response messages defines the number of
    /// messages that are queued in the responder. Each <see cref="QueuedMessage"/> response shall be
    /// composed of a single message response.
    /// 
    /// A responder with multiple messages queued shall first respond with the most urgent message.
    /// The message count of the responder shall be decremented prior to sending the response.
    /// 
    /// A responder with no messages queued shall respond to a <see cref="QueuedMessage"/> message with a
    /// <see cref="StatusMessage"/> response. A StatusMessage response with a PDL of 0x00 does not
    /// imply that the responder supports the <see cref="StatusMessage"/> PID.
    /// </remarks>
    public class QueuedMessage
    {
        /// <summary>
        /// Requests that the device sends a queued message.
        /// </summary>
        /// <remarks>
        /// The response to this message is the queued message.
        /// </remarks>
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.QueuedMessage)
            {
            }

            /// <summary>
            /// Determines what queued message the device should send.
            /// </summary>
            public StatusTypes StatusType { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                StatusType = (StatusTypes)data.ReadByte(); ;
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte((byte) StatusType);
            }

            #endregion
        }
    }
}
