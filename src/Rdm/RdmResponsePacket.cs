namespace Haukcode.ArtNet.Rdm;

public abstract class RdmResponsePacket : RdmPacket
{
    protected RdmResponsePacket()
    {
        Header = new RdmHeader();
    }

    public RdmResponsePacket(RdmCommands command, RdmParameters parameterId)
        : base(command, parameterId)
    {
    }

    /// <summary>
    /// Response type is the same byte as the request Port ID. Keep it as a view
    /// of the header so parsing and writing cannot get out of sync.
    /// </summary>
    public RdmResponseTypes ResponseType
    {
        get => (RdmResponseTypes)Header.PortOrResponseType;
        set => Header.PortOrResponseType = (byte)value;
    }
}
