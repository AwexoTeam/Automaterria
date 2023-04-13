using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Enums;
using Automaterria.Code.Ui;
using Automaterria.Code;
using Automaterria.Entities;

namespace Automaterria.Tiles
{
    public class SolarPanelTile : ModTile
    {
        public override string Texture => "Automaterria/Assets/Machines/SolarPanelTile";
        public override string Name => "SolarPanelTile";

        private static int _tileid;
        public static int tileid
        {
            get
            {
                if (_tileid > 0)
                    return _tileid;

                _tileid = ModContent.TileType<SolarPanelTile>();
                return _tileid;
            }
        }

        public override void SetStaticDefaults()
        {
            var entity = ModContent.GetInstance<SolarBlockEntity>();

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(entity.Hook_AfterPlacement, -1, 0, false);
            TileUtils.QuickSetFurniture(this, 2, 2, 0, null, false, Color.Red, false, true, "Factory");

            Main.tileSolidTop[Type] = true;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile tile = Main.tile[i, j];

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
            }
        }

        public override bool RightClick(int i, int j)
        {
            TileEntity e = null;
            bool found = TileEntity.ByPosition.TryGetValue(new Point16(i, j), out e);

            if (found && e is SolarBlockEntity factory)
            {
                string name = $"Solar {factory.storedPower}/{SolarBlockEntity.solarPanelMaxPower}";
                FactoryUI.factoryUIState.Toggle(factory, name, i, j);
                factory.UIUpdate(factory, i, j);
                return base.RightClick(i, j);
            }

            return false;
        }

    }
}