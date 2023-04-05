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

namespace Automaterria.Code.Factories.ItemPuller
{
    public class ItemPullerEntity : Factory
    {
        public FactoryErrorCode lastError;

        public override int requiredPower => GlobalConfig.ItemPullerPowerReq;

        public override int inventorySpaces => 5;
        public override bool givesPower => true;
        public override int tickDely => GlobalConfig.ItemPullerTickDely;

        public override FactoryType factoryType => FactoryType.ItemPuller;

        public const int solarPanelPower = 10;
        public const int solarPanelMaxPower = 100;

        public override bool IsTileValidForEntity(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            
            bool rtn = (tile.TileType == ModContent.TileType<ItemPullerTile>()) && tile.HasTile;

            return rtn;
        }

        protected override void Tick()
        {
            lastError = ValidateSetup();
            if (lastError != FactoryErrorCode.Success)
                return;

            //TODO: Find connecting chests.
            Vector2Int pos = new Vector2Int(Position.X, Position.Y);
            float lowest = float.MaxValue;
            Chest input = null;

            foreach (var chest in Main.chest)
            {
                if (chest == null)
                    continue;

                Vector2Int chestPos = new Vector2Int(chest.x, chest.y);
                float dis = chestPos.Distance(pos);

                if (dis >= lowest)
                    continue;

                if (dis >= 2f)
                    continue;

                lowest = dis;
                input = chest;
            }

            if(input == null)
            {
                lastError = FactoryErrorCode.NoInputs;
                return;
            }

            var outputs = connectors.FindAll(x => x.isOutput).Select(x => x.chest).ToArray();
            if(Array.TrueForAll(inventory, x => x == null || x.IsAir))
            {
                Transfer(null, input, outputs);
                return;
            }

            

            Transfer(inventory, input, outputs);
        }

        private FactoryErrorCode ValidateSetup()
        {
            FactoryErrorCode error = FactoryErrorCode.UnknownError;
            error = BasicSetupValidation(false, true);
            if (error != FactoryErrorCode.Success)
                return error;

            List<Chest> outputs = connectors
                .Where(x => x.isOutput)
                .Where(x => x.chest != null)
                .Select(x => x.chest).ToList();

            bool hasSpace = outputs.SelectMany(c => c.item).ToList().Exists(i => i.IsAir);
            if (!hasSpace)
                return FactoryErrorCode.NoSpaceInChest;

            return FactoryErrorCode.Success;
        }

        private void Transfer(Item[] _filter, Chest input, params Chest[] outputs)
        {
            var inventorySlots = new List<Item>();
            foreach (var item in input.item)
            {
                if (item == null || item.IsAir)
                    continue;

                inventorySlots.Add(item);
            }

            if(_filter != null && _filter.Length > 0)
            {
                List<Item> filter = new List<Item>();
                foreach (var item in inventory)
                {
                    if (item == null || item.IsAir)
                        continue;

                    filter.Add(new Item(item.type, item.stack));
                }

                List<Item> filteredItem = new List<Item>();
                foreach (var item in filter)
                {
                    var validItem = inventorySlots.Find(x => x.type == item.type);
                    if (validItem == null)
                        continue;

                    filteredItem.Add(validItem);
                }

                inventorySlots.Clear();
                inventorySlots.AddRange(filteredItem);
            }
            
            Item first = inventorySlots.FirstOrDefault();
            if(first == null)
            {
                lastError = FactoryErrorCode.NotEnoughIngredients;
                return;
            }

            AddToChest(first, outputs);
            first.TurnToAir();
        }
    }

}
