namespace Haukcode.ArtNet.Rdm.Packets.Control;

/// <summary>
/// This parameter is used to get or set whether the device runs its self test at power on (E1.37-1).
/// </summary>
public class PowerOnSelfTest
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.PowerOnSelfTest)
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
            : base(RdmCommands.GetResponse, RdmParameters.PowerOnSelfTest)
        {
        }

        public bool Enabled { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            Enabled = data.ReadBool();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteBool(Enabled);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.PowerOnSelfTest)
        {
        }

        public bool Enabled { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            Enabled = data.ReadBool();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteBool(Enabled);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.PowerOnSelfTest)
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
