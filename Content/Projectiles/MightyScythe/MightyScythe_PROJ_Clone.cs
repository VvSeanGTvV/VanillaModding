using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.External.AI;
using static Humanizer.In;

namespace VanillaModding.Content.Projectiles.MightyScythe
{
    public class MightyScythe_PROJ_Clone : ModProjectile
    {

        private float rotdef = 0f;
        public bool enemySide = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 92; // The width of projectile hitbox
            Projectile.height = 72; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = !enemySide; // Can the projectile deal damage to enemies?
            Projectile.hostile = enemySide; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = (int)Math.Round(int.MaxValue / 1.5); ; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame

            //Projectile.light = 1f;
            Projectile.alpha = 128;
            Projectile.scale = 0.75f;

            //AIType = ProjectileID.Bullet; // Act exactly like default Bullet

        }
        readonly float rotSpeed = 0.005f;
        readonly int beforeHome = 45; // Time before homing
        readonly int maxDebounce = 2; // How much it goes back to the target NPC

        private float ySpeed = 1f; // Y Vel before homing
        private float timer;
        private Vector2 lastMousePos;
        private SpriteEffects SF = SpriteEffects.None;

        public override void AI()
        {
            if (lastMousePos.Equals(Vector2.Zero)) lastMousePos = Main.MouseWorld;
            timer++;
            Lighting.AddLight(Projectile.position, 0.28f, 1f, 0.98f);
            rotdef += rotSpeed;

            Projectile.spriteDirection = Projectile.direction;
            if (Projectile.direction == -1)
            {
                SF = SpriteEffects.FlipHorizontally;
            }

            Projectile.rotation += rotdef * Projectile.direction;

            if (rotdef >= 360f) rotdef = 0f;

            NPC closestNPC = AdvAI.FindClosestNPC(2048f, Projectile);
            if (closestNPC == null)
                return;

            if (timer >= beforeHome && timer < beforeHome + 5) Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - closestNPC.Center).SafeNormalize(Vector2.Zero) * 40f, 0.25f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;


            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SF, 0);


            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D textureGlow = ModContent.Request<Texture2D>($"{nameof(VanillaModding)}/Content/Projectiles/MightyScythe/MightyScythe_PROJ_Glow", AssetRequestMode.ImmediateLoad).Value;
            Vector2 drawOrigin = new Vector2(textureGlow.Width * 0.5f, Projectile.height * 0.5f);

            Main.EntitySpriteDraw(textureGlow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SF, 0);

        }
    }
}