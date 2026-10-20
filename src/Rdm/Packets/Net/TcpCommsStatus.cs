namespace Haukcode.ArtNet.Rdm.Packets.Net;

/// <summary>
/// This parameter is used to get the broker connection status of an RDMnet component per scope,
/// or reset its counters (E1.33 §7.6.3).
/// </summary>
public class TcpCommsStatus
{
    /// <summary>Size of the NUL-padded scope field.</summary>
    public const int ScopeLength = 63;

    internal static string ReadScope(RdmBinaryReader data) =>
        Encoding.UTF8.GetString(data.ReadBytes(ScopeLength)).TrimEnd('\0');

    internal static void WriteScope(RdmBinaryWriter data, string scope)
    {
        var field = new byte[ScopeLength];
        var bytes = Encoding.UTF8.GetBytes(scope ?? string.Empty);

        // Truncate on a character boundary
        int length = Math.Min(bytes.Length, ScopeLength);
        while (length > 0 && length < bytes.Length && (bytes[length] & 0xC0) == 0x80)
            length--;

        bytes.AsSpan(0, length).CopyTo(field);
        data.WriteBytes(field);
    }

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.TcpCommsStatus)
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
    /// One message describes one scope; a component with several scopes sends the rest with
    /// ACK_OVERFLOW.
    /// </remarks>
    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.TcpCommsStatus)
        {
        }

        public string Scope { get; set; } = string.Empty;

        /// <summary>The broker's IPv4 address, 0.0.0.0 when not connected over IPv4.</summary>
        public IPAddress BrokerIPv4Address { get; set; } = IPAddress.Any;

        /// <summary>The broker's IPv6 address, :: when not connected over IPv6.</summary>
        public IPAddress BrokerIPv6Address { get; set; } = IPAddress.IPv6Any;

        public ushort BrokerPort { get; set; }

        public ushort UnhealthyTcpEvents { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            Scope = ReadScope(data);
            BrokerIPv4Address = new IPAddress(data.ReadBytes(4));
            BrokerIPv6Address = new IPAddress(data.ReadBytes(16));
            BrokerPort = data.ReadUInt16();
            UnhealthyTcpEvents = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            WriteScope(data, Scope);
            data.WriteBytes(BrokerIPv4Address.MapToIPv4().GetAddressBytes());
            data.WriteBytes(BrokerIPv6Address.MapToIPv6().GetAddressBytes());
            data.WriteUInt16(BrokerPort);
            data.WriteUInt16(UnhealthyTcpEvents);
        }
    }

    /// <summary>
    /// Resets the counters for a scope.
    /// </summary>
    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.TcpCommsStatus)
        {
        }

        public string Scope { get; set; } = string.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            Scope = ReadScope(data);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            WriteScope(data, Scope);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.TcpCommsStatus)
        {
        }

        protected override void ReadData(RdmBinaryReader data)
        {
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
        }
    }
}
