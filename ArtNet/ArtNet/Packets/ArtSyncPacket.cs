using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Haukcode.ArtNet.IO;

namespace Haukcode.ArtNet.Packets
{
    public class ArtSyncPacket : ArtNetPacket
    {
        public ArtSyncPacket()
            : base(ArtNetOpCodes.Sync)
        {
        }

        public ArtSyncPacket(ArtNetReceiveData data)
            : base(data)
        {
        }

        #region Packet Properties

        private short aux = 0;

        public short Aux
        {
            get { return aux; }
            set { aux = value; }
        }

        #endregion

        public override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);

            Aux = data.ReadInt16();
        }

        public override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);
            
            data.WriteNetwork(Aux);
        }
    }
}
