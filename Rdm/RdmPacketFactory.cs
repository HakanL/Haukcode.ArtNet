using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Haukcode.Rdm.Packets.Net;
using Haukcode.Rdm.Packets.DMX;
using Haukcode.Rdm.Packets.Product;
using Haukcode.Rdm.Packets.Parameters;
using Haukcode.Rdm.Packets.Control;
using Haukcode.Rdm.Packets.Status;
using Haukcode.Rdm.Packets.Power;
using Haukcode.Rdm.Packets.Management;
using Haukcode.Rdm.Packets.Discovery;
using Haukcode.Rdm.Packets.Configuration;

namespace Haukcode.Rdm
{
    public static class RdmPacketFactory
    {
        static RdmPacketFactory()
        {
            RegisterDiscoveryMessages();
            RegisterStatusMessages();
            RegisterCoreMessages();
            RegisterRdmNetMessages();
            RegisterProductMessages();
            RegisterDmxMessages();
            RegisterDimmerMessages();
            RegisterPowerMessages();
            RegisterConfigurationMessages();
            RegisterControlMessages();
        }

        private static void RegisterDiscoveryMessages()
        {
            RegisterPacketType(RdmCommands.Discovery, RdmParameters.DiscoveryUniqueBranch, typeof(DiscoveryUniqueBranch.Request));
            RegisterPacketType(RdmCommands.DiscoveryResponse, RdmParameters.DiscoveryUniqueBranch, typeof(DiscoveryUniqueBranch.Reply));

            RegisterPacketType(RdmCommands.Discovery, RdmParameters.DiscoveryMute, typeof(DiscoveryMute.Request));
            RegisterPacketType(RdmCommands.DiscoveryResponse, RdmParameters.DiscoveryMute, typeof(DiscoveryMute.Reply));

            RegisterPacketType(RdmCommands.Discovery, RdmParameters.DiscoveryUnMute, typeof(DiscoveryMute.Request));
            RegisterPacketType(RdmCommands.DiscoveryResponse, RdmParameters.DiscoveryUnMute, typeof(DiscoveryMute.Reply));

        }

