using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common;
using VanillaModding.Common.GlobalNPCs;
using VanillaModding.External.AI;
using static System.Net.Mime.MediaTypeNames;

namespace VanillaModding.Content.Projectiles.DiceProjectile
{
    internal class DiceThrowableProjectile : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6 * 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30; // The width of projectile hitbox
            Projectile.height = 30; // The height of projectile hitbox
            Projectile.aiStyle = -1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60 * 5;
            Projectile.alpha = 0;
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.scale = 1.15f;
            Projectile.light = 1f;
            //Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
            //AIType = ProjectileID.Bullet; // Act exactly like default Bullet
        }

        double d;
        bool f = true;
        public override void AI()
        {
            d += 0.1d;
            Projectile.frame = (int)Math.Round(d);

            Projectile.velocity.Y = Projectile.velocity.Y + 0.15f; // 0.1f for arrow gravity, 0.4f for knife gravity

            if (Projectile.velocity.Y > 32f) Projectile.velocity.Y = 32f;
            Projectile.rotation += MathHelper.ToRadians(15f) * Projectile.direction;

            NPC closest = AdvAI.FindClosestNPC(50f, Projectile, npc =>
            {
                if (npc == null) return false;
                var f = npc.GetGlobalNPC<VanillaModdingNPC>();
                return f.rolling || f.hasAnyDiceEffect;
            });
            if (closest != null)
            {
                var d = closest.GetGlobalNPC<VanillaModdingNPC>();
                if (closest.getRect().Intersects(Projectile.getRect()))
                {
                    f = false;
                    Projectile.NewProjectile(closest.GetSource_FromAI(), closest.Center, Vector2.Zero, ModContent.ProjectileType<DiceProjectile>(), 0, 0, -1, closest.whoAmI, Projectile.ai[1]);
                    Projectile.Kill();
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            // Icy Blue Fire Dust spawn
            for (int i = 0; i < 40; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, Projectile.velocity.X/2, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, Projectile.velocity.X/2, 0f, 100, default, 2f);
                dust.velocity *= 3f;
            }

            if (Projectile.owner == Main.myPlayer && f)
            {
                Item.NewItem(Projectile.GetSource_DropAsItem(), (int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height, (int)Projectile.ai[1]);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            bool hittable;
            VanillaModdingNPC modNPC = target.GetGlobalNPC<VanillaModdingNPC>();
            hittable = !modNPC.rolling; // always never give another roll if an NPC is rolling.
            return hittable ? null : false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CanHit(Projectile.position, projHitbox.Width, projHitbox.Height, targetHitbox.Center(), 1, 1);
        }

        public override bool CanHitPvp(Player target)
        {
            bool hittable;
            VanillaModdingPlayer modPlayer = target.GetModPlayer<VanillaModdingPlayer>();
            hittable = !modPlayer.rolling; // always never give another roll if an NPC is rolling.
            return base.CanHitPvp(target) && hittable;
        }
    }
}
