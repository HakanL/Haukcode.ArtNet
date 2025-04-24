namespace Haukcode.ArtNet.Rdm.Packets.DMX;

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
                slot.Offset = data.ReadInt16();
                slot.Type = (SlotTypes) data.ReadByte();
                slot.SlotLink = (int)data.ReadInt16();
                Slots.Add(slot);
            }
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            foreach (SlotInformation slot in Slots)
            {
                data.WriteUInt16(slot.Offset);
                data.WriteByte((byte) slot.Type);
                data.WriteUInt16((short)slot.SlotLink);
            }
        }

        #endregion
    }
}
