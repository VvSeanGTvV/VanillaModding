using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Weapon.Clicker;

namespace VanillaModding
{
    internal class VanillaModdingSystem : ModSystem
    {
        public override void AddRecipeGroups()
        {
            // Create a group where Iron Bar and Lead Bar are interchangeable
            RecipeGroup woodCursorGroup = new RecipeGroup(
                () => "Any Cursor Wood", // Display name
                ModContent.ItemType<WoodCursor>(),
                ModContent.ItemType<EbonwoodCursor>(),
                ModContent.ItemType<PearlwoodCursor>()
            );

            RecipeGroup woodGroup = new RecipeGroup(
                () => "Any Wood", // Display name
                ItemID.Wood,
                ItemID.Ebonwood,
                ItemID.RichMahogany,
                ItemID.Shadewood,
                ItemID.Pearlwood,
                ItemID.BorealWood,
                ItemID.PalmWood,
                ItemID.DynastyWood,
                ItemID.SpookyWood,
                ItemID.AshWood
            );

            // Register the group with a unique name
            RecipeGroup.RegisterGroup("VMT:AnyCursorWood", woodCursorGroup);
            RecipeGroup.RegisterGroup("VMT:AnyWood", woodGroup);
        }
    }
}
