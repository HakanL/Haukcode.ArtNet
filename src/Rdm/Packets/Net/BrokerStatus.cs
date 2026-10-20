namespace Haukcode.ArtNet.Rdm.Packets.Net;

/// <summary>
/// State of an RDMnet Broker, and enabling or disabling it (E1.33 7.6.4). Root device only.
/// </summary>
public class BrokerStatus
{
    /// <summary>Broker state (E1.33 Table A-18).</summary>
    public enum BrokerStates : byte
    {
        Disabled = 0x00,
        Active = 0x01,
        Standby = 0x02
    }

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.BrokerStatus)
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
            : base(RdmCommands.GetResponse, RdmParameters.BrokerStatus)
        {
        }

        /// <summary>Whether the Broker accepts a SET of this parameter.</summary>
        public bool SetAllowed { get; set; }

        public BrokerStates State { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            SetAllowed = data.ReadBool();
            State = (BrokerStates)data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteBool(SetAllowed);
            data.WriteByte((byte)State);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.BrokerStatus)
        {
        }

        /// <summary>Disabled or Active; Standby is not valid in a SET.</summary>
        public BrokerStates State { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            State = (BrokerStates)data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte((byte)State);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.BrokerStatus)
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
