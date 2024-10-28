using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.DMX
{
    /// <summary>
    /// This parameter shall be used for requesting the default values for the given DMX512 slot offsets
    /// for a device.
    /// </summary>
    public class DefaultSlotValue
    {
        public struct SlotValue
        {
            public SlotValue(short offset, byte value)
                : this()
            {
                Offset = offset;
                Value = value;
            }

            public short Offset { get; set; }

            public byte Value { get; set; }
        }

        public class Get : RdmResponsePacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.DefaultSlotValue)
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
                : base(RdmCommands.GetResponse, RdmParameters.DefaultSlotValue)
            {
                DefaultValues = new List<SlotValue>();
            }

            public List<SlotValue> DefaultValues { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                for (int n = 0; n < Header.ParameterDataLength / 3; n++)
                {
                    SlotValue slot = new SlotValue();
                    slot.Offset = data.ReadHiLoInt16();
                    slot.Value = data.ReadByte();
                    DefaultValues.Add(slot);
                }
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                foreach (SlotValue value in DefaultValues)
                {
                    data.WriteHiLoInt16(value.Offset);
                    data.WriteByte(value.Value);
                }
            }

            #endregion
        }
    }
}
