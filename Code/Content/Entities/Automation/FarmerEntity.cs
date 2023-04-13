using Automaterria.Code;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Automaterria.Tiles;

namespace Automaterria.Entities
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

        public static FarmerRecipe GetHerbBase(int seed, int herb, int stack = 1)
        {
            Item item_seed = new Item(seed, 1);
            Item item_herb = new Item(herb, stack);

            return new FarmerRecipe(item_seed, item_seed, item_herb);
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

        public override int requiredPower => GlobalConfig.FamerPowerReq;
        public override int inventorySpaces => 1;
        public override bool givesPower => true;
        public override int tickDely => GlobalConfig.FarmerTickDely;
        public override FactoryType factoryType => FactoryType.Farmer;
        public FactoryErrorCode lastCode;

        public override bool IsTileValidForEntity(int i, int j)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            
            bool rtn = (tile.TileType == ModContent.TileType<FarmerTile>()) && tile.HasTile;

            return rtn;
        }

        protected override void Tick()
        {
            BasicSetupValidation();
            FarmerRecipe recipe = null;
            lastError = FarmerTick(out recipe);
            if (lastError != FactoryErrorCode.Success)
                return;
                
            TakePower(requiredPower);

            var outputs = connectors
                .Where(x => x.isOutput)
                .Where(x => x.chest != null)
                .Select(x => x.chest);

            foreach (var r in recipe.rewards)
            {
                AddToChest(r, outputs.ToArray());
            }
        }

        private FactoryErrorCode FarmerTick(out FarmerRecipe recipe)
        {
            recipe = null;

            var recipes = possibleItems.FindAll(x => x.seed.type == inventory[0].type);
            if (recipes == null || recipes.Count <= 0)
                return FactoryErrorCode.NoRecipe;
            
            int index = Main.rand.Next(recipes.Count);
            recipe = recipes[index];

            return FactoryErrorCode.Success;
        }

    }
}
