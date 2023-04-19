using Automaterria.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Automaterria.Code.Content.Items.Intermediates
{
    //TODO: make this a buffer block item
    public class InventoryModuleItem : ModItem
    {
        public override string Texture => "Automaterria/Assets/Items/InventoryModuleItem";
        public override string Name => "InventoryModule";

        private static int _tileid;
        public static int tileid
        {
            get
            {
                if (_tileid > 0)
                    return _tileid;

                _tileid = ModContent.ItemType<InventoryModuleItem>();
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
            var recipe1 = CreateRecipe();
            recipe1.AddIngredient(ItemID.Chest, 1);
            recipe1.AddIngredient<WireItem>(5);
            recipe1.AddIngredient<HardPlatingItem>();
            recipe1.AddTile(TileID.HeavyWorkBench);
            recipe1.Register();
        }
    }
}
