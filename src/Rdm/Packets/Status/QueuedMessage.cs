namespace Haukcode.ArtNet.Rdm.Packets.Status;

/// <summary>
/// Obtains a message from the responders message queue.
/// </summary>
/// <remarks>
/// The Message Count field of all response messages defines the number of
/// messages that are queued in the responder. Each <see cref="QueuedMessage"/> response shall be
/// composed of a single message response.
/// 
/// A responder with multiple messages queued shall first respond with the most urgent message.
/// The message count of the responder shall be decremented prior to sending the response.
/// 
/// A responder with no messages queued shall respond to a <see cref="QueuedMessage"/> message with a
/// <see cref="StatusMessage"/> response. A StatusMessage response with a PDL of 0x00 does not
/// imply that the responder supports the <see cref="StatusMessage"/> PID.
/// </remarks>
public class QueuedMessage
{
    /// <summary>
    /// Requests that the device sends a queued message.
    /// </summary>
    /// <remarks>
    /// The response to this message is the queued message.
    /// </remarks>
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.QueuedMessage)
        {
        }

        /// <summary>
        /// Determines what queued message the device should send.
        /// </summary>
        public StatusTypes StatusType { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            StatusType = (StatusTypes)data.ReadByte(); ;
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteByte((byte) StatusType);
        }

        #endregion
    }
}
