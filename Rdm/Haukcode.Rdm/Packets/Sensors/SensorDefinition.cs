using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.IO;

namespace Haukcode.Rdm.Packets.Sensors
{
    /// <summary>
    /// This parameter is used to retrieve the definition of a specific sensor.
    /// </summary>
    public class SensorDefinition
    {
        public enum SensorTypes
        {
            Temperature = 0x00,
            Voltage = 0x01,
            Current = 0x02,
            Frequency = 0x03,
            Resistance = 0x04,
            Power = 0x05,
            Mass = 0x06,
            Length = 0x07,
            Area = 0x08,
            Volume = 0x09,
            Denisty = 0x0A,
            Velocity = 0x0B,
            Acceleration = 0x0C,
            Force = 0x0D,
            Energy = 0x0E,
            Pressure = 0x0F,
            Time = 0x10,
            Angle = 0x11,
            PositionX = 0x12,
            PositionY = 0x13,
            PositionZ = 0x14,
            AngularVelocity = 0x15,
            LuminousIntensity = 0x16,
            LumninousFlux = 0x17,
            Illuminance = 0x18,
            ChrominanceRed = 0x19,
            ChrominanceGreen = 0x1A,
            ChrominanceBlue = 0x1B,
            Contacts = 0x1C,
            Memory = 0x1D,
            Items = 0x1E,
            Humidity = 0x1F,
            Counter16Bit = 0x20,
            Other = 0x7F,
        }

        public enum SensorUnit
        {
            None = 0x00,
            Centigrade = 0x01,
            VoltsDC = 0x02,
            VoltsACPeak = 0x03,
            VoltsACRms = 0x04,
            AmpereDC = 0x05,
            AmpereACPeak = 0x06,
            AmpereACRms = 0x07,
            Hertz = 0x08,
            Ohm = 0x09,
            Watt = 0x0A,
            Kilogram = 0x0B,
            Meters = 0x0C,
            MetersSquared = 0x0D,
            MetersCubed = 0x0E,
            KilogramsPerMeterCubed = 0x0F,
            MetersPerSecond = 0x10,
            MetersPerSecondSquared = 0x11,
            Newton = 0x12,
            Joule = 0x13,
            Pascal = 0x14,
            Second = 0x15,
            Degree = 0x16,
            Steradian = 0x17,
            Candela = 0x18,
            Lumen = 0x19,
            Lux = 0x1A,
            Ire = 0x1B,
            Byte = 0x1C,
        }

        public enum SensorPrefix
        {
            None = 0x00,
            Deci = 0x01,
            Centi = 0x02,
            Milli = 0x03,
            Micro = 0x04,
            Nano = 0x05,
            Pico = 0x06,
            Fempto = 0x07,
            Atto = 0x08,
            Zepto = 0x09,
            Yocto = 0x0A,
            Deca = 0x11,
            Hecto = 0x12,
            Kilo = 0x13,
            Mega = 0x14,
            Giga = 0x15,
            Terra = 0x16,
            Peta = 0x17,
            Exa = 0x18,
            Zetta = 0x19,
            Yotta = 0x1A,
        }

        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.SensorDefinition)
            {
            }

            public byte SensorNumber { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                SensorNumber = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte(SensorNumber);
            }

            #endregion
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.SensorDefinition)
            {
            }

            public byte SensorNumber { get; set; }

            public SensorTypes Type { get; set; }

            public SensorUnit Unit { get; set; }

            public SensorPrefix Prefix { get; set; }

            public short RangeMinValue { get; set; }

            public short RangeMaxValue { get; set; }

            public short NormalMinValue { get; set; }

            public short NormalMaxValue { get; set; }

            public byte RecordValueSupport { get; set; }

            public string Description { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                SensorNumber = data.ReadByte();
                Type = (SensorTypes) data.ReadByte();
                Unit = (SensorUnit)data.ReadByte();
                Prefix = (SensorPrefix)data.ReadByte();
                RangeMinValue = data.ReadInt16();
                RangeMaxValue = data.ReadInt16();
                NormalMinValue = data.ReadInt16();
                NormalMaxValue = data.ReadInt16();
                RecordValueSupport = data.ReadByte();
                Description = data.ReadString(Header.ParameterDataLength-13);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteByte(SensorNumber);
                data.WriteByte((byte) Type);
                data.WriteByte((byte) Unit);
                data.WriteByte((byte) Prefix);
                data.WriteUInt16(RangeMinValue);
                data.WriteUInt16(RangeMaxValue);
                data.WriteUInt16(NormalMinValue);
                data.WriteUInt16(NormalMaxValue);
                data.WriteByte(RecordValueSupport);
                data.WriteString(Description);
            }

            #endregion
        }
    }
}
