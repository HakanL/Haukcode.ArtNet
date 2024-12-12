using System;
using System.Linq;
using Haukcode.ArtNet.IO;
using Haukcode.ArtNet.Packets;

namespace Haukcode.ArtNet.Packets
{
    public class ArtAddressPacket : ArtNetPacket
    {
        private byte[] swIn = { 0, 0, 0, 0 };
        private byte[] swOut = { 0, 0, 0, 0 };
        public byte NetSwitch { get; set; } = 0;
        public byte BindIndex { get; set; } = 0;
        public string ShortName { get; set; } = "";
        public string LongName { get; set; } = "";

        public byte[] SwIn
        {
            get => swIn;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("The SwIn must be an array of 4 bytes.");
                
                swIn = value;
            }
        }

        public byte[] SwOut
        {
            get => swOut;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("The SwOut must be an array of 4 bytes.");
                
                swOut = value;
            }
        }

        public byte SubSwitch { get; set; } = 0;
        public byte AcnPriority { get; set; } = 255;
        public ArtAddressCommand Command { get; set; } = ArtAddressCommand.AcNone;


        public ArtAddressPacket() : base(ArtNetOpCodes.Address)
        {
        }

        public ArtAddressPacket(ArtPollReplyPacket src) : base(ArtNetOpCodes.Address)
        {
            // Not all devices will be on code and set the address regardless of Bit 7. So better provide all the default values.
            BindIndex = src.BindIndex;
            SwIn = src.SwIn.Select(x => (byte)(x | 0x80)).ToArray();
            SwOut = src.SwOut.Select(x => (byte)(x | 0x80)).ToArray();
            SubSwitch = (byte)((src.SubSwitch & 0x000F) | 0x80);
            NetSwitch = (byte)(((src.SubSwitch & 0x7F00) >> 8) | 0x80);
        }

        public ArtAddressPacket(ArtNetReceiveData data) : base(data)
        {
        }

        public override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);
            NetSwitch = data.ReadByte();
            BindIndex = data.ReadByte();
            ShortName = data.ReadString(18);
            LongName = data.ReadString(64);
            SwIn = data.ReadBytes(4);
            SwOut = data.ReadBytes(4);
            SubSwitch = data.ReadByte();
            AcnPriority = data.ReadByte();
            Command = (ArtAddressCommand)data.ReadByte();
        }

        public override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);
            data.WriteByte(NetSwitch);
            data.WriteByte(BindIndex);
            data.WriteString(ShortName, 18);
            data.WriteString(LongName, 64);
            data.WriteByteArray(SwIn);
            data.WriteByteArray(SwOut);
            data.WriteByte(SubSwitch);
            data.WriteByte(AcnPriority);
            data.WriteByte((byte)Command);
        }
    }
}