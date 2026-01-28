using Humanizer;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common.GlobalNPCs;
using VanillaModding.Common.Systems;

using VanillaModding.External.AI;

namespace VanillaModding.Content.NPCs.Fish
{

    internal class RedFish : ModNPC
    {
        public static LocalizedText BestiaryText
        {
            get; private set;
        }

        // Sway Related NPC
        float maxSwayRotation = 0.2f; // Maximum rotation in radians (~11.5 degrees)
        float swaySpeed = 0.1f; // How quickly the rotation adjusts

        // Timer Related NPC
        private float bobTimer;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            Main.npcCatchable[Type] = true; // This is for certain release situations

            // These three are typical critter values
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
            NPCID.Sets.TownCritter[Type] = true;

            // The frog is immune to confused
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Burning] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.FlameWhipEnemyDebuff] = true;
            BestiaryText = this.GetLocalization("Bestiary");
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
                new FlavorTextBestiaryInfoElement(BestiaryText.ToString())

                //new FlavorTextBestiaryInfoElement("A worm that likes eating people.")
            });
        }


        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 22;

            NPC.damage = 0;
            NPC.defense = int.MaxValue;
            NPC.lifeMax = 5;
            NPC.catchItem = ModContent.ItemType<Items.Weapon.Throwable.Redfish.RedFish>();

            //NPC.HitSound = SoundID.NPCHit25;
            //NPC.DeathSound = SoundID.NPCDeath28;

            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 0, 10);
            NPC.aiStyle = -1;

        }

        int timerTile = 0;
        int si = 0;
        bool once, twice, YeetMode;
        public override void AI()
        {
            if (!YeetMode) 
            {
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
                    NPC.rotation = 0;
                    NPC.velocity = new Vector2(NPC.spriteDirection * 1.2f, bobAmount);
                }
                else
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        NPC.rotation = 0;
                        NPC.velocity.X = Main.rand.NextFloat(-1f, 1f) * 1.8f;
                        NPC.velocity.Y = -4f;
                    };

                    float targetRotation = NPC.velocity.X * 0.05f;
                    targetRotation = MathHelper.Clamp(targetRotation, -maxSwayRotation, maxSwayRotation);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, swaySpeed);
                }
            }

            int aggroTo = NPC.GetGlobalNPC<VanillaModdingNPC>().aggroTo;
            if (aggroTo >= 0 && !YeetMode)
            {
                
                var nearPlayer = AdvAI.FindClosestPlayer(225f, NPC.position, plr => plr.whoAmI != aggroTo);
                if (nearPlayer != null && ((nearPlayer.Center.Distance(NPC.Center) > 125f) || NPC.GetGlobalNPC<VanillaModdingNPC>().attacked))
                {
                    Vector2 start = NPC.Center;
                    Vector2 end = nearPlayer.Center;

                    float speed = 12f; // adjust for how hard it launches

                    Vector2 direction = end - start;
                    direction.Normalize();

                    NPC.velocity = direction * speed;
                    SoundEngine.PlaySound(VanillaModdingSoundID.FishSpeak, NPC.position);
                    NPC.spriteDirection = (NPC.Center.X > nearPlayer.Center.X) ? -1 : 1;
                    YeetMode = true;
                    //Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, init, ModContent.ProjectileType<Projectiles.FishProjectile.RedFish>(), 10, 6, -1, Type, aggroTo+1, NPC.life);
                    //NPC.active = false;
                }
            }

            if (aggroTo >= 0) 
            {
                //NPC.GetGlobalNPC<VanillaModdingNPC>().statLifeMax2 = 15000 - NPC.GetGlobalNPC<VanillaModdingNPC>().statLifeMax;
                if (!twice)
                {
                    twice = true;
                    NPC.lifeMax = 15000;
                    NPC.life = 15000;
                    NPC.defense = 0;
                }
            } else if (aggroTo <= -1)
            {
                NPC.lifeMax = 5;
                NPC.defense = int.MaxValue;
            }

            if (YeetMode)
            {
                var nearPlayer = AdvAI.FindClosestPlayer(225f, NPC.position, plr => plr.whoAmI != aggroTo);
                if (nearPlayer != null && NPC.getRect().Intersects(nearPlayer.getRect()))
                {
                    double d = nearPlayer.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 157, NPC.spriteDirection);
                    if (!nearPlayer.onHitDodge)
                    {
                        nearPlayer.velocity.X += NPC.velocity.X;
                        nearPlayer.velocity.Y += NPC.velocity.Y;
                        NPC.velocity.X = -NPC.velocity.X;
                        NPC.velocity.Y -= 5f;
                    }
                    
                    if (d > 0) SoundEngine.PlaySound(VanillaModdingSoundID.FishHit, NPC.position);
                }
                NPC.GetGlobalNPC<VanillaModdingNPC>().attacked = false;
                NPC.rotation += MathHelper.ToRadians(15f) * NPC.spriteDirection;
                if ((NPC.velocity.Y > 0 && NPC.wet) || NPC.velocity.Y == 0) YeetMode = false;
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
