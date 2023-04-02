﻿using Automaterria.Code.Factories.Crafter;
using Automaterria.Code.Pipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Automaterria.Code.Ui
{


    public class FactoryUI : UIState
    {
        public static bool isOn = false;

        public static UserInterface factoryInterface;
        public static FactoryUI factoryUIState;

        public const int elementCount = 2;
        public const int width = 200;
        public const int objectHeight = 30;
        public const int height = elementCount * (doubleOffset + objectHeight) + doubleOffset;
        public const int offset = 10;
        public const int doubleOffset = offset * 2;

        public ItemEditorSlot craftSlot;
        public ItemEditorSlot stationSlot;

        public ItemEditorSlot[] inventory;

        public override void OnInitialize()
        {
            inventory = new ItemEditorSlot[Factory.MAX_INVENTORY_SPACE];

            UIPanel panel = new UIPanel();
            panel.Width.Set(width, 0);
            panel.Height.Set(height, 0);
            panel.MarginTop = Main.screenHeight / 2;
            panel.MarginLeft = Main.screenWidth / 2;
            Append(panel);

            UIText header = new UIText("Crafter");
            header.HAlign = 0.5f;
            header.Top.Set(offset, 0);
            header.Height.Set(objectHeight, 0);
            panel.Append(header);

            craftSlot = new ItemEditorSlot(0);
            craftSlot.Width.Set(objectHeight, 0);
            craftSlot.Height.Set(objectHeight, 0);
            craftSlot.Top.Set(objectHeight + offset, 0);
            craftSlot.Left.Set(offset, 0);
            panel.Append(craftSlot);
            inventory[0] = craftSlot;

            stationSlot = new ItemEditorSlot(0);
            stationSlot.Width.Set(objectHeight, 0);
            stationSlot.Height.Set(objectHeight, 0);
            stationSlot.Left.Set(width - doubleOffset - objectHeight * 2, 0);
            stationSlot.Top.Set(objectHeight + offset, 0);
            panel.Append(stationSlot);
            inventory[1] = stationSlot;
        }

        public void Toggle(Factory entity, int i, int j)
        {
            if (!isOn)
            {
                Show(entity, i, j);
                return;
            }

            Hide(entity, i, j);
        }

        public void Show(Factory entity, int x, int y)
        {
            if(entity.inventory == null)
            {
                Console.WriteLine("Null inventory for " + entity.factoryType);
                return;
            }

            for (int i = 0; i < entity.inventory.Length; i++)
            {
                Item currItem = entity.inventory[i];
                if (currItem != null && !currItem.IsAir)
                    inventory[i].SetItem(currItem, false);
            }

            isOn = true;
            factoryInterface?.SetState(factoryUIState);
        }

        public void Hide(Factory entity, int x, int y)
        {
            //Factory.GetConnectingChests(x + 1, y);
            //Factory.GetConnectingChests(x - 1, y);
            //Factory.GetConnectingChests(x, y + 1);
            //Factory.GetConnectingChests(x, y - 1);

            for (int i = 0; i < inventory.Length; i++)
            {
                if (entity.inventory.Length <= inventory.Length)
                    continue;

                Item currItem = inventory[i].StoredItem;
                if (currItem != null && !currItem.IsAir)
                    entity.inventory[i] = currItem;
            }

            isOn = false;
            factoryInterface?.SetState(null);
        }
    }
}
