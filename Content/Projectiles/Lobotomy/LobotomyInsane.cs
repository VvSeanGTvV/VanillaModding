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
using Terraria.DataStructures;
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
            if (Projectile.ai[0] == 0)
            {
                if (Projectile.velocity.LengthSquared() > 0)
                {
                    velocityDirection = Projectile.velocity;
                    Projectile.velocity = Vector2.Zero;
                    rotationOffset = 0f;
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
                    Vector2 unit = velocityDirection.RotatedBy(rotationOffset).SafeNormalize(Vector2.UnitX);
                    Vector2 beamStart = Projectile.Center - unit * (actualBeamLength / 2f);
                    Vector2 beamEnd = Projectile.Center + unit * (actualBeamLength / 2f);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            Vector2 offset = new Vector2(Main.rand.Next(-500, 500), 360f);
                            int projectile = Projectile.NewProjectile(Projectile.GetSource_FromThis(), offset, new Vector2(10, 0), ModContent.ProjectileType<LobotomyInsane>(), 20, 5, -1, 1);
                            Main.projectile[projectile].timeLeft = 60 * 5;

                            int fadeDuration = (int)(60 * 1.5f);
                            alpha = 255 - (int)(255f * Projectile.ai[1] / fadeDuration);
                            if (Projectile.ai[1] > (int)(60 * 1.5f)) Projectile.Kill();
                        }
                    }
                }
            }
            else
            {
                alpha = 255;
                Projectile.hostile = true;
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
                Main.spriteBatch.Draw(texture, position, null, new Color(255, 255, 255, 0), 0f, origin, Projectile.scale + Main.rand.NextFloat(-0.5f, 0.5f), SpriteEffects.None, 0f); // Main Body
            }


            return false; // We handled drawing
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(VanillaModdingSoundID.LobotomyInsane, Projectile.position);

        }
    }
}
