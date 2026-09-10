using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items.Weapon.Throwable
{
    internal class Egg : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(56, 20);

            Item.DamageType = DamageClass.Ranged;
            Item.damage = 13;

            Item.knockBack = 3.5f;
            Item.noMelee = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 21;

            Item.maxStack = Item.CommonMaxStack;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);

            Item.shoot = ModContent.ProjectileType<Content.Projectiles.Lepus.Egg>();
            Item.shootSpeed = 8f;
            Item.consumable = true;
            Item.ammo = Item.type;
        }
    }
}
