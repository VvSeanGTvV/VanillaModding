using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using ReLogic.Content;

namespace VanillaModding.Projectiles.OcramProjectile
{
    internal class RedLaser : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EyeLaser);
            AIType = ProjectileID.EyeLaser;

            Projectile.hostile = true;
            Projectile.tileCollide = false;

            Projectile.scale = 1.15f;
            Projectile.alpha = 255;

            Projectile.width = 6;

            Projectile.timeLeft = 900;
            Projectile.penetrate = -1;

            Projectile.light = 0.1f;
        }

        public override void AI()
        {
            if (Projectile.timeLeft <= 895) Projectile.alpha = 50;
            Lighting.AddLight(Projectile.Center, 0.957f / 1.5f, 0, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            String path = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_LightTrail").Replace(@"\", "/");
            Texture2D textureGlow = (Texture2D)ModContent.Request<Texture2D>($"{path}", AssetRequestMode.ImmediateLoad).Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            SpriteEffects effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (Projectile.timeLeft < 890)
            {
                for (int k = 0; k < Projectile.oldPos.Length - 1; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                    Color color = new Color(60 - k * 5, 10, 60 + k * 0, 40 + k * 0);
                    float rotation = (float)Math.Atan2(Projectile.oldPos[k].Y - Projectile.oldPos[k + 1].Y, Projectile.oldPos[k].X - Projectile.oldPos[k + 1].X);
                    spriteBatch.Draw(textureGlow, drawPos, null, color, rotation, drawOrigin, (Projectile.scale - k / (float)Projectile.oldPos.Length) * 0.75f, effects, 0f);
                    spriteBatch.Draw(textureGlow, drawPos - Projectile.oldPos[k] * 0.5f + Projectile.oldPos[k + 1] * 0.5f, null, color, rotation, drawOrigin, (Projectile.scale - k / (float)Projectile.oldPos.Length) * 0.75f, effects, 0f);
                }
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
            => new Color(255, 255, 255, 200);
    }
}