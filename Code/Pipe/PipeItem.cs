using Automaterria.Code.Crafter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Automaterria.Code.Pipe
{
    public class PipeItem : ModItem
    {
        public override string Name => "PipeBlock";

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Very basicPipe");
            Item.createTile = ModContent.TileType<CrafterTile>();
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

            Item.DefaultToPlaceableTile(ModContent.TileType<PipeTile>());
        }

        public override void AddRecipes()
        {

            var recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);

            recipe.Register();
        }

    }
}
