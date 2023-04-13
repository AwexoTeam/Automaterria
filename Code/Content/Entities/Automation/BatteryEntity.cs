using Automaterria.Code;
using Terraria;
using Terraria.ModLoader;
using Automaterria.Tiles;

namespace Automaterria.Entities
{
    public class BatteryEntity : Factory
    {
        public override int inventorySpaces => 1;
        public override int tickDely => 150;

        public override FactoryType factoryType => FactoryType.Battery;

        public override bool IsTileValidForEntity(int i, int j)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            bool rtn = (tile.TileType == ModContent.TileType<BatteryTile>()) && tile.HasTile;

            return rtn;
        }

        protected override void Tick()
        {
        }
    }
}
