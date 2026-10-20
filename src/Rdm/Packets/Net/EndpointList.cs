namespace Haukcode.ArtNet.Rdm.Packets.Net;

/// <summary>
/// This parameter is used to retrieve a packed list of all endpoints on a gateway, with the
/// exception of the Management Endpoint (E1.37-7 §6.1).
/// </summary>
/// <remarks>
/// The list of Endpoint IDs shall not include the Management Endpoint ID. If the device does not have
/// any Endpoints (other than the Management Endpoint) then it shall return a PDL of 0.
/// </remarks>
public class EndpointList
{
    /// <summary>
    /// Endpoint types (E1.37-7 Table A-5).
    /// </summary>
    public enum EndpointTypes : byte
    {
        Virtual = 0x00,
        Physical = 0x01
    }

    public readonly record struct Endpoint(ushort Id, EndpointTypes Type);

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.EndpointList)
        {
        }

        protected override void ReadData(RdmBinaryReader data)
        {
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
        }
    }

    /// <remarks>
    /// This reads the first message of a response. With ACK_OVERFLOW the following messages carry
    /// only endpoint records, without the list change number.
    /// </remarks>
    public class Reply : RdmResponsePacket
    {
        // Endpoint ID (16 bits) + Endpoint Type (8 bits)
        private const int EndpointRecordSize = 3;

        public Reply()
            : base(RdmCommands.GetResponse, RdmParameters.EndpointList)
        {
        }

        public uint ListChangeNumber { get; set; }

        public List<Endpoint> Endpoints { get; set; } = [];

        protected override void ReadData(RdmBinaryReader data)
        {
            ListChangeNumber = data.ReadUInt32();

            Endpoints = [];
            for (int n = 0; n < (Header.ParameterDataLength - 4) / EndpointRecordSize; n++)
                Endpoints.Add(new Endpoint(data.ReadUInt16(), (EndpointTypes)data.ReadByte()));
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt32(ListChangeNumber);
            foreach (var endpoint in Endpoints)
            {
                data.WriteUInt16(endpoint.Id);
                data.WriteByte((byte)endpoint.Type);
            }
        }
    }
}
