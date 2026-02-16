using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.DamageClasses;

namespace VanillaModding.Content.Items.Accessories.Vanity
{
    [AutoloadEquip(EquipType.Neck)]
    internal class RTSoft : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 34;
            Item.accessory = true;
            Item.vanity = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }
    }
}
