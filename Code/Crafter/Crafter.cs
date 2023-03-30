using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Automaterria.Code.Crafter
{
    public struct Vector2Int
    {
        public int X;
        public int Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Vector2Int a, Vector2Int b)
        {
            if (a.X != b.X)
                return false;

            if (a.Y != b.Y)
                return false;

            return true;
        }

        public static bool operator !=(Vector2Int a, Vector2Int b)
            => !(a == b);

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            if ((obj is Vector2Int))
                return false;

            Vector2Int a = (Vector2Int)obj;
            return a == this;
        }
    }

    public static class Crafter
    {
        public static List<Vector2Int> tilePositions;

        public static void Initialize()
        {
            tilePositions = new List<Vector2Int>();
        }

        public static void Tick()
        {
            if (tilePositions == null)
                return;

            if (tilePositions.Count <= 0)
                return;

            Console.WriteLine("Tick!");
            foreach (var positions in tilePositions)
            {
                var chest = Main.chest.ToList().Find(x => x != null && x.x == positions.X + 1);
                if (chest == null)
                    continue;


            }
        }

        public static void AddTile(int x, int y)
        {
            Vector2Int pos = new Vector2Int(x, y);

            if (tilePositions.Contains(pos))
                return;

            tilePositions.Add(pos);
        }
        public static void RemoveTile(int x, int y)
        {
            Vector2Int pos = new Vector2Int(x, y);

            if (!tilePositions.Contains(pos))
                return;

            tilePositions.Remove(pos);
        }

        public static Chest[] GetConnectingChests(int x, int y)
        {
            List<Chest> chests = new List<Chest>();
            foreach (Chest curr in Main.chest)
            {
                if (curr == null)
                    continue;

                int cx = curr.x;
                int cy = curr.y;

                if(GetValidNeighbor(x, y).Contains(new Vector2Int(cx, cy)))
                    Console.WriteLine("Found Chest!");
            }


            return chests.ToArray();
        }

        private static Vector2Int[] GetValidNeighbor(int x, int y)
        {
            Vector2Int[] neighbors = new Vector2Int[4];
            neighbors[0] = new Vector2Int(x + 1, y);
            neighbors[1] = new Vector2Int(x - 1, y);
            neighbors[2] = new Vector2Int(x, y + 1);
            neighbors[3] = new Vector2Int(x, y - 1);

            return neighbors;
        }
    }
}
