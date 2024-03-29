using Haukcode.ArtNet.IO;
using System;
using System.IO;

namespace Haukcode.ArtNet.Packets
{
    public class ArtNetPacket
    {
        public ArtNetPacket(ArtNetOpCodes opCode)
        {
            OpCode = opCode;
        }

        public ArtNetPacket(ArtNetReceiveData data)
        {
            DataLength = data.DataLength - 12;  // Subtract ArtNet header
            var packetReader = new ArtNetBinaryReader(new MemoryStream(data.buffer));
            ReadData(packetReader);
        }

        public int DataLength { get; }

        public byte[] ToArray()
        {
            using (var stream = new MemoryStream())
            {
                WriteData(new ArtNetBinaryWriter(stream));

                return stream.ToArray();
            }
        }

        #region Packet Properties

        private string protocol = "Art-Net";

        public string Protocol
        {
            get { return protocol; }
            protected set
            {
                if (value.Length > 8)
                    protocol = value.Substring(0, 8);
                else
                    protocol = value;
            }
        }


        private short version = 14;

        public short Version
        {
            get { return version; }
            protected set { version = value; }
        }

        private ArtNetOpCodes opCode = ArtNetOpCodes.None;

        public virtual ArtNetOpCodes OpCode
        {
            get { return opCode; }
            protected set { opCode = value; }
        }

        #endregion

        public virtual void ReadData(ArtNetBinaryReader data)
        {
            Protocol = data.ReadNetworkString(8);
            OpCode = (ArtNetOpCodes)data.ReadNetwork16();

            //For some reason the poll packet header does not include the version.
            if (OpCode != ArtNetOpCodes.PollReply)
                Version = data.ReadNetwork16();

        }

        public virtual void WriteData(ArtNetBinaryWriter data)
        {
            data.WriteNetwork(Protocol, 8);
            data.WriteNetwork((short)OpCode);

            //For some reason the poll packet header does not include the version.
            if (OpCode != ArtNetOpCodes.PollReply)
                data.WriteNetwork(Version);

        }

        public static ArtNetPacket Create(ArtNetReceiveData data, Func<ushort, ArtNetReceiveData, ArtNetPacket> customPacketCreator)
        {
            switch ((ArtNetOpCodes)data.OpCode)
            {
                case ArtNetOpCodes.Poll:
                    return new ArtPollPacket(data);

                case ArtNetOpCodes.PollReply:
                    return new ArtPollReplyPacket(data);

                case ArtNetOpCodes.Dmx:
                    return new ArtNetDmxPacket(data);

                case ArtNetOpCodes.Sync:
                    return new ArtSyncPacket(data);

                case ArtNetOpCodes.TodRequest:
                    return new ArtTodRequestPacket(data);

                case ArtNetOpCodes.TodData:
                    return new ArtTodDataPacket(data);

                case ArtNetOpCodes.TodControl:
                    return new ArtTodControlPacket(data);

                case ArtNetOpCodes.Rdm:
                    return new ArtRdmPacket(data);

                case ArtNetOpCodes.RdmSub:
                    return new ArtRdmSubPacket(data);

                case ArtNetOpCodes.ArtTrigger:
                    return new ArtTriggerPacket(data);

                case ArtNetOpCodes.IpProg:
                    return new ArtIpProgPacket(data);

                case ArtNetOpCodes.IpProgReply:
                    return new ArtIpProgReplyPacket(data);

                case ArtNetOpCodes.Address:
                    return new ArtAddressPacket(data);
                case ArtNetOpCodes.Input:
                    return new ArtInputPacket(data);

                default:
                    if (customPacketCreator != null)
                    {
                        var customPacket = customPacketCreator(data.OpCode, data);
                        if (customPacket != null)
                            return customPacket;
                    }

                    return new ArtNetUnknownPacket(data);
            }
        }
    }
}
