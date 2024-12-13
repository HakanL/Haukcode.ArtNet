using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Sensors
{
    public class SensoreValue
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.SensorValue)
            {
            }

            public byte SensorNumber { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                SensorNumber = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte(SensorNumber);
            }

            #endregion
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.SensorValue)
            {
            }

            public byte SensorNumber { get; set; }

            public short PresentValue { get; set; }

            public short MinValue { get; set; }

            public short MaxValue { get; set; }

            public short RecordedValue { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                SensorNumber = data.ReadByte();
                PresentValue = data.ReadInt16();
                MinValue = data.ReadInt16();
                MaxValue = data.ReadInt16();
                RecordedValue = data.ReadInt16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte(SensorNumber);
                data.WriteUInt16(PresentValue);
                data.WriteUInt16(MinValue);
                data.WriteUInt16(MaxValue);
                data.WriteUInt16(RecordedValue);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.SensorValue)
            {
            }

            public byte SensorNumber { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                SensorNumber = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte(SensorNumber);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.SensorValue)
            {
            }

            public byte SensorNumber { get; set; }

            public short PresentValue { get; set; }

            public short MinValue { get; set; }

            public short MaxValue { get; set; }

            public short RecordedValue { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                SensorNumber = data.ReadByte();
                PresentValue = data.ReadInt16();
                MinValue = data.ReadInt16();
                MaxValue = data.ReadInt16();
                RecordedValue = data.ReadInt16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte(SensorNumber);
                data.WriteUInt16(PresentValue);
                data.WriteUInt16(MinValue);
                data.WriteUInt16(MaxValue);
                data.WriteUInt16(RecordedValue);
            }

            #endregion
        }
    }
}
