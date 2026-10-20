namespace Haukcode.ArtNet.Rdm.Packets.Parameters;

/// <summary>
/// Get or set one PID for a range of items (e.g. every sensor) on one device in a single message
/// (E1.20-2025 10.4.8). With ACK_OVERFLOW only the first message carries the PID; read the sequence
/// with <see cref="RdmPacket.ReadOverflowPacket"/>.
/// </summary>
public class PackedPidIndex
{
    /// <summary>Item count asking for all available data.</summary>
    public const ushort AllItems = 0xFFFF;

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.PackedPidIndex)
        {
        }

        public RdmParameters ParameterId { get; set; }

        public ushort FirstItem { get; set; }

        /// <summary>Number of items, or <see cref="AllItems"/>.</summary>
        public ushort ItemCount { get; set; } = AllItems;

        protected override void ReadData(RdmBinaryReader data)
        {
            ParameterId = (RdmParameters)data.ReadUInt16();
            FirstItem = data.ReadUInt16();
            ItemCount = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16((ushort)ParameterId);
            data.WriteUInt16(FirstItem);
            data.WriteUInt16(ItemCount);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.PackedPidIndex)
        {
        }

        public RdmParameters ParameterId { get; set; }

        /// <summary>Per item: that PID's parameter data. Items need not be contiguous.</summary>
        public List<PackedPidEntry> Entries { get; set; } = [];

        protected override void ReadData(RdmBinaryReader data)
        {
            ParameterId = (RdmParameters)data.ReadUInt16();
            Entries = PackedPidEntry.ReadList(data, Header.ParameterDataLength - 2);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16((ushort)ParameterId);
            PackedPidEntry.WriteList(data, Entries);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.PackedPidIndex)
        {
        }

        public RdmParameters ParameterId { get; set; }

        /// <summary>Per item: the parameter data to set.</summary>
        public List<PackedPidEntry> Entries { get; set; } = [];

        protected override void ReadData(RdmBinaryReader data)
        {
            ParameterId = (RdmParameters)data.ReadUInt16();
            Entries = PackedPidEntry.ReadList(data, Header.ParameterDataLength - 2);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16((ushort)ParameterId);
            PackedPidEntry.WriteList(data, Entries);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.PackedPidIndex)
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
