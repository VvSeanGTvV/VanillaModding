using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Drawing.Drawing2D;
using System.Security.Policy;
using Terraria.Audio;
using VanillaModding.Content.Projectiles.Bombs;
using VanillaModding.Content.Projectiles.SpikyRed;
using Mono.Cecil;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace VanillaModding.Content.NPCs.Sirius
{
    [AutoloadBossHead]
    internal class Sirius : ModNPC
    {


        // Speed Related NPC
        float npcSpeed = 40f; // The Speed at which the NPC moves towards the target
        float npcAccel = 0.015f; // The Acceleration at which the NPC moves towards the target 

        // Dash Related NPC
        int divDashSpeed = 35; // Division of the speed Dash
        int maxDashTime = 50; // Maximum Time to Dash

        // Offset NPC Guard
        float offsetX = 1000f;
        float offsetY = 300f;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 10;
            NPCID.Sets.TrailingMode[Type] = 0;

            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Specify the debuffs it is immune to. Most NPCs are immune to Confused.
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
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

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar,
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Spiky, and Speedy")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 570;
            NPC.height = 758;

            NPC.damage = 105;
            NPC.defense = 20;
            NPC.lifeMax = 200000;

            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
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

        float theYtarget;
        bool yes;
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) NPC.TargetClosest();

            Player closestNPC = Main.player[NPC.target];
            if (closestNPC == null)
                return;

            if (closestNPC.dead)
            {
                // If the targeted player is dead, flee
                NPC.velocity.Y -= 0.04f;
                // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
                NPC.EncourageDespawn(10);
                return;
            }
            if (!yes)
            {
                theYtarget = closestNPC.Center.Y;
                yes = true;
            }




            //CheckSecond();

            AttackPhase(0, closestNPC);
        }

        static int spikeSpawn = 8;
        static int spikeWallSpawn = 6;
        static float gFXyDef = 400f;

        // Timer
        int t1 = 0;

        // Spike Spawn 
        int s1 = 0;
        int s2 = 0;

        // Bool
        bool flipthis;
        bool onDash;

        // TAGG
        int tagset;

        // Offset gFX
        float gFXy = gFXyDef;

        // Vector2
        Vector2 lastPOS;
        Vector2 lastPOStarget;

        // just sav
        int dmgOrigin;

        private void AttackPhase(int phaseType, Player target)
        {
            var source = NPC.GetSource_FromAI();
            if (phaseType < 0) return;
            

            Vector2 s = NPC.DirectionTo(target.Center);
            float sf = s.ToRotation();
            if (!onDash) NPC.rotation = sf;

            if (!(NPC.Center.Distance(target.Center) < 1128f) && !onDash)
            {
                NPC.velocity = -Vector2.Lerp(-NPC.velocity, (NPC.Center - target.Center).SafeNormalize(Vector2.Zero) * npcSpeed, npcAccel * 4.25f);
                return;
            }

            Vector2 abovePlayer = target.Top + new Vector2(NPC.direction, -(NPC.height + offsetY));
            Vector2 sidePlayer = target.Center + new Vector2(-(NPC.width + offsetX), NPC.direction);

            Vector2 position = NPC.Center;
            Vector2 targetPosition = target.Center; //new Vector2(NPC.Center.X + 30f, NPC.Center.Y);
            Vector2 direction = targetPosition - position;

            //Vector2 menono = (!flipthis) ? new Vector2(target.Center.X - offsetX, target.Center.Y - (380f/5f + gFXy/5f)) - NPC.position : new Vector2(target.Center.X + (offsetX - 650f), target.Center.Y - (380f/5f - gFXy/5f)) - NPC.position;

            if (phaseType == 0) // first phase
            {
                
                if (!onDash) if (!flipthis) NPC.velocity = -Vector2.Lerp(-NPC.velocity, (NPC.position - new Vector2(target.Center.X - offsetX, target.Center.Y - (380f + gFXy))).SafeNormalize(Vector2.Zero) * npcSpeed, npcAccel * 4.25f); else NPC.velocity = -Vector2.Lerp(-NPC.velocity, (NPC.position - new Vector2(target.Center.X + (offsetX - 650f), target.Center.Y - (380f - gFXy))).SafeNormalize(Vector2.Zero) * npcSpeed, npcAccel * 4.25f); //NPC.position = new Vector2(target.Center.X + (offsetX - 600f), target.Center.Y - (380f - gFXy));

                t1++;
                if (tagset == 0)
                {
                    if (s1 < spikeSpawn && t1 > 3)
                    {
                        float numberProjectiles = 2; // 3 shots
                        float rotation = MathHelper.ToRadians(45);//Shoots them in a 45 degree radius. (This is technically 90 degrees because it's 45 degrees up from your cursor and 45 degrees down)
                        position += Vector2.Normalize(NPC.velocity) * 45f; //45 should equal whatever number you had on the previous line
                        for (int i = 0; i < numberProjectiles; i++)
                        {
                            Vector2 perturbedSpeed = NPC.velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // Vector for spread. Watch out for dividing by 0 if there is only 1 projectile.
                            Projectile.NewProjectile(source, position, perturbedSpeed, ModContent.ProjectileType<SpikyRedE>(), 108, 16, -1, 1); //Creates a new projectile with our new vector for spread.
                        }
                        // return false; //makes sure it doesn't shoot the projectile again after this

                        //NPC.position = (!flipthis) ? new Vector2(target.Center.X - offsetX, target.Center.Y - (380f + gFXy)) : new Vector2(target.Center.X + (offsetX - 650f), target.Center.Y - (380f - gFXy));
                        t1 = 0;
                        s1++;
                        //gFXy -= 100f;
                        //float mul = (Main.rand.NextFloat() >= 0f) ? -0.5f : 0.5f;
                        //float mula = (flipthis) ? 1.5f : -1.5f;
                        //Vector2 dir2 = direction + new Vector2(0, 150f * mul);
                        //if (Main.netMode != NetmodeID.MultiplayerClient) Projectile.NewProjectile(source, NPC.Center + new Vector2(0, 0 * mula), dir2 / 20, ModContent.ProjectileType<SpikyRedE>(), 108, 16, -1, 1); //create the projectile
                    }
                    if (s1 >= spikeSpawn)
                    {
                        gFXy = 0f;
                        tagset = 1;
                        t1 = 0;
                    }
                }
                if (tagset == 1)
                {
                    if (t1 > 5 && s2 < spikeWallSpawn)
                    {
                        t1 = 0;
                        s2++;

                        if (s2 < spikeSpawn / 2f) SpawnWallSpike(source, ModContent.ProjectileType<SpikyRedE>(), new Vector2(NPC.Center.X, NPC.Center.Y + NPC.height / 2f), direction / 20, 3, true); else SpawnWallSpike(source, ModContent.ProjectileType<SpikyRedE>(), new Vector2(NPC.Center.X, NPC.Center.Y - NPC.height / 2f), direction / 20, 3, true);
                    }
                    if (s2 >= spikeWallSpawn)
                    {
                        gFXy = 0f;
                        tagset = 2;
                        t1 = 0;

                    }
                }
                if (tagset == 2)
                {
                    NPC.velocity = direction / (NPC.Center.Distance(target.Center) / divDashSpeed);
                    flipthis = !flipthis;
                    onDash = true;
                    tagset = 3;
                    dmgOrigin = NPC.damage;
                    NPC.damage = 0;
                }
                if (tagset == 3)
                {
                    NPC.alpha = (int)((t1 / 10) * 255);
                    if(t1 >= maxDashTime * 2f)
                    {
                        NPC.damage = dmgOrigin;
                        ResetValue();
                        tagset = 0;
                    }
                }
                

                //NPC.velocity = new Vector2(100, 100);
                //NPC.velocity = (NPC.Center - sidePlayer).SafeNormalize(Vector2.Zero) * (npcSpeed * ((NPC.Center - sidePlayer).SafeNormalize(Vector2.Zero).Length() / 10));  // -Vector2.Lerp(-NPC.velocity, (NPC.Center - sidePlayer).SafeNormalize(Vector2.Zero) * npcSpeed, npcAccel);
            }

            if (phaseType == 1) // second phase
            {
                
            }
        }

        public override void FindFrame(int frameHeight)
        {
            // This NPC animates with a simple "go from start frame to final frame, and loop back to start frame" rule
            // In this case: First stage: 0-1-2-0-1-2, Second stage: 3-4-5-3-4-5, 5 being "total frame count - 1"
            int startFrame = 0;
            int finalFrame = 0;
            //if (SecondSprite)
            //{
            //    startFrame = 1;
            //    finalFrame = 1;
            //}
            if (onDash)
            {
                startFrame = 2;
                finalFrame = 2;
            }

            // Frame
            NPC.frameCounter = 0;
            NPC.frame.Y += frameHeight;

            if (NPC.frame.Y > finalFrame * frameHeight)
            {
                NPC.frame.Y = startFrame * frameHeight;
            }
        }

        private void ResetValue() 
        {
            t1 = 0;
            s1 = 0;
            s2 = 0;
            gFXy = gFXyDef;
            onDash = false;
            NPC.alpha = 0;
        }

        private void SpawnWallSpike(Terraria.DataStructures.IEntitySource entitySource, int Type, Vector2 position, Vector2 velocity, int count, bool vertical)
        {
            ModProjectile Modprojectile = ModContent.GetModProjectile(Type);
            Projectile projectile = Modprojectile.Projectile;
            float scale = projectile.scale;
            int width = projectile.width * (int)scale;
            int height = projectile.height * (int)scale;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int startH = -height * (count / 2);
                int startW = -width * (count / 2);
                for (int i = 0; i < count; i++)
                {
                    
                    if(vertical)
                    {
                        Projectile.NewProjectile(entitySource, new Vector2(position.X, position.Y - startH), velocity / 20, ModContent.ProjectileType<SpikyRedE>(), 108, 16, -1, 1);
                        startH += height;
                    }
                    else
                    {
                        Projectile.NewProjectile(entitySource, new Vector2(position.X - startW, position.Y), velocity / 20, ModContent.ProjectileType<SpikyRedE>(), 108, 16, -1, 1);
                        startW += width;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteEffects effects = (NPC.spriteDirection == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 origin = new(texture.Width / 2, texture.Height / Main.npcFrameCount[NPC.type] / 2);
            Vector2 siriusPos = new Vector2(NPC.position.X - Main.screenPosition.X + NPC.width / 2 - texture.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + NPC.height - texture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale);
            for (int i = 1; i < NPC.oldPos.Length; i++)
            {
                Color color = Lighting.GetColor((int)(NPC.position.X + NPC.width * 0.5) / 16, (int)((NPC.position.Y + NPC.height * 0.5) / 16.0));
                color = NPC.GetAlpha(color);
                color *= (NPC.oldPos.Length - i) / 15f;
                Main.spriteBatch.Draw(texture, siriusPos - NPC.velocity * i * 0.5f, new Rectangle?(NPC.frame), color * 0.25f, NPC.rotation, origin, NPC.scale, effects, 0f);
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (onDash) return new Color(1f * 0.97f, 0.5f * 0.97f, 0.5f * 0.97f, 0.25f); else return Color.White;
        }
    }
}
