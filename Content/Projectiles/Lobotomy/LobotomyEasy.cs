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
        private Vector2 origin;
        private float t = 0f; // Normalized progress [0, 1]
        private float tSpeed = 1f / 60f; // One arc per 60 frames
        private float parabolaWidth = 60f; // Width of the arc
        private float parabolaHeight = 20f; // Height of the arc
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
            // Initialize
            if (Projectile.localAI[0] == 0)
            {
                origin = Projectile.Center;
                Projectile.localAI[0] = 1;
            }

            // Clamp t to avoid overshooting
            t = MathHelper.Clamp(t, 0f, 2f);

            // Move from -width/2 to +width/2
            float x = MathHelper.Lerp(-parabolaWidth / 2f, parabolaWidth / 2f, t);

            // True parabola: y = a * x^2
            float a = parabolaHeight / (float)Math.Pow(parabolaWidth / 2f, 2);
            float y = a * x * x;

            // Drift upward
            float upwardOffset = -t * 60f; // Upward motion over time

            // Apply position
            Projectile.Center = origin + new Vector2(x, y + upwardOffset);

            // Progress along the curve
            t += tSpeed;

            // ✅ Reset only when t reaches 1.0 (end of arc)
            if (t >= 2.0f)
            {
                t = 0f;
                origin = Projectile.Center;
            }

            // Optional rotation (based on movement)
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}
