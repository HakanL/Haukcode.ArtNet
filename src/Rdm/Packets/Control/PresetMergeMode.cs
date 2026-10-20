namespace Haukcode.ArtNet.Rdm.Packets.Control;

/// <summary>
/// This parameter is used to get or set how a playing preset merges with the DMX512 input (E1.37-1).
/// </summary>
public class PresetMergeMode
{
    public enum MergeModes : byte
    {
        Default = 0x00,
        Htp = 0x01,
        Ltp = 0x02,
        DmxOnly = 0x03,
        Other = 0xFF
    }

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.PresetMergeMode)
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
            : base(RdmCommands.GetResponse, RdmParameters.PresetMergeMode)
        {
        }

        public MergeModes MergeMode { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            MergeMode = (MergeModes)data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte((byte)MergeMode);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.PresetMergeMode)
        {
        }

        public MergeModes MergeMode { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            MergeMode = (MergeModes)data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte((byte)MergeMode);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.PresetMergeMode)
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
