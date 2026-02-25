using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.DamageClasses;

namespace VanillaModding.Content.Items.Weapon.Clicker
{
    internal class SpookywoodCursor : ClickerItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            range = 350f;
            Item.DamageType = ModContent.GetInstance<Click>();
            Item.damage = 12;
            Item.knockBack = 2f;
            Item.width = 16;
            Item.height = 24;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpookyWood, 100)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
