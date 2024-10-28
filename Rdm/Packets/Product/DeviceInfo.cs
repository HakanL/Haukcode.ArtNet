using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                RdmProtocolVersion = data.ReadHiLoInt16();
                DeviceModelId = data.ReadHiLoInt16();
                ProductCategory = (ProductCategories) data.ReadHiLoInt16();
                SoftwareVersionId = data.ReadHiLoInt32();
                DmxFootprint = data.ReadHiLoInt16();
                DmxPersonality = data.ReadByte();
                DmxPersonalityCount = data.ReadByte();
                DmxStartAddress = data.ReadHiLoInt16();
                SubDeviceCount = data.ReadHiLoInt16();
                SensorCount = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteHiLoInt16(RdmProtocolVersion);
                data.WriteHiLoInt16(DeviceModelId);
                data.WriteHiLoInt16((short) ProductCategory);
                data.WriteHiLoInt32(SoftwareVersionId);
                data.WriteHiLoInt16(DmxFootprint);
                data.WriteByte(DmxPersonality);
                data.WriteByte(DmxPersonalityCount);
                data.WriteHiLoInt16(DmxStartAddress);
                data.WriteHiLoInt16(SubDeviceCount);
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
