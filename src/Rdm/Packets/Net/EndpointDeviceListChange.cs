namespace Haukcode.ArtNet.Rdm.Packets.Net;

public class EndpointDeviceListChange
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.EndpointDeviceListChange)
        {
        }

        public ushort EndpointID { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
        }
    }

    public class Reply : RdmResponsePacket
    {
        public Reply()
            : base(RdmCommands.GetResponse, RdmParameters.EndpointDeviceListChange)
        {
        }

        public ushort EndpointID { get; set; }

        public uint ListChangeNumber { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
            ListChangeNumber = data.ReadUInt32();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
            data.WriteUInt32(ListChangeNumber);
        }
    }
}
