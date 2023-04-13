using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Automaterria.Code.Ui;
using Microsoft.Xna.Framework.Graphics;
using Automaterria.Code;
using Automaterria.Entities;

namespace Automaterria.Tiles
{
    public class ItemPullerTile : ModTile
    {
        public override string Texture => "Automaterria/Assets/Pipes/ItemPullerTile";
        public override string Name => "ItemPullerTile";

        private static int _tileid;
        public static int tileid
        {
            get
            {
                if (_tileid > 0)
                    return _tileid;

                _tileid = ModContent.TileType<ItemPullerTile>();
                return _tileid;
            }
        }

        public override void SetStaticDefaults()
        {
            var entity = ModContent.GetInstance<ItemPullerEntity>();

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(entity.Hook_AfterPlacement, -1, 0, false);
            TileUtils.QuickSetFurniture(this, 1, 1, 0, null, false, Color.Red, true, true, "Factory");

            TileObjectData.newTile.CoordinateHeights = new int[] { 20 };
            TileObjectData.newTile.CoordinateWidth = 20;
            TileObjectData.newTile.DrawYOffset = -2;
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

            if (found && e is ItemPullerEntity factory)
            {
                FactoryUI.factoryUIState.Toggle(factory, $"Item Puller - {factory.lastError}", i, j);
                factory.UIUpdate(factory, i, j);
                return base.RightClick(i, j);
            }

            return false;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            int frameX = 0;
            int frameY = 0;
            if (TileUtils.ShouldPipeCOnnect(i - 1, j))
                frameX += 18;

            if (TileUtils.ShouldPipeCOnnect(i + 1, j))
                frameX += 36;
            if (TileUtils.ShouldPipeCOnnect(i, j - 1))
                frameY += 18;
            if (TileUtils.ShouldPipeCOnnect(i, j + 1))
                frameY += 36;

            Main.tile[i, j].TileFrameX = (short)frameX;
            Main.tile[i, j].TileFrameY = (short)frameY;
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            base.PostDraw(i, j, spriteBatch);
            
        }
    }
}