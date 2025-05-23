﻿using Haukcode.ArtNet.Rdm.Packets.Status;

namespace Haukcode.ArtNet.Rdm.Packets.Net;

public class BackgroundStatusType
{
    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get,RdmParameters.BackgroundStatusType)
        {
        }

        public short EndpointID { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.BackgroundStatusType)
        {
        }

        public short EndpointID { get; set; }

        public StatusTypes StatusType { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadInt16();
            StatusType = (StatusTypes) data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
            data.WriteByte((byte) StatusType);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.BackgroundStatusType)
        {
        }

        public short EndpointID { get; set; }

        public StatusTypes StatusType { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            EndpointID = data.ReadInt16();
            StatusType = (StatusTypes) data.ReadByte();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16(EndpointID);
            data.WriteByte((byte)StatusType);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.BackgroundStatusType)
        {
        }

        protected override void ReadData(RdmBinaryReader data)
        {
            //Parameter Data Empty
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            //Parameter Data Empty
        }
    }
}
