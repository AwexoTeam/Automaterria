using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Automaterria.Code.Crafter
{
    public enum Direction
    {
        Right,
        Left,
        Top,
        Bottom,
    }

    public struct Vector2Int
    {
        public int X;
        public int Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class CrafterEntity : ModTileEntity
    {
        public List<Chest> inputs;

        public override bool IsTileValidForEntity(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
			bool rtn = (tile.TileType == ModContent.TileType<CrafterTile>()) && tile.HasTile;

            return rtn;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 0);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            else { inputs = new List<Chest>(); }

            return Place(i, j);
        }

        public override void Update()
        {
            if (!IsTileValidForEntity(Position.X, Position.Y))
            {
                inputs = new List<Chest>();
                Kill(Position.X, Position.Y);
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (inputs == null || inputs.Count <= 0)
                ValidateNeighbours();
        }

        public Vector2Int[] viableNeighbors = new Vector2Int[]
        {
            //Possible Right placements
            new Vector2Int(1,0),
            new Vector2Int(1,1),
            new Vector2Int(1,-1),

            //Possible Left placements
            new Vector2Int(-2,0),
            new Vector2Int(-2,1),
            new Vector2Int(-2,-1),

            //Possible Top placements
            new Vector2Int(0,-2),
            new Vector2Int(-1,-2),
            new Vector2Int(1,-2),

            //Possible Bottom placements
            new Vector2Int(0,1),
            new Vector2Int(-1,1),
            new Vector2Int(1,1),
        };

        public Dictionary<Direction, Vector2Int> directions = new Dictionary<Direction, Vector2Int>()
        {
            {Direction.Left, new Vector2Int(-1,0) },
            {Direction.Right, new Vector2Int(1,0) },
            {Direction.Top, new Vector2Int(-1,0) },
            {Direction.Bottom, new Vector2Int(1,0) },
        };

        private void ValidateNeighbours()
        {
            if (inputs == null)
                inputs = new List<Chest>();

            inputs.Clear();

            int x = Position.X;
            int y = Position.Y;
            List<int> possibleIndex = new List<int>();

            int dir = 0;

            for (int i = 0; i < viableNeighbors.Length; i+=3)
            {
                Direction currDir = (Direction)dir;
                var lookPos = directions[currDir];

                dir++;
                
                //if (Framing.GetTileSafely(x + lookPos.X, y + lookPos.Y).TileType != 21)
                //    continue;

                var pos1 = viableNeighbors[i];
                var pos2 = viableNeighbors[i+1];
                var pos3 = viableNeighbors[i+2];

                possibleIndex.Clear();
                possibleIndex.Add(Chest.FindChestByGuessing(pos1.X + x, pos1.Y + y));
                possibleIndex.Add(Chest.FindChestByGuessing(pos2.X + x, pos2.Y + y));
                possibleIndex.Add(Chest.FindChestByGuessing(pos3.X + x, pos3.Y + y));

                if (!possibleIndex.Exists(x => x > -1))
                    continue;

                int index = possibleIndex.Find(x => x != -1);
                Chest foundChest = Main.chest[index];
                if (!inputs.Contains(foundChest))
                    inputs.Add(foundChest);

            }

            Console.WriteLine($"There is currently {inputs.Count} chest(s)!");
        }

    }
}
    