using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using VanillaModding.Content.Buffs;
using VanillaModding.External.AI;

namespace VanillaModding.Content.Projectiles.DiceProjectile
{
    internal class DiceProjectile : ModProjectile
    {
        public readonly int maxRollTime = 60 * 10;
        public readonly int maxDisplayTime = 60 * 3;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30; // The width of projectile hitbox
            Projectile.height = 30; // The height of projectile hitbox
            Projectile.aiStyle = -1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60 * 999;
            Projectile.alpha = 0;
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.scale = 1f;
            //Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
            //AIType = ProjectileID.Bullet; // Act exactly like default Bullet
        }

        int timer = 0;
        int mult = 0;
        int waut = 0;
        public override void AI()
        {
            timer++;
            Player player = Main.player[Projectile.owner];
            if (timer <= maxRollTime)
            {
                waut++;
                if (waut >= timer / (maxRollTime / 5))
                {
                    Projectile.frame = mult = Main.rand.Next(0, 6);
                    waut = 0;
                }
            } 
            else if (!player.HasBuff(ModContent.BuffType<DiceBuff>()))
            {
                DynamicDiceBuff modPlayer = player.GetModPlayer<DynamicDiceBuff>();
                modPlayer.DiceMult = mult + 1;
                player.AddBuff(ModContent.BuffType<DiceBuff>(), 60 * 30);
                
            }
            if (timer > maxRollTime + maxDisplayTime)
            {
                Projectile.Kill();
            }

            if (Projectile.velocity.X > -0.1f && Projectile.velocity.X < 0.1f) Projectile.velocity.X = 0f;
            if (Projectile.velocity.Y > -0.1f && Projectile.velocity.Y < 0.1f) Projectile.velocity.Y = 0f;
            Projectile.velocity *= 0.75f; // Make it slow down.

            Projectile.rotation = 0;
        }
    }
}
