using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace VanillaModding.Projectiles.MightyScythe.MightyProjectile
{
    internal class MightyProjectile : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 92; // The width of projectile hitbox
            Projectile.height = 72; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = (int)Math.Round(int.MaxValue / 1.5); ; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame

            //Projectile.light = 1f;
            //Projectile.alpha = 100;

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
            lastMousePos = new Vector2(Projectile.ai[0], Projectile.ai[1]);
            timer++;

            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation += 0.25f * Projectile.direction;

            if (Projectile.ai[2] == 0){
                Lighting.AddLight(Projectile.position, 0.28f, 1f, 0.98f);

                ySpeed += 0.1f;
                if (timer < beforeHome) Projectile.velocity = new Vector2(0, -ySpeed);
                if (timer >= beforeHome && timer <= beforeHome + 5)
                {
                    Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - lastMousePos).SafeNormalize(Vector2.Zero) * 40f, 0.25f);
                }
            }
            if (Projectile.ai[2] == 1)
            {
                Lighting.AddLight(Projectile.Center, new Vector3(0.769f, 0.294f, 0.325f));

                if (Projectile.alpha <= 140)
                {
                    Projectile.alpha += 1;
                }
                if (Projectile.alpha > 140)
                {

                    Projectile.alpha += 15;

                }
                Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - lastMousePos).SafeNormalize(Vector2.Zero) * 40f, 0.0075f);
            }
            if (Projectile.ai[2] == 2)
            {
                Lighting.AddLight(Projectile.Center, new Vector3(0.478f, 0.282f, 1f));

                if (Projectile.alpha <= 140)
                {
                    Projectile.alpha += 1;
                }
                if (Projectile.alpha > 140)
                {

                    Projectile.alpha += 15;

                }
                Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - lastMousePos).SafeNormalize(Vector2.Zero) * 40f, 0.0075f);
            }
            if (Projectile.ai[2] == 3)
            {
                Lighting.AddLight(Projectile.Center, new Vector3(0.569f, 0.871f, 1f));

                Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - lastMousePos).SafeNormalize(Vector2.Zero) * 40f, 0.005f);
            }
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            } 
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[2] == 0)
            {
                explodeScythe(target);
                Projectile.Kill();
            }
            Projectile.Kill();
        }

        public void explodeScythe(NPC npc)
        {
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
                i++;
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Vector for spread. Watch out for dividing by 0 if there is only 1 projectile.
                Projectile.NewProjectile(enS, new Vector2(position.X, position.Y), new Vector2(perturbedSpeed.X, perturbedSpeed.Y) * speedMul, ModContent.ProjectileType<MightyProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner,npc.Center.X, npc.Center.Y, i); //Creates a new projectile with our new vector for spread.
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color defColor;
            if (Projectile.ai[2] == 2 || Projectile.ai[2] == 1)
            {
                if (Projectile.timeLeft >= 50) //it'll start to fade in when timeLeft is >= 50
                {
                    defColor = new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 100);
                }
                byte num4 = (byte)(Projectile.timeLeft * 3);
                byte num5 = (byte)(100.0 * ((double)num4 / (double)byte.MaxValue));
                defColor = new Color((int)num4, (int)num4, (int)num4, (int)num5);
            }
            else
            {
                defColor = new Color(255, 255, 255, 100);
            }
            String path = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_" + Projectile.ai[2].ToString()).Replace(@"\", "/");
            Texture2D textureNew = (Texture2D)ModContent.Request<Texture2D>($"{path}", AssetRequestMode.ImmediateLoad).Value;
            Vector2 drawOrigin = new Vector2(textureNew.Width * 0.5f, textureNew.Height * 0.5f);
            Projectile.width = textureNew.Width;
            Projectile.height = textureNew.Height;

            Main.EntitySpriteDraw(textureNew, Projectile.Center - Main.screenPosition, null, defColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
