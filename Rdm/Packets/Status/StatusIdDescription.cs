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
    /// This parameter is used to request an ASCII text description of a given Status ID. The description
    /// may be up to 32 characters.
    /// </summary>
    public class StatusIdDescription
    {

        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.StatusIdDescription)
            {
            }

            /// <summary>
            /// The status to request the description for.
            /// </summary>
            public short StatusId { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                StatusId = data.ReadNetwork16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(StatusId);
            }

            #endregion
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.StatusIdDescription)
            {
            }

            /// <summary>
            /// The description for the requested status.
            /// </summary>
            public string Description { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                Description = data.ReadNetworkString(Header.ParameterDataLength);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(Description);
            }

            #endregion
        }
    }
}
