using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using VanillaModding.Content.DamageClasses;

namespace VanillaModding.Content.Items.Weapon.Clicker
{
    internal class BaseCursor : ClickerItem
    {
        public override void SetDefaults()
        {
            range = 650f;
            Item.DamageType = ModContent.GetInstance<Click>();
            Item.damage = 50;
            Item.knockBack = 1f;
            Item.width = 46;
            Item.height = 52;
        }
    }
}
