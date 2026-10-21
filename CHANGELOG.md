# Changelog

The package version is major.minor from version.json plus the CI run number, so
every push to main publishes a new patch version. Breaking changes bump the
major version.

## 5.0 (2026-10-20) - breaking

Standards compliance pass against Art-Net 4 and ANSI E1.20-2025.

- Breaking: fields that Art-Net 4 and E1.20 / E1.37-7 define as unsigned are
  now ushort/uint, so values with the top bit set no longer read back negative
  (for example ArtPollReplyPacket.Oem, EstaCode and FirmwareVersion,
  ArtNetDmxPacket.Universe, ArtTriggerPacket.OemCode, RDM parameter IDs,
  sub-devices and sensor values). ArtNetUnknownPacket takes ArtNetOpCodes.
- Wire fixes: Art-Net packet layouts and length validation, RDM packets that
  sent or parsed the wrong parameter data, RDM response type and Port ID read
  from the header, NACK reason length, ArtSync Aux byte order.
- ArtNetClient follows the Art-Net 4 transmit rules and fixes sequence
  wrap-around, the loopback filter and destination handling.
- Added the remaining Art-Net 4 packets, ArtPoll targeted mode and the newer
  ArtPollReply fields.
- RDM: request workflow and direct-line discovery, a DISC_UNIQUE_BRANCH
  encoder/decoder, typed packets for the E1.20-2025 PIDs, E1.20-2025 sensor
  types and units, the E1.37-2 IPv4/DNS PIDs, the remaining E1.33 PIDs and the
  published E1.37-7 and E1.33 gateway PIDs. All RDM packet types are
  registered in RdmPacketFactory.
- RDM robustness: length limits, malformed replies and checksum validation.
- Timecode generator sample: restart and drift fixes.
- The package targets net10.0 and embeds its PDB with SourceLink, so the
  debugger can step into the library source (5.0.149 and later).

## 4.0 (2026-06-02) - breaking

- RDM correctness: SET NACKs are no longer parsed as successful replies, and
  the ProductCategories Control block, SlotIds.ColorSemaphore and
  PowerState.Normal values match E1.20.
- Sensor definition and sensor value replies parse to typed packets.
- Breaking: misspelled identifiers renamed (for example SensoreValue to
  SensorValue, IdentifyModes.Quit to Quiet, Colour* to Color*), and RDM PIDs
  moved to namespaces matching their category.
- Added net10.0.

Later 4.0.x releases:

- Typed ArtTimeCode packet (OpTimeCode 0x9700) and a timecode generator sample.
- Tolerates ArtSync packets without the spare Aux bytes and truncated legacy
  ArtPollReply packets.
- Kernel receive timestamps on Linux (KernelReceiveTimestampsObserved).
- Performance: no per-packet allocations on the send and receive paths,
  sending sharded by universe, zero-copy DMX data on parse.
- SendFromListenSocketAsync.

## 3.0 (2025-04-24) - breaking

- RDM split into its own assembly, Haukcode.ArtNet.Rdm, shipped in this
  package.

Later 3.0.x releases: .NET 10 support, a send-immediately method, and a check
for DMX4ALL custom packets.

## 2.0 (2024-12-12) - breaking

- Rewritten on the Haukcode.HighPerfComm library.
- Later 2.0.x releases: .NET 8 support, binding fix, OpReply data length fix.

## 1.x

- 1.2.2 - Bug fixes
- 1.1.0 - Implemented ArtSync
- 1.0.18 - Fixed bug in RdmPacketFactory
- 1.0.17 - Fixed bug in ArtNetReplyPacket
- 1.0.0 - Initial release
