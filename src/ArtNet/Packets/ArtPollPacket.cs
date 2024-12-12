using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Haukcode.ArtNet.IO;

namespace Haukcode.ArtNet.Packets
{
    public class ArtPollPacket : ArtNetPacket
    {
        public ArtPollPacket()
            : base(ArtNetOpCodes.Poll)
        {
        }

        public ArtPollPacket(ArtNetReceiveData data)
            : base(data)
        {
            
        }

        #region Packet Properties

        private byte talkToMe=0;

        public byte TalkToMe
        {
            get { return talkToMe; }
            set { talkToMe = value; }
        }

        #endregion

        public override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);

            TalkToMe = data.ReadByte();
        }

        public override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);

            data.WriteByte(TalkToMe);
            data.WriteByte((byte)0);
        }

    }
}
