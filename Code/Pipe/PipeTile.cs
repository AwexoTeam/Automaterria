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
        private static int _tileid;
        public static int tileid
        { 
            get
            {
                if (_tileid > 0)
                    return _tileid;

                _tileid = ModContent.TileType<PipeTile>();
                return _tileid;
            }
        }

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

        private static List<Vector2Int> hasChecked = new List<Vector2Int>();
        public static Chest[] GetConnectingChests(int i, int j)
        {
            hasChecked.Clear();

            Queue<Vector2Int> toCheck = new Queue<Vector2Int>();
            List<Vector2Int> possibleChests = new List<Vector2Int>();
            toCheck.Enqueue(new Vector2Int(i, j));

            int currDepth = 0;
            

            while(currDepth <= searchDepth && toCheck.Count > 0)
            {
                currDepth++;
                Vector2Int currCheck = toCheck.Dequeue();

                i = currCheck.X;
                j = currCheck.Y;

                if(!hasChecked.Contains(new Vector2Int(i+1,j)))
                    DoIteration(ref toCheck, ref possibleChests, i + 1, j);

                if (!hasChecked.Contains(new Vector2Int(i - 1, j)))
                    DoIteration(ref toCheck, ref possibleChests, i - 1, j);

                if (!hasChecked.Contains(new Vector2Int(i, j + 1)))
                    DoIteration(ref toCheck, ref possibleChests, i, j + 1);

                if (!hasChecked.Contains(new Vector2Int(i, j - 1)))
                    DoIteration(ref toCheck, ref possibleChests, i, j - 1);

                hasChecked.Add(new Vector2Int(i, j));
            }

            if (currDepth <= 1)
                return new Chest[0];

            if (possibleChests.Count <= 0)
                return new Chest[0];

            Vector2Int pos = possibleChests.First();
            int index = Chest.FindChestByGuessing(pos.X, pos.Y);

            List<Chest> chests = new List<Chest>();

            foreach (var chest in Main.chest)
            {
                if (chest == null)
                    continue;

                Vector2Int chestPos = new Vector2Int(chest.x, chest.y);
                foreach (var chkPos in possibleChests)
                {
                    if(chkPos.Distance(chestPos) <= 1.5f)
                    {
                        if (chests.Contains(chest))
                            continue;

                        chests.Add(chest);
                    }
                }
            }

            return chests.ToArray();
        }

        private static void DoIteration(ref Queue<Vector2Int> chk, ref List<Vector2Int> chests, int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if(tile.TileType == 21)
            {
                chests.Add(new Vector2Int(i,j));
                return;
            }

            if (tile.TileType != tileid)
                return;

            chk.Enqueue(new Vector2Int(i, j));
        }
    }
}
