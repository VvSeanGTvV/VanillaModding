using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using VanillaModding.Common.Systems;
using VanillaModding.Common.UI;
using VanillaModding.Common.Utilities;
using VanillaModding.Content.Buffs;
using VanillaModding.Content.Dusts;
using VanillaModding.Content.Items;
using VanillaModding.Content.Items.Accessories;
using VanillaModding.Content.Items.Accessories.Book;
using VanillaModding.Content.Items.Consumable.Healing;
using VanillaModding.Content.Prefixes;
using VanillaModding.Content.Projectiles.EffectProjectile;
using VanillaModding.Content.Projectiles.KoboldDynamite;

namespace VanillaModding.Common
{
    internal class VanillaModdingPlayer : ModPlayer
    {
        // Cursor related variables
        public bool overrideCursor = false;
        public int cursorItem = -1;

        // DPS METER 60ROLL
        private int[] clickBuffer = new int[60];
        private int clickbufferIndex = 0;

        // This is Life/Mana modification related thing.
        public int DiamondHeart, MaxDiamondHeart = 20;
        public int LunarHeart, MaxLunarHeart = 20;

        // Held Item of prefix and class
        int currentPrefix = 0;
        DamageClass currentClass = null;

        // Empowered Tool/Weapon
        public int Empowered = 0;

        // Accessories Bool
        public bool accSatanicBible = false;
        public bool accEpipen = false;
        public bool accValentineRing = false;
        public bool accSharedBrutalShield = false;
        public bool accBrutalShield = false;
        

        // This variable is for D I C E item.
        /// <summary>
        /// Player, currently rolling a Dice
        /// </summary>
        public bool rolling;
        /// <summary>
        /// Has any existing Debuff/buff 
        /// </summary>
        public bool hasAnyDiceEffect;
        /// <summary>
        /// Dice number that has been rolled
        /// </summary>
        public int DiceMult;
        /// <summary>
        /// Total rolls, also used for dice incremental chance of death outcome.
        /// </summary>
        public int totalRolls;

        // Buffs Variables
        /// <summary>
        /// has been stunned by the Stunned debuff
        /// </summary>
        public bool stunned;
        public bool Adrenaline, NaturalAdrenaline = false;

        #region reset functions
        // The ResetEffects hook is important for buffs to work correctly.
        // It resets the effects applied by your buff when it expires.
        public override void ResetEffects()
        {
            hasAnyDiceEffect = false;
            stunned = false;
        }

        public void ResetBool()
        {
            accBrutalShield = accSharedBrutalShield = false;
            accSatanicBible = false;
            accEpipen = false;
        }

        public void ResetDice()
        {
            totalRolls = 0;
            DiceMult = 0;
            rolling = false;
            hasAnyDiceEffect = false;
        }

        #endregion

