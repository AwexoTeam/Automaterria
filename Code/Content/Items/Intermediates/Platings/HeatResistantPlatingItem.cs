using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Automaterria.Items
{
    internal class HeatResistantPlatingItem : ModItem
    {
        public override string Texture => "Automaterria/Assets/Items/HeatResistantPlatingItem";
        public override string Name => "HeatResistantPlating";

        private static int _tileid;
        public static int tileid
        {
            get
            {
                if (_tileid > 0)
                    return _tileid;

                _tileid = ModContent.ItemType<HeatResistantPlatingItem>();
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
            int item1 = ItemID.HellstoneBar;
            
            var recipe1 = CreateRecipe();
            recipe1.AddIngredient(item1, ItemRecipeHelper.PLATING_COST);
            recipe1.AddTile(TileID.HeavyWorkBench);
            recipe1.Register();


        }
    }
}
