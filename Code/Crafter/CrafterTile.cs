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

namespace Automaterria.Code.Crafter
{
    public class CrafterTile : ModTile
    {
        public override string Name => "AutoCrafterTile";

        public override void SetStaticDefaults()
        {
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;
            TileID.Sets.IsAMechanism[Type] = true;

            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileMergeDirt[Type] = false;

            var entity = ModContent.GetInstance<CrafterEntity>();
            var hook = entity.Hook_AfterPlacement;

            //TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(hook, -1, 1, false);
            
            //TileObjectData.addTile(Type);
        }

        // Read the comments above on AddMapEntry.
        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameY / 18);

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
            Crafter.RemoveTile(i, j);
            
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            base.PlaceInWorld(i, j, item);
            Crafter.AddTile(i, j);

        }
        //public override void PlaceInWorld(int i, int j, Item item)
        //{
        //    Tile tile = Main.tile[i, j];

        //    if (Main.netMode == NetmodeID.MultiplayerClient)
        //    {
        //        NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
        //    }
        //}

        //public override void KillMultiTile(int i, int j, int frameX, int frameY)
        //{
        //    Point16 origin = TileUtils.GetTileOrigin(i, j);
        //    ModContent.GetInstance<CrafterEntity>().Kill(origin.X, origin.Y);
        //}
    }
}
