using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Projectiles.Lepus;

namespace VanillaModding.Content.Items.Weapon.Ranged
{
    internal class EggCannon : ModItem
    {
        public override void SetDefaults()
        {
            Item.Size = new Vector2(56, 20);

            Item.DamageType = DamageClass.Ranged;
            Item.damage = 20;

            Item.knockBack = 0.5f;
            Item.noMelee = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = Item.useTime = 24;

            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);

            Item.shoot = ModContent.ProjectileType<EasterEgg>();
            Item.shootSpeed = 10f;
        }

        public override Vector2? HoldoutOffset()
            => new Vector2(-5, 0);
    }
}
