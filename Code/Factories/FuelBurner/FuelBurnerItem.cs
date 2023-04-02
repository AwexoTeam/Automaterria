using Terraria;
using System;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;

namespace Automaterria.Code.Factories.FuelBurner
{
    public class FuelBurnerItem : ModItem
    {
        public override string Name => "FuelBurnerItem";

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Burns Biomass and Wood for energy!");
            Item.createTile = ModContent.TileType<FuelBurnerBlock>();
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

            Item.DefaultToPlaceableTile(ModContent.TileType<FuelBurnerBlock>());
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
