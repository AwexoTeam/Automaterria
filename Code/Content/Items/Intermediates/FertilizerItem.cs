using Automaterria.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Automaterria.Items
{
    internal class FertilizerItem : ModItem
    {
        public Dictionary<int,int> fertilizable = new Dictionary<int,int>
        {
            { TileID.Dirt, 20 },
        };

        public override string Texture => "Automaterria/Assets/Items/FertilizerItem";
        public override string Name => "Fertilizer";

        private static int _tileid;
        public static int tileid
        {
            get
            {
                if (_tileid > 0)
                    return _tileid;

                _tileid = ModContent.ItemType<FertilizerItem>();
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
            foreach (var pair in fertilizable)
            {
                var recipe = CreateRecipe();
                recipe.AddIngredient(pair.Key, pair.Value);
                recipe.Register();
            }
        }
    }
}
