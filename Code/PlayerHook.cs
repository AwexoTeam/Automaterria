using Automaterria.Code;
using Automaterria.Code.Crafter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Automaterria
{
    public enum LookDirection
    {
        Left = -2,
        Right = 1,
        Up,
        Down,
    }

    internal class PlayerHook : ModPlayer
    {
        private List<Chest> availableChest;

        public override void OnEnterWorld(Player player)
        {
            base.OnEnterWorld(player);

            availableChest = new List<Chest>();
            SystemHook.OnModTick += Automaterria_OnModTick;
        }

        private void Automaterria_OnModTick()
        {
            availableChest.Clear();
            Chest middle = null;

            if (!HasChestByName("Filter", out middle) && !HasChestByName("Crafter", out middle))
                return;

            if(middle.name == "Crafter")
                OnCraftingChestTick(middle);
            
        }

        private bool HasChestByName(string name, out Chest chest)
        {
            chest = null;
            if (Main.chest == null)
                return false;

            foreach (Chest curr in Main.chest)
            {
                if (curr == null)
                    continue;

                if (curr.name != name)
                    continue;

                chest = curr;
                return true;
            }

            return false;

        }

        private bool IsChestNextTo(LookDirection dir, Chest origin, out Chest neighbour)
        {
            neighbour = null;

            int x = origin.x + (int)dir;
            int y = origin.y;

            foreach (Chest curr in Main.chest)
            {
                if (curr == null)
                    continue;

                if (curr.x != x)
                    continue;

                if (curr.y != y)
                    continue;

                neighbour = curr;
                return true;
            }

            return false;
        }

        private bool isEmpty(Chest chest)
            => Array.TrueForAll(chest.item, i => i.IsAir);

        private void OnCraftingChestTick(Chest chest)
        {
            Chest input = chest;
            Chest output = GetValidNeighbour(chest, LookDirection.Left, false);
            
            if(output == null)
            {
                Console.WriteLine("No Output chest found");
                return;
            }

            if (GetChestItemCount(input) < 2)
                return;

            Item craftingItem = input.item.First(x => !x.IsAir);
            List<Item> materials = Array.FindAll(input.item, x => !x.IsAir).ToList();
            materials.Remove(craftingItem);

            Recipe recipe = Array.Find(Main.recipe, x => craftingItem.type == x.createItem.type);
            if(recipe == null)
            {
                Console.WriteLine("Couldnt find recipe for " + craftingItem.Name);
                return;
            }

            if (!CanCraft(recipe, materials))
                return;

            Craft(recipe, materials, output);
        }

        private void Craft(Recipe recipe, List<Item> materials, Chest output)
        {
            Dictionary<int, int> requiredItems = new Dictionary<int, int>();
            foreach (var item in recipe.requiredItem)
            {
                if (!requiredItems.ContainsKey(item.type))
                    requiredItems.Add(item.type, 0);

                requiredItems[item.type] = item.stack;
            }

            foreach (Item mat in materials)
            {
                if (requiredItems.Count <= 0)
                    break;

                if (!requiredItems.ContainsKey(mat.type))
                    continue;

                int matRequirement = requiredItems[mat.type];
                if(mat.stack > matRequirement)
                {
                    mat.stack -= matRequirement;
                    requiredItems.Remove(mat.type);
                    continue;
                }

                if(mat.stack < matRequirement)
                {
                    requiredItems[mat.type] -= mat.stack;
                    mat.TurnToAir();
                    continue;
                }

                requiredItems.Remove(mat.type);
                mat.TurnToAir();
            }


        }

        private bool CanCraft(Recipe recipe, List<Item> materials)
        {
            foreach (var craftMat in recipe.requiredItem)
            {
                if (!materials.Exists(x => x.type == craftMat.type))
                {
                    //Console.WriteLine("Missing material: " + craftMat.Name);
                    return false;
                }

                var materialInstances = materials.FindAll(x => x.type == craftMat.type);
                int stackCnt = materialInstances.Sum(x => x.stack);
                if (stackCnt < craftMat.stack)
                {
                    //Console.WriteLine("Not enough of material " + craftMat.Name + " " + stackCnt + "/" + craftMat.stack);
                    return false;
                }
            }

            return true;
        }

        private Chest GetValidNeighbour(Chest origin, LookDirection dir, bool doEmptyCheck = true)
        {
            Chest rtn = null;

            if (!IsChestNextTo(dir, origin, out rtn))
                return null;

            if (isEmpty(rtn) && doEmptyCheck)
                return null;

            return rtn;
        }

        private int GetChestItemCount(Chest chest)
            => Array.FindAll(chest.item, i => !i.IsAir).Length;
    }
}
