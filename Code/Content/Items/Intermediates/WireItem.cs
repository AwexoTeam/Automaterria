using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Automaterria.Items
{
    public class WireItem : ModItem
    {
        public override string Texture => "Automaterria/Assets/Items/WireItem";
        public override string Name => "Wire";

        private static int _tileid;
        public static int tileid
        {
            get
            {
                if (_tileid > 0)
                    return _tileid;

                _tileid = ModContent.ItemType<WireItem>();
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
            var copperRecipe = CreateRecipe();
            copperRecipe.AddIngredient(BareWireItem.tileid, 1);
            copperRecipe.AddIngredient(InsolatorItem.tileid, 1);
            copperRecipe.Register();

        }
    }
}
