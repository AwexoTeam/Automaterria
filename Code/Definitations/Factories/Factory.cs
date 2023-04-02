using Automaterria.Code;
using Automaterria.Code.Definitations.Factories;
using Automaterria.Code.Factories.Battery;
using Automaterria.Code.Factories.Crafter;
using Automaterria.Code.Factories.FuelBurner;
using Automaterria.Code.Factories.SolarPanel;
using Automaterria.Code.Pipe;
using Automaterria.Code.Pipe.BasicPipe;
using Automaterria.Code.Pipe.InputPipe;
using Automaterria.Code.Pipe.OutputPipe;
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

namespace Automaterria.Code
{

    public enum FactoryErrorCode
    {
        Success,
        NoOutputs,

    }

    public enum FactoryType
    {
        None,
        Crafter,
        FuelBurner,
        SolarPanel,
        Battery,
        Farmer,
    }

    public abstract class Factory : ModTileEntity
    {
        public static List<Factory> factories;
        public abstract int tickDely { get; }
        public abstract FactoryType factoryType { get; }
        private DateTime lastTick = DateTime.Now;

        public List<PipeConnector> connectors;
        public virtual bool givesPower { get; }
        public virtual bool requiresPower { get; }
        public int storedPower;
        public virtual int requiredPower { get; }
        public abstract int inventorySpaces { get; }

