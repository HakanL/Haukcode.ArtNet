namespace Haukcode.ArtNet.Rdm;

public abstract class RdmRequestPacket : RdmPacket
{
    public RdmRequestPacket(RdmCommands command, RdmParameters parameterId)
        : base(command, parameterId)
    {
    }

    /// <summary>
    /// The Port ID shares its header byte with the response type. Keep it as a view of the
    /// header so parsing and writing cannot get out of sync.
    /// </summary>
    public byte PortId
    {
        get => Header.PortOrResponseType;
        set => Header.PortOrResponseType = value;
    }
}
