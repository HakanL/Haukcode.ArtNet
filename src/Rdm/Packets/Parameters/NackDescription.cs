namespace Haukcode.ArtNet.Rdm.Packets.Parameters;

/// <summary>
/// Text description of a manufacturer-specific NACK reason code, 0x8000 to 0xFFFF
/// (E1.20-2025 10.4.6). GET only, root device only.
/// </summary>
public class NackDescription
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.NackDescription)
        {
        }

        public ushort ReasonCode { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            ReasonCode = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(ReasonCode);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.NackDescription)
        {
        }

        public ushort ReasonCode { get; set; }

        public string Description { get; set; } = string.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            ReasonCode = data.ReadUInt16();
            Description = data.ReadString(Header.ParameterDataLength - 2);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(ReasonCode);
            data.WriteString(Description);
        }
    }
}
