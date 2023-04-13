using Terraria;
using System;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;
using Automaterria.Tiles;

namespace Automaterria.Items
{
    public class ItemPullerItem : ModItem
    {
        public override string Texture => "Automaterria/Assets/Items/ItemPullerItem";
        public override string Name => "ItemPullerItem";

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Passively Gives Energy if its day!");
            Item.createTile = ModContent.TileType<ItemPullerTile>();
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

            Item.DefaultToPlaceableTile(ModContent.TileType<ItemPullerTile>());
        }

        public override void AddRecipes()
        {

            var recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);

            recipe.Register();
        }

        public override void RightClick(Terraria.Player player)
        {
            Console.WriteLine("Hey!");
        }
    }
}
