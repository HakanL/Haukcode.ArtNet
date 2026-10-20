namespace Haukcode.ArtNet.Rdm.Packets.Product;

/// <summary>
/// This parameter shall be used for requesting technology details for a device.
/// </summary>
/// <remarks>
/// Product Detail information may be used by the controller for grouping or other sorting methods
/// when patching or displaying system information. Not all Product Detail definitions may be
/// appropriate for all Product Categories.
/// 
/// This standard does not place any restrictions on the use of Product Categories and Product
/// Detail, which are intended to convey general information about the product to the controller.
/// 
/// Devices fitted with Residual Current Detectors (RCD) or Ground Fault Interrupt (GFI) devices
/// may declare such functionality using this PID.
/// </remarks>
public class ProductDetailIdList
{
    /// <summary>
    /// A list of properties a fixture may contain to be obtained through the
    /// ProductDetailIdList message.
    /// </summary>
    public enum DetailId
    {
        NotDeclared = 0x0,

        //Fixtures
        Arc = 0x1,
        MetalHalide = 0x2,
        Incandescent = 0x3,
        Led = 0x4,
        Fluorescent = 0x5,
        ColdCathode = 0x6,
        Electroluminescent = 0x7,
        Laser = 0x8,
        Flashtube = 0x9,

        //Fixture Accessories
        ColorScroller = 0x100,
        ColorWheel = 0x101,
        ColorChange = 0x102,
        IrisDouser = 0x103,
        DimmingShutter = 0x104,
        ProfileShutter = 0x105,
        BarndoorShutter = 0x106,
        EffectsDisc = 0x107,
        GoboRotator = 0x108,

        //Projectors
        Video = 0x200,
        Slide = 0x201,
        Film = 0x202,
        OilWheel = 0x203,
        LcdGate = 0x204,

        //Atmospheric Effects
        FoggerGlycol = 0x300,
        FoggerMineralOil = 0x301,
        FoggerWater = 0x302,
        CO2 = 0x303,
        LN2 = 0x304,
        Bubble = 0x305,
        FlamePropane = 0x306,
        FlameOther = 0x307,
        OlfactoryStimulator = 0x308,
        Snow = 0x309,
        WaterJet = 0x30A,
        Wind = 0x30B,
        Confetti = 0x30C,
        Hazard = 0x30D,

        // Dimmer types
        PhaseControl = 0x0400,
        ReversePhaseControl = 0x0401,
        Sine = 0x0402,
        Pwm = 0x0403,
        DC = 0x0404,
        HfBallast = 0x0405,
        HfHvNeonBallast = 0x0406,
        HfHvEl = 0x0407,
        MhrBallast = 0x0408,
        BitAngleModulation = 0x0409,
        FrequencyModulation = 0x040A,
        HighFrequency12V = 0x040B,
        RelayMechanical = 0x040C,
        RelayElectronic = 0x040D,
        SwitchElectronic = 0x040E,
        Contactor = 0x040F,

        // Scenic
        MirrorballRotator = 0x0500,
        OtherRotator = 0x0501,
        KabukiDrop = 0x0502,
        Curtain = 0x0503,
        Lineset = 0x0504,
        MotorControl = 0x0505,
        DamperControl = 0x0506,

        // Data distribution
        Splitter = 0x0600,
        EthernetNode = 0x0601,
        Merge = 0x0602,
        Datapatch = 0x0603,
        WirelessLink = 0x0604,

        // Data conversion
        ProtocolConverter = 0x0701,
        AnalogDemultiplex = 0x0702,
        AnalogMultiplex = 0x0703,
        SwitchPanel = 0x0704,

        // Audio-visual
        Router = 0x0800,
        Fader = 0x0801,
        Mixer = 0x0802,

        // Connections
        ChangeoverManual = 0x0900,
        ChangeoverAuto = 0x0901,
        Test = 0x0902,

        // Power
        GfiRcd = 0x0A00,
        Battery = 0x0A01,
        ControllableBreaker = 0x0A02,

        // Sensors
        Input = 0x0B00,
        Sensor = 0x0B01,

        Other = 0x7FFF
    }

    public class Get : RdmRequestPacket
    {
        public Get()
            : base(RdmCommands.Get,RdmParameters.ProductDetailIdList)
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
            : base(RdmCommands.GetResponse, RdmParameters.ProductDetailIdList)
        {
            Details = new List<DetailId>();
        }

        public List<DetailId> Details { get; set; }

        #region Read and Write

        protected override void ReadData(RdmBinaryReader data)
        {
            for (int n = 0; n < Header.ParameterDataLength / 2; n++)
                Details.Add((DetailId)data.ReadUInt16());
        }

        protected override void WriteData(RdmBinaryWriter data)
        {
            foreach (DetailId id in Details)
                data.WriteUInt16((short)id);
        }

        #endregion
    }
}
