namespace Haukcode.ArtNet.Rdm.Packets.Net;

/// <summary>
/// DNS search domain Clients use to locate a Broker, 0-231 octets (E1.33 7.6.1). Root device only.
/// </summary>
public class SearchDomain
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.SearchDomain)
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
            : base(RdmCommands.GetResponse, RdmParameters.SearchDomain)
        {
        }

        public string DomainName { get; set; } = string.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            DomainName = data.ReadString(Header.ParameterDataLength);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteString(DomainName, 231);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.SearchDomain)
        {
        }

        public string DomainName { get; set; } = string.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            DomainName = data.ReadString(Header.ParameterDataLength);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteString(DomainName, 231);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.SearchDomain)
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
