using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;
using VanillaModding.Content.NPCs.TheChosenOne;

namespace VanillaModding.Content.Projectiles.Lobotomy
{
    internal class LobotomyLaser : ModProjectile
    {
        // The maximum brightness of the light emitted by the beams. Brightness scales from 0 to this value as the Prism's charge increases.
        private const float BeamLightBrightness = 0.75f;
        private const float BeamLength = 2400F*3; // Beam distance — adjust for range


        float actualBeamLength = 0f;
        public override void SetDefaults()
        {
            Projectile.width = 16; // Solid line width
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 3; // Short lifespan, update as needed
            Projectile.DamageType = DamageClass.Magic;
            Projectile.scale = 0.25f;
        }

        Vector2 velocityDirection;
        float rotationOffset = 0f;

        int alpha = 0;
        bool fire = false;
        public override void AI()
        {
            // Beam logic — set velocity direction or position, etc.
            if (Projectile.velocity.LengthSquared() > 0)
            {
                velocityDirection = Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
                rotationOffset = MathHelper.ToRadians(Main.rand.NextFloat(0f, 360f));
            }
            Projectile.rotation = velocityDirection.RotatedBy(rotationOffset).ToRotation();
            actualBeamLength = BeamLength;
            //Main.NewText($"{Projectile.ai[1]} | ID: {Projectile.whoAmI}");

            Vector2 beamDims = new Vector2(velocityDirection.Length() * BeamLength, Projectile.width * Projectile.scale);
            if (Main.netMode != NetmodeID.Server)
            {
                //ProduceWaterRipples(beamDims);
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] < (int)(60 * 0.5f))
            {
                int fadeDuration = (int)(60 * 0.5f);
                alpha = (int)(255f * Projectile.ai[1] / fadeDuration);
            }

            if (Projectile.ai[1] > (int)(60 * 1f))
            {
                if (!fire) SoundEngine.PlaySound(VanillaModdingSoundID.LobotomyLaserFire, Projectile.position);
                Projectile.hostile = true;
                fire = true;
                
            }

            if (Projectile.ai[1] > (int)(60 * 1f))
            {
                if (Projectile.ai[1] > (int)(60 * 1.1f)) Projectile.hostile = false;
                int fadeDuration = (int)(60 * 1.5f);
                alpha = 255 - (int)(255f * Projectile.ai[1] / fadeDuration);
                if (Projectile.ai[1] > (int)(60 * 1.5f)) Projectile.Kill();
            }

            if (alpha > 255)
                alpha = 255;
            if (alpha < 0)
                alpha = 0;


            rotationOffset += MathHelper.ToRadians(.05f); // Slowly rotate the beam over time
            //SoundEngine.PlaySound(SoundID.Item15, Projectile.position);
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

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host Prism), that's good enough.
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }

            // Otherwise, perform an AABB line collision check to check the whole beam.
            float _ = float.NaN;
            Vector2 unit = velocityDirection.RotatedBy(rotationOffset).SafeNormalize(Vector2.UnitX);
            Vector2 beamStart = Projectile.Center - unit * (actualBeamLength / 2f);
            Vector2 beamEnd = Projectile.Center + unit * (actualBeamLength / 2f);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamStart, beamEnd, (Projectile.width + 2) * Projectile.scale, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.LaserMachinegunLaser).Value;
            String pathBlast = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Blast").Replace(@"\", "/");
            Texture2D blast = (Texture2D)ModContent.Request<Texture2D>($"{pathBlast}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 unit = velocityDirection.RotatedBy(rotationOffset).SafeNormalize(Vector2.UnitX);

            Vector2 mid = Projectile.Center;
            Vector2 start = mid - unit * (actualBeamLength / 2f); // Start from one end of the symmetrical beam
            Color beamColor = new Color(255, 0, 0, 0); // Adjustable color

            // Define source rectangles for each frame
            int frameHeight = texture.Height / 3;
            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);

            float opacity = (alpha / 255f);
            float totalLength = (Projectile.height + actualBeamLength);
            for (float i = 0; i <= totalLength; i += Projectile.height)
            {
                Vector2 drawPos = start + unit * i - Main.screenPosition;
                Main.spriteBatch.Draw(
                    (fire) ? blast : texture,
                    drawPos,
                    null,
                    beamColor * opacity,
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
