using Automaterria.Code;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Automaterria.Tiles;

namespace Automaterria.Entities
{
    public enum QuarryRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
    }

    public class QuarryLoot
    {
        public int reward;
        public QuarryRarity rarity;
        public int pickaxePower;

        public QuarryLoot(int reward, QuarryRarity rarity, int pickaxePower)
        {
            this.reward = reward;
            this.rarity = rarity;
            this.pickaxePower = pickaxePower;
        }
    }

    public class QuarryEntity : Factory
    {
        public static List<QuarryLoot> possibleOutcomes = new List<QuarryLoot>()
        {
            new QuarryLoot(ItemID.StoneBlock, QuarryRarity.Common, 35),
            new QuarryLoot(ItemID.SandBlock, QuarryRarity.Common, 35),
            new QuarryLoot(ItemID.MudBlock, QuarryRarity.Common, 35),
            new QuarryLoot(ItemID.ClayBlock, QuarryRarity.Common, 35),
            new QuarryLoot(ItemID.DirtBlock, QuarryRarity.Common, 35),
            new QuarryLoot(ItemID.SnowBlock, QuarryRarity.Common, 35),

            new QuarryLoot(ItemID.CopperOre, QuarryRarity.Uncommon, 35),
            new QuarryLoot(ItemID.TinOre, QuarryRarity.Uncommon, 35),
            new QuarryLoot(ItemID.IronOre, QuarryRarity.Uncommon, 35),
            new QuarryLoot(ItemID.LeadOre, QuarryRarity.Uncommon, 35),
            new QuarryLoot(ItemID.TungstenOre, QuarryRarity.Uncommon, 35),
            new QuarryLoot(ItemID.SilverOre, QuarryRarity.Uncommon, 35),

            new QuarryLoot(ItemID.GoldOre, QuarryRarity.Rare, 35),
            new QuarryLoot(ItemID.PlatinumOre, QuarryRarity.Rare, 35),
            new QuarryLoot(ItemID.CrimtaneOre, QuarryRarity.Rare, 55),
            new QuarryLoot(ItemID.DemoniteOre, QuarryRarity.Rare, 55),
            new QuarryLoot(ItemID.Obsidian, QuarryRarity.Rare, 55),

            new QuarryLoot(ItemID.Hellstone, QuarryRarity.Epic, 65),
            new QuarryLoot(ItemID.CobaltOre, QuarryRarity.Epic, 100),
            new QuarryLoot(ItemID.PalladiumOre, QuarryRarity.Epic, 100),
            new QuarryLoot(ItemID.MythrilOre, QuarryRarity.Epic, 110),
            new QuarryLoot(ItemID.OrichalcumOre, QuarryRarity.Epic, 110),

            new QuarryLoot(ItemID.AdamantiteOre, QuarryRarity.Legendary, 150),
            new QuarryLoot(ItemID.TitaniumOre, QuarryRarity.Legendary, 150),
            new QuarryLoot(ItemID.ChlorophyteOre, QuarryRarity.Legendary, 200),
        };

        public override int tickDely => GlobalConfig.QuarryTickDely;
        public override int requiredPower => GlobalConfig.QuarryPowerReq;

        public override FactoryType factoryType => FactoryType.Quarry;

        public override int inventorySpaces => 1;
        public override bool givesPower => false;

        public override bool IsTileValidForEntity(int i, int j)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            bool rtn = (tile.TileType == ModContent.TileType<QuarryTile>()) && tile.HasTile;

            return rtn;
        }

        protected override void Tick()
        {
            lastError = ValidateSetup();
            if (lastError != FactoryErrorCode.Success)
                return;

            Item pickaxe = inventory[0];
            int pickAxePower = pickaxe.pick;
            var possibleItems = possibleOutcomes.FindAll(x => x.pickaxePower <= pickAxePower);

            QuarryRarity rarity = GetRandomWeightedRarity();
            possibleItems.RemoveAll(x => x.rarity != rarity);

            if(possibleItems.Count <= 0)
            {
                possibleItems = possibleOutcomes.FindAll(x => x.pickaxePower <= pickAxePower);
                possibleItems.RemoveAll(x => x.rarity != QuarryRarity.Common);
            }

            if(possibleItems.Count <= 0)
            {
                lastError = FactoryErrorCode.UnknownError;
                return;
            }

            TakePower(requiredPower);

            int index = Main.rand.Next(possibleItems.Count);
            int rewardId = possibleItems[index].reward;

            var chests = connectors
                .Where(x => x.isOutput)
                .Where(x => x.chest != null)
                .Select(x => x.chest)
                .ToArray();

            Item reward = new Item(rewardId, 1, 0);

            AddToChest(reward, chests);
        }

        private FactoryErrorCode ValidateSetup()
        {
            this.lastError = BasicSetupValidation(false, true);

            if (lastError != FactoryErrorCode.Success)
                return lastError;

            if (inventory == null)
                return FactoryErrorCode.NotCorrectStation;

            Item pickaxe = inventory[0];
            if (pickaxe == null || pickaxe.IsAir)
                return FactoryErrorCode.NotCorrectStation;

            int pickAxePower = pickaxe.pick;
            var possibleItems = possibleOutcomes.FindAll(x => x.pickaxePower <= pickAxePower);
            if (possibleItems.Count <= 0)
                return FactoryErrorCode.NoCraft;

            return FactoryErrorCode.Success;
        }

        private QuarryRarity GetRandomWeightedRarity()
        {
            int weightedSum = 0;
            weightedSum += GlobalConfig.CommonLootWeight;
            weightedSum += GlobalConfig.UncommonLootWeight;
            weightedSum += GlobalConfig.RareLootWeight;
            weightedSum += GlobalConfig.EpicLootWeight;
            weightedSum += GlobalConfig.LegendaryLootWeight;

            int num = Main.rand.Next(0, weightedSum);
            int min = 0;
            int max = 0;

            if (DoCheck(num, GlobalConfig.CommonLootWeight, ref min, ref max))
                return QuarryRarity.Common;

            if (DoCheck(num, GlobalConfig.UncommonLootWeight, ref min, ref max))
                return QuarryRarity.Uncommon;

            if (DoCheck(num, GlobalConfig.RareLootWeight, ref min, ref max))
                return QuarryRarity.Rare;

            if (DoCheck(num, GlobalConfig.EpicLootWeight, ref min, ref max))
                return QuarryRarity.Epic;

            if (DoCheck(num, GlobalConfig.LegendaryLootWeight, ref min, ref max))
                return QuarryRarity.Legendary;

            return QuarryRarity.Common;
        }

        private bool DoCheck(int num, int incre, ref int min, ref int max)
        {
            min = max;
            max += incre;

            return num > min && num <= max;
        }
    }
}
