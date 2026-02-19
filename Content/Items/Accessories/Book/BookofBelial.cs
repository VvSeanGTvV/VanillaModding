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
        public float damageBonus = 1.322f;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Math.Floor((damageBonus - 1f) * 1000f)/10f);
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 2, silver: 10);
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if (
                equippedItem.type == ModContent.ItemType<BookSatanicBible>()
                ) return false;
            return base.CanAccessoryBeEquippedWith(equippedItem, incomingItem, player);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) *= damageBonus;
        }
    }
}
