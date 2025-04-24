namespace Haukcode.ArtNet.Rdm;

public class UnknownRdmPacketException : NotImplementedException
{
    public UnknownRdmPacketException(RdmHeader header)
        : base(string.Format("An RDM packet with the PID 0x{0:0000} is not recognised.", ((int)header.ParameterId).ToString("X")))
    {
        Header = header;
    }

    public RdmHeader Header { get; set; }
}
