namespace Haukcode.ArtNet.Packets;

/// <summary>Front panel indicator state (ArtPollReply Status1 bits 7-6).</summary>
public enum IndicatorStates : byte
{
    Unknown = 0,
    Locate = 1,
    Mute = 2,
    Normal = 3
}

/// <summary>Who set the Port-Address (ArtPollReply Status1 bits 5-4).</summary>
public enum PortAddressAuthorities : byte
{
    Unknown = 0,
    FrontPanel = 1,
    Network = 2
}

/// <summary>ArtPollReply Status1 bits 2-0.</summary>
[Flags]
public enum PollReplyStatus1 : byte
{
    None = 0x00,
    UbeaPresent = 0x01,
    RdmCapable = 0x02,
    BootedFromRom = 0x04
}

/// <summary>ArtPollReply Status2.</summary>
[Flags]
public enum PollReplyStatus2 : byte
{
    None = 0x00,
    WebConfiguration = 0x01,
    DhcpConfigured = 0x02,
    DhcpCapable = 0x04,
    PortAddress15Bit = 0x08,
    ArtNetSacnSwitchable = 0x10,
    Squawking = 0x20,
    OutputStyleSwitchable = 0x40,
    RdmControlViaArtAddress = 0x80
}

/// <summary>What a node does when network data is lost (ArtPollReply Status3 bits 7-6).</summary>
public enum FailsafeStates : byte
{
    Hold = 0,
    Zero = 1,
    Full = 2,
    Scene = 3
}

/// <summary>ArtPollReply Status3 bits 5-0.</summary>
[Flags]
public enum PollReplyStatus3 : byte
{
    None = 0x00,
    BackgroundDiscoveryControl = 0x01,
    BackgroundQueue = 0x02,
    RdmNet = 0x04,
    PortDirectionSwitchable = 0x08,
    Llrp = 0x10,
    ProgrammableFailsafe = 0x20
}

/// <summary>Per-port input status (ArtPollReply GoodInput).</summary>
[Flags]
public enum GoodInputFlags : byte
{
    None = 0x00,

    /// <summary>Set: the input converts to sACN; clear: to Art-Net.</summary>
    ConvertToSacn = 0x01,
    ReceiveErrors = 0x04,
    InputDisabled = 0x08,
    TextPackets = 0x10,
    Sips = 0x20,
    TestPackets = 0x40,
    DataReceived = 0x80
}

/// <summary>Per-port output status (ArtPollReply GoodOutputA).</summary>
[Flags]
public enum GoodOutputAFlags : byte
{
    None = 0x00,

    /// <summary>Set: the output converts from sACN; clear: from Art-Net.</summary>
    ConvertFromSacn = 0x01,
    MergeModeLtp = 0x02,
    ShortDetected = 0x04,
    Merging = 0x08,
    TextPackets = 0x10,
    Sips = 0x20,
    TestPackets = 0x40,
    DataOutput = 0x80
}

/// <summary>Per-port output status (ArtPollReply GoodOutputB).</summary>
[Flags]
public enum GoodOutputBFlags : byte
{
    None = 0x00,
    BackgroundDiscoveryDisabled = 0x10,
    DiscoveryNotRunning = 0x20,
    OutputStyleContinuous = 0x40,
    RdmDisabled = 0x80
}

/// <summary>How a node collects status and queued messages from its RDM devices (ArtPollReply BackgroundQueuePolicy).</summary>
public enum BackgroundQueuePolicies : byte
{
    StatusNone = 0,
    StatusAdvisory = 1,
    StatusWarning = 2,
    StatusError = 3,
    Disabled = 4
}
