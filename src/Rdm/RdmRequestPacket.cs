namespace Haukcode.ArtNet.Rdm;

public abstract class RdmRequestPacket : RdmPacket
{
    /// <summary>
    /// Port ID used unless one is set: E1.20 §6.2.7.1 requires 1-255 and responders NACK 0.
    /// </summary>
    public const byte DefaultPortId = 1;

    public RdmRequestPacket(RdmCommands command, RdmParameters parameterId)
        : base(command, parameterId)
    {
        PortId = DefaultPortId;
    }

    /// <summary>
    /// The controller port the request is sent from, 1-255. It shares its header byte with the
    /// response type, so it's a view of the header and parsing and writing can't get out of sync.
    /// </summary>
    public byte PortId
    {
        get => Header.PortOrResponseType;
        set => Header.PortOrResponseType = value;
    }
}
