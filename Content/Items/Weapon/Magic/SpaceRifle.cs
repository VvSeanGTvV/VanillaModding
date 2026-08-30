using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Projectiles.Laser;

namespace VanillaModding.Content.Items.Weapon.Magic
{
    internal class SpaceRifle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 24;
            Item.scale = 0.85f;
            Item.rare = ItemRarityID.Blue;

            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            Item.UseSound = SoundID.Item157;

            Item.DamageType = DamageClass.Magic;
            Item.damage = 32;
            Item.knockBack = 1f;
            Item.noMelee = true;
            Item.mana = 10;

            // Gun Properties
            Item.shoot = ProjectileID.GreenLaser; // For some reason, all the guns in the vanilla source have this.
            Item.shootSpeed = 35f; // The speed of the projectile (measured in pixels per frame.)
            Item.value = Item.sellPrice(0, 1, 20, 0);
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.armor[0].type == ItemID.MeteorHelmet &&
                player.armor[1].type == ItemID.MeteorSuit &&
                player.armor[2].type == ItemID.MeteorLeggings)
            {
                mult *= 0.15f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpaceGun, 1)
                .AddIngredient(ItemID.FallenStar, 2)
                .AddIngredient(ItemID.IllegalGunParts, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-2f, 0f);
    }
}
