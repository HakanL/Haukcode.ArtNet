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
    /// Estimated response time: tenths of a second for ACK_TIMER, milliseconds for
    /// ACK_TIMER_HI_RES (see <see cref="EstimatedResponseDelay"/>).
    /// </summary>
    public ushort EstimatedResponseTime { get; set; }

    /// <summary>
    /// <see cref="EstimatedResponseTime"/> as a duration, in the unit its response type uses.
    /// </summary>
    public TimeSpan EstimatedResponseDelay => ResponseType == RdmResponseTypes.AckTimerHiRes
        ? TimeSpan.FromMilliseconds(EstimatedResponseTime)
        : TimeSpan.FromMilliseconds(EstimatedResponseTime * 100);

    #region Read and Write

    protected override void ReadData(RdmBinaryReader data)
    {
        EstimatedResponseTime = data.ReadUInt16();
    }

    protected override void WriteData(RdmBinaryWriter data)
    {
        data.WriteUInt16(EstimatedResponseTime);
    }

    #endregion

}
