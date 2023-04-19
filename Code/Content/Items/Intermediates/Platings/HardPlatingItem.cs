using Automaterria.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Automaterria.Items
{
    public class HardPlatingItem : ModItem
    {
        public override string Texture => "Automaterria/Assets/Items/HardPlatingItem";
        public override string Name => "HardPlating";

        private static int _tileid;
        public static int tileid
        {
            get
            {
                if (_tileid > 0)
                    return _tileid;

                _tileid = ModContent.ItemType<HardPlatingItem>();
                return _tileid;
            }
        }

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Something");
        }

        public override void SetDefaults()
        {
            Item.width = 1;
            Item.height = 1;

            Item.maxStack = 999;
            Item.consumable = true;
            Item.value = 100;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useTurn = true;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            int item1 = ItemID.IronBar;
            int item2 = ItemID.LeadBar;

            var recipe1 = CreateRecipe();
            recipe1.AddIngredient(item1, ItemRecipeHelper.PLATING_COST);
            recipe1.AddTile(TileID.HeavyWorkBench);
            recipe1.Register();

            var recipe2 = CreateRecipe();
            recipe2.AddIngredient(item2, ItemRecipeHelper.PLATING_COST);
            recipe2.AddTile(TileID.HeavyWorkBench);
            recipe2.Register();

        }
    }
}
