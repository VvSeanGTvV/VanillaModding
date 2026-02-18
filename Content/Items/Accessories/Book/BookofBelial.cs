using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items.Accessories.Book
{
    internal class BookofBelial : ModItem
    {
        public float damageBonus = 1.50f;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Math.Floor((damageBonus - 1f) * 1000f)/10f);
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) *= damageBonus;
        }
    }
}
