using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common;

namespace VanillaModding.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)]
    internal class RestorationFlower : ModItem
    {
        public static readonly int MultiplicativeDelayDecrease = 10;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MultiplicativeDelayDecrease);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 3, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.manaCost -= (MultiplicativeDelayDecrease / 100f);
            player.PotionDelayModifier *= 1f - (MultiplicativeDelayDecrease / 100f);
            if (player.statLife <= player.statLifeMax2 * 0.25f && player.potionDelay <= 0 && !player.HasBuff(BuffID.PotionSickness)) player.QuickHeal();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<HealingFlower>(), 1)
                .AddIngredient(ItemID.ManaFlower, 1)
                .AddIngredient(ItemID.PinkGel, 1)
                .AddIngredient(ItemID.CrystalShard, 2)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe(1)
                .AddIngredient(ItemID.FlowerofFire, 1)
                .AddIngredient(ItemID.NaturesGift, 1)
                .AddIngredient(ItemID.RestorationPotion, 1)
                .AddIngredient(ItemID.CrystalShard, 2)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