        private Item[] _inventory = new Item[MAX_INVENTORY_SPACE];
        public virtual Item[] inventory { get { return _inventory; } set { _inventory = value; } }
        public const int MAX_INVENTORY_SPACE = 5;

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (factories == null)
                factories = new List<Factory>();

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 0);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }

            if (Main.netMode == NetmodeID.Server)
            {
                factories.Add(this);
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, 0, Position.X, Position.Y);
            }

            int rtn = Place(i, j);
            PostPlacement(i, j, style, direction, alternate);
            return rtn;
        }

        public override void NetReceive(BinaryReader reader)
        {
            byte type = reader.ReadByte();
            //TODO: check if this is a factory.

            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            FactoryType _factoryType = (FactoryType)reader.ReadInt16();

            PostNetRecieve(reader, _factoryType, x, y);
        }

        public override void NetSend(BinaryWriter writer)
        {
            Console.WriteLine("Sent!");
            writer.Write(type);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write((int)factoryType);

            PostNetSend(writer);
        }

        protected virtual void PostNetSend(BinaryWriter writer) { }
        protected virtual void PostNetRecieve(BinaryReader reader, FactoryType type, short x, short y) { }

        protected virtual void PostPlacement(int x, int y, int style, int direction, int alternative) { }


        public override void Load()
        {
            base.Load();
            if (inventory == null)
            {
                int inv = inventorySpaces > MAX_INVENTORY_SPACE ? MAX_INVENTORY_SPACE : inventorySpaces;

                inventory = new Item[inv];
                Console.WriteLine("Initialized Inv for "+factoryType+" with "+inventory.Length+"!");
            }
        }
        public override void Update()
        {
            if (!IsTileValidForEntity(Position.X, Position.Y))
            {
                Kill(Position.X, Position.Y);
            }

            var span = DateTime.Now - lastTick;
            if (span.TotalMilliseconds < tickDely)
                return;

            Tick();
            lastTick = DateTime.Now;
        }

        protected abstract void Tick();

        protected List<PipeConnector> GetConnectingChest(out FactoryErrorCode code)
        {
            List<PipeConnector> chests = new List<PipeConnector>();

            int i = Position.X;
            int j = Position.Y;

            chests.AddRange(GetConnectingChests(i + 1, j));
            chests.AddRange(GetConnectingChests(i - 1, j));
            chests.AddRange(GetConnectingChests(i, j + 1));
            chests.AddRange(GetConnectingChests(i, j - 1));

            code = chests.Count <= 0 ? FactoryErrorCode.NoOutputs : FactoryErrorCode.Success;
            
            return chests;
        }

        protected bool AddToChest(Recipe recipe, Item crafterItem, params Chest[] chests)
        {
            //TODO: better stack finding
            //TODO: figure out why you cant add to last
            
            bool hasAdded = false;
            Chest output = null;

            foreach (var c in chests)
            {
                for (int i = 0; i < c.item.Length - 1; i++)
                {
                    Item currItem = c.item[i];
                    if (currItem.type == crafterItem.type)
                    {
                        if (currItem.stack + recipe.createItem.stack <= currItem.maxStack)
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

            if (!hasAdded)
                output.AddItemToShop(recipe.createItem);

            if (output == null)
                return false;

            return hasAdded;
        }

        private const int searchDepth = 100;
        private static List<Vector2Int> hasChecked = new List<Vector2Int>();
        
        public static PipeConnector[] GetConnectingChests(int i, int j)
        {
            hasChecked.Clear();

            Queue<Vector2Int> toCheck = new Queue<Vector2Int>();
            List<Vector2Int> possibleChests = new List<Vector2Int>();
            List<PipeConnector> pipeTilesConnecting = new List<PipeConnector>();

            toCheck.Enqueue(new Vector2Int(i, j));

            int currDepth = 0;

            while (currDepth <= searchDepth && toCheck.Count > 0)
            {
                currDepth++;
                Vector2Int currCheck = toCheck.Dequeue();

                i = currCheck.X;
                j = currCheck.Y;

                if (!hasChecked.Contains(new Vector2Int(i + 1, j)))
                    DoIteration(ref toCheck, ref possibleChests, ref pipeTilesConnecting, i + 1, j, currCheck);

                if (!hasChecked.Contains(new Vector2Int(i - 1, j)))
                    DoIteration(ref toCheck, ref possibleChests, ref pipeTilesConnecting, i - 1, j, currCheck);

                if (!hasChecked.Contains(new Vector2Int(i, j + 1)))
                    DoIteration(ref toCheck, ref possibleChests, ref pipeTilesConnecting, i, j + 1, currCheck);

                if (!hasChecked.Contains(new Vector2Int(i, j - 1)))
                    DoIteration(ref toCheck, ref possibleChests, ref pipeTilesConnecting, i, j - 1, currCheck);

                hasChecked.Add(new Vector2Int(i, j));
            }

            if (currDepth <= 1)
                return new PipeConnector[0];

            if (possibleChests.Count <= 0)
                return new PipeConnector[0];

            Vector2Int pos = possibleChests.First();
            int index = Chest.FindChestByGuessing(pos.X, pos.Y);
            List<PipeConnector> rtn = new List<PipeConnector>();
            foreach (var chest in Main.chest)
            {
                if (chest == null)
                    continue;

                Vector2Int chestPos = new Vector2Int(chest.x, chest.y);
                foreach (var chkPos in possibleChests)
                {
                    if (chkPos.Distance(chestPos) <= 1.5f)
                    {
                        if (pipeTilesConnecting.Exists(x => x.chest == chest))
                            continue;

                        PipeConnector connector = pipeTilesConnecting.Find(x => chkPos == x.chestPos);
                        connector.chest = chest;

                        rtn.Add(connector);
                    }
                }
            }

            return rtn.ToArray();
        }

        private static int[] validPipeIds = new int[]
        {
            PipeTile.tileid,
            InputPipeTile.tileid,
            OutputPipeTile.tileid,
        };

        private static void DoIteration(ref Queue<Vector2Int> chk, ref List<Vector2Int> chests, ref List<PipeConnector> connectors, int i, int j, Vector2Int origin)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile originTile = Framing.GetTileSafely(origin.X, origin.Y);

            if (tile.TileType == 21)
            {
                chests.Add(new Vector2Int(i, j));
                PipeConnectorMode mode = PipeConnectorMode.None;
                bool isInput = originTile.TileType == InputPipeTile.tileid;
                bool isOutput = originTile.TileType == OutputPipeTile.tileid;

                mode = isInput ? PipeConnectorMode.Input : mode;
                mode = isOutput ? PipeConnectorMode.Output : mode;

                connectors.Add(new PipeConnector(null, mode, origin, new Vector2Int(i, j)));
                return;
            }

            if (!validPipeIds.Contains(tile.TileType))
                return;

            chk.Enqueue(new Vector2Int(i, j));
        }

        public virtual void UIUpdate(Factory entity, int x, int y)
        {

        }

        public int GetPower(int i, int j, int power, bool clear = true)
        {
            if (clear)
                hasChecked.Clear();

            if (hasChecked.Contains(new Vector2Int(i, j)))
                return power;

            hasChecked.Add(new Vector2Int(i, j));
            Tile tile = Framing.GetTileSafely(i, j);
            if (ByPosition.TryGetValue(new Point16(i, j), out TileEntity e) && e is Factory factory)
            {
                power += factory.givesPower ? factory.storedPower : 0;
            }

            if (!tile.RedWire)
                return power;

            if (Framing.GetTileSafely(i + 1, j).RedWire)
                power = GetPower(i + 1, j, power, false);

            if (Framing.GetTileSafely(i - 1, j).RedWire)
                power = GetPower(i - 1, j, power, false);

            if (Framing.GetTileSafely(i, j + 1).RedWire)
                power = GetPower(i, j + 1, power, false);

            if (Framing.GetTileSafely(i, j - 1).RedWire)
                power = GetPower(i, j - 1, power, false);

            return power;
        }

        public bool HasItemInSlot(int index)
        {
            if (inventory == null)
                return false;

            if (inventory.Length <= index)
                return false;


            if (inventory[index] == null)
                return false;

            if (inventory[index].IsAir)
                return false;

            return true;
        }
    }
}
