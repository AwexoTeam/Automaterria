using Terraria;
using System;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;
using Automaterria.Tiles;

namespace Automaterria.Items
{
    internal class TapperItem : ModItem
    {
        public override string Texture => "Automaterria/Assets/Machines/TapperTile";
        public override string Name => "Tapper";

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Gets resin from tree");
            Item.createTile = ModContent.TileType<TapperTile>();
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

            Item.DefaultToPlaceableTile(ModContent.TileType<TapperTile>());
        }

        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 200);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }

        public override void RightClick(Terraria.Player player)
        {
            Console.WriteLine("Hey!");
        }
    }
}
