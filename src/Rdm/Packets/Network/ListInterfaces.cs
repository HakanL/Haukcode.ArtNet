namespace Haukcode.ArtNet.Rdm.Packets.Network;

/// <summary>
/// The device's IPv4 network interfaces (E1.37-2 4.1). GET only.
/// </summary>
public class ListInterfaces
{
    /// <summary>
    /// An interface: its identifier (1 to 0xFFFFFF00) and its ARP hardware type (1 = Ethernet).
    /// </summary>
    public readonly record struct NetworkInterface(uint Id, ushort HardwareType);

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.ListInterfaces)
        {
        }

        protected override void ReadData(RdmBinaryReader data)
        {
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
        }
    }

    public class GetReply : RdmResponsePacket
    {
        // Interface Identifier (32 bits) + Interface Hardware Type (16 bits)
        private const int RecordSize = 6;

        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.ListInterfaces)
        {
        }

        public List<NetworkInterface> Interfaces { get; set; } = [];

        protected override void ReadData(RdmBinaryReader data)
        {
            Interfaces = [];
            for (int n = 0; n < Header.ParameterDataLength / RecordSize; n++)
                Interfaces.Add(new NetworkInterface(data.ReadUInt32(), data.ReadUInt16()));
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            foreach (var networkInterface in Interfaces)
            {
                data.WriteUInt32(networkInterface.Id);
                data.WriteUInt16(networkInterface.HardwareType);
            }
        }
    }
}
