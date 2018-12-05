using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Haukcode.Rdm.Packets.Product
{
    /// <summary>
    /// This parameter is used to identify languages that the device supports for using the LANGUAGE
    /// parameter.
    /// </summary>
    /// <remarks>
    /// The response contains a packed message of 2 character Language Codes as defined
    /// by ISO 639-1. International Standard ISO 639-1, Code for the representation of names of
    /// languages - Part 1: Alpha 2 code.
    /// </remarks>
    public class LanguageCapabilities
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.LanguageCapabilities)
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
                : base(RdmCommands.GetResponse, RdmParameters.LanguageCapabilities)
            {
                Languages = new List<CultureInfo>();
            }

            public List<CultureInfo> Languages { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                for(int n=0;n<Header.ParameterDataLength/2;n++)
                    Languages.Add(new CultureInfo(data.ReadNetworkString(2)));
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                foreach(CultureInfo language in Languages)
                    data.WriteNetwork(language.TwoLetterISOLanguageName);
            }

            #endregion
        }
    }
}
