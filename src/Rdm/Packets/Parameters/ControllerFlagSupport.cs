namespace Haukcode.ArtNet.Rdm.Packets.Parameters;

/// <summary>
/// The Controller Flags a responder supports (E1.20-2025 10.4.5). A controller only sets a flag in
/// <see cref="RdmHeader.ControllerFlags"/> once the responder has confirmed it here. GET only, root
/// device only.
/// </summary>
public class ControllerFlagSupport
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.ControllerFlagSupport)
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
            : base(RdmCommands.GetResponse, RdmParameters.ControllerFlagSupport)
        {
        }

        public RdmControllerFlags SupportedFlags { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            SupportedFlags = (RdmControllerFlags)data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte((byte)SupportedFlags);
        }
    }
}
