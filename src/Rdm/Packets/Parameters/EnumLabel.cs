namespace Haukcode.ArtNet.Rdm.Packets.Parameters;

/// <summary>
/// Text label for a value of a manufacturer-specific enumerated parameter (E1.20-2025 10.4.3).
/// GET only.
/// </summary>
public class EnumLabel
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.EnumLabel)
        {
        }

        /// <summary>The manufacturer-specific PID, 0x8000 to 0xFFDF.</summary>
        public RdmParameters ParameterId { get; set; }

        public uint EnumerationIndex { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            ParameterId = (RdmParameters)data.ReadUInt16();
            EnumerationIndex = data.ReadUInt32();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16((ushort)ParameterId);
            data.WriteUInt32(EnumerationIndex);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.EnumLabel)
        {
        }

        public RdmParameters ParameterId { get; set; }

        public uint EnumerationIndex { get; set; }

        public uint MaximumEnumerationIndex { get; set; }

        public string Label { get; set; } = string.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            ParameterId = (RdmParameters)data.ReadUInt16();
            EnumerationIndex = data.ReadUInt32();
            MaximumEnumerationIndex = data.ReadUInt32();
            Label = data.ReadString(Header.ParameterDataLength - 10);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16((ushort)ParameterId);
            data.WriteUInt32(EnumerationIndex);
            data.WriteUInt32(MaximumEnumerationIndex);
            data.WriteString(Label);
        }
    }
}
