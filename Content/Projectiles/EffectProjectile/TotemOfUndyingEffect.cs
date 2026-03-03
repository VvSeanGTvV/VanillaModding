using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
using VanillaModding.Common.Utilities;
using VanillaModding.Content.Dusts;

namespace VanillaModding.Content.Projectiles.EffectProjectile
{
    internal class TotemOfUndyingEffect : ModProjectile
    {
        int originalTime = 60 * 1;
        public override void SetDefaults()
        {
            
            Projectile.width = 16; // The width of projectile hitbox
            Projectile.height = 28; // The height of projectile hitbox
            Projectile.aiStyle = -1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1;
            Projectile.timeLeft = originalTime;
            Projectile.alpha = 0;
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.scale = 1f;
            //Projectile.light = 1f;
        }

        float multiplier = 0;
        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            float a = (1 - (Projectile.timeLeft / originalTime));
            multiplier = 1 + (a * 5);

            Projectile.rotation += MathHelper.ToRadians((float)Math.Pow(10f, a*2f));
            base.AI();
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(VanillaModdingSoundID.DeathNoteItemAsylum, Projectile.Center);
            // Spawn particles
            SpawnHelper.SpawnCircleDust(
                Projectile.Center,
                Color.LightGoldenrodYellow,
                ModContent.DustType<ColorableDust>(),
                20,
                10
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);

            Vector2 position = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(texture, position, null, lightColor * multiplier, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f); // Main Body
            return false;
        }
    }
}
