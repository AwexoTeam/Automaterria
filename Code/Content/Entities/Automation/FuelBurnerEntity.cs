using Automaterria.Code;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Automaterria.Tiles;

namespace Automaterria.Entities
{
    public class FuelBurnerEntity : Factory
    {
        public static Dictionary<int, int> validFuels = new Dictionary<int, int>();

        public override int tickDely => GlobalConfig.FuelBurnerTickDely;

        public override FactoryType factoryType => FactoryType.FuelBurner;

        public override int inventorySpaces => 1;
        public override bool givesPower => true;

        public override bool IsTileValidForEntity(int i, int j)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            bool rtn = (tile.TileType == ModContent.TileType<FuelBurnerTile>()) && tile.HasTile;

            return rtn;
        }


        protected override void Tick()
        {
            if (inventory == null)
                return;

            Item fuel = inventory[0];
            if (fuel == null || fuel.IsAir)
                return;

            if (!validFuels.ContainsKey(fuel.type))
                return;

            fuel.stack--;
            if (fuel.stack <= 0)
                fuel.TurnToAir();

            storedPower = validFuels[fuel.type];
        }
    }
}
