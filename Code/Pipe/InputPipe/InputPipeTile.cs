
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Automaterria.Code.Pipe.InputPipe
{
    internal class InputPipeTile : ModTile
    {

		private static int _tileid;
        public static int tileid
        {
            get
            {
                if (_tileid > 0)
                    return _tileid;

                _tileid = ModContent.TileType<InputPipeTile>();
                return _tileid;
            }
        }

        public override string Name => "InputPipeTile";

        public override void SetStaticDefaults()
        {
            TileUtils.QuickSetFurniture(this, 1, 1, 0, null, false, Color.Red, true, true, "Pipe");

		}

        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile tile = Main.tile[i, j];

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            int frameX = 0;
            int frameY = 0;
            if (TileUtils.ShouldPipeCOnnect(i-1,j))
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
    }
}
