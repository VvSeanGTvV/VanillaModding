using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Projectiles.Lepus;

namespace VanillaModding.Content.Items.Weapon.Ranged
{
    internal class FlintlockRifle : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(82, 22);

            Item.DamageType = DamageClass.Ranged;
            Item.damage = 120;

            Item.knockBack = 0.5f;
            Item.noMelee = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = Item.useTime = 54;

            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);

            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
            => new Vector2(-15, 0);
    }
}
