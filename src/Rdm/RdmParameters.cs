namespace Haukcode.ArtNet.Rdm;

public enum RdmParameters
{
    None = 0,

    //---- Network Management ----
    DiscoveryUniqueBranch = 0x0001,
    DiscoveryMute = 0x0002,
    DiscoveryUnMute = 0x0003,
    ProxiedDevices = 0x0010,
    ProxiedDeviceCount = 0x0011,
    CommsStatus = 0x0015,

    //---- Status ----
    QueuedMessage = 0x020,
    StatusMessage = 0x030,
    StatusIdDescription = 0x031,
    ClearStatusId = 0x032,
    SubDeviceStatusReportThreshold = 0x033,

    //---- RDM ----
    SupportedParameters = 0x0050,
    ParameterDescription = 0x0051,

    //---- Product Information ----
    DeviceInfo = 0x0060,
    ProductDetailIdList = 0x0070,
    DeviceModelDescription = 0x0080,
    ManufacturerLabel = 0x0081,
    DeviceLabel = 0x0082,
    FactoryDefaults = 0x0090,
    LanguageCapabilities = 0x00A0,
    Language = 0x00B0,
    SoftwareVersionLabel = 0x00C0,
    BootSoftwareVersionId = 0x00C1,
    BootSoftwareVersionLabel = 0x00C2,

    //---- DMX512 ----
    DmxPersonality = 0x00E0,
    DmxPersonalityDescription = 0x00E1,
    DmxStartAddress = 0x00F0,
    SlotInfo = 0x0120,
    SlotDescription = 0x0121,
    DefaultSlotValue = 0x0122,
    DmxBlockAddress = 0x0140,
    DmxFailMode = 0x0141,
    DmxStartupMode = 0x0142,

    //---- Sensors ----
    SensorDefinition = 0x0200,
    SensorValue = 0x0201,
    RecordSensors = 0x0202,

    //---- Dimmers ----
    DimmerInfo = 0x0340,
    MinimumLevel = 0x0341,
    MaximumLevel = 0x0342,
    Curve = 0x0343,
    CurveDescription = 0x0344,
    OutputResponseTime = 0x0345,
    OutputResponseTimeDescription = 0x0346,
    ModulationFrequency = 0x0347,
    ModulationFrequencyDescription = 0x0348,

    //---- Power/Lamp Settings ----
    DeviceHours = 0x0400,
    LampHours = 0x0401,
    LampStrikes = 0x0402,
    LampState = 0x0403,
    LampOnMode = 0x0404,
    DevicePowerCycles = 0x0405,
    BurnIn = 0x0440,

    //---- Display Settings ----
    DisplayInvert = 0x0500,
    DisplayLevel = 0x0501,

    //---- Configuration ----
    PanInvert = 0x0600,
    TiltInvert = 0x0601,
    PanTiltSwap = 0x0602,
    RealTimeClock = 0x0603,
    LockPin = 0x0640,
    LockState = 0x0641,
    LockStateDescription = 0x0642,

    //---- Control ----
    IdentifyDevice = 0x1000,
    ResetDevice = 0x1001,
    PowerState = 0x1010,
    PerformSelfTest = 0x1020,
    SelfTestDescription = 0x1021,
    CapturePreset = 0x1030,
    PresetPlayback = 0x1031,
    IdentifyMode = 0x1040,
    PresetInfo = 0x1041,
    PresetStatus = 0x1042,
    PresetMergeMode = 0x1043,
    PowerOnSelfTest = 0x1044,

    //---- RDMnet (E1.33 Table A-15) ----
    ComponentScope = 0x0800,
    SearchDomain = 0x0801,
    TcpCommsStatus = 0x0802,
    BrokerStatus = 0x0803,

    //---- Gateway and splitter configuration (E1.37-7 Table A-1) ----
    EndpointList = 0x0900,
    EndpointListChange = 0x0901,
    EndpointIdentify = 0x0902,
    EndpointToUniverse = 0x0903,
    EndpointMode = 0x0904,
    EndpointLabel = 0x0905,
    RdmTrafficEnable = 0x0906,
    DiscoveryState = 0x0907,
    BackgroundDiscovery = 0x0908,
    EndpointTiming = 0x0909,
    EndpointTimingDescription = 0x090A,

    /// <summary>ENDPOINT_RESPONDERS</summary>
    EndpointDevices = 0x090B,

    /// <summary>ENDPOINT_RESPONDER_LIST_CHANGE</summary>
    EndpointDeviceListChange = 0x090C,
    BindingControlFields = 0x090D,
    BackgroundQueuedStatusPolicy = 0x090E,
    BackgroundQueuedStatusPolicyDescription = 0x090F
}

public static class RdmParametersExtensions
{
    public static bool IsManufacturerPID(this RdmParameters pid)
    {
        return (int)pid >= 0x8000 && (int)pid <= 0xFFDF;
    }
}
