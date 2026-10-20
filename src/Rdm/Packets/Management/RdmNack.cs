namespace Haukcode.ArtNet.Rdm.Packets.Management;

public class RdmNack : RdmResponsePacket
{
    public RdmNack()
    {
    }

    public RdmNack(RdmCommands command, RdmParameters parameterId)
        : base(command, parameterId)
    {
        ResponseType = RdmResponseTypes.NackReason;
    }

    public NackReason Reason { get; set; }

    #region Read and Write

    protected override void ReadData(RdmBinaryReader data)
    {
        Reason = (NackReason)data.ReadUInt16();
    }

    protected override void WriteData(RdmBinaryWriter data)
    {
        // The NACK reason code is 16 bits on the wire
        data.WriteUInt16((ushort)Reason);
    }

    #endregion
}
