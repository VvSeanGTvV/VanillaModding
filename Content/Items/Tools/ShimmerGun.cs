using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Projectiles.Bullets;
using VanillaModding.Content.Rarities;

namespace VanillaModding.Content.Items.Tools
{
    internal class ShimmerGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SlimeGun);
            Item.shoot = ModContent.ProjectileType<ShimmerBullet>();
            Item.value = Item.sellPrice(0, 0, 30, 0);
            Item.rare = ModContent.RarityType<CosmicPurple>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-2f, 0f);
    }
}