        #region Shield Absorption Team
        private bool TeammateCanAbsorbDamage()
        {
            foreach (var otherPlayer in Main.ActivePlayers)
            {
                if (otherPlayer.whoAmI != Main.myPlayer && IsAbleToAbsorbDamageForTeammate(otherPlayer, Player.team))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsAbleToAbsorbDamageForTeammate(Player player, int team)
        {
            return player.active
                && !player.dead
                && !player.immune // This check can be removed, allowing players to take hits for team-mates in quick succession. Removing it can also help with de-syncs where the player getting hurt thinks there is no-one to tank the damage, but by the time the hit arrives on the player with the shield, they take extra damage
                && player.GetModPlayer<VanillaModdingPlayer>().accBrutalShield
                && player.team == team
                && player.statLife > player.statLifeMax2 * BrutalShield.DamageAbsorptionAbilityLifeThreshold;
        }

        // This code finds the closest player wearing AbsorbTeamDamageAccessory.
        private static bool IsClosestShieldWearerInRange(Player player, Vector2 target, int team)
        {
            if (!IsAbleToAbsorbDamageForTeammate(player, team))
            {
                return false;
            }

            float distance = player.Distance(target);
            if (distance > BrutalShield.DamageAbsorptionRange)
            {
                return false; // player we're out of range, so can't take the hit
            }

            foreach (var otherPlayer in Main.ActivePlayers)
            {
                if (otherPlayer.whoAmI != Main.myPlayer && IsAbleToAbsorbDamageForTeammate(otherPlayer, team))
                {
                    float otherPlayerDistance = otherPlayer.Distance(target);
                    if (distance > otherPlayerDistance || (distance == otherPlayerDistance && otherPlayer.whoAmI < Main.myPlayer))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            for (int i = 0; i < 49; i++)
            {
                Item item = Player.inventory[i];
                if (item.type == ModContent.ItemType<ResurrectionPotion>() && item.stack > 0 && !Player.HasBuff(BuffID.PotionSickness))
                {
                    Player.statLife = (int)(Player.statLifeMax2 * 0.5f);
                    Player.statMana = (int)(Player.statManaMax2 * 0.5f);
                    SoundEngine.PlaySound(SoundID.Item3, Player.position);
                    SoundEngine.PlaySound(VanillaModdingSoundID.Hallelujah, Player.position);

                    int potionDuration = (int)Player.PotionDelayModifier.ApplyTo(Player.potionDelayTime * 1.25f);
                    Player.AddBuff(BuffID.PotionSickness, potionDuration);
                    Player.potionDelay = potionDuration;

                    Player.manaSick = true;
                    Player.AddBuff(BuffID.ManaSickness, (int)(Player.manaSickTime * 1.25f));

                    item.stack--;
                    if (item.stack <= 0) item.TurnToAir();
                    return false;
                }
            }

            Adrenaline = false;
            NaturalAdrenaline = false;
            return true;
        }

        public override void PreUpdate()
        {
            ResetBool();
            base.PreUpdate();
        }

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default with { 
                Base = 
                DiamondHeart * Content.Items.Consumable.Life.MythrilCanister.LifePerFruit + 
                LunarHeart * Content.Items.Consumable.Life.LuminiteHeart.LifePerFruit 
            };

            // Alternatively:  health = StatModifier.Default with { Base = exampleLifeFruits * ExampleLifeFruit.LifePerFruit };
            mana = StatModifier.Default;
            //mana.Base = exampleManaCrystals * ExampleManaCrystal.ManaPerCrystal;
            // Alternatively:  mana = StatModifier.Default with { Base = exampleManaCrystals * ExampleManaCrystal.ManaPerCrystal };
        }

        /// <summary>
        /// A helper function to check if player is on PVP, but if this Player on team, check the other player if not same team and on PVP.
        /// </summary>
        /// <param name="other"> other player to Check </param>
        /// <returns></returns>
        public bool isPlayerPVP(Player other) => (other.hostile && Player.team != 0) || (Player.team != other.team && other.hostile);

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (accBrutalShield && Player == Main.LocalPlayer && TeammateCanAbsorbDamage())
            {
                modifiers.FinalDamage *= 1f - BrutalShield.DamageAbsorptionMultiplier;
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (accSatanicBible)
            {
                Entity from = null;
                info.DamageSource.TryGetCausingEntity(out from);

                for (int i = 0; i < 15; i++)
                    if (Main.rand.NextBool())
                        Projectile.NewProjectile(Player.GetSource_FromAI(), Player.position, new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f)), ProjectileID.CursedFlameFriendly, BookSatanicBible.cursedflameDMG, 1, Main.myPlayer);

                for (int i = 0; i < 15; i++)
                    if (Main.rand.NextBool())
                        Projectile.NewProjectile(Player.GetSource_FromAI(), Player.position, new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f)), ProjectileID.BookOfSkullsSkull, BookSatanicBible.skeletonDMG, 1, Main.myPlayer);

            }

