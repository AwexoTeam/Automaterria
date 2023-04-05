using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
    }
}
