namespace Haukcode.ArtNet.Rdm.Packets.Control;

/// <summary>
/// Which self tests exist, which are running, how they behave and their results
/// (E1.20-2025 10.11.8). GET only. With ACK_OVERFLOW only the first message carries the result
/// code PID; read the sequence with <see cref="RdmPacket.ReadOverflowPacket"/>.
/// </summary>
public class SelfTestEnhanced
{
    /// <summary>Self Test Status (E1.20-2025 Table 10-4).</summary>
    public enum SelfTestStatus : byte
    {
        NotSupported = 0x00,
        NotRun = 0x01,
        Aborted = 0x02,
        Active = 0x03,
        Pass = 0x04,
        Fail = 0x05,
        NoAnalysis = 0x06,
        ResultCodeAvailable = 0x07,
        Other = 0xFF
    }

    /// <summary>Self Test Capability (E1.20-2025 Table 10-3).</summary>
    [Flags]
    public enum SelfTestCapabilities : ushort
    {
        None = 0x0000,
        AutoTerminates = 0x0001,
        DmxRestricted = 0x0002,
        RdmRestricted = 0x0004,
        AllIgnoresAutoTerminate = 0x0008,
        ResultCodes = 0x0010,
        StatusMessages = 0x0020
    }

    public readonly record struct SelfTest(byte TestNumber, SelfTestStatus Status, SelfTestCapabilities Capabilities, ushort ResultCode);

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get, RdmParameters.SelfTestEnhanced)
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
        // Test number, status, capability (16 bits), result code (16 bits)
        private const int RecordSize = 6;

        public GetReply()
            : base(RdmCommands.GetResponse, RdmParameters.SelfTestEnhanced)
        {
        }

        /// <summary>
        /// PID that enumerates the manufacturer-specific result codes (use with ENUM_LABEL), or 0.
        /// </summary>
        public RdmParameters ResultCodeEnumerationPid { get; set; }

        public List<SelfTest> Tests { get; set; } = [];

        protected override void ReadData(RdmBinaryReader data)
        {
            ResultCodeEnumerationPid = (RdmParameters)data.ReadUInt16();

            Tests = [];
            for (int n = 0; n < (Header.ParameterDataLength - 2) / RecordSize; n++)
                Tests.Add(new SelfTest(data.ReadByte(), (SelfTestStatus)data.ReadByte(), (SelfTestCapabilities)data.ReadUInt16(), data.ReadUInt16()));
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            data.WriteUInt16((ushort)ResultCodeEnumerationPid);
            foreach (var test in Tests)
            {
                data.WriteByte(test.TestNumber);
                data.WriteByte((byte)test.Status);
                data.WriteUInt16((ushort)test.Capabilities);
                data.WriteUInt16(test.ResultCode);
            }
        }
    }
}
