using Terraria;
using System;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;
using Automaterria.Tiles;

namespace Automaterria.Items
{
    public class FuelBurnerItem : ModItem
    {
        public override string Texture => "Automaterria/Assets/Machines/FuelBurnerTile";
        public override string Name => "FuelBurner";

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Burns Biomass and Wood for energy!");
            Item.createTile = ModContent.TileType<FuelBurnerTile>();
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

            Item.DefaultToPlaceableTile(ModContent.TileType<FuelBurnerTile>());
        }

        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            ItemRecipeHelper.AddBasicMachineIngredients(ref recipe, 1, MachineTypes.Cheap);
            recipe.AddIngredient(ItemID.Furnace, 1);
            recipe.AddIngredient<BatteryItem>();
            recipe.AddTile(TileID.HeavyWorkBench);

            recipe.Register();
        }

        public override void RightClick(Terraria.Player player)
        {
            Console.WriteLine("Hey!");
        }
    }
}
