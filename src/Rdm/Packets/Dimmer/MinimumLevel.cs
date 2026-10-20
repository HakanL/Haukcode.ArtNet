namespace Haukcode.ArtNet.Rdm.Packets.Dimmer;

/// <summary>
/// This parameter is used to get or set the dimmer's minimum level (E1.37-1).
/// </summary>
public class MinimumLevel
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.MinimumLevel)
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
            : base(RdmCommands.GetResponse, RdmParameters.MinimumLevel)
        {
        }

        public ushort MinimumLevelIncreasing { get; set; }

        public ushort MinimumLevelDecreasing { get; set; }

        public bool OnBelowMinimum { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            MinimumLevelIncreasing = data.ReadUInt16();
            MinimumLevelDecreasing = data.ReadUInt16();
            OnBelowMinimum = data.ReadBool();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(MinimumLevelIncreasing);
            data.WriteUInt16(MinimumLevelDecreasing);
            data.WriteBool(OnBelowMinimum);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.MinimumLevel)
        {
        }

        public ushort MinimumLevelIncreasing { get; set; }

        public ushort MinimumLevelDecreasing { get; set; }

        public bool OnBelowMinimum { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            MinimumLevelIncreasing = data.ReadUInt16();
            MinimumLevelDecreasing = data.ReadUInt16();
            OnBelowMinimum = data.ReadBool();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(MinimumLevelIncreasing);
            data.WriteUInt16(MinimumLevelDecreasing);
            data.WriteBool(OnBelowMinimum);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.MinimumLevel)
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
