using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Projectiles.OcramProjectile;
using VanillaModding.Content.Projectiles.PrismLaser;

namespace VanillaModding.Content.NPCs.TheChosenOne
{
    internal class TheChosenOne : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 11;
            NPCID.Sets.TrailCacheLength[Type] = 10;
            NPCID.Sets.TrailingMode[Type] = 0;

            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Specify the debuffs it is immune to. Most NPCs are immune to Confused.
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            // This boss also becomes immune to OnFire and all buffs that inherit OnFire immunity during the second half of the fight. See the ApplySecondStageBuffImmunities method.

            // Influences how the NPC looks in the Bestiary
            float Scale = 0.5f;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //CustomTexturePath = "ExampleMod/Assets/Textures/Bestiary/MinionBoss_Preview",
                PortraitScale = Scale, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = -((NPC.height / 2f + 7f) * Scale),
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            //90x82 FRAME
            NPC.width = 82;
            NPC.height = 90;

            NPC.damage = 105;
            NPC.defense = 20;
            NPC.lifeMax = 200000;

            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.aiStyle = -1;
            NPC.scale = 1f;

            NPC.knockBackResist = 0;
            NPC.boss = true;

            if (!Main.dedServ)
            {
                Music = MusicID.OtherworldlyBoss1; //MusicLoader.GetMusicSlot(Mod, "Music/Ocram");
            }
            //ScaleStats();
        }

        int stg = 0, timer = 0, timer1 = 0;
        bool onStand = true, battleStart = false, onGround;
        int Frame = 0;
        public override void AI()
        {
            onGround = false;
            if (onStand)
            {
                NPC.ai[0] += 1f;
                if (NPC.ai[0] >= 5f)
                {
                    NPC.ai[0] = 5f;
                    NPC.velocity.Y += 0.1f;
                }
                if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.velocity.Y = 0;
                    onGround = true;
                }
                if (NPC.velocity.Y > 16f) NPC.velocity.Y = 16f;
            }

            timer++;
            if (onStand && timer > 60 * 3)
            {
                Frame = 1;
                onStand = false;
            } 
            else if (timer > 60 * 3.15f)
            {

                if (!battleStart)
                {
                    NPC.velocity.Y -= 16f;
                } 
                else
                {
                    if (NPC.velocity.Y > -0.1f && NPC.velocity.Y < 0.1f) NPC.velocity.Y = 0f;
                    NPC.velocity.Y *= 0.95f;
                    if (Frame < 4)
                    {
                        int frameSpeed = 2;
                        NPC.frameCounter += 0.5f;
                        if (NPC.frameCounter > frameSpeed)
                        {
                            NPC.frameCounter = 0;
                            Frame++;
                        }
                    }
                }

                if (timer > 60 * 3.25f) FirstStage();
                battleStart = true;
            }
        }

        int laser0 = -1, laser1 = -1;
        public void FirstStage()
        {
            var source = NPC.GetSource_FromAI();

            timer1++;
            if (stg == 0)
            {
                if (timer1 > 20)
                {
                    Vector2 direction = Vector2.UnitY * 10f; // Adjust speed as needed
                    if (Main.netMode != NetmodeID.MultiplayerClient && laser0 == -1) laser0 = Projectile.NewProjectile(source, NPC.Center - new Vector2(3 + (2 * ((Frame - 7))), 15).RotatedBy(-0.25f * (Frame - 7)), direction, ModContent.ProjectileType<PrismLaser>(), 17, 8, -1, NPC.whoAmI);
                    if (Main.netMode != NetmodeID.MultiplayerClient && laser1 == -1) laser1 = Projectile.NewProjectile(source, NPC.Center - new Vector2(-(10 + (2 * ((Frame - 7)))), 15).RotatedBy(-0.25f * (Frame - 7)), direction, ModContent.ProjectileType<PrismLaser>(), 17, 8, -1, NPC.whoAmI);

                    if (laser0 != -1)
                    {
                        Main.projectile[laser0].ai[1] = 3 + (2 * ((Frame - 7)));
                        Main.projectile[laser0].ai[2] = -0.25f * (Frame - 7);
                    }
                    if (laser1 != -1)
                    {
                        Main.projectile[laser1].ai[1] = -(10 + (2 * ((Frame - 7))));
                        Main.projectile[laser1].ai[2] = -0.25f * (Frame - 7);
                    }
                    if (Frame < 8)
                    {
                        int frameSpeed = 2;
                        NPC.frameCounter += 0.5f;
                        if (NPC.frameCounter > frameSpeed)
                        {
                            NPC.frameCounter = 0;
                            Frame++;
                        }
                    }
                    else
                    {

                        int frameSpeed = 2;
                        NPC.frameCounter += 0.5f;
                        if (NPC.frameCounter > frameSpeed)
                        {
                            stg++;
                            if (laser0 != -1) Main.projectile[laser0].Kill();
                            if (laser1 != -1) Main.projectile[laser1].Kill();
                            Frame = 4;
                            laser0 = laser1 = -1;
                            timer1 = 0;
                        }
                    }
                }
                else if (timer1 < 10)
                {
                    Frame = 3;
                } 
                else
                {
                    Frame = 6;
                }
            } 
            if (stg == 1)
            {

            }
        }

        public override void FindFrame(int frameHeight)
        {
            /*int startFrame = 0;
            int finalFrame = 3;

            int frameSpeed = 2;
            NPC.frameCounter += 0.5f;
            NPC.frameCounter += NPC.velocity.Length() / 10f; // Make the counter go faster with more movement speed
            if (NPC.frameCounter > frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > finalFrame * frameHeight)
                {
                    NPC.frame.Y = startFrame * frameHeight;
                }
            }*/
            NPC.frame.Y = Frame * frameHeight;
        }
    }
}
