using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;

namespace VanillaModding.Content.Items.Accessories
{
    internal class HealingFlower : ModItem
    {
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

                    player.AddBuff(BuffID.PotionSickness, 40 * 60);
                    player.potionDelay = 40 * 60;

                    SoundEngine.PlaySound(SoundID.Item3, player.position);
                    highHealItem.stack--;
                    if (highHealItem.stack <= 0) highHealItem.TurnToAir();
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.FlowerofFire, 1)
                .AddIngredient(ItemID.HealingPotion, 1)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
