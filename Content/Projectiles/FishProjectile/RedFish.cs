using Microsoft.Xna.Framework;
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

namespace VanillaModding.Content.Projectiles.FishProjectile
{
    internal class RedFish : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.alpha = 0;
            Projectile.light = 0f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;

            //AIType = ProjectileID.Bullet; // Act exactly like default Bullet

        }

        public override void AI()
        {
            Projectile.velocity.Y = Projectile.velocity.Y + 0.3f; // 0.1f for arrow gravity, 0.4f for knife gravity

            if (Projectile.velocity.Y > 32f) Projectile.velocity.Y = 32f;
            Projectile.rotation += MathHelper.ToRadians(15f) * Projectile.direction;
            //Projectile.damage += 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(VanillaModdingSoundID.FishSpeak, Projectile.position);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(VanillaModdingSoundID.FishHit, Projectile.position);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            SoundEngine.PlaySound(VanillaModdingSoundID.FishHit, Projectile.position);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit25, Projectile.position);
        }
    }
}
