using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.IO;

namespace Haukcode.Rdm.Packets.Product
{
    /// <summary>
    /// This parameter is used to instruct a device to revert to its Factory Default user settings or
    /// configuration as determined by the Manufacturer.
    /// </summary>
    public class FactoryDefaults
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.FactoryDefaults)
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
                : base(RdmCommands.GetResponse, RdmParameters.FactoryDefaults)
            {
            }

            public bool IsFactoryDefaults { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                IsFactoryDefaults = data.ReadBool();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteBool(IsFactoryDefaults);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.FactoryDefaults)
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
                : base(RdmCommands.SetResponse, RdmParameters.FactoryDefaults)
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
