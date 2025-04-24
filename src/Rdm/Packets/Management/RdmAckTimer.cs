namespace Haukcode.ArtNet.Rdm.Packets.Management;

public class RdmAckTimer : RdmPacket
{
    public RdmAckTimer()
    {
    }

    public RdmAckTimer(RdmCommands command, RdmParameters parameterId)
        : base(command, parameterId)
    {
        Header.PortOrResponseType = (byte)RdmResponseTypes.AckTimer;
    }

    public short EstimatedResponseTime { get; set; }

    #region Read and Write

    protected override void ReadData(RdmBinaryReader data)
    {
        EstimatedResponseTime = data.ReadInt16();
    }

    protected override void WriteData(RdmBinaryWriter data)
    {
        data.WriteUInt16(EstimatedResponseTime);
    }

    #endregion

}
