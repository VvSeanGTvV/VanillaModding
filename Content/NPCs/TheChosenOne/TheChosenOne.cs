using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
using VanillaModding.Common.Systems;
using VanillaModding.Content.Projectiles.OcramProjectile;
using VanillaModding.Content.Projectiles.RedSolidLaser;

namespace VanillaModding.Content.NPCs.TheChosenOne
{
    [AutoloadBossHead]
    internal class TheChosenOne : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 12;
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
                Music = VanillaModdingMusicID.TheChosenOne;
            }
            //ScaleStats();
        }

        int stg = 0, timer = 0, timer1 = 0, fromWhoAmI = -1;
        bool onStand = true, battleStart = false, onGround = false, lunaticFinish, prepDash, isClone;
        int Frame = 0;
        List<int> aliveClone = new List<int>();
        public override void AI()
        {
            isClone = NPC.ai[2] > 0;
            if (isClone)
            {
                if (fromWhoAmI == -1)
                {
                    NPC.lifeMax = NPC.lifeMax / 200;
                    NPC.life = NPC.life / 200;
                    NPC.defense = NPC.defense / 20;
                    NPC.boss = false;
                    fromWhoAmI = (int)NPC.ai[1];
                    stg = (int)NPC.ai[0];
                }
                if (!Main.npc[fromWhoAmI].active) NPC.active = false;
                onStand = onGround = false;
                FirstStage();

                NPC.scale = 0.85f;
                NPC.immortal = NPC.ai[2] == 1;
                battleStart = true;
                NPC.noTileCollide = true;
                if (NPC.velocity.Y > -0.1f && NPC.velocity.Y < 0.1f) NPC.velocity.Y = 0f;
                NPC.velocity.Y *= 0.95f;
                return;
            }

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
                NPC.noTileCollide = true;
            }

            if (AFK)
            {
                bool stillAliveClone = false;
                foreach (int item in aliveClone)
                {
                    NPC npcClone = Main.npc[item];
                    if (npcClone.active && npcClone.life > 0) stillAliveClone = true;
                    if (stillAliveClone) break;
                }

                float playerY = Main.player[NPC.target].Center.Y;
                float playerX = Main.player[NPC.target].Center.X;
                float xMargin = 80f; // tolerance in pixels
                float yMargin = 80f; // tolerance in pixels

                float npcX = NPC.Center.X;
                float npcY = NPC.Center.Y;
                float desiredVelocityX = (playerX - NPC.Center.X) * 0.1f;
                float desiredVelocityY = ((playerY - NPC.height - 50f) - NPC.Center.Y) * 0.1f;

                // Smooth velocity
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, desiredVelocityX, 0.1f);
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, desiredVelocityY, 0.1f);

                Frame = 2;
                stg = 0;
                AFK = stillAliveClone;
                if (alpha > 0) alpha *= 0.95f;
                if (alpha < 0.25f) alpha = 0.25f;
            }
            NPC.damage = (!AFK) ? NPC.defDamage : 0;
            NPC.dontTakeDamage = AFK;
            //Main.NewText($"STG: {stg} Timer: {timer} Timer1: {timer1} Frame: {Frame} Laser0: {laser0} Laser1: {laser1}", Color.White);
        }

        public void finishClone()
        {
            if (isClone && NPC.ai[2] == 1)
            {
                fromWhoAmI = -1;
                NPC.active = false;
            }
        }
        
        /// <summary>
        /// Creates a clone of itself.
        /// </summary>
        /// <param name="stg"> what stage it runs on </param>
        /// <param name="Offset"> Offset spawning the clone </param>
        /// <param name="cloneType"> 1 = Temporary | 2 = Attached Clone </param>
        public void CreateClone(int stg, Vector2 Offset, int cloneType = 1)
        {
            if (isClone) return;
            var source = NPC.GetSource_FromAI();
            int clone = NPC.NewNPC(source, (int)(NPC.Center.X + Offset.X), (int)(NPC.Center.Y + Offset.Y), Type, ai0: stg, ai1: NPC.whoAmI, ai2: cloneType);
            if (clone != Main.maxNPCs)
            {
                Main.npc[clone].velocity = Vector2.Zero;
                Main.npc[clone].netUpdate = true;
            }
            if (cloneType == 2) aliveClone.Add(clone);
        }

        int dashStart = 3, dashCount = 0, dashTimer = 0;
        int laser0 = -1, laser1 = -1, lunaticOrb = -1;
        float angle = -1f, dashF = 0f, alpha = 1f;
        bool OnDash = false, alreadyDash = false, AFK;
        public void FirstStage()
        {
            NPC.TargetClosest(true);
            var source = NPC.GetSource_FromAI();
            if (AFK) return;

            alpha = 1f;
            timer1++;
            if (!alreadyDash) dashCount = dashStart;
            if (stg == 0)
            {
                if (timer1 > 20)
                {
                    Vector2 direction = Vector2.UnitY * 10f; // Adjust speed as needed
                    if (Main.netMode != NetmodeID.MultiplayerClient && laser0 == -1) laser0 = Projectile.NewProjectile(source, NPC.Center - new Vector2(3 + (2 * ((Frame - 7))), 15).RotatedBy(-0.25f * (Frame - 7)), direction, ModContent.ProjectileType<RedSolidLaser>(), 17, 8, -1, NPC.whoAmI);
                    if (Main.netMode != NetmodeID.MultiplayerClient && laser1 == -1) laser1 = Projectile.NewProjectile(source, NPC.Center - new Vector2(-(10 + (2 * ((Frame - 7)))), 15).RotatedBy(-0.25f * (Frame - 7)), direction, ModContent.ProjectileType<RedSolidLaser>(), 17, 8, -1, NPC.whoAmI);

                    if (laser0 != -1)
                    {
                        Main.projectile[laser0].ai[1] = 3 + (2 * angle);
                        Main.projectile[laser0].ai[2] = -0.25f * angle;
                    }
                    if (laser1 != -1)
                    {
                        Main.projectile[laser1].ai[1] = -(10 + (2 * angle));
                        Main.projectile[laser1].ai[2] = -0.25f * angle;
                    }

                    angle += 0.25f;
                    if (Frame < 9)
                    {

                        Frame = 8 + (int)Math.Round(angle);
                        /*int frameSpeed = 2;
                        NPC.frameCounter += 0.5f;
                        if (NPC.frameCounter > frameSpeed)
                        {
                            NPC.frameCounter = 0;
                            Frame++;
                        }*/
                    }
                    else
                    {

                        int frameSpeed = 2;
                        NPC.frameCounter += 0.5f;
                        if (NPC.frameCounter > frameSpeed)
                        {
                            finishClone();
                            stg++;
                            if (laser0 != -1) Main.projectile[laser0].Kill();
                            if (laser1 != -1) Main.projectile[laser1].Kill();
                            Frame = 4;
                            laser0 = laser1 = -1;
                            timer1 = 0; angle = -1;
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
            else if (stg == 1)
            {
                if (timer1 < 60)
                {
                    Frame = 2;
                    NPC.frameCounter = 0;
                }
                else
                {
                    int frameSpeed = 2;
                    NPC.frameCounter += 0.5f;
                    if (NPC.frameCounter > frameSpeed && Frame < 6 && !lunaticFinish)
                    {
                        Frame++;
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frameCounter > frameSpeed && Frame > 3 && lunaticFinish)
                    {
                        Frame--;
                        NPC.frameCounter = 0;
                    }
                    if (Frame >= 6)
                    {

                        if (Main.netMode != NetmodeID.MultiplayerClient && lunaticOrb == -1) lunaticOrb = Projectile.NewProjectile(source, NPC.Center + new Vector2(0, -(NPC.height / 2 + 65f)), Vector2.Zero, ProjectileID.CultistBossLightningOrb, 50, 8, -1);
                        if (lunaticOrb != -1)
                        {
                            if (!Main.projectile[lunaticOrb].active) lunaticFinish = true;
                        }
                    }
                    if (Frame <= 4 && lunaticFinish)
                    {
                        finishClone();
                        stg++;
                        Frame = 4;
                        lunaticOrb = -1;
                        timer1 = 0;
                        lunaticFinish = false;
                    }
                }
            }
            else if (stg == 2)
            {
                float playerY = Main.player[NPC.target].Center.Y;
                if (dashCount > 0)
                {
                    if (!prepDash)
                    {
                        dashTimer = 0;
                        float yMargin = 80f; // tolerance in pixels

                        float npcY = NPC.Center.Y;
                        float desiredVelocityY = (playerY - NPC.Center.Y) * 0.1f;

                        // Smooth velocity
                        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, desiredVelocityY, 0.1f);
                        NPC.velocity.X = 0;
                        prepDash = (Math.Abs(playerY - npcY) <= yMargin); timer1 = 0;
                        Frame = 4;
                        OnDash = false;
                        NPC.spriteDirection = (NPC.Center.X < Main.player[NPC.target].Center.X) ? 1 : -1;
                    }
                    else
                    {
                        if (timer1 > 30)
                        {
                            float dashSpeed = 48f;
                            alreadyDash = true;
                            // Dash horizontally toward player
                            float direction = Math.Sign(Main.player[NPC.target].Center.X - NPC.Center.X);
                            dashTimer++;
                            if (!OnDash)
                            {
                                dashF = direction;
                                OnDash = true;
                            }
                            NPC.velocity.X = dashF * dashSpeed;
                            Frame = 11;
                            if (dashTimer > 15)
                            {
                                dashCount--;
                                prepDash = false;
                            }
                        }
                    }
                }
                if (timer1 > 30 && dashCount <= 0)
                {
                    finishClone();
                    NPC.velocity = Vector2.Zero;
                    stg++;
                    Frame = 4;
                    timer1 = 0;
                    prepDash = false;
                    alreadyDash = false;
                }
            }
            else if (stg == 3)
            {

                if (timer1 <= 1) NPC.velocity.Y = -15f; else NPC.velocity.Y *= 0.95f;
                if (NPC.velocity.Y > -0.1f && NPC.velocity.Y < 0.1f)
                {
                    if (isClone || Main.rand.NextBool()) stg = 0; else stg++;
                    NPC.velocity.Y = 0f;
                }
            }
            else if (stg == 4)
            {
                if (!AFK)
                {
                    float playerX = Main.player[NPC.target].Center.X;
                    float xMargin = 80f; // tolerance in pixels

                    float npcX = NPC.Center.X;
                    float desiredVelocityX = (playerX - NPC.Center.X) * 0.1f;

                    // Smooth velocity
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, desiredVelocityX, 0.1f);
                    if (Math.Abs(playerX - npcX) <= xMargin)
                    {
                        AFK = true;
                        CreateClone(2, new Vector2(-250, 350), 2);
                        CreateClone(2, new Vector2(250, 350), 2);
                    }
                }
                
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteEffects effects = (NPC.spriteDirection == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 origin = new(texture.Width / 2, texture.Height / Main.npcFrameCount[NPC.type] / 2);
            Vector2 curPosition = new Vector2(NPC.position.X - Main.screenPosition.X + NPC.width / 2 - texture.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + NPC.height - texture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale);
            for (int i = 1; i < NPC.oldPos.Length; i++)
            {
                Color color = Lighting.GetColor((int)(NPC.position.X + NPC.width * 0.5) / 16, (int)((NPC.position.Y + NPC.height * 0.5) / 16.0));
                color = NPC.GetAlpha(color);
                color *= (NPC.oldPos.Length - i) / 15f;
                Main.spriteBatch.Draw(texture, curPosition - NPC.velocity * i * 0.5f, new Rectangle?(NPC.frame), color * 0.25f, NPC.rotation, origin, NPC.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture, curPosition, NPC.frame, drawColor * alpha, NPC.rotation, origin, NPC.scale, effects, 0f);
            return false;
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
