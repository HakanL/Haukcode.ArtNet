namespace Haukcode.ArtNet.Rdm.Packets.Network;

/// <summary>
/// The device's DNS domain name, up to 231 characters; empty means none (E1.37-2 4.14).
/// </summary>
public class DnsDomainName
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.DnsDomainName)
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
            : base(RdmCommands.GetResponse, RdmParameters.DnsDomainName)
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
            : base(RdmCommands.Set, RdmParameters.DnsDomainName)
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
            : base(RdmCommands.SetResponse, RdmParameters.DnsDomainName)
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
