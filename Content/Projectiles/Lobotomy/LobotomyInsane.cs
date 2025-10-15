using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;

namespace VanillaModding.Content.Projectiles.Lobotomy
{
    internal class LobotomyInsane : ModProjectile
    {
        // The maximum brightness of the light emitted by the beams. Brightness scales from 0 to this value as the Prism's charge increases.
        private const float BeamLightBrightness = 0.75f;
        private const float BeamLength = 2400F * 3; // Beam distance — adjust for range


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
            //Projectile.timeLeft = 60 * 3; // Short lifespan, update as needed
            Projectile.DamageType = DamageClass.Magic;
            Projectile.scale = 0.5f;
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
                rotationOffset = 90f;
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
                
            }

            if (alpha > 255)
                alpha = 255;
            if (alpha < 0)
                alpha = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.LaserMachinegunLaser).Value;
            String pathSide = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Side").Replace(@"\", "/");
            Texture2D side = (Texture2D)ModContent.Request<Texture2D>($"{pathSide}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 unit = velocityDirection.RotatedBy(rotationOffset).SafeNormalize(Vector2.UnitX);

            Vector2 mid = Projectile.Center;
            Vector2 start = mid - unit * (actualBeamLength / 2f); // Start from one end of the symmetrical beam
            Color beamColor = new Color(255, 185, 239, 0); // Adjustable color

            // Define source rectangles for each frame
            int frameHeight = side.Height;
            Vector2 origin = new Vector2(side.Width / 2f, frameHeight / 2f);
            if (Projectile.ai[0] == 0)
            {
                float opacity = (alpha / 255f);
                float totalLength = (Projectile.height + actualBeamLength);
                for (float i = 0; i <= totalLength; i += Projectile.height)
                {
                    Vector2 drawPos = start + unit * i - Main.screenPosition;
                    Main.spriteBatch.Draw(
                        side,
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

                for (float i = 0; i <= totalLength; i += Projectile.height)
                {
                    Vector2 drawPos = start + unit * i - Main.screenPosition;
                    Main.spriteBatch.Draw(
                        side,
                        drawPos,
                        null,
                        beamColor * opacity,
                        Projectile.rotation + MathHelper.PiOver2,
                        origin,
                        Projectile.scale,
                        SpriteEffects.None,
                        0f
                    );
                }
            } 
            else
            {
                Vector2 position = Projectile.Center - Main.screenPosition;
                Main.spriteBatch.Draw(texture, position, null, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f); // Main Body
            }


                return false; // We handled drawing
        }
    }
}
