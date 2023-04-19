using Automaterria.Code;
using Automaterria.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Automaterria.Entities
{
    public class TapperEntity : Factory
    {
        public override int inventorySpaces => 1;
        public override int tickDely => 150;

        public override FactoryType factoryType => FactoryType.Battery;

        public override bool IsTileValidForEntity(int i, int j)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            bool rtn = (tile.TileType == ModContent.TileType<TapperTile>()) && tile.HasTile;

            return rtn;
        }

        protected override void Tick()
        {
            int x = Position.X;
            int y = Position.Y;
            
            List<Tile> neighbours = new List<Tile>();
            Tile n = Main.tile[x, y + 1];
            Tile e = Main.tile[x + 1, y];
            Tile s = Main.tile[x, y - 1];
            Tile w = Main.tile[x - 1, y];

            neighbours.Add(n);
            neighbours.Add(e);
            neighbours.Add(s);
            neighbours.Add(w);

            neighbours.RemoveAll(t => t == null);
            neighbours.RemoveAll(t => t.TileType != 5 && t.TileType != 21);

            if (neighbours.Count < 2)
                return;

            if (!neighbours.Exists(x => x.TileType == 5))
                return;

            Tile chest = neighbours.Find(x => x.TileType == 21);
            if (chest == null)
                return;

            Console.WriteLine("Everything is set!");
        }
    }
}
