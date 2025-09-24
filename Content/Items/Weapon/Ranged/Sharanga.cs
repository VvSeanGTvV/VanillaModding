using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Projectiles.Arrows;

namespace VanillaModding.Content.Items.Weapon.Ranged
{
    internal class Sharanga : ModItem
    {
        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 24; 
            Item.height = 48; 
            Item.scale = 0.95f;
            Item.rare = ItemRarityID.Pink; 

            // Use Properties
            Item.useTime = 22; // The item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 22; // The length of the item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot; 
            Item.autoReuse = true; 

            // The sound that this item plays when used.
            Item.UseSound = SoundID.Item5;

            // Weapon Properties
            Item.DamageType = DamageClass.Ranged; // Sets the damage type to ranged.
            Item.damage = 36*2; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.knockBack = 2f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.noMelee = true; // So the item's animation doesn't do damage.

            // Gun Properties
            Item.shoot = ModContent.ProjectileType<SpectralArrow>(); // For some reason, all the guns in the vanilla source have this.
            Item.shootSpeed = 16f; // The speed of the projectile (measured in pixels per frame.)
            Item.useAmmo = AmmoID.Arrow; // The "ammo Id" of the ammo item that this weapon uses. Ammo IDs are magic numbers that usually correspond to the item id of one item that most commonly represent the ammo type.
        }

        public override void AddRecipes()
        {
            Recipe recipe1 = CreateRecipe();
            recipe1.AddIngredient(ItemID.MoltenFury, 1);
            recipe1.AddIngredient(ItemID.ShroomiteBar,35);
            recipe1.AddTile(TileID.Anvils);
            recipe1.Register();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale = player.GetAdjustedItemScale(Item);
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SpectralArrow>(), damage, knockback, player.whoAmI);

            return false;
        }
    }
}
