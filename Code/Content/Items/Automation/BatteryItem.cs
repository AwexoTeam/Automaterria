using Terraria;
using System;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;
using Automaterria.Items;

namespace Automaterria.Tiles
{
    public class BatteryItem : ModItem
    {
        public override string Texture => "Automaterria/Assets/Machines/BatteryTile";
        public override string Name => "Battery";

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Stores Eenrgy");
            Item.createTile = ModContent.TileType<BatteryTile>();
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

            Item.DefaultToPlaceableTile(ModContent.TileType<BatteryTile>());
        }

        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            ItemRecipeHelper.AddBasicMachineIngredients(ref recipe, 0, MachineTypes.Cheap);
            recipe.AddIngredient<CapacitorItem>(4);
            recipe.AddIngredient<WireItem>(10);

            recipe.Register();
        }

        public override void RightClick(Terraria.Player player)
        {
            Console.WriteLine("Hey!");
        }
    }
}