        private static void RegisterStatusMessages()
        {
            //QueuedMessage
            RegisterPacketType(RdmCommands.Get, RdmParameters.QueuedMessage, typeof(QueuedMessage.Get));

            //StatusMessage
            RegisterPacketType(RdmCommands.Get, RdmParameters.StatusMessage, typeof(StatusMessage.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.StatusMessage, typeof(StatusMessage.GetReply));

            //ClearStatusId
            RegisterPacketType(RdmCommands.Set, RdmParameters.ClearStatusId, typeof(ClearStatusId.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.ClearStatusId, typeof(ClearStatusId.SetReply));
        }

        private static void RegisterCoreMessages()
        {
            //SupportedParameters
            RegisterPacketType(RdmCommands.Get, RdmParameters.SupportedParameters, typeof(SupportedParameters.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.SupportedParameters, typeof(SupportedParameters.GetReply));

            //ParameterDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.ParameterDescription, typeof(ParameterDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.ParameterDescription, typeof(ParameterDescription.GetReply));
        }



        private static void RegisterRdmNetMessages()
        {
            //Endpoint List
            RegisterPacketType(RdmCommands.Get, RdmParameters.EndpointList, typeof(EndpointList.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.EndpointList, typeof(EndpointList.Reply));

            //EndpointListChange
            RegisterPacketType(RdmCommands.Get, RdmParameters.EndpointListChange, typeof(EndpointListChange.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.EndpointListChange, typeof(EndpointListChange.Reply));

            //EndpointIdentify
            RegisterPacketType(RdmCommands.Get, RdmParameters.EndpointIdentify, typeof(EndpointIdentify.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.EndpointIdentify, typeof(EndpointIdentify.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.EndpointIdentify, typeof(EndpointIdentify.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.EndpointIdentify, typeof(EndpointIdentify.SetReply));

            //EndpointToUniverse
            RegisterPacketType(RdmCommands.Get, RdmParameters.EndpointToUniverse, typeof(EndpointToUniverse.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.EndpointToUniverse, typeof(EndpointToUniverse.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.EndpointToUniverse, typeof(EndpointToUniverse.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.EndpointToUniverse, typeof(EndpointToUniverse.SetReply));
            
            //RdmTrafficEnable
            RegisterPacketType(RdmCommands.Get, RdmParameters.RdmTrafficEnable, typeof(RdmTrafficEnable.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.RdmTrafficEnable, typeof(BackgroundDiscovery.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.RdmTrafficEnable, typeof(RdmTrafficEnable.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.RdmTrafficEnable, typeof(RdmTrafficEnable.SetReply));

            //EndpointMode
            RegisterPacketType(RdmCommands.Get, RdmParameters.EndpointMode, typeof(EndpointMode.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.EndpointMode, typeof(EndpointMode.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.EndpointMode, typeof(EndpointMode.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.EndpointMode, typeof(EndpointMode.SetReply));

            //EndpointLabel
            RegisterPacketType(RdmCommands.Get, RdmParameters.EndpointLabel, typeof(EndpointLabel.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.EndpointLabel, typeof(EndpointLabel.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.EndpointLabel, typeof(EndpointLabel.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.EndpointLabel, typeof(EndpointLabel.SetReply));

            //DiscoveryState
            RegisterPacketType(RdmCommands.Get, RdmParameters.DiscoveryState, typeof(DiscoveryState.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DiscoveryState, typeof(DiscoveryState.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.DiscoveryState, typeof(DiscoveryState.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.DiscoveryState, typeof(DiscoveryState.SetReply));

            //BackgroundDiscovery
            RegisterPacketType(RdmCommands.Get, RdmParameters.BackgroundDiscovery, typeof(BackgroundDiscovery.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.BackgroundDiscovery, typeof(BackgroundDiscovery.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.BackgroundDiscovery, typeof(BackgroundDiscovery.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.BackgroundDiscovery, typeof(BackgroundDiscovery.SetReply));

            //EndpointTiming
            RegisterPacketType(RdmCommands.Get, RdmParameters.EndpointTiming, typeof(EndpointTiming.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.EndpointTiming, typeof(EndpointTiming.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.EndpointTiming, typeof(EndpointTiming.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.EndpointTiming, typeof(EndpointTiming.SetReply));

            //EndpointTimingDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.EndpointTimingDescription, typeof(EndpointTimingDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.EndpointTimingDescription, typeof(EndpointTimingDescription.GetReply));

            //EndpointDeviceListChange
            RegisterPacketType(RdmCommands.Get, RdmParameters.EndpointDeviceListChange, typeof(EndpointDeviceListChange.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.EndpointDeviceListChange, typeof(EndpointDeviceListChange.Reply));

            //Endpoint Devices
            RegisterPacketType(RdmCommands.Get, RdmParameters.EndpointDevices, typeof(EndpointDevices.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.EndpointDevices, typeof(EndpointDevices.Reply));

            //BindingControlFields
            //TcpCommsStatus
            //BackgroundQueuedStatusPolicy
            //BackgroundQueuedStatusPolicyDescription
            //BackgroundStatusType
            //QueuedStatusEndpointCollection
            //QueuedStatusUIDCollection
        }

        private static void RegisterProductMessages()
        {
            //DeviceInfo
            RegisterPacketType(RdmCommands.Get, RdmParameters.DeviceInfo, typeof(DeviceInfo.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DeviceInfo, typeof(DeviceInfo.GetReply));

            //ProductDetailIdList
            RegisterPacketType(RdmCommands.Get, RdmParameters.ProductDetailIdList, typeof(ProductDetailIdList.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.ProductDetailIdList, typeof(ProductDetailIdList.GetReply));

            //DeviceModelDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.DeviceModelDescription, typeof(DeviceModelDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DeviceModelDescription, typeof(DeviceModelDescription.GetReply));

            //ManufacturerLabel
            RegisterPacketType(RdmCommands.Get, RdmParameters.ManufacturerLabel, typeof(ManufacturerLabel.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.ManufacturerLabel, typeof(ManufacturerLabel.GetReply));

            //DeviceLabel
            RegisterPacketType(RdmCommands.Get, RdmParameters.DeviceLabel, typeof(DeviceLabel.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DeviceLabel, typeof(DeviceLabel.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.DeviceLabel, typeof(DeviceLabel.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.DeviceLabel, typeof(DeviceLabel.SetReply));

            //FactoryDefaults
            RegisterPacketType(RdmCommands.Get, RdmParameters.FactoryDefaults, typeof(FactoryDefaults.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.FactoryDefaults, typeof(FactoryDefaults.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.FactoryDefaults, typeof(FactoryDefaults.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.FactoryDefaults, typeof(FactoryDefaults.SetReply));

            //LanguageCapabilities
            RegisterPacketType(RdmCommands.Get, RdmParameters.LanguageCapabilities, typeof(LanguageCapabilities.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.LanguageCapabilities, typeof(LanguageCapabilities.GetReply));

            //Language
            RegisterPacketType(RdmCommands.Get, RdmParameters.Language, typeof(Language.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.Language, typeof(Language.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.Language, typeof(Language.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.Language, typeof(Language.SetReply));

            //SoftwareVersionLabel
            RegisterPacketType(RdmCommands.Get, RdmParameters.SoftwareVersionLabel, typeof(SoftwareVersionLabel.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.SoftwareVersionLabel, typeof(SoftwareVersionLabel.GetReply));

            //BootSoftwareVersionId
            RegisterPacketType(RdmCommands.Get, RdmParameters.BootSoftwareVersionId, typeof(BootSoftwareVersionId.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.BootSoftwareVersionId, typeof(BootSoftwareVersionId.GetReply));

            //SoftwareVersionLabel
            RegisterPacketType(RdmCommands.Get, RdmParameters.BootSoftwareVersionLabel, typeof(BootSoftwareVersionLabel.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.BootSoftwareVersionLabel, typeof(BootSoftwareVersionLabel.GetReply));
        }

        private static void RegisterPowerMessages()
        {
            //DeviceHours
            RegisterPacketType(RdmCommands.Get, RdmParameters.DeviceHours, typeof(DeviceHours.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DeviceHours, typeof(DeviceHours.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.DeviceHours, typeof(DeviceHours.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.DeviceHours, typeof(DeviceHours.SetReply));

            //LampHours
            RegisterPacketType(RdmCommands.Get, RdmParameters.LampHours, typeof(LampHours.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.LampHours, typeof(LampHours.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.LampHours, typeof(LampHours.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.LampHours, typeof(LampHours.SetReply));

            //LampStrikes
            RegisterPacketType(RdmCommands.Get, RdmParameters.LampStrikes, typeof(LampStrikes.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.LampStrikes, typeof(LampStrikes.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.LampStrikes, typeof(LampStrikes.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.LampStrikes, typeof(LampStrikes.SetReply));

            //LampState
            RegisterPacketType(RdmCommands.Get, RdmParameters.LampState, typeof(LampState.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.LampState, typeof(LampState.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.LampState, typeof(LampState.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.LampState, typeof(LampState.SetReply));

            //LampOnMode
            RegisterPacketType(RdmCommands.Get, RdmParameters.LampOnMode, typeof(LampOnMode.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.LampOnMode, typeof(LampOnMode.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.LampOnMode, typeof(LampOnMode.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.LampOnMode, typeof(LampOnMode.SetReply));

            //DevicePowerCycles
            RegisterPacketType(RdmCommands.Get, RdmParameters.DevicePowerCycles, typeof(DevicePowerCycles.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DevicePowerCycles, typeof(DevicePowerCycles.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.DevicePowerCycles, typeof(DevicePowerCycles.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.DevicePowerCycles, typeof(DevicePowerCycles.SetReply));

            //BurnIn
            RegisterPacketType(RdmCommands.Get, RdmParameters.BurnIn, typeof(BurnIn.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.BurnIn, typeof(BurnIn.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.BurnIn, typeof(BurnIn.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.BurnIn, typeof(BurnIn.SetReply));
        }

        private static void RegisterDmxMessages()
        {
            //DmxPersonality
            RegisterPacketType(RdmCommands.Get, RdmParameters.DmxPersonality, typeof(DmxPersonality.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DmxPersonality, typeof(DmxPersonality.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.DmxPersonality, typeof(DmxPersonality.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.DmxPersonality, typeof(DmxPersonality.SetReply));

            //DmxPersonalityDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.DmxPersonalityDescription, typeof(DmxPersonalityDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DmxPersonalityDescription, typeof(DmxPersonalityDescription.GetReply));

            //DmxStartAddress
            RegisterPacketType(RdmCommands.Get, RdmParameters.DmxStartAddress, typeof(DmxStartAddress.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DmxStartAddress, typeof(DmxStartAddress.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.DmxStartAddress, typeof(DmxStartAddress.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.DmxStartAddress, typeof(DmxStartAddress.SetReply));

            //DmxBlockAddress
            RegisterPacketType(RdmCommands.Get, RdmParameters.DmxBlockAddress, typeof(DmxBlockAddress.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DmxBlockAddress, typeof(DmxBlockAddress.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.DmxBlockAddress, typeof(DmxBlockAddress.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.DmxBlockAddress, typeof(DmxBlockAddress.SetReply));

            //SlotInfo
            RegisterPacketType(RdmCommands.Get, RdmParameters.SlotInfo, typeof(SlotInfo.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.SlotInfo, typeof(SlotInfo.GetReply));

            //SlotDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.SlotDescription, typeof(SlotDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.SlotDescription, typeof(SlotDescription.GetReply));

            //DefaultSlotValue
            RegisterPacketType(RdmCommands.Get, RdmParameters.DefaultSlotValue, typeof(DefaultSlotValue.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DefaultSlotValue, typeof(DefaultSlotValue.GetReply));

            //DmxStartupMode
            RegisterPacketType(RdmCommands.Get, RdmParameters.DmxStartupMode, typeof(DmxStartupMode.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DmxStartupMode, typeof(DmxStartupMode.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.DmxStartupMode, typeof(DmxStartupMode.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.DmxStartupMode, typeof(DmxStartupMode.SetReply));
        }

        private static void RegisterDimmerMessages()
        {
            //Curve
            RegisterPacketType(RdmCommands.Get, RdmParameters.Curve, typeof(Curve.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.Curve, typeof(Curve.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.Curve, typeof(Curve.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.Curve, typeof(Curve.SetReply));

            //CurveDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.CurveDescription, typeof(CurveDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.CurveDescription, typeof(CurveDescription.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.CurveDescription, typeof(CurveDescription.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.CurveDescription, typeof(CurveDescription.SetReply));

            //DimmerInfo
            RegisterPacketType(RdmCommands.Get, RdmParameters.DimmerInfo, typeof(DimmerInfo.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.DimmerInfo, typeof(DimmerInfo.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.DimmerInfo, typeof(DimmerInfo.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.DimmerInfo, typeof(DimmerInfo.SetReply));

            //MaximumLevel
            RegisterPacketType(RdmCommands.Get, RdmParameters.MaximumLevel, typeof(MaximumLevel.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.MaximumLevel, typeof(MaximumLevel.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.MaximumLevel, typeof(MaximumLevel.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.MaximumLevel, typeof(MaximumLevel.SetReply));

            //MinimumLevel
            RegisterPacketType(RdmCommands.Get, RdmParameters.MinimumLevel, typeof(MinimumLevel.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.MinimumLevel, typeof(MinimumLevel.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.MinimumLevel, typeof(MinimumLevel.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.MinimumLevel, typeof(MinimumLevel.SetReply));

            //ModulationFrequency
            RegisterPacketType(RdmCommands.Get, RdmParameters.ModulationFrequency, typeof(ModulationFrequency.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.ModulationFrequency, typeof(ModulationFrequency.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.ModulationFrequency, typeof(ModulationFrequency.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.ModulationFrequency, typeof(ModulationFrequency.SetReply));

            //ModulationFrequencyDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.ModulationFrequencyDescription, typeof(ModulationFrequencyDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.ModulationFrequencyDescription, typeof(ModulationFrequencyDescription.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.ModulationFrequencyDescription, typeof(ModulationFrequencyDescription.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.ModulationFrequencyDescription, typeof(ModulationFrequencyDescription.SetReply));

            //OutputResponseTime
            RegisterPacketType(RdmCommands.Get, RdmParameters.OutputResponseTime, typeof(OutputResponseTime.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.OutputResponseTime, typeof(OutputResponseTime.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.OutputResponseTime, typeof(OutputResponseTime.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.OutputResponseTime, typeof(OutputResponseTime.SetReply));

            //OutputResponseTimeDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.OutputResponseTimeDescription, typeof(OutputResponseTimeDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.OutputResponseTimeDescription, typeof(OutputResponseTimeDescription.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.OutputResponseTimeDescription, typeof(OutputResponseTimeDescription.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.OutputResponseTimeDescription, typeof(OutputResponseTimeDescription.SetReply));
        }

        private static void RegisterControlMessages()
        {
            //IdentifyDevice
            RegisterPacketType(RdmCommands.Get, RdmParameters.IdentifyDevice, typeof(IdentifyDevice.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.IdentifyDevice, typeof(IdentifyDevice.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.IdentifyDevice, typeof(IdentifyDevice.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.IdentifyDevice, typeof(IdentifyDevice.SetReply));

            //IdentifyMode
            RegisterPacketType(RdmCommands.Get, RdmParameters.IdentifyMode, typeof(IdentifyMode.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.IdentifyMode, typeof(IdentifyMode.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.IdentifyMode, typeof(IdentifyMode.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.IdentifyMode, typeof(IdentifyMode.SetReply));

            //ResetDevice
            RegisterPacketType(RdmCommands.Set, RdmParameters.ResetDevice, typeof(ResetDevice.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.ResetDevice, typeof(ResetDevice.SetReply));

            //PowerState
            RegisterPacketType(RdmCommands.Get, RdmParameters.PowerState, typeof(PowerState.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PowerState, typeof(PowerState.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.PowerState, typeof(PowerState.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.PowerState, typeof(PowerState.SetReply));

            //PerformSelfTest
            RegisterPacketType(RdmCommands.Get, RdmParameters.PerformSelfTest, typeof(PerformSelfTest.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PerformSelfTest, typeof(PerformSelfTest.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.PerformSelfTest, typeof(PerformSelfTest.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.PerformSelfTest, typeof(PerformSelfTest.SetReply));

            //PowerOnSelfTest
            RegisterPacketType(RdmCommands.Get, RdmParameters.PowerOnSelfTest, typeof(PowerOnSelfTest.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PowerOnSelfTest, typeof(PowerOnSelfTest.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.PowerOnSelfTest, typeof(PowerOnSelfTest.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.PowerOnSelfTest, typeof(PowerOnSelfTest.SetReply));

            //SelfTestDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.SelfTestDescription, typeof(SelfTestDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.SelfTestDescription, typeof(SelfTestDescription.GetReply));

            //CapturePreset
            RegisterPacketType(RdmCommands.Set, RdmParameters.CapturePreset, typeof(CapturePreset.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.CapturePreset, typeof(CapturePreset.SetReply));

            //PresetPlayback
            RegisterPacketType(RdmCommands.Get, RdmParameters.PresetPlayback, typeof(PresetPlayback.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PresetPlayback, typeof(PresetPlayback.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.PresetPlayback, typeof(PresetPlayback.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.PresetPlayback, typeof(PresetPlayback.SetReply));

            //Preset Info
            RegisterPacketType(RdmCommands.Get, RdmParameters.PresetInfo, typeof(PresetInfo.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PresetInfo, typeof(PresetInfo.GetReply));

            //Preset Status
            RegisterPacketType(RdmCommands.Get, RdmParameters.PresetStatus, typeof(PresetStatus.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PresetStatus, typeof(PresetStatus.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.PresetStatus, typeof(PresetStatus.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.PresetStatus, typeof(PresetStatus.SetReply));

            //Preset Merge Mode
            RegisterPacketType(RdmCommands.Get, RdmParameters.PresetMergeMode, typeof(PresetMergeMode.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PresetMergeMode, typeof(PresetMergeMode.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.PresetMergeMode, typeof(PresetMergeMode.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.PresetMergeMode, typeof(PresetMergeMode.SetReply));
        }

        private static void RegisterConfigurationMessages()
        {
            //LockState
            RegisterPacketType(RdmCommands.Get, RdmParameters.LockState, typeof(LockState.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.LockState, typeof(LockState.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.LockState, typeof(LockState.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.LockState, typeof(LockState.SetReply));

            //LockPin
            RegisterPacketType(RdmCommands.Get, RdmParameters.LockPin, typeof(LockPin.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.LockPin, typeof(LockPin.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.LockPin, typeof(LockPin.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.LockPin, typeof(LockPin.SetReply));

            //LockStateDescription
            RegisterPacketType(RdmCommands.Get, RdmParameters.LockStateDescription, typeof(LockStateDescription.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.LockStateDescription, typeof(LockStateDescription.GetReply));

            //PanInvert
            RegisterPacketType(RdmCommands.Get, RdmParameters.PanInvert, typeof(PanInvert.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PanInvert, typeof(PanInvert.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.PanInvert, typeof(PanInvert.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.PanInvert, typeof(PanInvert.SetReply));

            //TiltInvert
            RegisterPacketType(RdmCommands.Get, RdmParameters.TiltInvert, typeof(TiltInvert.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.TiltInvert, typeof(TiltInvert.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.TiltInvert, typeof(TiltInvert.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.TiltInvert, typeof(TiltInvert.SetReply));

            //PanTiltSwap
            RegisterPacketType(RdmCommands.Get, RdmParameters.PanTiltSwap, typeof(PanTiltSwap.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.PanTiltSwap, typeof(PanTiltSwap.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.PanTiltSwap, typeof(PanTiltSwap.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.PanTiltSwap, typeof(PanTiltSwap.SetReply));

            //RealTimeClock
            RegisterPacketType(RdmCommands.Get, RdmParameters.RealTimeClock, typeof(RealTimeClock.Get));
            RegisterPacketType(RdmCommands.GetResponse, RdmParameters.RealTimeClock, typeof(RealTimeClock.GetReply));
            RegisterPacketType(RdmCommands.Set, RdmParameters.RealTimeClock, typeof(RealTimeClock.Set));
            RegisterPacketType(RdmCommands.SetResponse, RdmParameters.RealTimeClock, typeof(RealTimeClock.SetReply));
        }

        private struct PacketKey
        {
            public PacketKey(RdmCommands command, RdmParameters parameter)
            {
                this.Command = command;
                this.Parameter = parameter;
            }

            public RdmCommands Command;
            public RdmParameters Parameter;
        }

        private static Dictionary<PacketKey, Type> packetStore = new Dictionary<PacketKey, Type>();

        public static void RegisterPacketType(RdmCommands command, RdmParameters parameter, Type packetType)
        {
            

            PacketKey key = new PacketKey();
            key.Command = command;
            key.Parameter = parameter;

            if(packetStore.ContainsKey(key))
                throw new InvalidOperationException(string.Format("The packet {0} is already registered for {1}.",parameter.ToString(),command.ToString()));

            packetStore[key] = packetType;
        }

        public static RdmPacket Build(RdmHeader header)
        {
            if (IsErrorResponse(header))
            {
                //Error Response Packets
                return BuildErrorResponse(header);
            }
            else
            {
                Type packetType;
                if (packetStore.TryGetValue(new PacketKey(header.Command, header.ParameterId), out packetType))
                {
                    return RdmPacket.Create(header, packetType);
                }
            }

            return null;
        }

        public static bool IsErrorResponse(RdmHeader header)
        {
            if (!IsResponse(header))
                return false;

            switch ((RdmResponseTypes)header.PortOrResponseType)
            {
                case RdmResponseTypes.Ack:
                case RdmResponseTypes.AckOverflow:
                    return false;
            }

            return true;
        }

        public static bool IsResponse(RdmHeader header)
        {
            return header.Command == RdmCommands.GetResponse || header.Command != RdmCommands.SetResponse;
        }

        public static RdmPacket BuildErrorResponse(RdmHeader header)
        {
            if (header.PortOrResponseType != 0)
            {
                switch ((RdmResponseTypes)header.PortOrResponseType)
                {
                    case RdmResponseTypes.AckTimer:
                        return RdmPacket.Create(header, typeof(RdmAckTimer));
                    case RdmResponseTypes.NackReason:
                        return RdmPacket.Create(header, typeof(RdmNack));
                }
            }

            return null;
        }

    }
}
