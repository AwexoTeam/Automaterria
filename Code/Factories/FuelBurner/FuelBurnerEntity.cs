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
        public static Dictionary<int, int> validFuels = new Dictionary<int, int>();

        public override int tickDely => GlobalConfig.FuelBurnerTickDely;

        public override FactoryType factoryType => FactoryType.FuelBurner;

        public override int inventorySpaces => 1;
        public override bool givesPower => true;

        public override bool IsTileValidForEntity(int i, int j)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            bool rtn = (tile.TileType == ModContent.TileType<FuelBurnerBlock>()) && tile.HasTile;

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
