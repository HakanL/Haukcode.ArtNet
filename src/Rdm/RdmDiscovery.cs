using Haukcode.ArtNet.Rdm.Packets.Discovery;

namespace Haukcode.ArtNet.Rdm;

/// <summary>
/// Finds the responders on a DMX line by binary search with DISC_UNIQUE_BRANCH, DISC_MUTE and
/// DISC_UN_MUTE (E1.20 7). This is for a controller that drives RDM on a line directly; over
/// Art-Net the node runs discovery itself and reports devices with ArtTodData.
/// </summary>
/// <remarks>
/// The class only runs the search. The line I/O is up to the caller, through three callbacks:
/// sending the requests (<see cref="DiscoveryUniqueBranch.Request"/>, <see cref="DiscoveryMute"/>
/// and <see cref="DiscoveryUnMute"/>) and collecting what comes back.
/// </remarks>
public class RdmDiscovery
{
    /// <summary>
    /// Sends DISC_UNIQUE_BRANCH for the range and returns the bytes received in the response
    /// window, or null (or empty) when nothing answered.
    /// </summary>
    public delegate Task<byte[]?> UniqueBranchHandler(UId lowerBound, UId upperBound, CancellationToken cancellationToken);

    /// <summary>Sends DISC_MUTE to the device; true when it answered with a valid mute response.</summary>
    public delegate Task<bool> MuteHandler(UId device, CancellationToken cancellationToken);

    /// <summary>Broadcasts DISC_UN_MUTE so every device takes part in discovery again.</summary>
    public delegate Task UnMuteAllHandler(CancellationToken cancellationToken);

    // The whole UID space except the broadcast address FFFF:FFFFFFFF
    private const ulong LowestUId = 0;
    private const ulong HighestUId = 0xFFFF_FFFF_FFFE;

    private readonly UniqueBranchHandler uniqueBranch;
    private readonly MuteHandler mute;
    private readonly UnMuteAllHandler unMuteAll;

    public RdmDiscovery(UniqueBranchHandler uniqueBranch, MuteHandler mute, UnMuteAllHandler unMuteAll)
    {
        this.uniqueBranch = uniqueBranch;
        this.mute = mute;
        this.unMuteAll = unMuteAll;
    }

    /// <summary>
    /// Runs a full discovery: un-mutes every device, then searches the whole UID range.
    /// </summary>
    /// <returns>The devices found, each confirmed with DISC_MUTE, in the order they were found.</returns>
    public async Task<IReadOnlyList<UId>> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        await this.unMuteAll(cancellationToken);

        return await DiscoverMoreAsync(cancellationToken);
    }

    /// <summary>
    /// Incremental discovery: searches without un-muting first, so only devices that aren't
    /// muted (new ones, or ones that lost power) answer.
    /// </summary>
    public async Task<IReadOnlyList<UId>> DiscoverMoreAsync(CancellationToken cancellationToken = default)
    {
        var found = new List<UId>();
        await SearchAsync(LowestUId, HighestUId, found, cancellationToken);

        return found;
    }

    private async Task SearchAsync(ulong lower, ulong upper, List<UId> found, CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var response = await this.uniqueBranch(ToUId(lower), ToUId(upper), cancellationToken);
            if (response == null || response.Length == 0)
                return;

            // A clean response is one device, unless a collision happened to decode: DISC_MUTE
            // confirms it. Then search the same branch again for the devices it hid.
            if (DiscoveryUniqueBranchResponse.TryDecode(response, out var device))
            {
                ulong value = ToUInt64(device);
                if (value >= lower && value <= upper && !found.Contains(device) && await this.mute(device, cancellationToken))
                {
                    found.Add(device);
                    continue;
                }
            }

            // Several devices answered at once: split the range. A single UID that still doesn't
            // give a usable answer is given up on, so a faulty responder can't stall the search.
            if (lower == upper)
                return;

            ulong middle = lower + (upper - lower) / 2;
            await SearchAsync(lower, middle, found, cancellationToken);
            await SearchAsync(middle + 1, upper, found, cancellationToken);
            return;
        }
    }

    private static ulong ToUInt64(UId id) => ((ulong)id.ManufacturerId << 32) | id.DeviceId;

    private static UId ToUId(ulong value) => new UId((ushort)(value >> 32), (uint)value);
}
