using Humanizer;
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
using VanillaModding.Common.GlobalNPCs;
using VanillaModding.Common.Systems;

using VanillaModding.External.AI;

namespace VanillaModding.Content.NPCs.Fish
{
    internal class RedFish : ModNPC
    {
        // Sway Related NPC
        float maxSwayRotation = 0.2f; // Maximum rotation in radians (~11.5 degrees)
        float swaySpeed = 0.1f; // How quickly the rotation adjusts

        // Timer Related NPC
        private float bobTimer;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            Main.npcCatchable[Type] = true;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
            NPCID.Sets.TownCritter[Type] = true;
        }

        
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 22;

            NPC.defense = 5;
            NPC.lifeMax = 45000;
            NPC.catchItem = ModContent.ItemType<Items.Weapon.Throwable.Redfish.RedFish>();

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 0, 10);
            NPC.aiStyle = -1;

        }

        int timerTile = 0;
        int si = 0;
        bool once;
        public override void AI()
        {
            if (NPC.ai[0] > 0)
            {
                NPC.GetGlobalNPC<VanillaModdingNPC>().aggroTo = (int)NPC.ai[0] - 1;
                NPC.life = (int)NPC.ai[1];
            }
            si = (NPC.wet) ? 0 : 2;
            if (NPC.wet)
            {
                bobTimer += 0.05f;
                float bobAmount = (float)Math.Sin(bobTimer) * 0.5f;
                if (Collision.SolidCollision(new Vector2(NPC.Center.X, NPC.Bottom.Y), NPC.width, NPC.height)) bobAmount = -1;
                if (Collision.SolidCollision(new Vector2(NPC.position.X + (NPC.spriteDirection * 4), NPC.position.Y), NPC.width, NPC.height)) FlipFish(); else if (once) timerTile++;

                if (timerTile >= 60 * 3)
                {
                    timerTile = 0;
                    once = false;
                }
                NPC.velocity = new Vector2(NPC.spriteDirection * 1.2f, bobAmount);
            } 
            else
            {
                if (NPC.velocity.Y == 0f) 
                {
                    NPC.velocity.X = Main.rand.NextFloat(-1f, 1f) * 1.8f;
                    float targetRotation = NPC.velocity.X * 0.05f;
                    targetRotation = MathHelper.Clamp(targetRotation, -maxSwayRotation, maxSwayRotation);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, swaySpeed);
                    NPC.velocity.Y = -4f;
                };
            }

            int aggroTo = NPC.GetGlobalNPC<VanillaModdingNPC>().aggroTo;
            if (aggroTo >= 0)
            {
                //Main.NewText($"aaa {aggroTo}");
                var nearPlayer = AdvAI.FindClosestPlayer(175f, NPC.position, plr => plr.whoAmI != aggroTo);
                if (nearPlayer != null && (nearPlayer.Center.Distance(NPC.Center) > 100f))
                {
                    Vector2 start = NPC.Center;
                    Vector2 end = nearPlayer.Center;

                    float gravity = 0.3f;
                    float time = 0.45f;

                    Vector2 delta = end - start;
                    float vx = delta.X / time;
                    float vy = (delta.Y / time) - (gravity * time / 2f);
                    Vector2 init = new Vector2(vx / 20f, vy / 15f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, init, ModContent.ProjectileType<Projectiles.FishProjectile.RedFish>(), 10, 6, -1, Type, aggroTo+1, NPC.life);
                    NPC.active = false;
                }
            }
        }

        private void FlipFish()
        {
            once = true;
            NPC.spriteDirection = -NPC.spriteDirection;
            NPC.velocity.X *= -1;
            NPC.velocity.Y = -1;
        }

        public override void FindFrame(int frameHeight)
        {
            int startFrame = si;
            int finalFrame = si + 1;

            // Frame
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter > 2.5f)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
            }

            if (NPC.frame.Y > finalFrame * frameHeight) NPC.frame.Y = startFrame * frameHeight;
        }
    }
}
