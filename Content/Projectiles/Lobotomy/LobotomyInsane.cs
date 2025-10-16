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
            Projectile.width = 100;
            Projectile.height = 100;
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
        float rotationOffset = 0f, scaleOffset = 0f;

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
                    if (fire == false) SoundEngine.PlaySound(VanillaModdingSoundID.LobotomyInsane, Projectile.position);
                    fire = true;
                    for (int i = 0; i < 12; i++)
                    {
                        Vector2 offset = new Vector2(Main.rand.Next(-250, 250)*0.5f, 760f+ Main.rand.Next(-150, 150));
                        if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool())
                        {
                            int projectile = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position - offset.RotatedBy(Projectile.rotation - MathHelper.PiOver2), new Vector2(0, 50).RotatedBy(Projectile.rotation - MathHelper.PiOver2), ModContent.ProjectileType<LobotomyInsane>(), 20, 5, -1, 1);
                            Main.projectile[projectile].timeLeft = 60 * 10;
                        }

                        int fadeDuration = (int)(60 * 2f);
                        alpha = 255 - (int)(255f * Projectile.ai[1] / fadeDuration);
                        if (Projectile.ai[1] > (int)(60 * 2f)) Projectile.Kill();
                    }
                }
            }
            else
            {
                alpha = 255;
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
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
            int frameHeight = 16;
            Vector2 origin = new Vector2(side.Width / 2f, frameHeight / 2f);
            Vector2 originA = new Vector2(texture.Width / 2f, texture.Height / 2f);
            if (Projectile.ai[0] == 0)
            {
                float opacity = (alpha / 255f);
                float totalLength = (frameHeight + actualBeamLength);
                for (float i = 0; i <= totalLength; i += frameHeight / 2f)
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

                for (float i = 0; i <= totalLength; i += frameHeight / 2f)
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
                Main.spriteBatch.Draw(texture, position, null, new Color(255, 255, 255, 0), Projectile.rotation, originA, Projectile.scale + scaleOffset, SpriteEffects.None, 0f); // Main Body
            }


            return false; // We handled drawing
        }

        public override void OnSpawn(IEntitySource source)
        {
            scaleOffset = Main.rand.NextFloat(-0.5f, 0.5f);
            
        }
    }
}
