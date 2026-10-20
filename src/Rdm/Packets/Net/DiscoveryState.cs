namespace Haukcode.ArtNet.Rdm.Packets.Net;

public class DiscoveryState
{
    public enum DiscoveryStates
    {
        Incomplete = 0x0,
        Incremental = 0x1,
        Full = 0x2,
        NotActive = 0x4
    }

    /// <summary>Device count while discovery hasn't completed.</summary>
    public const ushort DeviceCountIncomplete = 0x0000;

    /// <summary>Device count when the gateway can't tell how many devices it has found.</summary>
    public const ushort DeviceCountUnknown = 0xFFFF;

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.DiscoveryState)
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

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.DiscoveryState)
        {
        }

        public ushort EndpointID { get; set; }

        public ushort DeviceCount { get; set; }

        public DiscoveryStates DiscoveryState { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
            DeviceCount = data.ReadUInt16();
            DiscoveryState = (DiscoveryStates)data.ReadByte();

        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
            data.WriteUInt16(DeviceCount);
            data.WriteByte((byte)DiscoveryState);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.DiscoveryState)
        {
        }

        public ushort EndpointID { get; set; }

        public DiscoveryStates DiscoveryState { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
            DiscoveryState = (DiscoveryStates)data.ReadByte();

        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
            data.WriteByte((byte)DiscoveryState);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.DiscoveryState)
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
}
