namespace Haukcode.ArtNet.Rdm.Packets.Parameters;

/// <summary>
/// This parameter is used to retrieve the definition of some manufacturer-specific PIDs. The purpose 
/// of this parameter is to allow a controller to retrieve enough information about the manufacturerspecific
/// PID to generate Get and Set commands.
/// </summary>
public class ParameterDescription
{
    /// <summary>
    /// Min/max/default values on the wire (E1.20 10.4.2): 32 bits, values of narrower types padded
    /// with zero bytes, and zero for data types other than 8/16/32-bit integers.
    /// </summary>
    public static uint EncodeValue(long value, ParameterDataTypes dataType) => dataType switch
    {
        ParameterDataTypes.UInt8 or ParameterDataTypes.Int8 => (uint)(value & 0xFF),
        ParameterDataTypes.UInt16 or ParameterDataTypes.Int16 => (uint)(value & 0xFFFF),
        ParameterDataTypes.UInt32 or ParameterDataTypes.Int32 => unchecked((uint)value),
        _ => 0
    };

    /// <summary>
    /// The value a min/max/default field holds for the data type, sign-extended for signed types.
    /// </summary>
    public static long DecodeValue(uint raw, ParameterDataTypes dataType) => dataType switch
    {
        ParameterDataTypes.UInt8 => raw & 0xFF,
        ParameterDataTypes.Int8 => unchecked((sbyte)raw),
        ParameterDataTypes.UInt16 => raw & 0xFFFF,
        ParameterDataTypes.Int16 => unchecked((short)raw),
        ParameterDataTypes.UInt32 => raw,
        ParameterDataTypes.Int32 => unchecked((int)raw),
        _ => 0
    };

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.ParameterDescription)
        {
        }

        public RdmParameters ParameterId { get; set; }

        protected override void ReadData(RdmBinaryReader data)
        {
            ParameterId = (RdmParameters)data.ReadUInt16();
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16((short)ParameterId);
        }
    }

    public class GetReply : RdmResponsePacket
    {
        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.ParameterDescription)
        {
        }

        /// <summary>
        /// The manufacturer specific PID requested by the controller. Range 0x8000 to 0xFFDF.
        /// </summary>
        public RdmParameters ParameterId { get; set; }

        /// <summary>
        /// PDL Size defines the number used for the PDL field in all GET_RESPONSE and SET messages
        /// associated with this PID. In the case of the value of DS_ASCII, the PDL Size represents the
        /// maximum length of a variable sized ASCII string.
        /// </summary>
        public byte PDLSize { get; set; }

        /// <summary>
        /// Data Type defines the size of the data entries in the PD of the message for this PID. For
        /// example: unsigned 8-bit character versus signed 16-bit word. Table A-15 enumerates the field
        /// codes.
        /// </summary>
        public ParameterDataTypes DataType { get; set; }

        /// <summary>
        /// Command Class defines whether Get and or Set messages are implemented for the specified
        /// PID. Table A-16 enumerates the field codes.
        /// </summary>
        public ParameterCommandClasses CommandClass { get; set; }

        /// <summary>
        /// Type is an unsigned 8-bit value enumerated by Table A-12. It defines the type of data that is
        /// described by the specified PID.
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Unit is an unsigned 8-bit value enumerated by Table A-13. It defines the SI (International System
        /// of units) unit of the specified PID data.
        /// </summary>
        public byte Unit { get; set; }

        /// <summary>
        /// Prefix is an unsigned 8-bit value enumerated by Table A-14. It defines the SI Prefix and
        /// multiplication factor of the units.
        /// </summary>
        public byte Prefix { get; set; }

        /// <summary>
        /// This is a 32-bit field that represents the lowest value that data can reach. The format of the
        /// number is defined by DATA TYPE. This field has no meaning for a Data Type of DS_BIT_FIELD
        /// or DS_ASCII. For Data Types less than 32-bits, the Most Significant Bytes shall be padded with
        /// 0x00 out to 32-bits. For example, an 8-bit data value of 0x12 shall be represented in the field as:
        /// 0x00000012.
        /// </summary>
        public long MinValidValue { get; set; }

        /// <summary>
        /// This is a 32-bit field that represents the highest value that data can reach. The format of the
        /// number is defined by DATA TYPE. This field has no meaning for a Data Type of DS_BIT_FIELD
        /// or DS_ASCII. For Data Types less than 32-bits, the Most Significant Bytes shall be padded with
        /// 0x00 out to 32-bits. For example, an 8-bit data value of 0x12 shall be represented in the field as:
        /// 0x00000012.
        /// </summary>
        public long MaxValidValue { get; set; }

        /// <summary>
        /// This is a 32-bit field that represents the default value of that data. This field has no meaning for a
        /// Data Type of DS_BIT_FIELD or DS_ASCII. The default value shall be within the minimum and
        /// maximum range. For Data Types less than 32-bits, the Most Significant Bytes shall be padded
        /// with 0x00 out to 32-bits. For example, an 8-bit data value of 0x12 shall be represented in the field
        /// as: 0x00000012.
        /// </summary>
        public long DefaultValue { get; set; }

        /// <summary>
        /// The Description field is used to describe the function of the specified PID. This text field shall be
        /// variable up to 32 characters in length.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        protected override void ReadData(RdmBinaryReader data)
        {
            ParameterId = (RdmParameters)data.ReadUInt16();
            PDLSize = data.ReadByte();
            DataType = (ParameterDataTypes)data.ReadByte();
            CommandClass = (ParameterCommandClasses)data.ReadByte();
            Type = data.ReadByte();
            Unit = data.ReadByte();
            Prefix = data.ReadByte();
            MinValidValue = DecodeValue(data.ReadUInt32(), DataType);
            MaxValidValue = DecodeValue(data.ReadUInt32(), DataType);
            DefaultValue = DecodeValue(data.ReadUInt32(), DataType);
            Description = data.ReadString(Header.ParameterDataLength - 20);
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16((ushort)ParameterId);
            data.WriteByte(PDLSize);
            data.WriteByte((byte)DataType);
            data.WriteByte((byte)CommandClass);
            data.WriteByte(Type);
            data.WriteByte(Unit);
            data.WriteByte(Prefix);
            data.WriteUInt32(EncodeValue(MinValidValue, DataType));
            data.WriteUInt32(EncodeValue(MaxValidValue, DataType));
            data.WriteUInt32(EncodeValue(DefaultValue, DataType));
            data.WriteString(Description);
        }
    }
}
