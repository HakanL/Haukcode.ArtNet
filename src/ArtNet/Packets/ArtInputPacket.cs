using System;
using Haukcode.ArtNet.IO;

namespace Haukcode.ArtNet.Packets
{
    public class ArtInputPacket : ArtNetPacket
    {
        private byte[] inputs = { 0, 0, 0, 0 };
        public byte BindIndex { get; set; } = 0;

        public short NumPorts { get; set; } = 0;

        public byte[] Inputs
        {
            get => inputs;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("The input must be an array of 4 bytes.");

                inputs = value;
            }
        }

        public ArtInputPacket() : base(ArtNetOpCodes.Input)
        {
        }

        public ArtInputPacket(ArtNetReceiveData data) : base(data)
        {
        }

        public override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);
            data.ReadByte();
            BindIndex = data.ReadByte();
            NumPorts = data.ReadHiLoInt16();
            Inputs = data.ReadBytes(4);
        }

        public override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);
            data.WriteByte((byte)0);
            data.WriteByte(BindIndex);
            data.WriteHiLoInt16(NumPorts);
            data.WriteByteArray(Inputs);
        }
    }
}