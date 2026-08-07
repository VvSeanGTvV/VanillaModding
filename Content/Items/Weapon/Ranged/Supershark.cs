using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Materials.Bars;
using VanillaModding.Content.Projectiles.Laser;

namespace VanillaModding.Content.Items.Weapon.Ranged
{
    internal class Supershark : ModItem
    {
        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 44; // Hitbox width of the item.
            Item.height = 24; // Hitbox height of the item.
            Item.rare = ItemRarityID.Pink; // The color that the item's name will be in-game.
            Item.scale = 1.25f;

            // Use Properties
            Item.useTime = 5; // The item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 5; // The length of the item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
            Item.autoReuse = true; // Whether or not you can hold click to automatically use it again.

            // The sound that this item plays when used.
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.UseSound = SoundID.Item11;

            // Weapon Properties
            Item.DamageType = DamageClass.Ranged; // Sets the damage type to ranged.
            Item.damage = 35; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.knockBack = 1.5f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.noMelee = true; // So the item's animation doesn't do damage.

            // Gun Properties
            Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
            Item.shootSpeed = 20f; // The speed of the projectile (measured in pixels per frame.) This value equivalent to Handgun
            Item.useAmmo = AmmoID.Bullet; // The "ammo Id" of the ammo item that this weapon uses. Ammo IDs are magic numbers that usually correspond to the item id of one item that most commonly represent the ammo type.
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.Megashark, 1)
                .AddIngredient(ItemID.IllegalGunParts, 1)
                .AddIngredient(ItemID.SoulofFright, 20)
                .AddIngredient(ItemID.SandBlock, 20)
                .AddIngredient(ItemID.SharkFin, 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        // This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, -2f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) Projectile.NewProjectile(source, position + new Vector2(0, Main.rand.NextFloat(-.25f, .25f) + .25f), velocity.RotatedByRandom(MathHelper.ToRadians(7.5f)), type, damage, knockback, player.whoAmI);
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool(4);
    }
}
