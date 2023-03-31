using Automaterria.Code;
using Automaterria.Code.Pipe;
using Automaterria.Code.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Automaterria.Code.Crafter
{

	public class CrafterEntity : ModTileEntity
	{
		public Item crafterItem = null;
		public Item stationItem = null;

		private enum CrafterErrorCode
		{
			NoInputs,
			NoCraft,
			NotCorrectStation,
			NoRecipe,
			NotEnoughIngredients,
			NoOutputs,
			NoSpaceInChest,
			Success,
		}

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

		private CrafterErrorCode lastCode = CrafterErrorCode.Success;
		public override void Update()
		{
			CrafterErrorCode code = CrafterTick();

			if (code != lastCode)
			{
				Console.WriteLine(code);
				lastCode = code;
			}
		}

		private CrafterErrorCode CrafterTick()
		{
			if (!IsTileValidForEntity(Position.X, Position.Y))
			{
				inputs = new List<Chest>();
				Kill(Position.X, Position.Y);
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
				return CrafterErrorCode.Success;

			if (inputs == null || inputs.Count <= 0)
				ValidateNeighbours();

			if (inputs.Count <= 0)
				return CrafterErrorCode.NoInputs;

			if (crafterItem == null)
				return CrafterErrorCode.NoCraft;
			
			if (crafterItem.IsAir)
				return CrafterErrorCode.NoCraft;

			Recipe recipe = Array.Find(Main.recipe, x => x.createItem.type == crafterItem.type);
			
			if (recipe == null)
				return CrafterErrorCode.NoRecipe;

			//TODO: check if it needs a tileID
			if (false)
				return CrafterErrorCode.NotCorrectStation;

			List<Item> materials = inputs.SelectMany(x => x.item).ToList();
			Dictionary<int, int> leftToCraft = new Dictionary<int, int>();
			materials.RemoveAll(x => x.IsAir);

			foreach (var ingredient in recipe.requiredItem)
			{
				if (!materials.Exists(x => x.type == ingredient.type))
					return CrafterErrorCode.NotEnoughIngredients;

				int type = ingredient.type;

				var soughtMaterial = materials.Where(x => x.type == type);
				int count = soughtMaterial.Sum(x => x.stack);

				if (count < ingredient.stack)
					return CrafterErrorCode.NotEnoughIngredients;

				leftToCraft.Add(ingredient.type, ingredient.stack);
			}

			List<Chest> chests = new List<Chest>();
			int i = Position.X;
			int j = Position.Y;

			//TODO: Dont do floodfill in the fucking tick function...
			chests.AddRange(PipeTile.GetConnectingChests(i + 1, j));
			chests.AddRange(PipeTile.GetConnectingChests(i - 1, j));
			chests.AddRange(PipeTile.GetConnectingChests(i, j + 1));
			chests.AddRange(PipeTile.GetConnectingChests(i, j - 1));

			if (chests.Count <= 0)
				return CrafterErrorCode.NoOutputs;

			Chest output = null;

			//TODO: better stack finding
			//TODO: figure out why you cant add to last

			bool hasAdded = false;
			foreach (var c in chests)
			{
				for (i = 0; i < c.item.Length-1; i++)
				{
					Item currItem = c.item[i];
					if(currItem.type == crafterItem.type)
					{
						if(currItem.stack + recipe.createItem.stack <= currItem.maxStack)
						{
							hasAdded = true;
							currItem.stack += recipe.createItem.stack;
							output = c;
							break;
						}
					}

					if (c.item[i].IsAir)
					{
						output = c;
						break;
					}
				}
			}

			if (output == null)
				return CrafterErrorCode.NoSpaceInChest;

			//TODO: Reduce this to one loop through.
			foreach (var mat in materials)
			{
				if (!leftToCraft.ContainsKey(mat.type))
					continue;

				int countLeftOfMat = leftToCraft[mat.type];
				int matStack = mat.stack;
				int matType = mat.type;

				if (matStack <= countLeftOfMat)
				{
					mat.TurnToAir();
				}
				else
				{
					mat.stack -= countLeftOfMat;
				}

				leftToCraft[matType] -= matStack;
				if (leftToCraft[matType] <= 0)
					leftToCraft.Remove(matType);
			}

			//TODO: Round-Robin
			
			if(!hasAdded)
				output.AddItemToShop(recipe.createItem);

			return CrafterErrorCode.Success;
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

			for (int i = 0; i < viableNeighbors.Length; i += 3)
			{
				Direction currDir = (Direction)dir;
				var lookPos = directions[currDir];

				dir++;

				var pos1 = viableNeighbors[i];
				var pos2 = viableNeighbors[i + 1];
				var pos3 = viableNeighbors[i + 2];

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
		}

		public override void SaveData(TagCompound tag)
		{
			Console.WriteLine("Saving...");
			base.SaveData(tag);

			tag["craftItemType"] = crafterItem == null ? 0 : crafterItem.type;
			tag["craftItemStack"] = crafterItem == null ? 0 : crafterItem.stack;

			tag["stationItemType"] = stationItem == null ? 0 : stationItem.type;
			tag["stationItemStack"] = stationItem == null ? 0 : stationItem.stack;

		}

		public override void LoadData(TagCompound tag)
		{
			base.LoadData(tag);
			
			int craftItemType = tag.GetAsInt("craftItemType");
			int craftItemStack = tag.GetAsInt("craftItemStack");
			Item lookup = null;

			if(craftItemType > 0)
			{
				lookup = Array.Find(Main.item, i => i.type == craftItemType);
				if(lookup != null)
					crafterItem = new Item(craftItemType, craftItemStack);
			}

			Console.WriteLine($"x{craftItemStack} ID:{craftItemType}");
			
			int stationItemType = tag.GetAsInt("stationItemType");
			int stationItemStack = tag.GetAsInt("stationItemStack");

			if(stationItemType > 0)
			{
				lookup = Array.Find(Main.item, i => i.type == stationItemType);
				if (lookup != null)
					stationItem = new Item(stationItemType, stationItemStack);
			}

			Console.WriteLine($"x{stationItemStack} ID:{stationItemType}");		
		}
	}
}