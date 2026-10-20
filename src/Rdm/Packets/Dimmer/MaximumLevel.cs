namespace Haukcode.ArtNet.Rdm.Packets.Dimmer;

/// <summary>
/// This parameter is used to get or set the dimmer's maximum level (E1.37-1).
/// </summary>
public class MaximumLevel
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.MaximumLevel)
        {
        }

        protected override void ReadData(RdmBinaryReader data)
        {
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.MaximumLevel)
        {
        }

        public ushort Level { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            Level = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(Level);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.MaximumLevel)
        {
        }

        public ushort Level { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            Level = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(Level);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.MaximumLevel)
        {
        }

        protected override void ReadData(RdmBinaryReader data)
        {
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
        }
    }
}
