using Terraria;
using System;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;
using Automaterria.Tiles;
using Automaterria.Code.Content.Items.Intermediates;

namespace Automaterria.Items
{
    public class FarmerItem : ModItem
    {
        public override string Texture => "Automaterria/Assets/Machines/FarmerTile";
        public override string Name => "AutoFarmer";

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Passively Gives Energy if its day!");
            Item.createTile = ModContent.TileType<FarmerTile>();
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

            Item.DefaultToPlaceableTile(ModContent.TileType<FarmerTile>());
        }

        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            ItemRecipeHelper.AddBasicMachineIngredients(ref recipe, 1, MachineTypes.Hard);
            recipe.AddIngredient(ItemID.DirtBlock, 200);
            recipe.AddIngredient<FertilizerItem>(20);
            recipe.AddTile(TileID.HeavyWorkBench);

            recipe.Register();
        }

        public override void RightClick(Terraria.Player player)
        {
            Console.WriteLine("Hey!");
        }
    }
}
