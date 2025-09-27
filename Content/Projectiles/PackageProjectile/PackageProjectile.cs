using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Projectiles.PackageProjectile
{
    internal class PackageProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.damage = 5;
            Projectile.timeLeft = 300; // 300 = 5 seconds (60 ticks -> 1s)
            Projectile.penetrate = -1; // Infinite penetration so that the blast can hit all enemies within its radius.
        }

        public override void AI()
        {
            Projectile.timeLeft = 300;
            if (Projectile.velocity.Y <= 10f) Projectile.velocity.Y += 0.25f;
            if (Projectile.velocity.X > -0.1f && Projectile.velocity.X < 0.1f) Projectile.velocity.X = 0f;
            Projectile.velocity.X *= 0.98f;
            //Projectile.rotation = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + Projectile.ai[0];
            float spinSpeed = MathHelper.Clamp(Projectile.velocity.X * 0.01f, -0.25f, 0.25f);
            Projectile.rotation += spinSpeed;
        }

        /*public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Confused, 60 * 30);
            target.Hurt(PlayerDeathReason.ByProjectile(target.))
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Confused, 60 * 30);
            target.HitEffect(damageDone, 1.0);
        }*/

        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            // Get a list of ALL valid item IDs (vanilla + modded)
            List<int> validItemIDs = new List<int>();

            foreach (var kv in ContentSamples.ItemsByType)
            {
                int itemID = kv.Key;
                Item item = kv.Value;

                // skip unimplemented or useless items
                if (itemID <= 0 || item.IsAir || item.maxStack <= 0 || item.DamageType == DamageClass.Default)
                    continue;
                validItemIDs.Add(itemID);
            }

            var source = Projectile.GetSource_FromAI();
            // Now spawn a few random items
            for (int i = 0; i < 3; i++)
            {
                int randomID = validItemIDs[Main.rand.Next(validItemIDs.Count)];
                int spawned = Item.NewItem(source, Projectile.getRect(), randomID);

                if (Main.item[spawned] is Item spawnedItem)
                {
                    spawnedItem.velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-4f, -1f));
                }
            }
            Projectile.Kill();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}
