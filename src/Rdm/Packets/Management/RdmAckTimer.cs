namespace Haukcode.ArtNet.Rdm.Packets.Management;

public class RdmAckTimer : RdmResponsePacket
{
    public RdmAckTimer()
    {
    }

    public RdmAckTimer(RdmCommands command, RdmParameters parameterId)
        : base(command, parameterId)
    {
        ResponseType = RdmResponseTypes.AckTimer;
    }

    /// <summary>
    /// Estimated response time in tenths of a second (100 ms increments).
    /// </summary>
    public ushort EstimatedResponseTime { get; set; }

    #region Read and Write

    protected override void ReadData(RdmBinaryReader data)
    {
        EstimatedResponseTime = unchecked((ushort)data.ReadInt16());
    }

    protected override void WriteData(RdmBinaryWriter data)
    {
        data.WriteUInt16(EstimatedResponseTime);
    }

    #endregion
}
