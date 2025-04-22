using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.IO;

namespace Haukcode.Rdm.Packets.Product
{
    /// <summary>
    /// This parameter is used to retrieve a variety of information about the device that is normally
    /// required by a controller.
    /// </summary>
    public class DeviceInfo
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.DeviceInfo)
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
                : base(RdmCommands.GetResponse, RdmParameters.DeviceInfo)
            {
            }

            public short RdmProtocolVersion { get; set; }

            public short DeviceModelId { get; set; }

            public ProductCategories ProductCategory { get; set; }

            public int SoftwareVersionId { get; set; }

            public short DmxFootprint { get; set; }

            public byte DmxPersonality { get; set; }

            public byte DmxPersonalityCount { get; set; }

            public short DmxStartAddress { get; set; }

            public short SubDeviceCount { get; set; }

            public byte SensorCount { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                RdmProtocolVersion = data.ReadInt16();
                DeviceModelId = data.ReadInt16();
                ProductCategory = (ProductCategories) data.ReadInt16();
                SoftwareVersionId = data.ReadHiLoInt32();
                DmxFootprint = data.ReadInt16();
                DmxPersonality = data.ReadByte();
                DmxPersonalityCount = data.ReadByte();
                DmxStartAddress = data.ReadInt16();
                SubDeviceCount = data.ReadInt16();
                SensorCount = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteUInt16(RdmProtocolVersion);
                data.WriteUInt16(DeviceModelId);
                data.WriteUInt16((short) ProductCategory);
                data.WriteHiLoInt32(SoftwareVersionId);
                data.WriteUInt16(DmxFootprint);
                data.WriteByte(DmxPersonality);
                data.WriteByte(DmxPersonalityCount);
                data.WriteUInt16(DmxStartAddress);
                data.WriteUInt16(SubDeviceCount);
                data.WriteByte(SensorCount);
            }

            #endregion

            public override string ToString()
            {
                return string.Format("{0}",ProductCategory.ToString());
            }
        }
    }
}
