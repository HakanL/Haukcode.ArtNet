namespace Haukcode.ArtNet.Packets;

public enum PollReplyPortTypes
{
    DMX512 = 0,
    Midi = 1,
    Avab = 2,
    ColortranCmx = 3,
    Adb62_5 = 4,
    ArtNet = 5,
    Dali = 6,

    IsInputPort = 64,
    IsOutputPort = 128,
}
