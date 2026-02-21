using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using VanillaModding.Common.UI;
using VanillaModding.Content.DamageClasses;
using VanillaModding.Content.Items;
using VanillaModding.Content.Items.Accessories.Book;
using VanillaModding.Content.Prefixes;
using VanillaModding.Content.Projectiles.MightyScythe.MightyProjectile;
using VanillaModding.External.AI;
using static Terraria.NPC;

namespace VanillaModding.Common
{
    internal class VanillaModdingPlayer : ModPlayer
    {
        // Cursor related variables
        public bool overrideCursor = false;
        public int cursorItem = -1;
        /// <summary>
        /// Range of the cursor, can be adjusted by accessories or buffs, used for checking if the cursor can hit the target or not.
        /// </summary>
        public float cursorRange = 0;
        public int cursorDamageTotal = 0;
        public float cursorKnockbackTotal = 0;
        public List<(int, int)> stackedCursorBuff = new List<(int, int)>();

        // This is Life/Mana modification related thing.
        public int DiamondHeart, MaxDiamondHeart = 20;
        public int LunarHeart, MaxLunarHeart = 20;

        // Held Item of prefix and class
        int currentPrefix = 0;
        DamageClass currentClass = null;

        // Accessories Bool
        public bool accSatanicBible = false;

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

        // The ResetEffects hook is important for buffs to work correctly.
        // It resets the effects applied by your buff when it expires.
        public override void ResetEffects()
        {
            hasAnyDiceEffect = false;
            stunned = false;
        }

        public void ResetCursorStat()
        {
            overrideCursor = false;
            cursorItem = -1;

            stackedCursorBuff.Clear();
            cursorDamageTotal = 0;
            cursorRange = cursorKnockbackTotal = 0;
        }

        public void ResetBool()
        {
            accSatanicBible = false;
        }

        public override void PreUpdate()
        {
            ResetBool();
            ResetCursorStat();
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

        public void UpdateCursorDamage(Player myPlayer)
        {
            int cursor = myPlayer.HeldItem.ModItem != null && myPlayer.HeldItem.ModItem is ClickerItem ? myPlayer.HeldItem.type : -1;
            if (cursor <= -1) return;
            if (myPlayer.HeldItem.ModItem != null && myPlayer.HeldItem.ModItem is ClickerItem heldCursor)
            {
                cursorRange = heldCursor.range;
                cursorDamageTotal = heldCursor.Item.damage;
                cursorKnockbackTotal = heldCursor.Item.knockBack;
            }
            if (ModContent.GetModItem(cursor) != null && Main.mouseLeft && (ModContent.GetModItem(cursor).Item.autoReuse || Main.mouseLeftRelease)  && CursorUI.ValidCursorConditions(myPlayer, ModContent.GetModItem(cursor), cursorRange) && ModContent.GetModItem(cursor) is ClickerItem item)
            {
                NPC nearNPC = AdvAI.FindClosestNPC(5f * 16f, Main.MouseWorld);
                Player nearPlayer = AdvAI.FindClosestPlayer(5f * 16f, Main.MouseWorld, player => isPlayerPVP(player) && player.active && !player.dead);
                if (nearNPC != null)
                {
                    bool crit = Main.rand.Next() < myPlayer.GetTotalCritChance(item.Item.DamageType) / 100f;
                    StatModifier damageModifier = myPlayer.GetTotalDamage(item.Item.DamageType);
                    float finalDamage = damageModifier.ApplyTo(cursorDamageTotal);

                    //Main.NewText($"Item: {item.Name}, Range: {item.range}, {myPlayer.position.DistanceSQ(Main.MouseWorld)}, DamageB/F/M/A: {damageModifier.Base}:{damageModifier.Flat}:{damageModifier.Multiplicative}:{damageModifier.Additive}, DamageF: {finalDamage}");

                    myPlayer.ApplyDamageToNPC(nearNPC, (int)finalDamage, cursorKnockbackTotal, myPlayer.direction, crit, item.Item.DamageType, true);
                    foreach (var buffData in stackedCursorBuff)
                    {
                        nearNPC.AddBuff(buffData.Item1, buffData.Item2);
                    }
                }
                if (nearPlayer != null)
                {
                    //Main.NewText($"Item: {item.Name}, Range: {item.range}, {myPlayer.position.DistanceSQ(Main.MouseWorld)}");

                    bool crit = Main.rand.Next(100) < myPlayer.GetTotalCritChance(item.Item.DamageType);
                    nearPlayer.Hurt(Terraria.DataStructures.PlayerDeathReason.ByCustomReason($"{myPlayer.name} used {item.Name} on {nearPlayer.name}"), cursorDamageTotal, myPlayer.direction, true);
                    //myPlayer.ApplyDamageToNPC(nearNPC, item.Item.damage, item.Item.knockBack, myPlayer.direction, crit, item.Item.DamageType);
                    foreach (var buffData in stackedCursorBuff)
                    {
                        nearPlayer.AddBuff(buffData.Item1, buffData.Item2);
                    }
                }
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
            base.OnHurt(info);
        }

        public override void PostUpdate()
        {
            Player myPlayer = Main.LocalPlayer;

            currentPrefix = myPlayer.HeldItem.prefix;
            currentClass = myPlayer.HeldItem.DamageType;

            // Buffs related to prefixes
            if (currentPrefix == ModContent.PrefixType<Colossal>())
            {
                myPlayer.AddBuff(BuffID.Slow, 2);
            }

            UpdateCursorDamage(myPlayer);
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
        public void ResetDice()
        {
            totalRolls = 0;
            DiceMult = 0;
            rolling = false;
            hasAnyDiceEffect = false;
        }

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
    }
}
