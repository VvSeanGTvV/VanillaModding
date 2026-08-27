using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common;
using VanillaModding.Content.Items.Materials;

namespace VanillaModding.Content.Items.Accessories
{
    public class AbsorbTeamBrutalShield : ModBuff
    {
        public override LocalizedText Description => base.Description.WithFormatArgs(BrutalShield.DamageAbsorptionPercent);

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<VanillaModdingPlayer>().accSharedBrutalShield = true;
        }
    }

    [AutoloadEquip(EquipType.Shield)]
    internal class BrutalShield : ModItem
    {
        public static readonly int DamageAbsorptionAbilityLifeThresholdPercent = 35;
        public static float DamageAbsorptionAbilityLifeThreshold => DamageAbsorptionAbilityLifeThresholdPercent / 100f;

        public static readonly int DamageAbsorptionPercent = 30;
        public static float DamageAbsorptionMultiplier => DamageAbsorptionPercent / 100f;

        // 50 tiles is 800 world units. (50 * 16 == 800)
        public static readonly int DamageAbsorptionRange = 800;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageAbsorptionPercent, DamageAbsorptionAbilityLifeThresholdPercent);

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 38;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 16;
            Item.value = Item.buyPrice(0, 20, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.noKnockback = true;
            player.aggro += 500;
            player.GetModPlayer<VanillaModdingPlayer>().accBrutalShield = true;

            // Remember that UpdateAccessory runs for all players on all clients. Only check every 10 ticks
            if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
            {
                Player localPlayer = Main.LocalPlayer;
                if (localPlayer.team == player.team && player.team != 0 && player.statLife > player.statLifeMax2 * DamageAbsorptionAbilityLifeThreshold && player.Distance(localPlayer.Center) <= DamageAbsorptionRange)
                {
                    // The buff is used to visually indicate to the player that they are defended, and is also synchronized automatically to other players, letting them know that we were defended at the time we took the hit
                    localPlayer.AddBuff(ModContent.BuffType<AbsorbTeamBrutalShield>(), 20);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HeroShield)
                .AddIngredient(ItemID.CobaltShield)
                .AddIngredient<SoulofBlight>(10)
                .AddIngredient(ItemID.AdamantiteBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.HeroShield)
                .AddIngredient(ItemID.CobaltShield)
                .AddIngredient<SoulofBlight>(10)
                .AddIngredient(ItemID.TitaniumBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
