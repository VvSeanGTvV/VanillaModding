using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.NPCs.DeliveryDrone;
using VanillaModding.Content.Projectiles.Bombs;
using VanillaModding.Content.Projectiles.DiceProjectile;

namespace VanillaModding.Content.Items.Bombs
{
    internal class Firecracker : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 40;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.White;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;

            // Gun Properties
            Item.shoot = ModContent.ProjectileType<FirecrackerBomb>(); // For some reason, all the guns in the vanilla source have this.
            Item.shootSpeed = 5.5f; // The speed of the projectile (measured in pixels per frame.)
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 5; i++)
                Projectile.NewProjectile(source, player.MountedCenter, velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f)), ModContent.ProjectileType<FirecrackerBomb>(), damage, knockback / 2, player.whoAmI);

            return false;
        }
    }
}
