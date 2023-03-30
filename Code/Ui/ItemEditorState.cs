//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Automaterria.Code.Ui
{
    public class UIPanelTab : UITextPanel<LocalizedText>
    {
        public readonly string Name;

        public UIPanelTab(string name, LocalizedText text, float textScale = 1, bool large = false) : base(text, textScale, large)
        {
            Name = name;
        }
    }
}
