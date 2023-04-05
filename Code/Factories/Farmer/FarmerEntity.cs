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
            
            bool rtn = (tile.TileType == ModContent.TileType<FarmerBlock>()) && tile.HasTile;

            return rtn;
        }

        protected override void Tick()
        {
            FarmerTick();
        }

        private FactoryErrorCode FarmerTick()
        {
            lastCode = ValidateSetup();

            if (lastCode != FactoryErrorCode.Success)
                return lastCode;

            FarmerRecipe recipe = null;
            recipe = possibleItems.Find(x => x.seed.type == inventory[0].type);

            string output = "Planting " + recipe.seed.Name + "\nGiving:\n";
            foreach (var item in recipe.rewards)
            {
                output += item.Name + "\n";
            }

            Console.WriteLine(output);
            return FactoryErrorCode.Success;
        }

        private FactoryErrorCode ValidateSetup()
        {
            if (inventory[0] == null || inventory[0].IsAir)
                return FactoryErrorCode.NoCraft;

            if (connectors == null || connectors.Count <= 0)
                {
                    if (connectors != null)
                        connectors.Clear();

                    FactoryErrorCode ferr = FactoryErrorCode.Success;
                    connectors = GetConnectingChest(out ferr);
                    if (ferr != FactoryErrorCode.Success)
                        return ferr;
                }

            List<Chest> inputs = connectors.Where(x => x.isInput).Where(x => x.chest != null).Select(x => x.chest).ToList();

            List<Chest> outputs = connectors.Where(x => x.isOutput).Where(x => x.chest != null).Select(x => x.chest).ToList();

            if (inputs.Count <= 0)
                return FactoryErrorCode.NoInputs;

            if (outputs.Count <= 0)
                return FactoryErrorCode.NoOutputs;

            var recipes = possibleItems.FindAll(x => x.seed.type == inventory[0].type);
            if (recipes == null || recipes.Count <= 0)
                return FactoryErrorCode.NoRecipe;

            int index = Main.rand.Next(recipes.Count);
            var recipe = recipes[index];

            foreach (var r in recipe.rewards)
            {
                AddToChest(r, outputs.ToArray());
            }

            return FactoryErrorCode.Success;
        }
    }
}
