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
    internal class EbonwoodCursor : ClickerItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<WoodCursor>();
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            range = 250f;
            Item.DamageType = ModContent.GetInstance<Click>();
            Item.damage = 3;
            Item.knockBack = 2f;
            Item.width = 16;
            Item.height = 24;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Ebonwood, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
