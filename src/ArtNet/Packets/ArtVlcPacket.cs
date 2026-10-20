namespace Haukcode.ArtNet.Packets;

/// <summary>VLC flags (ArtVlc Flags byte).</summary>
[Flags]
public enum ArtVlcFlags : byte
{
    None = 0x00,

    /// <summary>Repeat the transmission until another packet is received.</summary>
    Beacon = 0x20,

    /// <summary>A reply to the request with the same transaction number.</summary>
    Reply = 0x40,

    /// <summary>The payload is IEEE VLC data; otherwise <see cref="ArtVlcPacket.PayloadLanguage"/> defines it.</summary>
    Ieee = 0x80
}

/// <summary>Registered VLC payload languages.</summary>
public enum ArtVlcPayloadLanguages : ushort
{
    BeaconUrl = 0x0000,
    BeaconText = 0x0001,
    BeaconLocationId = 0x0002
}

/// <summary>
/// Visible Light Communication data (Art-Net 4): an ArtNzs with start code 0x91 whose data starts
/// with the VLC header. Unicast only.
/// </summary>
public class ArtVlcPacket : ArtNzsPacket
{
    public const byte VlcStartCode = 0x91;

    // Magic numbers at the start of the VLC data: ManIdHi, ManIdLo, SubCode
    private static ReadOnlySpan<byte> Magic => [0x41, 0x4C, 0x45];

    // Magic (3), Flags, Trans (2), SlotAddr (2), PayCount (2), PayCheck (2), Spare, Depth,
    // Freq (2), Mod (2), PayLang (2), BeacRep (2)
    private const int HeaderLength = 22;

    /// <summary>Most payload bytes that fit in the 512 slots after the VLC header.</summary>
    public const int MaxPayloadLength = MaxDataLength - HeaderLength;

    public ArtVlcPacket()
    {
        StartCode = VlcStartCode;
    }

    public ArtVlcFlags Flags { get; set; }

    /// <summary>0 for the first packet of a transaction, 0xFFFF for the last; others count up from 1.</summary>
    public ushort Transaction { get; set; }

    /// <summary>The slot (1-512) of the device the packet is for; 0 means every device on the Port-Address.</summary>
    public ushort SlotAddress { get; set; }

    /// <summary>Modulation depth in percent (1-100); 0 means the transmitter default.</summary>
    public byte Depth { get; set; }

    /// <summary>Modulation frequency in Hz; 0 means the transmitter default.</summary>
    public ushort Frequency { get; set; }

    /// <summary>Modulation type; 0 means the transmitter default.</summary>
    public ushort Modulation { get; set; }

    public ArtVlcPayloadLanguages PayloadLanguage { get; set; }

    /// <summary>Beacon repeat frequency in Hz (with <see cref="ArtVlcFlags.Beacon"/>); 0 means the transmitter default.</summary>
    public ushort BeaconRepeat { get; set; }

    public byte[] Payload { get; set; } = [];

    /// <summary>The VLC header followed by the payload, carried as the ArtNzs data.</summary>
    public override ReadOnlyMemory<byte> Data
    {
        get => BuildData();
        set { }
    }

    internal static bool IsVlc(byte startCode, ReadOnlySpan<byte> data) =>
        startCode == VlcStartCode && data.Length >= HeaderLength && data[..3].SequenceEqual(Magic);

    internal static ArtVlcPacket FromNzs(byte sequence, ushort universe, byte[] data)
    {
        static ushort U16(byte[] d, int i) => (ushort)((d[i] << 8) | d[i + 1]);

        // Vlc[0-2] magic, [3] Flags, [4-5] Trans, [6-7] SlotAddr, [8-9] PayCount, [10-11] PayCheck,
        // [12] spare, [13] Depth, [14-15] Freq, [16-17] Mod, [18-19] PayLang, [20-21] BeacRep
        int payloadCount = Math.Min(U16(data, 8), data.Length - HeaderLength);

        return new ArtVlcPacket
        {
            Sequence = sequence,
            Universe = universe,
            Flags = (ArtVlcFlags)data[3],
            Transaction = U16(data, 4),
            SlotAddress = U16(data, 6),
            Depth = data[13],
            Frequency = U16(data, 14),
            Modulation = U16(data, 16),
            PayloadLanguage = (ArtVlcPayloadLanguages)U16(data, 18),
            BeaconRepeat = U16(data, 20),
            Payload = data.AsSpan(HeaderLength, payloadCount).ToArray()
        };
    }

    /// <summary>16-bit unsigned additive checksum of the payload, as the PayCheck field carries it.</summary>
    public static ushort PayloadChecksum(ReadOnlySpan<byte> payload)
    {
        ushort sum = 0;
        foreach (byte value in payload)
            sum += value;

        return sum;
    }

    private byte[] BuildData()
    {
        if (Payload.Length > MaxPayloadLength)
            throw new InvalidOperationException($"ArtVlc payload is at most {MaxPayloadLength} bytes.");

        var data = new byte[HeaderLength + Payload.Length];
        Magic.CopyTo(data);
        data[3] = (byte)Flags;
        Put(data, 4, Transaction);
        Put(data, 6, SlotAddress);
        Put(data, 8, (ushort)Payload.Length);
        Put(data, 10, PayloadChecksum(Payload));
        data[12] = 0;
        data[13] = Depth;
        Put(data, 14, Frequency);
        Put(data, 16, Modulation);
        Put(data, 18, (ushort)PayloadLanguage);
        Put(data, 20, BeaconRepeat);
        Payload.CopyTo(data, HeaderLength);

        return data;

        static void Put(byte[] d, int i, ushort value)
        {
            d[i] = (byte)(value >> 8);
            d[i + 1] = (byte)value;
        }
    }
}
