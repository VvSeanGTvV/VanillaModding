using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)]
    internal class TropicalFlower : ModItem
    {
        public static readonly int MultiplicativeDelayDecrease = 12;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MultiplicativeDelayDecrease);
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 4, 30, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.manaCost -= (MultiplicativeDelayDecrease / 100f);
            player.PotionDelayModifier *= 1f - (MultiplicativeDelayDecrease / 100f);
            if (player.statLife <= player.statLifeMax2 * 0.25f && player.potionDelay <= 0 && !player.HasBuff(BuffID.PotionSickness))
            {
                player.AddBuff(BuffID.Honey, 60 * 30);
                player.QuickHeal();
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<RestorationFlower>(), 1)
                .AddIngredient(ItemID.LifeFruit, 1)
                .AddIngredient(ItemID.BottledHoney, 2)
                .AddIngredient(ItemID.JungleSpores, 5)
                .AddIngredient(ItemID.Vine, 2)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
