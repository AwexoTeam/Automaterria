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

namespace Automaterria.Code.Factories.FuelBurner
{
    public class FuelBurnerEntity : Factory
    {
        public override int tickDely => 150;

        public override FactoryType factoryType => FactoryType.FuelBurner;

        public override int inventorySpaces => 1;

        public override bool IsTileValidForEntity(int i, int j)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            bool rtn = (tile.TileType == ModContent.TileType<FuelBurnerBlock>()) && tile.HasTile;

            return rtn;
        }


        protected override void Tick()
        {
        }
    }
}
