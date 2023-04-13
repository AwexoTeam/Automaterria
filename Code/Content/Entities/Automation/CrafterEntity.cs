using Automaterria.Code;
using Automaterria.Code.Definitations.Factories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Automaterria.Tiles;

namespace Automaterria.Entities
{
    public class CrafterEntity : Factory
	{
		public FactoryErrorCode lastCode = FactoryErrorCode.Success;

		public override int tickDely => GlobalConfig.CrafterTickDely;
        public override int requiredPower => GlobalConfig.CrafterPowerReq;

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

		protected override void Tick()
		{
			FactoryErrorCode code = FactoryErrorCode.UnknownError;
			int pow = GetPower(Position.X,Position.Y, 0);

			code = pow < requiredPower ? FactoryErrorCode.NotEnoughPower : CrafterTick();

			if (code != lastCode)
			{
				Console.WriteLine(code);
				lastCode = code;
			}
		}
		private FactoryErrorCode CrafterTick()
		{
			BasicSetupValidation();
			if (lastError != FactoryErrorCode.Success)
				return lastError;

			if (false)
				return FactoryErrorCode.NotCorrectStation;

			
			Recipe recipe = Array.Find(Main.recipe, x => x.createItem.type == inventory[0].type);


			var chests = connectors
				.Where(x => x.isOutput)
				.Where(x => x.chest != null)
				.Select(x => x.chest);


			//TODO: Round-Robin
			lastError = Craft(recipe);
			if (lastError != FactoryErrorCode.Success)
				return lastError;

			if (AddToChest(recipe.createItem, chests.ToArray()))
				return FactoryErrorCode.NoSpaceInChest;

			TakePower(requiredPower);

			return FactoryErrorCode.Success;
		}

		private FactoryErrorCode ValidateSetup()
		{
			if (connectors == null || connectors.Count <= 0)
			{
				if (connectors != null)
					connectors.Clear();

				FactoryErrorCode ferr = FactoryErrorCode.Success;
				connectors = GetConnectingChest(out ferr);
				if (ferr != FactoryErrorCode.Success)
					return ferr;
			}

			List<Chest> inputs = connectors.Where(x => x.isInput).Where(x => x.chest != null).Select(x => x.chest).ToList();
			if (inputs.Count <= 0)
				return FactoryErrorCode.NoInputs;

			if (inventory[0] == null)
				return FactoryErrorCode.NoCraft;

            Console.WriteLine("Got this far!");
			if (inventory[0].IsAir)
				return FactoryErrorCode.NoCraft;

			if (!Array.Exists(Main.recipe, x => x.createItem.type == inventory[0].type))
				return FactoryErrorCode.NoRecipe;

			//TODO: check if it needs a tileID
			
			return FactoryErrorCode.Success;
		}

		private Dictionary<int, int> GetRequiredMaterials(out FactoryErrorCode code, Recipe recipe)
		{
			List<Chest> inputs = connectors.Where(x => x.isInput).Where(x => x.chest != null).Select(x => x.chest).ToList();

			List<Item> materials = inputs.SelectMany(x => x.item).ToList();
			materials.RemoveAll(x => x.IsAir);

			Dictionary<int, int> leftToCraft = new Dictionary<int, int>();

			foreach (var ingredient in recipe.requiredItem)
			{
				if (!materials.Exists(x => x.type == ingredient.type))
				{
					code = FactoryErrorCode.NotEnoughIngredients;
					return leftToCraft;
				}

				int type = ingredient.type;

				var soughtMaterial = materials.Where(x => x.type == type);
				int count = soughtMaterial.Sum(x => x.stack);

				if (count < ingredient.stack)
				{
					code = FactoryErrorCode.NotEnoughIngredients;
					return leftToCraft;
				}

				leftToCraft.Add(ingredient.type, ingredient.stack);
			}

			code = FactoryErrorCode.Success;
			return leftToCraft;
		}

		private FactoryErrorCode Craft(Recipe recipe)
		{
			FactoryErrorCode code = FactoryErrorCode.Success;
			List<Chest> inputs = connectors.Where(x => x.isInput).Where(x => x.chest != null).Select(x => x.chest).ToList();
			List<Item> materials = inputs.SelectMany(x => x.item).ToList();
			materials.RemoveAll(x => x.IsAir);

			var leftToCraft = GetRequiredMaterials(out code, recipe);
			if (code != FactoryErrorCode.Success)
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