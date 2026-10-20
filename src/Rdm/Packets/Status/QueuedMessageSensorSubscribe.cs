namespace Haukcode.ArtNet.Rdm.Packets.Status;

/// <summary>
/// The sensors whose values are reported through QUEUED_MESSAGE (E1.20-2025 10.3.6).
/// </summary>
public class QueuedMessageSensorSubscribe
{
    public enum Actions : byte
    {
        Remove = 0x00,
        Add = 0x01
    }

    /// <summary>Sensor number that addresses all sensors; only valid in a SET.</summary>
    public const byte AllSensors = 0xFF;

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.QueuedMessageSensorSubscribe)
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
            : base(RdmCommands.GetResponse, RdmParameters.QueuedMessageSensorSubscribe)
        {
        }

        public List<byte> Sensors { get; set; } = [];

        protected override void ReadData(RdmBinaryReader data)
        {
            Sensors = [.. data.ReadBytes(Header.ParameterDataLength)];
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteBytes([.. Sensors]);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.QueuedMessageSensorSubscribe)
        {
        }

        public Actions Action { get; set; }

        /// <summary>The sensors to add or remove; may include <see cref="AllSensors"/>.</summary>
        public List<byte> Sensors { get; set; } = [];

        protected override void ReadData(RdmBinaryReader data)
        {
            Action = (Actions)data.ReadByte();
            Sensors = [.. data.ReadBytes(Header.ParameterDataLength - 1)];
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte((byte)Action);
            data.WriteBytes([.. Sensors]);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.QueuedMessageSensorSubscribe)
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
