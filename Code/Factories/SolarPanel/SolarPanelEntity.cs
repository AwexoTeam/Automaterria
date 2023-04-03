using Automaterria.Code;
using Automaterria.Code.Definitations.Factories;
using Automaterria.Code.Pipe;
using Automaterria.Code.Ui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Automaterria.Code.Factories.SolarPanel
{
    public class SolarBlockEntity : Factory
    {
        public override int inventorySpaces => 0;
        public override bool givesPower => true;
        public override int tickDely => 100;

        public override FactoryType factoryType => FactoryType.SolarPanel;

        public const int solarPanelPower = 10;
        public const int solarPanelMaxPower = 100;

        public override bool IsTileValidForEntity(int i, int j)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            
            bool rtn = (tile.TileType == ModContent.TileType<SolarPanelBlock>()) && tile.HasTile;

            return rtn;
        }

        protected override void Tick()
        {
            if (storedPower >= solarPanelMaxPower)
            {
                storedPower = solarPanelMaxPower;
                return;
            }

            storedPower += solarPanelPower;
        }
    }
}
