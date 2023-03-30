using Terraria;
using System;
using Terraria.ModLoader;
using System.Linq;

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

    }
}