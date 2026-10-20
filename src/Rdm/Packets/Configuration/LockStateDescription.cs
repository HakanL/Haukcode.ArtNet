namespace Haukcode.ArtNet.Rdm.Packets.Configuration;

/// <summary>
/// This parameter is used to get a text description, up to 32 characters, of a lock state (E1.37-1).
/// </summary>
public class LockStateDescription
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.LockStateDescription)
        {
        }

        public byte LockState { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            LockState = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(LockState);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.LockStateDescription)
        {
        }

        public byte LockState { get; set; }

        public string Description { get; set; } = string.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            LockState = data.ReadByte();
            Description = data.ReadString(Header.ParameterDataLength - 1);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(LockState);
            data.WriteString(Description);
        }
    }
}
