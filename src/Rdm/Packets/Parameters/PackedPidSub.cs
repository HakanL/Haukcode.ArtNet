namespace Haukcode.ArtNet.Rdm.Packets.Parameters;

/// <summary>
/// Get or set one PID across a range of sub-devices in a single message (E1.20-2025 10.4.7). Sent to
/// the root device. With ACK_OVERFLOW only the first message carries the PID and index; read the
/// sequence with <see cref="RdmPacket.ReadOverflowPacket"/>.
/// </summary>
public class PackedPidSub
{
    /// <summary>Device count meaning all sub-devices.</summary>
    public const ushort AllDevices = 0xFFFF;

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.PackedPidSub)
        {
        }

        public RdmParameters ParameterId { get; set; }

        /// <summary>Index for indexed PIDs (e.g. the sensor number), otherwise 0.</summary>
        public ushort Index { get; set; }

        /// <summary>First device, 0x0000 (root) to 0xFFF0.</summary>
        public ushort FirstDevice { get; set; }

        /// <summary>Number of devices, 1 to 0xFFF1, or <see cref="AllDevices"/>.</summary>
        public ushort DeviceCount { get; set; } = AllDevices;

        protected override void ReadData(RdmBinaryReader data)
        {
            ParameterId = (RdmParameters)data.ReadUInt16();
            Index = data.ReadUInt16();
            FirstDevice = data.ReadUInt16();
            DeviceCount = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16((ushort)ParameterId);
            data.WriteUInt16(Index);
            data.WriteUInt16(FirstDevice);
            data.WriteUInt16(DeviceCount);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.PackedPidSub)
        {
        }

        public RdmParameters ParameterId { get; set; }

        public ushort Index { get; set; }

        /// <summary>Per device (0x0000 = root): that PID's parameter data.</summary>
        public List<PackedPidEntry> Entries { get; set; } = [];

        protected override void ReadData(RdmBinaryReader data)
        {
            ParameterId = (RdmParameters)data.ReadUInt16();
            Index = data.ReadUInt16();
            Entries = PackedPidEntry.ReadList(data, Header.ParameterDataLength - 4);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16((ushort)ParameterId);
            data.WriteUInt16(Index);
            PackedPidEntry.WriteList(data, Entries);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.PackedPidSub)
        {
        }

        public RdmParameters ParameterId { get; set; }

        public ushort Index { get; set; }

        /// <summary>Per device (0x0000 = root): the parameter data to set.</summary>
        public List<PackedPidEntry> Entries { get; set; } = [];

        protected override void ReadData(RdmBinaryReader data)
        {
            ParameterId = (RdmParameters)data.ReadUInt16();
            Index = data.ReadUInt16();
            Entries = PackedPidEntry.ReadList(data, Header.ParameterDataLength - 4);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16((ushort)ParameterId);
            data.WriteUInt16(Index);
            PackedPidEntry.WriteList(data, Entries);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.PackedPidSub)
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
