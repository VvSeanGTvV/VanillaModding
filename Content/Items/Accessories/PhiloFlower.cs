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

namespace VanillaModding.Content.Items.Accessories
{
    internal class PhiloFlower : ModItem
    {
        public static readonly int MultiplicativeDelayDecrease = 25;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(0, MultiplicativeDelayDecrease);
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if (equippedItem.type == ModContent.ItemType<HealingFlower>()) return false;
            if (equippedItem.type == ItemID.PhilosophersStone) return false;
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.PotionDelayModifier *= 1 - MultiplicativeDelayDecrease / 100f;
            if (player.statLife <= player.statLifeMax2 * 0.25f && player.potionDelay <= 0 && !player.HasBuff(BuffID.PotionSickness))
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
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<HealingFlower>(), 1)
                .AddIngredient(ItemID.PhilosophersStone, 1)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
