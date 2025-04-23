using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.IO;

namespace Haukcode.Rdm.Packets.Net
{
    /// <summary>
    /// This parameter is used to assign an Endpoint on an E1.33 device to a specific E1.31 DMX512 Universe.
    /// </summary>
    /// <remarks>
    /// This message shall only be used on E1.33 networks and shall not be implemented on an E1.20 network as an E1.20 network cannot span beyond the context of a single universe.
    /// </remarks>
    public class EndpointToUniverse
    {
        public enum EndpointTypes
        {
            Virtual = 0,
            Physical = 1
        }

        public enum UniverseMode
        {
            Disabled,
            Standard,
            Composite
        }

        public class Get : RdmRequestPacket
        {
            public Get() : base(RdmCommands.Get, RdmParameters.EndpointToUniverse)
            {
            }

            public short EndpointID { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadInt16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteUInt16(EndpointID);
            }
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply() : base(RdmCommands.GetResponse, RdmParameters.EndpointToUniverse)
            {
            }


            public short EndpointID { get; set; }

            private short universeNumber = 0;

            public short UniverseNumber
            {
                get { return universeNumber; }
                set
                {
                    if (EndpointMode != UniverseMode.Standard)
                        throw new ArgumentException("Unable to assign universe to endpoint that is not a standard endpoint. Please change the endpoint mode to Standard and try again.");

                    universeNumber = value;
                }
            }

            public EndpointTypes EndpointType { get; set; }

            public UniverseMode EndpointMode { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadInt16();

                int universeNumber = data.ReadInt16();
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
                        UniverseNumber = (short)universeNumber;
                        break;

                }
                EndpointType = (EndpointTypes)data.ReadByte();

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

                data.WriteByte((byte)EndpointType);
            }
        }

        public class Set : RdmRequestPacket
        {
            public Set() : base(RdmCommands.Set, RdmParameters.EndpointToUniverse)
            {
                EndpointMode = UniverseMode.Standard;
            }

            public short EndpointID { get; set; }

            private short universeNumber = 0;

            public short UniverseNumber
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
                EndpointID = data.ReadInt16();

                int universeNumber = data.ReadInt16();
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
                        UniverseNumber = (short)universeNumber;
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

        public class SetReply : RdmResponsePacket
        {
            public SetReply() : base(RdmCommands.SetResponse, RdmParameters.EndpointToUniverse)
            {
            }

            protected override void ReadData(RdmBinaryReader data)
            {
                //Parameter Data Empty
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                //Parameter Data Empty
            }
        }

    }
}
