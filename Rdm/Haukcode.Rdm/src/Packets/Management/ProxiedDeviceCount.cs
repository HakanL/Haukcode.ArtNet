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
using Haukcode.Rdm.IO;

namespace Haukcode.Rdm.Packets.Management
{
    /// <summary>
    /// This parameter is used to identify the number of devices being represented by a proxy and
    /// whether the list of represented device UIDs has changed.
    /// </summary>
    /// <remarks>
    /// If the List Change flag is set then the controller should request the procied devices.
    /// 
    /// The device will automatically clear the List Change flag after all the proxied UID’s have been
    /// retrieved using the ProxiedDevice message.
    /// 
    /// A proxy device shall indicate any change in it's device list through a QueuedMessage.
    /// </remarks>
    public class ProxiedDeviceCount
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.ProxiedDeviceCount)
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
                : base(RdmCommands.GetResponse, RdmParameters.ProxiedDeviceCount)
            {
            }

            /// <summary>
            /// The number of proxied devices connected to this proxy and discovered.
            /// </summary>
            public short DeviceCount { get; set; }

            /// <summary>
            /// Whether the list of proxied devices has changed since the list was last obtained.
            /// </summary>
            public bool ListChanged { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                DeviceCount = data.ReadInt16();
                ListChanged = data.ReadBool();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteUInt16(DeviceCount);
                data.WriteBool(ListChanged);
            }

            #endregion
        }
    }
}
