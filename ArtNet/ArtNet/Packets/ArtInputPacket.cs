using System;
using System.Collections.Generic;
using System.Text;
using Haukcode.ArtNet.IO;
using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.ArtNet.Packets
{
    public class ArtInputPacket : ArtNetPacket
    {
        public byte BindIndex { get; set; } = 0;

        public short NumPorts { get; set; } = 0;

        public byte[] Inputs { get; set; } = { 0, 0, 0, 0 };

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
            NumPorts = data.ReadInt16();
            Inputs = data.ReadBytes(4);
        }

        public override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);
            data.Write((byte)0);
            data.Write(BindIndex);
            data.WriteNetwork(NumPorts);
            data.Write(Inputs);
        }
    }
}
