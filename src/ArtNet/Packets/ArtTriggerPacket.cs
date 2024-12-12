using Haukcode.ArtNet.IO;
using System;

namespace Haukcode.ArtNet.Packets
{
    public class ArtTriggerPacket : ArtNetPacket
    {
        public ArtTriggerPacket()
            : base(ArtNetOpCodes.Trigger)
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
            OemCode = data.ReadHiLoInt16();
            Key = data.ReadByte();
            SubKey = data.ReadByte();
            Data = data.ReadBytes(512);
        }

        public override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);

            data.WriteByte(Filler1);
            data.WriteByte(Filler2);
            data.WriteHiLoInt16(OemCode);
            data.WriteByte(Key);
            data.WriteByte(SubKey);
            data.WriteByteArray(Data);
        }
    }
}
