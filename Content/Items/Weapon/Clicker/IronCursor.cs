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
    internal class IronCursor : ClickerItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            range = 270f;
            Item.DamageType = ModContent.GetInstance<Click>();
            Item.damage = 3;
            Item.knockBack = 2f;
            Item.width = 16;
            Item.height = 24;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Pearlwood, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
