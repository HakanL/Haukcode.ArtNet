using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.IO;

namespace Haukcode.Rdm.Packets.Control
{
    public class PerformSelfTest
    {
        public enum TestMode
        {
            Off = 0x0,
            All = 0xFF
        }

        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.PerformSelfTest)
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
                : base(RdmCommands.GetResponse, RdmParameters.PerformSelfTest)
            {
            }

            public bool IsTestActive { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                IsTestActive = data.ReadBool();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteBool(IsTestActive);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.PerformSelfTest)
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

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.PerformSelfTest)
            {
            }

            public byte TestNumber { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                TestNumber = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte(TestNumber);
            }

            #endregion
        }
    }
}
