namespace Haukcode.ArtNet.Rdm.Packets.DMX;

public enum SlotIds
{
    Undefined = 0xFFFF,

    //Intensity Functions
    Intensity = 1,
    IntensityMaster = 2,

    //Movement Functions
    Pan = 0x101,
    Tilt = 0x102,

    //Color Functions
    ColorWheel = 0x0201,
    ColorSubCyan = 0x0202,
    ColorSubYellow = 0x0203,
    ColorSubMagenta = 0x0204,
    ColorAddRed = 0x0205,
    ColorAddGreen = 0x0206,
    ColorAddBlue = 0x0207,
    ColorCorrection = 0x0208,
    ColorScroll = 0x0209,
    ColorAddLime = 0x020A,
    ColorAddIndigo = 0x020B,
    ColorAddCyan = 0x020C,
    ColorAddDeepRed = 0x020D,
    ColorAddDeepBlue = 0x020E,
    ColorAddNaturalWhite = 0x020F,
    ColorSemaphore = 0x0210,
    ColorAddAmber = 0x0211,
    ColorAddWhite = 0x0212,
    ColorAddWarmWhite = 0x0213,
    ColorAddCoolWhite = 0x0214,
    ColorSubUV = 0x0215,
    ColorHue = 0x0216,
    ColorSaturation = 0x0217,
    ColorAddUV = 0x0218,

    //Image Functions
    StaticGoboWheel = 0x0301,
    RotoGoboWheel = 0x0302,
    PrismWheel = 0x0303,
    EffectsWheel = 0x0304,

    //Beam Functions
    BeamSizeIris = 0x0401,
    Edge = 0x0402,
    Frost = 0x0403,
    Strobe = 0x0404,
    Zoom = 0x0405,
    FramingShutter = 0x0406,
    ShutterRotate = 0x0407,
    Douser = 0x0408,
    BarnDoors = 0x0409,

    //Control Functions
    LampControl = 0x0501,
    FixtureControl = 0x0502,
    FixtureSpeed = 0x0503,
    Macro = 0x0504,
    PowerControl = 0x0505,
    FanControl = 0x0506,
    HeaterControl = 0x0507,
    FountainControl = 0x0508
}
