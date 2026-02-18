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

        int stg = 0, timer = 0, timer1 = 0;
        bool onStand = true, battleStart = false, onGround, lunaticFinish, prepDash;
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
                NPC.noTileCollide = true;
            }

            //Main.NewText($"STG: {stg} Timer: {timer} Timer1: {timer1} Frame: {Frame} Laser0: {laser0} Laser1: {laser1}", Color.White);
        }
        int dashStart = 3, dashCount = 0, dashTimer = 0;
        int laser0 = -1, laser1 = -1, lunaticOrb = -1;
        float angle = -1f, dashF = 0f;
        bool OnDash = false;
        public void FirstStage()
        {
            var source = NPC.GetSource_FromAI();

            timer1++;
            if (stg != 2) dashCount = dashStart;
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
                NPC.TargetClosest(true);
                float playerY = Main.player[NPC.target].Center.Y;
                if (dashCount > 0)
                {
                    if (!prepDash)
                    {
                        dashTimer = 0;
                        float yMargin = 20f; // tolerance in pixels

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
                    NPC.velocity = Vector2.Zero;
                    stg++;
                    Frame = 4;
                    timer1 = 0;
                    prepDash = false;
                }
            }
            else if (stg == 3)
            {
                if (timer1 <= 1) NPC.velocity.Y = -15f; else NPC.velocity.Y *= 0.95f;
                if (NPC.velocity.Y > -0.1f && NPC.velocity.Y < 0.1f)
                {
                    stg = 0;
                    NPC.velocity.Y = 0f;
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
            return true;
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
