using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Automaterria.Tiles;

namespace Automaterria.Items
{
    public  class OutputPipeItem : ModItem
    {
        public override string Texture => "Automaterria/Assets/Items/OutputPipeItem";
        public override string Name => "OutputPipe";

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Very basic output Pipe");
            Item.createTile = ModContent.TileType<OutputPipeTile>();
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

            Item.DefaultToPlaceableTile(ModContent.TileType<OutputPipeTile>());
        }

        public override void AddRecipes()
        {

            var recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);

            recipe.Register();
        }

    }
}
