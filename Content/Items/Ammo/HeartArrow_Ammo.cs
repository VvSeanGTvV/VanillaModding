using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Projectiles.Arrows;

namespace VanillaModding.Content.Items.Ammo
{
    internal class HeartArrow_Ammo : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 14; // The width of item hitbox
            Item.height = 32; // The height of item hitbox

            Item.damage = 4; // The damage for projectiles isn't actually 8, it actually is the damage combined with the projectile and the item together
            Item.DamageType = DamageClass.Ranged; // What type of damage does this ammo affect?

            Item.maxStack = Item.CommonMaxStack; // The maximum number of items that can be contained within a single stack
            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible
            Item.knockBack = 3f; // Sets the item's knockback. Ammunition's knockback added together with weapon and projectiles.
            Item.value = Item.sellPrice(0, 0, 0, 15); // Item price in copper coins (can be converted with Item.sellPrice/Item.buyPrice)
            Item.rare = ItemRarityID.Orange; // The color that the item's name will be in-game.
            Item.shoot = ModContent.ProjectileType<HeartArrow>(); // The projectile that weapons fire when using this item as ammunition.
            Item.shootSpeed = 12f; // The speed of the projectile.

            Item.ammo = AmmoID.Arrow; // Important. The first item in an ammo class sets the AmmoID to its type
        }
    }
}
