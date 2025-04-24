namespace Haukcode.ArtNet.Rdm.Packets.DMX;

public class DmxPersonality
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.DmxPersonality)
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
            : base(RdmCommands.GetResponse, RdmParameters.DmxPersonality)
        {
        }

        public byte CurrentPersonalityIndex { get; set; }

        public byte PersonalityCount { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            CurrentPersonalityIndex = data.ReadByte();
            PersonalityCount = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(CurrentPersonalityIndex);
            data.WriteByte(PersonalityCount);
        }

        #endregion
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.DmxPersonality)
        {
        }

        public byte PersonalityIndex { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            PersonalityIndex = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(PersonalityIndex);
        }

        #endregion
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.DmxPersonality)
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
