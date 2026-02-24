using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.DamageClasses;

namespace VanillaModding.Content.Items.Weapon.Clicker
{
    internal class WoodCursor : ClickerItem
    {
        public override void SetDefaults()
        {
            range = 250f;
            Item.DamageType = ModContent.GetInstance<Click>();
            Item.damage = 2;
            Item.knockBack = 1f;
            Item.width = 16;
            Item.height = 24;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
