using Automaterria.Code.Content.Items.Intermediates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Automaterria.Items
{
    public enum MachineTypes
    {
        Cheap,
        Conductive,
        Hard,
        Heat,
        Reinforced,
    }

    public static class ItemRecipeHelper
    {
        public const int PLATING_COST = 20;

        public static void AddBasicMachineIngredients(ref Recipe recipe, int inventorySpaces, MachineTypes type)
        {
            recipe.AddIngredient<CapacitorItem>();
            recipe.AddIngredient<WireItem>(50);

            int platingID = GetPlatingByMachineType(type);
            recipe.AddIngredient(platingID, 6);

            if (inventorySpaces > 0)
                recipe.AddIngredient<InventoryModuleItem>(inventorySpaces);
        }

        public static int GetPlatingByMachineType(MachineTypes type)
        {
            switch (type)
            {
                case MachineTypes.Cheap:
                    return ModContent.ItemType<CheapPlatingItem>();
                case MachineTypes.Conductive:
                    return ModContent.ItemType<HardPlatingItem>();
                case MachineTypes.Hard:
                    return ModContent.ItemType<ConductivePlatingItem>();
                case MachineTypes.Heat:
                    return ModContent.ItemType<HeatResistantPlatingItem>();
                case MachineTypes.Reinforced:
                    return ModContent.ItemType<ReinforcedPlatingItem>();
                default:
                    return -1;
            }
        }
    }
}
