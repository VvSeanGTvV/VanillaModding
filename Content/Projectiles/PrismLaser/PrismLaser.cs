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
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.NPCs.TheChosenOne;

namespace VanillaModding.Content.Projectiles.PrismLaser
{
    internal class PrismLaser : ModProjectile
    {
        // The maximum brightness of the light emitted by the beams. Brightness scales from 0 to this value as the Prism's charge increases.
        private const float BeamLightBrightness = 0.75f;
        private const float BeamLength = 2400f; // Beam distance — adjust for range


        float actualBeamLength = 0f;
        public override void SetDefaults()
        {
            Projectile.width = 22; // Solid line width
            Projectile.height = 22;
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

        NPC hostNPCActive;
        Vector2 velocityDirection;
        public override void AI()
        {
            // Beam logic — set velocity direction or position, etc.
            hostNPCActive = Main.npc[(int)HostNPC];
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
            //Main.NewText($"{Projectile.ai[1]} | ID: {Projectile.whoAmI}");

            Vector2 beamDims = new Vector2(velocityDirection.Length() * BeamLength, Projectile.width * Projectile.scale);
            if (Main.netMode != NetmodeID.Server)
            {
                ProduceWaterRipples(beamDims);
            }
        }

        private void ProduceWaterRipples(Vector2 beamDims)
        {
            WaterShaderData shaderData = (WaterShaderData)Filters.Scene["WaterDistortion"].GetShader();

            // A universal time-based sinusoid which updates extremely rapidly. GlobalTime is 0 to 3600, measured in seconds.
            float waveSine = 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
            Vector2 ripplePos = Projectile.position + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(Projectile.rotation);

            // WaveData is encoded as a Color. Not really sure why.
            Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
            shaderData.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, Projectile.rotation);
        }

        private float BeamHitScan(int NumSamplePoints)
        {
            Vector2 samplingPoint = Projectile.Center;

            NPC npc = Main.npc[(int)HostNPC];
            //if (!Collision.CanHitLine(player.Center, 0, 0, Projectile.Center, 0, 0)) samplingPoint = player.Center;
            if (!Collision.CanHitLine(npc.Center, 0, 0, Projectile.Center, 0, 0)) samplingPoint = npc.Center;

            // Must match the draw direction!
            Vector2 unit = velocityDirection.RotatedBy(Projectile.ai[2]).SafeNormalize(Vector2.UnitX);
            float[] laserScanResults = new float[NumSamplePoints];
            Collision.LaserScan(samplingPoint, unit, Projectile.width * 0.5f * Projectile.scale, BeamLength, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= NumSamplePoints;

            Vector2 hitPos = Projectile.Center + unit * actualBeamLength;
            Dust hitDust = Dust.NewDustPerfect(hitPos, DustID.RedTorch, new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-2f, -4f)));
            hitDust.noGravity = true;

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
            if (!hostNPCActive.active)
            {
                return false;
            }

            Vector2 start = hostNPCActive.Center - new Vector2(Projectile.ai[1], 15); ;
            Vector2 unit = velocityDirection.RotatedBy(Projectile.ai[2]).SafeNormalize(Vector2.UnitX);
            Color beamColor = Color.Red; // Adjustable color

            // Define source rectangles for each frame
            int frameHeight = texture.Height / 3;
            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);

            float totalLength = (Projectile.height + actualBeamLength);
            for (float i = 0; i <= totalLength; i += Projectile.height)
            {
                int frame = 0;
                if (i > 0) frame++;
                if (i >= (totalLength - frameHeight)) frame++;
                Vector2 drawPos = start + unit * i - Main.screenPosition;
                Main.spriteBatch.Draw(
                    texture,
                    drawPos,
                    new Rectangle(0, frameHeight * frame, texture.Width, Projectile.height),
                    beamColor,
                    Projectile.rotation - MathHelper.PiOver2,
                    origin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0f
                );
            }

            return false; // We handled drawing
        }
    }
}
