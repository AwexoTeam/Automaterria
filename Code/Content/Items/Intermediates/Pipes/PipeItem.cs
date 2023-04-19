using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Automaterria.Tiles;

namespace Automaterria.Items
{
    public class PipeItem : ModItem
    {
        public override string Name => "Pipe";
        public override string Texture => "Automaterria/Assets/Items/PipeItem";
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Very basicPipe");
            Item.createTile = ModContent.TileType<PipeTile>();
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
            int item1 = ItemID.CopperBar;
            int item2 = ItemID.TinBar;

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