using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automaterria.Code
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

		public float Distance(Vector2Int b)
			=> Vector2Int.Distance(this, b);

		public static float Distance(Vector2Int a, Vector2Int b)
		{
			float p1 = b.X - a.X;
			p1 = MathF.Pow(p1, 2);

			float p2 = b.Y - a.Y;
			p2 = MathF.Pow(p2, 2);

			return MathF.Sqrt(p1 + p2);
		}
	}
}
