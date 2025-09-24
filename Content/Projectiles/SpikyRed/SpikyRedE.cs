using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using VanillaModding.External.AI;

namespace VanillaModding.Content.Projectiles.SpikyRed
{
    internal class SpikyRedE : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 62; // The width of projectile hitbox
            Projectile.height = 38; // The height of projectile hitbox
            Projectile.aiStyle = -1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = (int)Math.Round(int.MaxValue / 1.5); ; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.scale = 2f;
            //Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame

            //AIType = ProjectileID.Bullet; // Act exactly like default Bullet
        }

        //Vector2 speedSave = //Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - closestNPC.Center).SafeNormalize(Vector2.Zero) * projSpeed, 0.1f);
        public override void AI()
        {
            AdvAI.FrameAnimate(0, 1, 5, Projectile);

            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            // Adding Pi to rotation if facing left corrects the drawing
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? MathHelper.Pi : 0f);

            if (Projectile.ai[0] == 1)
            {
                Projectile.ai[0] = 0;
                Projectile.ai[1] = Projectile.velocity.X;
                Projectile.ai[2] = Projectile.velocity.Y;
                Projectile.velocity = Vector2.Zero;
            }
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(Projectile.ai[1], Projectile.ai[2]).SafeNormalize(Vector2.Zero) * 40f, 0.1f);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}
