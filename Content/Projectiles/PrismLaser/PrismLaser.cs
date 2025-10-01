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
        float beamLength = 2400f; // Beam distance — adjust for range
        float actualBeamLength = 0f;
        public override void SetDefaults()
        {
            Projectile.width = 10; // Solid line width
            Projectile.height = 10;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120; // Short lifespan, update as needed
            Projectile.DamageType = DamageClass.Magic;
        }

        // This property encloses the internal AI variable Projectile.ai[1].
        private float HostNPC
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        Vector2 velocityDirection;
        public override void AI()
        {
            // Beam logic — set velocity direction or position, etc.
            NPC hostNPCActive = Main.npc[(int)HostNPC];
            if (!hostNPCActive.active || hostNPCActive.type != ModContent.NPCType<TheChosenOne>())
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.velocity.LengthSquared() > 0)
            {
                velocityDirection = Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
            }
            Projectile.position = hostNPCActive.Center - new Vector2(Projectile.ai[1], 15);
            Projectile.rotation = velocityDirection.RotatedBy(Projectile.ai[2]).ToRotation();
            actualBeamLength = BeamHitScan(3);

            if (Main.netMode != NetmodeID.Server)
            {
                //ProduceWaterRipples(beamDims);
            }
        }

        private float BeamHitScan(int NumSamplePoints)
        {
            Vector2 samplingPoint = Projectile.Center;

            Player player = Main.player[Projectile.owner];
            NPC npc = Main.npc[(int)HostNPC];
            if (!Collision.CanHitLine(player.Center, 0, 0, Projectile.Center, 0, 0)) samplingPoint = player.Center;
            if (!Collision.CanHitLine(npc.Center, 0, 0, Projectile.Center, 0, 0)) samplingPoint = npc.Center;

            // Must match the draw direction!
            Vector2 unit = velocityDirection.RotatedBy(Projectile.ai[2]).SafeNormalize(Vector2.UnitX);
            float[] laserScanResults = new float[NumSamplePoints];
            Collision.LaserScan(samplingPoint, unit, Projectile.width * 0.5f * Projectile.scale, beamLength, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= NumSamplePoints;

            Vector2 hitPos = Projectile.Center + unit * actualBeamLength;
            Dust.NewDustPerfect(hitPos, DustID.RedTorch, Vector2.Zero).noGravity = true;

            return averageLengthSample;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host Prism), that's good enough.
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }

            // Otherwise, perform an AABB line collision check to check the whole beam.
            float _ = float.NaN;
            Vector2 unit = velocityDirection.RotatedBy(Projectile.ai[2]).SafeNormalize(Vector2.UnitX);
            Vector2 beamEndPos = Projectile.Center + unit * actualBeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, (Projectile.width + 2) * Projectile.scale, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.LaserMachinegunLaser).Value;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 start = Projectile.Center;
            Vector2 unit = velocityDirection.RotatedBy(Projectile.ai[2]).SafeNormalize(Vector2.UnitX);
            Color beamColor = Color.Red; // Adjustable color

            // Define source rectangles for each frame
            int frameHeight = texture.Height / 3;
            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);

            for (float i = 1; i <= actualBeamLength; i += Projectile.height)
            {
                int frame = 0;
                if (i > 1) frame++;
                if (i >= actualBeamLength - frameHeight) frame++;
                Vector2 drawPos = start + unit * i - Main.screenPosition;
                Main.spriteBatch.Draw(
                    texture,
                    drawPos,
                    new Rectangle(0, frameHeight * frame, texture.Width, Projectile.height),
                    beamColor,
                    Projectile.rotation - MathHelper.PiOver2,
                    origin,
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }

            return false; // We handled drawing
        }
    }
}
