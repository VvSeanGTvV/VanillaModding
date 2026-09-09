using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Projectiles.Tizona;

namespace VanillaModding.Content.Projectiles.Arrows
{
    internal class VulcanBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAMineThatDealsTripleDamageWhenStationary[Type] = true; // Deal triple damage when not moving and "armed".
            ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true; // Damage dealt to players does not scale with difficulty in vanilla.

            ProjectileID.Sets.Explosive[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10; // The width of projectile hitbox
            Projectile.height = 10; // The height of projectile hitbox
            Projectile.aiStyle = -1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 300; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
        }

        public override void AI()
        {
           if (Main.rand.NextBool(3))
           {
                var smokeDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, -Projectile.velocity.X * 0.25f, -Projectile.velocity.Y * 0.25f, 100, default, 1.75f);
                smokeDust.velocity *= 0.85f;
           }

            var fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, -Projectile.velocity.X * 0.25f, -Projectile.velocity.Y * 0.25f, 100, default, 1.25f);
            fireDust.noGravity = true;
            fireDust.velocity *= 0.98f;

            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.PrepareBombToBlow();
            }

            // Apply gravity after a quarter of a second
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 15f)
            {
                Projectile.ai[0] = 15f;
                Projectile.velocity.Y += 0.15f;
            }

            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 3;
            return false;
        }

        public override void PrepareBombToBlow()
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false; // This is important or the explosion will be in the wrong place if the mine explodes on slopes.
            Projectile.alpha = 255; // Make the mine invisible.

            // Resize the hitbox of the projectile for the blast "radius".
            // Rocket I: 128, Rocket III: 200, Mini Nuke Rocket: 250
            // Measurements are in pixels, so 128 / 16 = 8 tiles.
            Projectile.Resize(124, 124);
            // Set the knockback of the blast.
            // Rocket I: 8f, Rocket III: 10f, Mini Nuke Rocket: 12f
            Projectile.knockBack = 8f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(4) && !target.HasBuff(BuffID.Bleeding)) target.AddBuff(BuffID.Bleeding, 220);
            OnHitEntity(target);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.PvP)
            {
                if (Main.rand.NextBool(4) && !target.HasBuff(BuffID.Bleeding)) target.AddBuff(BuffID.Bleeding, 220);
                OnHitEntity(target);
            }
        }

        public void OnHitEntity(Entity target)
        {
            Vector2 velocity = Vector2.Normalize(target.Center - Projectile.Center) * 10f;
            if (Main.myPlayer == Projectile.owner) explodeBulb(target);
        }

        private void explodeBulb(Entity target)
        {
            var position = Projectile.position;
            var speedX = Projectile.velocity.X;
            var speedY = Projectile.velocity.Y;
            float speedMul = 1.5f;
            float numberProjectiles = 2; // 3 shots
            float rotation = MathHelper.ToRadians(45);//Shoots them in a 45 degree radius. (This is technically 90 degrees because it's 45 degrees up from your cursor and 45 degrees down)
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f; //45 should equal whatever number you had on the previous line
            var enS = Projectile.GetSource_FromThis();
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Vector for spread. Watch out for dividing by 0 if there is only 1 projectile.
                Projectile.NewProjectile(enS, position, perturbedSpeed * speedMul, ModContent.ProjectileType<DeadlyBulb>(), (int)(Projectile.damage / numberProjectiles), Projectile.knockBack, Projectile.owner, target.whoAmI); //Creates a new projectile with our new vector for spread.

            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer && (int)Projectile.ai[1] != Type)
            {
                int stored = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, (int)Projectile.ai[1], 1, 0, Projectile.owner);
                Main.projectile[stored].DamageType = DamageClass.Ranged;
                Main.projectile[stored].timeLeft = 2;
            }
            
            // Play an exploding sound.
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            // Resize the projectile again so the explosion dust and gore spawn from the middle.
            // Rocket I: 22, Rocket III: 80, Mini Nuke Rocket: 50
            Projectile.Resize(22, 22);

            // Spawn a bunch of smoke dusts.
            for (int i = 0; i < 30; i++)
            {
                var smokeDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                smokeDust.velocity *= 1.4f;
            }

            // Spawn a bunch of fire dusts.
            for (int j = 0; j < 20; j++)
            {
                var fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                fireDust.noGravity = true;
                fireDust.velocity *= 7f;
                fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                fireDust.velocity *= 3f;
            }

            // Spawn a bunch of smoke gores.
            for (int k = 0; k < 2; k++)
            {
                float speedMulti = 0.4f;
                if (k == 1)
                {
                    speedMulti = 0.8f;
                }

                var smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.velocity *= speedMulti;
                smokeGore.velocity += Vector2.One;
                smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.velocity *= speedMulti;
                smokeGore.velocity.X -= 1f;
                smokeGore.velocity.Y += 1f;
                smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.velocity *= speedMulti;
                smokeGore.velocity.X += 1f;
                smokeGore.velocity.Y -= 1f;
                smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.velocity *= speedMulti;
                smokeGore.velocity -= Vector2.One;
            }

            // To make the explosion destroy tiles, take a look at the commented out code in Example Rocket Projectile.
        }
    }
}
