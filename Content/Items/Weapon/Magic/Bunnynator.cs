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
            Item.width = 58;
            Item.height = 30;
            Item.scale = 0.85f;
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
            Item.damage = 22; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.knockBack = 2f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.noMelee = true; // So the item's animation doesn't do damage.
            Item.mana = 6; // The amount of mana the player needs to consume to use this item.

            // Gun Properties
            Item.shoot = ModContent.ProjectileType<OrangeLaser>(); // For some reason, all the guns in the vanilla source have this.
            Item.shootSpeed = 64f; // The speed of the projectile (measured in pixels per frame.)
            Item.value = Item.sellPrice(0, 3, 30, 0);
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5f, 0f);

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.armor[0].type == ItemID.MeteorHelmet &&
                player.armor[1].type == ItemID.MeteorSuit &&
                player.armor[2].type == ItemID.MeteorLeggings)
            {
                mult *= 0f;
            }
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpaceGun, 1)
                .AddIngredient(ItemID.Bunny, 1)
                .AddIngredient(ItemID.Hay, 2)
                .AddIngredient(ItemID.Vine, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
