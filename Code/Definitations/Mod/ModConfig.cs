using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

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

        public int number { get; set; }
        public Dictionary<int, int> plantableIds { get; set; }
    }

    public static class GlobalConfig
    {

    }
}
