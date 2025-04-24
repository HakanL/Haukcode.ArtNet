namespace Haukcode.ArtNet.Rdm.Packets.Product;

/// <summary>
/// This parameter is used to retrieve the unique Boot Software Version ID for the device. The Boot
/// Software Version ID is a 32-bit value determined by the Manufacturer.
/// </summary>
/// <remarks>
/// The 32-bit Boot Software Version ID may use any encoding scheme such that the Controller may
/// identify devices containing the same boot software versions.
/// 
/// Any devices from the same manufacturer with differing boot software shall not report back the
/// same Boot Software Version ID.
/// </remarks>
public class BootSoftwareVersionId
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get,RdmParameters.BootSoftwareVersionId)
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
            : base(RdmCommands.GetResponse, RdmParameters.BootSoftwareVersionId)
        {
        }

        public int VersionId { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            VersionId = data.ReadHiLoInt32();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteHiLoInt32(VersionId);
        }

        #endregion
    }
}
