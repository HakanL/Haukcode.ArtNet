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

namespace Haukcode.Rdm.Packets.Management
{
    /// <summary>
    /// Used to collect information that may be useful in analyzing the integrity of the communication system.
    /// </summary>
    /// <remarks>
    /// A responder shall respond with a cumulative total of each error type in the response message defined below.
    /// </remarks>
    public class CommsStatus
    {
        /// <summary>
        /// Requests information about the amount of errors encountered by a device.
        /// </summary>
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.CommsStatus)
            {
            }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
            }

            #endregion
        }

        /// <summary>
        /// Contains information aabout the amount of errors encountered by a device.
        /// </summary>
        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.CommsStatus)
            {
            }

            /// <summary>
            /// The message ended before the Message Length field was received.
            /// </summary>
            public short ShortMessage { get; set; }

            /// <summary>
            /// The number of slots actually received did not match the Message Length plus
            /// the size of the Checksum.
            /// </summary>
            /// <remarks>
            /// This counter shall only be incremented if the Destination UID in the
            /// packet matches the Device’s UID.
            /// </remarks>
            public short LengthMismatch { get; set; }

            /// <summary>
            /// The message checksum failed.
            /// </summary>
            /// <remarks>
            /// This counter shall only be incremented if the Destination UID in the packet matches the Device’s UID.
            /// </remarks>
            public short ChecksumFail { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                ShortMessage = data.ReadHiLoInt16();
                LengthMismatch = data.ReadHiLoInt16();
                ChecksumFail = data.ReadHiLoInt16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteHiLoInt16(ShortMessage);
                data.WriteHiLoInt16(LengthMismatch);
                data.WriteHiLoInt16(ChecksumFail);
            }

            #endregion
        }

        /// <summary>
        /// Clears all the error counts to zero.
        /// </summary>
        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.CommsStatus)
            {
            }


            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
            }

            #endregion
        }

        /// <summary>
        /// Confirmation that the error counts have been cleared to zero.
        /// </summary>
        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.CommsStatus)
            {
            }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
            }

            #endregion
        }
    }
}
