using log4net.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;
using VanillaModding.Content.NPCs.Ocram.Ocram_Minions;
using VanillaModding.Content.Projectiles.Lobotomy;

namespace VanillaModding.Content.NPCs.LobotomyGod
{
    [AutoloadBossHead]
    internal class LobotomyGod : ModNPC
    {

        // Sway Related NPC
        float maxSwayRotation = 0.6f; // Maximum rotation in radians (~11.5 degrees)
        float swaySpeed = 0.1f;   // How quickly the rotation adjusts
        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;

            NPC.damage = 35;
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
                Music = VanillaModdingMusicID.LobotomyGod;
            }
        }

        public float rotationSpeed = 0.02f;
        public float rotating = 0f;

        public float flapDuration = 40f; // total frames per flap cycle
        public float flapTime = 0f;

        private int phase = 0;
        private float angleOffset = 0;

        private int timesSpawned = 0;
        bool includeLaser = false, includeLobotomyEasy = false;
        bool alreadyuseHoming = false;

        private void Phase(int phase)
        {
            this.phase = phase;
            timesSpawned = 0;
            while (this.phase == 2 && alreadyuseHoming) this.phase = getNewPhase();
            includeLaser = (this.phase == -1) ? true : (this.phase == 1) ? Main.rand.NextBool() : false ;
            includeLobotomyEasy = (this.phase == -1) ? Main.rand.NextBool() : (this.phase == 1) ? Main.rand.NextBool() : false;
            alreadyuseHoming = false;
        }
        
        private int getNewPhase()
        {
            int[] attackTable = new int[]
            {
                -1, -1, -1, -1, -1, -1, -1,
                 0,  0, 
                 1,  1,  1,  1,  1,               
                 2,  2,  2,  
                 3,  3,  3,
                 4,
            };

            int attackType = attackTable[Main.rand.Next(attackTable.Length)];

            return attackType;
        }

        public override void AI()
        {
            rotating += rotationSpeed;
            flapTime = (float)Main.timeForVisualEffects % flapDuration;
            if (rotating >= MathHelper.TwoPi) rotating = 0f;

            // This should almost always be the first code in AI() as it is responsible for finding the proper player target
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }

            Player player = Main.player[NPC.target];

            if (player.dead)
            {
                // If the targeted player is dead, flee
                NPC.velocity.Y -= 0.04f;
                // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
                NPC.EncourageDespawn(10);
                return;
            }

            Vector2 moveTo = player.Center;
            // Movement
            float speed = 24f;
            float inertia = 20f;

            if (NPC.ai[0] % 60 == 0 && includeLaser && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 7; i++)
                {
                    Vector2 offset = new Vector2(Main.rand.Next(-640, 640), Main.rand.Next(-640, 320));
                    int projectile = Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center - offset, new Vector2(10, 0), ModContent.ProjectileType<LobotomyLaser>(), 20, 5, -1);
                    Main.projectile[projectile].timeLeft = 500;
                }
            }

            if (phase == -1)
            {
                NPC.ai[1]++;

                Vector2 abovePlayer = player.Top + new Vector2(0, -(NPC.height + 300f));
                Vector2 toAbovePlayer = abovePlayer - NPC.Center;
                Vector2 toAbovePlayerNormalized = toAbovePlayer.SafeNormalize(Vector2.UnitY);

                moveTo = toAbovePlayerNormalized * speed;

                if (NPC.ai[1] >= 60 * 10)
                {
                    NPC.ai[1] = 0;
                    Phase(getNewPhase());
                }
            }

            if (phase == 0)
            {
                float arcWidth = 1200f;
                float arcPeakHeight = 600f;   // Distance above the player's center
                float arcDuration = 60f;

                // Time + progress
                float time = NPC.ai[0];
                float progress = (time % arcDuration) / arcDuration;

                // Direction (alternates every arc)
                int currentArc = (int)(time / arcDuration);
                int facing = (currentArc % 2 == 0) ? 1 : -1;

                // X: Lerp from left to right (or right to left)
                float xOffset = MathHelper.Lerp(-arcWidth / 2f, arcWidth / 2f, progress) * facing;

                // Y: Parabola from 0 → -arcPeakHeight → 0
                float yOffset = -4f * arcPeakHeight * progress * (1f - progress);

                // Final position relative to player
                Vector2 arcPos = player.Center + new Vector2(xOffset, yOffset * progress);

                
                Vector2 toArc = arcPos - NPC.Center;
                moveTo = toArc.SafeNormalize(Vector2.Zero) * speed;

                if (NPC.ai[0] % 10 == 0)
                {
                    int numberOfProjectiles = 14;
                    float projectileSpeed = 6f;
                    Vector2 spawnPosition = NPC.Center; // or any origin point
                    int projectileType = ModContent.ProjectileType<LobotomyNormal_Enemy>(); // replace with your projectile

                    for (int i = 0; i < numberOfProjectiles; i++)
                    {
                        float angle = MathHelper.ToRadians(360f / numberOfProjectiles * i) + rotating; // optional rotating offset
                        Vector2 direction = new Vector2(1f, 0f).RotatedBy(angle + angleOffset); // unit vector rotated

                        Vector2 velocity = direction * projectileSpeed;

                        if (Main.netMode != NetmodeID.MultiplayerClient) Projectile.NewProjectile(
                            NPC.GetSource_FromAI(),
                            spawnPosition,
                            velocity,
                            projectileType,
                            10,
                            5f,
                            -1
                        );
                    }
                    timesSpawned++;
                    angleOffset += MathHelper.ToRadians(10f); // Increment the angle offset for next time
                }

                if (timesSpawned >= 7) Phase(getNewPhase());
            }
            if (phase == 1)
            {
                Vector2 abovePlayer = player.Top + new Vector2(0, -(NPC.height + 300f));
                Vector2 toAbovePlayer = abovePlayer - NPC.Center;
                Vector2 toAbovePlayerNormalized = toAbovePlayer.SafeNormalize(Vector2.UnitY);

                speed = 12f;
                inertia = 40f;
                moveTo = toAbovePlayerNormalized * speed;

                if (NPC.ai[0] % 30 == 0)
                {
                    int numberOfProjectiles = 14;
                    float projectileSpeed = 6f;
                    Vector2 spawnPosition = NPC.Center; // or any origin point
                    int projectileType = ModContent.ProjectileType<LobotomyNormal_Enemy>(); // replace with your projectile

                    for (int i = 0; i < numberOfProjectiles; i++)
                    {
                        float angle = MathHelper.ToRadians(360f / numberOfProjectiles * i) + rotating; // optional rotating offset
                        Vector2 direction = new Vector2(1f, 0f).RotatedBy(angle + angleOffset); // unit vector rotated

                        Vector2 velocity = direction * projectileSpeed;

                        if (Main.netMode != NetmodeID.MultiplayerClient) Projectile.NewProjectile(
                            NPC.GetSource_FromAI(),
                            spawnPosition,
                            velocity,
                            projectileType,
                            10,
                            5f,
                            -1
                        );
                    }
                    timesSpawned++;
                    angleOffset += MathHelper.ToRadians(10f); // Increment the angle offset for next time
                }

                if (timesSpawned >= 12) Phase(getNewPhase());
            }
            if (phase == 2)
            {
                Vector2 abovePlayer = player.Top + new Vector2(0, -(NPC.height + 300f));
                Vector2 toAbovePlayer = abovePlayer - NPC.Center;
                Vector2 toAbovePlayerNormalized = toAbovePlayer.SafeNormalize(Vector2.UnitY);

                speed = 12f;
                inertia = 40f;
                moveTo = toAbovePlayerNormalized * speed;

                if (NPC.ai[0] % 10 == 0)
                {
                    float projectileSpeed = 6f;
                    Vector2 spawnPosition = NPC.Center; // or any origin point
                    int projectileType = ModContent.ProjectileType<LobotomyExtremeDemon_Enemy>(); // replace with your projectile

                    float angle = MathHelper.ToRadians(Main.rand.NextFloat(-180f, 180f)) + rotating; // optional rotating offset
                    Vector2 direction = new Vector2(1f, 0f).RotatedBy(angle + angleOffset); // unit vector rotated

                    Vector2 velocity = direction * projectileSpeed;

                    if (Main.netMode != NetmodeID.MultiplayerClient) Projectile.NewProjectile(
                        NPC.GetSource_FromAI(),
                        spawnPosition,
                        velocity,
                        projectileType,
                        10,
                        5f,
                        -1
                    );

                    timesSpawned++;
                    angleOffset += MathHelper.ToRadians(10f); // Increment the angle offset for next time
                }

                if (timesSpawned >= 12)
                {
                    alreadyuseHoming = true;
                    Phase(getNewPhase());
                }
            }

            if (phase == 3)
            {
                Vector2 abovePlayer = player.Top + new Vector2(0, -(NPC.height + 300f));
                Vector2 toAbovePlayer = abovePlayer - NPC.Center;
                Vector2 toAbovePlayerNormalized = toAbovePlayer.SafeNormalize(Vector2.UnitY);

                speed = 12f;
                inertia = 40f;
                moveTo = toAbovePlayerNormalized * speed;

                if (NPC.ai[0] % 60 * 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int projectile0 = Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center - new Vector2(Main.rand.Next(-640, 640), -640), new Vector2(10, 0), ModContent.ProjectileType<LobotomyEasy>(), 50, 0, -1, player.direction);
                    int projectile1 = Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center - new Vector2(Main.rand.Next(-640, 640), 0), new Vector2(0, 1), ModContent.ProjectileType<LobotomyInsane>(), 50, 0, -1);
                    timesSpawned++;
                }



                if (timesSpawned >= 12)
                {
                    Phase(getNewPhase());
                }
            }

            if (phase == 4)
            {
                Vector2 abovePlayer = player.Top + new Vector2(0, -(NPC.height + 300f));
                Vector2 toAbovePlayer = abovePlayer - NPC.Center;
                Vector2 toAbovePlayerNormalized = toAbovePlayer.SafeNormalize(Vector2.UnitY);

                speed = 12f;
                inertia = 40f;
                moveTo = toAbovePlayerNormalized * speed;

                if (NPC.ai[0] % 60 * 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float x = Main.rand.Next(-640, 640);
                    for (int i = 0; i < 3; i++)
                    {
                        int projectile1 = Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center - new Vector2(x + (250f * player.direction * i), 0), new Vector2(0, 1), ModContent.ProjectileType<LobotomyInsane>(), 50, 0, -1);
                    }
                    //int projectile0 = Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center - new Vector2(Main.rand.Next(-640, 640), -640), new Vector2(10, 0), ModContent.ProjectileType<LobotomyEasy>(), 50, 0, -1, player.direction);
                    timesSpawned++;
                }

                if (timesSpawned >= 1)
                {
                    Phase(getNewPhase());
                }
            }

            NPC.velocity = (NPC.velocity * (inertia - 1f) + moveTo) / inertia;

            NPC.ai[0]++;
            
            float targetRotation = NPC.velocity.X * 0.05f;
            targetRotation = MathHelper.Clamp(targetRotation, -maxSwayRotation, maxSwayRotation);
            NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, swaySpeed);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 position = NPC.Center - Main.screenPosition;
            String pathMainGlow = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Main_Glow").Replace(@"\", "/");
            String pathWing = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Wing").Replace(@"\", "/");
            String pathHand = nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_Hand").Replace(@"\", "/");

            Texture2D Hand = (Texture2D)ModContent.Request<Texture2D>($"{pathHand}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D Wing = (Texture2D)ModContent.Request<Texture2D>($"{pathWing}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D MainGlow = (Texture2D)ModContent.Request<Texture2D>($"{pathMainGlow}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D OriginTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new(OriginTexture.Width / 2, OriginTexture.Height / 2);
            Vector2 originGlow = new(MainGlow.Width / 2, MainGlow.Height / 2);
            Vector2 originHand = new(Hand.Width / 2, Hand.Height / 2);
            Vector2 originWing = new(Wing.Width / 2, Wing.Height / 2);

            Vector2 eyeOffset0 = new(25, -10);
            Vector2 eyeOffset1 = new(-25, -10);

            float progress = flapTime / flapDuration;
            float wingRotation;
            if (progress < 0.5f) // Upstroke (slow → fast)
            {
                float upT = progress / 0.5f; // Normalized [0,1]
                float easedUp = upT * upT; // Ease-in (slow start, fast end)
                wingRotation = MathHelper.Lerp(0.95f, -0.15f, easedUp); // Moving up
            }
            else // Downstroke (fast → slow)
            {
                float downT = (progress - 0.5f) / 0.5f; // Normalized [0,1]
                float easedDown = 1f - (float)Math.Pow(1f - downT, 5f); // Strong ease-out
                wingRotation = MathHelper.Lerp(-0.15f, 0.95f, easedDown); // Moving down
            }

            Vector2 WingOffset = new(-125, 0);
            Main.spriteBatch.Draw(Wing, position + WingOffset.RotatedBy(-wingRotation), null, Color.White, -wingRotation, originWing, NPC.scale - 0.45f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Wing, position - WingOffset.RotatedBy(wingRotation), null, Color.White, wingRotation, originWing, NPC.scale - 0.45f, SpriteEffects.FlipHorizontally, 0f);

            // GLOW
            Main.spriteBatch.Draw(MainGlow, position, null, new Color(200, 255, 200, 0), 0f, originGlow, NPC.scale + 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position, null, new Color(50, 255, 50, 0), 0f, originGlow, NPC.scale + 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position, null, new Color(0, 255, 0, 0), 0f, originGlow, NPC.scale + 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(OriginTexture, position, null, Color.White , NPC.rotation, origin, NPC.scale + 0.25f, SpriteEffects.None, 0f); // Main Body
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset0.RotatedBy(NPC.rotation), null, new Color(255, 0, 0, 0), 0f, originGlow, NPC.scale - 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset0.RotatedBy(NPC.rotation), null, new Color(255, 50, 50, 0), 0f, originGlow, NPC.scale - 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset0.RotatedBy(NPC.rotation), null, new Color(255, 225, 200, 0), 0f, originGlow, NPC.scale - 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset1.RotatedBy(NPC.rotation), null, new Color(255, 0, 0, 0), 0f, originGlow, NPC.scale - 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset1.RotatedBy(NPC.rotation), null, new Color(255, 50, 50, 0), 0f, originGlow, NPC.scale - 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(MainGlow, position + eyeOffset1.RotatedBy(NPC.rotation), null, new Color(255, 225, 200, 0), 0f, originGlow, NPC.scale - 0.75f, SpriteEffects.None, 0f);
            
            // HANDS
            for (int angle = 0; angle < 360; angle += 45)
            {
                // Convert degrees to radians for use in Vector2 rotation
                float radians = MathHelper.ToRadians(angle) + rotating;

                Vector2 HandsOffset = new(-225, 0);
                Main.spriteBatch.Draw(Hand, position + HandsOffset.RotatedBy(radians), null, Color.Green, radians - MathHelper.PiOver2, originHand, NPC.scale, SpriteEffects.None, 0f);
            }
            for (int angle = 0; angle < 360; angle += 24)
            {
                // Convert degrees to radians for use in Vector2 rotation
                float radians = MathHelper.ToRadians(angle) - rotating;

                Vector2 HandsOffset = new(-125, 0);
                Main.spriteBatch.Draw(Hand, position + HandsOffset.RotatedBy(radians), null, Color.Green, radians - MathHelper.PiOver2, originHand, NPC.scale - 0.5f, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
