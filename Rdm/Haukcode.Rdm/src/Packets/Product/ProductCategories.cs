using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haukcode.Rdm.Packets.Product
{
    public enum ProductCategories
    {
        NotDeclared = 0x0000,
        Other = 0x7FFF,

        //Fixture
        Fixture = 0x0100,
        FixtureFixed = 0x0101,
        FixtureMovingYoke = 0x0102,
        FixtureMovingMirror = 0x0103,
        FixtureOther = 0x01FF,

        //Fixture Accessory
        FixtureAccessory = 0x0200,
        FixtureAccessoryColor = 0x0201,
        FixtureAccessoryYoke = 0x0202,
        FixtureAccessoryMirror = 0x0203,
        FixtureAccessoryEffect = 0x0204,
        FixtureAccessoryBeam = 0x0205,
        FixtureAccessoryOther = 0x02FF,

        //Projector
        Projector = 0x0300,
        ProjectorFixed = 0x0301,
        ProjectorMovingYoke = 0x0302,
        ProjectorMovingMirror = 0x0303,
        ProjectorOther = 0x03FF,

        //Atmospheric
        Atmospheric = 0x0400,
        AtmosphericEffect = 0x0401,
        AtmosphericPyro = 0x0402,
        AtmosphericOther = 0x04FF,

        //Dimmer
        Dimmer = 0x0500,
        DimmerAcIncandescent = 0x0501,
        DimmerAcFluorescent = 0x0502,
        DimmerAcColdCathode = 0x0503,
        DimmerAcNonDim = 0x0504,
        DimmerAcElv = 0x0505,
        DimmerDcOther = 0x0506,
        DimmerDcLevel = 0x0507,
        DimmerDcPwm = 0x0508,
        DimmerCsLed = 0x0509,
        DimmerOther = 0x05FF,

        //Power Control
        Power = 0x0600,
        PowerControl = 0x0601,
        PowerSource = 0x0602,
        PowerOther = 0x06FF,

        //Scenic
        Scenic = 0x0700,
        ScenicDrive = 0x0701,
        ScenicOther = 0x07FF,

        //Data
        Data = 0x0800,
        DataDistribution = 0x0801,
        DataConcersion = 0x0802,
        DataOther = 0x08FF,

        //Audio-Visual
        Av = 0x0900,
        AvAudio = 0x0901,
        AvVideo = 0x0902,
        AvOther = 0x09FF,

        //Monitoring Equipment
        Monitor = 0x0A00,
        MonitorACLinePower = 0x0A01,
        MonitorDCLinePower = 0x0A02,
        MonitorEnviromental = 0x0A03,
        MonitorOther = 0x0AFF,

        //Controller / Backup
        Control = 0x0700,
        ControlController = 0x0701,
        ControlBackupDevice = 0x0702,
        ControlOther = 0x07FF,

        //Test Equipment
        Test = 0x7100,
        TestEquipment = 0x7101,
        TestOther = 0x71FF
    }
}
