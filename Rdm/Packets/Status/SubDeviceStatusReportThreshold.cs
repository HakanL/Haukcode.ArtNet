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
    /// This parameter is used to set the verbosity of Sub-Device reporting using the Status Type codes.
    /// </summary>
    /// <remarks>
    /// This feature is used to inhibit reports from, for example, a specific dimmer in a rack that is
    /// generating repeated errors.
    /// </remarks>
    public class SubDeviceStatusReportThreshold
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.SubDeviceStatusReportThreshold)
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

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.SubDeviceStatusReportThreshold)
            {
            }

            /// <summary>
            /// The status type being inhibited.
            /// </summary>
            public StatusTypes StatusType { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                StatusType = (StatusTypes)data.ReadByte(); ;
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write((byte)StatusType);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.SubDeviceStatusReportThreshold)
            {
            }

            /// <summary>
            /// The status type to inhibit.
            /// </summary>
            public StatusTypes StatusType { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                StatusType = (StatusTypes)data.ReadByte(); ;
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write((byte)StatusType);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.SubDeviceStatusReportThreshold)
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
