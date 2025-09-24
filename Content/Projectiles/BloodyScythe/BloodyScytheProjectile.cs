using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace VanillaModding.Content.Projectiles.BloodyScythe
{
    internal class BloodyScytheProjectile : ModProjectile
    {
        float rotSpeed = 0.5f;
        float rotMul = 5f;
        public bool enemySide = false;

        public override void SetDefaults()
        {
            Projectile.width = 66; // The width of projectile hitbox
            Projectile.height = 58; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = !enemySide; // Can the projectile deal damage to enemies?
            Projectile.hostile = enemySide; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Melee; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 2; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 300; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            
            
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame

            //Projectile.light = 0.25f;
            //Projectile.alpha = 55;

            //AIType = ProjectileID.Bullet; // Act exactly like default Bullet

        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation += rotSpeed * rotMul / 10f * (float)Projectile.direction;

            Lighting.AddLight(Projectile.Center, new Vector3(0.769f, 0.294f, 0.325f));

            Projectile.velocity *= 0.96f;

            rotMul -= .005f;
            if (rotMul <= 0) rotMul = 0;

            if (Projectile.alpha <= 140)
            {
                Projectile.alpha += 1;
            }
            if (Projectile.alpha > 140)
            {

                Projectile.alpha += 15;

            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft >= 50) //it'll start to fade in when timeLeft is >= 50
            {
                return new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 100);
            }
            byte num4 = (byte)(Projectile.timeLeft * 3);
            byte num5 = (byte)(100.0 * ((double)num4 / (double)byte.MaxValue));
            return new Color((int)num4, (int)num4, (int)num4, (int)num5);
        }
    }
}
