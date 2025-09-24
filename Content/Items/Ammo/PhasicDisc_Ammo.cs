using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using VanillaModding.Content.Projectiles.PhasicWarpEjector;

namespace VanillaModding.Content.Items.Ammo
{
    internal class PhasicDisc_Ammo : ModItem
    {
        public override void SetStaticDefaults()
        {
            AmmoID.Sets.IsSpecialist[Type] = true; // This item will benefit from the Shroomite Helmet.

        }
        public override void SetDefaults()
        {
            Item.width = 28; // The width of item hitbox
            Item.height = 20; // The height of item hitbox

            Item.damage = 125; // The damage for projectiles isn't actually 8, it actually is the damage combined with the projectile and the item together
            Item.DamageType = DamageClass.Ranged; // What type of damage does this ammo affect?

            Item.maxStack = Item.CommonMaxStack; // The maximum number of items that can be contained within a single stack
            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible
            Item.knockBack = 7f; // Sets the item's knockback. Ammunition's knockback added together with weapon and projectiles.
            Item.value = Item.buyPrice(silver: 30);
            Item.rare = ItemRarityID.Orange; // The color that the item's name will be in-game.
            Item.shoot = ModContent.ProjectileType<PhasicDisk>(); // The projectile that weapons fire when using this item as ammunition.
            Item.shootSpeed = 16f/2; // The speed of the projectile.

            Item.ammo = Item.type; // Make the item as an ammo ID
            Item.scale = 1f;
        }
    }
}
