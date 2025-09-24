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
using static Humanizer.In;

namespace VanillaModding.Content.Projectiles.MightyScythe
{
    public class MightyScythe_PROJ : ModProjectile
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

            //AIType = ProjectileID.Bullet; // Act exactly like default Bullet

        }
        readonly float rotSpeed = 0.005f;
        readonly int beforeHome = 45; // Time before homing
        readonly int maxDebounce = 2; // How much it goes back to the target NPC

        private float ySpeed = 1f; // Y Vel before homing
        private float timer;
        private Vector2 lastMousePos;

        public override void AI()
        {
            if (lastMousePos.Equals(Vector2.Zero)) lastMousePos = Main.MouseWorld;
            timer++;
            Lighting.AddLight(Projectile.position, 0.28f, 1f, 0.98f);
            rotdef += rotSpeed;

            Projectile.spriteDirection = Projectile.direction;
            

            Projectile.rotation += rotdef * Projectile.direction;

            if (rotdef >= 360f) rotdef = 0f;
            ySpeed += 0.1f;
            if (timer < beforeHome) Projectile.velocity = new Vector2(0, -ySpeed);
            if (timer >= beforeHome && timer <= beforeHome + 5)
            {
                //if (Projectile.Center.Distance(this.lastMousePos) < 256f) this.mul = Projectile.Center.Distance(this.lastMousePos)/50f;
                Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - lastMousePos).SafeNormalize(Vector2.Zero) * 40f, 0.25f);

            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects SF = SpriteEffects.None;
            if (Projectile.direction == -1)
            {
                SF = SpriteEffects.FlipHorizontally;
            }
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
            explodeScythe();
            Projectile.Kill();
        }

        public void explodeScythe()
        {
            int ded = 0;
            var position = Projectile.position;
            var speedX = Projectile.velocity.X;
            var speedY = Projectile.velocity.Y;
            float speedMul = 2f;
            float numberProjectiles = 3; // 3 shots
            float rotation = MathHelper.ToRadians(45);//Shoots them in a 45 degree radius. (This is technically 90 degrees because it's 45 degrees up from your cursor and 45 degrees down)
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f; //45 should equal whatever number you had on the previous line
            var enS = Projectile.GetSource_FromThis();
            int i = 0;
            while (i < numberProjectiles)
            {
                ded++;
                if (ded > 1000f)
                {
                    i++;
                    Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Vector for spread. Watch out for dividing by 0 if there is only 1 projectile.
                    Projectile.NewProjectile(enS, new Vector2(position.X, position.Y), new Vector2(perturbedSpeed.X, perturbedSpeed.Y) * speedMul, ModContent.ProjectileType<MightyScythe_PROJ_Clone>(), Projectile.damage * 2, Projectile.knockBack * 2, Projectile.owner); //Creates a new projectile with our new vector for spread.
                    ded = 0;
                }
            }
        }

        public override void PostDraw(Color lightColor)
        {
            SpriteEffects SF = SpriteEffects.None;
            if (Projectile.direction == -1)
            {
                SF = SpriteEffects.FlipHorizontally;
            }
            Texture2D textureGlow = ModContent.Request<Texture2D>($"{nameof(VanillaModding)}/Projectiles/MightyScythe/MightyScythe_PROJ_Glow", AssetRequestMode.ImmediateLoad).Value;
            Vector2 drawOrigin = new Vector2(textureGlow.Width * 0.5f, Projectile.height * 0.5f);

            Main.EntitySpriteDraw(textureGlow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SF, 0);

        }
    }
}