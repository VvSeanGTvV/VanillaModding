using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Consumable;
using VanillaModding.Content.Items.Materials.Bars;
using VanillaModding.Content.Projectiles.Arrows;
using VanillaModding.Content.Projectiles.Laser;

namespace VanillaModding.Content.Items.Ammo
{
    internal class PlasmicBulletBall : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 12; // The width of item hitbox
            Item.height = 12; // The height of item hitbox

            Item.damage = 9; // The damage for projectiles isn't actually 8, it actually is the damage combined with the projectile and the item together
            Item.DamageType = DamageClass.Ranged; // What type of damage does this ammo affect?

            Item.maxStack = Item.CommonMaxStack; // The maximum number of items that can be contained within a single stack
            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible
            Item.knockBack = 3f; // Sets the item's knockback. Ammunition's knockback added together with weapon and projectiles.
            Item.value = Item.sellPrice(0, 0, 5, 0); // Item price in copper coins (can be converted with Item.sellPrice/Item.buyPrice)
            Item.rare = ItemRarityID.Orange; // The color that the item's name will be in-game.
            Item.shoot = ModContent.ProjectileType<PlasmaBullet>(); // The projectile that weapons fire when using this item as ammunition.
            Item.shootSpeed = 24f; // The speed of the projectile.

            Item.ammo = AmmoID.Bullet; // Important. The first item in an ammo class sets the AmmoID to its type
        }

        public override void AddRecipes()
        {
            CreateRecipe(60)
                .AddIngredient(ItemID.MusketBall, 60)
                .AddIngredient(ModContent.ItemType<ElectrifiedBar>(), 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
