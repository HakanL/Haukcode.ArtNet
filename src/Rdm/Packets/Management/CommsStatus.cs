namespace Haukcode.ArtNet.Rdm.Packets.Management;

/// <summary>
/// Used to collect information that may be useful in analyzing the integrity of the communication system.
/// </summary>
/// <remarks>
/// A responder shall respond with a cumulative total of each error type in the response message defined below.
/// </remarks>
public class CommsStatus
{
    /// <summary>
    /// Requests information about the amount of errors encountered by a device.
    /// </summary>
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.CommsStatus)
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

    /// <summary>
    /// Contains information aabout the amount of errors encountered by a device.
    /// </summary>
    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.CommsStatus)
        {
        }

        /// <summary>
        /// The message ended before the Message Length field was received.
        /// </summary>
        public short ShortMessage { get; set; }

        /// <summary>
        /// The number of slots actually received did not match the Message Length plus
        /// the size of the Checksum.
        /// </summary>
        /// <remarks>
        /// This counter shall only be incremented if the Destination UID in the
        /// packet matches the Device’s UID.
        /// </remarks>
        public short LengthMismatch { get; set; }

        /// <summary>
        /// The message checksum failed.
        /// </summary>
        /// <remarks>
        /// This counter shall only be incremented if the Destination UID in the packet matches the Device’s UID.
        /// </remarks>
        public short ChecksumFail { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            ShortMessage = data.ReadInt16();
            LengthMismatch = data.ReadInt16();
            ChecksumFail = data.ReadInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(ShortMessage);
            data.WriteUInt16(LengthMismatch);
            data.WriteUInt16(ChecksumFail);
        }

        #endregion
    }

    /// <summary>
    /// Clears all the error counts to zero.
    /// </summary>
    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.CommsStatus)
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

    /// <summary>
    /// Confirmation that the error counts have been cleared to zero.
    /// </summary>
    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.CommsStatus)
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
