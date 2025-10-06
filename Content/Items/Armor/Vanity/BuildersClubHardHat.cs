using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    internal class BuildersClubHardHat : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30; // Width of the item
            Item.height = 14; // Height of the item
            Item.value = Item.sellPrice(silver: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.White; // The rarity of the item
            //Item.defense = 5; // The amount of defense the item will give when equipped
            Item.vanity = true;
        }

        public override void SetStaticDefaults()
        {
            // HidesHands defaults to true which we don't want.
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }
    }
}
