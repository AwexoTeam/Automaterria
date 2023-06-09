using Terraria;
using System;
using Terraria.ModLoader;
using System.Linq;
using Terraria.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Automaterria.Code.Ui;

namespace Automaterria.Code
{
    public class SystemHook : ModSystem
    {
        public int interval = 100;

        private DateTime lastTick = DateTime.Now;

        public delegate void _OnModTick();
        public static event _OnModTick OnModTick;

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            var span = DateTime.Now - lastTick;

            if (span.TotalMilliseconds <= interval)
                return;

            OnModTick?.Invoke();
            lastTick = DateTime.Now;
        }

        public override void Load()
        {
            base.Load();
            if (!Main.dedServ)
            {
                FactoryUI.factoryInterface = new UserInterface();
                FactoryUI.factoryUIState = new FactoryUI();
                FactoryUI.factoryUIState.Activate();
            }
        }

        private GameTime _lastUpdateUiGameTime;
        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (FactoryUI.factoryInterface?.CurrentState != null)
            {
                FactoryUI.factoryInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "MyMod: MyInterface",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && FactoryUI.factoryInterface?.CurrentState != null)
                        {
                            FactoryUI.factoryInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

    }
}