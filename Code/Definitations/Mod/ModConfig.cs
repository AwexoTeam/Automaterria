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
        public int CrafterTickDely { get; set; }
        public int FarmerTickDely { get; set; }
        public int FuelBurnerTickDely { get; set; }
        public int ItemPullerTickDely { get; set; }
        public int SolarPanelTickDely { get; set; }
        public int ThermalTickDely { get; set; }


        [Header("Power Requirement Settings")]
        public int CrafterPowerReq { get; set; }
        public int FamerPowerReq { get; set; }
        public int ItemPullerPowerReq { get; set; }
        public int QuarryPowerReq { get; set; }

        
        [Header("Power Generation Settings")]
        public int BatteryMax { get; set; }
        public int SolarPanelPower { get; set; }
        public int FuelPower { get; set; }
        public int ThermalPower { get; set; }

        [Header("Item Collections")]
        public Dictionary<int, int> otherFuels { get; set; }
        //TODO: otehr grow recipes.
    }

    public static class GlobalConfig
    {
        public static int CrafterTickDely => Config.config.CrafterTickDely;
        public static int FarmerTickDely => Config.config.FarmerTickDely;
        public static int FuelBurnerTickDely => Config.config.FuelBurnerTickDely;
        public static int ItemPullerTickDely => Config.config.ItemPullerTickDely;
        public static int SolarPanelTickDely => Config.config.SolarPanelTickDely;
        public static int ThermalTickDely => Config.config.ThermalTickDely;


        public static int CrafterPowerReq => Config.config.CrafterPowerReq;
        public static int FamerPowerReq => Config.config.FamerPowerReq;
        public static int ItemPullerPowerReq => Config.config.ItemPullerPowerReq;
        public  static int QuarryPowerReq => Config.config.QuarryPowerReq;


        public static int BatteryMax => Config.config.BatteryMax;
        public static int SolarPanelPower => Config.config.SolarPanelPower;
        public static int FuelPower => Config.config.FuelPower;
        public static int ThermalPower => Config.config.ThermalPower;
    }
}
