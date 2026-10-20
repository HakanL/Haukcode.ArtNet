namespace Haukcode.ArtNet.Rdm.Packets.Network;

/// <summary>
/// The device's unqualified host name, 1-63 characters (E1.37-2 4.13).
/// </summary>
public class DnsHostname
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.DnsHostname)
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
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.DnsHostname)
        {
        }

        public string Hostname { get; set; } = string.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            Hostname = data.ReadString(Header.ParameterDataLength);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteString(Hostname, 63);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.DnsHostname)
        {
        }

        public string Hostname { get; set; } = string.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            Hostname = data.ReadString(Header.ParameterDataLength);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteString(Hostname, 63);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.DnsHostname)
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
