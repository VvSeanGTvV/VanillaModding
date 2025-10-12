using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
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
        public float value = -1f;
        public Vector2 startPosition;

        public override void OnSpawn(IEntitySource source)
        {
            startPosition = Projectile.position;
        }

        public override void AI()
        {
            value += 0.05f;  // increase value each tick

            if (value > 1f)
            {
                value = -1f;
                Projectile.position = startPosition;  // reset position on each bounce cycle
            }

            float velocityFactor = value < 0 ? value : 1 - (value - 1) * (value - 1);
            Projectile.velocity = new Vector2(0, -10f * velocityFactor);
        }
    }
}
