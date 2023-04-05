using Automaterria.Code.Factories.Crafter;
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

        public const int objectHeight = 30;
        public const int offset = 10;
        public const int doubleOffset = offset * 2;

        private ItemEditorSlot[] inventory;
        private UIText header;
        private UIPanel panel;

        public override void OnInitialize()
        {
            panel = new UIPanel();
            header = new UIText("Factory");

            inventory = new ItemEditorSlot[Factory.MAX_INVENTORY_SPACE];
            float width = 0;
            float height = 0;

            GetRect(Factory.MAX_INVENTORY_SPACE, out width, out height);

            for (int i = 0; i < Factory.MAX_INVENTORY_SPACE; i++)
            {

                ItemEditorSlot slot = new ItemEditorSlot(i);
                slot.Width.Set(objectHeight, 0);
                slot.Height.Set(objectHeight, 0);
                slot.Left.Set((width - doubleOffset) - ((objectHeight*2) * (i+1)), 0);
                slot.Top.Set(objectHeight + offset, 0);
                panel.Append(slot);
                inventory[i] = slot;

            }

            ResizeUI("Factory", Factory.MAX_INVENTORY_SPACE);
        }

        private void GetRect(int inventorySpace, out float width, out float height)
        {
            
            width = 0;
            width = objectHeight + doubleOffset;
            width *= inventorySpace + 1;
            
            height = doubleOffset + objectHeight;
            height *= 2;
            height += doubleOffset;

            if (inventorySpace <= 1)
            {
                width = 200;
                height = doubleOffset + objectHeight;
            }
        }

        private void ResizeUI(string name, int inventorySpaces)
        {
            float width = 0;
            float height = 0;
            GetRect(inventorySpaces, out width, out height);

            panel.Width.Set(width, 0);
            panel.Height.Set(height, 0);
            panel.MarginTop = Main.screenHeight / 2;
            panel.MarginTop -= width;

            panel.MarginLeft = Main.screenWidth / 2;
            panel.MarginLeft -= height;
            Append(panel);

            header.SetText(name);
            header.HAlign = 0.5f;
            header.Top.Set(offset, 0);
            header.Height.Set(objectHeight, 0);
            panel.Append(header);

            for (int i = 0; i < Factory.MAX_INVENTORY_SPACE; i++)
            {
                ItemEditorSlot slot = inventory[i];
                panel.RemoveChild(slot);

                if (i < inventorySpaces)
                {
                    slot.Width.Set(objectHeight, 0);
                    slot.Height.Set(objectHeight, 0);
                    slot.Left.Set((width - doubleOffset) - ((objectHeight * 2) * (i + 1)), 0);
                    slot.Top.Set(objectHeight + offset, 0);
                    panel.Append(slot);
                }
            }

        }

        public void Toggle(Factory entity, string name, int i, int j)
        {
            if (!isOn)
            {
                ResizeUI(name, entity.inventorySpaces);
                Show(entity, name, i, j);
                return;
            }

            Hide(entity, i, j);
        }

        public void Show(Factory entity, string name, int x, int y)
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
                else
                    inventory[i].SetItem(new Item(0,0), false);
            }

            header.SetText(name);
            isOn = true;
            factoryInterface?.SetState(factoryUIState);
        }

        public void Hide(Factory entity, int x, int y)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (entity.inventorySpaces <= i)
                {
                    Console.WriteLine("Too much inventory allocated!");
                    continue;
                }

                Item currItem = inventory[i].StoredItem;
                Console.WriteLine("Storing Item: " + currItem.Name + " into slot " + i);

                if (currItem != null && !currItem.IsAir)
                    entity.inventory[i] = currItem;
            }

            isOn = false;
            factoryInterface?.SetState(null);
        }
    }
}
