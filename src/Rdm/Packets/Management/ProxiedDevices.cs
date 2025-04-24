namespace Haukcode.ArtNet.Rdm.Packets.Management;

/// <summary>
/// This parameter is used to retrieve the UIDs from a device identified as a proxy during discovery.
/// </summary>
/// <remarks>
/// The response to this parameter contains a packed list of 48-bit UIDs for all devices represented
/// by the proxy.
/// 
/// If there are no current devices being proxied then the Parameter Data Length field shall be returned as 0x00.
/// </remarks>
public class ProxiedDevices
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.ProxiedDevices)
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
            : base(RdmCommands.GetResponse, RdmParameters.ProxiedDevices)
        {
            DeviceIds = new List<UId>();
        }

        /// <summary>
        /// A list of device ids for devices discovered by the proxy.
        /// </summary>
        public List<UId> DeviceIds { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            for (int n = 0; n < Header.ParameterDataLength / 6; n++)
            {
                DeviceIds.Add(data.ReadUId());
            }
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            foreach (UId id in DeviceIds)
                data.WriteUid(id);
        }

        #endregion
    }
}
