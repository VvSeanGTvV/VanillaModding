using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common;

namespace VanillaModding.Content.Items.Accessories
{
    internal class ValentineRing : ModItem
    {
        public override void SetDefaults()
        {
            int width = 30; int height = 30;
            Item.Size = new Vector2(width, height);

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Blue;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            VanillaModdingPlayer VMP = player.GetModPlayer<VanillaModdingPlayer>();
            if (VMP != null) VMP.accValentineRing = true;
            player.jumpSpeedBoost += 2.25f;
        }
    }
}
