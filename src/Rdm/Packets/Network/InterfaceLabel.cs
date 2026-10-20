namespace Haukcode.ArtNet.Rdm.Packets.Network;

/// <summary>
/// Label of a network interface (E1.37-2 4.2). GET only.
/// </summary>
public class InterfaceLabel
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.InterfaceLabel)
        {
        }

        /// <summary>Interface identifier from LIST_INTERFACES.</summary>
        public uint InterfaceId { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            InterfaceId = data.ReadUInt32();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt32(InterfaceId);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.InterfaceLabel)
        {
        }

        /// <summary>Interface identifier from LIST_INTERFACES.</summary>
        public uint InterfaceId { get; set; }

        /// <summary>Up to 32 characters.</summary>
        public string Label { get; set; } = string.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            InterfaceId = data.ReadUInt32();
            Label = data.ReadString(Header.ParameterDataLength - 4);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt32(InterfaceId);
            data.WriteString(Label, 32);
        }
    }
}
