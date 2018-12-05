using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm;
using Haukcode.ArtNet.IO;

namespace Haukcode.ArtNet.Packets
{
    public class ArtRdmSubPacket : ArtNetPacket
    {
        public ArtRdmSubPacket()
            : base(ArtNetOpCodes.RdmSub)
        {
            RdmVersion = 1;
        }

        public ArtRdmSubPacket(ArtNetReceiveData data)
            : base(data)
        {

        }

        #region Packet Properties

        public byte RdmVersion { get; set; }

        public UId DeviceId { get; set; }

        public RdmCommands Command { get; set; }

        public RdmParameters ParameterId { get; set; }

        public short SubDevice { get; set; }

        public short SubCount { get; set; }

        public byte[] RdmData { get; set; }


        #endregion

        public override void ReadData(ArtNetBinaryReader data)
        {
            var reader = new RdmBinaryReader(data.BaseStream);
            
            base.ReadData(data);            

            RdmVersion = data.ReadByte();
            data.BaseStream.Seek(1, System.IO.SeekOrigin.Current);
            DeviceId = reader.ReadUId();
            data.BaseStream.Seek(1, System.IO.SeekOrigin.Current);
            Command = (RdmCommands) data.ReadByte();
            ParameterId = (RdmParameters) reader.ReadNetwork16();
            SubDevice = reader.ReadNetwork16();
            SubCount = reader.ReadNetwork16();
            data.BaseStream.Seek(4, System.IO.SeekOrigin.Current);
        }

        public override void WriteData(ArtNetBinaryWriter data)
        {
            var writer = new RdmBinaryWriter(data.BaseStream);

            base.WriteData(data);

            writer.Write(RdmVersion);
            writer.Write(new byte[1]);
            writer.Write(DeviceId);
            writer.Write(new byte[1]);
            writer.Write((byte)Command);
            writer.WriteNetwork((short)ParameterId);
            writer.WriteNetwork(SubDevice);
            writer.WriteNetwork(SubCount);
            writer.Write(new byte[4]);
            writer.Write(RdmData);
        }


    }
}
