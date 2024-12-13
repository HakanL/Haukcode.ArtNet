using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Control
{
    /// <summary>
    /// This parameter is used to capture a static scene into a Preset within the responder. The actual
    /// data that is captured is beyond the scope of this standard. Upon receipt of this parameter the
    /// responder shall capture the scene and store it to the designated preset.
    /// </summary>
    /// <remarks>
    /// Fade and Wait times for building sequences may also be included. Times are in tenths of a
    /// second. When timing information is not required the fields shall be set to 0x00.
    /// 
    /// The Up Fade Time is the fade in time for the current scene and the Down Fade Time is the down
    /// fade for the previous scene or active look. The Wait Time is the time the device spends holding
    /// the current scene before proceeding to play the next scene when the presets are being played
    /// back as a sequence using PRESET_PLAYBACK_ALL.
    /// </remarks>
    public class CapturePreset
    {
        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.CapturePreset)
            {
            }

            public short SceneNumber { get; set; }

            public short UpFadeTime { get; set; }

            public short DownFadeTime { get; set; }

            public short WaitTime { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                SceneNumber = data.ReadInt16();
                UpFadeTime = data.ReadInt16();
                DownFadeTime = data.ReadInt16();
                WaitTime = data.ReadInt16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteUInt16(SceneNumber);
                data.WriteUInt16(UpFadeTime);
                data.WriteUInt16(DownFadeTime);
                data.WriteUInt16(WaitTime);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.CapturePreset)
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
