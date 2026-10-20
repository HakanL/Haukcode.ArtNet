namespace Haukcode.ArtNet.Packets;

/// <summary>
/// ArtPoll (Art-Net 4). Receivers accept 14-byte packets and treat missing fields as zero.
/// </summary>
public class ArtPollPacket : ArtNetPacket
{
    public ArtPollPacket()
        : base(ArtNetOpCodes.Poll)
    {
    }

    // Flags, DiagPriority, TargetPortAddressTop (2), TargetPortAddressBottom (2), EstaMan (2), Oem (2)
    protected override int DataLength => 10;

    internal static ArtPollPacket Parse(BigEndianBinaryReader reader)
    {
        var target = new ArtPollPacket
        {
            TalkToMe = reader.ReadByte(),
            DiagPriority = reader.BytesLeft > 0 ? reader.ReadByte() : (byte)0
        };

        // Fields added in later revisions; older controllers send a shorter packet
        if (reader.BytesLeft >= 2)
            target.TargetPortAddressTop = reader.ReadUInt16();
        if (reader.BytesLeft >= 2)
            target.TargetPortAddressBottom = reader.ReadUInt16();
        if (reader.BytesLeft >= 2)
            target.EstaManufacturer = reader.ReadUInt16();
        if (reader.BytesLeft >= 2)
            target.Oem = reader.ReadUInt16();

        reader.SkipBytes(reader.BytesLeft);

        return target;
    }

    /// <summary>
    /// The raw Flags byte; see <see cref="Flags"/>.
    /// </summary>
    public byte TalkToMe { get; set; }

    public ArtPollFlags Flags
    {
        get => (ArtPollFlags)TalkToMe;
        set => TalkToMe = (byte)value;
    }

    /// <summary>
    /// The lowest priority of diagnostics messages that should be sent.
    /// </summary>
    public byte DiagPriority { get; set; }

    /// <summary>Top of the Port-Address range nodes must be subscribed to (with <see cref="ArtPollFlags.TargetedMode"/>).</summary>
    public ushort TargetPortAddressTop { get; set; }

    /// <summary>Bottom of the Port-Address range nodes must be subscribed to (with <see cref="ArtPollFlags.TargetedMode"/>).</summary>
    public ushort TargetPortAddressBottom { get; set; }

    /// <summary>ESTA manufacturer code of the sender.</summary>
    public ushort EstaManufacturer { get; set; }

    /// <summary>OEM code of the sending product.</summary>
    public ushort Oem { get; set; }

    /// <summary>
    /// Poll only nodes subscribed to a Port-Address between <paramref name="bottom"/> and
    /// <paramref name="top"/>, inclusive.
    /// </summary>
    public static ArtPollPacket Targeted(ushort bottom, ushort top, ArtPollFlags flags = ArtPollFlags.None) => new()
    {
        Flags = flags | ArtPollFlags.TargetedMode,
        TargetPortAddressBottom = bottom,
        TargetPortAddressTop = top
    };

    protected override void WriteData(ref SpanBinaryWriter writer)
    {
        writer.WriteByte(TalkToMe);
        writer.WriteByte(DiagPriority);
        writer.WriteUInt16(TargetPortAddressTop);
        writer.WriteUInt16(TargetPortAddressBottom);
        writer.WriteUInt16(EstaManufacturer);
        writer.WriteUInt16(Oem);
    }
}
