using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace VanillaModding.Content.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Legs)]
    internal class TorosLegs : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 16; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.White; // The rarity of the item
            //Item.defense = 5; // The amount of defense the item will give when equipped
            Item.vanity = true;
        }

        public override void SetStaticDefaults()
        {
            // HidesHands defaults to true which we don't want.
            ArmorIDs.Legs.Sets.HidesBottomSkin[Item.legSlot] = true;
        }
    }
}
