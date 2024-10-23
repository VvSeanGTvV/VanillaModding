using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace VanillaModding.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    internal class TorosHead : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30; // Width of the item
            Item.height = 28; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.White; // The rarity of the item
            //Item.defense = 5; // The amount of defense the item will give when equipped
            Item.vanity = true;
        }

        public override void SetStaticDefaults()
        {
            // HidesHands defaults to true which we don't want.
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
        }
    }
}
