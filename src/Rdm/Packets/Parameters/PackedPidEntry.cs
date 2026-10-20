namespace Haukcode.ArtNet.Rdm.Packets.Parameters;

/// <summary>
/// One entry of a PACKED_PID_SUB or PACKED_PID_INDEX list: the sub-device or item it belongs to, and
/// that PID's parameter data for it.
/// </summary>
public readonly record struct PackedPidEntry(ushort Id, byte[] Data)
{
    internal static List<PackedPidEntry> ReadList(RdmBinaryReader data, int length)
    {
        var entries = new List<PackedPidEntry>();

        // Id (16 bits), PDL (8 bits), PD
        while (length >= 3)
        {
            ushort id = data.ReadUInt16();
            byte parameterDataLength = data.ReadByte();
            length -= 3;

            if (parameterDataLength > length)
                throw new InvalidPacketException($"A packed entry claims {parameterDataLength} bytes but only {length} remain.");

            entries.Add(new PackedPidEntry(id, data.ReadBytes(parameterDataLength)));
            length -= parameterDataLength;
        }

        return entries;
    }

    internal static void WriteList(RdmBinaryWriter data, List<PackedPidEntry> entries)
    {
        foreach (var entry in entries)
        {
            if (entry.Data.Length > RdmHeader.MaxParameterDataLength)
                throw new InvalidOperationException($"A packed entry's data is at most {RdmHeader.MaxParameterDataLength} bytes.");

            data.WriteUInt16(entry.Id);
            data.WriteByte((byte)entry.Data.Length);
            data.WriteBytes(entry.Data);
        }
    }
}
