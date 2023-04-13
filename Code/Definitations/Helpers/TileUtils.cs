using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Automaterria.Code.Ui;
using Automaterria.Tiles;

namespace Automaterria.Code
{
	public static class TileUtils
	{
		public static int[] FactoryIDs = new int[]
		{
			21,
			PipeTile.tileid,
			InputPipeTile.tileid,
			OutputPipeTile.tileid,
			ItemPullerTile.tileid,
			FuelBurnerTile.tileid,
			CrafterTile.tileid,
			QuarryTile.tileid,
			FarmerTile.tileid,
		};

		public static void QuickSetFurniture(this ModTile tile, int width, int height, int dustType, SoundStyle? hitSound, bool tallBottom, Color mapColor, bool solidTop = false, bool solid = false, string mapName = "", AnchorData bottomAnchor = default, AnchorData topAnchor = default, int[] anchorTiles = null, bool faceDirection = false, bool wallAnchor = false, Point16 Origin = default)
		{
			Main.tileLavaDeath[tile.Type] = false;
			Main.tileFrameImportant[tile.Type] = true;
			Main.tileSolidTop[tile.Type] = solidTop;
			Main.tileSolid[tile.Type] = solid;

			TileObjectData.newTile.Width = width;
			TileObjectData.newTile.Height = height;

			TileObjectData.newTile.CoordinateHeights = new int[height];

			for (int k = 0; k < height; k++)
			{
				TileObjectData.newTile.CoordinateHeights[k] = 16;
			}

			if (tallBottom) //this breaks for some tiles: the two leads are multitiles and tiles with random styles
				TileObjectData.newTile.CoordinateHeights[height - 1] = 18;

			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.Origin = Origin == default ? new Point16(width / 2, height - 1) : Origin;

			if (bottomAnchor != default)
				TileObjectData.newTile.AnchorBottom = bottomAnchor;
			
			if (topAnchor != default)
				TileObjectData.newTile.AnchorTop = topAnchor;

			if (anchorTiles != null)
				TileObjectData.newTile.AnchorAlternateTiles = anchorTiles;

			if (wallAnchor)
				TileObjectData.newTile.AnchorWall = true;

			if (faceDirection)
			{
				TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
				TileObjectData.newTile.StyleHorizontal = true;
				TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
				TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
				TileObjectData.addAlternate(1);
			}

			TileObjectData.addTile(tile.Type);

			ModTranslation name = tile.CreateMapEntryName();
			name.SetDefault(mapName);
			tile.AddMapEntry(mapColor, name);
		}

		/// <summary>
		/// Gets the top-left tile of a multitile
		/// </summary>
		/// <param name="i">The tile X-coordinate</param>
		/// <param name="j">The tile Y-coordinate</param>
		public static Point16 GetTileOrigin(int i, int j)
		{
			//Framing.GetTileSafely ensures that the returned Tile instance is not null
			//Do note that neither this method nor Framing.GetTileSafely check if the wanted coordiates are in the world!
			Tile tile = Framing.GetTileSafely(i, j);

			Point16 coord = new Point16(i, j);
			Point16 frame = new Point16(tile.TileFrameX / 18, tile.TileFrameY / 18);

			return coord - frame;
		}

		/// <summary>
		/// Uses <seealso cref="GetTileOrigin(int, int)"/> to try to get the entity bound to the multitile at (<paramref name="i"/>, <paramref name="j"/>).
		/// </summary>
		/// <typeparam name="T">The type to get the entity as</typeparam>
		/// <param name="i">The tile X-coordinate</param>
		/// <param name="j">The tile Y-coordinate</param>
		/// <param name="entity">The found <typeparamref name="T"/> instance, if there was one.</param>
		/// <returns><see langword="true"/> if there was a <typeparamref name="T"/> instance, or <see langword="false"/> if there was no entity present OR the entity was not a <typeparamref name="T"/> instance.</returns>
		public static bool TryGetTileEntityAs<T>(int i, int j, out T entity) where T : TileEntity
		{
			Point16 origin = GetTileOrigin(i, j);

			//TileEntity.ByPosition is a Dictionary<Point16, TileEntity> which contains all placed TileEntity instances in the world
			//TryGetValue is used to both check if the dictionary has the key, origin, and get the value from that key if it's there
			if (TileEntity.ByPosition.TryGetValue(origin, out TileEntity existing) && existing is T existingAsT)
			{
				entity = existingAsT;
				return true;
			}

			entity = null;
			return false;
		}

		public static bool ShouldPipeCOnnect(int i, int j)
		{
			if (!(WorldGen.InWorld(i, j) && Main.tile[i, j].HasTile))
				return false;

			int type = Main.tile[i, j].TileType;

			if (FactoryIDs.Contains(type))
				return true;

			return false;
		}

		public static bool BasicRightClick(int i, int j, out Factory factory)
		{
			TileEntity e = null;
			factory = null;
			bool found = TileEntity.ByPosition.TryGetValue(new Point16(i, j), out e);
			if (!found)
				return false;

			if (!(e is Factory))
				return false;

			factory = (Factory)e;
			FactoryUI.factoryUIState.Toggle(factory, factory.GetUIName(), i, j);
			factory.UIUpdate(factory, i, j);

			return true;
		}

		public static bool BasicRightClick(int i, int j)
        {
			Factory f = null;
			return BasicRightClick(i, j, out f);
        }
	}
}
