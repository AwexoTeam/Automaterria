using Automaterria.Code;
using Terraria;
using Terraria.ModLoader;
using Automaterria.Tiles;

namespace Automaterria.Entities
{
    public class SolarBlockEntity : Factory
    {
        public override int inventorySpaces => 0;
        public override bool givesPower => true;
        public override int tickDely => GlobalConfig.SolarPanelTickDely;

        public override FactoryType factoryType => FactoryType.SolarPanel;

        public const int solarPanelMaxPower = 100;

        public override bool IsTileValidForEntity(int i, int j)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            
            bool rtn = (tile.TileType == ModContent.TileType<SolarPanelTile>()) && tile.HasTile;

            return rtn;
        }

        protected override void Tick()
        {
            if (storedPower >= solarPanelMaxPower)
            {
                storedPower = solarPanelMaxPower;
                return;
            }

            storedPower += GlobalConfig.SolarPanelPower;
        }
    }
}
