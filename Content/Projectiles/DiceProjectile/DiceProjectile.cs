using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common;
using VanillaModding.Common.GlobalNPCs;
using VanillaModding.Common.Systems;
using VanillaModding.Content.Buffs;
using VanillaModding.Content.Items.Consumable;
using VanillaModding.External.AI;

namespace VanillaModding.Content.Projectiles.DiceProjectile
{
    internal class DiceProjectile : ModProjectile
    {

        public readonly int buffLast = 60 * 5;
        public readonly int maxRollTime = 60 * 10;
        public readonly int maxDisplayTime = 60 * 3;
        public int diceType = 0;

        private int colorTotal = 4;
        private int frameCount = 6;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = frameCount * colorTotal;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30; // The width of projectile hitbox
            Projectile.height = 30; // The height of projectile hitbox
            Projectile.aiStyle = -1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.DamageType = DamageClass.Magic; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60 * 999;
            Projectile.alpha = 0;
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.scale = 1f;
            Projectile.light = 1f;
            //Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
            //AIType = ProjectileID.Bullet; // Act exactly like default Bullet
        }

        int timer = 0;
        int mult = 0;
        int waut = 0;
        int bfti = 0;
        bool once = false; 
        public override void AI()
        {
            timer++;
            bool playerMode = Projectile.ai[0] == 1;

            //Main.NewText($"{Projectile.ai[0]} + {Projectile.ai[1]}");
            Player player = Main.player[Projectile.owner];
            NPC npc = Main.npc[(int)Projectile.ai[1]];

            if (player == null && npc == null) Projectile.Kill();
            Vector2 top = (player != null && playerMode) ? player.Top : npc.Top;
            if (playerMode)
            {
                VanillaModdingPlayer modPlayer = player.GetModPlayer<VanillaModdingPlayer>();
                bool hasAnyEffect = player.HasBuff(ModContent.BuffType<DiceBuff>()) || player.HasBuff(ModContent.BuffType<DiceDebuff>());
                if (timer <= maxRollTime)
                {
                    modPlayer.rolling = true;
                    waut++;
                    if (waut >= timer / (maxRollTime / 10))
                    {
                        float biasFactor = 1f + modPlayer.totalRolls * 0.05f; // The more rolls, the stronger the bias
                        float raw = (float)Math.Pow(Main.rand.NextDouble(), 1.0 / biasFactor); // Biased toward higher numbers
                        diceType = (int)(raw * colorTotal);

                        mult = Main.rand.Next(0, 6);
                        Projectile.frame = mult + (diceType * frameCount);
                        waut = 0;
                        //SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
                        //SoundEngine.PlaySound(SoundID.Item40, Projectile.position); TOO ANNOYING lol
                    }
                }
                else if (!once)
                {
                    modPlayer.totalRolls++;
                    if (diceType == 0)
                    {
                        modPlayer.DiceMult = mult + 1;
                        player.AddBuff(ModContent.BuffType<DiceBuff>(), buffLast * modPlayer.totalRolls);
                        once = true;
                        SoundEngine.PlaySound(SoundID.Item4, Projectile.position);
                    }
                    if (diceType == 1)
                    {
                        int amount = player.statLifeMax2 / Math.Max(Math.Min(mult, 1), frameCount);
                        //player.statLife = Math.Min(amount, player.statLifeMax2);
                        once = true;
                        player.HealEffect(amount, true);
                        SoundEngine.PlaySound(SoundID.Item4, Projectile.position);
                    }
                    if (diceType == 2)
                    {
                        modPlayer.DiceMult = mult + 1;
                        player.AddBuff(ModContent.BuffType<DiceDebuff>(), buffLast * modPlayer.totalRolls);
                        once = true;
                        SoundEngine.PlaySound(SoundID.Item40, Projectile.position);
                    }
                    if (diceType == 3)
                    {
                        player.KillMe(PlayerDeathReason.ByCustomReason(Dice.BadLuckDeath.ToNetworkText()), player.statLifeMax2 * 2, 0);
                        once = true;

                        for (int i = 0; i < 20; i++)
                        {
                            Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                            dust.velocity *= 1.4f;
                        }

                        // Fire Dust spawn
                        for (int i = 0; i < 40; i++)
                        {
                            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0f, 0f, 100, default, 3f);
                            dust.noGravity = true;
                            dust.velocity *= 5f;
                            dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0f, 0f, 100, default, 2f);
                            dust.velocity *= 3f;
                        }

                        SoundEngine.PlaySound(SoundID.Item37, Projectile.position);
                        SoundEngine.PlaySound(VanillaModdingSoundID.DeathNoteItemAsylum, Projectile.position);
                    }
                    modPlayer.rolling = false;
                }
                else
                {
                    bfti++;
                    if (bfti >= buffLast || !hasAnyEffect) Projectile.Kill();
                }
                if (!player.active || player.dead) Projectile.Kill();
            } 
            else
            {
                DiceNPC modNPC = npc.GetGlobalNPC<DiceNPC>();
                bool hasAnyEffect = npc.HasBuff(ModContent.BuffType<DiceBuff>()) || npc.HasBuff(ModContent.BuffType<DiceDebuff>());
                if (timer <= maxRollTime)
                {
                    modNPC.rolling = true;
                    waut++;
                    if (waut >= timer / (maxRollTime / 10))
                    {
                        float biasFactor = 1f + modNPC.totalRolls * 0.05f; // The more rolls, the stronger the bias
                        float raw = (float)Math.Pow(Main.rand.NextDouble(), 1.0 / biasFactor); // Biased toward higher numbers
                        diceType = (int)(raw * colorTotal);

                        mult = Main.rand.Next(0, 6);
                        Projectile.frame = mult + (diceType * frameCount);
                        waut = 0;
                        //SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
                        //SoundEngine.PlaySound(SoundID.Item40, Projectile.position); TOO ANNOYING lol
                    }
                }
                else if (!once)
                {
                    modNPC.totalRolls++;
                    if (diceType == 0)
                    {
                        modNPC.DiceMult = mult + 1;
                        npc.AddBuff(ModContent.BuffType<DiceBuff>(), buffLast * modNPC.totalRolls);
                        once = true;
                        SoundEngine.PlaySound(SoundID.Item4, Projectile.position);
                    }
                    if (diceType == 1)
                    {
                        int amount = npc.lifeMax / Math.Max(Math.Min(mult, 1), frameCount);
                        //player.statLife = Math.Min(amount, player.statLifeMax2);
                        once = true;
                        npc.HealEffect(amount, true);
                        SoundEngine.PlaySound(SoundID.Item4, Projectile.position);
                    }
                    if (diceType == 2)
                    {
                        modNPC.DiceMult = mult + 1;
                        npc.AddBuff(ModContent.BuffType<DiceDebuff>(), buffLast * modNPC.totalRolls);
                        once = true;
                        SoundEngine.PlaySound(SoundID.Item40, Projectile.position);
                    }
                    if (diceType == 3)
                    {
                        npc.StrikeInstantKill();
                        once = true;

                        for (int i = 0; i < 20; i++)
                        {
                            Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                            dust.velocity *= 1.4f;
                        }

                        // Fire Dust spawn
                        for (int i = 0; i < 40; i++)
                        {
                            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0f, 0f, 100, default, 3f);
                            dust.noGravity = true;
                            dust.velocity *= 5f;
                            dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0f, 0f, 100, default, 2f);
                            dust.velocity *= 3f;
                        }

                        SoundEngine.PlaySound(SoundID.Item37, Projectile.position);
                        SoundEngine.PlaySound(VanillaModdingSoundID.DeathNoteItemAsylum, Projectile.position);
                    }
                    modNPC.rolling = false;
                }
                else
                {
                    bfti++;
                    if (bfti >= buffLast || !hasAnyEffect) Projectile.Kill();
                }
                if (!npc.active) Projectile.Kill();
            }

            Projectile.position = top - new Vector2(Projectile.width / 2, Projectile.height + 15f);
            /*if (Projectile.velocity.X > -0.1f && Projectile.velocity.X < 0.1f) Projectile.velocity.X = 0f;
            if (Projectile.velocity.Y > -0.1f && Projectile.velocity.Y < 0.1f) Projectile.velocity.Y = 0f;
            Projectile.velocity *= 0.75f;*/
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = 0;
        }
    }
}