            // On Hurt is used in this example to act upon another player being hurt.
            // If the player who was hurt was defended, check if the local player should take the remaining damage for them
            Player localPlayer = Main.LocalPlayer;
            if (accSharedBrutalShield && Player != localPlayer && IsClosestShieldWearerInRange(localPlayer, Player.Center, Player.team))
            {
                // The intention of AbsorbTeamDamageAccessory is to transfer 30% of damage taken by teammates to the wearer.
                // In ModifiedHurt, we reduce the damage by 30%. The resulting reduced damage is passed to OnHurt, where the player wearing AbsorbTeamDamageAccessory hurts themselves.
                // Since OnHurt is provided with the damage already reduced by 30%, we need to reverse the math to determine how much the damage was originally reduced by
                // Working through the math, the amount of damage that was reduced is equal to: damage * (percent / (1 - percent))
                float percent = BrutalShield.DamageAbsorptionMultiplier;
                int damage = (int)(info.Damage * (percent / (1 - percent)));

                // Don't bother pinging the defending player and upsetting their immunity frames if the portion of damage we're taking rounds down to 0
                if (damage > 0)
                {
                    localPlayer.Hurt(PlayerDeathReason.ByOther(16), damage, 0);
                }
            }
            base.OnHurt(info);
        }

        public override void UpdateLifeRegen()
        {
            Player myPlayer = Main.LocalPlayer;
            if (accValentineRing) myPlayer.lifeRegen *= 2;
        }

        public override void PostUpdate()
        {
            Player myPlayer = Main.LocalPlayer;

            if (!NaturalAdrenaline
                && !myPlayer.HasBuff(ModContent.BuffType<AdrenalineExhausted>())
                && !myPlayer.HasBuff(ModContent.BuffType<Adrenaline>())
                ) NaturalAdrenaline = myPlayer.statLife > myPlayer.statLifeMax * 0.35f && accEpipen;

            

            currentPrefix = myPlayer.HeldItem.prefix;
            currentClass = myPlayer.HeldItem.DamageType;

            // Buffs related to prefixes
            if (currentPrefix == ModContent.PrefixType<Colossal>())
            {
                myPlayer.AddBuff(BuffID.Slow, 2);
            }
            
            if (Adrenaline && !myPlayer.HasBuff(ModContent.BuffType<Adrenaline>()))
            {
                myPlayer.AddBuff(BuffID.Weak, 60 * 30);
                myPlayer.AddBuff(BuffID.Dazed, 60 * 30);
                myPlayer.AddBuff(BuffID.Darkness, 60 * 15);
                myPlayer.AddBuff(BuffID.Blackout, 60 * 5);
                myPlayer.AddBuff(ModContent.BuffType<AdrenalineExhausted>(), 60 * 15);
                Adrenaline = false;
            }

            if (NaturalAdrenaline && myPlayer.statLife < myPlayer.statLifeMax * 0.35f)
            {
                myPlayer.AddBuff(ModContent.BuffType<Adrenaline>(), 60 * 15);
                NaturalAdrenaline = false;
                Adrenaline = true;
            }

            base.PostUpdate();
        }

        public override void PostUpdateMiscEffects()
        {
            Player myPlayer = Main.LocalPlayer;
            if (currentPrefix == ModContent.PrefixType<Colossal>())
            {
                myPlayer.moveSpeed *= 0.85f;
            }

            if (stunned)
            {
                myPlayer.moveSpeed *= 0f;
            }
            base.PostUpdateMiscEffects();
        }

