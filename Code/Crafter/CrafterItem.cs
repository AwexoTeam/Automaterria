using Terraria;
using System;
using Terraria.ModLoader;
using Automaterria.Code.Resources;
using System.Collections.Generic;
using Terraria.ID;

namespace Automaterria.Code.Crafter
{
    public class CrafterItem : ModItem
    {
        public override string Name => "AutoCrafterBlock";

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Very basic Auto Crafter");
            Item.createTile = ModContent.TileType<CrafterTile>();

        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 8;

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
