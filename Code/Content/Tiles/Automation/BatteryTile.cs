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
    public class BatteryTile : ModTile
    {
        public override string Texture => "Automaterria/Assets/Machines/BatteryTile";
        public override string Name => "BatteryTile";
        private static int _tileid;
        public static int tileid
        {
            get
            {
                if (_tileid > 0)
                    return _tileid;

                _tileid = ModContent.TileType<BatteryTile>();
                return _tileid;
            }
        }

        public override void SetStaticDefaults()
        {
            var entity = ModContent.GetInstance<BatteryEntity>();

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
            => TileUtils.BasicRightClick(i, j);

    }
}