namespace Haukcode.ArtNet.Rdm.Packets.Net;

/// <summary>
/// This parameter is used to get or set the policy a gateway uses to collect queued and status
/// messages in the background (E1.37-7 §7.1). It applies to the root device or a sub-device, not to
/// an endpoint.
/// </summary>
public class BackgroundQueuedStatusPolicy
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.BackgroundQueuedStatusPolicy)
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
            : base(RdmCommands.GetResponse, RdmParameters.BackgroundQueuedStatusPolicy)
        {
        }

        public byte CurrentPolicyID { get; set; }

        public byte PolicyCount { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            CurrentPolicyID = data.ReadByte();
            PolicyCount = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(CurrentPolicyID);
            data.WriteByte(PolicyCount);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.BackgroundQueuedStatusPolicy)
        {
        }

        public byte PolicyID { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            PolicyID = data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte(PolicyID);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.BackgroundQueuedStatusPolicy)
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
