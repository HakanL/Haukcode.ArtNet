namespace Haukcode.ArtNet.Rdm.Packets.Dimmer;

/// <summary>
/// This parameter is used to get or select the dimmer curve (E1.37-1). Curves are numbered from 1;
/// <see cref="CurveDescription"/> describes each one.
/// </summary>
public class Curve
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.Curve)
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
            : base(RdmCommands.GetResponse, RdmParameters.Curve)
        {
        }

        public byte CurrentCurve { get; set; }

        public byte CurveCount { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            CurrentCurve = data.ReadByte();
            CurveCount = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(CurrentCurve);
            data.WriteByte(CurveCount);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.Curve)
        {
        }

        public byte CurveNumber { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            CurveNumber = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(CurveNumber);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.Curve)
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
