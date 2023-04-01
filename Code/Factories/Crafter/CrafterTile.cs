using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Enums;
using Automaterria.Code.Ui;

namespace Automaterria.Code.Factories.Crafter
{
    public class CrafterTile : ModTile
    {
        public override string Name => "AutoCrafterTile";

        public override void SetStaticDefaults()
        {
            var entity = ModContent.GetInstance<CrafterEntity>();

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(entity.Hook_AfterPlacement, -1, 0, false);
            TileUtils.QuickSetFurniture(this, 1, 1, 0, null, false, Color.Red, false, true, "Factory");
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile tile = Main.tile[i, j];

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
            }
        }

        public override bool RightClick(int i, int j)
        {
            CrafterEntity entity = null;
            if (!TileUtils.TryGetTileEntityAs(i, j, out entity))
                return false;

            CrafterUIState.crafterUIState.Toggle(entity,i,j);
            entity.UIUpdate(entity, i, j);
            return base.RightClick(i, j);
        }

    }
}
