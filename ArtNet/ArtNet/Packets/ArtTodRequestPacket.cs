using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Haukcode.ArtNet.IO;

namespace Haukcode.ArtNet.Packets
{
    public class ArtTodRequestPacket : ArtNetPacket
    {
        public ArtTodRequestPacket()
            : base(ArtNetOpCodes.TodRequest)
        {
            RequestedUniverses = new List<byte>();
        }

        public ArtTodRequestPacket(ArtNetReceiveData data)
            : base(data)
        {
            
        }

        #region Packet Properties

        public byte Net { get; set; }

        public byte Command { get; set; }

        public List<byte> RequestedUniverses { get; set; }
	
	
        #endregion

        public override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);

            data.BaseStream.Seek(9, System.IO.SeekOrigin.Current);
            Net = data.ReadByte();
            Command = data.ReadByte();
            int count = data.ReadByte();
            RequestedUniverses = new List<byte>(data.ReadBytes(count));
        }

        public override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(new byte[9]);
            data.Write(Net);
            data.Write(Command);
            data.Write((byte) RequestedUniverses.Count);
            data.Write(RequestedUniverses.ToArray());
        }
	

    }
}
