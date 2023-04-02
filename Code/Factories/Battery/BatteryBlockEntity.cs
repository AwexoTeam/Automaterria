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

namespace Automaterria.Code.Factories.Battery
{
    public class BatteryBlockEntity : Factory
    {
        public override int inventorySpaces => 1;
        public override int tickDely => 150;

        public override FactoryType factoryType => FactoryType.Battery;

        public override bool IsTileValidForEntity(int i, int j)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            bool rtn = (tile.TileType == ModContent.TileType<BatteryBlock>()) && tile.HasTile;

            return rtn;
        }

        protected override void Tick()
        {
        }
    }
}
