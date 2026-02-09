using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using VanillaModding.Content.Prefixes;

namespace VanillaModding.Common
{
    internal class VanillaModdingPlayer : ModPlayer
    {
        // This is Life/Mana modification related thing.
        public int DiamondHeart, MaxDiamondHeart = 20;
        public int LunarHeart, MaxLunarHeart = 20;

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

        DamageClass currentClass;
        int currentPrefix;
        public override void PostUpdate()
        {
            Player myPlayer = Main.LocalPlayer;
            currentPrefix = myPlayer.HeldItem.prefix;

            currentClass = myPlayer.HeldItem.DamageType;
            if (currentPrefix == ModContent.PrefixType<Hot>()) myPlayer.AddBuff(BuffID.Burning, 60);
            base.PostUpdate();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (currentClass == DamageClass.Melee ||
                currentClass == DamageClass.MeleeNoSpeed ||
                currentClass == DamageClass.SummonMeleeSpeed
                )
            {
                if (currentPrefix == ModContent.PrefixType<Spicy>()) target.AddBuff(BuffID.OnFire, 3 * 60);
                if (currentPrefix == ModContent.PrefixType<Hot>()) target.AddBuff(BuffID.OnFire, 6 * 60);
            }
            base.ModifyHitNPC(target, ref modifiers);
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

            if (DiamondHeart != clone.DiamondHeart)
            {
                // This example calls SyncPlayer to send all the data for this ModPlayer when any change is detected, but if you are dealing with a large amount of data you should try to be more efficient and use custom packets to selectively send only specific data that has changed.
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }

            if (LunarHeart != clone.LunarHeart)
            {
                // This example calls SyncPlayer to send all the data for this ModPlayer when any change is detected, but if you are dealing with a large amount of data you should try to be more efficient and use custom packets to selectively send only specific data that has changed.
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }
        }
    }
}
