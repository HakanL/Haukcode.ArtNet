using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.IO;

namespace Haukcode.Rdm.Packets.Parameters
{
    /// <summary>
    /// This parameter is used to retrieve the definition of some manufacturer-specific PIDs. The purpose 
    /// of this parameter is to allow a controller to retrieve enough information about the manufacturerspecific
    /// PID to generate Get and Set commands.
    /// </summary>
    public class ParameterDescription
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.ParameterDescription)
            {
            }

            public RdmParameters ParameterId { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                ParameterId = (RdmParameters)((ushort)data.ReadInt16());
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
            public byte DataType { get; set; }

            /// <summary>
            /// Command Class defines whether Get and or Set messages are implemented for the specified
            /// PID. Table A-16 enumerates the field codes.
            /// </summary>
            public byte CommandClass { get; set; }

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
            public int MinValidValue { get; set; }

            /// <summary>
            /// This is a 32-bit field that represents the highest value that data can reach. The format of the
            /// number is defined by DATA TYPE. This field has no meaning for a Data Type of DS_BIT_FIELD
            /// or DS_ASCII. For Data Types less than 32-bits, the Most Significant Bytes shall be padded with
            /// 0x00 out to 32-bits. For example, an 8-bit data value of 0x12 shall be represented in the field as:
            /// 0x00000012.
            /// </summary>
            public int MaxValidValue { get; set; }

            /// <summary>
            /// This is a 32-bit field that represents the default value of that data. This field has no meaning for a
            /// Data Type of DS_BIT_FIELD or DS_ASCII. The default value shall be within the minimum and
            /// maximum range. For Data Types less than 32-bits, the Most Significant Bytes shall be padded
            /// with 0x00 out to 32-bits. For example, an 8-bit data value of 0x12 shall be represented in the field
            /// as: 0x00000012.
            /// </summary>
            public int DefaultValue { get; set; }

            /// <summary>
            /// The Description field is used to describe the function of the specified PID. This text field shall be
            /// variable up to 32 characters in length.
            /// </summary>
            public string Description { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                ParameterId = (RdmParameters)((ushort)data.ReadInt16());
                PDLSize = data.ReadByte();
                DataType = data.ReadByte();
                CommandClass = data.ReadByte();
                Type = data.ReadByte();
                Unit = data.ReadByte();
                Prefix = data.ReadByte();
                MinValidValue = data.ReadHiLoInt32();
                MaxValidValue = data.ReadHiLoInt32();
                DefaultValue = data.ReadHiLoInt32();
                Description = data.ReadString(Header.ParameterDataLength - 20);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteUInt16((ushort)ParameterId);
                data.WriteByte(PDLSize);
                data.WriteByte(DataType);
                data.WriteByte(CommandClass);
                data.WriteByte(Type);
                data.WriteByte(Unit);
                data.WriteByte(Prefix);
                data.WriteHiLoInt32(MinValidValue);
                data.WriteHiLoInt32(MaxValidValue);
                data.WriteHiLoInt32(DefaultValue);
                data.WriteBytes(Encoding.ASCII.GetBytes(Description));
                ;
            }
        }
    }
}
