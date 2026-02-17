using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Projectiles.Arrows;
using VanillaModding.Content.Projectiles.Laser;

namespace VanillaModding.Content.Items.Weapon.Magic
{
    internal class Bunnynator : ModItem
    {
        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 90;
            Item.height = 42;
            Item.scale = 0.65f;
            Item.rare = ItemRarityID.Blue;

            // Use Properties
            Item.useTime = 17; // The item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 17; // The length of the item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            // The sound that this item plays when used.
            Item.UseSound = SoundID.Item12;

            // Weapon Properties
            Item.DamageType = DamageClass.Magic; // Sets the damage type to ranged.
            Item.damage = 46; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.knockBack = 2f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.noMelee = true; // So the item's animation doesn't do damage.
            Item.mana = 6; // The amount of mana the player needs to consume to use this item.

            // Gun Properties
            Item.shoot = ModContent.ProjectileType<OrangeLaser>(); // For some reason, all the guns in the vanilla source have this.
            Item.shootSpeed = 64f; // The speed of the projectile (measured in pixels per frame.)
            Item.value = Item.sellPrice(0, 1, 30, 0);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15f, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpaceGun, 1)
                .AddIngredient(ItemID.BunnyEars, 1)
                .AddIngredient(ItemID.BunnyTail, 1)
                .AddIngredient(ItemID.Vine, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
