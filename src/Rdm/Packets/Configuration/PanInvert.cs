namespace Haukcode.ArtNet.Rdm.Packets.Configuration;

/// <summary>
/// This parameter is used to retrieve or change the Pan Invert setting.
/// </summary>
public class PanInvert
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.PanInvert)
        {
        }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
        }

        #endregion
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.PanInvert)
        {
        }

        public bool Inverted { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            Inverted = data.ReadBool();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteBool(Inverted);
        }

        #endregion
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.PanInvert)
        {
        }

        public bool Inverted { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            Inverted = data.ReadBool();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteBool(Inverted);
        }

        #endregion
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.PanInvert)
        {
        }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
        }

        #endregion
    }
}
