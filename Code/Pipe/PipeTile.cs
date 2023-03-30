using Automaterria.Code.Crafter;
using Automaterria.Code.Ui;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Automaterria.Code.Pipe
{
    public class PipeTile : ModTile
    {
        public static int searchDepth = 100;

        public override string Name => "PipeTile";

        public override void SetStaticDefaults()
        {
            TileUtils.QuickSetFurniture(this, 1, 1, 0, null, false, Color.Red, false, true, "Pipe");
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
            CrafterUIState.Toggle();
            return base.RightClick(i, j);
        }

        public static Chest[] GetConnectingChests(int i, int j)
        {
            Queue<Vector2Int> toCheck = new Queue<Vector2Int>();
            toCheck.Enqueue(new Vector2Int(i, j));

            int id 
            int currDepth = 0;

            while(currDepth <= searchDepth && toCheck.Count > 0)
            {
                currDepth++;
                Vector2Int currCheck = toCheck.Dequeue();


            }

            return null;
        }
    }
}
