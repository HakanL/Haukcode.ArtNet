using System;

namespace Haukcode.ArtNet
{
    public enum ArtNetOpCodes : ushort
    {
        None = 0x0000,
        Poll = 0x2000,
        PollReply = 0x2100,
        DiagData = 0x2300,
        Command = 0x2400,
        DataRequest = 0x2700,
        DataReply = 0x2800,
        [Obsolete]
        Dmx = 0x5000,           // Duplicate, non-standard names
        Output = 0x5000,
        Nzs = 0x5100,
        Sync = 0x5200,
        Address = 0x6000,
        Input = 0x7000,
        TodRequest = 0x8000,
        TodData = 0x8100,
        TodControl = 0x8200,
        Rdm = 0x8300,
        RdmSub = 0x8400,
        VideoSetup = 0xa010,
        Videalette = 0xa020,
        VideoData = 0xa040,
        MacMaster = 0xf000,
        MacSlave = 0xf100,
        FirmwareMaster = 0xf200,
        FirmwareReply = 0xf300,
        FileTnMaster = 0xf400,
        FileFnMaster = 0xf500,
        FileFnReply = 0xf600,
        IpProg = 0xf800,
        IpProgReply = 0xf900,
        Media = 0x9000,
        MediaPatch = 0x9100,
        MediaControl = 0x9200,
        MediaContrlReply = 0x9300,
        TimeCode = 0x9700,
        TimeSync = 0x9800,
        [Obsolete]
        ArtTrigger = 0x9900,        // Duplicate, non-standard names
        Trigger = 0x9900,
        Directory = 0x9a00,
        DirectoryReply = 0x9b00
    }

    public enum ArtNetStyles
    {
        StNode = 0x00,
        StServer = 0x01,
        StMedia = 0x02,
        StRoute = 0x03,
        StBackup = 0x04,
        StConfig = 0x05
    }

    [Flags]
    public enum ArtIpProgCommand : byte
    {
        None = 0,
        Netmask = (1 << 1),
        IpAddress = (1 << 2),
        ResetToDefault = (1 << 3),
        DefaultGateway = (1 << 4),
        EnableDHCP = (1 << 6),
        EnableProgramming = (1 << 7),
    }

    
    public enum ArtAddressCommand : byte
    {
        AcNone = 0x00,
        AcCancelMerge = 0x01,
        AcLedNormal = 0x02,
        AcLedMute = 0x03,
        AcLedLocate = 0x04,
        AcResetRxFlags = 0x05,
        AcAnalysisOn = 0x06,
        AcAnalysisOff = 0x07,
        AcFailHold = 0x08,
        AcFailZero = 0x09,
        AcFailFull = 0x0A,
        AcFailScene = 0x0B,
        AcFailRecord = 0x0C,
        AcMergeLtp0 = 0x10,
        AcMergeLtp1 = 0x11,
        AcMergeLtp2 = 0x12,
        AcMergeLtp3 = 0x13,
        AcDirectionTx0 = 0x20,
        AcDirectionTx1 = 0x21,
        AcDirectionTx2 = 0x22,
        AcDirectionTx3 = 0x23,
        AcDirectionRx0 = 0x30,
        AcDirectionRx1 = 0x31,
        AcDirectionRx2 = 0x32,
        AcDirectionRx3 = 0x33,
        AcMergeHtp0 = 0x50,
        AcMergeHtp1 = 0x51,
        AcMergeHtp2 = 0x52,
        AcMergeHtp3 = 0x53,
        AcArtNetSel0 = 0x60,
        AcArtNetSel1 = 0x61,
        AcArtNetSel2 = 0x62,
        AcArtNetSel3 = 0x63,
        AcAcnSel0 = 0x70,
        AcAcnSel1 = 0x71,
        AcAcnSel2 = 0x72,
        AcAcnSel3 = 0x73,
        AcClearOp0 = 0x90,
        AcClearOp1 = 0x91,
        AcClearOp2 = 0x92,
        AcClearOp3 = 0x93,
        AcStyleDelta0 = 0xA0,
        AcStyleDelta1 = 0xA1,
        AcStyleDelta2 = 0xA2,
        AcStyleDelta3 = 0xA3,
        AcStyleConst0 = 0xB0,
        AcStyleConst1 = 0xB1,
        AcStyleConst2 = 0xB2,
        AcStyleConst3 = 0xB3,
        AcRdmEnable0 = 0xC0,
        AcRdmEnable1 = 0xC1,
        AcRdmEnable2 = 0xC2,
        AcRdmEnable3 = 0xC3,
        AcRdmDisable0 = 0xD0,
        AcRdmDisable1 = 0xD1,
        AcRdmDisable2 = 0xD2,
        AcRdmDisable3 = 0xD3
    }

    public enum ArtNetOemCodes
    {
        OemOarwSm1 = 0x09d0      //Company Name: Oarw, Product Name: Screen Monkey, Number of DMX Inputs: 0, Number of DMX Outputs: 1, Are dmx ports physical or emulated: Emulated, Is RDM Supported: Not at this time
    }
}
