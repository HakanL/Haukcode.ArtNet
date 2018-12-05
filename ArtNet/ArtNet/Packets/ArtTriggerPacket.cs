using Haukcode.ArtNet.IO;
using System;

namespace Haukcode.ArtNet.Packets
{
    public class ArtTriggerPacket : ArtNetPacket
    {
        public ArtTriggerPacket()
            : base(ArtNetOpCodes.ArtTrigger)
        {
        }

        public ArtTriggerPacket(ArtNetReceiveData data)
            : base(data)
        {

        }

        public byte Filler1 { get; set; }

        public byte Filler2 { get; set; }

        public Int16 OemCode { get; set; }

        public byte Key { get; set; }

        public byte SubKey { get; set; }

        public byte[] Data { get; set; }

        public override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);

            Filler1 = data.ReadByte();
            Filler2 = data.ReadByte();
            OemCode = data.ReadNetwork16();
            Key = data.ReadByte();
            SubKey = data.ReadByte();
            Data = data.ReadBytes(512);
        }

        public override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(Filler1);
            data.Write(Filler2);
            data.WriteNetwork(OemCode);
            data.Write(Key);
            data.Write(SubKey);
            data.Write(Data);
        }
    }
}
