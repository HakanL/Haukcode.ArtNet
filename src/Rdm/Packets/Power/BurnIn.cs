namespace Haukcode.ArtNet.Rdm.Packets.Power;

/// <summary>
/// This parameter is used to burn in a lamp for a number of hours (E1.37-1). The reply holds the
/// hours remaining, 0 when no burn in is running.
/// </summary>
public class BurnIn
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.BurnIn)
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
            : base(RdmCommands.GetResponse, RdmParameters.BurnIn)
        {
        }

        public byte HoursRemaining { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            HoursRemaining = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(HoursRemaining);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.BurnIn)
        {
        }

        public byte Hours { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            Hours = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(Hours);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.BurnIn)
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
