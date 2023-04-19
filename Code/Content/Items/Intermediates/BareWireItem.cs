﻿using Automaterria.Items;
using Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Automaterria.Items
{
    public class BareWireItem : ModItem
    {
        public override string Texture => "Automaterria/Assets/Items/BareWireItem";
        public override string Name => "BareWire";

        private static int _tileid;
        public static int tileid
        {
            get
            {
                if (_tileid > 0)
                    return _tileid;

                _tileid = ModContent.ItemType<BareWireItem>();
                return _tileid;
            }
        }

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Something");
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
        }

        public override void AddRecipes()
        {
            var copperRecipe = CreateRecipe(5);
            copperRecipe.AddIngredient(ItemID.CopperBar, 1);
            copperRecipe.AddTile(TileID.Anvils);
            copperRecipe.Register();

            var tinRecipe = CreateRecipe(5);
            tinRecipe.AddIngredient(ItemID.TinBar, 1);
            tinRecipe.AddTile(TileID.Anvils);
            tinRecipe.Register();
        }
    }
}
