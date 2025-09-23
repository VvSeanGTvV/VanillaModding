using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using VanillaModding.Projectiles.OcramProjectile;
using VanillaModding.NPCs.Ocram.Ocram_Minions;
using Mono.Cecil;
using Terraria.DataStructures;
using Microsoft.CodeAnalysis;
using VanillaModding.Projectiles.Tizona;
using Terraria.Audio;

namespace VanillaModding.NPCs.Ocram
{
    [AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head icon
    internal class Ocram : ModNPC
    {
        // Projectile Properties
        int LaserProjectileCount = 5; // How many times it summons a Laser Projectile.
        int LaserShotPerSec = 3; // Laser Shot / Sec ^
        int LaserRepeat = 3; // How many times it repeats the Laser Routine ^
        int LaserDelay = 60; // How many miliseconds before the next repeat

        int DemonScytheCount = 10; // How many times it summons a Demon Sickle upon dash
        int ArmThrowAway = 3;

        int MaxMinions = 12; // Maximum Minions it can Spawn

        //int SpamDemonthing = 10; // Spawn Demon Sickle upon Dash (varies on how many)
        //int maxMinions = 6; // Maximum minion spawn

        // Dash boi
        int dashMax = 2; // Maximum Dash it can do
        int minDashTime = 2; // Minimum Time to Dash
        int maxDashTime = 50; // Maximum Time to Dash
        int divDashSpeed = 20; // Division of the speed Dash

        // Speed Related NPC
        float npcSpeed = 40f; // The Speed at which the NPC moves towards the target
        float npcAccel = 0.015f; // The Acceleration at which the NPC moves towards the target 

        // Offset NPC Guard
        float offsetX = 200f;
        float offsetY = 200f;

        // Bestiary Localized Text
        public static LocalizedText BestiaryText
        {
            get; private set;
        }
        public override void SetStaticDefaults()
        {
            //Main.npcFrameCount[Type] = 1;

            // Custom Texture Path for NPC Bestiary
            // String CustomTexturePath = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "Bestiary").Replace(@"\", "/");
            // Texture2D BestiaryTexture = (Texture2D)ModContent.Request<Texture2D>($"{CustomTexturePath}", AssetRequestMode.ImmediateLoad).Value;

            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Specify the debuffs it is immune to. Most NPCs are immune to Confused.
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            // This boss also becomes immune to OnFire and all buffs that inherit OnFire immunity during the second half of the fight. See the ApplySecondStageBuffImmunities method.

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "VanillaModding/NPCs/Ocram/OcramBestiary",
                PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = -((190f / 2f - 190f / 2f) * 0.6f),
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            BestiaryText = this.GetLocalization("Bestiary");
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(BestiaryText.ToString())
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 264;
            NPC.height = 190;

            NPC.damage = 65;
            NPC.defense = 20;
            NPC.lifeMax = 35000;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.aiStyle = -1;

            NPC.knockBackResist = 0;

            NPC.boss = true;

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/Ocram");
            }
        }

        public void ScaleStats(int numPlayers, float balance, float bossAdjustment)
        {
            if (Main.expertMode) // onExpertMode
            {
                LaserProjectileCount = 10 * 3;
                DemonScytheCount = 15;
                dashMax = 3;
                MaxMinions = 15;
                //SpamPerSecond = 4;
                //divDashSpeed = 15;
            }

            if (Main.getGoodWorld) // "For The Worthy"
            {
                LaserProjectileCount = 10 * 3;
                DemonScytheCount = 15;
                dashMax = 3;
                MaxMinions = 15; // Increase by 5 if using the "For The Worthy" seed
                //SpamPerSecond = 3;
                //divDashSpeed = 10;
            }
        }

        // Etc. VARIABLE AI (do nut touch)
        // INT
        int stg; // STAGE AI
        int dashstg; // STG DASH
        int boosSTG; // boss STAGE
        int INITANGLE; // INITAL ANGLE OFFSET

        
        int t1; // TIMER 1
        int ki; // TIMER for MINION SPAWNER
        int t2;
        int t3;

        int dash;

        // FLOAT
        float i1;
        float i2;
        float i3;

        float l1;
        float l2;
        float l3;

        float h1;

        // ARM ARRANGEMENT ANGLE OFFSET
        float FrontArmAngle = 0f;
        float MidArmAngle = 0f;
        float BackArmAngle = 0f;
        float divAng = 10f; //DIVISION ANGLE

        float x1Offset; //OFFSET X1
        float y1Offset; //OFFSET Y1

        // BOOL
        bool onAnimation; //ANIMATING IN PROGRESS?
        bool onDash; //DASHING IN PROGRESS?
        bool onSpawn; //SPAWNING IN PROGRESS?
        bool leftSide, onSide, roar;

        int leyeg = 1;
        public override void AI()
        {
            i1 += 0.25f;
            if (i1 >= 1 || INITANGLE >= 1) { i2 += 0.25f; if (INITANGLE < 1) INITANGLE = 1; }
            if (i2 >= 1 || INITANGLE >= 2) { i3 += 0.25f; if (INITANGLE < 2) INITANGLE = 2; }
            var source = NPC.GetSource_FromAI();

            bool isdudnotaiming = (CloseTargetPlayer() == null);
            if (Main.dayTime) NPC.velocity.Y -= 0.04f;
            if (isdudnotaiming || !Main.dayTime) return;

            var target = CloseTargetPlayer();
            Vector2 s = NPC.DirectionTo(target.Center);
            float sf = s.ToRotation() - MathHelper.PiOver2;
            NPC.rotation = sf;

            //NPC POSITION OFFSET GUARD
            Vector2 abovePlayer = target.Top + new Vector2(NPC.direction, -(NPC.height + offsetY));
            Vector2 sidePlayer = (leftSide ? target.Left : target.Right) + new Vector2((leftSide ? -(NPC.width + offsetX) : (NPC.width + offsetX)), NPC.direction);

            if (!(NPC.Center.Distance(target.Center) < 512f) && !onSpawn && !roar)
            {
                NPC.velocity = -Vector2.Lerp(-NPC.velocity, (NPC.Center - target.Center).SafeNormalize(Vector2.Zero) * npcSpeed, npcAccel * 2.25f);
                return;
            }

            if (!onDash && !roar)
            {
                FrontArmAngle = (float)Math.Sin(i1) / divAng;
                MidArmAngle = (float)Math.Sin(i2) / divAng;
                BackArmAngle = (float)Math.Sin(i3) / divAng;
                if (!onSide) NPC.velocity = -Vector2.Lerp(-NPC.velocity, (NPC.Center - abovePlayer).SafeNormalize(Vector2.Zero) * npcSpeed, npcAccel * 2.25f); else NPC.velocity = -Vector2.Lerp(-NPC.velocity, (NPC.Center - sidePlayer).SafeNormalize(Vector2.Zero) * npcSpeed, npcAccel * 2.25f);
            }

            t1++;
            if (boosSTG == 0)
            {
                if (NPC.life < NPC.lifeMax / 1.5f)
                {
                    boosSTG = 1;
                    stg = 0;
                    onDash = false;
                    onSpawn = false;
                    ResetStage(0, boosSTG);
                }
                if (stg == 0) {
                    if (t1 >= LaserShotPerSec && l1 < LaserProjectileCount)
                    {
                        Vector2 position = NPC.Center - new Vector2(-45, 0);
                        Vector2 targetPosition = target.Center;
                        Vector2 direction = targetPosition - position;

                        if (Main.netMode != NetmodeID.MultiplayerClient) Projectile.NewProjectile(source, NPC.Center - new Vector2(-45, 0), direction, ModContent.ProjectileType<PinkishLaser>(), 47, 8); //create the projectile

                        Vector2 position1 = NPC.Center + new Vector2(-45, 0);
                        Vector2 targetPosition1 = target.Center;
                        Vector2 direction1 = targetPosition1 - position1;

                        if (Main.netMode != NetmodeID.MultiplayerClient) Projectile.NewProjectile(source, NPC.Center + new Vector2(-45, 0), direction1, ModContent.ProjectileType<PinkishLaser>(), 47, 8); //create the projectile
                        leyeg = 3;

                        l1++;
                        t1 = 0;
                    } else
                    if (t1 >= LaserDelay && l2 < LaserRepeat)
                    {
                        t1 = 0;
                        l1 = 0;
                        l2++;
                    }
                    if (t1 >= LaserDelay && l2 >= LaserRepeat) ResetStage(Main.rand.Next(0, 3), boosSTG);
                }

                if (stg == 1)
                {
                    onDash = true;
                    if (t1 < 20)
                    {
                        l3 -= 0.025f;
                        FrontArmAngle = l3;
                        MidArmAngle = l3;
                        BackArmAngle = l3;
                    }

                    if (t1 >= 20) t2++;
                    if (t2 < 10) NPC.rotation = sf;
                    if (t2 >= 5 && dashstg == 0 && dash < dashMax)
                    {
                        NPC.rotation = sf;
                        float num332 = 14f;
                        SoundEngine.PlaySound(SoundID.Roar with { PitchVariance = 0.15f, MaxInstances = 0 }, NPC.position);
                        Vector2 vector33 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                        float num333 = Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2 - vector33.X;
                        float num334 = Main.player[NPC.target].position.Y + Main.player[NPC.target].height / 2 - vector33.Y;
                        float num335 = (float)Math.Sqrt((double)(num333 * num333 + num334 * num334));
                        num335 = num332 / num335;
                        NPC.velocity.X = num333 * num335;
                        NPC.velocity.Y = num334 * num335;
                        NPC.velocity *= 2.5f;
                        dashstg = 1;
                        t2 = 0;
                    }
                    if (t2 >= 50 && dashstg == 1 && dash < dashMax)
                    {
                        dashstg = 0;
                        t2 = 0;
                        dash++;
                    }
                    if (t2 >= 10 && dash >= dashMax) ResetStage(2, boosSTG);
                }

                if(stg == 2)
                {
                    if (t2 <= 30)
                    {
                        t2++;
                    }
                    onSpawn = true;
                    h1 += 0.1f;
                    NPC.velocity.X = (float)Math.Cos(h1) * t2;
                    NPC.velocity.Y = (float)Math.Sin(h1) * t2;
                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) - 1.57f;
                    ki++;
                    if (NPC.CountNPCS(ModContent.NPCType<OcramServants>()) < MaxMinions && ki > 10)
                    {
                        ki = 0;
                        SoundEngine.PlaySound(SoundID.NPCDeath45, NPC.position);
                        NPC.NewNPC(source, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OcramServants>(), 0, NPC.whoAmI);
                    }
                    if (NPC.CountNPCS(ModContent.NPCType<OcramServants>()) >= MaxMinions && ki > 10) ResetStage(Main.rand.Next(0, 3), boosSTG);
                }
            }
            if (boosSTG == 1)
            {
                if (stg == 0)
                {
                    if (t1 >= (LaserShotPerSec * 1.15f) && l1 < LaserProjectileCount)
                    {
                        Vector2 position = NPC.Center - new Vector2(0, 45);
                        Vector2 targetPosition = target.Center;
                        Vector2 direction = targetPosition - position;

                        if (Main.netMode != NetmodeID.MultiplayerClient) Projectile.NewProjectile(source, NPC.Center - new Vector2(0, 45), direction, ModContent.ProjectileType<RedLaser>(), 67, 8); //create the projectile
                        leyeg = 3;

                        l1++;
                        t1 = 0;
                    }
                    else
                    if (t1 >= LaserDelay && l2 < LaserRepeat)
                    {
                        t1 = 0;
                        l1 = 0;
                        l2++;
                    }
                    if (t1 >= LaserDelay && l2 >= LaserRepeat) ResetStage(Main.rand.Next(1, 4), boosSTG);
                }
                if (stg == 1)
                {
                    if (t1 >= (LaserShotPerSec / 1.5f) && l1 < LaserProjectileCount)
                    {
                        onSide = true;
                        leftSide = !leftSide;
                        Vector2 position = NPC.Center - new Vector2(45, 0);
                        Vector2 targetPosition = target.Center;
                        Vector2 direction = targetPosition - position;

                        if (Main.netMode != NetmodeID.MultiplayerClient) Projectile.NewProjectile(source, NPC.Center - new Vector2(45, 0), direction.RotatedByRandom(MathHelper.ToRadians(12)), ModContent.ProjectileType<PinkishLaser>(), 27, 8); //create the projectile
                        leyeg = 3;

                        l1++;
                        t1 = 0;
                    }
                    else
                    if (t1 >= LaserDelay && l2 < LaserRepeat)
                    {
                        t1 = 0;
                        l1 = 0;
                        l2++;
                    }
                    if (t1 >= LaserDelay && l2 >= LaserRepeat) ResetStage(2, boosSTG);
                }

                if (stg == 2)
                {
                    if (t2 <= 30)
                    {
                        t2++;
                    }
                    onSpawn = true;
                    h1 += 0.1f;
                    NPC.velocity.X = (float)Math.Cos(h1) * t2;
                    NPC.velocity.Y = (float)Math.Sin(h1) * t2;
                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) - 1.57f;
                    ki++;
                    if (NPC.CountNPCS(ModContent.NPCType<OcramServants>()) < MaxMinions && ki > 10)
                    {
                        ki = 0;
                        SoundEngine.PlaySound(SoundID.NPCDeath45, NPC.position);
                        NPC.NewNPC(source, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OcramServants>(), 0, NPC.whoAmI);
                    }
                    if (NPC.CountNPCS(ModContent.NPCType<OcramServants>()) >= MaxMinions && ki > 10) ResetStage(Main.rand.Next(0, 4), boosSTG);
                }

                if (stg == 3)
                {
                    SoundEngine.PlaySound(SoundID.Roar with { PitchVariance = 0.15f, MaxInstances = 0 }, NPC.position);
                    roar = true;
                    stg = 99;
                }
                if (stg == 99)
                {
                    NPC.velocity.X = 0f;
                    NPC.velocity.Y = 0f;
                    NPC.rotation = 0f;
                    t3++;
                    if (t3>=120) ResetStage(Main.rand.Next(0, 2), boosSTG);
                }
            }
        }

        private void ResetStage(int selectSTG, int boos)
        {
            t1 = 0;
            onSpawn = false;
            onDash = false;
            if (boos == 0)
            {
                if (selectSTG == 0) l2 = l1 = 0;
                if (selectSTG == 1) l3 = dash = t2 = 0;
                if (selectSTG == 2) h1 = ki = t2 = 0;
            }

            if (boos == 1)
            {
                roar = false;
                onSide = false;
                if (selectSTG == 0) l2 = l1 = 0;
                if (selectSTG == 1) l2 = l1 = 0;
                if (selectSTG == 2) h1 = ki = t2 = 0;
                if (selectSTG == 3) h1 = ki = t3 = 0;
                //if (selectSTG == 1) l3 = dash = t2 = 0;
                //if (selectSTG == 2) h1 = ki = t2 = 0;
            }
            stg = selectSTG;
        }
        
        private void SpawnMinions (int MaxMinions, IEntitySource source, int SpawnSpeed)
        {
            ki++;
            if(NPC.CountNPCS(ModContent.NPCType<OcramServants>()) < MaxMinions && ki > SpawnSpeed)
            {
                ki = 0;
                NPC.NewNPC(source, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OcramServants>(), 0, NPC.whoAmI);
            }
        }

        private Player CloseTargetPlayer()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) NPC.TargetClosest();

            Player closestNPC = Main.player[NPC.target];
            if (closestNPC == null)
                return null;

            if (closestNPC.dead || Main.dayTime)
            {
                // If the targeted player is dead or out of range of the Desert, flee
                NPC.velocity.Y -= 0.04f;
                // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
                NPC.EncourageDespawn(10);
                return null;
            }
            return closestNPC;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.5f * 1.3f);
            NPC.damage = (int)(NPC.damage * 0.7f);
            if (numPlayers <= 1) return;
            float healthBoost = 0.35f;
            for (int k = 1; k < numPlayers; k++)
            {
                NPC.lifeMax += (int)(NPC.lifeMax * healthBoost);
                healthBoost += (1 - healthBoost) / 3;
            }
            ScaleStats(numPlayers, balance, bossAdjustment);
        }

        private int lightspeed = 5;
        private int light = 0;
        private int lg = 1;

        private int eyelightspeed = 5;
        private int lighteye = 0;

        private int eyeBlink;

        private Vector2 ocramOldPos;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            String pathSecondPhase = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_SecondPhase").Replace(@"\", "/");
            String pathSecondPhaseRoar = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_SecondPhase_roar").Replace(@"\", "/");

            String pathBrain = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "Brain").Replace(@"\", "/");
            String pathBrainEye = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "BrainEye").Replace(@"\", "/");
            String pathBrainEyeGlow = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "BrainEye_Glow").Replace(@"\", "/");
            String pathLight = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "Light").Replace(@"\", "/");

            String pathLight1 = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "Light1").Replace(@"\", "/");
            String pathLight2 = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "Light2").Replace(@"\", "/");
            String pathLight3 = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "Light3").Replace(@"\", "/");

            String pathEye1 = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "Eye1").Replace(@"\", "/");
            String pathEye2 = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "Eye2").Replace(@"\", "/");
            String pathEye3 = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "Eye3").Replace(@"\", "/");

            String pathTentacleBack = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "TentacleLong").Replace(@"\", "/");
            String pathTentacleMid = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "TentacleMiddle").Replace(@"\", "/");
            String pathTentacleFront = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "TentacleShort").Replace(@"\", "/");

            String pathFrontArm = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "ArmFront").Replace(@"\", "/");
            String pathBackArm = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "ArmBack").Replace(@"\", "/");
            String pathMidArm = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "ArmMiddle").Replace(@"\", "/");

            String pathFrontArmFlip = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "ArmFrontFlip").Replace(@"\", "/");
            String pathBackArmFlip = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "ArmBackFlip").Replace(@"\", "/");
            String pathMidArmFlip = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "ArmMiddleFlip").Replace(@"\", "/");

            String pathShortArm = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "ArmShort").Replace(@"\", "/");
            String pathShortArmFlip = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "ArmShort").Replace(@"\", "/");

            Texture2D FrontArm = (Texture2D)ModContent.Request<Texture2D>($"{pathFrontArm}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D MidArm = (Texture2D)ModContent.Request<Texture2D>($"{pathMidArm}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D BackArm = (Texture2D)ModContent.Request<Texture2D>($"{pathBackArm}", AssetRequestMode.ImmediateLoad).Value;

            Texture2D FrontArmFlip = (Texture2D)ModContent.Request<Texture2D>($"{pathFrontArmFlip}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D MidArmFlip = (Texture2D)ModContent.Request<Texture2D>($"{pathMidArmFlip}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D BackArmFlip = (Texture2D)ModContent.Request<Texture2D>($"{pathBackArmFlip}", AssetRequestMode.ImmediateLoad).Value;

            Texture2D TentacleBack = (Texture2D)ModContent.Request<Texture2D>($"{pathTentacleBack}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D TentacleMid = (Texture2D)ModContent.Request<Texture2D>($"{pathTentacleMid}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D TentacleFront = (Texture2D)ModContent.Request<Texture2D>($"{pathTentacleFront}", AssetRequestMode.ImmediateLoad).Value;

            Texture2D ShortArm = (Texture2D)ModContent.Request<Texture2D>($"{pathShortArm}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D ShortArmFlip = (Texture2D)ModContent.Request<Texture2D>($"{pathShortArmFlip}", AssetRequestMode.ImmediateLoad).Value;

            Texture2D Brain = (Texture2D)ModContent.Request<Texture2D>($"{pathBrain}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D BrainEye = (Texture2D)ModContent.Request<Texture2D>($"{pathBrainEye}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D BrainEyeGlow = (Texture2D)ModContent.Request<Texture2D>($"{pathBrainEyeGlow}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D Light = (Texture2D)ModContent.Request<Texture2D>($"{pathLight}", AssetRequestMode.ImmediateLoad).Value;

            Texture2D LightBottom = (Texture2D)ModContent.Request<Texture2D>($"{pathLight1}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D LightMiddle = (Texture2D)ModContent.Request<Texture2D>($"{pathLight2}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D LightTop = (Texture2D)ModContent.Request<Texture2D>($"{pathLight3}", AssetRequestMode.ImmediateLoad).Value;

            Texture2D Eye1 = (Texture2D)ModContent.Request<Texture2D>($"{pathEye1}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D Eye2 = (Texture2D)ModContent.Request<Texture2D>($"{pathEye2}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D Eye3 = (Texture2D)ModContent.Request<Texture2D>($"{pathEye3}", AssetRequestMode.ImmediateLoad).Value;

            Texture2D OriginTexture = ModContent.Request<Texture2D>(Texture).Value;
            if(boosSTG == 1) OriginTexture = (Texture2D)ModContent.Request<Texture2D>($"{pathSecondPhase}", AssetRequestMode.ImmediateLoad).Value;
            if (roar) OriginTexture = (Texture2D)ModContent.Request<Texture2D>($"{pathSecondPhaseRoar}", AssetRequestMode.ImmediateLoad).Value;

            Vector2 origin = new(OriginTexture.Width / 2, OriginTexture.Height / 2);
            Vector2 originBack = new(BackArm.Width, 2);
            Vector2 originMid = new(MidArm.Width - 6, 2);
            Vector2 originFront = new(2, 2);
            Vector2 originEye = new(BrainEye.Width / 2, BrainEye.Height / 2);

            Vector2 originBackFlip = new(2, 2);
            Vector2 originMidFlip = new(2 + 6, 2);
            Vector2 originFrontFlip = new(FrontArm.Width, 2);

            Vector2 ocramPos = NPC.Center - Main.screenPosition;

            Main.spriteBatch.Draw(BackArm, ocramPos - new Vector2(80, 3).RotatedBy(NPC.rotation), null, drawColor, NPC.rotation + BackArmAngle * 1.75f, originBack, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MidArm, ocramPos - new Vector2(92, -13).RotatedBy(NPC.rotation), null, drawColor, NPC.rotation + MidArmAngle * 2.25f, originMid, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(FrontArm, ocramPos - new Vector2(86, -23).RotatedBy(NPC.rotation), null, drawColor, NPC.rotation + FrontArmAngle, originFront, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(ShortArm, ocramPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(BackArmFlip, ocramPos - new Vector2(-84, 3).RotatedBy(NPC.rotation), null, drawColor, NPC.rotation - BackArmAngle * 1.75f, originBackFlip, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MidArmFlip, ocramPos - new Vector2(-96, -13).RotatedBy(NPC.rotation), null, drawColor, NPC.rotation - MidArmAngle * 2.25f, originMidFlip, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(FrontArmFlip, ocramPos - new Vector2(-90, -23).RotatedBy(NPC.rotation), null, drawColor, NPC.rotation - FrontArmAngle, originFrontFlip, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(ShortArm, ocramPos - new Vector2(-2, 0), null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.FlipHorizontally, 0f);

            Main.spriteBatch.Draw(TentacleBack, ocramPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(TentacleMid, ocramPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(TentacleFront, ocramPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(TentacleBack, ocramPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(TentacleMid, ocramPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(TentacleFront, ocramPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.FlipHorizontally, 0f);

            Main.spriteBatch.Draw(Brain, ocramPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

            if (onDash)
            {
                for (int i = 1; i < NPC.oldPos.Length; i++)
                {
                    Color color = Color.Red;
                    color = NPC.GetAlpha(color);
                    color *= (NPC.oldPos.Length - i) / 15f;
                    Main.spriteBatch.Draw(OriginTexture, new Vector2(NPC.position.X - Main.screenPosition.X + NPC.width / 2 - OriginTexture.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + NPC.height - OriginTexture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale) - NPC.velocity * i * 0.5f, new Rectangle?(NPC.frame), color, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            Main.spriteBatch.Draw(OriginTexture, ocramPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

            if (boosSTG == 1)
            {
                Main.spriteBatch.Draw(LightBottom, ocramPos, null, Color.White * 0.75f, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                float eyeRotation = (float)Math.Atan2(NPC.Center.Y - (Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)), NPC.Center.X - (Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)));
                Vector2 topLayer = NPC.Center - Main.player[NPC.target].Center;
                float playerDistance = (float)Math.Sqrt(topLayer.X * topLayer.X + topLayer.Y * topLayer.Y);
                ocramPos -= new Vector2(Math.Min(500, playerDistance), 0).RotatedBy(eyeRotation) / 120;
                if (ocramOldPos == Vector2.Zero) ocramOldPos = ocramPos;
                Main.spriteBatch.Draw(BrainEye, (ocramPos + ocramOldPos) / 2, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

                if (eyeBlink < 200f)
                {
                    eyeBlink++;
                    float blink = (float)Math.Sin(eyeBlink / 4) / 2 + 0.5f;
                    drawColor = new Color(1f - blink, 0.5f * blink, 2.5f * blink, 0);
                    Main.spriteBatch.Draw(BrainEyeGlow, (ocramPos + ocramOldPos) / 2, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                }
                else eyeBlink = 0;

                ocramOldPos = ocramPos;
            }


            if (boosSTG == 0) {
                Main.spriteBatch.Draw(Light, ocramPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

                if (leyeg == 1) Main.spriteBatch.Draw(Eye1, ocramPos, null, Color.White, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                if (leyeg == 2) Main.spriteBatch.Draw(Eye2, ocramPos, null, Color.White, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                if (leyeg == 3) Main.spriteBatch.Draw(Eye3, ocramPos, null, Color.White, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

                if (lg == 1) Main.spriteBatch.Draw(LightBottom, ocramPos, null, Color.White * 1.75f, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                if (lg == 2) Main.spriteBatch.Draw(LightMiddle, ocramPos, null, Color.White * 1.75f, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                if (lg == 3) Main.spriteBatch.Draw(LightTop, ocramPos, null, Color.White * 1.75f, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

                light++;
                if (light >= lightspeed)
                {
                    light = 0;
                    lg++;
                    if (lg > 3) lg = 1;
                }

                if (leyeg > 1) lighteye++;
                if (lighteye >= lightspeed)
                {
                    lighteye = 0;
                    leyeg--;
                    if (leyeg < 1) leyeg = 1;
                }
            }
            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
            => potionType = ItemID.GreaterHealingPotion;
    }
}
