namespace Haukcode.ArtNet.Rdm;

public class RdmHeader
{
    public const int Size = 8;

    /// <summary>
    /// Most parameter data one RDM message can carry; larger responses are split with ACK_OVERFLOW.
    /// </summary>
    public const int MaxParameterDataLength = 231;

    public RdmHeader()
    {
        DestinationId = UId.Empty;
        SourceId = UId.Empty;
    }

    #region Packet Contents

    public byte MessageLength { get; set; }

    public UId DestinationId { get; set; }

    public UId SourceId { get; set; }

    public byte TransactionNumber { get; set; }

    public byte PortOrResponseType { get; set; }

    /// <summary>
    /// In a response, the number of queued messages. In a request, E1.20-2025 calls this byte
    /// Controller Flags (see <see cref="ControllerFlags"/>).
    /// </summary>
    public byte MessageCount { get; set; }

    /// <summary>
    /// Request-side view of the <see cref="MessageCount"/> byte (E1.20-2025 6.2.8).
    /// </summary>
    public RdmControllerFlags ControllerFlags
    {
        get => (RdmControllerFlags)MessageCount;
        set => MessageCount = (byte)value;
    }

    public ushort SubDevice { get; set; }

    public RdmCommands Command { get; set; }

    public RdmParameters ParameterId { get; set; }

    /// <summary>
    /// Length of the parameter data. Set when the header is read and when the packet is written;
    /// set it yourself when parsing a body with <see cref="RdmPacket.ReadPacket(RdmHeader, RdmBinaryReader)"/>.
    /// </summary>
    public byte ParameterDataLength { get; set; }

    #endregion

    #region Read and Write

    public void ReadData(RdmBinaryReader data)
    {
        MessageLength = data.ReadByte();
        DestinationId = data.ReadUId();
        SourceId = data.ReadUId();
        TransactionNumber = data.ReadByte();
        PortOrResponseType = data.ReadByte();
        MessageCount = data.ReadByte();
        SubDevice = data.ReadUInt16();
        Command = (RdmCommands)data.ReadByte();
        ParameterId = (RdmParameters)data.ReadUInt16();
        ParameterDataLength = data.ReadByte();
    }

    private long messageLengthPosition = -1;
    private long dataLengthPosition = 0;

    public void WriteData(RdmBinaryWriter data)
    {
        // Save position so we can write length later.
        messageLengthPosition = data.BaseStream.Position;

        data.WriteByte(MessageLength);
        data.WriteUid(DestinationId);
        data.WriteUid(SourceId);
        data.WriteByte(TransactionNumber);
        data.WriteByte(PortOrResponseType);
        data.WriteByte(MessageCount);
        data.WriteUInt16(SubDevice);
        data.WriteByte((byte)Command);
        data.WriteUInt16((short)ParameterId);

        // Save position so we can write length later.
        dataLengthPosition = data.BaseStream.Position;
        data.WriteByte(ParameterDataLength);
    }

    public void WriteLength(RdmBinaryWriter data)
    {
        if (messageLengthPosition < 0 || dataLengthPosition <= 0)
            throw new InvalidOperationException("Packet data has not been written yet. You can not write the length until the body is written.");

        long parameterDataLength = (data.BaseStream.Position - 1) - dataLengthPosition;
        if (parameterDataLength > MaxParameterDataLength)
            throw new InvalidOperationException($"The parameter data is {parameterDataLength} bytes but an RDM message carries at most {MaxParameterDataLength}; split larger responses with ACK_OVERFLOW.");

        MessageLength = (byte)(data.BaseStream.Position - messageLengthPosition + 2);
        ParameterDataLength = (byte)parameterDataLength;

        //Write Message Length
        data.BaseStream.Seek(messageLengthPosition, System.IO.SeekOrigin.Begin);
        data.WriteByte(MessageLength);

        //Write Parameter Data Length
        data.BaseStream.Seek(dataLengthPosition, System.IO.SeekOrigin.Begin);
        data.WriteByte(ParameterDataLength);

        //Move back to end of stream.
        data.BaseStream.Seek(0, System.IO.SeekOrigin.End);
    }
    
    #endregion
}
