using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.DMX
{
    public enum SlotTypes
    {
        Primary = 0,
        Fine = 1,
        Timing = 2,
        Speed = 3,
        Control = 4,
        Index = 5,
        Rotation = 6,
        IndexRotation = 7,
        Undefined = 0xff
    }

    public enum SlotIds
    {
        Undefined = 0xFFFF,

        //Intensity Functions
        Intensity = 1,
        IntensityMaster = 2,

        //Movement Functions
        Pan = 0x101,
        Tilt = 0x102,

        //Colour Functions
        ColourWheel = 0x0201,
        ColourSubCyan = 0x0202,
        ColourSubYellow = 0x0203,
        ColourSubMagenta = 0x0204,
        ColourAddRed = 0x0205,
        ColourAddGreen = 0x0206,
        ColourAddBlue = 0x0207,
        ColourCorrection = 0x0208,
        ColourScroll = 0x0209,
        ColourSemaphore = 0x0210,

        //Image Functions
        StaticGoboWheel = 0x0301,
        RotoGoboWheel = 0x0302,
        PrismWheel = 0x0303,
        EffectsWheel = 0x0304,

        //Beam Functions
        BeamSizeIris = 0x0401,
        Edge = 0x0402,
        Frost = 0x0403,
        Strobe = 0x0404,
        Zoom = 0x0405,
        FramingShutter = 0x0406,
        ShutterRotate = 0x0407,
        Douser = 0x0408,
        BarnDoors = 0x0409,

        //Control Functions
        LampControl = 0x0501,
        FixtureControl = 0x0502,
        FixtureSpeed = 0x0503,
        Macro = 0x0504
    }

    /// <summary>
    /// This parameter is used to retrieve basic information about the functionality of the DMX512 slots
    /// used to control the device.
    /// </summary>
    public class SlotInfo
    {
        public struct SlotInformation
        {
            public SlotInformation(short offset, SlotIds id):this()
            {
                this.Offset = offset;
                this.Type = SlotTypes.Primary;
                this.Id = id;
            }

            public SlotInformation(short offset, SlotTypes type, int slotLink)
                : this()
            {
                this.Offset = offset;
                this.Type = type;
                this.SlotLink = slotLink;
            }

            public short Offset { get; set; }

            public SlotTypes Type { get; set; }

            public SlotIds Id
            {
                get { return (SlotIds)SlotLink; }
                set { SlotLink = (int)value; }
            }

            public int SlotLink { get; set; }     
        }

        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.SlotInfo)
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
                : base(RdmCommands.GetResponse, RdmParameters.SlotInfo)
            {
                Slots = new List<SlotInformation>();
            }

            public List<SlotInformation> Slots { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                Slots.Clear();
                for (int n = 0; n < Header.ParameterDataLength / 5; n++)
                {
                    SlotInformation slot = new SlotInformation();
                    slot.Offset = data.ReadHiLoInt16();
                    slot.Type = (SlotTypes) data.ReadByte();
                    slot.SlotLink = (int)data.ReadHiLoInt16();
                    Slots.Add(slot);
                }
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                foreach (SlotInformation slot in Slots)
                {
                    data.WriteHiLoInt16(slot.Offset);
                    data.WriteByte((byte) slot.Type);
                    data.WriteHiLoInt16((short)slot.SlotLink);
                }
            }

            #endregion
        }
    }
}
