﻿using Automaterria.Code;
using Automaterria.Code.Definitations.Factories;
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
    }

    public abstract class Factory : ModTileEntity
    {
        public static List<Factory> factories;
        public static List<int> factoryId;
        public abstract int tickDely { get; }
        public abstract FactoryType factoryType { get; }
        private DateTime lastTick = DateTime.Now;

        public List<PipeConnector> connectors;
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
    }
}
