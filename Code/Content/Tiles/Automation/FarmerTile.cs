using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Automaterria.Code.Ui;
using Automaterria.Code;
using Automaterria.Entities;

namespace Automaterria.Tiles
{
    public class FarmerTile : ModTile
    {
        public override string Texture => "Automaterria/Assets/Machines/FarmerTile";
        public override string Name => "FarmerTile";

        private static int _tileid;
        public static int tileid
        {
            get
            {
                if (_tileid > 0)
                    return _tileid;

                _tileid = ModContent.TileType<FarmerTile>();
                return _tileid;
            }
        }

        public override void SetStaticDefaults()
        {
            var entity = ModContent.GetInstance<FarmerEntity>();

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(entity.Hook_AfterPlacement, -1, 0, false);
            TileUtils.QuickSetFurniture(this, 2, 2, 0, null, false, Color.Red, false, true, "Factory");

            Main.tileSolidTop[Type] = true;
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
            TileEntity e = null;
            bool found = TileEntity.ByPosition.TryGetValue(new Point16(i, j), out e);

            if (found && e is FarmerEntity factory)
            {
                FactoryUI.factoryUIState.Toggle(factory, $"Farmer - {factory.lastCode}", i, j);
                factory.UIUpdate(factory, i, j);
                return base.RightClick(i, j);
            }

            return false;
        }

    }
}