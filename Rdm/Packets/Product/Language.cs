using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Haukcode.Rdm.Packets.Product
{
    /// <summary>
    /// This parameter is used to change the language of the messages from the device.
    /// </summary>
    /// <remarks>
    /// Supported languages of the device can be determined by the LANGUAGE_CAPABILITIES.
    /// 
    /// The Language Codes are 2 character alpha codes as defined by ISO 639-1. International
    /// Standard ISO 639-1, Code for the representation of names of languages - Part 1: Alpha 2 code.
    /// </remarks>
    public class Language
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.Language)
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
                : base(RdmCommands.GetResponse, RdmParameters.Language)
            {
            }       

            public CultureInfo Language { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                Language = new CultureInfo(data.ReadString(Header.ParameterDataLength));
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteString(Language.TwoLetterISOLanguageName);
            }

            #endregion
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.Language)
            {
            }

            public CultureInfo Language { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                Language = new CultureInfo(data.ReadString(Header.ParameterDataLength));
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteString(Language.TwoLetterISOLanguageName);
            }

            #endregion
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.Language)
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
