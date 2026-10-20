namespace Haukcode.ArtNet.Rdm.Packets.Parameters;

/// <summary>
/// Supported PIDs with how each can be used (E1.20-2025 10.4.4). GET only. Unlike
/// SUPPORTED_PARAMETERS, the list includes every supported PID except the discovery PIDs.
/// </summary>
public class SupportedParametersEnhanced
{
    /// <summary>PID Support bits (E1.20-2025 Table 10-1a).</summary>
    [Flags]
    public enum PidSupport : ushort
    {
        None = 0x0000,
        Get = 0x0001,
        Set = 0x0002,
        PackedPidSubGet = 0x0004,
        PackedPidSubSet = 0x0008,
        PackedPidIndexGet = 0x0010,
        PackedPidIndexSet = 0x0020,
        NonIdenticalSubDevices = 0x0040,
        JsonMetadata = 0x0080
    }

    public readonly record struct SupportedParameter(RdmParameters ParameterId, PidSupport Support);

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.SupportedParametersEnhanced)
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
        // PID (16 bits) + PID Support (16 bits)
        private const int RecordSize = 4;

        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.SupportedParametersEnhanced)
        {
        }

        public List<SupportedParameter> Parameters { get; set; } = [];

        protected override void ReadData(RdmBinaryReader data)
        {
            Parameters = [];
            for (int n = 0; n < Header.ParameterDataLength / RecordSize; n++)
                Parameters.Add(new SupportedParameter((RdmParameters)data.ReadUInt16(), (PidSupport)data.ReadUInt16()));
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            foreach (var parameter in Parameters)
            {
                data.WriteUInt16((ushort)parameter.ParameterId);
                data.WriteUInt16((ushort)parameter.Support);
            }
        }
    }
}
