using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Control
{
    /// <summary>
    /// This parameter is used to recall pre-recorded Presets.
    /// </summary>
    public class PresetPlayback
    {
        public enum PlayMode
        {
            Off = 0x0,
            Scene = 0x1,
            All = 0xffff
        }

        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.PresetPlayback)
            {
            }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
            }

            #endregion
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.PresetPlayback)
            {
            }

            public PlayMode Mode { get; set; }

            public ushort SceneNumber { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                SceneNumber = (ushort) data.ReadHiLoInt16();
                if (SceneNumber > (ushort) PlayMode.Off || SceneNumber < (ushort) PlayMode.All)
                    Mode = PlayMode.Scene;
                else
                    Mode = (PlayMode) SceneNumber;
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                if (Mode == PlayMode.Scene)
                    data.WriteHiLoInt16((short) SceneNumber);
                else
                    data.WriteHiLoInt16((short) Mode);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.PresetPlayback)
            {
            }

            public PlayMode Mode { get; set; }

            public ushort SceneNumber { get; set; }

            public byte Level { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                SceneNumber = (ushort) data.ReadHiLoInt16();
                if (SceneNumber > (ushort)PlayMode.Off || SceneNumber < (ushort)PlayMode.All)
                    Mode = PlayMode.Scene;
                else
                    Mode = (PlayMode)SceneNumber;
                Level = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                if (Mode == PlayMode.Scene)
                    data.WriteHiLoInt16((short)SceneNumber);
                else
                    data.WriteHiLoInt16((short)Mode);
                data.WriteByte(Level);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.PresetPlayback)
            {
            }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
            }

            #endregion
        }
    }
}
