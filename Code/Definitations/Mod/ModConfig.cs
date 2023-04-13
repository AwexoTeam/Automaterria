using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Default;

namespace Automaterria.Code
{
    public class Config : ModConfig
    {
        public static Config config
        {
            get
            {
                return ModContent.GetInstance<Config>();
            }
        }

        public override ConfigScope Mode => ConfigScope.ServerSide;
        [Header("Machine Tick Settings")]
        [Range(1, 600000)] public int CrafterTickDely { get; set; }
        [Range(1, 600000)] public int FarmerTickDely { get; set; }
        [Range(1, 600000)] public int FuelBurnerTickDely { get; set; }
        [Range(1, 600000)] public int ItemPullerTickDely { get; set; }
        [Range(1, 600000)] public int SolarPanelTickDely { get; set; }
        [Range(1, 600000)] public int ThermalTickDely { get; set; }
        [Range(1, 600000)] public int QuarryTickDely { get; set; }


        [Header("Power Requirement Settings")]
        [Range(1, 1000)] public int CrafterPowerReq { get; set; }
        [Range(1, 1000)] public int FamerPowerReq { get; set; }
        [Range(1, 1000)] public int ItemPullerPowerReq { get; set; }
        [Range(1, 1000)] public int QuarryPowerReq { get; set; }

        
        [Header("Power Generation Settings")]
        [Range(1, 1000)] public int BatteryMax { get; set; }
        [Range(1, 1000)] public int SolarPanelPower { get; set; }
        [Range(1, 1000)] public int FuelPower { get; set; }
        [Range(1, 1000)] public int ThermalPower { get; set; }

        [Header("Item Collections")]
        public Dictionary<int, int> otherFuels { get; set; }

        [Header("Quarry Settings")]
        [Range(1, 100)] public int CommonLootWeight { get; set; }
        [Range(1, 100)] public int UncommonLootWeight { get; set; }
        [Range(1, 100)] public int RareLootWeight { get; set; }
        [Range(1, 100)] public int EpicLootWeight { get; set; }
        [Range(1, 100)] public int LegendaryLootWeight { get; set; }

    }

    public static class GlobalConfig
    {
        public static int CrafterTickDely => Config.config.CrafterTickDely;
        public static int FarmerTickDely => Config.config.FarmerTickDely;
        public static int FuelBurnerTickDely => Config.config.FuelBurnerTickDely;
        public static int ItemPullerTickDely => Config.config.ItemPullerTickDely;
        public static int SolarPanelTickDely => Config.config.SolarPanelTickDely;
        public static int ThermalTickDely => Config.config.ThermalTickDely;
        public static int QuarryTickDely => Config.config.QuarryTickDely;


        public static int CrafterPowerReq => Config.config.CrafterPowerReq;
        public static int FamerPowerReq => Config.config.FamerPowerReq;
        public static int ItemPullerPowerReq => Config.config.ItemPullerPowerReq;
        public  static int QuarryPowerReq => Config.config.QuarryPowerReq;


        public static int BatteryMax => Config.config.BatteryMax;
        public static int SolarPanelPower => Config.config.SolarPanelPower;
        public static int FuelPower => Config.config.FuelPower;
        public static int ThermalPower => Config.config.ThermalPower;

        public static int CommonLootWeight => Config.config.CommonLootWeight;
        public static int UncommonLootWeight => Config.config.UncommonLootWeight;
        public static int RareLootWeight => Config.config.RareLootWeight;
        public static int EpicLootWeight => Config.config.EpicLootWeight;
        public static int LegendaryLootWeight => Config.config.LegendaryLootWeight;
    }
}
