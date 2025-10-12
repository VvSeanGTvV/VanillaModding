using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace VanillaModding.Content.Projectiles.Lobotomy
{
    internal class LobotomyEasy : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 100; // The width of projectile hitbox
            Projectile.height = 100; // The height of projectile hitbox

            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 0f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 60 * 5; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
        }

        public override void AI()
        {
            // Settings
            float arcLength = 60f;  // How long one full arc lasts (in frames)
            float arcWidth = 100f;  // Width of the arc
            float arcHeight = 40f;  // Height of the arc
            Vector2 center = Projectile.Center; // Will be overridden below

            // Time progress (0 to 1)
            float progress = Projectile.ai[0] / arcLength;

            // Reset when arc finishes to make it loop
            if (Projectile.ai[0] >= arcLength)
            {
                Projectile.ai[0] = 0;
            }

            // Calculate X and Y offset using parametric equation
            float x = progress * arcWidth;
            float y = -(4 * arcHeight / (arcWidth * arcWidth)) * (x - arcWidth / 2f) * (x - arcWidth / 2f);

            // Movement direction (right to left or left to right)
            Vector2 origin = Projectile.ai[1] == 0 ? Projectile.Center - new Vector2(arcWidth / 2f, 0)
                                                   : Projectile.Center + new Vector2(arcWidth / 2f, 0);

            // Set position based on arc
            Vector2 offset = new Vector2(x - arcWidth / 2f, y);
            Projectile.position = origin + offset;

            // Advance time
            Projectile.ai[0]++;
        }
    }
}
