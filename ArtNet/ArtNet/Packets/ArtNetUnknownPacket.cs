using Haukcode.ArtNet.IO;
using System;

namespace Haukcode.ArtNet.Packets
{
    public class ArtNetUnknownPacket : ArtNetPacket
    {
        public ArtNetUnknownPacket(int opCode)
            : base((ArtNetOpCodes)opCode)
        {
        }

        public ArtNetUnknownPacket(ArtNetReceiveData data)
            : base(data)
        {
        }

        public byte[] Data { get; set; }

        public override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);

            Data = data.ReadBytes(DataLength);
        }

        public override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(Data);
        }
    }
}