        public override void PostUpdateRunSpeeds()
        {
            Player myPlayer = Main.LocalPlayer;
            if (currentPrefix == ModContent.PrefixType<Colossal>())
            {
                myPlayer.maxRunSpeed *= 0.25f;
                myPlayer.accRunSpeed *= 0.5f;
            }
            if (stunned)
            {
                myPlayer.maxRunSpeed *= 0f;
                myPlayer.accRunSpeed *= 0f;
                myPlayer.dashTime = 0;
                myPlayer.controlJump = false;
            }
            base.PostUpdateRunSpeeds();
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (proj.ai[2] != 0 && !(proj.owner <= -1)) ApplyEffectsFromPrefix((int)proj.ai[2], Main.LocalPlayer);
            base.OnHitByProjectile(proj, hurtInfo);
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            DamageClass currentClass = item.DamageType;
            int currentPrefix = item.prefix;
            if (currentClass == DamageClass.Melee ||
               currentClass == DamageClass.MeleeNoSpeed ||
               currentClass == DamageClass.SummonMeleeSpeed
               )
                ApplyEffectsFromPrefix((int)currentPrefix, target);
            base.ModifyHitNPCWithItem(item, target, ref modifiers);
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            ApplyEffectsFromPrefix((int)proj.ai[2], target);
            base.ModifyHitNPCWithProj(proj, target, ref modifiers);
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Player myPlayer = Main.LocalPlayer;

            DamageClass currentClass = item.DamageType;
            int currentPrefix = item.prefix;
            bool hasEffects =
                (currentPrefix == ModContent.PrefixType<Spiky>()) ||
                (currentPrefix == ModContent.PrefixType<Venomous>())
                ;
            bool isMelee =
                (currentClass == DamageClass.Melee ||
               currentClass == DamageClass.MeleeNoSpeed ||
               currentClass == DamageClass.SummonMeleeSpeed
               );
            if (hasEffects && isMelee) Projectile.NewProjectile(source, position, velocity, type, damage, knockback, myPlayer.whoAmI, ai2:currentPrefix);
            return !hasEffects || !isMelee;
        }

        /// <summary>
        /// A Helper function to apply prefix related effects on the target, used for both Projectile and Item hit.
        /// </summary>
        /// <param name="prefix"> Prefix Modifier </param>
        /// <param name="target"> Victim </param>
        public void ApplyEffectsFromPrefix(int prefix, Entity target)
        {
            if (target == null) return;
            if (target is NPC npc)
            {
                 if (prefix == ModContent.PrefixType<Spiky>()) npc.AddBuff(BuffID.Bleeding, 5 * 60);
                 if (prefix == ModContent.PrefixType<Venomous>()) npc.AddBuff(BuffID.Poisoned, 8 * 60);
            }
            if (target is Player player)
            {
                if (prefix == ModContent.PrefixType<Spiky>()) player.AddBuff(BuffID.Bleeding, 5 * 60);
                if (prefix == ModContent.PrefixType<Venomous>()) player.AddBuff(BuffID.Poisoned, 8 * 60);
            }
        }

        /// <summary>
        /// Resets the entire DICE stats for the player.
        /// Useful, once a player dies to properly reset.
        /// </summary>
        

        public override void OnEnterWorld()
        {
            ResetDice(); //TODO: should it be saved for balancing situation?
            base.OnEnterWorld();
        }

        public override void UpdateDead()
        {
            ResetDice();
            base.UpdateDead();
        }

        public override void OnRespawn()
        {
            ResetDice();
            base.OnRespawn();
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)VanillaModding.MessageType.VMTStatIncreasePlayerSync);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)DiamondHeart);
            packet.Write((byte)LunarHeart);
            //packet.Write((byte)exampleManaCrystals);
            packet.Send(toWho, fromWho);
        }

        // Called in ExampleMod.Networking.cs
        public void ReceivePlayerSync(BinaryReader reader)
        {
            DiamondHeart = reader.ReadByte();
            LunarHeart = reader.ReadByte();
            //exampleManaCrystals = reader.ReadByte();
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            VanillaModdingPlayer clone = (VanillaModdingPlayer)targetCopy;
            clone.DiamondHeart = DiamondHeart;
            clone.LunarHeart = LunarHeart;
            //clone.exampleManaCrystals = exampleManaCrystals;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            VanillaModdingPlayer clone = (VanillaModdingPlayer)clientPlayer;

            if (DiamondHeart != clone.DiamondHeart || LunarHeart != clone.LunarHeart)
            {
                // This example calls SyncPlayer to send all the data for this ModPlayer when any change is detected, but if you are dealing with a large amount of data you should try to be more efficient and use custom packets to selectively send only specific data that has changed.
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["diamondHeart"] = DiamondHeart;
            tag["lunarHeart"] = LunarHeart;
        }

        public override void LoadData(TagCompound tag)
        {
            DiamondHeart = tag.GetInt("diamondHeart");
            LunarHeart = tag.GetInt("lunarHeart");
        }
    }
}
