namespace Haukcode.ArtNet.Rdm.Packets.Dimmer;

/// <summary>
/// This parameter is used to get the frequency and a text description, up to 32 characters, of a
/// modulation frequency setting (E1.37-1). GET only.
/// </summary>
public class ModulationFrequencyDescription
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.ModulationFrequencyDescription)
        {
        }

        public byte Setting { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            Setting = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(Setting);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.ModulationFrequencyDescription)
        {
        }

        public byte Setting { get; set; }

        /// <summary>
        /// Frequency in Hz, 0xFFFFFFFF when not declared.
        /// </summary>
        public uint Frequency { get; set; }

        public string Description { get; set; } = string.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            Setting = data.ReadByte();
            Frequency = data.ReadUInt32();
            Description = data.ReadString(Header.ParameterDataLength - 5);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(Setting);
            data.WriteUInt32(Frequency);
            data.WriteString(Description);
        }
    }
}
