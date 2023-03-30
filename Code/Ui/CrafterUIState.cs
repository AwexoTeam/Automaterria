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


    public class CrafterUIState : UIState
    {
        public static bool isOn = false;

        public static UserInterface crafterInterface;
        public static CrafterUIState crafterUIState;

        public const int elementCount = 2;
        public const int width = 200;
        public const int objectHeight = 30;
        public const int height = elementCount * (doubleOffset + objectHeight) + doubleOffset;
        public const int offset = 10;
        public const int doubleOffset = offset * 2;

        public static ItemEditorSlot craftSlot;
        public static ItemEditorSlot stationSlot;
        public override void OnInitialize()
        {
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

            stationSlot = new ItemEditorSlot(0);
            stationSlot.Width.Set(objectHeight, 0);
            stationSlot.Height.Set(objectHeight, 0);
            stationSlot.Left.Set(width - doubleOffset - objectHeight * 2, 0);
            stationSlot.Top.Set(objectHeight + offset, 0);
            panel.Append(stationSlot);

        }

        public static void Toggle()
        {
            if (isOn)
                Hide();
            else
                Show();
        }

        public static void Show()
        {
            isOn = true;
            crafterInterface?.SetState(crafterUIState);
        }

        public static void Hide()
        {
            isOn = false;
            crafterInterface?.SetState(null);
        }
    }
}
