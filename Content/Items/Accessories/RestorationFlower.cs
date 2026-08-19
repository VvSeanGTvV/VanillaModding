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
        public static readonly int MultiplicativeDelayDecrease = 8;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MultiplicativeDelayDecrease);
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.manaCost -= (MultiplicativeDelayDecrease / 100f);
            player.PotionDelayModifier *= 1f - (MultiplicativeDelayDecrease / 100f);
            if (player.statLife <= player.statLifeMax2 * 0.25f && player.potionDelay <= 0 && !player.HasBuff(BuffID.PotionSickness)) player.QuickHeal();
            /*if (player.statLife <= player.statLifeMax2 * 0.25f && player.potionDelay <= 0 && !player.HasBuff(BuffID.PotionSickness))
            {
                int highHeal = 0;
                Item highHealItem = null;
                for (int i = 0; i < 49; i++)
                {
                    Item item = player.inventory[i];
                    if (item.healLife > highHeal && item.stack > 0)
                    {
                        highHeal = item.healLife;
                        highHealItem = item;
                    }
                }
                if (highHeal > 0 && highHealItem != null)
                {
                    player.Heal(highHeal);
                    player.HealEffect(highHeal);

                    int potionDuration = (int)player.PotionDelayModifier.ApplyTo(player.potionDelayTime);
                    player.AddBuff(BuffID.PotionSickness, potionDuration);
                    player.potionDelay = potionDuration;

                    SoundEngine.PlaySound(SoundID.Item3, player.position);
                    highHealItem.stack--;
                    if (highHealItem.stack <= 0) highHealItem.TurnToAir();
                }
            }*/
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<HealingFlower>(), 1)
                .AddIngredient(ItemID.ManaFlower, 1)
                .AddIngredient(ItemID.PinkGel, 1)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe(1)
                .AddIngredient(ItemID.NaturesGift, 1)
                .AddIngredient(ItemID.FlowerofFire, 1)
                .AddIngredient(ItemID.RestorationPotion, 1)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
