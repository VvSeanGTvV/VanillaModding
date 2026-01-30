using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;
using VanillaModding.Content.Items.Materials;
using VanillaModding.Content.Items.Weapon.Throwable.Lobotomy;

namespace VanillaModding.Content.Projectiles.Lobotomy
{
    public class LobotomyNormal : ModProjectile
    {

        private float rotdef = 0f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 32; // The width of projectile hitbox
            Projectile.height = 32; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame

            //AIType = ProjectileID.Bullet; // Act exactly like default Bullet

        }

        int penetrateLeft = 3;
        int timer = 0;
        public override void AI()
        {
            
            //Projectile.penetrate = -1;
            Projectile.rotation += MathHelper.ToRadians(15f) * Projectile.direction;

            if (Projectile.ai[0] >= 0)
            {
                Projectile.timeLeft = 2;
                Player owner = Main.player[Projectile.owner];

                Vector2 directionToPlayer = owner.Center - Projectile.Center;
                float distance = directionToPlayer.Length();
                if (distance >= 550f) Projectile.ai[0] = 1f;
                if (Projectile.ai[0] >= 1f)
                {
                    Projectile.tileCollide = false;

                    float returnSpeed = 14f;
                    float inertia = 20f;

                    // Kill when close enough
                    if (distance < 20f)
                    {
                        //Item.NewItem(Projectile.GetSource_FromAI(), Projectile.position, ModContent.ItemType<LobotomyThrowable>());
                        Projectile.Kill();
                        return;
                    }

                    directionToPlayer.Normalize();
                    directionToPlayer *= returnSpeed;

                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + directionToPlayer) / inertia;
                    timer++;
                    if (timer >= 5)
                    {
                        Projectile.damage++;
                        timer = 0;
                    }
                }
                //Projectile.damage += 1;
            }
            else
            {
                if (Projectile.ai[0] == -1)
                {
                    Projectile.penetrate = penetrateLeft;
                    Projectile.ai[0]--;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] >= 0)
            {
                penetrateLeft--;
                if (penetrateLeft <= 0) Projectile.ai[0] = 1f;
            }
            else
            {
                Projectile.penetrate--;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // If collide with tile, reduce the penetrate.
            // So the projectile can reflect at most 5 times
            if (Projectile.ai[0] >= 0)
            {
                penetrateLeft--;
                if (penetrateLeft <= 0) Projectile.ai[0] = 1f;
            }
            else
            {
                Projectile.penetrate--;
            }

            int p = (Projectile.ai[0] >= 0) ? penetrateLeft : Projectile.penetrate; 
            if (p <= 0)
            {
                if (Projectile.ai[0] >= 0) Projectile.ai[0] = 1f; else Projectile.Kill();
            }
            else
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                //SoundEngine.PlaySound(VanillaModdingSoundID.FireInTheHoleHigh, Projectile.position);

                // If the projectile hits the left or right side of the tile, reverse the X velocity
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }

                // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }

            return false;
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
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void OnKill(int timeLeft)
        {

            // This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            //SoundEngine.PlaySound(VanillaModdingSoundID.FireInTheHoleHigh, Projectile.position);
        }

        public override void OnSpawn(IEntitySource data)
        {
            SoundEngine.PlaySound(VanillaModdingSoundID.FireInTheHoleHigh, Projectile.position);
        }
    }
}