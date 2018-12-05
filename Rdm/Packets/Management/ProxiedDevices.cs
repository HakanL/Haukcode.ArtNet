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
    /// This parameter is used to retrieve the UIDs from a device identified as a proxy during discovery.
    /// </summary>
    /// <remarks>
    /// The response to this parameter contains a packed list of 48-bit UIDs for all devices represented
    /// by the proxy.
    /// 
    /// If there are no current devices being proxied then the Parameter Data Length field shall be returned as 0x00.
    /// </remarks>
    public class ProxiedDevices
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.ProxiedDevices)
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
                : base(RdmCommands.GetResponse, RdmParameters.ProxiedDevices)
            {
                DeviceIds = new List<UId>();
            }

            /// <summary>
            /// A list of device ids for devices discovered by the proxy.
            /// </summary>
            public List<UId> DeviceIds { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                for (int n = 0; n < Header.ParameterDataLength / 6; n++)
                {
                    DeviceIds.Add(data.ReadUId());
                }
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                foreach (UId id in DeviceIds)
                    data.Write(id);
            }

            #endregion
        }
    }
}
