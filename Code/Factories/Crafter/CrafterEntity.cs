using Automaterria.Code;
using Automaterria.Code.Definitations.Factories;
using Automaterria.Code.Pipe;
using Automaterria.Code.Ui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Automaterria.Code.Factories.Crafter
{
	public class CrafterEntity : Factory
	{
		private enum CrafterErrorCode
		{
			UnknownError,
			NoInputs,
			NoCraft,
			NotCorrectStation,
			NoRecipe,
			NotEnoughIngredients,
			NoOutputs,
			NoSpaceInChest,
			Success,
            NotEnoughPower,
        }

		private CrafterErrorCode lastCode = CrafterErrorCode.Success;

		public override int tickDely => 150;
        public override int requiredPower => 100;

        public override FactoryType factoryType => FactoryType.Crafter;

		public override int inventorySpaces => 2;

        #region Formalities
        public override bool IsTileValidForEntity(int i, int j)
		{

			Tile tile = Framing.GetTileSafely(i, j);
			bool rtn = (tile.TileType == ModContent.TileType<CrafterTile>()) && tile.HasTile;

			return rtn;
		}

		protected override void PostPlacement(int x, int y, int style, int direction, int alternative)
		{
			base.PostPlacement(x, y, style, direction, alternative);
			if (Main.netMode != NetmodeID.MultiplayerClient)
				connectors = new List<PipeConnector>();
		}
		#endregion

		#region Data IO
		public override void SaveData(TagCompound tag)
		{
			Console.WriteLine("Saving...");
			base.SaveData(tag);

			tag["craftItemType"] = !HasItemInSlot(0) ? 0 : inventory[0].type;
			tag["craftItemStack"] = !HasItemInSlot(0) ? 0 : inventory[0].stack;

			tag["stationItemType"] = !HasItemInSlot(1) ? 0 : inventory[1].type;
			tag["stationItemStack"] = !HasItemInSlot(1) ? 0 : inventory[1].stack;
		}

		public override void LoadData(TagCompound tag)
		{
			base.LoadData(tag);

			int craftItemType = tag.GetAsInt("craftItemType");
			int craftItemStack = tag.GetAsInt("craftItemStack");
			Item lookup = null;

			if (craftItemType > 0)
			{
				lookup = Array.Find(Main.item, i => i.type == craftItemType);
				if (lookup != null)
					inventory[0] = new Item(craftItemType, craftItemStack);
			}

			Console.WriteLine($"x{craftItemStack} ID:{craftItemType}");

			int stationItemType = tag.GetAsInt("stationItemType");
			int stationItemStack = tag.GetAsInt("stationItemStack");

			if (stationItemType > 0)
			{
				lookup = Array.Find(Main.item, i => i.type == stationItemType);
				if (lookup != null)
					inventory[1] = new Item(stationItemType, stationItemStack);
			}

			Console.WriteLine($"x{stationItemStack} ID:{stationItemType}");
		}
		#endregion

		protected override void Tick()
		{
			
			CrafterErrorCode code = CrafterErrorCode.UnknownError;
			int pow = GetPower(Position.X,Position.Y, 0);

			code = pow < requiredPower ? CrafterErrorCode.NotEnoughPower : CrafterTick();

			if (code != lastCode)
			{
				Console.WriteLine(code);
				lastCode = code;
			}
		}
		private CrafterErrorCode CrafterTick()
		{
			

			if (Main.netMode == NetmodeID.MultiplayerClient)
				return CrafterErrorCode.Success;

			//TODO: Dont do floodfill in the fucking tick function...
			FactoryErrorCode factoryError = FactoryErrorCode.Success;
			connectors = GetConnectingChest(out factoryError);
			
			//return CrafterErrorCode.Success;
			
			if (factoryError != FactoryErrorCode.Success)
				return ConvertFactoryCode(factoryError);

			CrafterErrorCode code = ValidateSetup();
			if (code != CrafterErrorCode.Success)
				return code;

			Recipe recipe = Array.Find(Main.recipe, x => x.createItem.type == inventory[0].type);


			var chests = connectors
				.Where(x => x.isOutput)
				.Where(x => x.chest != null)
				.Select(x => x.chest);


			//TODO: Round-Robin
			code = Craft(recipe);
			if (code != CrafterErrorCode.Success)
				return code;

			if (AddToChest(recipe, inventory[0], chests.ToArray()))
				return CrafterErrorCode.NoSpaceInChest;

			return CrafterErrorCode.Success;
		}

		private CrafterErrorCode ValidateSetup()
		{
			if (connectors == null || connectors.Count <= 0)
			{
				if (connectors != null)
					connectors.Clear();

				FactoryErrorCode ferr = FactoryErrorCode.Success;
				connectors = GetConnectingChest(out ferr);
				if (ferr != FactoryErrorCode.Success)
					return ConvertFactoryCode(ferr);
			}

			List<Chest> inputs = connectors.Where(x => x.isInput).Where(x => x.chest != null).Select(x => x.chest).ToList();
			if (inputs.Count <= 0)
				return CrafterErrorCode.NoInputs;

            Console.WriteLine("Got this far!");
            Console.WriteLine("Is Inventory: " + inventory[0] == null);
			if (inventory[0] == null)
				return CrafterErrorCode.NoCraft;

			if (inventory[0].IsAir)
				return CrafterErrorCode.NoCraft;

			if (!Array.Exists(Main.recipe, x => x.createItem.type == inventory[0].type))
				return CrafterErrorCode.NoRecipe;

			//TODO: check if it needs a tileID
			if (false)
				return CrafterErrorCode.NotCorrectStation;

			return CrafterErrorCode.Success;
		}

		private Dictionary<int, int> GetRequiredMaterials(out CrafterErrorCode code, Recipe recipe)
		{
			List<Chest> inputs = connectors.Where(x => x.isInput).Where(x => x.chest != null).Select(x => x.chest).ToList();

			List<Item> materials = inputs.SelectMany(x => x.item).ToList();
			materials.RemoveAll(x => x.IsAir);

			Dictionary<int, int> leftToCraft = new Dictionary<int, int>();

			foreach (var ingredient in recipe.requiredItem)
			{
				if (!materials.Exists(x => x.type == ingredient.type))
				{
					code = CrafterErrorCode.NotEnoughIngredients;
					return leftToCraft;
				}

				int type = ingredient.type;

				var soughtMaterial = materials.Where(x => x.type == type);
				int count = soughtMaterial.Sum(x => x.stack);

				if (count < ingredient.stack)
				{
					code = CrafterErrorCode.NotEnoughIngredients;
					return leftToCraft;
				}

				leftToCraft.Add(ingredient.type, ingredient.stack);
			}

			code = CrafterErrorCode.Success;
			return leftToCraft;
		}

		private CrafterErrorCode Craft(Recipe recipe)
		{
			CrafterErrorCode code = CrafterErrorCode.Success;
			List<Chest> inputs = connectors.Where(x => x.isInput).Where(x => x.chest != null).Select(x => x.chest).ToList();
			List<Item> materials = inputs.SelectMany(x => x.item).ToList();
			materials.RemoveAll(x => x.IsAir);

			var leftToCraft = GetRequiredMaterials(out code, recipe);
			if (code != CrafterErrorCode.Success)
				return code;

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

			return code;
		}

		private CrafterErrorCode ConvertFactoryCode(FactoryErrorCode code)
		{
			switch (code)
			{
				case FactoryErrorCode.Success:
					return CrafterErrorCode.Success;
				case FactoryErrorCode.NoOutputs:
					return CrafterErrorCode.NoOutputs;
				default:
					return CrafterErrorCode.UnknownError;
			}
		}

		protected override void PostNetSend(BinaryWriter writer)
		{
			base.PostNetSend(writer);

			int crafterItemId = HasItemInSlot(0) ? inventory[0].type : -1;
			int stationItemId = HasItemInSlot(1) ? inventory[1].type : -1;

			writer.Write(crafterItemId);
			writer.Write(stationItemId);
		}

		protected override void PostNetRecieve(BinaryReader reader, FactoryType type, short x, short y)
		{
			base.PostNetRecieve(reader, type, x, y);

			CrafterEntity entity = null;
			if (!TileUtils.TryGetTileEntityAs(x, y, out entity))
			{
				Console.WriteLine($"{x},{y} is not a valid CrafterEntity.");
				return;
			}

			int crafterId = reader.ReadInt32();
			int stationId = reader.ReadInt32();

			if (crafterId > 0)
				inventory[0] = Main.item.Where(x => x.type == crafterId).First();

			if (stationId > 0)
				inventory[1] = Main.item.Where(x => x.type == stationId).First();

			factories.Add(entity);
		}
	}
}