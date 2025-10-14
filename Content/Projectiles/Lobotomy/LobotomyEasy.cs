using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;

namespace VanillaModding.Content.Projectiles.Lobotomy
{
    internal class LobotomyEasy : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
        }

        public int maxBounce = 4;
        public override void SetDefaults()
        {
            Projectile.width = 200; // The width of projectile hitbox
            Projectile.height = 200; // The height of projectile hitbox

            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 0f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 60 * 999; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.penetrate = -1;
        }
        
        public int bounce = 0;
        public float value = -1f;
        public Vector2 startPosition;

        public override void OnSpawn(IEntitySource source)
        {
            startPosition = Projectile.position;
            SoundEngine.PlaySound(VanillaModdingSoundID.LobotomyEasySpell, Projectile.position);
        }

        public override void AI()
        {
            if (value == -1f) SoundEngine.PlaySound(VanillaModdingSoundID.LobotomyEasy, Projectile.position);
            value += 0.05f / 2.75f;  // increase value each tick

            if (value > 1f)
            {
                value = -1f;
                float lastPositionY = Projectile.position.Y;
                Projectile.position = new Vector2(startPosition.X, lastPositionY);  // reset position on each bounce cycle
                SoundEngine.PlaySound(VanillaModdingSoundID.LobotomyEasyBounce, Projectile.position);
                PunchCameraModifier modifier = new(Projectile.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, FullName);
                Main.instance.CameraModifiers.Add(modifier);
                bounce++;
            }

            float velocityFactor = value < 0 ? value : 1 - (value - 1) * (value - 1);
            Projectile.velocity = new Vector2(-7f * velocityFactor * Projectile.ai[0], -7);
            Projectile.rotation += MathHelper.ToRadians((-7 / 2.75f) * Projectile.ai[0]);
            if (bounce >= maxBounce + 1) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            String pathGlow = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Glow").Replace(@"\", "/");
            //String pathGlowTrail = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Glow_Trail").Replace(@"\", "/");

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = (Texture2D)ModContent.Request<Texture2D>($"{pathGlow}", AssetRequestMode.ImmediateLoad).Value;
           // Texture2D glowTrail = (Texture2D)ModContent.Request<Texture2D>($"{pathGlowTrail}", AssetRequestMode.ImmediateLoad).Value;
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);
            Vector2 originGlow = new(glow.Width / 2, glow.Height / 2);

            Vector2 position = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(glow, position, null, new Color(0, 0, 255, 0), 0f, originGlow, Projectile.scale - 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow, position, null, new Color(0, 187/2, 255, 0), 0f, originGlow, Projectile.scale - 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow, position, null, new Color(0, 187, 255, 0), 0f, originGlow, Projectile.scale - 0.8f, SpriteEffects.None, 0f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                float c = k / (float)Projectile.oldPos.Length;
                float g = 1f - c;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(texture, drawPos, null, new Color(0, 187/2, 255, 0) * g, Projectile.oldRot[k] + MathHelper.PiOver2, origin, Projectile.scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, drawPos, null, new Color(0, 187, 255, 0) * g, Projectile.oldRot[k] + MathHelper.PiOver2 + MathHelper.ToRadians(5f), origin, Projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, position, null, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f); // Main Body

            return false;
        }
    }
}
