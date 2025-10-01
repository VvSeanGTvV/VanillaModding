using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.NPCs.TheChosenOne;

namespace VanillaModding.Content.Projectiles.PrismLaser
{
    internal class PrismLaser : ModProjectile
    {
        float beamLength = 1000f; // Beam distance — adjust for range
        float actualBeamLength = 0f;
        public override void SetDefaults()
        {
            Projectile.width = 10; // Solid line width
            Projectile.height = 10;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60; // Short lifespan, update as needed
            Projectile.DamageType = DamageClass.Magic;
        }

        Vector2 velocityDirection;
        public override void AI()
        {
            // Beam logic — set velocity direction or position, etc.
            if (Projectile.velocity.LengthSquared() > 0)
            {
                velocityDirection = Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
            }
            Projectile.rotation = velocityDirection.ToRotation();
            actualBeamLength = BeamHitScan(3);
        }

        private float BeamHitScan(int NumSamplePoints)
        {
            Vector2 samplingPoint = Projectile.Center;

            Player player = Main.player[Projectile.owner];
            NPC npc = Main.npc[Projectile.owner];
            if (!Collision.CanHitLine(player.Center, 0, 0, Projectile.Center, 0, 0)) samplingPoint = player.Center;
            if (!Collision.CanHitLine(npc.Center, 0, 0, Projectile.Center, 0, 0)) samplingPoint = npc.Center;

            float[] laserScanResults = new float[NumSamplePoints];
            Collision.LaserScan(samplingPoint, velocityDirection, 0 * Projectile.scale, beamLength, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= NumSamplePoints;

            return averageLengthSample;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.LaserMachinegunLaser).Value;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 start = Projectile.Center;
            Vector2 unit = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Color beamColor = Color.Red; // Adjustable color

            for (float i = 0; i <= actualBeamLength; i += texture.Height*2)
            {
                Vector2 drawPos = start + unit * i - Main.screenPosition;
                Main.spriteBatch.Draw(
                    texture,
                    drawPos,
                    null,
                    beamColor,
                    Projectile.rotation,
                    new Vector2(texture.Width / 2, texture.Height / 2),
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }

            return false; // We handled drawing
        }
    }
}
