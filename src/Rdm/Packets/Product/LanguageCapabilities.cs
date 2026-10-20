namespace Haukcode.ArtNet.Rdm.Packets.Product;

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
        }

        /// <summary>The supported 2-character ISO 639-1 language codes.</summary>
        public List<string> LanguageCodes { get; set; } = [];

        /// <summary>
        /// The supported languages that are known cultures.
        /// </summary>
        public IEnumerable<CultureInfo> Languages =>
            LanguageCodes.Select(Product.LanguageCodes.ToCulture).OfType<CultureInfo>();

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            LanguageCodes.Clear();
            for (int n = 0; n < Header.ParameterDataLength / Product.LanguageCodes.Length; n++)
                LanguageCodes.Add(Product.LanguageCodes.Read(data));
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            foreach (string code in LanguageCodes)
                Product.LanguageCodes.Write(data, code);
        }

        #endregion
    }
}
