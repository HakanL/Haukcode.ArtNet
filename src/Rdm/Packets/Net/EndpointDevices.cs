namespace Haukcode.ArtNet.Rdm.Packets.Net;

public class EndpointDevices
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.EndpointDevices)
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
            : base(RdmCommands.GetResponse, RdmParameters.EndpointDevices)
        {
        }

        public ushort EndpointID { get; set; }

        public uint ListChangeNumber { get; set; }

        private List<UId> deviceIds = new List<UId>();

        public List<UId> DeviceIds
        {
            get { return deviceIds; }
            set { deviceIds = value; }
        }


        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
            ListChangeNumber = data.ReadUInt32();

            for (int n = 0; n < (Header.ParameterDataLength - 6) / 6; n++)
            {
                DeviceIds.Add(data.ReadUId());
            }
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
            data.WriteUInt32(ListChangeNumber);

            foreach (UId id in DeviceIds)
                data.WriteUid(id);
        }
    }
}
