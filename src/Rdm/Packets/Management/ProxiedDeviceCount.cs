namespace Haukcode.ArtNet.Rdm.Packets.Management;

/// <summary>
/// This parameter is used to identify the number of devices being represented by a proxy and
/// whether the list of represented device UIDs has changed.
/// </summary>
/// <remarks>
/// If the List Change flag is set then the controller should request the procied devices.
/// 
/// The device will automatically clear the List Change flag after all the proxied UID’s have been
/// retrieved using the ProxiedDevice message.
/// 
/// A proxy device shall indicate any change in it's device list through a QueuedMessage.
/// </remarks>
public class ProxiedDeviceCount
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.ProxiedDeviceCount)
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
            : base(RdmCommands.GetResponse, RdmParameters.ProxiedDeviceCount)
        {
        }

        /// <summary>
        /// The number of proxied devices connected to this proxy and discovered.
        /// </summary>
        public short DeviceCount { get; set; }

        /// <summary>
        /// Whether the list of proxied devices has changed since the list was last obtained.
        /// </summary>
        public bool ListChanged { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            DeviceCount = data.ReadInt16();
            ListChanged = data.ReadBool();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(DeviceCount);
            data.WriteBool(ListChanged);
        }

        #endregion
    }
}
