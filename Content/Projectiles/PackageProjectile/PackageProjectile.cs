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
            Projectile.timeLeft = 1200; // 300 = 5 seconds (60 ticks -> 1s)
            Projectile.penetrate = -1; // Infinite penetration so that the blast can hit all enemies within its radius.
        }

        public override void AI()
        {
            // Arrow Gravity
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 15f)
            {
                Projectile.ai[0] = 15f;
                Projectile.velocity.Y += 0.1f;
            }
            if (Projectile.velocity.Y > 16f) Projectile.velocity.Y = 16f;
            if (Projectile.velocity.X > -0.1f && Projectile.velocity.X < 0.1f) Projectile.velocity.X = 0f;
            Projectile.velocity.X *= 0.985f;

            float spinSpeed = MathHelper.Clamp(Projectile.velocity.X * 0.01f, -0.25f, 0.25f);
            Projectile.rotation += spinSpeed;

            foreach (Projectile otherProjectile in Main.ActiveProjectiles)
            {
                if (otherProjectile.whoAmI == Projectile.whoAmI) continue; //Skip this projectile caue why?
                if (otherProjectile.getRect().Intersects(Projectile.getRect()))
                {
                    SpawnRandomItems(3);
                    otherProjectile.Kill();
                    Projectile.Kill();
                }
            }
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

        public void SpawnRandomItems(int numberOfItems)
        {
            // Create a list of tuples: (itemID, weight)
            List<(int itemID, float weight)> weightedItems = new List<(int, float)>();

            foreach (var kv in ContentSamples.ItemsByType)
            {
                int itemID = kv.Key;
                Item item = kv.Value;

                // Skip invalid items
                if (itemID <= 0 || item.IsAir || item.maxStack <= 0 || item.DamageType == DamageClass.Default)
                    continue;

                // Convert rarity to weight: common items have higher weight
                // Clamp rare value to a reasonable range to avoid extreme weights
                int rare = item.rare;
                float weight;
                switch (rare)
                {
                    case -1:  // Gray
                        weight = 10f;
                        break;
                    case 0:   // White (common)
                        weight = 5f;
                        break;
                    case 1:
                    case 2:   // Blue / Uncommon
                        weight = 2f;
                        break;
                    case 3:
                    case 4:   // Green / Rare-ish
                        weight = 1f;
                        break;
                    case 5:
                    case 6:
                    case 7:   // Orange / Light red
                        weight = 0.5f;
                        break;
                    default:  // very rare / expert / special
                        weight = 0.2f;
                        break;
                }

                if (rare >= 4 && !Main.hardMode) continue; // Prevent hardmode items from dropping in pre-hardmode
                if (rare >= 7) // Prevent Plantera/Golem items from dropping in pre-hardmode or pre-Plantara
                {
                    if (!Main.hardMode) continue; // Just in case
                    if (!NPC.downedGolemBoss) continue;
                    if (!NPC.downedPlantBoss) continue;
                }
                if (rare >= 9) // Prevent Lunar Event items from dropping in pre-hardmode or pre-Moon Lord
                {
                    if (!Main.hardMode) continue; // Just in case
                    if (!NPC.downedTowers) continue;
                }
                if (rare >= 9) // Prevent Moon Lord items from dropping in pre-hardmode or pre-Moon Lord
                {
                    if (!Main.hardMode) continue; // Just in case
                    if (!NPC.downedTowers) continue;
                    if (!NPC.downedMoonlord) continue;
                }
                if (rare == -12 && !Main.expertMode) continue; // Prevent expert mode exclusive items from dropping in normal mode
                if (rare == -13 && !Main.masterMode) continue; // Prevent master mode exclusive items from dropping in normal mode
                if (rare == -11) continue; // Prevent quest items from dropping :D
                weightedItems.Add((itemID, weight));
            }

            int ChooseWeightedItem(List<(int itemID, float weight)> items)
            {
                float totalWeight = 0f;
                foreach (var pair in items)
                    totalWeight += pair.weight;

                float rand = Main.rand.NextFloat() * totalWeight;
                foreach (var pair in items)
                {
                    rand -= pair.weight;
                    if (rand <= 0f)
                        return pair.itemID;
                }

                // Fallback (shouldn't happen)
                return items[Main.rand.Next(items.Count)].itemID;
            }


            var source = Projectile.GetSource_FromAI();
            // Now spawn a few random items
            for (int i = 0; i < numberOfItems; i++)
            {
                int randomID = ChooseWeightedItem(weightedItems);
                int spawned = Item.NewItem(source, Projectile.getRect(), randomID);

                if (Main.item[spawned] is Item spawnedItem)
                {
                    spawnedItem.velocity = new Vector2(Main.rand.NextFloat(-4f, 4f) - Projectile.velocity.X, Main.rand.NextFloat(-4f, -1f));
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SpawnRandomItems(3);
            Projectile.Kill();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            base.OnKill(timeLeft);
        }
    }
}
