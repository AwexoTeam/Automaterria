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

namespace Automaterria.Code.Factories.Farmer
{
    public class FarmerRecipe
    {
        public Item seed;
        public Item[] rewards;

        public FarmerRecipe(Item seed, params Item[] rewards)
        {
            this.seed = seed;
            this.rewards = rewards;
        }

        public FarmerRecipe(Item seed, params int[] idsNAmount)
        {
            if ((idsNAmount.Length % 2) != 0)
                return;

            int amount = (int)((float)idsNAmount.Length / 2f);
            rewards = new Item[amount];

            this.seed = seed;
            for (int i = 0; i < idsNAmount.Length; i+=2)
            {
                int type = idsNAmount[i];
                int stack = idsNAmount[i++];

                rewards[i] = new Item(type, stack);
            }
        }

        public static FarmerRecipe GetHerbBase(int seed, int herb, int stack = 1)
        {
            return new FarmerRecipe(new Item(seed), seed, 1, herb, stack);
        }
    }

    public class FarmerEntity : Factory
    {
        public static List<FarmerRecipe> possibleItems = new List<FarmerRecipe>
        {
            FarmerRecipe.GetHerbBase(ItemID.BlinkrootSeeds, ItemID.Blinkroot),
            FarmerRecipe.GetHerbBase(ItemID.DaybloomSeeds, ItemID.Daybloom),
            FarmerRecipe.GetHerbBase(ItemID.DeathweedSeeds, ItemID.Deathweed),
            FarmerRecipe.GetHerbBase(ItemID.FireblossomSeeds, ItemID.Fireblossom),
            FarmerRecipe.GetHerbBase(ItemID.MoonglowSeeds, ItemID.Moonglow),
            FarmerRecipe.GetHerbBase(ItemID.ShiverthornSeeds, ItemID.Shiverthorn),
            FarmerRecipe.GetHerbBase(ItemID.WaterleafSeeds, ItemID.Waterleaf),

            FarmerRecipe.GetHerbBase(ItemID.Acorn, ItemID.Wood, 20),
            FarmerRecipe.GetHerbBase(ItemID.Acorn, ItemID.RichMahogany, 20),
            FarmerRecipe.GetHerbBase(ItemID.Acorn, ItemID.Ebonwood, 20),
            FarmerRecipe.GetHerbBase(ItemID.Acorn, ItemID.Shadewood, 20),
            FarmerRecipe.GetHerbBase(ItemID.Acorn, ItemID.BorealWood, 20),
            FarmerRecipe.GetHerbBase(ItemID.Acorn, ItemID.Pearlwood, 20),

            FarmerRecipe.GetHerbBase(ItemID.GemTreeAmberSeed, ItemID.Amber, 2),
            FarmerRecipe.GetHerbBase(ItemID.GemTreeAmethystSeed, ItemID.Amethyst, 2),
            FarmerRecipe.GetHerbBase(ItemID.GemTreeDiamondSeed, ItemID.Diamond, 2),
            FarmerRecipe.GetHerbBase(ItemID.GemTreeEmeraldSeed, ItemID.Emerald, 2),
            FarmerRecipe.GetHerbBase(ItemID.GemTreeSapphireSeed, ItemID.Sapphire, 2),
            FarmerRecipe.GetHerbBase(ItemID.GemTreeTopazSeed, ItemID.Topaz, 2),
        };
        public override int inventorySpaces => 2;
        public override bool givesPower => true;
        public override int tickDely => 5000;
        public override FactoryType factoryType => FactoryType.Farmer;

        public override bool IsTileValidForEntity(int i, int j)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            
            bool rtn = (tile.TileType == ModContent.TileType<FarmerBlock>()) && tile.HasTile;

            return rtn;
        }

        protected override void Tick()
        {

        }
    }
}
