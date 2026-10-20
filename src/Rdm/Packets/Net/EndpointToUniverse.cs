namespace Haukcode.ArtNet.Rdm.Packets.Net;

/// <summary>
/// This parameter is used to assign an Endpoint on an E1.33 device to a specific E1.31 DMX512 Universe.
/// </summary>
/// <remarks>
/// Universe 0 means the endpoint is unpatched; 0xFFFF (composite) is only reported by a GET.
/// E1.37-7 also allows this PID within a splitter to assign inputs to outputs.
/// </remarks>
public class EndpointToUniverse
{
    /// <summary>Highest universe a SET may assign.</summary>
    public const ushort MaxUniverse = 63999;

    public enum UniverseMode
    {
        Disabled,
        Standard,
        Composite
    }

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.EndpointToUniverse)
        {
        }

        public ushort EndpointID { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.EndpointToUniverse)
        {
            EndpointMode = UniverseMode.Standard;
        }


        public ushort EndpointID { get; set; }

        private ushort universeNumber = 0;

        public ushort UniverseNumber
        {
            get { return universeNumber; }
            set
            {
                if (EndpointMode != UniverseMode.Standard)
                    throw new ArgumentException("Unable to assign universe to endpoint that is not a standard endpoint. Please change the endpoint mode to Standard and try again.");

                universeNumber = value;
            }
        }

        public UniverseMode EndpointMode { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();

            ushort universeNumber = data.ReadUInt16();
            switch (universeNumber)
            {
                case 0:
                    EndpointMode = UniverseMode.Disabled;
                    break;
                case 0xFFFF:
                    EndpointMode = UniverseMode.Composite;
                    break;
                default:
                    EndpointMode = UniverseMode.Standard;
                    UniverseNumber = universeNumber;
                    break;

            }

        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);

            switch (EndpointMode)
            {
                case UniverseMode.Disabled:
                    data.WriteUInt16(0);
                    break;
                case UniverseMode.Composite:
                    data.WriteUInt16(65535);
                    break;
                default:
                    data.WriteUInt16(UniverseNumber);
                    break;
            }

        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.EndpointToUniverse)
        {
            EndpointMode = UniverseMode.Standard;
        }

        public ushort EndpointID { get; set; }

        private ushort universeNumber = 0;

        public ushort UniverseNumber
        {
            get { return universeNumber; }
            set
            {
                if (EndpointMode != UniverseMode.Standard)
                    throw new ArgumentException("Unable to assign universe to endpoint that is not a standard endpoint. Please change the endpoint mode to Standard and try again.");

                universeNumber = value;
            }
        }

        public UniverseMode EndpointMode { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();

            ushort universeNumber = data.ReadUInt16();
            switch (universeNumber)
            {
                case 0:
                    EndpointMode = UniverseMode.Disabled;
                    break;
                case 0xFFFF:
                    EndpointMode = UniverseMode.Composite;
                    break;
                default:
                    EndpointMode = UniverseMode.Standard;
                    UniverseNumber = universeNumber;
                    break;

            }
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            // "Only Universe Numbers in the range 0 - 63999 are valid in SET_COMMAND"
            if (EndpointMode == UniverseMode.Composite || (EndpointMode == UniverseMode.Standard && UniverseNumber > MaxUniverse))
                throw new InvalidOperationException($"A SET can only assign universe 0 (unpatched) or 1 to {MaxUniverse}.");

            data.WriteUInt16(EndpointID);

            switch (EndpointMode)
            {
                case UniverseMode.Disabled:
                    data.WriteUInt16(0);
                    break;
                case UniverseMode.Composite:
                    data.WriteUInt16(65535);
                    break;
                default:
                    data.WriteUInt16(UniverseNumber);
                    break;
            }
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.EndpointToUniverse)
        {
        }

        public ushort EndpointID { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
        }
    }

}
