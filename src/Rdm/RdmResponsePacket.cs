namespace Haukcode.ArtNet.Rdm;

public abstract class RdmResponsePacket : RdmPacket
{
    public RdmResponsePacket(RdmCommands command, RdmParameters parameterId)
        : base(command, parameterId)
    {
    }

    public RdmResponseTypes ResponseType { get; set; }
}
