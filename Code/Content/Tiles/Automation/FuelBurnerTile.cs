using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Automaterria.Code;
using Automaterria.Entities;

namespace Automaterria.Tiles
{
    public class FuelBurnerTile : ModTile
    {
        public override string Texture => "Automaterria/Assets/Machines/FuelBurnerTile";
        public override string Name => "FuelBurnerTile";
        private static int _tileid;
        public static int tileid
        {
            get
            {
                if (_tileid > 0)
                    return _tileid;

                _tileid = ModContent.TileType<FuelBurnerTile>();
                return _tileid;
            }
        }

        public override void SetStaticDefaults()
        {
            var entity = ModContent.GetInstance<FuelBurnerEntity>();

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(entity.Hook_AfterPlacement, -1, 0, false);
            TileUtils.QuickSetFurniture(this, 2, 2, 0, null, false, Color.Red, false, true, "Factory");
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
            FuelBurnerEntity entity = null;
            if (!TileUtils.TryGetTileEntityAs(i, j, out entity))
                return false;

            //CrafterUIState.crafterUIState.Toggle(entity, i, j);
            //entity.UIUpdate(entity, i, j);
            return base.RightClick(i, j);
        }

    }
}