namespace Haukcode.ArtNet.Rdm.Packets.Configuration;

/// <summary>
/// This parameter is used to retrieve or set the real-time clock in a device.
/// </summary>
public class RealTimeClock
{
    /// <summary>
    /// The clock fields as sent on the wire. Devices can report values a <see cref="DateTime"/>
    /// can't hold, such as an unset clock (month 0) or a leap second (second 60).
    /// </summary>
    public readonly record struct ClockValue(ushort Year, byte Month, byte Day, byte Hour, byte Minute, byte Second)
    {
        /// <summary>
        /// The clock as a <see cref="DateTime"/>, or null when the fields aren't a valid date and
        /// time. A leap second (60) is reported as second 59.
        /// </summary>
        public DateTime? ToDateTime()
        {
            if (Year is < 1 or > 9999 || Month is < 1 or > 12 || Day < 1 || Day > DateTime.DaysInMonth(Year, Month) ||
                Hour > 23 || Minute > 59 || Second > 60)
            {
                return null;
            }

            return new DateTime(Year, Month, Day, Hour, Minute, Math.Min(Second, (byte)59));
        }

        public static ClockValue FromDateTime(DateTime value) =>
            new((ushort)value.Year, (byte)value.Month, (byte)value.Day, (byte)value.Hour, (byte)value.Minute, (byte)value.Second);

        internal static ClockValue Read(RdmBinaryReader data) =>
            new(data.ReadUInt16(), data.ReadByte(), data.ReadByte(), data.ReadByte(), data.ReadByte(), data.ReadByte());

        internal void Write(RdmBinaryWriter data)
        {
            data.WriteUInt16(Year);
            data.WriteByte(Month);
            data.WriteByte(Day);
            data.WriteByte(Hour);
            data.WriteByte(Minute);
            data.WriteByte(Second);
        }
    }

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.RealTimeClock)
        {
        }

        protected override void ReadData(RdmBinaryReader data)
        {
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.RealTimeClock)
        {
        }

        public ClockValue Clock { get; set; }

        /// <summary>
        /// The clock as a <see cref="DateTime"/>, or null when the device reported an invalid date.
        /// </summary>
        public DateTime? ClockTime
        {
            get => Clock.ToDateTime();
            set => Clock = value.HasValue ? ClockValue.FromDateTime(value.Value) : default;
        }

        protected override void ReadData(RdmBinaryReader data)
        {
            Clock = ClockValue.Read(data);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            Clock.Write(data);
        }
    }

    public class Set : RdmRequestPacket
    {
        public Set()
            : base(RdmCommands.Set, RdmParameters.RealTimeClock)
        {
        }

        public ClockValue Clock { get; set; }

        /// <summary>
        /// The clock as a <see cref="DateTime"/>, or null when the fields aren't a valid date.
        /// </summary>
        public DateTime? ClockTime
        {
            get => Clock.ToDateTime();
            set => Clock = value.HasValue ? ClockValue.FromDateTime(value.Value) : default;
        }

        protected override void ReadData(RdmBinaryReader data)
        {
            Clock = ClockValue.Read(data);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            Clock.Write(data);
        }
    }

    public class SetReply : RdmResponsePacket
    {
        public SetReply()
            : base(RdmCommands.SetResponse, RdmParameters.RealTimeClock)
        {
        }

        protected override void ReadData(RdmBinaryReader data)
        {
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
        }
    }
}
