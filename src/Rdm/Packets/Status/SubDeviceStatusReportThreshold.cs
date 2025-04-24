namespace Haukcode.ArtNet.Rdm.Packets.Status;

/// <summary>
/// This parameter is used to set the verbosity of Sub-Device reporting using the Status Type codes.
/// </summary>
/// <remarks>
/// This feature is used to inhibit reports from, for example, a specific dimmer in a rack that is
/// generating repeated errors.
/// </remarks>
public class SubDeviceStatusReportThreshold
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.SubDeviceStatusReportThreshold)
        {
        }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
        }

        #endregion
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.SubDeviceStatusReportThreshold)
        {
        }

        /// <summary>
        /// The status type being inhibited.
        /// </summary>
        public StatusTypes StatusType { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            StatusType = (StatusTypes)data.ReadByte(); ;
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte((byte)StatusType);
        }

        #endregion
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.SubDeviceStatusReportThreshold)
        {
        }

        /// <summary>
        /// The status type to inhibit.
        /// </summary>
        public StatusTypes StatusType { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            StatusType = (StatusTypes)data.ReadByte(); ;
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte((byte)StatusType);
        }

        #endregion
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.SubDeviceStatusReportThreshold)
        {
        }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
        }

        #endregion
    }
}
